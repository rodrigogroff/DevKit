namespace ServerIsoV2
{
    public partial class IsoCommand
    {
        public bool montaConfirmacaoCE(ISO8583 regIso)
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

                if (regIso.bit62 == "")
                {
                    Log("bit62 vazio!");
                    return false;
                }

                if (regIso.bit127 == "")
                {
                    Log("bit127 vazio!");
                    return false;
                }

                if (regIso.bit127.Length < 4)
                {
                    Log("bit127 menor de 4 chars!");
                    return false;
                }

                return true;
            }
            catch (System.Exception ex)
            {
                LogFalha("montaConfirmacaoCE exception: " + ex.ToString());
                return false;
            }
        }
    }
}
