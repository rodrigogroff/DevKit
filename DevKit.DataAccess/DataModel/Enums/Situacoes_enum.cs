﻿using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
    public class Situacao
    {
        public long id { get; set; }
        public string stName { get; set; }
    }

    public class SituacaoReport
    {
        public int count;
        public List<Situacao> results;
    }

    public class EnumSituacao
    {
        public List<Situacao> lst = new List<Situacao>();

        public EnumSituacao()
        {
            lst.Add(new Situacao() { id = 1, stName = "Habilitado" });
            lst.Add(new Situacao() { id = 2, stName = "Bloqueado" });
            lst.Add(new Situacao() { id = 3, stName = "Cancelado" });
        }

        public Situacao Get(long _id)
        {
            return lst.Where(y => y.id == _id).FirstOrDefault();
        }
    }
}

