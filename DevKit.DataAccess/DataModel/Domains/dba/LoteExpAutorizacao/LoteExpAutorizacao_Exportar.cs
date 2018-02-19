using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DataModel
{
    public partial class LoteExpAutorizacao
    {
        public string Exportar(DevKitDB db, long? fkEmpresa, long? nuMes, long? nuAno)
        {
            var emp = db.Empresa.Where(y => y.id == fkEmpresa).FirstOrDefault();

            string tag = emp.nuEmpresa.ToString().PadLeft(6, '0') +
                         nuMes.ToString().PadLeft(2, '0') +
                         nuAno.ToString();

            var util = new Util();

            string dir = "c:\\lotes_grafica\\";
            string file = dir + "LoteExpAuto_" + tag + ".txt";
            
            var lstId = new List<long?>();

            using (var sw = new StreamWriter(file, false, Encoding.Default))
            {
                var lstAutorizacoes = db.Autorizacao.
                                        Where(y => y.fkEmpresa == fkEmpresa &&
                                                   y.nuMes == nuMes &&
                                                   y.nuAno == nuAno).
                                        OrderBy(y => y.dtSolicitacao).
                                        ToList();

                // header
                sw.WriteLine("empresa;mês;ano;data solicitação;portador;cpf portador;matricula;cpfCnpj credenciado;tuss;");
                
                lstId = lstAutorizacoes.Select(a => a.fkAssociado).Distinct().ToList();
                var lstAssoc = db.Associado.Where(y => lstId.Contains(y.id)).ToList();

                lstId = lstAssoc.Select(a => a.fkSecao).Distinct().ToList();
                var lstSecao = db.EmpresaSecao.Where(y => lstId.Contains(y.id)).ToList();

                lstId = lstAutorizacoes.Select(a => a.fkCredenciado).Distinct().ToList();
                var lstCreds = db.Credenciado.Where(y => lstId.Contains(y.id)).ToList();

                lstId = lstAutorizacoes.Select(a => a.fkProcedimento).Distinct().ToList();
                var lstProc = db.TUSS.Where(y => lstId.Contains(y.id)).ToList();

                foreach (var item in lstAutorizacoes)
                {
                    var assoc = lstAssoc.Where(y => y.id == item.fkAssociado).FirstOrDefault();
                    var secao = lstSecao.Where(y => y.id == assoc.fkSecao).FirstOrDefault();
                    var cred = lstCreds.Where (y=>y.id == item.fkCredenciado).FirstOrDefault();
                    var proc = lstProc.Where(y => y.id == item.fkProcedimento).FirstOrDefault();

                    var line = secao.nuEmpresa.ToString() + ";" + nuMes + ";" + nuAno + ";";

                    line += Convert.ToDateTime(item.dtSolicitacao).ToString("dd/MM/yyyy HH:mm") + ";";
                    line += assoc.stName + ";";
                    line += assoc.stCPF + ";";
                    line += assoc.nuMatricula + ";";
                    line += cred.stCnpj + ";";
                    line += proc.nuCodTUSS + ";";

                    sw.WriteLine(line);
                }
            }

            return file;
        }
    }
}
