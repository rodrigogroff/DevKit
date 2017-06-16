using System;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Setup
	{
        public List<SetupLog> logs;
	}

	public class SetupLog
	{
		public string sdtLog,
						stUser,
						stDetails;
	}
}
