using Api.Master.Controllers;
using Master.Infra.Entity.Database;
using Master.Repository;
using Master.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;

namespace IntegrationTest
{
    [TestClass]
    public class TstCtrlAuthenticate : TstBaseIntegration 
    {
        CtrlAuthenticate SetupController()
        {
            return new CtrlAuthenticate(null)
            { 
                network = new Master.LocalNetwork
                {
                    sqlServer = connStr
                }
            };
        }

        [TestMethod]
        public void EmpresaInexistente()
        {
            #region - code - 

            SetupDatabase();
            var ctrl = SetupController();

            var ret = ctrl.Post(new Entities.Api.User.DtoLoginInformation
            {
                empresa = "4444",
                matricula = "1",
                codAcesso = "0864",
                venc = "0716",
                login = "",
                userType = "2",                
                email = "",
                senha = "1234"
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();

            #endregion
        }

        [TestMethod]
        public void EmpresaBloqueada()
        {
            #region - code -

            SetupDatabase();
            var ctrl = SetupController();

            using (var db = new NpgsqlConnection(connStr))
            {
                db.Open();

                var da = new DapperRepository();

                da.InsertEmpresa(db, new Master.Infra.Entity.Database.Empresa
                {
                    bBlocked = true,
                    nuEmpresa = 4444,
                });

                db.Close();
            }

            var ret = ctrl.Post(new Entities.Api.User.DtoLoginInformation
            {
                empresa = "4444",
                matricula = "1",
                codAcesso = "0864",
                venc = "0716",
                login = "",
                userType = "2",
                email = "",
                senha = "1234"
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();

            #endregion
        }

        [TestMethod]
        public void CartaoInexistente()
        {
            #region - code -

            SetupDatabase();
            var ctrl = SetupController();

            using (var db = new NpgsqlConnection(connStr))
            {
                db.Open();

                var da = new DapperRepository();

                da.InsertEmpresa(db, new Master.Infra.Entity.Database.Empresa
                {
                    bBlocked = false,
                    nuEmpresa = 2,
                });

                db.Close();
            }

            var ret = ctrl.Post(new Entities.Api.User.DtoLoginInformation
            {
                empresa = "2",
                matricula = "1",
                codAcesso = "0864",
                venc = "0716",
                login = "",
                userType = "2",
                email = "",
                senha = "1234"
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();

            #endregion
        }

        [TestMethod]
        public void CartaoSenhaErrada()
        {
            #region - code -

            SetupDatabase();
            var ctrl = SetupController();

            using (var db = new NpgsqlConnection(connStr))
            {
                db.Open();

                var da = new DapperRepository();

                da.InsertEmpresa(db, new Master.Infra.Entity.Database.Empresa
                {
                    bBlocked = false,
                    nuEmpresa = 2,
                });

                da.InsertCartao(db, new Master.Infra.Entity.Database.Cartao
                {
                    fkEmpresa = 1,
                    nuEmitido = 2,
                    nuMatricula = 1,
                    nuStatus = CartaoStatus.Bloqueado,
                    nuViaCartao = 1,
                    stVenctoCartao = "0716",
                    stSenha = SrvBaseService.DESCript("1111"),
                    nuTitularidade = 1,
                });

                db.Close();
            }

            var ret = ctrl.Post(new Entities.Api.User.DtoLoginInformation
            {
                empresa = "2",
                matricula = "1",
                codAcesso = "0864",
                venc = "0716",
                login = "",
                userType = "2",
                email = "",
                senha = "1234"
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();

            #endregion
        }

        [TestMethod]
        public void CartaoStatusBloqueado()
        {
            #region - code -

            SetupDatabase();
            var ctrl = SetupController();

            using (var db = new NpgsqlConnection(connStr))
            {
                db.Open();

                var da = new DapperRepository();

                da.InsertEmpresa(db, new Master.Infra.Entity.Database.Empresa
                {
                    bBlocked = false,
                    nuEmpresa = 2,
                });

                da.InsertCartao(db, new Master.Infra.Entity.Database.Cartao
                {
                    fkEmpresa = 1,
                    nuEmitido = 2,
                    nuMatricula = 1,
                    nuStatus = CartaoStatus.Bloqueado,
                    nuViaCartao = 1,
                    stVenctoCartao = "0716",
                    stSenha = SrvBaseService.DESCript("1234"),
                    nuTitularidade = 1,
                });

                db.Close();
            }

            var ret = ctrl.Post(new Entities.Api.User.DtoLoginInformation
            {
                empresa = "2",
                matricula = "1",
                codAcesso = "0864",
                venc = "0716",
                login = "",
                userType = "2",
                email = "",
                senha = "1234"
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();

            #endregion
        }

        [TestMethod]
        public void CartaoVencErrado()
        {
            #region - code -

            SetupDatabase();
            var ctrl = SetupController();

            using (var db = new NpgsqlConnection(connStr))
            {
                db.Open();

                var da = new DapperRepository();

                da.InsertEmpresa(db, new Master.Infra.Entity.Database.Empresa
                {
                    bBlocked = false,
                    nuEmpresa = 2,
                });

                da.InsertCartao(db, new Master.Infra.Entity.Database.Cartao
                {
                    fkEmpresa = 1,
                    nuEmitido = 2,
                    nuMatricula = 1,
                    nuStatus = CartaoStatus.Habilitado,
                    nuViaCartao = 1,
                    stVenctoCartao = "0711",
                    stSenha = SrvBaseService.DESCript("1234"),
                    nuTitularidade = 1,
                });

                db.Close();
            }

            var ret = ctrl.Post(new Entities.Api.User.DtoLoginInformation
            {
                empresa = "2",
                matricula = "1",
                codAcesso = "0864",
                venc = "0716",
                login = "",
                userType = "2",
                email = "",
                senha = "1234"
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();

            #endregion
        }

        [TestMethod]
        public void CartaoCodAcessoErrado()
        {
            #region - code -

            SetupDatabase();
            var ctrl = SetupController();

            using (var db = new NpgsqlConnection(connStr))
            {
                db.Open();

                var da = new DapperRepository();

                da.InsertEmpresa(db, new Master.Infra.Entity.Database.Empresa
                {
                    bBlocked = false,
                    nuEmpresa = 2,
                });

                da.InsertCartao(db, new Master.Infra.Entity.Database.Cartao
                {
                    fkEmpresa = 1,
                    nuEmitido = 2,
                    nuMatricula = 1,
                    nuStatus = CartaoStatus.Habilitado,
                    nuViaCartao = 1,
                    stVenctoCartao = "0716",
                    stSenha = SrvBaseService.DESCript("1234"),
                    nuTitularidade = 1,
                    stCpf = "57357552004",
                });

                db.Close();
            }

            var ret = ctrl.Post(new Entities.Api.User.DtoLoginInformation
            {
                empresa = "2",
                matricula = "1",
                codAcesso = "0311",
                venc = "0716",
                login = "",
                userType = "2",
                email = "",
                senha = "1234"
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();

            #endregion
        }

        [TestMethod]
        public void AcessoOK()
        {
            #region - code -

            SetupDatabase();
            var ctrl = SetupController();

            using (var db = new NpgsqlConnection(connStr))
            {
                db.Open();

                var da = new DapperRepository();

                da.InsertEmpresa(db, new Master.Infra.Entity.Database.Empresa
                {
                    bBlocked = false,
                    nuEmpresa = 2,
                });

                da.InsertCartao(db, new Master.Infra.Entity.Database.Cartao
                {
                    fkEmpresa = 1,
                    nuEmitido = 2,
                    nuMatricula = 1,
                    nuStatus = CartaoStatus.Habilitado,
                    nuViaCartao = 1,
                    stVenctoCartao = "0716",
                    stSenha = SrvBaseService.DESCript("1234"),
                    nuTitularidade = 1,
                    stCpf = "57357552004",
                });

                db.Close();
            }

            var ret = ctrl.Post(new Entities.Api.User.DtoLoginInformation
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

            if (ret.ToString().Contains("BadRequest"))
                Assert.Fail();

            #endregion
        }
    }
}
