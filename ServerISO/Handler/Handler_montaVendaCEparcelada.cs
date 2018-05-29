

public partial class ClientHandler
{
    private string montaCNET_VendaCEparcelada(ref ISO8583 regIso)
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

            // original
            //string str2 = "05" + "CE" + "CE" + str1 + (regIso.trilha2.Trim().Length != 27 ? ("999999" + regIso.trilha2.Substring(17, 6) + regIso.trilha2.Substring(23, 6) + regIso.trilha2.Substring(29, 3)).PadLeft(27, '0') : regIso.trilha2.Trim()) + regIso.senha.PadLeft(16, '0') + regIso.valor.PadLeft(12, '0');

            if (regIso.trilha2 == "")
            {
                Log("trilha vazia!");
                return "";
            }

            // ajustado
            string str2 = "05CECE1" + codLoja.TrimStart('0').PadLeft(7, '0') +
                (regIso.trilha2.Trim().Length != 27 ? ("999999" + regIso.trilha2.Substring(17, 6) +
                regIso.trilha2.Substring(23, 6) + regIso.trilha2.Substring(29, 3)).PadLeft(27, '0') :
                regIso.trilha2.Trim()) + regIso.senha.PadLeft(16, '0') + regIso.valor.PadLeft(12, '0');

            //                             1         2         
            //                   012345678901234567890123456789
            //bit(35) - trilha = 826766001401000650011651018

            if (regIso.bit62 == "")
            {
                Log("BIT 62 vazio!");
                return "";
            }

            if (regIso.bit62.Length < 2)
            {
                Log("BIT 62 vazio!");
                return "";
            }

            if (regIso.bit62.Substring(0, 2) == "00")
                return "";

            int num1 = int.Parse(regIso.valor);
            int num2 = int.Parse(regIso.bit62.Substring(0, 2));
            int num3 = num1 / num2;
            int num4 = num2 * num3;
            int num5 = num1 - num4;
            int num6 = num3 + num5;
            string str3 = num2.ToString("00");
            for (int index = 0; index < num2; ++index)
                str3 = index != 0 ? str3 + num3.ToString("000000000000") : str3 + num6.ToString("000000000000");
            string registro = str2 + str3;

            // ajustado
            registro = registro.PadRight(200, '*') + str1 + regIso.nsuOrigem;

            regIso.bit62 = str3;
            
            return registro;
        }
        catch (System.Exception ex)
        {
            LogFalha(ex.ToString());
            return "";
        }
    }
}

