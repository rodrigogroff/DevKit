// Decompiled with JetBrains decompiler
// Type: ISO8583
// Assembly: ConveyISO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A1A29DB8-D4AD-4F47-B8FA-3FADEED7E861
// Assembly location: C:\Users\rodrigo.groff\Desktop\ciso\ConveyISO.exe

using System;
using System.Collections;
using System.Globalization;

public partial class ISO8583
{
    private BitArray mBits1 = new BitArray(65),
                     mBits2 = new BitArray(65);

    public string strErro = "";

    private bool m_erro = false;

    private string m_registro = " ",
                   m_bit22 = "",
                   m_bit62 = "",
                   m_bit63 = "",
                   m_bit64 = "",
                   m_bit120 = "",
                   m_bit49 = "",
                   m_bit90 = "",
                   m_bit127 = "",
                   m_bit125 = "",
                   m_mapaBit1 = "",
                   m_mapaBit2 = "",
                   m_trilha2 = "",
                   m_valor = "",
                   m_terminal = "",
                   m_codLoja = "",
                   m_codigo = "",
                   m_codProcessamento = "",
                   m_PAN = "",
                   m_datetime = "",
                   m_time = "",
                   m_date = "",
                   m_dateExp = "",
                   m_security = "",
                   m_codResposta = "",
                   m_senha = "",
                   m_nsuOrigem = "",
                   m_nsu = "";

    public string codigo
    {
        get
        {
            return this.m_codigo;
        }
        set
        {
            this.m_codigo = value.PadLeft(4, '0');
        }
    }

    public bool erro
    {
        get
        {
            return this.m_erro;
        }
        set
        {
        }
    }

    public string mapaBit1
    {
        get
        {
            this.m_mapaBit1 = "";
            int num = 0;
            int bitnum = 1;
            for (int index1 = 0; index1 < 8; ++index1)
            {
                for (int index2 = 0; index2 < 8; ++index2)
                {
                    if (this.getBit(bitnum))
                        num += (int)Math.Pow(2.0, (double)(7 - index2));
                    ++bitnum;
                }
                this.m_mapaBit1 = this.m_mapaBit1 + string.Format("{0:X2}", (object)num);
                num = 0;
            }
            return this.m_mapaBit1;
        }
        set
        {
        }
    }

    public string mapaBit2
    {
        get
        {
            this.m_mapaBit2 = "";
            int num1 = 0;
            int num2 = 1;
            for (int index1 = 0; index1 < 8; ++index1)
            {
                for (int index2 = 0; index2 < 8; ++index2)
                {
                    if (this.getBit(num2 + 64))
                        num1 += (int)Math.Pow(2.0, (double)(7 - index2));
                    ++num2;
                }
                this.m_mapaBit2 = this.m_mapaBit2 + string.Format("{0:X2}", (object)num1);
                num1 = 0;
            }
            return this.m_mapaBit2;
        }
        set
        {
        }
    }

    public string PAN
    {
        get
        {
            return this.m_PAN;
        }
        set
        {
            this.setBit(2);
            this.m_PAN = value.PadLeft(19, '0');
        }
    }

    public string codProcessamento
    {
        get
        {
            return this.m_codProcessamento;
        }
        set
        {
            this.setBit(3);
            this.m_codProcessamento = value.PadLeft(6, '0');
        }
    }

    public string valor
    {
        get
        {
            return this.m_valor;
        }
        set
        {
            this.setBit(4);
            this.m_valor = value.PadLeft(12, '0');
        }
    }

    public string datetime
    {
        get
        {
            return this.m_datetime;
        }
        set
        {
        }
    }

    public string Time
    {
        get
        {
            return this.m_time;
        }
        set
        {
        }
    }

    public string Date
    {
        get
        {
            return this.m_date;
        }
        set
        {
        }
    }

    public string DateExp
    {
        get
        {
            return this.m_dateExp;
        }
        set
        {
            this.setBit(14);
            this.m_dateExp = value.PadLeft(4, '0');
        }
    }

    public string bit22
    {
        get
        {
            return this.m_bit22;
        }
        set
        {
            this.setBit(22);
            this.m_bit22 = value.PadRight(3, '0');
        }
    }

    public string trilha2
    {
        get
        {
            return this.m_trilha2;
        }
        set
        {
            this.setBit(35);
            this.m_trilha2 = value;
        }
    }

    public string codResposta
    {
        get
        {
            return this.m_codResposta;
        }
        set
        {
            this.setBit(39);
            this.m_codResposta = value.PadLeft(2, '0');
        }
    }

    public string terminal
    {
        get
        {
            return this.m_terminal;
        }
        set
        {
            this.setBit(41);
            this.m_terminal = value.PadLeft(8, '0');
        }
    }

    public string codLoja
    {
        get
        {
            return this.m_codLoja;
        }
        set
        {
            this.setBit(42);
            this.m_codLoja = value.PadLeft(15, '0');
        }
    }

    public string senha
    {
        get
        {
            return this.m_senha;
        }
        set
        {
            this.setBit(52);
            this.m_senha = value.PadLeft(16, '0');
        }
    }

    public string securityControl
    {
        get
        {
            return this.m_security;
        }
        set
        {
            this.setBit(53);
            this.m_security = value.PadLeft(6, '0');
        }
    }

    public string bit62
    {
        get
        {
            if (this.m_bit62.Length > 3)
                return this.m_bit62.Substring(3);
            return this.m_bit62;
        }
        set
        {
            this.setBit(62);
            this.m_bit62 = value.Length.ToString("000") + value;
        }
    }

    public string bit63
    {
        get
        {
            if (this.m_bit63.Length > 3)
                return this.m_bit63.Substring(3);
            return this.m_bit63;
        }
        set
        {
            this.setBit(63);
            this.m_bit63 = value.Length.ToString("000") + value;
        }
    }

    public string bit64
    {
        get
        {
            if (this.m_bit64.Length > 3)
                return this.m_bit64.Substring(3);
            return this.m_bit64;
        }
        set
        {
            this.setBit(64);
            this.m_bit64 = value.Length.ToString("000") + value;
        }
    }

    public string bit49
    {
        get
        {
            return this.m_bit49;
        }
        set
        {
            this.setBit(49);
            this.m_bit49 = value.PadLeft(3, '0');
        }
    }

    public string bit90
    {
        get
        {
            return this.m_bit90;
        }
        set
        {
            this.setBit(90);
            this.m_bit90 = value.PadRight(42, '0');
        }
    }

    public string bit120
    {
        get
        {
            if (this.m_bit120.Length > 3)
                return this.m_bit120.Substring(3);
            return this.m_bit120;
        }
        set
        {
            this.setBit(120);
            this.m_bit120 = value.Length.ToString("000") + value;
        }
    }

    public string registro
    {
        get
        {
            return this.monta_registro();
        }
        set
        {
            this.m_registro = value;
            this.desmonta_registro(value);
        }
    }

    public string relacaoBits
    {
        get
        {
            string str = "";
            for (int bitnum = 0; bitnum < 128; ++bitnum)
            {
                if (this.getBit(bitnum))
                    str = str + bitnum.ToString() + ",";
            }
            if (str.Length > 0)
                str = str.Substring(0, str.Length - 1);
            return str;
        }
        set
        {
        }
    }

    public string bit125
    {
        get
        {
            return this.m_bit125;
        }
        set
        {
            this.setBit(125);
            this.m_bit125 = value;
        }
    }

    public string bit127
    {
        get
        {
            return this.m_bit127;
        }
        set
        {
            this.setBit((int)sbyte.MaxValue);
            this.m_bit127 = value;
        }
    }

    public string nsuOrigem
    {
        get
        {
            return this.m_nsuOrigem;
        }
        set
        {
            this.setBit(11);
            this.m_nsuOrigem = value.PadLeft(6, '0');
        }
    }

    public string nsu
    {
        get
        {
            return this.m_nsu;
        }
        set
        {
            this.setBit(37);
            this.m_nsu = value.PadLeft(6, '0');
        }
    }

    public ISO8583()
    {
        this.limpa();
    }

    public ISO8583(string reg)
    {
        this.limpa();
        this.registro = reg;
    }

    private void setBit(int bitnum)
    {
        if (bitnum > 128)
            return;
        if (bitnum < 65)
            this.mBits1.Set(bitnum, true);
        else
            this.mBits2.Set(bitnum - 64, true);
    }

    private void unsetBit(int bitnum)
    {
        if (bitnum > 128)
            return;
        if (bitnum < 65)
            this.mBits1.Set(bitnum, false);
        else
            this.mBits2.Set(bitnum - 64, false);
    }

    private void limpaBit(int bitnum)
    {
        if (bitnum > 128)
            return;
        if (bitnum < 65)
            this.mBits1.Set(bitnum, false);
        else
            this.mBits2.Set(bitnum - 64, false);
    }

    private bool getBit(int bitnum)
    {
        if (bitnum > 128)
            return false;
        if (bitnum < 65)
            return this.mBits1.Get(bitnum);
        return this.mBits2.Get(bitnum - 64);
    }
    
    public void limpa()
    {
        for (int index = 0; index < 128; ++index)
            this.unsetBit(index + 1);
        this.m_codigo = "0000";
        this.m_codProcessamento = "000000";
        this.m_PAN = "0000000000000000000";
        this.m_datetime = DateTime.Now.ToString("MMddHHmmss");
        DateTime now = DateTime.Now;
        this.m_date = now.ToString("MMdd");
        now = DateTime.Now;
        this.m_time = now.ToString("HHmmss");
        this.m_codResposta = "00";
        this.m_senha = "                ";
        this.m_security = "";
        this.m_dateExp = "";
        this.m_terminal = "00000000";
        this.m_bit22 = "";
        this.m_bit62 = "";
        this.m_bit64 = "";
        this.m_codLoja = "";
        this.setBit(1);
        this.setBit(7);
        this.setBit(12);
        this.setBit(13);
    }
}
