
public partial class ClientHandler
{
    private bool montaCancelamento(ISO8583 regIso)
    {
        try
        {
            if (regIso.codLoja == "")
            {
                Log("codLoja vazio!");
                return false;
            }

            if (regIso.codLoja.Length < 4)
            {
                Log("codLoja menor de 4 chars!");
                return false;
            }

            if (regIso.terminal == "")
            {
                Log("terminal vazio!");
                return false;
            }

            if (regIso.terminal.Length < 4)
            {
                Log("terminal menor de 4 chars!");
                return false;
            }

            if (regIso.bit125 == "")
            {
                Log("bit125 vazio!");
                return false;
            }

            if (regIso.bit125.Length < 4)
            {
                Log("bit125 menor de 4 chars!");
                return false;
            }

            return true;            
        }
        catch (System.Exception ex)
        {
            LogFalha("montaCancelamento exception: " + ex.ToString());
            return false;
        }
    }
}

