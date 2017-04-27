
CREATE TABLE IF NOT EXISTS public."Setup" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Setup" OWNER to postgres;
ALTER TABLE public."Setup" ADD COLUMN if not exists "stPhoneMask" character varying(99);
ALTER TABLE public."Setup" ADD COLUMN if not exists "stDateFormat" character varying(99);
ALTER TABLE public."Setup" ADD COLUMN if not exists "stProtocolFormat" character varying(20);

CREATE TABLE IF NOT EXISTS public."Profile" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Profile" OWNER to postgres;
ALTER TABLE public."Profile" ADD COLUMN if not exists "stName" character varying(200);
ALTER TABLE public."Profile" ADD COLUMN if not exists "stPermissions" character varying(9999);

CREATE TABLE IF NOT EXISTS public."User" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."User" OWNER to postgres;
ALTER TABLE public."User" ADD COLUMN if not exists "bActive" boolean;
ALTER TABLE public."User" ADD COLUMN if not exists "stLogin" character varying(200);
ALTER TABLE public."User" ADD COLUMN if not exists "stPassword" character varying(30);
ALTER TABLE public."User" ADD COLUMN if not exists "fkProfile" bigint;
ALTER TABLE public."User" ADD COLUMN if not exists "dtLastLogin" timestamp without time zone;
ALTER TABLE public."User" ADD COLUMN if not exists "dtCreation" timestamp without time zone;
   
CREATE TABLE IF NOT EXISTS public."UserEmail" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."UserEmail" OWNER to postgres;
ALTER TABLE public."UserEmail" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."UserEmail" ADD COLUMN if not exists "stEmail" character varying(250);

CREATE TABLE IF NOT EXISTS public."UserPhone" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."UserPhone" OWNER to postgres;
ALTER TABLE public."UserPhone" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."UserPhone" ADD COLUMN if not exists "stPhone" character varying(50);
ALTER TABLE public."UserPhone" ADD COLUMN if not exists "stDescription" character varying(50);

CREATE TABLE IF NOT EXISTS public."Project" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Project" OWNER to postgres;
ALTER TABLE public."Project" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."Project" ADD COLUMN if not exists "stName" character varying(99);
ALTER TABLE public."Project" ADD COLUMN if not exists "fkProjectTemplate" bigint;
ALTER TABLE public."Project" ADD COLUMN if not exists "dtCreation" timestamp without time zone;

CREATE TABLE IF NOT EXISTS public."ProjectUser" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."ProjectUser" OWNER to postgres;
ALTER TABLE public."ProjectUser" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."ProjectUser" ADD COLUMN if not exists "fkProject" bigint;
ALTER TABLE public."ProjectUser" ADD COLUMN if not exists "stRole" character varying(99);
ALTER TABLE public."ProjectUser" ADD COLUMN if not exists "dtJoin" timestamp without time zone;

CREATE TABLE IF NOT EXISTS public."ProjectPhase" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."ProjectPhase" OWNER to postgres;
ALTER TABLE public."ProjectPhase" ADD COLUMN if not exists "stName" character varying(99);
ALTER TABLE public."ProjectPhase" ADD COLUMN if not exists "fkProject" bigint;

CREATE TABLE IF NOT EXISTS public."ProjectSprint" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."ProjectSprint" OWNER to postgres;
ALTER TABLE public."ProjectSprint" ADD COLUMN if not exists "stName" character varying(200);
ALTER TABLE public."ProjectSprint" ADD COLUMN if not exists "stDescription" character varying(1000);	
ALTER TABLE public."ProjectSprint" ADD COLUMN if not exists "fkProject" bigint;	
ALTER TABLE public."ProjectSprint" ADD COLUMN if not exists "fkPhase" bigint;	
	
CREATE TABLE IF NOT EXISTS public."TaskType" ( id bigserial NOT NULL,	PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."TaskType" OWNER to postgres;
ALTER TABLE public."TaskType" ADD COLUMN if not exists "bManaged" boolean;
ALTER TABLE public."TaskType" ADD COLUMN if not exists "bCondensedView" boolean;
ALTER TABLE public."TaskType" ADD COLUMN if not exists "bKPA" boolean;
ALTER TABLE public."TaskType" ADD COLUMN if not exists "stName" character varying(200);
ALTER TABLE public."TaskType" ADD COLUMN if not exists "fkProject" bigint;

CREATE TABLE IF NOT EXISTS public."TaskCategory" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."TaskCategory" OWNER to postgres;
ALTER TABLE public."TaskCategory" ADD COLUMN if not exists "stName" character varying(200);
ALTER TABLE public."TaskCategory" ADD COLUMN if not exists "stAbreviation" character varying(10);
ALTER TABLE public."TaskCategory" ADD COLUMN if not exists "stDescription" character varying(500);
ALTER TABLE public."TaskCategory" ADD COLUMN if not exists "fkTaskType" bigint;    
ALTER TABLE public."TaskCategory" ADD COLUMN if not exists "bExpires" boolean;
ALTER TABLE public."TaskCategory" ADD COLUMN if not exists "nuExpiresDays" bigint;
ALTER TABLE public."TaskCategory" ADD COLUMN if not exists "nuExpiresHours" bigint;
ALTER TABLE public."TaskCategory" ADD COLUMN if not exists "nuExpiresMinutes" bigint;

CREATE TABLE IF NOT EXISTS public."TaskFlow" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."TaskFlow" OWNER to postgres;
ALTER TABLE public."TaskFlow" ADD COLUMN if not exists "bForceComplete" boolean;
ALTER TABLE public."TaskFlow" ADD COLUMN if not exists "bForceOpen" boolean;
ALTER TABLE public."TaskFlow" ADD COLUMN if not exists "stName" character varying(200);
ALTER TABLE public."TaskFlow" ADD COLUMN if not exists "nuOrder" bigint;
ALTER TABLE public."TaskFlow" ADD COLUMN if not exists "fkTaskType" bigint;
ALTER TABLE public."TaskFlow" ADD COLUMN if not exists "fkTaskCategory" bigint;
	
CREATE TABLE IF NOT EXISTS public."ProjectSprintVersion" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."ProjectSprintVersion" OWNER to postgres;
ALTER TABLE public."ProjectSprintVersion" ADD COLUMN if not exists "stName" character varying(20);
ALTER TABLE public."ProjectSprintVersion" ADD COLUMN if not exists "fkSprint" bigint;
ALTER TABLE public."ProjectSprintVersion" ADD COLUMN if not exists "fkVersionState" bigint;

CREATE TABLE IF NOT EXISTS public."Task" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Task" OWNER to postgres;
ALTER TABLE public."Task" ADD COLUMN if not exists "bComplete" boolean;
ALTER TABLE public."Task" ADD COLUMN if not exists "dtStart" timestamp without time zone;
ALTER TABLE public."Task" ADD COLUMN if not exists "dtLastEdit" timestamp without time zone;
ALTER TABLE public."Task" ADD COLUMN if not exists "stProtocol" character varying(20);
ALTER TABLE public."Task" ADD COLUMN if not exists "stTitle" character varying(200);
ALTER TABLE public."Task" ADD COLUMN if not exists "stLocalization" character varying(200);
ALTER TABLE public."Task" ADD COLUMN if not exists "stDescription" character varying(4000);
ALTER TABLE public."Task" ADD COLUMN if not exists "nuPriority" bigint;
ALTER TABLE public."Task" ADD COLUMN if not exists "fkProject" bigint;
ALTER TABLE public."Task" ADD COLUMN if not exists "fkPhase" bigint;
ALTER TABLE public."Task" ADD COLUMN if not exists "fkSprint" bigint;
ALTER TABLE public."Task" ADD COLUMN if not exists "fkUserStart" bigint;
ALTER TABLE public."Task" ADD COLUMN if not exists "fkVersion" bigint;
ALTER TABLE public."Task" ADD COLUMN if not exists "fkTaskType" bigint;
ALTER TABLE public."Task" ADD COLUMN if not exists "fkTaskCategory" bigint;
ALTER TABLE public."Task" ADD COLUMN if not exists "fkTaskFlowCurrent" bigint;
ALTER TABLE public."Task" ADD COLUMN if not exists "fkReleaseVersion" bigint;
ALTER TABLE public."Task" ADD COLUMN if not exists "fkUserResponsible" bigint;
ALTER TABLE public."Task" ADD COLUMN if not exists "dtExpired" timestamp without time zone;

CREATE TABLE IF NOT EXISTS public."TaskProgress" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."TaskProgress" OWNER to postgres;
ALTER TABLE public."TaskProgress" ADD COLUMN if not exists "fkTask" bigint;
ALTER TABLE public."TaskProgress" ADD COLUMN if not exists "fkUserAssigned" bigint;
ALTER TABLE public."TaskProgress" ADD COLUMN if not exists "dtLog" timestamp without time zone;

CREATE TABLE IF NOT EXISTS public."TaskMessage" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."TaskMessage" OWNER to postgres;
ALTER TABLE public."TaskMessage" ADD COLUMN if not exists "stMessage" character varying(999);
ALTER TABLE public."TaskMessage" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."TaskMessage" ADD COLUMN if not exists "fkTask" bigint;
ALTER TABLE public."TaskMessage" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."TaskMessage" ADD COLUMN if not exists "fkCurrentFlow" bigint;

CREATE TABLE IF NOT EXISTS public."TaskFlowChange" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."TaskFlowChange" OWNER to postgres;
ALTER TABLE public."TaskFlowChange" ADD COLUMN if not exists "stMessage" character varying(300);
ALTER TABLE public."TaskFlowChange" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."TaskFlowChange" ADD COLUMN if not exists "fkTask" bigint;
ALTER TABLE public."TaskFlowChange" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."TaskFlowChange" ADD COLUMN if not exists "fkOldFlowState" bigint;
ALTER TABLE public."TaskFlowChange" ADD COLUMN if not exists "fkNewFlowState" bigint;

CREATE TABLE IF NOT EXISTS public."TaskTypeAccumulator" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."TaskTypeAccumulator" OWNER to postgres;
ALTER TABLE public."TaskTypeAccumulator" ADD COLUMN if not exists "bEstimate" boolean;
ALTER TABLE public."TaskTypeAccumulator" ADD COLUMN if not exists "stName" character varying(30);
ALTER TABLE public."TaskTypeAccumulator" ADD COLUMN if not exists "fkTaskType" bigint;
ALTER TABLE public."TaskTypeAccumulator" ADD COLUMN if not exists "fkTaskAccType" bigint;
ALTER TABLE public."TaskTypeAccumulator" ADD COLUMN if not exists "fkTaskFlow" bigint;
ALTER TABLE public."TaskTypeAccumulator" ADD COLUMN if not exists "fkTaskCategory" bigint;

CREATE TABLE IF NOT EXISTS public."TaskAccumulatorValue" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."TaskAccumulatorValue" OWNER to postgres;
ALTER TABLE public."TaskAccumulatorValue" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."TaskAccumulatorValue" ADD COLUMN if not exists "nuValue" bigint;
ALTER TABLE public."TaskAccumulatorValue" ADD COLUMN if not exists "nuHourValue" bigint;
ALTER TABLE public."TaskAccumulatorValue" ADD COLUMN if not exists "nuMinValue" bigint;
ALTER TABLE public."TaskAccumulatorValue" ADD COLUMN if not exists "fkTask" bigint;
ALTER TABLE public."TaskAccumulatorValue" ADD COLUMN if not exists "fkTaskAcc" bigint;
ALTER TABLE public."TaskAccumulatorValue" ADD COLUMN if not exists "fkUser" bigint;

CREATE TABLE IF NOT EXISTS public."AuditLog" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."AuditLog" OWNER to postgres;
ALTER TABLE public."AuditLog" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."AuditLog" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."AuditLog" ADD COLUMN if not exists "fkActionLog" bigint;
ALTER TABLE public."AuditLog" ADD COLUMN if not exists "nuType" bigint;
ALTER TABLE public."AuditLog" ADD COLUMN if not exists "fkTarget" bigint;
ALTER TABLE public."AuditLog" ADD COLUMN if not exists "stLog" character varying(999);
ALTER TABLE public."AuditLog" ADD COLUMN if not exists "stDetailLog" character varying(3999);

CREATE TABLE IF NOT EXISTS public."Survey" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Survey" OWNER to postgres;
ALTER TABLE public."Survey" ADD COLUMN if not exists "stTitle" character varying(200);
ALTER TABLE public."Survey" ADD COLUMN if not exists "stMessage" character varying(2000);
ALTER TABLE public."Survey" ADD COLUMN if not exists "fkProject" bigint;
ALTER TABLE public."Survey" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."Survey" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."Survey" ADD COLUMN if not exists "bActive" boolean;

CREATE TABLE IF NOT EXISTS public."SurveyOption" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SurveyOption" OWNER to postgres;
ALTER TABLE public."SurveyOption" ADD COLUMN if not exists "fkSurvey" bigint;
ALTER TABLE public."SurveyOption" ADD COLUMN if not exists "nuOrder" int;
ALTER TABLE public."SurveyOption" ADD COLUMN if not exists "stOption" character varying(200);

CREATE TABLE IF NOT EXISTS public."SurveyUserOption" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SurveyUserOption" OWNER to postgres;
ALTER TABLE public."SurveyUserOption" ADD COLUMN if not exists "fkSurvey" bigint;
ALTER TABLE public."SurveyUserOption" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."SurveyUserOption" ADD COLUMN if not exists "fkSurveyOption" bigint;
ALTER TABLE public."SurveyUserOption" ADD COLUMN if not exists "dtLog" timestamp without time zone;
	
CREATE TABLE IF NOT EXISTS public."CompanyNews" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."CompanyNews" OWNER to postgres;
ALTER TABLE public."CompanyNews" ADD COLUMN if not exists "stTitle" character varying(200);
ALTER TABLE public."CompanyNews" ADD COLUMN if not exists "stMessage" character varying(2000);
ALTER TABLE public."CompanyNews" ADD COLUMN if not exists "fkProject" bigint;
ALTER TABLE public."CompanyNews" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."CompanyNews" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."CompanyNews" ADD COLUMN if not exists "bActive" boolean;
	
CREATE TABLE IF NOT EXISTS public."UserNewsRead" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."UserNewsRead" OWNER to postgres;
ALTER TABLE public."UserNewsRead" ADD COLUMN if not exists "fkNews" bigint;
ALTER TABLE public."UserNewsRead" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."UserNewsRead" ADD COLUMN if not exists "fkUser" bigint;
