//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/t4models).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------
using System;
using System.Linq;

using LinqToDB;
using LinqToDB.Mapping;

namespace DataModel
{
	/// <summary>
	/// Database       : DevKit
	/// Data Source    : localhost
	/// Server Version : 9.6.1
	/// </summary>
	public partial class DevKitDB : LinqToDB.Data.DataConnection
	{
		public ITable<AuditLog>               AuditLogs               { get { return this.GetTable<AuditLog>(); } }
		public ITable<Client>                 Clients                 { get { return this.GetTable<Client>(); } }
		public ITable<ClientGroup>            ClientGroups            { get { return this.GetTable<ClientGroup>(); } }
		public ITable<ClientGroupAssociation> ClientGroupAssociations { get { return this.GetTable<ClientGroupAssociation>(); } }
		public ITable<CompanyNews>            CompanyNews             { get { return this.GetTable<CompanyNews>(); } }
		public ITable<Profile>                Profiles                { get { return this.GetTable<Profile>(); } }
		public ITable<Project>                Projects                { get { return this.GetTable<Project>(); } }
		public ITable<ProjectPhase>           ProjectPhases           { get { return this.GetTable<ProjectPhase>(); } }
		public ITable<ProjectSprint>          ProjectSprints          { get { return this.GetTable<ProjectSprint>(); } }
		public ITable<ProjectSprintVersion>   ProjectSprintVersions   { get { return this.GetTable<ProjectSprintVersion>(); } }
		public ITable<ProjectUser>            ProjectUsers            { get { return this.GetTable<ProjectUser>(); } }
		public ITable<Setup>                  Setups                  { get { return this.GetTable<Setup>(); } }
		public ITable<Survey>                 Surveys                 { get { return this.GetTable<Survey>(); } }
		public ITable<SurveyOption>           SurveyOptions           { get { return this.GetTable<SurveyOption>(); } }
		public ITable<SurveyUserOption>       SurveyUserOptions       { get { return this.GetTable<SurveyUserOption>(); } }
		public ITable<Task>                   Tasks                   { get { return this.GetTable<Task>(); } }
		public ITable<TaskAccumulatorValue>   TaskAccumulatorValues   { get { return this.GetTable<TaskAccumulatorValue>(); } }
		public ITable<TaskCategory>           TaskCategories          { get { return this.GetTable<TaskCategory>(); } }
		public ITable<TaskCheckPoint>         TaskCheckPoints         { get { return this.GetTable<TaskCheckPoint>(); } }
		public ITable<TaskCheckPointMark>     TaskCheckPointMarks     { get { return this.GetTable<TaskCheckPointMark>(); } }
		public ITable<TaskClient>             TaskClients             { get { return this.GetTable<TaskClient>(); } }
		public ITable<TaskClientGroup>        TaskClientGroups        { get { return this.GetTable<TaskClientGroup>(); } }
		public ITable<TaskDependency>         TaskDependencies        { get { return this.GetTable<TaskDependency>(); } }
		public ITable<TaskFlow>               TaskFlows               { get { return this.GetTable<TaskFlow>(); } }
		public ITable<TaskFlowChange>         TaskFlowChanges         { get { return this.GetTable<TaskFlowChange>(); } }
		public ITable<TaskMessage>            TaskMessages            { get { return this.GetTable<TaskMessage>(); } }
		public ITable<TaskProgress>           TaskProgresses          { get { return this.GetTable<TaskProgress>(); } }
		public ITable<TaskQuestion>           TaskQuestions           { get { return this.GetTable<TaskQuestion>(); } }
		public ITable<TaskType>               TaskTypes               { get { return this.GetTable<TaskType>(); } }
		public ITable<TaskTypeAccumulator>    TaskTypeAccumulators    { get { return this.GetTable<TaskTypeAccumulator>(); } }
		public ITable<User>                   Users                   { get { return this.GetTable<User>(); } }
		public ITable<UserEmail>              UserEmails              { get { return this.GetTable<UserEmail>(); } }
		public ITable<UserNewsRead>           UserNewsReads           { get { return this.GetTable<UserNewsRead>(); } }
		public ITable<UserPhone>              UserPhones              { get { return this.GetTable<UserPhone>(); } }

		public DevKitDB()
		{
			InitDataContext();
		}

		public DevKitDB(string configuration)
			: base(configuration)
		{
			InitDataContext();
		}

		partial void InitDataContext();
	}

	[Table(Schema="public", Name="AuditLog")]
	public partial class AuditLog
	{
		[PrimaryKey, Identity] public long      id          { get; set; } // bigint
		[Column,     Nullable] public DateTime? dtLog       { get; set; } // timestamp (6) without time zone
		[Column,     Nullable] public long?     fkUser      { get; set; } // bigint
		[Column,     Nullable] public long?     fkActionLog { get; set; } // bigint
		[Column,     Nullable] public long?     nuType      { get; set; } // bigint
		[Column,     Nullable] public long?     fkTarget    { get; set; } // bigint
		[Column,     Nullable] public string    stLog       { get; set; } // character varying(999)
		[Column,     Nullable] public string    stDetailLog { get; set; } // character varying(3999)
	}

	[Table(Schema="public", Name="Client")]
	public partial class Client
	{
		[PrimaryKey, Identity] public long      id              { get; set; } // bigint
		[Column,     Nullable] public DateTime? dtStart         { get; set; } // timestamp (6) without time zone
		[Column,     Nullable] public long?     fkUser          { get; set; } // bigint
		[Column,     Nullable] public string    stName          { get; set; } // character varying(200)
		[Column,     Nullable] public string    stContactEmail  { get; set; } // character varying(200)
		[Column,     Nullable] public string    stContactPhone  { get; set; } // character varying(20)
		[Column,     Nullable] public string    stContactPerson { get; set; } // character varying(200)
		[Column,     Nullable] public string    stInfo          { get; set; } // character varying(2000)
	}

	[Table(Schema="public", Name="ClientGroup")]
	public partial class ClientGroup
	{
		[PrimaryKey, Identity] public long      id      { get; set; } // bigint
		[Column,     Nullable] public DateTime? dtStart { get; set; } // timestamp (6) without time zone
		[Column,     Nullable] public long?     fkUser  { get; set; } // bigint
		[Column,     Nullable] public string    stName  { get; set; } // character varying(200)
	}

	[Table(Schema="public", Name="ClientGroupAssociation")]
	public partial class ClientGroupAssociation
	{
		[PrimaryKey, Identity] public long  id            { get; set; } // bigint
		[Column,     Nullable] public long? fkClient      { get; set; } // bigint
		[Column,     Nullable] public long? fkClientGroup { get; set; } // bigint
	}

	[Table(Schema="public", Name="CompanyNews")]
	public partial class CompanyNews
	{
		[PrimaryKey, Identity] public long      id        { get; set; } // bigint
		[Column,     Nullable] public string    stTitle   { get; set; } // character varying(200)
		[Column,     Nullable] public string    stMessage { get; set; } // character varying(2000)
		[Column,     Nullable] public long?     fkProject { get; set; } // bigint
		[Column,     Nullable] public DateTime? dtLog     { get; set; } // timestamp (6) without time zone
		[Column,     Nullable] public long?     fkUser    { get; set; } // bigint
		[Column,     Nullable] public bool?     bActive   { get; set; } // boolean
	}

	[Table(Schema="public", Name="Profile")]
	public partial class Profile
	{
		[PrimaryKey, Identity] public long   id            { get; set; } // bigint
		[Column,     Nullable] public string stName        { get; set; } // character varying(200)
		[Column,     Nullable] public string stPermissions { get; set; } // character varying(9999)
	}

	[Table(Schema="public", Name="Project")]
	public partial class Project
	{
		[PrimaryKey, Identity] public long      id                { get; set; } // bigint
		[Column,     Nullable] public long?     fkUser            { get; set; } // bigint
		[Column,     Nullable] public string    stName            { get; set; } // character varying(99)
		[Column,     Nullable] public long?     fkProjectTemplate { get; set; } // bigint
		[Column,     Nullable] public DateTime? dtCreation        { get; set; } // timestamp (6) without time zone
	}

	[Table(Schema="public", Name="ProjectPhase")]
	public partial class ProjectPhase
	{
		[PrimaryKey, Identity] public long   id        { get; set; } // bigint
		[Column,     Nullable] public string stName    { get; set; } // character varying(99)
		[Column,     Nullable] public long?  fkProject { get; set; } // bigint
	}

	[Table(Schema="public", Name="ProjectSprint")]
	public partial class ProjectSprint
	{
		[PrimaryKey, Identity] public long   id            { get; set; } // bigint
		[Column,     Nullable] public string stName        { get; set; } // character varying(200)
		[Column,     Nullable] public string stDescription { get; set; } // character varying(1000)
		[Column,     Nullable] public long?  fkProject     { get; set; } // bigint
		[Column,     Nullable] public long?  fkPhase       { get; set; } // bigint
	}

	[Table(Schema="public", Name="ProjectSprintVersion")]
	public partial class ProjectSprintVersion
	{
		[PrimaryKey, Identity] public long   id             { get; set; } // bigint
		[Column,     Nullable] public string stName         { get; set; } // character varying(20)
		[Column,     Nullable] public long?  fkSprint       { get; set; } // bigint
		[Column,     Nullable] public long?  fkVersionState { get; set; } // bigint
	}

	[Table(Schema="public", Name="ProjectUser")]
	public partial class ProjectUser
	{
		[PrimaryKey, Identity] public long      id        { get; set; } // bigint
		[Column,     Nullable] public long?     fkUser    { get; set; } // bigint
		[Column,     Nullable] public long?     fkProject { get; set; } // bigint
		[Column,     Nullable] public string    stRole    { get; set; } // character varying(99)
		[Column,     Nullable] public DateTime? dtJoin    { get; set; } // timestamp (6) without time zone
	}

	[Table(Schema="public", Name="Setup")]
	public partial class Setup
	{
		[PrimaryKey, Identity] public long   id               { get; set; } // bigint
		[Column,     Nullable] public string stPhoneMask      { get; set; } // character varying(99)
		[Column,     Nullable] public string stDateFormat     { get; set; } // character varying(99)
		[Column,     Nullable] public string stProtocolFormat { get; set; } // character varying(20)
	}

	[Table(Schema="public", Name="Survey")]
	public partial class Survey
	{
		[PrimaryKey, Identity] public long      id        { get; set; } // bigint
		[Column,     Nullable] public string    stTitle   { get; set; } // character varying(200)
		[Column,     Nullable] public string    stMessage { get; set; } // character varying(2000)
		[Column,     Nullable] public long?     fkProject { get; set; } // bigint
		[Column,     Nullable] public DateTime? dtLog     { get; set; } // timestamp (6) without time zone
		[Column,     Nullable] public long?     fkUser    { get; set; } // bigint
		[Column,     Nullable] public bool?     bActive   { get; set; } // boolean
	}

	[Table(Schema="public", Name="SurveyOption")]
	public partial class SurveyOption
	{
		[PrimaryKey, Identity] public long   id       { get; set; } // bigint
		[Column,     Nullable] public long?  fkSurvey { get; set; } // bigint
		[Column,     Nullable] public int?   nuOrder  { get; set; } // integer
		[Column,     Nullable] public string stOption { get; set; } // character varying(200)
	}

	[Table(Schema="public", Name="SurveyUserOption")]
	public partial class SurveyUserOption
	{
		[PrimaryKey, Identity] public long      id             { get; set; } // bigint
		[Column,     Nullable] public long?     fkSurvey       { get; set; } // bigint
		[Column,     Nullable] public long?     fkUser         { get; set; } // bigint
		[Column,     Nullable] public long?     fkSurveyOption { get; set; } // bigint
		[Column,     Nullable] public DateTime? dtLog          { get; set; } // timestamp (6) without time zone
	}

	[Table(Schema="public", Name="Task")]
	public partial class Task
	{
		[PrimaryKey, Identity] public long      id                { get; set; } // bigint
		[Column,     Nullable] public bool?     bComplete         { get; set; } // boolean
		[Column,     Nullable] public DateTime? dtStart           { get; set; } // timestamp (6) without time zone
		[Column,     Nullable] public DateTime? dtLastEdit        { get; set; } // timestamp (6) without time zone
		[Column,     Nullable] public string    stProtocol        { get; set; } // character varying(20)
		[Column,     Nullable] public string    stTitle           { get; set; } // character varying(200)
		[Column,     Nullable] public string    stLocalization    { get; set; } // character varying(200)
		[Column,     Nullable] public string    stDescription     { get; set; } // character varying(4000)
		[Column,     Nullable] public long?     nuPriority        { get; set; } // bigint
		[Column,     Nullable] public long?     fkProject         { get; set; } // bigint
		[Column,     Nullable] public long?     fkPhase           { get; set; } // bigint
		[Column,     Nullable] public long?     fkSprint          { get; set; } // bigint
		[Column,     Nullable] public long?     fkUserStart       { get; set; } // bigint
		[Column,     Nullable] public long?     fkVersion         { get; set; } // bigint
		[Column,     Nullable] public long?     fkTaskType        { get; set; } // bigint
		[Column,     Nullable] public long?     fkTaskCategory    { get; set; } // bigint
		[Column,     Nullable] public long?     fkTaskFlowCurrent { get; set; } // bigint
		[Column,     Nullable] public long?     fkReleaseVersion  { get; set; } // bigint
		[Column,     Nullable] public long?     fkUserResponsible { get; set; } // bigint
		[Column,     Nullable] public DateTime? dtExpired         { get; set; } // timestamp (6) without time zone
	}

	[Table(Schema="public", Name="TaskAccumulatorValue")]
	public partial class TaskAccumulatorValue
	{
		[PrimaryKey, Identity] public long      id          { get; set; } // bigint
		[Column,     Nullable] public DateTime? dtLog       { get; set; } // timestamp (6) without time zone
		[Column,     Nullable] public long?     nuValue     { get; set; } // bigint
		[Column,     Nullable] public long?     nuHourValue { get; set; } // bigint
		[Column,     Nullable] public long?     nuMinValue  { get; set; } // bigint
		[Column,     Nullable] public long?     fkTask      { get; set; } // bigint
		[Column,     Nullable] public long?     fkTaskAcc   { get; set; } // bigint
		[Column,     Nullable] public long?     fkUser      { get; set; } // bigint
	}

	[Table(Schema="public", Name="TaskCategory")]
	public partial class TaskCategory
	{
		[PrimaryKey, Identity] public long   id               { get; set; } // bigint
		[Column,     Nullable] public string stName           { get; set; } // character varying(200)
		[Column,     Nullable] public string stAbreviation    { get; set; } // character varying(10)
		[Column,     Nullable] public string stDescription    { get; set; } // character varying(500)
		[Column,     Nullable] public long?  fkTaskType       { get; set; } // bigint
		[Column,     Nullable] public bool?  bExpires         { get; set; } // boolean
		[Column,     Nullable] public long?  nuExpiresDays    { get; set; } // bigint
		[Column,     Nullable] public long?  nuExpiresHours   { get; set; } // bigint
		[Column,     Nullable] public long?  nuExpiresMinutes { get; set; } // bigint
	}

	[Table(Schema="public", Name="TaskCheckPoint")]
	public partial class TaskCheckPoint
	{
		[PrimaryKey, Identity] public long   id         { get; set; } // bigint
		[Column,     Nullable] public string stName     { get; set; } // character varying(50)
		[Column,     Nullable] public long?  fkCategory { get; set; } // bigint
		[Column,     Nullable] public bool?  bMandatory { get; set; } // boolean
	}

	[Table(Schema="public", Name="TaskCheckPointMark")]
	public partial class TaskCheckPointMark
	{
		[PrimaryKey, Identity] public long      id           { get; set; } // bigint
		[Column,     Nullable] public long?     fkCheckPoint { get; set; } // bigint
		[Column,     Nullable] public long?     fkUser       { get; set; } // bigint
		[Column,     Nullable] public long?     fkTask       { get; set; } // bigint
		[Column,     Nullable] public DateTime? dtLog        { get; set; } // timestamp (6) without time zone
	}

	[Table(Schema="public", Name="TaskClient")]
	public partial class TaskClient
	{
		[PrimaryKey, Identity] public long  id       { get; set; } // bigint
		[Column,     Nullable] public long? fkTask   { get; set; } // bigint
		[Column,     Nullable] public long? fkClient { get; set; } // bigint
	}

	[Table(Schema="public", Name="TaskClientGroup")]
	public partial class TaskClientGroup
	{
		[PrimaryKey, Identity] public long  id            { get; set; } // bigint
		[Column,     Nullable] public long? fkTask        { get; set; } // bigint
		[Column,     Nullable] public long? fkClientGroup { get; set; } // bigint
	}

	[Table(Schema="public", Name="TaskDependency")]
	public partial class TaskDependency
	{
		[PrimaryKey, Identity] public long      id         { get; set; } // bigint
		[Column,     Nullable] public DateTime? dtLog      { get; set; } // timestamp (6) without time zone
		[Column,     Nullable] public long?     fkUser     { get; set; } // bigint
		[Column,     Nullable] public long?     fkMainTask { get; set; } // bigint
		[Column,     Nullable] public long?     fkSubTask  { get; set; } // bigint
	}

	[Table(Schema="public", Name="TaskFlow")]
	public partial class TaskFlow
	{
		[PrimaryKey, Identity] public long   id             { get; set; } // bigint
		[Column,     Nullable] public bool?  bForceComplete { get; set; } // boolean
		[Column,     Nullable] public bool?  bForceOpen     { get; set; } // boolean
		[Column,     Nullable] public string stName         { get; set; } // character varying(200)
		[Column,     Nullable] public long?  nuOrder        { get; set; } // bigint
		[Column,     Nullable] public long?  fkTaskType     { get; set; } // bigint
		[Column,     Nullable] public long?  fkTaskCategory { get; set; } // bigint
	}

	[Table(Schema="public", Name="TaskFlowChange")]
	public partial class TaskFlowChange
	{
		[PrimaryKey, Identity] public long      id             { get; set; } // bigint
		[Column,     Nullable] public string    stMessage      { get; set; } // character varying(300)
		[Column,     Nullable] public DateTime? dtLog          { get; set; } // timestamp (6) without time zone
		[Column,     Nullable] public long?     fkTask         { get; set; } // bigint
		[Column,     Nullable] public long?     fkUser         { get; set; } // bigint
		[Column,     Nullable] public long?     fkOldFlowState { get; set; } // bigint
		[Column,     Nullable] public long?     fkNewFlowState { get; set; } // bigint
	}

	[Table(Schema="public", Name="TaskMessage")]
	public partial class TaskMessage
	{
		[PrimaryKey, Identity] public long      id            { get; set; } // bigint
		[Column,     Nullable] public string    stMessage     { get; set; } // character varying(999)
		[Column,     Nullable] public DateTime? dtLog         { get; set; } // timestamp (6) without time zone
		[Column,     Nullable] public long?     fkTask        { get; set; } // bigint
		[Column,     Nullable] public long?     fkUser        { get; set; } // bigint
		[Column,     Nullable] public long?     fkCurrentFlow { get; set; } // bigint
	}

	[Table(Schema="public", Name="TaskProgress")]
	public partial class TaskProgress
	{
		[PrimaryKey, Identity] public long      id             { get; set; } // bigint
		[Column,     Nullable] public long?     fkTask         { get; set; } // bigint
		[Column,     Nullable] public long?     fkUserAssigned { get; set; } // bigint
		[Column,     Nullable] public DateTime? dtLog          { get; set; } // timestamp (6) without time zone
	}

	[Table(Schema="public", Name="TaskQuestion")]
	public partial class TaskQuestion
	{
		[PrimaryKey, Identity] public long      id             { get; set; } // bigint
		[Column,     Nullable] public string    stStatement    { get; set; } // character varying(2000)
		[Column,     Nullable] public string    stAnswer       { get; set; } // character varying(2000)
		[Column,     Nullable] public long?     fkTask         { get; set; } // bigint
		[Column,     Nullable] public long?     fkUserOpen     { get; set; } // bigint
		[Column,     Nullable] public long?     fkUserDirected { get; set; } // bigint
		[Column,     Nullable] public bool?     bFinal         { get; set; } // boolean
		[Column,     Nullable] public DateTime? dtOpen         { get; set; } // timestamp (6) without time zone
		[Column,     Nullable] public DateTime? dtClosed       { get; set; } // timestamp (6) without time zone
	}

	[Table(Schema="public", Name="TaskType")]
	public partial class TaskType
	{
		[PrimaryKey, Identity] public long   id             { get; set; } // bigint
		[Column,     Nullable] public bool?  bManaged       { get; set; } // boolean
		[Column,     Nullable] public bool?  bCondensedView { get; set; } // boolean
		[Column,     Nullable] public bool?  bKPA           { get; set; } // boolean
		[Column,     Nullable] public string stName         { get; set; } // character varying(200)
		[Column,     Nullable] public long?  fkProject      { get; set; } // bigint
	}

	[Table(Schema="public", Name="TaskTypeAccumulator")]
	public partial class TaskTypeAccumulator
	{
		[PrimaryKey, Identity] public long   id             { get; set; } // bigint
		[Column,     Nullable] public bool?  bEstimate      { get; set; } // boolean
		[Column,     Nullable] public string stName         { get; set; } // character varying(30)
		[Column,     Nullable] public long?  fkTaskType     { get; set; } // bigint
		[Column,     Nullable] public long?  fkTaskAccType  { get; set; } // bigint
		[Column,     Nullable] public long?  fkTaskFlow     { get; set; } // bigint
		[Column,     Nullable] public long?  fkTaskCategory { get; set; } // bigint
	}

	[Table(Schema="public", Name="User")]
	public partial class User
	{
		[PrimaryKey, Identity] public long      id          { get; set; } // bigint
		[Column,     Nullable] public bool?     bActive     { get; set; } // boolean
		[Column,     Nullable] public string    stLogin     { get; set; } // character varying(200)
		[Column,     Nullable] public string    stPassword  { get; set; } // character varying(30)
		[Column,     Nullable] public long?     fkProfile   { get; set; } // bigint
		[Column,     Nullable] public DateTime? dtLastLogin { get; set; } // timestamp (6) without time zone
		[Column,     Nullable] public DateTime? dtCreation  { get; set; } // timestamp (6) without time zone
	}

	[Table(Schema="public", Name="UserEmail")]
	public partial class UserEmail
	{
		[PrimaryKey, Identity] public long   id      { get; set; } // bigint
		[Column,     Nullable] public long?  fkUser  { get; set; } // bigint
		[Column,     Nullable] public string stEmail { get; set; } // character varying(250)
	}

	[Table(Schema="public", Name="UserNewsRead")]
	public partial class UserNewsRead
	{
		[PrimaryKey, Identity] public long      id     { get; set; } // bigint
		[Column,     Nullable] public long?     fkNews { get; set; } // bigint
		[Column,     Nullable] public DateTime? dtLog  { get; set; } // timestamp (6) without time zone
		[Column,     Nullable] public long?     fkUser { get; set; } // bigint
	}

	[Table(Schema="public", Name="UserPhone")]
	public partial class UserPhone
	{
		[PrimaryKey, Identity] public long   id            { get; set; } // bigint
		[Column,     Nullable] public long?  fkUser        { get; set; } // bigint
		[Column,     Nullable] public string stPhone       { get; set; } // character varying(50)
		[Column,     Nullable] public string stDescription { get; set; } // character varying(50)
	}

	public static partial class TableExtensions
	{
		public static AuditLog Find(this ITable<AuditLog> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static Client Find(this ITable<Client> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static ClientGroup Find(this ITable<ClientGroup> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static ClientGroupAssociation Find(this ITable<ClientGroupAssociation> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static CompanyNews Find(this ITable<CompanyNews> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static Profile Find(this ITable<Profile> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static Project Find(this ITable<Project> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static ProjectPhase Find(this ITable<ProjectPhase> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static ProjectSprint Find(this ITable<ProjectSprint> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static ProjectSprintVersion Find(this ITable<ProjectSprintVersion> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static ProjectUser Find(this ITable<ProjectUser> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static Setup Find(this ITable<Setup> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static Survey Find(this ITable<Survey> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static SurveyOption Find(this ITable<SurveyOption> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static SurveyUserOption Find(this ITable<SurveyUserOption> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static Task Find(this ITable<Task> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static TaskAccumulatorValue Find(this ITable<TaskAccumulatorValue> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static TaskCategory Find(this ITable<TaskCategory> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static TaskCheckPoint Find(this ITable<TaskCheckPoint> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static TaskCheckPointMark Find(this ITable<TaskCheckPointMark> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static TaskClient Find(this ITable<TaskClient> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static TaskClientGroup Find(this ITable<TaskClientGroup> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static TaskDependency Find(this ITable<TaskDependency> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static TaskFlow Find(this ITable<TaskFlow> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static TaskFlowChange Find(this ITable<TaskFlowChange> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static TaskMessage Find(this ITable<TaskMessage> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static TaskProgress Find(this ITable<TaskProgress> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static TaskQuestion Find(this ITable<TaskQuestion> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static TaskType Find(this ITable<TaskType> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static TaskTypeAccumulator Find(this ITable<TaskTypeAccumulator> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static User Find(this ITable<User> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static UserEmail Find(this ITable<UserEmail> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static UserNewsRead Find(this ITable<UserNewsRead> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static UserPhone Find(this ITable<UserPhone> table, long id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}
	}
}
