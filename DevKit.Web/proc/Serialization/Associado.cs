
using System.Collections.Generic;

namespace DevKit.Web.Controllers
{
    public class Associado
    {
        public string id = "",
                      nome = "",
                      dispMensal = "",
                      dispTotal = "",
                      dispExtra = "",
                      email = "",
                      maxParcelasEmpresa = "";

        public List<string> lstParcelas = new List<string>();

        public bool bloqueado = false;
    }
}