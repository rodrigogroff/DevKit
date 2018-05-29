
public partial class ClientHandler
{
    private string montaDesfazimento(ISO8583 regIso, bool novo)
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
            //string registro = "05" + "CE" + "DF" + str.PadLeft(8, '0') + "0".PadLeft(43, '0') + regIso.valor.PadLeft(12, '0');

            // ajustado
            string registro = "05CEDF";

            if (novo)
                registro += "1" + codLoja.TrimStart('0').PadLeft(7, '0') + "0".PadLeft(43, '0') + regIso.valor.PadLeft(12, '0');
            else
                registro += "0" + str.PadLeft(8, '0') + "0".PadLeft(43, '0') + regIso.valor.PadLeft(12, '0');
            
            registro = registro.PadRight(200, '*') + str + regIso.nsuOrigem.TrimStart('0').PadLeft(8, '0');

            Log(registro);

            return registro;
        }
        catch (System.Exception ex)
        {
            LogFalha("montaDesfazimento exception: " + ex.ToString());
            return "";
        }
    }
}
