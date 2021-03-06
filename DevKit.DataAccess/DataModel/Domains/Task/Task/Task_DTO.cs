﻿using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
        public object anexedEntity;

		public string	sdtStart = "",
						snuPriority = "",
						sfkUserStart = "",
						sfkUserResponsible = "",
						sfkTaskType = "",
						sfkTaskCategory = "",
						sfkTaskFlowCurrent = "",
						sfkProject = "",
						sfkPhase = "",
						sfkSprint = "",
						sfkVersion = "",
		                stUserMessage = "",
						fkNewFlow_Message = "",
						updateCommand = "";

		public long? fkNewFlow = null;

        public List<TaskLog> logs;
        public List<TaskProgress> usrProgress;
		public List<TaskMessage> usrMessages;
		public List<TaskFlowChange> flows;
		public List<TaskTypeAccumulator> accs;		
		public List<TaskDependency> dependencies;
		public List<TaskCheckPoint> checkpoints;
		public List<TaskQuestion> questions;
		public List<TaskClient> clients;
		public List<TaskClientGroup> clientGroups;
        public List<TaskCustomStep> customSteps;
    }
    
    public class TaskListing
    {
        public string   id,
                        stProtocol, 
                        sdtStart,
                        stTitle, 
                        stLocalization, 
                        sfkProject, 
                        sfkPhase, 
                        sfkSprint, 
                        sfkVersion, 
                        snuPriority, 
                        sfkTaskFlowCurrent, 
                        sfkUserStart,
                        sfkUserResponsible, 
                        sfkTaskType, 
                        sfkTaskCategory;
    }

    public class TaskReport
    {
        public int count = 0;
        public List<TaskListing> results = new List<TaskListing>();
    }
}
