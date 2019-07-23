using DataModel;
using System;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.Linq;
using LinqToDB;

#region - ClientConnectionPool - 

public class ClientConnectionPool
{
    Queue SyncdQ = Queue.Synchronized(new Queue());

    public void Enqueue(ClientHandler client)
    {
        SyncdQ.Enqueue(client);
    }

    public ClientHandler Dequeue()
    {
        return (ClientHandler)(SyncdQ.Dequeue());
    }

    public int Count
    {
        get { return SyncdQ.Count; }
    }

    public object SyncRoot
    {
        get { return SyncdQ.SyncRoot; }
    }
}

#endregion

#region - ClientService - 

public class ClientService
{
    const int NUM_OF_THREAD = 150;

    ClientConnectionPool ConnectionPool;
    Thread[] ThreadTask = new Thread[NUM_OF_THREAD];

    bool ContinueProcess = false;

    public ClientService(ClientConnectionPool ConnectionPool)
    {
        this.ConnectionPool = ConnectionPool;
    }

    public void Start()
    {
        ContinueProcess = true;

        // Start threads to handle Client Task
        for (int i = 0; i < ThreadTask.Length; i++)
        {
            ThreadTask[i] = new Thread(new ThreadStart(this.Process));
            ThreadTask[i].Start();
        }
    }

    void Process()
    {
        while (ContinueProcess)
        {
            ClientHandler client = null;

            lock (ConnectionPool.SyncRoot)
            {
                if (ConnectionPool.Count > 0)
                    client = ConnectionPool.Dequeue();
            }

            if (client != null)
            {
                client.Process();

                if (client.Alive)
                    ConnectionPool.Enqueue(client);
            }

            Thread.Sleep(100);
        }
    }

    public void Stop()
    {
        ContinueProcess = false;

        for (int i = 0; i < ThreadTask.Length; i++)
        {
            if (ThreadTask[i] != null && ThreadTask[i].IsAlive)
                ThreadTask[i].Join();
        }

        while (ConnectionPool.Count > 0)
        {
            ClientHandler client = ConnectionPool.Dequeue();
            client.Close();
            Console.WriteLine("Client connection is closed!");
        }
    }
}

#endregion

public class SynchronousSocketListener
{
    public static int portNum = 2700;   // software express

    public static int Main(String[] args)
    {
        Console.WriteLine("\n" + DateTime.Now + "] 1.00001");
        Console.WriteLine("\nCNET ISO [" + portNum + "]");

        new Thread(new ThreadStart(BatchService_Fechamento)).Start();
        new Thread(new ThreadStart(BatchService_ConfirmacaoAuto)).Start();

        using (var db = new AutorizadorCNDB())
        {
            var term = db.T_Cartao.FirstOrDefault();
            Console.WriteLine("DBCHECK -> OK!");
        }

        StartListening();

        Console.WriteLine("\nHit enter to continue...");
        Console.Read();

        return 0;
    }

    protected static void BatchService_Fechamento()
    {
        Console.WriteLine("BatchService_Fechamento...");

        while (true)
        {
            using (var db = new AutorizadorCNDB())
            {
                string currentEmpresa = "";

                try
                {
                    var dt = DateTime.Now;

                    var diaFechamento = dt.Day;
                    var horaAtual = dt.ToString("HHmm");
                    var ano = dt.ToString("yyyy");
                    var mes = dt.ToString("MM").PadLeft(2, '0');

                    var lstEmpresas = db.T_Empresa.Where(y => y.nu_diaFech == diaFechamento && y.st_horaFech == horaAtual).ToList();

                    foreach (var empresa in lstEmpresas)
                    {
                        // ------------------------------
                        // só fecha uma vez no mes
                        // ------------------------------

                        if (db.LOG_Fechamento.Any(y => y.st_ano == ano &&
                                                        y.st_mes == mes &&
                                                        y.fk_empresa == empresa.i_unique))
                            continue;

                        currentEmpresa = empresa.st_empresa;

                        db.Insert(new LOG_Audit
                        {
                            dt_operacao = DateTime.Now,
                            fk_usuario = null,
                            st_oper = "Fechamento [INICIO]",
                            st_empresa = currentEmpresa,
                            st_log = "Ano " + ano + " Mes " + mes
                        });

                        var g_job = new T_JobFechamento
                        {
                            dt_inicio = DateTime.Now,
                            dt_fim = null,
                            fk_empresa = (int)empresa.i_unique,
                            st_ano = ano,
                            st_mes = mes
                        };

                        // ----------------------------
                        // registra job
                        // ----------------------------

                        g_job.i_unique = Convert.ToInt32(db.InsertWithIdentity(g_job));

                        // ----------------------------
                        // busca parcelas
                        // ----------------------------

                        long totValor = 0;

                        foreach (var parc in db.T_Parcelas.Where(y => y.fk_empresa == empresa.i_unique && y.nu_parcela > 0).ToList())
                        {
                            // ----------------------------
                            // somente confirmadas
                            // ----------------------------

                            var logTrans = db.LOG_Transacoes.FirstOrDefault(y => y.i_unique == parc.fk_log_transacoes);

                            if (logTrans == null)
                                continue;
                            else
                            {
                                if (logTrans.tg_confirmada == null)
                                    continue;

                                if (logTrans.tg_confirmada.ToString() != TipoConfirmacao.Confirmada)
                                    continue;
                            }

                            // ----------------------------
                            // decrementa parcela
                            // ----------------------------

                            var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == parc.i_unique);
                            parcUpd.nu_parcela--;
                            db.Update(parcUpd);

                            // -------------------------------------------
                            // insere fechamento quando parcela zerar 
                            // -------------------------------------------

                            if (parcUpd.nu_parcela == 0)
                            {
                                totValor += (int)parc.vr_valor;

                                db.Insert(new LOG_Fechamento
                                {
                                    dt_compra = logTrans.dt_transacao,
                                    dt_fechamento = DateTime.Now,
                                    fk_cartao = parc.fk_cartao,
                                    fk_empresa = parc.fk_empresa,
                                    fk_loja = parc.fk_loja,
                                    fk_parcela = (int)parc.i_unique,
                                    nu_parcela = parc.nu_parcela,
                                    st_afiliada = "",
                                    st_ano = ano,
                                    st_mes = mes,
                                    vr_valor = parc.vr_valor
                                });
                            }
                        }

                        // ----------------------------
                        // registra job / finalizado!
                        // ----------------------------

                        g_job.dt_fim = DateTime.Now;

                        db.Update(g_job);

                        db.Insert(new LOG_Audit
                        {
                            dt_operacao = DateTime.Now,
                            fk_usuario = null,
                            st_oper = "Fechamento [OK]",
                            st_empresa = currentEmpresa,
                            st_log = "Ano " + ano + " Mes " + mes + " Valor => " + totValor
                        });
                    }
                }
                catch (SystemException ex)
                {
                    db.Insert(new LOG_Audit
                    {
                        dt_operacao = DateTime.Now,
                        fk_usuario = null,
                        st_oper = "Fechamento [ERRO]",
                        st_empresa = currentEmpresa,
                        st_log = ex.ToString()
                    });
                }
            }

            Thread.Sleep(1000 * 60);
        }
    }

    protected static void BatchService_ConfirmacaoAuto()
    {
        Console.WriteLine("BatchService_ConfirmacaoAuto...");

        while (true)
        {
            using (var db = new AutorizadorCNDB())
            {
                var dtNow = DateTime.Now;

                var dtIni = dtNow.AddSeconds(-60 * 6);
                var dtFim = dtNow.AddDays(-2);

                var queryX = db.LOG_Transacoes.
                                Where(y => y.dt_transacao > dtFim && y.dt_transacao < dtIni &&
                                           y.tg_confirmada.ToString() == TipoConfirmacao.Pendente &&
                                           y.tg_contabil.ToString() == TipoCaptura.SITEF).
                                ToList();

                foreach (var item in queryX)
                {
                    item.tg_confirmada = Convert.ToChar(TipoConfirmacao.Confirmada);
                    item.st_msg_transacao = "Conf. Auto";

                    db.Update(item);
                }
            }

            Thread.Sleep(5000);
        }
    }

    #region - code - 

    public static void StartListening()
    {
        var ConnectionPool = new ClientConnectionPool();
        var ClientTask = new ClientService(ConnectionPool);

        ClientTask.Start();

        var listener = new TcpListener(portNum);

        try
        {
            listener.Start();

            int ClientNbr = 0;

            // Start listening for connections.
            Console.WriteLine("Waiting for a connection...");

            while (true)
            {
                TcpClient handler = listener.AcceptTcpClient();

                if (handler != null)
                {
                    ++ClientNbr;
                    ConnectionPool.Enqueue(new ClientHandler(handler, ClientNbr));
                }

                Thread.Sleep(100);
            }

            //listener.Stop();
            //ClientTask.Stop();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

#endregion
}
