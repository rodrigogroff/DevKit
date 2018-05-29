
public partial class ClientHandler
{
    private string montaCancelamento(ISO8583 regIso, string trilha)
    {
        try
        {
            string codLoja = regIso.codLoja;
            string terminal = regIso.terminal;

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
            string str = (int.Parse(codLoja.Substring(codLoja.Length - 4, 4)) + int.Parse(s)).ToString("00000000");
            
            // original
            //string registro = "05" + "CE" + "CA" + str.PadLeft(8, '0') + trilha + regIso.bit125.Substring(3) + "00000" + regIso.bit125;

            if (regIso.bit125 == "")
            {
                Log("bit125 vazio!");
                return "";
            }

            if (regIso.bit125.Length < 4)
            {
                Log("bit125 menor de 4 chars!");
                return "";
            }

            // ajustado 
            string registro = "05CECA1" +
                                codLoja.TrimStart('0').PadLeft(7, '0') +
                                trilha +
                                regIso.bit125.Substring(3) +
                                "00000" + regIso.bit125;

            registro = registro.PadRight(200, '*') + str;

            Log(registro);

            return registro;
        }
        catch (System.Exception ex)
        {
            LogFalha("montaCancelamento exception: " + ex.ToString());
            return "";
        }
    }
}

