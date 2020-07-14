using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public partial class T_Empresa
    {
        public string svrMensalidade = "",
                        svrCartaoAtivo = "",
                        svrMinimo = "",
                        svrTransacao = "",
                        snuFranquia = "",
                        sfechFinalizado = "",
                        sDtFech = "",
                        sfechCartoes = "",
                        sfechValorTotal = "",
                        sultimo = "",
                        svariacao = "";

        public List<T_JobFechamento> lstFechamento = new List<T_JobFechamento>();
    }

    public partial class T_JobFechamento
    {
        public string   sdt_inicio = "",
                        sdt_fim = "",
                        sfechCartoes = "",
                        sfechValorTotal = "";
    }
}
