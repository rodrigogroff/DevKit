using System.Collections.Generic;

namespace DataModel
{
	public partial class TaskTypeAccumulator
	{
		public string sfkFlow = "";

		public List<LogAccumulatorValue> logs = new List<LogAccumulatorValue>();
	}

	public class LogAccumulatorValue
	{
		public string sfkUser = "",
			          sdtLog = "",
					  sValue = "";
	}
}
