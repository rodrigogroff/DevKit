﻿using System.Collections.Generic;

namespace DataModel
{
	public partial class TUSS
    {
        public object anexedEntity;

        public string updateCommand = "";
	}

    public partial class MedicoEmpresaTuss
    {
        public string stProcedimento;
    }

    public class TUSSReport
    {
        public int count = 0;
        public List<TUSS> results = new List<TUSS>();
    }
}
