using System;
using System.Collections;
using System.Globalization;

public partial class ISO8583
{
    private void desmonta_registro(string regISO)
    {
        int num1 = 1,
            num2 = 4,
            startIndex1 = 0,
            length1 = regISO.Length;

        this.m_codigo = regISO.Substring(0, 4);
        this.m_mapaBit1 = regISO.Substring(4, 16);
        
        string  str1 = "",
                str2 = "";

        while (startIndex1 < 16)
        {
            BitArray bitArray = new BitArray(BitConverter.GetBytes(int.Parse(this.m_mapaBit1.Substring(startIndex1, 2), NumberStyles.HexNumber)));
            for (int index = 0; index < 8; ++index)
            {
                str1 = !bitArray.Get(index) ? "0" + str1 : "1" + str1;
                ++num1;
            }
            str2 += str1;
            str1 = "";
            startIndex1 += 2;
        }

        int startIndex2 = num2 + 16;

        if (str2.Substring(0, 1) == "1")
        {
            this.m_mapaBit2 = regISO.Substring(20, 16);
            int startIndex3 = 0;
            while (startIndex3 < 16)
            {
                BitArray bitArray = new BitArray(BitConverter.GetBytes(int.Parse(this.m_mapaBit2.Substring(startIndex3, 2), NumberStyles.HexNumber)));
                for (int index = 0; index < 8; ++index)
                {
                    str1 = !bitArray.Get(index) ? "0" + str1 : "1" + str1;
                    ++num1;
                }
                str2 += str1;
                str1 = "";
                startIndex3 += 2;
            }
            startIndex2 += 16;
        }

        for (int startIndex3 = 0; startIndex3 < 128; ++startIndex3)
        {
            if (str2.Substring(startIndex3, 1) == "1")
                this.setBit(startIndex3 + 1);
        }

        for (int bitnum = 0; bitnum < 128; ++bitnum)
        {
            if (this.getBit(bitnum))
            {
                switch (bitnum)
                {
                    case 120:
                        if (startIndex2 + 3 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_bit120 = regISO.Substring(startIndex2, 3);
                        startIndex2 += 3;
                        int length2 = int.Parse(this.m_bit120);
                        if (startIndex2 + length2 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_bit120 = this.m_bit120 + regISO.Substring(startIndex2, length2);
                        startIndex2 += length2;
                        break;
                    case 125:
                        if (startIndex2 + 3 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        string s1 = regISO.Substring(startIndex2, 3);
                        startIndex2 += 3;
                        int length3 = int.Parse(s1);
                        if (startIndex2 + length3 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_bit125 = regISO.Substring(startIndex2, length3);
                        startIndex2 += length3;
                        break;
                    case (int)sbyte.MaxValue:
                        if (startIndex2 + 3 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        string s2 = regISO.Substring(startIndex2, 3);
                        startIndex2 += 3;
                        int length4 = int.Parse(s2);
                        if (startIndex2 + length4 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_bit127 = regISO.Substring(startIndex2, length4);
                        startIndex2 += length4;
                        break;
                    case 62:
                        if (startIndex2 + 3 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_bit62 = regISO.Substring(startIndex2, 3);
                        startIndex2 += 3;
                        int length5 = int.Parse(this.m_bit62);
                        if (startIndex2 + length5 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_bit62 = this.m_bit62 + regISO.Substring(startIndex2, length5);
                        startIndex2 += length5;
                        break;
                    case 63:
                        if (startIndex2 + 3 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_bit63 = regISO.Substring(startIndex2, 3);
                        startIndex2 += 3;
                        int length6 = int.Parse(this.m_bit62);
                        if (startIndex2 + length6 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_bit63 = this.m_bit63 + regISO.Substring(startIndex2, length6);
                        startIndex2 += length6;
                        break;
                    case 64:
                        if (startIndex2 + 3 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_bit64 = regISO.Substring(startIndex2, 3);
                        int startIndex3 = startIndex2 + 3;
                        int length7 = int.Parse(this.m_bit64);
                        if (startIndex3 + length7 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                        }
                        this.m_bit64 = this.m_bit64 + regISO.Substring(startIndex3, length7);
                        startIndex2 = startIndex3 + length7;
                        break;
                    case 90:
                        if (startIndex2 + 42 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_bit90 = regISO.Substring(startIndex2, 42);
                        startIndex2 += 42;
                        break;
                    case 35:
                        if (startIndex2 + 2 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        string s3 = regISO.Substring(startIndex2, 2);
                        startIndex2 += 2;
                        int length8 = int.Parse(s3);
                        if (startIndex2 + length8 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_trilha2 = regISO.Substring(startIndex2, length8);
                        startIndex2 += length8;
                        break;
                    case 37:
                        if (startIndex2 + 6 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_nsu = regISO.Substring(startIndex2, 6);
                        startIndex2 += 6;
                        break;
                    case 39:
                        if (startIndex2 + 2 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_codResposta = regISO.Substring(startIndex2, 2);
                        startIndex2 += 2;
                        break;
                    case 41:
                        if (startIndex2 + 8 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_terminal = regISO.Substring(startIndex2, 8);
                        startIndex2 += 8;
                        break;
                    case 42:
                        if (startIndex2 + 15 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_codLoja = regISO.Substring(startIndex2, 15);
                        startIndex2 += 15;
                        break;
                    case 49:
                        if (startIndex2 + 3 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_bit49 = regISO.Substring(startIndex2, 3);
                        startIndex2 += 3;
                        break;
                    case 52:
                        if (startIndex2 + 16 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_senha = regISO.Substring(startIndex2, 16);
                        startIndex2 += 16;
                        break;
                    case 53:
                        if (startIndex2 + 6 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_security = regISO.Substring(startIndex2, 6);
                        startIndex2 += 6;
                        break;
                    case 2:
                        if (startIndex2 + 19 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_PAN = regISO.Substring(startIndex2, 19);
                        startIndex2 += 19;
                        break;
                    case 3:
                        if (startIndex2 + 6 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_codProcessamento = regISO.Substring(startIndex2, 6);
                        startIndex2 += 6;
                        break;
                    case 4:
                        if (startIndex2 + 12 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_valor = regISO.Substring(startIndex2, 12);
                        startIndex2 += 12;
                        break;
                    case 7:
                        if (startIndex2 + 10 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_datetime = regISO.Substring(startIndex2, 10);
                        startIndex2 += 10;
                        break;
                    case 11:
                        if (startIndex2 + 6 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_nsuOrigem = regISO.Substring(startIndex2, 6);
                        startIndex2 += 6;
                        break;
                    case 12:
                        if (startIndex2 + 6 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_time = regISO.Substring(startIndex2, 6);
                        startIndex2 += 6;
                        break;
                    case 13:
                        if (startIndex2 + 4 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_date = regISO.Substring(startIndex2, 4);
                        startIndex2 += 4;
                        break;
                    case 14:
                        if (startIndex2 + 4 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_dateExp = regISO.Substring(startIndex2, 4);
                        startIndex2 += 4;
                        break;
                    case 22:
                        if (startIndex2 + 3 > length1)
                        {
                            LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
                            this.m_erro = true;
                            continue;
                        }
                        this.m_bit22 = regISO.Substring(startIndex2, 3);
                        startIndex2 += 3;
                        break;
                }
            }
        }
    }
}
