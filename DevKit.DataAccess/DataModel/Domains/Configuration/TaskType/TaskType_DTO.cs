﻿using System.Collections.Generic;

namespace DataModel
{
	public partial class TaskType
	{
        public object anexedEntity;

		public string updateCommand = "";		

		public Project project;

		public List<TaskCategory> categories;
		public List<TaskTypeLog> logs;
		public List<TaskCheckPoint> checkpoints;
	}

    public class TaskTypeReport
    {
        public int count = 0;
        public List<TaskType> results = new List<TaskType>();
    }

    public class TaskTypeLog
	{
		public string sdtLog,
						stUser,
						stDetails;
	}
}
