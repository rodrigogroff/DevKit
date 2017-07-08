using System.Collections.Generic;

namespace DataModel
{
	public partial class TaskTypeAccumulator
	{
		public string sfkFlow = "";

		public List<LogAccumulatorValue> logs;
	}

    public class TaskTypeAccumulatorReport
    {
        public int count = 0;
        public List<TaskTypeAccumulator> results = new List<TaskTypeAccumulator>();
    }

    public class LogAccumulatorValue
	{
		public long id = 0;

		public string sfkUser = "",
			          sdtLog = "",
					  sValue = "";
	}
}
