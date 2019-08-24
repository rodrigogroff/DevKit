using System;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using RestSharp;

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

    public static string localHost = "http://meuconvey.conveynet.com.br";
    //public static string localHost = "http://localhost:4091";

    public static int Main()
    {
        Console.WriteLine("\n" + DateTime.Now + "] 1.00002");
        Console.WriteLine("\nCNET ISO [" + portNum + "]");

        new Thread(new ThreadStart(BatchService_Fechamento)).Start();
        new Thread(new ThreadStart(BatchService_ConfirmacaoAuto)).Start();

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
            var serviceClient = new RestClient(localHost);
            var serviceRequest = new RestRequest("api/FechamentoServerISO", Method.GET);

            serviceClient.Execute(serviceRequest);

            Thread.Sleep(1000 * 60);
        }
    }

    protected static void BatchService_ConfirmacaoAuto()
    {
        Console.WriteLine("BatchService_ConfirmacaoAuto...");

        while (true)
        {
            var serviceClient = new RestClient(localHost);
            var serviceRequest = new RestRequest("api/ConfirmacaoAutoServerISO", Method.GET);

            serviceClient.Execute(serviceRequest);

            Thread.Sleep(5000);
        }
    }

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
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
