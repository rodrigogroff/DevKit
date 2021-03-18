using Api.Master.Controllers;
using Master.Data.Const;
using Master.Data.Database;
using Master.Data.Domains.User;
using Master.Infra;
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
                network = new LocalNetwork
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

            var ret = ctrl.Post(new DtoLoginInformation
            {
                empresa = "4444",
                matricula = "1",
                codAcesso = "0864",
                venc = "0716",
                login = "",
                userType = TipoUsuario.Associado.ToString(),
                email = "",
                senha = "1234"
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();

            #endregion
        }

        [TestMethod]
        public void TipoUsuarioInexistente()
        {
            #region - code - 

            SetupDatabase();
            var ctrl = SetupController();

            var ret = ctrl.Post(new DtoLoginInformation
            {
                empresa = "4444",
                matricula = "1",
                codAcesso = "0864",
                venc = "0716",
                login = "",
                userType = "9999",
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

                var da = new EmpresaDapperRepository();

                da.InsertEmpresa(db, new Empresa
                {
                    bBlocked = true,
                    nuEmpresa = 4444,
                });

                db.Close();
            }

            var ret = ctrl.Post(new DtoLoginInformation
            {
                empresa = "4444",
                matricula = "1",
                codAcesso = "0864",
                venc = "0716",
                login = "",
                userType = TipoUsuario.Associado.ToString(),
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

                var da = new EmpresaDapperRepository();

                da.InsertEmpresa(db, new Empresa
                {
                    bBlocked = false,
                    nuEmpresa = 2,
                });

                db.Close();
            }

            var ret = ctrl.Post(new DtoLoginInformation
            {
                empresa = "2",
                matricula = "1",
                codAcesso = "0864",
                venc = "0716",
                login = "",
                userType = TipoUsuario.Associado.ToString(),
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
                        nuEmitido = StatusExpedicao.NaoExpedido,
                        nuMatricula = 1,
                        nuStatus = CartaoStatus.Bloqueado,
                        nuViaCartao = 1,
                        stVenctoCartao = "0716",
                        stSenha = SrvBaseService.DESCript("1111"),
                        nuTitularidade = 1,
                    });
                }

                db.Close();
            }

            var ret = ctrl.Post(new DtoLoginInformation
            {
                empresa = "2",
                matricula = "1",
                codAcesso = "0864",
                venc = "0716",
                login = "",
                userType = TipoUsuario.Associado.ToString(),
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
                        nuEmitido = StatusExpedicao.NaoExpedido,
                        nuMatricula = 1,
                        nuStatus = CartaoStatus.Bloqueado,
                        nuViaCartao = 1,
                        stVenctoCartao = "0716",
                        stSenha = SrvBaseService.DESCript("1234"),
                        nuTitularidade = 1,
                    });
                }

                db.Close();
            }

            var ret = ctrl.Post(new DtoLoginInformation
            {
                empresa = "2",
                matricula = "1",
                codAcesso = "0864",
                venc = "0716",
                login = "",
                userType = TipoUsuario.Associado.ToString(),
                email = "",
                senha = "1234"
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();

            #endregion
        }

        [TestMethod]
        public void CartaoStatusNaoExpedido()
        {
            #region - code -

            SetupDatabase();
            var ctrl = SetupController();

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
                        nuMatricula = 1,
                        nuStatus = CartaoStatus.Habilitado,
                        nuEmitido = StatusExpedicao.NaoExpedido,
                        nuViaCartao = 1,
                        stVenctoCartao = "0716",
                        stSenha = SrvBaseService.DESCript("1234"),
                        nuTitularidade = 1,
                    });
                }

                db.Close();
            }

            var ret = ctrl.Post(new DtoLoginInformation
            {
                empresa = "2",
                matricula = "1",
                codAcesso = "0864",
                venc = "0716",
                login = "",
                userType = TipoUsuario.Associado.ToString(),
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
                        stVenctoCartao = "0711",
                        stSenha = SrvBaseService.DESCript("1234"),
                        nuTitularidade = 1,
                    });
                }

                db.Close();
            }

            var ret = ctrl.Post(new DtoLoginInformation
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
                    });
                }

                db.Close();
            }

            var ret = ctrl.Post(new DtoLoginInformation
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
                    });
                }

                db.Close();
            }

            var ret = ctrl.Post(new DtoLoginInformation
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
