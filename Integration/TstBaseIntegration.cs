using Api.Master.Controllers;
using Dapper;
using Master.Data.Const;
using Master.Data.Database;
using Master.Data.Domains.User;
using Master.Infra;
using Master.Repository;
using Master.Service;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.IO;
using System.Text.Json;

namespace IntegrationTest
{
    public class TstBaseIntegration
    {
        public string connStr = "User ID=postgres;Password=Gustavo123;Host=localhost;Port=5432;Database=ConveynetIntegration;";
        public string baseDb = File.ReadAllText(@"Repository\_CreateDB_pg.sql");
        
        public string[] truncateTables =
        {
            "Cartao",
            "ConfigPlasticoEnvio",
            "DashboardGrafico",
            "Empresa",
            "Faturamento",
            "FaturamentoDetalhe",
            "JobFechamento",
            "LogAudit",
            "LogFechamento",
            "LogNsu",
            "LogTransacao",
            "Loja",
            "LojaEmpresa",
            "LojaMsg",
            "LoteCartao",
            "LoteCartaoDetalhe",
            "Parceiro",
            "Parcela",
            "SaldoConvenio",
            "SolicitacaoVenda",
            "Terminal",
            "UsuarioEmissor",
            "UsuarioParceiro",
        };

        public void SetupDatabase()
        {
            using var db = new NpgsqlConnection(connStr);
            db.Open();

            new NpgsqlCommand(baseDb, db).ExecuteNonQuery();

            foreach (var item in truncateTables)
                db.Query("truncate table \"" + item + "\" RESTART IDENTITY");

            db.Close();
        }

        public void SetupLoginOK(long cota, long limiteM, long limiteT)
        {
            using (var db = new NpgsqlConnection(connStr))
            {
                db.Open();

                {
                    var da = new EmpresaDapperRepository();

                    da.InsertEmpresa(db, new Empresa
                    {
                        bBlocked = false,
                        nuEmpresa = 2,
                    });
                }
                {
                    var da = new CartaoDapperRepository();

                    da.InsertCartao(db, new Cartao
                    {
                        fkEmpresa = 1,
                        nuEmitido = StatusExpedicao.Expedido,
                        nuMatricula = 1,
                        nuStatus = CartaoStatus.Habilitado,
                        nuViaCartao = 1,
                        stVenctoCartao = "0716",
                        stSenha = SrvBaseService.DESCript("1234"),
                        nuTitularidade = 1,
                        stCpf = "57357552004",
                        vrCotaExtra = cota,
                        vrLimiteMensal = limiteM,
                        vrLimiteRotativo = 0,
                        vrLimiteTotal = limiteT,
                    });
                }

                db.Close();
            }
        }

        public string Login()
        {
            var ctrl = new CtrlAuthenticate(null, null)
            {
                network = new LocalNetwork
                {
                    sqlServer = connStr,
                }
            };

            OkObjectResult ret = (OkObjectResult) ctrl.Post(new DtoLoginInformation
            {
                empresa = "2",
                matricula = "1",
                codAcesso = "0314",
                venc = "0716",
                login = "",
                userType = "2",
                email = "",
                senha = "1234"
            });

            var dtoToken = ret.Value as DtoToken;
            
            return dtoToken.token;
        }
    }
}
