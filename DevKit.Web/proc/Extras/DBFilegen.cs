using System;
using System.Collections;
using System.Data.OleDb;
using System.IO;

public class DBfileGen
{
    private string[] dbNomeCampo = new string[100];
    private string[] dbFormato = new string[100];
    private int qtdCampos = 0;
    private int qtdRegistros = 0;
    private ArrayList registros = new ArrayList();
    private string m_diretorio = "";
    private string m_file = "";
    private string m_tipoDB = "";

    // Construtor da Classe
    public DBfileGen(string diretorio, string file, string tipodir)
    {
        m_tipoDB = tipodir;
        m_diretorio = diretorio;
        m_file = file;
    }

    // adiciona registros a serem gravados no arquivo
    public bool addReg(ArrayList campos)
    {
        for (int n = 0; n < campos.Count; n++)
            registros.Add(campos[n]);

        qtdRegistros++;
        return true;
    }

    // adiciona campo e formato correspondente  dos registros
    public bool addCampo(string nome, string tipo)
    {
        if (qtdCampos > 99)
            return false;
        dbNomeCampo[qtdCampos] = nome;
        dbFormato[qtdCampos] = tipo.ToUpper();
        qtdCampos++;
        return true;
    }

    // salva registros no DBF
    public bool save()
    {
        string inputDir = @m_diretorio;
        string strConn = "";
        // verifica se arquivo existe, se existe eu apago
        if (File.Exists(m_diretorio + @"\\" + m_file + "." + m_tipoDB)) // apaga arquivo se existir
        {
            File.Delete(m_diretorio + @"\\" + m_file + "." + m_tipoDB);
        }
        if (m_tipoDB == "dbf")
        {
            strConn = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + inputDir + ";Extended Properties=dBASE IV";
        }
        else if (m_tipoDB == "xls")
        {
            strConn = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + inputDir + @"\" + m_file + ".xls;Extended Properties=" + (char)34 + "Excel 8.0;HDR=Yes;" + (char)34;
        }
        else
            return false;
        // crio a tabela  e registro de inclusao inicial
        string insert = " insert into " + m_file + " ( ";
        string cmdScript = @" create table " + m_file + " ( ";
        for (int n = 0; n < qtdCampos; n++)
        {
            cmdScript += dbNomeCampo[n] + " " + dbFormato[n];
            insert += dbNomeCampo[n];
            if (n < qtdCampos - 1)
            {
                cmdScript += ", ";
                insert += ", ";
            }
        }
        cmdScript += ")";
        insert += ") values (";
        try
        {
            OleDbConnection cn = new OleDbConnection(strConn);
            cn.Open();
            OleDbCommand cmd = cn.CreateCommand();

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = cmdScript;
            cmd.ExecuteNonQuery();
            string insertPlus = insert;
            int work = 0;
            // cria registro a registro
            for (int n = 0; n < registros.Count; n++)
            {
                if (dbFormato[work].IndexOf("DATE") > -1 ||
                     dbFormato[work].IndexOf("TEXT") > -1 ||
                     dbFormato[work].IndexOf("CHAR") > -1 ||
                     dbFormato[work].IndexOf("STRING") > -1 ||
                     dbFormato[work].IndexOf("VARCHAR") > -1)
                {
                    insertPlus += "\"" + registros[n].ToString() + "\", ";
                }
                if (dbFormato[work].IndexOf("CURRENCY") > -1 ||
                     dbFormato[work].IndexOf("NUMERIC") > -1)
                {
                    string stNum = registros[n].ToString().Replace(',', '.');
                    insertPlus += stNum + ", ";
                }
                work++;
                if (work == qtdCampos)
                {
                    work = 0;
                    insertPlus = insertPlus.Remove(insertPlus.Length - 2); // tira virgula adicional
                    insertPlus += ")";
                    cmd.CommandText = insertPlus;
                    cmd.ExecuteNonQuery();
                    insertPlus = insert;
                }
            }
            cn.Close();
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }
}
