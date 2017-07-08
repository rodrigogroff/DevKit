using System;

namespace DevKit.Web.Controllers
{
    public class CacheHitRecord
    {
        public DateTime dt_last;
        public int hits = 0;
    }

    public static class CacheTags
    {
        // enums

        public const string EnumAccumulatorType = "EnumAccumulatorType";
        public const string EnumAccumulatorTypeReport = "EnumAccumulatorTypeReport";
        public const string EnumMonth = "EnumMonth";
        public const string EnumContactForm = "EnumContactForm";
        public const string EnumMonthReport = "EnumMonthReport";
        public const string EnumContactFormReport = "EnumContactFormReport";
        public const string EnumPriority = "EnumPriority";
        public const string EnumPriorityReport = "EnumPriorityReport";
        public const string EnumProjectTemplate = "EnumProjectTemplate";
        public const string EnumProjectTemplateReport = "EnumProjectTemplateReport";
        public const string EnumVersionState = "EnumVersionState";
        public const string EnumVersionStateReport = "EnumVersionStateReport";

        // combos

        public const string ClientComboReport = "ClientComboReport";
        public const string ClientCombo = "ClientCombo";
        public const string ClientGroupComboReport = "ClientComboReport";
        public const string ClientGroupCombo = "ClientCombo";
        public const string ProjectCombo = "ProjectCombo";
        public const string ProjectComboReport = "ProjectComboReport";
        public const string UserCombo = "UserCombo";
        public const string UserComboReport = "UserComboReport";
        public const string ProjectPhaseCombo = "ProjectPhaseCombo";
        public const string ProjectPhaseComboReport = "ProjectPhaseComboReport";
        public const string TaskCategoryCombo = "TaskCategoryCombo";
        public const string TaskCategoryComboReport = "TaskCategoryComboReport";
        public const string TaskFlowCombo = "TaskFlowCombo";
        public const string TaskFlowComboReport = "TaskFlowComboReport";
        public const string ProfileCombo = "ProfileCombo";
        public const string ProfileComboReport = "ProfileComboReport";
        public const string SprintCombo = "SprintCombo";
        public const string SprintComboReport = "SprintComboReport";
        public const string TaskTypeCombo = "TaskTypeCombo";
        public const string TaskTypeComboReport = "TaskTypeComboReport";
        public const string VersionCombo = "VersionCombo";
        public const string VersionComboReport = "VersionComboReport";

        // tables

        public const string Setup = "Setup";
        public const string Person = "Person";
        public const string PersonReport = "PersonReport";
        public const string User = "User";
        public const string UserReport = "UserReport";
        public const string Client = "Client";
        public const string ClientReport = "ClientReport";        
        public const string ClientGroup = "ClientGroup";
        public const string ClientGroupReport = "ClientGroupReport";
        public const string Task = "Task";
        public const string TaskReport = "TaskReport";
        public const string Profile = "Profile";
        public const string ProfileReport = "ProfileReport";        
        public const string CompanyNews = "CompanyNews";
        public const string CompanyNewsReport = "CompanyNewsReport";
        public const string TaskType = "TaskType";
        public const string TaskTypeReport = "TaskTypeReport";
        public const string Project = "Project";
        public const string ProjectReport = "ProjectReport";
        public const string ProjectPhase = "ProjectPhase";
        public const string ProjectPhaseReport = "ProjectPhaseReport";
        public const string ProjectSprint = "ProjectSprint";
        public const string ProjectSprintReport = "ProjectSprintReport";
        public const string ProjectSprintVersion = "ProjectSprintVersion";
        public const string ProjectSprintVersionReport = "ProjectSprintVersionReport";
        public const string Survey = "Survey";
        public const string SurveyReport = "SurveyReport";
        public const string TaskTypeAccumulator = "TaskTypeAccumulator";
        public const string TaskTypeAccumulatorReport = "TaskTypeAccumulatorReport";
        public const string TaskCategory = "TaskCategory";
        public const string TaskCategoryReport = "TaskCategoryReport";
        public const string TaskCheckPoint = "TaskCheckPoint";
        public const string TaskCheckPointReport = "TaskCheckPointReport";
        public const string TaskFlow = "TaskFlow";
        public const string TaskFlowReport = "TaskFlowReport";
    }
}
