using System;

namespace DevKit.Web.Controllers
{
    public class CodigoAcesso
    {
        public string Obter(string empresa, string matricula, string titularidade, int? via, string cpf)
        {
            return Obter ( empresa, 
                           matricula, 
                           Convert.ToInt32(titularidade), 
                           Convert.ToInt32(via), cpf);
        }

        public string Obter (string empresa, string matricula, int titularidade, int via, string cpf) 
        {
            string chave = matricula + empresa + titularidade.ToString().PadLeft(2, '0') + via + cpf.PadRight(14, ' ');
            int temp = 0;
            for (int n = 0; n<chave.Length; n++)
            {
                string s = chave.Substring(n, 1);
                char c = s[0]; // First character in s
                int i = c; // ascii code
                temp += i;
            }
            string calculado = temp.ToString("0000");

            temp += int.Parse(calculado[3].ToString() ) * 1000;
            temp += int.Parse(calculado[2].ToString() );
            if (temp > 9999) temp -= 2000;
            calculado = temp.ToString("0000");
            calculado = calculado.Substring( 2, 1 ) + 
                        calculado.Substring( 0, 1 ) + 
                        calculado.Substring( 3, 1 ) + 
                        calculado.Substring( 1, 1 );
            return calculado;
        }
    }
}
