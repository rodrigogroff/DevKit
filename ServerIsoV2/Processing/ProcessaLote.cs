using Newtonsoft.Json;
using RestSharp;
using System;

namespace ServerIsoV2
{
    public partial class ServerISO
    {
        public char sepPacotes = '\0';

        public bool IsVerbose = false;

        public void ProcessaLote(IsoCommand cmd)
        {
            cmd.Ended = false;
            cmd.Running = true;

            var dadosRecebidos = cmd.Text;

            try
            {
                Console.WriteLine("[Processing] " + dadosRecebidos);

                if (dadosRecebidos.Length < 20)
                {
                    Console.WriteLine("[#Rejected!] ");
                }
                else
                {
                    cmd.Log("Dados recebidos =>" + dadosRecebidos + "<=");

                    var isoCode = dadosRecebidos.Substring(0, 4);

                    cmd.Log("isoCode " + isoCode);

                    if (isoCode != "0200" && isoCode != "0202" && isoCode != "0400" && isoCode != "0420")
                    {
                        cmd.Log("Código de processamento inválido!");
                    }
                    else
                    {
                        var regIso = new ISO8583(dadosRecebidos);

                        cmd.Log(regIso);

                        if (dadosRecebidos.StartsWith("0200"))
                        {
                            #region - processa venda -

                            var monta = !(regIso.codProcessamento == "002000") ?
                                                                        cmd.VerificaVendaParcelada(regIso) :
                                                                        cmd.VerificaVendaCE(regIso);

                            if (!monta)
                            {
                                #region - envia 210 EXPRESS com erro -

                                cmd.LogFalha(isoCode + " montaCNET_Venda falhou");

                                var Iso210 = new ISO8583
                                {
                                    codResposta = "06",
                                    nsuOrigem = regIso.nsuOrigem,
                                    codProcessamento = regIso.codProcessamento,
                                    codigo = "0210",
                                    valor = regIso.valor,
                                    terminal = regIso.terminal,
                                    codLoja = regIso.codLoja
                                };

                                cmd.Log(Iso210);
                                SendSincrono(Iso210.registro, cmd);

                                #endregion
                            }
                            else
                            {
                                #region - processa venda normal - 

                                var serviceClient = new RestClient(hostAPI);
                                var serviceRequest = new RestRequest("api/VendaServerISO", Method.PUT);

                                var dadosVenda = new VendaIsoInputDTO
                                {
                                    nu_nsuOrig = regIso.nsuOrigem,
                                    st_codLoja = regIso.codLoja,
                                    st_empresa = regIso.trilha2.Substring(6, 6),
                                    st_matricula = regIso.trilha2.Substring(12, 6),
                                    st_senha = regIso.senha,
                                    st_terminal = regIso.terminal,
                                    st_titularidade = regIso.trilha2.Substring(18, 2),
                                    nu_parcelas = regIso.codProcessamento == "002000" ? "01" : regIso.bit62.Substring(0, 2),
                                    st_valores = regIso.codProcessamento == "002000" ? regIso.valor.ToString() : regIso.bit62.Substring(2),
                                    vr_valor = regIso.valor.ToString(),
                                };

                                serviceRequest.AddJsonBody(dadosVenda);

                                var response = serviceClient.Execute(serviceRequest);
                                var retornoVenda = JsonConvert.DeserializeObject<VendaIsoOutputDTO>(response.Content);

                                if (retornoVenda == null)
                                {
                                    cmd.LogFalha("*** Falha na comunicação com a API ****");
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(retornoVenda.st_msg))
                                        retornoVenda.st_msg = "  ";

                                    cmd.Log("NSU: " + retornoVenda.st_nsuRcb);
                                    cmd.Log("Código resp: " + retornoVenda.st_codResp);
                                    cmd.Log("Mensagem na log trans: " + retornoVenda.st_msg);

                                    var Iso210 = new ISO8583
                                    {
                                        codigo = "0210",
                                        codResposta = retornoVenda.st_codResp,
                                        nsuOrigem = regIso.nsuOrigem,
                                        codProcessamento = regIso.codProcessamento,
                                        valor = regIso.valor,
                                        terminal = regIso.terminal,
                                        codLoja = regIso.codLoja,
                                        bit62 = dadosVenda.st_empresa +
                                                dadosVenda.st_matricula +
                                                dadosVenda.st_titularidade +
                                                retornoVenda.st_via +
                                                retornoVenda.st_nomeCliente.PadRight(40, ' '),
                                        bit127 = retornoVenda.st_nsuRcb.PadLeft(9, '0')
                                    };

                                    cmd.Log(Iso210);
                                    SendSincrono(Iso210.registro, cmd);
                                }

                                #endregion
                            }

                            #endregion
                        }
                        else if (dadosRecebidos.StartsWith("0202"))
                        {
                            #region - processa confirmação - 

                            cmd.Log("Conf de venda");

                            var monta = cmd.VerificaConfirmacaoCE(regIso);

                            cmd.Log("Conf de venda: (monta)" + monta);

                            if (!monta)
                                cmd.LogFalha(isoCode + " montaConfirmacaoCE falhou");
                            else
                            {
                                #region - prepara confirmação - 

                                cmd.Log("prepara confirmação");

                                var serviceClient = new RestClient(hostAPI);
                                var serviceRequest = new RestRequest("api/VendaConfServerISO", Method.PUT);

                                serviceRequest.AddJsonBody(new VendaConfIsoInputDTO
                                {
                                    st_nsu = regIso.bit127
                                });

                                var response = serviceClient.Execute(serviceRequest);

                                cmd.Log("confirmação pronta");

                                #endregion
                            }

                            #endregion
                        }
                        else if (dadosRecebidos.StartsWith("0400") || dadosRecebidos.StartsWith("0420"))
                        {
                            #region - processa cancelamento / desfazimento - 

                            var codigoIso = "";
                            var montar = false;

                            if (isoCode == "0400")
                            {
                                codigoIso = "0410";
                                montar = cmd.VerificaCancelamento(regIso);
                            }
                            else if (isoCode == "0420")
                            {
                                codigoIso = "0430";
                                montar = cmd.VerificaDesfazimento(regIso);
                            }

                            cmd.Log("codigoIso " + codigoIso);

                            if (!montar)
                            {
                                #region - erro - 

                                cmd.LogFalha(isoCode + " montaDesfazimento / montaCancelamento falhou");

                                if (codigoIso == "0402")
                                {
                                    #region - erro cancelamento -

                                    var isoErro = new ISO8583
                                    {
                                        codResposta = "06",
                                        codigo = codigoIso,
                                        codProcessamento = regIso.codProcessamento,
                                        codLoja = regIso.codLoja,
                                        terminal = regIso.terminal,
                                        nsuOrigem = regIso.nsuOrigem,
                                    };

                                    cmd.Log(isoErro);

                                    Send(isoErro.registro, cmd);

                                    #endregion
                                }
                                else
                                {
                                    #region - erro desfazimento -

                                    var IsoErro = new ISO8583
                                    {
                                        codigo = codigoIso,
                                        codResposta = "06",
                                        nsuOrigem = regIso.nsuOrigem,
                                        valor = regIso.valor,
                                        terminal = regIso.terminal,
                                        codLoja = regIso.codLoja,
                                        bit62 = regIso.nsuOrigem.PadLeft(6, '0') + regIso.valor.PadLeft(12, '0')
                                    };

                                    cmd.Log(IsoErro);

                                    Send(IsoErro.registro, cmd);

                                    #endregion
                                }

                                #endregion
                            }
                            else
                            {
                                if (codigoIso == "0410")
                                {
                                    #region - cancelamento -

                                    cmd.Log("cancelamento " + regIso.bit125);

                                    var serviceClient = new RestClient(hostAPI);
                                    var serviceRequest = new RestRequest("api/VendaCancServerISO", Method.PUT);

                                    serviceRequest.AddJsonBody(new VendaCancIsoInputDTO
                                    {
                                        st_nsu = regIso.bit125
                                    });

                                    var response = serviceClient.Execute(serviceRequest);
                                    var resp = JsonConvert.DeserializeObject<VendaCancIsoOutputDTO>(response.Content);

                                    if (resp == null)
                                    {
                                        cmd.LogFalha("*** Falha na comunicação com a API ****");
                                    }
                                    else
                                    {
                                        var isoRegistro = new ISO8583
                                        {
                                            codigo = codigoIso,
                                            codProcessamento = regIso.codProcessamento,
                                            codLoja = regIso.codLoja,
                                            terminal = regIso.terminal,
                                            codResposta = resp.st_codResp,
                                            bit127 = regIso.bit125.PadLeft(6, '0') + regIso.valor.PadLeft(12, '0'),
                                            nsuOrigem = regIso.nsuOrigem,
                                        };

                                        cmd.Log(isoRegistro);

                                        SendSincrono(isoRegistro.registro, cmd);

                                        cmd.Log("cancelamento " + regIso.bit125 + " enviado!");
                                    }

                                    #endregion
                                }
                                else
                                {
                                    #region - desfazimento - 

                                    var serviceClient = new RestClient(hostAPI);
                                    var serviceRequest = new RestRequest("api/VendaDesfazServerISO", Method.PUT);

                                    serviceRequest.AddJsonBody(new VendaDesfazIsoInputDTO
                                    {
                                        st_nsu = regIso.nsuOrigem
                                    });

                                    var response = serviceClient.Execute(serviceRequest);
                                    var resp = JsonConvert.DeserializeObject<VendaDesfazIsoOutputDTO>(response.Content);

                                    if (resp == null)
                                    {
                                        cmd.LogFalha("*** Falha na comunicação com a API ****");
                                    }
                                    else
                                    {
                                        cmd.Log("Desfazimento " + regIso.nsuOrigem);

                                        var Iso430 = new ISO8583
                                        {
                                            codigo = codigoIso,
                                            codResposta = resp.st_codResp.Substring(0, 2),
                                            nsuOrigem = regIso.nsuOrigem,
                                            valor = regIso.valor,
                                            terminal = regIso.terminal,
                                            codLoja = regIso.codLoja,
                                            bit62 = regIso.nsuOrigem.PadLeft(6, '0') + regIso.valor.PadLeft(12, '0')
                                        };

                                        cmd.Log(Iso430);

                                        SendSincrono(Iso430.registro, cmd);

                                        cmd.Log("Desfazimento " + regIso.nsuOrigem + " enviado!");
                                    }

                                    #endregion
                                }
                            }

                            #endregion
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("FALHA:" + ex.ToString());

                cmd.LogFalha(ex.ToString());
            }
            
            WaitMessage(cmd);
        }
    }
}
