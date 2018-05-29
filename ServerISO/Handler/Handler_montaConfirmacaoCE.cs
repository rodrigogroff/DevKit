
public partial class ClientHandler
{
    private string montaConfirmacaoCE(ISO8583 regIso)
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
            string str1 = (int.Parse(codLoja.Substring(codLoja.Length - 4, 4)) + int.Parse(s)).ToString("00000000");

            string str2 = "";
            string str3 = regIso.bit127;

            if (regIso.bit62 == "")
            {
                Log("bit62 vazio!");
                return "";
            }

            if (regIso.bit127 == "")
            {
                Log("bit127 vazio!");
                return "";
            }

            if (regIso.bit127.Length < 4)
            {
                Log("bit127 menor de 4 chars!");
                return "";
            }

            if (str3 == "")
                str3 = "000000000";

            int length = regIso.trilha2.Trim().Length;

            string bit62 = regIso.bit62;

            if (regIso.bit62.Trim().Length == 27)
                str2 = regIso.bit62.Trim();
            else if (regIso.bit62.Trim().Length == 37)
                str2 = ("999999" +
                        regIso.bit62.Substring(17, 6) +
                        regIso.bit62.Substring(23, 6) +
                        regIso.bit62.Substring(29, 3)).PadLeft(27, '0');

            // original
            //string registro = "05" + "CE" + "CC" + str1.PadLeft(8, '0') + str2 + str3.Substring(3) + "00000" + str3;

            // ajustado
            string registro = "05CECC1" +
                                codLoja.TrimStart('0').PadLeft(7, '0') +
                                str2 +
                                str3.Substring(3) +
                                "00000" +
                                str3;

            registro = registro.PadRight(200, '*') + str1;

            return registro;
        }
        catch (System.Exception ex)
        {
            LogFalha("montaConfirmacaoCE exception: " + ex.ToString());
            return "";
        }
    }
}

