using Api.Master.Controllers;
using Master.Data.Const;
using Master.Data.Domains.Associado;
using Master.Infra;
using Master.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using System;

namespace IntegrationTest
{
    [TestClass]
    public class TstCtrlLimites : TstBaseIntegration 
    {
        CtrlLimites SetupController(string token)
        {
            var ret = new CtrlLimites(null,null)
            { 
                network = new LocalNetwork
                {
                    sqlServer = connStr,
                },                
            };

            ret._testToken = token;

            return ret;
        }

        [TestMethod]
        public void valoresPadrao()
        {
            #region - code - 

            SetupDatabase();
            SetupLoginOK(0, 10000, 100000);

            var token = Login();

            var ctrl = SetupController(token);

            ctrl._doNotSendEmail = true;
            ctrl._doNotUseCache = true;

            var ret = ctrl.associadoLimites();
            
            if (ret.ToString().Contains("BadRequest"))
                Assert.Fail();

            var retDto = (ret as OkObjectResult).Value as DtoAssociadoLimites;

            if (retDto.cotaExtra != "R$ 0,00")
                Assert.Fail();

            if (retDto.limiteMensalDisp != "R$ 100,00")
                Assert.Fail();

            if (retDto.mensalUtilizado != "R$ 0,00")
                Assert.Fail();

            #endregion
        }

        [TestMethod]
        public void valoresComVenda()
        {
            #region - code - 

            SetupDatabase();
            SetupLoginOK(0, 10000, 100000);

            #region - bd-setup - 

            using (var db = new NpgsqlConnection(connStr))
            {
                db.Open();

                {
                    var da = new LojaDapperRepository();

                    da.InsertLoja(db, new Master.Data.Database.Loja
                    {
                        bBlocked = false,
                        bCancel = false,
                        bIsentoFat = false,
                        bPortalSenha = false,
                        fkBanco = null,
                        stLoja ="123456",
                        stNome = "Loja de testes",                        
                    });

                    da.InsertTerminal(db, new Master.Data.Database.Terminal
                    {
                        fkLoja = 1,
                        stLocal = "teste",
                        stTerminal = "1"
                    });
                }

                {
                    var da = new LogTransacaoDapperRepository();

                    da.InsertLogTransacao(db, new Master.Data.Database.LogTransacao
                    {
                        bContabil = true,
                        dtTransacao = DateTime.Now,
                        fkCartao = 1,
                        fkEmpresa = 1,
                        fkLoja = 1,
                        fkTerminal = 1,
                        nuCodErro = 0,
                        nuConfirmada = TipoConfirmacao.Confirmada,
                        nuNsu = 101010,
                        nuNsuOrig = 1,
                        nuOperacao = 1,
                        nuParcelas = 1,
                        stDoc = "",
                        stMsg = "",
                        vrTotal = 100,
                    });
                }

                {
                    var da = new ParcelaDapperRepository();

                    da.InsertParcela(db, new Master.Data.Database.Parcela
                    {
                        dtInclusao = DateTime.Now,
                        fkCartao = 1,
                        fkEmpresa = 1,
                        fkLogTransacao = 1,
                        fkLoja = 1,
                        fkTerminal = 1,
                        nuIndice = 1,
                        nuNsu = 101010,
                        nuParcela = 1,
                        nuTotParcelas = 1,
                        vrValor = 100
                    });
                }

                db.Close();
            }

            #endregion

            var token = Login();

            var ctrl = SetupController(token);

            ctrl._doNotSendEmail = true;
            ctrl._doNotUseCache = true;

            var ret = ctrl.associadoLimites();

            if (ret.ToString().Contains("BadRequest"))
                Assert.Fail();

            var retDto = (ret as OkObjectResult).Value as DtoAssociadoLimites;

            if (retDto.cotaExtra != "R$ 0,00")
                Assert.Fail();

            if (retDto.limiteMensalDisp == "R$ 100,00")
                Assert.Fail();

            if (retDto.mensalUtilizado != "R$ 1,00")
                Assert.Fail();

            #endregion
        }
    }
}
