
public partial class ClientHandler
{
    private string montaCNET_VendaCE(ISO8583 regIso)
    {
        try
        {
            string codLoja = regIso.codLoja;

            Log("CodEstabelecimento: " + codLoja);

            string terminal = regIso.terminal;

            Log("codigo Terminal : " + terminal);

            if (regIso.codLoja == "")
            {
                Log("codLoja vazio!");
                return "";
            }

            if (regIso.codLoja.Length < 4)
            {
                Log("codLoja menor de 4 chars!");
                return "";
            }

            if (regIso.terminal == "")
            {
                Log("terminal vazio!");
                return "";
            }

            if (regIso.terminal.Length < 4)
            {
                Log("terminal menor de 4 chars!");
                return "";
            }

            string s = terminal.Substring(terminal.Length - 4, 4);

            string str = (int.Parse(codLoja.Substring(codLoja.Length - 4, 4)) +
                            int.Parse(s)).ToString("00000000");
            
            // original
            /*string registro = "05" + "CE" + "CE" + str.PadLeft(8, '0') + 
                        (regIso.trilha2.Trim().Length != 0 ? (regIso.trilha2.Trim().Length != 27 ? 
                        ("999999" + regIso.trilha2.Substring(17, 6) + 
                        regIso.trilha2.Substring(23, 6) + 
                        regIso.trilha2.Substring(29, 3)).PadLeft(27, '0') : 
                        regIso.trilha2.Trim()) : "999999999999999999999999999") + regIso.senha.PadLeft(16, '0') + 
                        regIso.valor.PadLeft(12, '0') + "01" + 
                        regIso.valor.PadLeft(12, '0');
                        */

            // ajustado
            string registro = "05CECE1" + codLoja.TrimStart('0').PadLeft(7,'0') +
                    (regIso.trilha2.Trim().Length != 0 ? (regIso.trilha2.Trim().Length != 27 ?
                    ("999999" + regIso.trilha2.Substring(17, 6) +
                    regIso.trilha2.Substring(23, 6) +
                    regIso.trilha2.Substring(29, 3)).PadLeft(27, '0') :
                    regIso.trilha2.Trim()) : "999999999999999999999999999") + regIso.senha.PadLeft(16, '0') +
                    regIso.valor.PadLeft(12, '0') + "01" +
                    regIso.valor.PadLeft(12, '0');

            registro = registro.PadRight(200, '*') + str + regIso.nsuOrigem;
            
            return registro;
        }
        catch (System.Exception ex)
        {
            LogFalha("montaCNET_VendaCE exception: " + ex.ToString());
            return "";
        }            
    }
}
