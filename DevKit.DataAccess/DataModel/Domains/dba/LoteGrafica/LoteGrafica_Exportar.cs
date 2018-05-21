using DataModel;
using LinqToDB;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace DataModel
{
	public partial class LoteGrafica
    {
		public string Exportar(DevKitDB db, string idLote)
		{
            var util = new Util();

            string dir = "c:\\lotes_grafica\\";
            string file = dir + "Lote_" + idLote + "_exp_arq.txt";

            using (var sw = new StreamWriter(file, false, Encoding.Default))
            {
                var query = from e in db.LoteGraficaCartao
                            where e.fkLoteGrafica.ToString() == idLote
                            select e;

                var lstAssoc = (from e in query
                                join assoc in db.Associado on e.fkAssociado equals assoc.id
                                select assoc).
                                ToList();

                var lstEmp = (from e in query
                                join emp in db.Empresa on e.fkEmpresa equals emp.id
                                select emp).
                                ToList();

                foreach (var item in query.ToList())
                {
                    var assoc = lstAssoc.Where(y => y.id == item.fkAssociado).FirstOrDefault();
                    var emp = lstEmp.Where(y => y.id == item.fkEmpresa).FirstOrDefault();

                    var secao = db.EmpresaSecao.Where (y=> y.id == assoc.fkSecao).FirstOrDefault();
                    var empresa = secao.nuEmpresa.ToString().PadLeft(6, '0');
                    var mat = assoc.nuMatricula.ToString().PadLeft(6, '0');
                    var nome = assoc.stName.PadRight(35, ' ').Substring(0, 35).TrimEnd();
                    var tit = assoc.nuTitularidade.ToString().PadLeft(2, '0');
                    var via = assoc.nuViaCartao.ToString().PadLeft(2, '0');

                    assoc.tgExpedicao = 1;

                    db.Update(assoc);

                    string line = "+";

                    line += nome + ",";
                    line += secao.nuEmpresa + ",";
                    line += mat + ",";
                    line += tit + "/" + via + ",";
                    line += util.calculaCodigoAcesso (empresa, mat, assoc.nuTitularidade.ToString(), assoc.nuViaCartao.ToString(), assoc.stCPF );
                    line += ",";
                    line += nome + ",";

                    if (assoc.nuDayAniversary != null && assoc.nuMonthAniversary != null && assoc.nuYearBirth != null)
                    {
                        line += assoc.nuDayAniversary.ToString().PadLeft(2, '0') + "/";
                        line += assoc.nuMonthAniversary.ToString().PadLeft(2, '0') + "/";
                        line += assoc.nuYearBirth.ToString() + ",";
                    }
                    else
                        line += ",";

                    if (assoc.nuMatSaude != null)
                        line += assoc.nuMatSaude.ToString();

                    line += "|";

                    line += "826766" + 
                            empresa +
                            mat +
                            item.nuTit.ToString() +
                            item.nuVia.ToString() +
                            "65" + tit + via;

                    line += "|";

                    sw.WriteLine(line);
                }
            }

            return file;
        }
	}
}
