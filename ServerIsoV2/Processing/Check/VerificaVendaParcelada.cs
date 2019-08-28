
namespace ServerIsoV2
{
    public partial class IsoCommand
    {
        public bool VerificaVendaParcelada(ISO8583 regIso)
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

                if (regIso.trilha2 == "")
                {
                    Log("trilha vazia!");
                    return false;
                }

                if (regIso.bit62 == "")
                {
                    Log("BIT 62 vazio!");
                    return false;
                }

                if (regIso.bit62.Length < 2)
                {
                    Log("BIT 62 vazio!");
                    return false;
                }

                if (regIso.bit62.Substring(0, 2) == "00")
                {
                    Log("BIT 62 zerado!");
                    return false;
                }

                return true;
            }
            catch (System.Exception ex)
            {
                LogFalha(ex.ToString());
                return false;
            }
        }
    }
}
