using DataModel;
using LinqToDB;
using System;
using System.Linq;
using Npgsql;

namespace GetStarted
{
    #region - tabelas -

    #region - done - 
    public class Cartao
    {
        #region - code - 

        public long id { get; set; }
        public long? fkEmpresa { get; set; }
        public long? nuMatricula { get; set; }
        public long? nuTitularidade { get; set; }
        public string stSenha { get; set; }
        public long? nuTipoCartao { get; set; }
        public string stVenctoCartao { get; set; }
        public long? nuStatus { get; set; }
        public long? nuSenhaErrada { get; set; }
        public DateTime? dtInclusao { get; set; }
        public DateTime? dtBloqueio { get; set; }
        public long? nuMotivoBloqueio { get; set; }
        public string stBanco { get; set; }
        public string stAgencia { get; set; }
        public string stConta { get; set; }
        public string stMatExtra { get; set; }
        public string stCelCartao { get; set; }
        public string stCpf { get; set; }
        public string stNome { get; set; }
        public string stEndereco { get; set; }
        public string stNumero { get; set; }
        public string stCompl { get; set; }
        public string stBairro { get; set; }
        public string stEstado { get; set; }
        public string stCidade { get; set; }
        public string stCEP { get; set; }
        public string stDDD { get; set; }
        public string stTelefone { get; set; }
        public DateTime? dtNasc { get; set; }
        public string stEmail { get; set; }
        public long? vrRenda { get; set; }
        public long? nuViaCartao { get; set; }
        public long? vrLimiteTotal  { get; set; }
        public long? vrLimiteMensal { get; set; }
        public long? vrLimiteRotativo { get; set; }
        public long? vrCotaExtra { get; set; }
        public long? nuEmitido { get; set; }
        public bool? bConvenioComSaldo { get; set; }
        public long? vrSaldoConvenio { get; set; }
        public DateTime? dtPedidoCartao { get; set; }
        public string stCodigoFOPA { get; set; }        

        #endregion
    }

    public class ConfigPlasticoEnvio
    {
        #region - code - 
        public long id { get; set; }
        public string stDias { get; set; }
        public string stHorario { get; set; }
        public string stEmails { get; set; }
        public bool? bAtivo { get; set; }
        public bool? dom { get; set; }
        public bool? seg { get; set; }
        public bool? ter { get; set; }
        public bool? qua { get; set; }
        public bool? qui { get; set; }
        public bool? sex { get; set; }
        public bool? sab { get; set; }
        public string stEmailSmtp { get; set; }
        public string stSenhaSmtp { get; set; }
        public string stHostSmtp { get; set; }
        public long? nuPortSmtp { get; set; }
        public string stStatus { get; set; }
        #endregion
    }

    public class DashboardGrafico
    {
        #region - code - 
        public long id { get; set; }
        public long? nuTotalTransacoes { get; set; }
        public long? nuTotalCartoes { get; set; }
        public long? nuTotalFinanc { get; set; }
        public long? nuDia { get; set; }
        public long? nuMes { get; set; }
        public long? nuAno { get; set; }
        public long? nuTotalLojas { get; set; }
        public DateTime? dtDia { get; set; }
        #endregion
    }

    public class Empresa
    {
        #region - code - 
        public long id { get; set; }
        public long? nuEmpresa { get; set; }
        public string stCNPJ { get; set; }
        public string stFantasia { get; set; }
        public string stSocial { get; set; }
        public string stEndereco { get; set; }
        public string stCidade { get; set; }
        public string stEstado { get; set; }
        public string stCEP { get; set; }
        public string stTelefone { get; set; }
        public long? nuParcelas { get; set; }
        public bool? bBlocked { get; set; }
        public long? fkAdmin { get; set; }
        public string stContaDeb { get; set; }
        public long? vrMensalidade { get; set; }
        public long? nuPctValor { get; set; }
        public long? vrTransacao { get; set; }
        public long? vrMinimo { get; set; }
        public long? nuFranquiaTrans { get; set; }
        public long? nuPeriodoFat { get; set; }
        public long? nuDiaVenc { get; set; }
        public string stBancoFat { get; set; }
        public long? vrCartaoAtivo { get; set; }
        public bool? bIsentoFat { get; set; }
        public string stObs { get; set; }
        public string stHomepage { get; set; }
        public long? nuDiaFech { get; set; }
        public string stHoraFech { get; set; }
        public bool? bConvenioSaldo { get; set; }
        public long? fkParceiro { get; set; }
        public string stEmailPlastico { get; set; }
        public bool? bContaCorrenteAssociado { get; set; }

        #endregion
    }

    public class EmpresaDespesa
    {
        #region - code - 
        public long id { get; set; }
        public long? fkEmpresa { get; set; }
        public string stCodigo { get; set; }
        public string stDescricao { get; set; }
        #endregion
    }

    public class Faturamento
    {
        #region - code - 
        public long id { get; set; }
        public long? fkEmpresa { get; set; }
        public long? fkLoja { get; set; }
        public long? vrCobranca { get; set; }
        public DateTime? dtVencimento { get; set; }
        public DateTime? dtBaixa { get; set; }
        public long? nuSituacao { get; set; }
        public long? nuRetBanco { get; set; }
        #endregion
    }

    public class FaturamentoDetalhe
    {
        #region - code - 
        public long id { get; set; }
        public long? fkFatura { get; set; }
        public long? nuTipoFat { get; set; }
        public long? nuQuantidade { get; set; }
        public long? vrCobranca { get; set; }
        public bool? bDesconto { get; set; }
        public string stExtras { get; set; }
        public long? fkEmpresa { get; set; }
        public long? fkLoja { get; set; }
        #endregion
    }

    public class JobFechamento
    {
        #region - code - 
        public long id { get; set; }
        public long? fkEmpresa { get; set; }
        public DateTime? dtInicio { get; set; }
        public DateTime? dtFim { get; set; }
        public long? nuMes { get; set; }
        public long? nuAno { get; set; }
        #endregion
    }

    public class LogAudit
    {
        #region - code - 
        public long id { get; set; }
        public long? fkUsuario { get; set; }
        public long? nuOperacao { get; set; }
        public DateTime? dtOperacao { get; set; }
        public long? fkGeneric { get; set; }
        public long? fkEmpresa { get; set; }
        public string stOperacao { get; set; }
        public string stLog { get; set; }
        #endregion
    }

    public class LogFechamento
    {
        #region - code - 
        public long id { get; set; }
        public long? nuMes { get; set; }
        public long? nuAno { get; set; }
        public long? vrValor { get; set; }
        public DateTime? dtFechamento { get; set; }
        public long? fkEmpresa { get; set; }
        public long? fkLoja { get; set; }
        public long? fkCartao { get; set; }
        public long? fkParcela { get; set; }
        public DateTime? dtCompra { get; set; }
        public long? nuParcela { get; set; }
        public string stCartao { get; set; }
        #endregion
    }

    public class LogNsu
    {
        #region - code - 
        public long id { get; set; }
        public DateTime? dtLog { get; set; }        
        #endregion
    }

    public class LogTransacao
    {
        #region - code - 
        public long id { get; set; }
        public long? fkTerminal { get; set; }
        public DateTime? dtTransacao { get; set; }
        public long? nuNsu { get; set; }
        public long? fkEmpresa { get; set; }
        public long? fkCartao { get; set; }
        public long? vrTotal { get; set; }
        public long? nuParcelas { get; set; }
        public long? nuCodErro { get; set; }
        public long? nuConfirmada { get; set; }
        public long? nuNsuOrig { get; set; }
        public long? nuOperacao { get; set; }
        public string stMsg { get; set; }
        public bool? bContabil { get; set; }
        public long? fkLoja { get; set; }
        public string stDoc { get; set; }
        #endregion
    }

    public class Loja
    {
        #region - code - 
        public long id { get; set; }
        public string stCNPJ { get; set; }
        public string stNome { get; set; }
        public string stSocial { get; set; }
        public string stEndereco { get; set; }
        public string stEnderecoInst { get; set; }
        public string stInscEst { get; set; }
        public string stCidade { get; set; }
        public string stEstado { get; set; }
        public string stCEP { get; set; }
        public string stTelefone { get; set; }
        public string stFax { get; set; }
        public string stContato { get; set; }
        public long? vrMensalidade { get; set; }
        public string stContaDeb { get; set; }
        public string stObs { get; set; }
        public string stLoja { get; set; }
        public bool? bBlocked { get; set; }
        public long? nuPctValor { get; set; }
        public long? vrTransacao { get; set; }
        public long? vrMinimo { get; set; }
        public long? nuFranquia { get; set; }
        public long? nuPeriodoFat { get; set; }
        public long? nuDiaVenc { get; set; }
        public long? nuTipoCob { get; set; }
        public long? nuBancoFat { get; set; }
        public bool? bIsentoFat { get; set; }
        public string stSenha { get; set; }
        public bool? bCancel { get; set; }
        public bool? bPortalSenha { get; set; }
        public string stEmail { get; set; }
        public string stCelular { get; set; }
        public string stBanco { get; set; }
        public string stAgencia { get; set; }
        public string stConta { get; set; }
        public long? fkBanco { get; set; }
        public string stCPFResp { get; set; }
        public string stDataResp { get; set; }
        #endregion
    }

    public class LojaEmpresa
    {
        #region - code - 
        public long id { get; set; }
        public long? fkLoja { get; set; }
        public long? fkEmpresa { get; set; }
        public long? nuTxAdmin { get; set; }
        public long? nuDiasRepasse { get; set; }
        public string stAgencia { get; set; }
        public string stConta { get; set; }
        public string stBanco { get; set; }
        #endregion
    }

    public class LojaMsg
    {
        #region - code - 
        public long id { get; set; }
        public long? fkLoja { get; set; }
        public string stMsg { get; set; }
        public string stLink { get; set; }
        public DateTime? dtValidade { get; set; }
        public DateTime? dtCriacao { get; set; }
        public bool? bAtiva { get; set; }        
        #endregion
    }

    public class LoteCartao
    {
        #region - code - 
        public long id { get; set; }
        public long? nuCartoes { get; set; }
        public long? fkEmpresa { get; set; }
        public long? nuSitLote { get; set; }
        public DateTime? dtAbertura { get; set; }
        public DateTime? dtEnvioGrafica { get; set; }
        public DateTime? dtAtivacao { get; set; }
        #endregion
    }

    public class LoteCartaoDetalhe
    {
        #region - code - 
        public long id { get; set; }
        public long? fkLote { get; set; }
        public long? fkEmpresa { get; set; }
        public long? fkCartao { get; set; }
        public long? nuMatricula { get; set; }
        public long? nuViaOriginal { get; set; }
        public long? nuTitularidade { get; set; }
        public string stCPF { get; set; }
        public string stNomeCartao { get; set; }
        public DateTime? dtAtivacao { get; set; }
        public DateTime? dtPedido { get; set; }
        #endregion
    }

    public class Parceiro
    {
        #region - code - 
        public long id { get; set; }
        public string stNome { get; set; }
        public DateTime? dtCadastro { get; set; }
        public string stTelefone { get; set; }
        public string stCelular { get; set; }
        public string stResp { get; set; }
        public string stSocial { get; set; }
        public string stCNPJ { get; set; }
        public string stEstado { get; set; }
        public string stCidade { get; set; }
        public string stEndereco { get; set; }
        public string stObs { get; set; }
        public string stCEP { get; set; }
        #endregion
    }

    public class Parcela
    {
        #region - code - 
        public long id { get; set; }
        public long? nuNsu { get; set; }
        public long? fkEmpresa { get; set; }
        public long? fkCartao { get; set; }
        public DateTime? dtInclusao { get; set; }
        public long? nuParcela { get; set; }
        public long? vrValor { get; set; }
        public long? nuIndice { get; set; }
        public long? fkLoja { get; set; }
        public long? nuTotParcelas { get; set; }
        public long? fkTerminal { get; set; }
        public long? fkLogTransacao { get; set; }
        #endregion
    }

    public class SaldoConvenio
    {
        #region - code - 
        public long id { get; set; }
        public long? fkCartao { get; set; }
        public long? vrSaldo { get; set; }
        public long? nuMes { get; set; }
        public long? nuAno { get; set; }
        #endregion
    }

    public class SolicitacaoVenda
    {
        #region - code - 
        public long id { get; set; }
        public long? fkCartao { get; set; }
        public long? fkLoja { get; set; }
        public long? vrValor { get; set; }
        public long? nuParcelas { get; set; }
        public bool? bAberto { get; set; }
        public long? dtSolic { get; set; }
        public long? fkTerminal { get; set; }
        public long? fkLogTrans { get; set; }
        public string stErro { get; set; }
        public string stParcelas { get; set; }
        #endregion
    }

    public class Terminal
    {
        #region - code - 
        public long id { get; set; }
        public long? fkLoja { get; set; }
        public string stTerminal { get; set; }
        public string stLocal { get; set; }
        #endregion
    }

    public class UsuarioEmissor
    {
        #region - code - 
        public long id { get; set; }
        public long? nuNivel { get; set; }
        public DateTime? dtTrocaSenha { get; set; }
        public DateTime? dtUltUso { get; set; }
        public long? nuSenhaErrada { get; set; }
        public bool? bTrocaSenha { get; set; }
        public long? fkEmpresa { get; set; }
        public string stSenha { get; set; }
        public bool? bBloqueio { get; set; }
        public string stNome { get; set; }
        public bool? bOperador { get; set; }
        public bool? bAviso { get; set; }
        #endregion
    }

    public class UsuarioParceiro
    {
        #region - code - 
        public long id { get; set; }
        public string stEmail { get; set; }
        public string stSenha { get; set; }
        public long? nuTipo { get; set; }
        public long? fkParceiro { get; set; }
        public bool? bAtivo { get; set; }
        public string stNome { get; set; }
        public DateTime? dtCadastro { get; set; }
        public DateTime? dtUltLogin { get; set; }
        #endregion
    }

    #endregion

    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now);

            var connStr = "User ID=postgres;Password=Gustavo123;Host=localhost;Port=5432;Database=Conveynet;";

            if (false)
            {

                #region - Cartao -             
                {
                    long max = 9999999999;
                    using (var db = new AutorizadorCNDB())
                        max = (long)db.T_Cartao.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                    for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                    {
                        using (var db = new AutorizadorCNDB())
                        using (var dbPg = new NpgsqlConnection(connStr))
                        {
                            var lst = db.T_Cartao.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                            if (lst.Count() == 0)
                                continue;

                            dbPg.Open();

                            foreach (var cart in lst)
                            {
                                var prop = db.T_Proprietario.FirstOrDefault(y => y.i_unique == cart.fk_dadosProprietario);

                                if (prop == null)
                                    continue;

                                string cpf = "", nome = "";

                                if (cart.st_titularidade == null)
                                    cart.st_titularidade = "1";

                                if (Convert.ToInt32(cart.st_titularidade) == 1)
                                {
                                    cpf = prop.st_cpf;
                                    nome = prop.st_nome;
                                }
                                else
                                {
                                    var dep = db.T_Dependente.FirstOrDefault(y => y.fk_proprietario == cart.fk_dadosProprietario);
                                    nome = dep.st_nome;
                                }

                                if (cart.tg_motivoBloqueio == '\0' || cart.tg_motivoBloqueio == null)
                                    cart.tg_motivoBloqueio = '0';

                                if (cart.tg_status == null)
                                    cart.tg_status = '0';

                                if (cart.tg_emitido == null)
                                    cart.tg_emitido = '0';

                                cart.st_matricula = cart.st_matricula.Replace(" ", "").TrimStart('0');

                                var novoCart = new Cartao
                                {
                                    id = Convert.ToInt64(cart.i_unique),
                                    fkEmpresa = Convert.ToInt64(db.T_Empresa.Where(y => y.st_empresa == cart.st_empresa).FirstOrDefault().i_unique),
                                    nuMatricula = Convert.ToInt64(cart.st_matricula),
                                    nuTitularidade = Convert.ToInt64(cart.st_titularidade),
                                    stSenha = cart.st_senha,
                                    nuTipoCartao = Convert.ToInt64(cart.tg_tipoCartao.ToString()),
                                    stVenctoCartao = cart.st_venctoCartao,
                                    nuStatus = Convert.ToInt64(cart.tg_status.ToString()),
                                    nuSenhaErrada = cart.nu_senhaErrada,
                                    dtInclusao = cart.dt_inclusao,
                                    dtBloqueio = cart.dt_bloqueio,
                                    nuMotivoBloqueio = Convert.ToInt64(cart.tg_motivoBloqueio.ToString()),
                                    stBanco = cart.st_banco,
                                    stAgencia = cart.st_agencia,
                                    stConta = cart.st_conta,
                                    stMatExtra = cart.st_matriculaExtra,
                                    stCelCartao = cart.st_celCartao,
                                    stCpf = cpf,
                                    stNome = nome,
                                    stEndereco = prop.st_endereco,
                                    stNumero = prop.st_numero,
                                    stCompl = prop.st_complemento,
                                    stBairro = prop.st_bairro,
                                    stEstado = prop.st_UF,
                                    stCidade = prop.st_cidade,
                                    stCEP = prop.st_cep,
                                    stDDD = prop.st_ddd,
                                    stTelefone = prop.st_telefone,
                                    dtNasc = prop.dt_nasc,
                                    stEmail = prop.st_email,
                                    vrRenda = prop.vr_renda,
                                    nuViaCartao = cart.nu_viaCartao,
                                    vrLimiteTotal = cart.vr_limiteTotal,
                                    vrLimiteMensal = cart.vr_limiteMensal,
                                    vrLimiteRotativo = cart.vr_limiteRotativo,
                                    vrCotaExtra = cart.vr_extraCota,
                                    nuEmitido = Convert.ToInt64(cart.tg_emitido.ToString()),
                                    bConvenioComSaldo = cart.tg_convenioComSaldo,
                                    vrSaldoConvenio = cart.vr_saldoConvenio,
                                    dtPedidoCartao = cart.dtPedidoCartao,
                                    stCodigoFOPA = cart.stCodigoFOPA,
                                };

                                using (var cmd = new NpgsqlCommand("INSERT INTO \"Cartao\" (\"id\",\"fkEmpresa\",\"nuMatricula\",\"nuTitularidade\",\"stSenha\",\"nuTipoCartao\"," +
                                                                    "\"stVenctoCartao\",\"nuStatus\",\"nuSenhaErrada\",\"dtInclusao\",\"dtBloqueio\",\"nuMotivoBloqueio\",\"stBanco\"," +
                                                                    "\"stAgencia\",\"stConta\",\"stMatExtra\",\"stCelCartao\",\"stCpf\",\"stNome\",\"stEndereco\",\"stNumero\",\"stCompl\"," +
                                                                    "\"stBairro\",\"stEstado\",\"stCidade\",\"stCEP\",\"stDDD\",\"stTelefone\",\"dtNasc\",\"stEmail\",\"vrRenda\",\"nuViaCartao\"," +
                                                                    "\"vrLimiteTotal\",\"vrLimiteMensal\",\"vrLimiteRotativo\",\"vrCotaExtra\",\"nuEmitido\",\"bConvenioComSaldo\",\"vrSaldoConvenio\"," +
                                                                    "\"dtPedidoCartao\",\"stCodigoFOPA\") " +
                                                                    "VALUES ( @id,@fkEmpresa,@nuMatricula,@nuTitularidade,@stSenha,@nuTipoCartao,@stVenctoCartao,@nuStatus,@nuSenhaErrada," +
                                                                    "@dtInclusao,@dtBloqueio,@nuMotivoBloqueio,@stBanco,@stAgencia,@stConta,@stMatExtra,@stCelCartao," +
                                                                    "@stCpf,@stNome,@stEndereco,@stNumero,@stCompl,@stBairro,@stEstado,@stCidade,@stCEP,@stDDD,@stTelefone,@dtNasc,@stEmail,@vrRenda," +
                                                                    "@nuViaCartao,@vrLimiteTotal,@vrLimiteMensal,@vrLimiteRotativo,@vrCotaExtra,@nuEmitido,@bConvenioComSaldo,@vrSaldoConvenio,@dtPedidoCartao,@stCodigoFOPA);", dbPg))
                                {
                                    cmd.Parameters.AddWithValue("id", ((object)novoCart.id) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkEmpresa", ((object)novoCart.fkEmpresa) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuMatricula", ((object)novoCart.nuMatricula) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuTitularidade", ((object)novoCart.nuTitularidade) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stSenha", ((object)novoCart.stSenha) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuTipoCartao", ((object)novoCart.nuTipoCartao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stVenctoCartao", ((object)novoCart.stVenctoCartao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuStatus", ((object)novoCart.nuStatus) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuSenhaErrada", ((object)novoCart.nuSenhaErrada) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtInclusao", ((object)novoCart.dtInclusao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtBloqueio", ((object)novoCart.dtBloqueio) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuMotivoBloqueio", ((object)novoCart.nuMotivoBloqueio) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stBanco", ((object)novoCart.stBanco) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stAgencia", ((object)novoCart.stAgencia) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stConta", ((object)novoCart.stConta) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stMatExtra", ((object)novoCart.stMatExtra) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stCelCartao", ((object)novoCart.stCelCartao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stCpf", ((object)novoCart.stCpf) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stNome", ((object)novoCart.stNome) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stEndereco", ((object)novoCart.stEndereco) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stNumero", ((object)novoCart.stNumero) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stCompl", ((object)novoCart.stCompl) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stBairro", ((object)novoCart.stBairro) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stEstado", ((object)novoCart.stEstado) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stCidade", ((object)novoCart.stCidade) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stCEP", ((object)novoCart.stCEP) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stDDD", ((object)novoCart.stDDD) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stTelefone", ((object)novoCart.stTelefone) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtNasc", ((object)novoCart.dtNasc) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stEmail", ((object)novoCart.stEmail) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrRenda", ((object)novoCart.vrRenda) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuViaCartao", ((object)novoCart.nuViaCartao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrLimiteTotal", ((object)novoCart.vrLimiteTotal) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrLimiteMensal", ((object)novoCart.vrLimiteMensal) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrLimiteRotativo", ((object)novoCart.vrLimiteRotativo) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrCotaExtra", ((object)novoCart.vrCotaExtra) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuEmitido", ((object)novoCart.nuEmitido) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("bConvenioComSaldo", ((object)novoCart.bConvenioComSaldo) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrSaldoConvenio", ((object)novoCart.vrSaldoConvenio) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtPedidoCartao", ((object)novoCart.dtPedidoCartao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stCodigoFOPA", ((object)novoCart.stCodigoFOPA) ?? DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            dbPg.Close();
                        }
                    }
                }

                #endregion

                #region - ConfigPlasticoEnvio - 


                {
                    using (var db = new AutorizadorCNDB())
                    using (var dbPg = new NpgsqlConnection(connStr))
                    {
                        var lst = db.ConfigPlasticoEnvio.OrderBy(y => y.id).ToList();

                        if (lst.Count() > 0)
                        {
                            dbPg.Open();

                            foreach (var item in lst)
                            {
                                var novo = new ConfigPlasticoEnvio
                                {
                                    id = item.id,
                                    stDias = item.stDias,
                                    stHorario = item.stHorario,
                                    stEmails = item.stEmails,
                                    bAtivo = item.bAtivo,
                                    dom = item.dom,
                                    seg = item.seg,
                                    ter = item.ter,
                                    qua = item.qua,
                                    qui = item.qui,
                                    sex = item.sex,
                                    sab = item.sab,
                                    stEmailSmtp = item.stEmailSmtp,
                                    stSenhaSmtp = item.stSenhaSmtp,
                                    stHostSmtp = item.stHostSmtp,
                                    nuPortSmtp = item.nuPortSmtp,
                                    stStatus = item.stStatus,
                                };

                                using (var cmd = new NpgsqlCommand("INSERT INTO \"ConfigPlasticoEnvio\" (\"id\",\"stDias\",\"stHorario\",\"stEmails\",\"bAtivo\"," +
                                                                    "\"dom\",\"seg\",\"ter\",\"qua\",\"qui\",\"sex\",\"sab\",\"stEmailSmtp\",\"stSenhaSmtp\",\"stHostSmtp\",\"nuPortSmtp\",\"stStatus\" ) " +
                                                                    "VALUES ( @id,@stDias,@stHorario,@stEmails,@bAtivo," +
                                                                    "@dom,@seg,@ter,@qua,@qui,@sex,@sab,@stEmailSmtp,@stSenhaSmtp,@stHostSmtp,@nuPortSmtp,@stStatus );", dbPg))
                                {
                                    cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stDias", ((object)novo.stDias) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stHorario", ((object)novo.stHorario) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stEmails", ((object)novo.stEmails) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("bAtivo", ((object)novo.bAtivo) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dom", ((object)novo.dom) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("seg", ((object)novo.seg) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("ter", ((object)novo.ter) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("qua", ((object)novo.qua) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("qui", ((object)novo.qui) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("sex", ((object)novo.sex) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("sab", ((object)novo.sab) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stEmailSmtp", ((object)novo.stEmailSmtp) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stSenhaSmtp", ((object)novo.stSenhaSmtp) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stHostSmtp", ((object)novo.stHostSmtp) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuPortSmtp", ((object)novo.nuPortSmtp) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stStatus", ((object)novo.stStatus) ?? DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region - DashboardGrafico - 

                {
                    long max = 9999999999;
                    using (var db = new AutorizadorCNDB())
                        max = (long)db.DashboardGrafico.OrderByDescending(y => y.id).FirstOrDefault().id;
                    for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                    {
                        using (var db = new AutorizadorCNDB())
                        using (var dbPg = new NpgsqlConnection(connStr))
                        {
                            var lst = db.DashboardGrafico.Where(y => y.id >= g && y.id < t).OrderBy(y => y.id).ToList();

                            if (lst.Count() == 0)
                                continue;

                            dbPg.Open();

                            foreach (var item in lst)
                            {
                                var novo = new DashboardGrafico
                                {
                                    id = item.id,
                                    nuTotalTransacoes = item.totalTransacoes,
                                    nuTotalCartoes = item.totalCartoes,
                                    nuTotalFinanc = item.totalFinanc,
                                    nuDia = item.nuDia,
                                    nuMes = item.nuMes,
                                    nuAno = item.nuAno,
                                    nuTotalLojas = item.totalLojas,
                                    dtDia = item.dtDia,
                                };

                                using (var cmd = new NpgsqlCommand("INSERT INTO \"DashboardGrafico\" (\"id\",\"nuTotalTransacoes\",\"nuTotalCartoes\",\"nuTotalFinanc\"," +
                                                                    "\"nuDia\",\"nuMes\",\"nuAno\",\"nuTotalLojas\",\"dtDia\" ) " +
                                                                    "VALUES ( @id,@nuTotalTransacoes,@nuTotalCartoes,@nuTotalFinanc," +
                                                                    "@nuDia,@nuMes,@nuAno,@nuTotalLojas,@dtDia);", dbPg))
                                {
                                    cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuTotalTransacoes", ((object)novo.nuTotalTransacoes) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuTotalCartoes", ((object)novo.nuTotalCartoes) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuTotalFinanc", ((object)novo.nuTotalFinanc) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuDia", ((object)novo.nuDia) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuMes", ((object)novo.nuMes) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuAno", ((object)novo.nuAno) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuTotalLojas", ((object)novo.nuTotalLojas) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtDia", ((object)novo.dtDia) ?? DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region - Empresa - 

                {
                    long max = 9999999999;
                    using (var db = new AutorizadorCNDB())
                        max = (long)db.T_Empresa.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                    for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                    {
                        using (var db = new AutorizadorCNDB())
                        using (var dbPg = new NpgsqlConnection(connStr))
                        {
                            var lst = db.T_Empresa.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                            if (lst.Count() == 0)
                                continue;

                            dbPg.Open();

                            foreach (var item in lst)
                            {
                                var novo = new Empresa
                                {
                                    id = Convert.ToInt64(item.i_unique),
                                    nuEmpresa = Convert.ToInt64(item.st_empresa),
                                    stCNPJ = item.nu_CNPJ,
                                    stFantasia = item.st_fantasia,
                                    stSocial = item.st_social,
                                    stEndereco = item.st_endereco,
                                    stCidade = item.st_cidade,
                                    stEstado = item.st_estado,
                                    stCEP = item.nu_CEP.ToString(),
                                    stTelefone = item.nu_telefone,
                                    nuParcelas = Convert.ToInt64(item.nu_parcelas),
                                    bBlocked = item.tg_blocked.ToString() == "1" ? true : false,
                                    fkAdmin = item.fk_admin,
                                    stContaDeb = item.nu_contaDeb,
                                    vrMensalidade = item.vr_mensalidade,
                                    nuPctValor = item.nu_pctValor,
                                    vrTransacao = item.vr_transacao,
                                    vrMinimo = item.vr_minimo,
                                    nuFranquiaTrans = item.nu_franquia,
                                    nuPeriodoFat = item.nu_periodoFat,
                                    nuDiaVenc = item.nu_diaVenc,
                                    stBancoFat = item.nu_bancoFat.ToString(),
                                    vrCartaoAtivo = item.vr_cartaoAtivo,
                                    bIsentoFat = item.tg_isentoFat == 1 ? true : false,
                                    stObs = item.st_obs,
                                    stHomepage = item.st_homepage,
                                    nuDiaFech = item.nu_diaFech,
                                    stHoraFech = item.st_horaFech,
                                    bConvenioSaldo = item.tg_convenioComSaldo,
                                    fkParceiro = item.fkParceiro,
                                    stEmailPlastico = item.st_emailPlastico,
                                    bContaCorrenteAssociado = item.bContaCorrenteAssociado
                                };

                                using (var cmd = new NpgsqlCommand("INSERT INTO \"Empresa\" (\"id\",\"nuEmpresa\",\"stCNPJ\",\"stFantasia\",\"stSocial\",\"stEndereco\",\"stCidade\"," +
                                                                    "\"stEstado\",\"stCEP\",\"stTelefone\",\"nuParcelas\",\"bBlocked\",\"fkAdmin\",\"stContaDeb\",\"vrMensalidade\",\"nuPctValor\",\"vrTransacao\"," +
                                                                    "\"vrMinimo\",\"nuFranquiaTrans\",\"nuPeriodoFat\",\"nuDiaVenc\",\"stBancoFat\",\"vrCartaoAtivo\",\"bIsentoFat\",\"stObs\"," +
                                                                    "\"stHomepage\",\"nuDiaFech\",\"stHoraFech\",\"bConvenioSaldo\",\"fkParceiro\",\"stEmailPlastico\", \"bContaCorrenteAssociado\" ) " +
                                                                    "VALUES ( @id,@nuEmpresa,@stCNPJ,@stFantasia,@stSocial,@stEndereco,@stCidade," +
                                                                    "@stEstado,@stCEP,@stTelefone,@nuParcelas,@bBlocked,@fkAdmin,@stContaDeb,@vrMensalidade,@nuPctValor,@vrTransacao," +
                                                                    "@vrMinimo,@nuFranquiaTrans,@nuPeriodoFat,@nuDiaVenc,@stBancoFat,@vrCartaoAtivo,@bIsentoFat,@stObs," +
                                                                    "@stHomepage,@nuDiaFech,@stHoraFech,@bConvenioSaldo,@fkParceiro,@stEmailPlastico,@bContaCorrenteAssociado );", dbPg))
                                {
                                    cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuEmpresa", ((object)novo.nuEmpresa) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stCNPJ", ((object)novo.stCNPJ) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stFantasia", ((object)novo.stFantasia) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stSocial", ((object)novo.stSocial) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stEndereco", ((object)novo.stEndereco) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stCidade", ((object)novo.stCidade) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stEstado", ((object)novo.stEstado) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stCEP", ((object)novo.stCEP) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stTelefone", ((object)novo.stTelefone) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuParcelas", ((object)novo.nuParcelas) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("bBlocked", ((object)novo.bBlocked) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkAdmin", ((object)novo.fkAdmin) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stContaDeb", ((object)novo.stContaDeb) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrMensalidade", ((object)novo.vrMensalidade) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuPctValor", ((object)novo.nuPctValor) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrTransacao", ((object)novo.vrTransacao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrMinimo", ((object)novo.vrMinimo) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuFranquiaTrans", ((object)novo.nuFranquiaTrans) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuPeriodoFat", ((object)novo.nuPeriodoFat) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuDiaVenc", ((object)novo.nuDiaVenc) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stBancoFat", ((object)novo.stBancoFat) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrCartaoAtivo", ((object)novo.vrCartaoAtivo) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("bIsentoFat", ((object)novo.bIsentoFat) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stObs", ((object)novo.stObs) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stHomepage", ((object)novo.stHomepage) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuDiaFech", ((object)novo.nuDiaFech) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stHoraFech", ((object)novo.stHoraFech) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("bConvenioSaldo", ((object)novo.bConvenioSaldo) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkParceiro", ((object)novo.fkParceiro) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stEmailPlastico", ((object)novo.stEmailPlastico) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("bContaCorrenteAssociado", ((object)novo.bContaCorrenteAssociado) ?? DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }

                                var lstDespesas = db.EmpresaDespesa.Where(y => y.fkEmpresa == item.i_unique).ToList();

                                foreach (var desp in lstDespesas)
                                {
                                    var d = new EmpresaDespesa
                                    { 
                                        id = desp.id,
                                        fkEmpresa= desp.fkEmpresa,
                                        stCodigo = desp.stCodigo,
                                        stDescricao = desp.stDescricao
                                    };

                                    using (var cmd = new NpgsqlCommand("INSERT INTO \"EmpresaDespesa\" (\"id\",\"fkEmpresa\",\"stCodigo\",\"stDescricao\" ) " +
                                                                    "VALUES ( @id,@fkEmpresa,@stCodigo,@stDescricao );", dbPg))
                                    {
                                        cmd.Parameters.AddWithValue("id", ((object)d.id) ?? DBNull.Value);
                                        cmd.Parameters.AddWithValue("fkEmpresa", ((object)d.fkEmpresa) ?? DBNull.Value);
                                        cmd.Parameters.AddWithValue("stCodigo", ((object)d.stCodigo) ?? DBNull.Value);
                                        cmd.Parameters.AddWithValue("stDescricao", ((object)d.stDescricao) ?? DBNull.Value);                                        

                                        cmd.ExecuteNonQuery();
                                    }

                                }
                            }

                            

                        }
                    }
                }
                #endregion

                #region - Faturamento - 

                {
                    long max = 9999999999;
                    using (var db = new AutorizadorCNDB())
                        max = (long)db.T_Faturamento.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                    for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                    {
                        using (var db = new AutorizadorCNDB())
                        using (var dbPg = new NpgsqlConnection(connStr))
                        {
                            var lst = db.T_Faturamento.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                            if (lst.Count() == 0)
                                continue;

                            dbPg.Open();

                            foreach (var item in lst)
                            {
                                var novo = new Faturamento
                                {
                                    id = Convert.ToInt64(item.i_unique),
                                    fkEmpresa = item.fk_empresa,
                                    fkLoja = item.fk_loja,
                                    dtBaixa = item.dt_baixa,
                                    dtVencimento = item.dt_vencimento,
                                    nuRetBanco = Convert.ToInt64(item.tg_retBanco),
                                    nuSituacao = Convert.ToInt64(item.tg_situacao),
                                    vrCobranca = Convert.ToInt64(item.vr_cobranca),
                                };

                                using (var cmd = new NpgsqlCommand("INSERT INTO \"Faturamento\" (\"id\",\"fkEmpresa\",\"fkLoja\",\"dtBaixa\",\"dtVencimento\",\"nuRetBanco\",\"nuSituacao\",\"vrCobranca\" ) " +
                                                                    "VALUES ( @id,@fkEmpresa,@fkLoja,@dtBaixa,@dtVencimento,@nuRetBanco,@nuSituacao,@vrCobranca );", dbPg))
                                {
                                    cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkEmpresa", ((object)novo.fkEmpresa) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkLoja", ((object)novo.fkLoja) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtBaixa", ((object)novo.dtBaixa) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtVencimento", ((object)novo.dtVencimento) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuRetBanco", ((object)novo.nuRetBanco) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuSituacao", ((object)novo.nuSituacao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrCobranca", ((object)novo.vrCobranca) ?? DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }

                                break;
                            }
                        }
                    }
                }
                #endregion

                #region - FaturamentoDetalhe - 

                {
                    long max = 9999999999;
                    using (var db = new AutorizadorCNDB())
                        max = (long)db.T_FaturamentoDetalhes.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                    for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                    {
                        using (var db = new AutorizadorCNDB())
                        using (var dbPg = new NpgsqlConnection(connStr))
                        {
                            var lst = db.T_FaturamentoDetalhes.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                            if (lst.Count() == 0)
                                continue;

                            dbPg.Open();

                            foreach (var item in lst)
                            {
                                var novo = new FaturamentoDetalhe
                                {
                                    id = Convert.ToInt64(item.i_unique),
                                    bDesconto = item.tg_desconto.ToString() == "1" ? true : false,
                                    fkEmpresa = Convert.ToInt64(item.fk_empresa),
                                    fkFatura = Convert.ToInt64(item.fk_fatura),
                                    fkLoja = Convert.ToInt64(item.fk_loja),
                                    nuQuantidade = Convert.ToInt64(item.nu_quantidade),
                                    nuTipoFat = Convert.ToInt64(item.tg_tipoFat.ToString()),
                                    stExtras = item.st_extras,
                                    vrCobranca = Convert.ToInt64(item.vr_cobranca),
                                };

                                using (var cmd = new NpgsqlCommand("INSERT INTO \"FaturamentoDetalhe\" (\"id\",\"bDesconto\",\"fkEmpresa\",\"fkFatura\",\"fkLoja\",\"nuQuantidade\",\"nuTipoFat\",\"stExtras\",\"vrCobranca\" ) " +
                                                                    "VALUES ( @id,@bDesconto,@fkEmpresa,@fkFatura,@fkLoja,@nuQuantidade,@nuTipoFat,@stExtras,@vrCobranca );", dbPg))
                                {
                                    cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("bDesconto", ((object)novo.bDesconto) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkEmpresa", ((object)novo.fkEmpresa) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkFatura", ((object)novo.fkFatura) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkLoja", ((object)novo.fkLoja) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuQuantidade", ((object)novo.nuQuantidade) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuTipoFat", ((object)novo.nuTipoFat) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stExtras", ((object)novo.stExtras) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrCobranca", ((object)novo.vrCobranca) ?? DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region - JobFechamento - 

                {
                    long max = 9999999999;
                    using (var db = new AutorizadorCNDB())
                        max = (long)db.T_JobFechamento.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                    for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                    {
                        using (var db = new AutorizadorCNDB())
                        using (var dbPg = new NpgsqlConnection(connStr))
                        {
                            var lst = db.T_JobFechamento.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                            if (lst.Count() == 0)
                                continue;

                            dbPg.Open();

                            foreach (var item in lst)
                            {
                                var novo = new JobFechamento
                                {
                                    id = Convert.ToInt64(item.i_unique),
                                    dtFim = item.dt_fim,
                                    dtInicio = item.dt_inicio,
                                    fkEmpresa = item.fk_empresa,
                                    nuAno = Convert.ToInt64(item.st_ano),
                                    nuMes = Convert.ToInt64(item.st_mes),
                                };

                                using (var cmd = new NpgsqlCommand("INSERT INTO \"JobFechamento\" (\"id\",\"dtFim\",\"dtInicio\",\"fkEmpresa\",\"nuAno\",\"nuMes\" ) " +
                                                                    "VALUES ( @id,@dtFim,@dtInicio,@fkEmpresa,@nuAno,@nuMes );", dbPg))
                                {
                                    cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtFim", ((object)novo.dtFim) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtInicio", ((object)novo.dtInicio) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkEmpresa", ((object)novo.fkEmpresa) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuAno", ((object)novo.nuAno) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuMes", ((object)novo.nuMes) ?? DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region - LogAudit - 

                {
                    long max = 9999999999;
                    using (var db = new AutorizadorCNDB())
                        max = (long)db.LOG_Audit.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                    for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                    {
                        using (var db = new AutorizadorCNDB())
                        using (var dbPg = new NpgsqlConnection(connStr))
                        {
                            var lst = db.LOG_Audit.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                            if (lst.Count() == 0)
                                continue;

                            dbPg.Open();

                            foreach (var item in lst)
                            {
                                if (item.tg_operacao == null)
                                    continue;

                                var novo = new LogAudit
                                {
                                    id = Convert.ToInt64(item.i_unique),
                                    dtOperacao = item.dt_operacao,
                                    fkEmpresa = item.st_empresa == null ? 0 : Convert.ToInt64(db.T_Empresa.FirstOrDefault(y => y.st_empresa == item.st_empresa).i_unique),
                                    fkGeneric = item.fk_generic,
                                    fkUsuario = item.fk_usuario,
                                    nuOperacao = item.tg_operacao == null ? 0 : Convert.ToInt64(item.tg_operacao.ToString()),
                                    stLog = item.st_log,
                                    stOperacao = item.st_oper,
                                };

                                using (var cmd = new NpgsqlCommand("INSERT INTO \"LogAudit\" (\"id\",\"dtOperacao\",\"fkEmpresa\",\"fkGeneric\",\"fkUsuario\",\"nuOperacao\",\"stLog\",\"stOperacao\" ) " +
                                                                    "VALUES ( @id,@dtOperacao,@fkEmpresa,@fkGeneric,@fkUsuario,@nuOperacao,@stLog,@stOperacao );", dbPg))
                                {
                                    cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtOperacao", ((object)novo.dtOperacao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkEmpresa", ((object)novo.fkEmpresa) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkGeneric", ((object)novo.fkGeneric) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkUsuario", ((object)novo.fkUsuario) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuOperacao", ((object)novo.nuOperacao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stLog", ((object)novo.stLog) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stOperacao", ((object)novo.stOperacao) ?? DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region - LogFechamento - 

                {
                    long max = 9999999999;
                    using (var db = new AutorizadorCNDB())
                        max = (long)db.LOG_Fechamento.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                    for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                    {
                        using (var db = new AutorizadorCNDB())
                        using (var dbPg = new NpgsqlConnection(connStr))
                        {
                            var lst = db.LOG_Fechamento.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                            if (lst.Count() == 0)
                                continue;

                            dbPg.Open();

                            foreach (var item in lst)
                            {
                                var novo = new LogFechamento
                                {
                                    id = Convert.ToInt64(item.i_unique),
                                    dtCompra = item.dt_compra,
                                    dtFechamento = item.dt_fechamento,
                                    fkCartao = item.fk_cartao,
                                    fkEmpresa = item.fk_empresa,
                                    fkLoja = item.fk_loja,
                                    fkParcela = item.fk_parcela,
                                    nuAno = Convert.ToInt64(item.st_ano),
                                    nuMes = Convert.ToInt64(item.st_mes),
                                    nuParcela = item.nu_parcela,
                                    stCartao = item.st_cartao,
                                    vrValor = item.vr_valor
                                };

                                using (var cmd = new NpgsqlCommand("INSERT INTO \"LogFechamento\" (\"id\",\"dtCompra\",\"dtFechamento\",\"fkCartao\",\"fkEmpresa\",\"fkLoja\",\"fkParcela\",\"nuAno\",\"nuMes\",\"nuParcela\",\"stCartao\",\"vrValor\" ) " +
                                                                    "VALUES ( @id,@dtCompra,@dtFechamento,@fkCartao,@fkEmpresa,@fkLoja,@fkParcela,@nuAno,@nuMes,@nuParcela,@stCartao,@vrValor );", dbPg))
                                {
                                    cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtCompra", ((object)novo.dtCompra) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtFechamento", ((object)novo.dtFechamento) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkCartao", ((object)novo.fkCartao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkEmpresa", ((object)novo.fkEmpresa) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkLoja", ((object)novo.fkLoja) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkParcela", ((object)novo.fkParcela) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuAno", ((object)novo.nuAno) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuMes", ((object)novo.nuMes) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuParcela", ((object)novo.nuParcela) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stCartao", ((object)novo.stCartao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrValor", ((object)novo.vrValor) ?? DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region - LogTransacao - 

                {
                    long max = 9999999999;
                    using (var db = new AutorizadorCNDB())
                        max = (long)db.LOG_Transacoes.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                    for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                    {
                        using (var db = new AutorizadorCNDB())
                        using (var dbPg = new NpgsqlConnection(connStr))
                        {
                            var lst = db.LOG_Transacoes.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                            if (lst.Count() == 0)
                                continue;

                            dbPg.Open();

                            foreach (var item in lst)
                            {
                                var novo = new LogTransacao
                                {
                                    id = Convert.ToInt64(item.i_unique),
                                    bContabil = item.tg_contabil?.ToString() == "1" ? true : false,
                                    dtTransacao = item.dt_transacao,
                                    fkCartao = item.fk_cartao,
                                    fkEmpresa = item.fk_empresa,
                                    fkLoja = item.fk_loja,
                                    fkTerminal = item.fk_terminal,
                                    nuCodErro = item.nu_cod_erro,
                                    nuConfirmada = Convert.ToInt64(item.tg_confirmada.ToString()),
                                    nuNsu = item.nu_nsu,
                                    nuNsuOrig = item.nu_nsuOrig,
                                    nuOperacao = Convert.ToInt64(item.en_operacao),
                                    nuParcelas = item.nu_parcelas,
                                    stDoc = item.st_doc,
                                    stMsg = item.st_msg_transacao,
                                    vrTotal = item.vr_total
                                };

                                using (var cmd = new NpgsqlCommand("INSERT INTO \"LogTransacao\" (\"id\",\"bContabil\",\"dtTransacao\",\"fkCartao\",\"fkEmpresa\",\"fkLoja\",\"fkTerminal\",\"nuCodErro\"," +
                                                                    "\"nuConfirmada\",\"nuNsu\",\"nuNsuOrig\",\"nuOperacao\",\"nuParcelas\",\"stDoc\",\"stMsg\",\"vrTotal\"  ) " +
                                                                    "VALUES ( @id,@bContabil,@dtTransacao,@fkCartao,@fkEmpresa,@fkLoja,@fkTerminal,@nuCodErro," +
                                                                    "@nuConfirmada,@nuNsu,@nuNsuOrig,@nuOperacao,@nuParcelas,@stDoc,@stMsg,@vrTotal );", dbPg))
                                {
                                    cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("bContabil", ((object)novo.bContabil) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtTransacao", ((object)novo.dtTransacao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkCartao", ((object)novo.fkCartao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkEmpresa", ((object)novo.fkEmpresa) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkLoja", ((object)novo.fkLoja) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkTerminal", ((object)novo.fkTerminal) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuCodErro", ((object)novo.nuCodErro) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuConfirmada", ((object)novo.nuConfirmada) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuNsu", ((object)novo.nuNsu) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuNsuOrig", ((object)novo.nuNsuOrig) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuOperacao", ((object)novo.nuOperacao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuParcelas", ((object)novo.nuParcelas) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stDoc", ((object)novo.stDoc) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stMsg", ((object)novo.stMsg) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrTotal", ((object)novo.vrTotal) ?? DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region - Loja - 

                {
                    long max = 9999999999;
                    using (var db = new AutorizadorCNDB())
                        max = (long)db.T_Loja.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                    for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                    {
                        using (var db = new AutorizadorCNDB())
                        using (var dbPg = new NpgsqlConnection(connStr))
                        {
                            var lst = db.T_Loja.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                            if (lst.Count() == 0)
                                continue;

                            dbPg.Open();

                            foreach (var item in lst)
                            {
                                var novo = new Loja
                                {
                                    id = Convert.ToInt64(item.i_unique),
                                    bBlocked = item.tg_blocked?.ToString() == "1" ? true : false,
                                    bCancel = item.tg_cancel == 1 ? true : false,
                                    bIsentoFat = item.tg_isentoFat == 1 ? true : false,
                                    bPortalSenha = item.tg_portalComSenha == 1 ? true : false,
                                    fkBanco = item.fk_banco,
                                    nuBancoFat = item.nu_bancoFat,
                                    nuDiaVenc = item.nu_diavenc,
                                    nuFranquia = item.nu_franquia,
                                    nuPctValor = item.nu_pctValor,
                                    nuPeriodoFat = item.nu_periodoFat,
                                    nuTipoCob = Convert.ToInt64(item.tg_tipoCobranca?.ToString()),
                                    stAgencia = item.st_agencia,
                                    stCelular = item.st_telCelular,
                                    stCEP = item.nu_CEP,
                                    stCidade = item.st_cidade,
                                    stCNPJ = item.nu_CNPJ,
                                    stConta = item.st_conta,
                                    stContaDeb = item.nu_contaDeb,
                                    stContato = item.st_contato,
                                    stCPFResp = item.st_cpfResp,
                                    stDataResp = item.st_dataResp,
                                    stEmail = item.st_email,
                                    stEndereco = item.st_endereco,
                                    stEnderecoInst = item.st_enderecoInst,
                                    stEstado = item.st_estado,
                                    stFax = item.nu_fax,
                                    stInscEst = item.nu_inscEst,
                                    stLoja = item.st_loja,
                                    stNome = item.st_nome,
                                    stObs = item.st_obs,
                                    stSenha = item.st_senha,
                                    stSocial = item.st_social,
                                    stTelefone = item.nu_telefone,
                                    vrMensalidade = item.vr_mensalidade,
                                    vrMinimo = item.vr_minimo,
                                    vrTransacao = item.vr_transacao
                                };

                                using (var cmd = new NpgsqlCommand("INSERT INTO \"Loja\" (\"id\",\"bBlocked\",\"bCancel\",\"bIsentoFat\",\"bPortalSenha\",\"fkBanco\",\"nuBancoFat\"," +
                                                                    "\"nuDiaVenc\",\"nuFranquia\",\"nuPctValor\",\"nuPeriodoFat\",\"nuTipoCob\",\"stAgencia\",\"stCelular\"," +
                                                                    "\"stCEP\",\"stCidade\",\"stCNPJ\",\"stConta\",\"stContaDeb\",\"stContato\",\"stCPFResp\"," +
                                                                    "\"stDataResp\",\"stEmail\",\"stEndereco\",\"stEnderecoInst\",\"stEstado\",\"stFax\",\"stInscEst\",\"stLoja\",\"stNome\"," +
                                                                    "\"stObs\",\"stSenha\",\"stSocial\",\"stTelefone\",\"vrMensalidade\",\"vrMinimo\",\"vrTransacao\" ) " +
                                                                    "VALUES ( @id,@bBlocked,@bCancel,@bIsentoFat,@bPortalSenha,@fkBanco,@nuBancoFat," +
                                                                    "@nuDiaVenc,@nuFranquia,@nuPctValor,@nuPeriodoFat,@nuTipoCob,@stAgencia,@stCelular," +
                                                                    "@stCEP,@stCidade,@stCNPJ,@stConta,@stContaDeb,@stContato,@stCPFResp," +
                                                                    "@stDataResp,@stEmail,@stEndereco,@stEnderecoInst,@stEstado,@stFax,@stInscEst,@stLoja,@stNome," +
                                                                    "@stObs,@stSenha,@stSocial,@stTelefone,@vrMensalidade,@vrMinimo,@vrTransacao );", dbPg))
                                {
                                    cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("bBlocked", ((object)novo.bBlocked) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("bCancel", ((object)novo.bCancel) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("bIsentoFat", ((object)novo.bIsentoFat) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("bPortalSenha", ((object)novo.bPortalSenha) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkBanco", ((object)novo.fkBanco) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuBancoFat", ((object)novo.nuBancoFat) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuDiaVenc", ((object)novo.nuDiaVenc) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuFranquia", ((object)novo.nuFranquia) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuPctValor", ((object)novo.nuPctValor) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuPeriodoFat", ((object)novo.nuPeriodoFat) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuTipoCob", ((object)novo.nuTipoCob) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stAgencia", ((object)novo.stAgencia) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stCelular", ((object)novo.stCelular) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stCEP", ((object)novo.stCEP) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stCidade", ((object)novo.stCidade) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stCNPJ", ((object)novo.stCNPJ) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stConta", ((object)novo.stConta) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stContaDeb", ((object)novo.stContaDeb) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stContato", ((object)novo.stContato) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stCPFResp", ((object)novo.stCPFResp) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stDataResp", ((object)novo.stDataResp) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stEmail", ((object)novo.stEmail) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stEndereco", ((object)novo.stEndereco) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stEnderecoInst", ((object)novo.stEnderecoInst) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stEstado", ((object)novo.stEstado) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stFax", ((object)novo.stFax) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stInscEst", ((object)novo.stInscEst) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stLoja", ((object)novo.stLoja) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stNome", ((object)novo.stNome) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stObs", ((object)novo.stObs) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stSenha", ((object)novo.stSenha) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stSocial", ((object)novo.stSocial) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stTelefone", ((object)novo.stTelefone) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrMensalidade", ((object)novo.vrMensalidade) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrMinimo", ((object)novo.vrMinimo) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("vrTransacao", ((object)novo.vrTransacao) ?? DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region - LojaEmpresa -

                {
                    long max = 9999999999;
                    using (var db = new AutorizadorCNDB())
                        max = (long)db.LINK_LojaEmpresa.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                    for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                    {
                        using (var db = new AutorizadorCNDB())
                        using (var dbPg = new NpgsqlConnection(connStr))
                        {
                            var lst = db.LINK_LojaEmpresa.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                            if (lst.Count() == 0)
                                continue;

                            dbPg.Open();

                            foreach (var item in lst)
                            {
                                var novo = new LojaEmpresa
                                {
                                    id = Convert.ToInt64(item.i_unique),
                                    fkEmpresa = item.fk_empresa,
                                    fkLoja = item.fk_loja,
                                    nuDiasRepasse = item.nu_dias_repasse,
                                    nuTxAdmin = item.tx_admin,
                                    stAgencia = item.st_ag,
                                    stBanco = item.st_banco,
                                    stConta = item.st_conta
                                };

                                using (var cmd = new NpgsqlCommand("INSERT INTO \"LojaEmpresa\" (\"id\",\"fkEmpresa\",\"fkLoja\",\"nuDiasRepasse\",\"nuTxAdmin\",\"stAgencia\",\"stBanco\",\"stConta\" ) " +
                                                                    "VALUES ( @id,@fkEmpresa,@fkLoja,@nuDiasRepasse,@nuTxAdmin,@stAgencia,@stBanco,@stConta );", dbPg))
                                {
                                    cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkEmpresa", ((object)novo.fkEmpresa) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkLoja", ((object)novo.fkLoja) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuDiasRepasse", ((object)novo.nuDiasRepasse) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuTxAdmin", ((object)novo.nuTxAdmin) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stAgencia", ((object)novo.stAgencia) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stBanco", ((object)novo.stBanco) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stConta", ((object)novo.stConta) ?? DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region - LojaMsg -

                {
                    long max = 9999999999;
                    using (var db = new AutorizadorCNDB())
                        max = (long)db.T_LojaMensagem.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                    for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                    {
                        using (var db = new AutorizadorCNDB())
                        using (var dbPg = new NpgsqlConnection(connStr))
                        {
                            var lst = db.T_LojaMensagem.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                            if (lst.Count() == 0)
                                continue;

                            dbPg.Open();

                            foreach (var item in lst)
                            {
                                var novo = new LojaMsg
                                {
                                    id = Convert.ToInt64(item.i_unique),
                                    bAtiva = item.tg_ativa,
                                    dtCriacao = item.dt_criacao,
                                    dtValidade = item.dt_validade,
                                    fkLoja = item.fk_loja,
                                    stLink = item.st_link,
                                    stMsg = item.st_msg
                                };

                                using (var cmd = new NpgsqlCommand("INSERT INTO \"LojaMsg\" (\"id\",\"bAtiva\",\"dtCriacao\",\"dtValidade\",\"fkLoja\",\"stLink\",\"stMsg\" ) " +
                                                                    "VALUES ( @id,@bAtiva,@dtCriacao,@dtValidade,@fkLoja,@stLink,@stMsg );", dbPg))
                                {
                                    cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("bAtiva", ((object)novo.bAtiva) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtCriacao", ((object)novo.dtCriacao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtValidade", ((object)novo.dtValidade) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkLoja", ((object)novo.fkLoja) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stLink", ((object)novo.stLink) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("stMsg", ((object)novo.stMsg) ?? DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region - LoteCartao -

                {
                    long max = 9999999999;
                    using (var db = new AutorizadorCNDB())
                        max = (long)db.T_LoteCartao.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                    for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                    {
                        using (var db = new AutorizadorCNDB())
                        using (var dbPg = new NpgsqlConnection(connStr))
                        {
                            var lst = db.T_LoteCartao.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                            if (lst.Count() == 0)
                                continue;

                            dbPg.Open();

                            foreach (var item in lst)
                            {
                                var novo = new LoteCartao
                                {
                                    id = Convert.ToInt64(item.i_unique),
                                    dtAbertura = item.dt_abertura,
                                    dtAtivacao = item.dt_ativacao,
                                    dtEnvioGrafica = item.dt_envio_grafica,
                                    fkEmpresa = item.fk_empresa,
                                    nuCartoes = item.nu_cartoes,
                                    nuSitLote = item.tg_sitLote
                                };

                                using (var cmd = new NpgsqlCommand("INSERT INTO \"LoteCartao\" (\"id\",\"dtAbertura\",\"dtAtivacao\",\"dtEnvioGrafica\",\"fkEmpresa\",\"nuCartoes\",\"nuSitLote\" ) " +
                                                                    "VALUES ( @id,@dtAbertura,@dtAtivacao,@dtEnvioGrafica,@fkEmpresa,@nuCartoes,@nuSitLote );", dbPg))
                                {
                                    cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtAbertura", ((object)novo.dtAbertura) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtAtivacao", ((object)novo.dtAtivacao) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("dtEnvioGrafica", ((object)novo.dtEnvioGrafica) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("fkEmpresa", ((object)novo.fkEmpresa) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuCartoes", ((object)novo.nuCartoes) ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("nuSitLote", ((object)novo.nuSitLote) ?? DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                #endregion

            }

            #region - LoteCartaoDetalhe -
            
            {
                long max = 9999999999;
                using (var db = new AutorizadorCNDB())
                    max = (long)db.T_LoteCartaoDetalhe.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                {
                    using (var db = new AutorizadorCNDB())
                    using (var dbPg = new NpgsqlConnection(connStr))
                    {
                        var lst = db.T_LoteCartaoDetalhe.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                        if (lst.Count() == 0)
                            continue;

                        dbPg.Open();

                        foreach (var item in lst)
                        {
                            var novo = new LoteCartaoDetalhe
                            {
                                id = Convert.ToInt64(item.i_unique),
                                dtAtivacao = item.dt_ativacao,
                                dtPedido = item.dt_pedido,
                                fkCartao = item.fk_cartao,
                                fkEmpresa = item.fk_empresa,
                                fkLote = item.fk_lote,
                                nuMatricula = item.nu_matricula,
                                nuTitularidade = item.nu_titularidade,
                                nuViaOriginal = item.nu_via_original,
                                stCPF = item.nu_cpf?.PadRight(20, ' ').Substring(0, 20).Trim(),
                                stNomeCartao = item.st_nome_cartao?.PadRight(30, ' ').Substring(0, 30).Trim(),
                            };

                            using (var cmd = new NpgsqlCommand("INSERT INTO \"LoteCartaoDetalhe\" (\"id\",\"dtAtivacao\",\"dtPedido\",\"fkCartao\",\"fkEmpresa\",\"fkLote\",\"nuMatricula\",\"nuTitularidade\",\"nuViaOriginal\",\"stCPF\",\"stNomeCartao\" ) " +
                                                                "VALUES ( @id,@dtAtivacao,@dtPedido,@fkCartao,@fkEmpresa,@fkLote,@nuMatricula,@nuTitularidade,@nuViaOriginal,@stCPF,@stNomeCartao );", dbPg))
                            {
                                cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("dtAtivacao", ((object)novo.dtAtivacao) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("dtPedido", ((object)novo.dtPedido) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("fkCartao", ((object)novo.fkCartao) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("fkEmpresa", ((object)novo.fkEmpresa) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("fkLote", ((object)novo.fkLote) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("nuMatricula", ((object)novo.nuMatricula) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("nuTitularidade", ((object)novo.nuTitularidade) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("nuViaOriginal", ((object)novo.nuViaOriginal) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stCPF", ((object)novo.stCPF) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stNomeCartao", ((object)novo.stNomeCartao) ?? DBNull.Value);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            #endregion

            #region - Parceiro -
            
            {
                long max = 9999999999;
                using (var db = new AutorizadorCNDB())
                    max = (long)db.Parceiro.OrderByDescending(y => y.id).FirstOrDefault().id;
                for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                {
                    using (var db = new AutorizadorCNDB())
                    using (var dbPg = new NpgsqlConnection(connStr))
                    {
                        var lst = db.Parceiro.Where(y => y.id >= g && y.id < t).OrderBy(y => y.id).ToList();

                        if (lst.Count() == 0)
                            continue;

                        dbPg.Open();

                        foreach (var item in lst)
                        {
                            var novo = new Parceiro
                            {
                                id = Convert.ToInt64(item.id),
                                dtCadastro = item.dtCadastro,
                                stCelular = item.stCelular,
                                stCEP = item.stCep,
                                stCidade = item.stCidade,
                                stCNPJ = item.stCNPJ,
                                stEndereco = item.stEndereco,
                                stEstado = item.stEstado,
                                stNome = item.stNome,
                                stObs = item.stObs,
                                stResp = item.stResp,
                                stSocial = item.stSocial,
                                stTelefone = item.stTelefone                                
                            };

                            using (var cmd = new NpgsqlCommand("INSERT INTO \"Parceiro\" (\"id\",\"dtCadastro\",\"stCelular\",\"stCEP\",\"stCidade\",\"stCNPJ\",\"stEndereco\",\"stEstado\",\"stNome\"," +
                                "\"stObs\",\"stResp\",\"stSocial\",\"stTelefone\" ) " +
                                                                "VALUES ( @id,@dtCadastro,@stCelular,@stCEP,@stCidade,@stCNPJ,@stEndereco,@stEstado,@stNome," +
                                "@stObs,@stResp,@stSocial,@stTelefone );", dbPg))
                            {
                                cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("dtCadastro", ((object)novo.dtCadastro) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stCelular", ((object)novo.stCelular) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stCEP", ((object)novo.stCEP) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stCidade", ((object)novo.stCidade) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stCNPJ", ((object)novo.stCNPJ) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stEndereco", ((object)novo.stEndereco) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stEstado", ((object)novo.stEstado) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stNome", ((object)novo.stNome) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stObs", ((object)novo.stObs) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stResp", ((object)novo.stResp) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stSocial", ((object)novo.stSocial) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stTelefone", ((object)novo.stTelefone) ?? DBNull.Value);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            #endregion

            #region - Parcela -
            
            {
                long max = 9999999999;
                using (var db = new AutorizadorCNDB())
                    max = (long)db.T_Parcelas.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                {
                    using (var db = new AutorizadorCNDB())
                    using (var dbPg = new NpgsqlConnection(connStr))
                    {
                        var lst = db.T_Parcelas.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                        if (lst.Count() == 0)
                            continue;

                        dbPg.Open();

                        foreach (var item in lst)
                        {
                            var novo = new Parcela
                            {
                                id = Convert.ToInt64(item.i_unique),                                
                                dtInclusao = item.dt_inclusao,
                                fkCartao = item.fk_cartao,
                                fkEmpresa = item.fk_empresa,
                                fkLogTransacao = item.fk_log_transacoes,
                                fkLoja = item.fk_loja,
                                fkTerminal = item.fk_terminal,
                                nuIndice = item.nu_indice,
                                nuNsu = item.nu_nsu,
                                nuParcela = item.nu_parcela,
                                nuTotParcelas = item.nu_tot_parcelas,
                                vrValor = item.vr_valor,                                
                            };

                            using (var cmd = new NpgsqlCommand("INSERT INTO \"Parcela\" (\"id\",\"dtInclusao\",\"fkCartao\",\"fkEmpresa\",\"fkLogTransacao\",\"fkLoja\",\"fkTerminal\",\"nuIndice\",\"nuNsu\",\"nuParcela\",\"nuTotParcelas\",\"vrValor\" ) " +
                                                                "VALUES ( @id,@dtInclusao,@fkCartao,@fkEmpresa,@fkLogTransacao,@fkLoja,@fkTerminal,@nuIndice,@nuNsu,@nuParcela,@nuTotParcelas,@vrValor );", dbPg))
                            {
                                cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);

                                cmd.Parameters.AddWithValue("dtInclusao", ((object)novo.dtInclusao) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("fkCartao", ((object)novo.fkCartao) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("fkEmpresa", ((object)novo.fkEmpresa) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("fkLogTransacao", ((object)novo.fkLogTransacao) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("fkLoja", ((object)novo.fkLoja) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("fkTerminal", ((object)novo.fkTerminal) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("nuIndice", ((object)novo.nuIndice) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("nuNsu", ((object)novo.nuNsu) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("nuParcela", ((object)novo.nuParcela) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("nuTotParcelas", ((object)novo.nuTotParcelas) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("vrValor", ((object)novo.vrValor) ?? DBNull.Value);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            #endregion

            #region - SaldoConvenio -
            if (false)
            {
                // não tem valores
            }
            #endregion

            #region - SolicitacaoVenda -
            if (false)
            {
               // não tem valores
            }
            #endregion

            #region - Terminal -
            
            {
                long max = 9999999999;
                using (var db = new AutorizadorCNDB())
                    max = (long)db.T_Terminal.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                {
                    using (var db = new AutorizadorCNDB())
                    using (var dbPg = new NpgsqlConnection(connStr))
                    {
                        var lst = db.T_Terminal.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                        if (lst.Count() == 0)
                            continue;

                        dbPg.Open();

                        foreach (var item in lst)
                        {
                            var novo = new Terminal
                            {
                                id = Convert.ToInt64(item.i_unique),
                                fkLoja = item.fk_loja,
                                stLocal = item.st_localizacao,
                                stTerminal = item.nu_terminal
                            };

                            using (var cmd = new NpgsqlCommand("INSERT INTO \"Terminal\" (\"id\",\"fkLoja\",\"stLocal\",\"stTerminal\" ) " +
                                                                "VALUES ( @id,@fkLoja,@stLocal,@stTerminal );", dbPg))
                            {
                                cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("fkLoja", ((object)novo.fkLoja) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stLocal", ((object)novo.stLocal) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stTerminal", ((object)novo.stTerminal) ?? DBNull.Value);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            #endregion

            #region - UsuarioEmissor -
            
            {
                long max = 9999999999;
                using (var db = new AutorizadorCNDB())
                    max = (long)db.T_Usuario.OrderByDescending(y => y.i_unique).FirstOrDefault().i_unique;
                for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                {
                    using (var db = new AutorizadorCNDB())
                    using (var dbPg = new NpgsqlConnection(connStr))
                    {
                        var lst = db.T_Usuario.Where(y => y.i_unique >= g && y.i_unique < t).OrderBy(y => y.i_unique).ToList();

                        if (lst.Count() == 0)
                            continue;

                        dbPg.Open();

                        foreach (var item in lst)
                        {
                            var novo = new UsuarioEmissor
                            {
                                id = Convert.ToInt64(item.i_unique),
                                bAviso = item.tg_aviso == 1 ? true: false,
                                bBloqueio = item.tg_bloqueio?.ToString() == "1" ? true : false,
                                bOperador = item.bOperador,
                                bTrocaSenha = item.tg_trocaSenha?.ToString() == "1" ? true : false,
                                dtTrocaSenha = item.dt_trocaSenha,
                                dtUltUso = item.dt_ultUso,
                                fkEmpresa = Convert.ToInt64(db.T_Empresa.FirstOrDefault( y=> y.st_empresa == item.st_empresa )?.i_unique),
                                nuNivel = Convert.ToInt64(item.tg_nivel?.ToString()),
                                nuSenhaErrada = item.nu_senhaErrada,
                                stNome = item.st_nome,
                                stSenha = item.st_senha,                                
                            };

                            using (var cmd = new NpgsqlCommand("INSERT INTO \"UsuarioEmissor\" (\"id\",\"bAviso\",\"bBloqueio\",\"bOperador\",\"bTrocaSenha\",\"dtTrocaSenha\",\"dtUltUso\",\"fkEmpresa\",\"nuNivel\",\"nuSenhaErrada\",\"stNome\",\"stSenha\" ) " +
                                                                "VALUES ( @id,@bAviso,@bBloqueio,@bOperador,@bTrocaSenha,@dtTrocaSenha,@dtUltUso,@fkEmpresa,@nuNivel,@nuSenhaErrada,@stNome,@stSenha );", dbPg))
                            {
                                cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);

                                cmd.Parameters.AddWithValue("bAviso", ((object)novo.bAviso) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("bBloqueio", ((object)novo.bBloqueio) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("bOperador", ((object)novo.bOperador) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("bTrocaSenha", ((object)novo.bTrocaSenha) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("dtTrocaSenha", ((object)novo.dtTrocaSenha) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("dtUltUso", ((object)novo.dtUltUso) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("fkEmpresa", ((object)novo.fkEmpresa) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("nuNivel", ((object)novo.nuNivel) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("nuSenhaErrada", ((object)novo.nuSenhaErrada) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stNome", ((object)novo.stNome) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stSenha", ((object)novo.stSenha) ?? DBNull.Value);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            #endregion

            #region - UsuarioParceiro -
            
            {
                long max = 9999999999;
                using (var db = new AutorizadorCNDB())
                    max = (long)db.UsuarioParceiro.OrderByDescending(y => y.id).FirstOrDefault().id;
                for (int t = 1000, g = 0; g < max; t += 1000, g += 1000)
                {
                    using (var db = new AutorizadorCNDB())
                    using (var dbPg = new NpgsqlConnection(connStr))
                    {
                        var lst = db.UsuarioParceiro.Where(y => y.id >= g && y.id < t).OrderBy(y => y.id).ToList();

                        if (lst.Count() == 0)
                            continue;

                        dbPg.Open();

                        foreach (var item in lst)
                        {
                            var novo = new UsuarioParceiro
                            {
                                id = Convert.ToInt64(item.id),
                                bAtivo = item.bAtivo,
                                dtCadastro = item.dtCadastro,
                                dtUltLogin = item.dtLastLogin,
                                fkParceiro = item.fkParceiro,
                                nuTipo = item.nuTipo,
                                stEmail = item.stEmail,
                                stNome = item.stNome,
                                stSenha = item.stSenha
                            };

                            using (var cmd = new NpgsqlCommand("INSERT INTO \"UsuarioParceiro\" (\"id\",\"bAtivo\",\"dtCadastro\",\"dtUltLogin\",\"fkParceiro\",\"nuTipo\",\"stEmail\",\"stNome\",\"stSenha\" ) " +
                                                                "VALUES ( @id,@bAtivo,@dtCadastro,@dtUltLogin,@fkParceiro,@nuTipo,@stEmail,@stNome,@stSenha );", dbPg))
                            {
                                cmd.Parameters.AddWithValue("id", ((object)novo.id) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("bAtivo", ((object)novo.bAtivo) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("dtCadastro", ((object)novo.dtCadastro) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("dtUltLogin", ((object)novo.dtUltLogin) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("fkParceiro", ((object)novo.fkParceiro) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("nuTipo", ((object)novo.nuTipo) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stEmail", ((object)novo.stEmail) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stNome", ((object)novo.stNome) ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("stSenha", ((object)novo.stSenha) ?? DBNull.Value);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            #endregion

            Console.WriteLine(DateTime.Now);
            Console.ReadLine();
        }
    }
}
