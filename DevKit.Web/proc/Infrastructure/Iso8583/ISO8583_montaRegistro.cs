
public partial class ISO8583
{
    private string monta_registro()
    {
        string str = "" + this.m_codigo + this.mapaBit1;

        for (int bitnum = 0; bitnum < 128; ++bitnum)
        {
            if (this.getBit(bitnum))
            {
                switch (bitnum)
                {
                    case 120:
                        str += this.m_bit120;
                        break;
                    case 125:
                        int length1 = this.m_bit125.Length;
                        str = str + length1.ToString("000") + this.m_bit125;
                        break;
                    case (int)sbyte.MaxValue:
                        int length2 = this.m_bit127.Length;
                        str = str + length2.ToString("000") + this.m_bit127;
                        break;
                    case 62:
                        str += this.m_bit62;
                        break;
                    case 63:
                        str += this.m_bit63;
                        break;
                    case 64:
                        str += this.m_bit64;
                        break;
                    case 90:
                        str += this.m_bit90;
                        break;
                    case 35:
                        int length3 = this.m_trilha2.Length;
                        str = str + length3.ToString("00") + this.m_trilha2;
                        break;
                    case 37:
                        str += this.m_nsu;
                        break;
                    case 39:
                        str += this.m_codResposta;
                        break;
                    case 41:
                        str += this.m_terminal;
                        break;
                    case 42:
                        str += this.m_codLoja;
                        break;
                    case 49:
                        str += this.m_bit49;
                        break;
                    case 52:
                        str += this.m_senha;
                        break;
                    case 53:
                        str += this.m_security;
                        break;
                    case 1:
                        str += this.mapaBit2;
                        break;
                    case 2:
                        str += this.m_PAN;
                        break;
                    case 3:
                        str += this.m_codProcessamento;
                        break;
                    case 4:
                        str += this.m_valor;
                        break;
                    case 7:
                        str += this.m_datetime;
                        break;
                    case 11:
                        str += this.m_nsuOrigem;
                        break;
                    case 12:
                        str += this.m_time;
                        break;
                    case 13:
                        str += this.m_date;
                        break;
                    case 14:
                        str += this.m_dateExp;
                        break;
                    case 22:
                        str += this.m_bit22;
                        break;
                }
            }
        }
        return str;
    }
}
