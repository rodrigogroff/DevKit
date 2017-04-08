
DROP TABLE public."Setup";

CREATE TABLE public."Setup"
(
    id bigserial NOT NULL,
    "stPhoneMask" character varying(99),
	"stDateFormat" character varying(99),
	"stProtocolFormat" character varying(20),
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Setup"
    OWNER to postgres;

DROP TABLE public."Profile";

CREATE TABLE public."Profile"
(
    id bigserial NOT NULL,
    "stName" character varying(200),
    "stPermissions" character varying(9999),
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Profile"
    OWNER to postgres;

DROP TABLE public."User";

CREATE TABLE public."User"
(
    id bigserial NOT NULL,
    "bActive" boolean,
	"stLogin" character varying(200),
    "stPassword" character varying(30),    
    "fkProfile" bigint,
    "dtLastLogin" timestamp without time zone,
    "dtCreation" timestamp without time zone,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."User"
    OWNER to postgres;

DROP TABLE public."UserEmail";

CREATE TABLE public."UserEmail"
(
    id bigserial NOT NULL,
    "stEmail" character varying(250),
	"fkUser" bigint,    
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."UserEmail"
    OWNER to postgres;

DROP TABLE public."UserPhone";

CREATE TABLE public."UserPhone"
(
    id bigserial NOT NULL,
    "stPhone" character varying(50),
    "stDescription" character varying(50),
	"fkUser" bigint,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."UserPhone"
    OWNER to postgres;

DROP TABLE public."Project";

CREATE TABLE public."Project"
(
    id bigserial NOT NULL,
    "stName" character varying(99),
	"fkUser" bigint,
	"fkProjectTemplate" bigint,
	"dtCreation" timestamp without time zone,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Project"
    OWNER to postgres;

DROP TABLE public."ProjectUser";

CREATE TABLE public."ProjectUser"
(
    id bigserial NOT NULL,
    "fkProject" bigint,
    "fkUser" bigint,
	"stRole" character varying(99),
	"dtJoin" timestamp without time zone,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."ProjectUser"
    OWNER to postgres;

DROP TABLE public."ProjectPhase";

CREATE TABLE public."ProjectPhase"
(
    id bigserial NOT NULL,
	"stName" character varying(99),
	"fkProject" bigint,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."ProjectPhase"
    OWNER to postgres;

DROP TABLE public."ProjectSprint";

CREATE TABLE public."ProjectSprint"
(
    id bigserial NOT NULL,
	"stName" character varying(200),
	"stDescription" character varying(1000),
    "fkProject" bigint,
	"fkPhase" bigint,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."ProjectSprint"
    OWNER to postgres;

DROP TABLE public."TaskType";
	
CREATE TABLE public."TaskType"
(
    id bigserial NOT NULL,
    "bManaged" boolean,
	"bCondensedView" boolean,	
	"bKPA" boolean,
	"stName" character varying(200),
	"fkProject" bigint,
	PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."TaskType"
    OWNER to postgres;

DROP TABLE public."TaskCategory";

CREATE TABLE public."TaskCategory"
(
    id bigserial NOT NULL,
    "stName" character varying(200),
	"stAbreviation" character varying(10),
	"stDescription" character varying(500),
	"fkTaskType" bigint,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."TaskCategory"
    OWNER to postgres;

DROP TABLE public."TaskFlow";

CREATE TABLE public."TaskFlow"
(
    id bigserial NOT NULL,
    "bForceComplete" boolean,
	"bForceOpen" boolean,
	"stName" character varying(200),
	"nuOrder" bigint,
	"fkTaskType" bigint,
	"fkTaskCategory" bigint,	
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."TaskFlow"
    OWNER to postgres;
	
DROP TABLE public."ProjectSprintVersion";

CREATE TABLE public."ProjectSprintVersion"
(
    id bigserial NOT NULL,
	"stName" character varying(20),
    "fkSprint" bigint,
	"fkVersionState" bigint,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."ProjectSprintVersion"
    OWNER to postgres;

DROP TABLE public."Task";

CREATE TABLE public."Task"
(
    id bigserial NOT NULL,
	"bComplete" boolean,
	"dtStart" timestamp without time zone,	
	"dtLastEdit" timestamp without time zone,	
	"stProtocol" character varying(20),
	"stTitle" character varying(200),
	"stLocalization" character varying(200),
	"stDescription" character varying(4000),
	"nuPriority" bigint,
	"fkProject" bigint,	
	"fkPhase" bigint,
    "fkSprint" bigint,
	"fkUserStart" bigint,
	"fkVersion" bigint,
	"fkTaskType" bigint,
	"fkTaskCategory" bigint,
	"fkTaskFlowCurrent" bigint,
	"fkReleaseVersion" bigint,
	"fkUserResponsible" bigint,	
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Task"
    OWNER to postgres;

DROP TABLE public."TaskProgress";

CREATE TABLE public."TaskProgress"
(
    id bigserial NOT NULL,
	"fkTask" bigint,
	"fkUserAssigned" bigint,
	"dtLog" timestamp without time zone,	
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."TaskProgress"
    OWNER to postgres;

DROP TABLE public."TaskMessage";

CREATE TABLE public."TaskMessage"
(
    id bigserial NOT NULL,
	"stMessage" character varying(999),
	"dtLog" timestamp without time zone,	
	"fkTask" bigint,
	"fkUser" bigint,
	"fkCurrentFlow" bigint,	
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."TaskMessage"
    OWNER to postgres;

DROP TABLE public."TaskFlowChange";

CREATE TABLE public."TaskFlowChange"
(
    id bigserial NOT NULL,
	"stMessage" character varying(300),
	"dtLog" timestamp without time zone,
	"fkTask" bigint,
	"fkUser" bigint,
	"fkOldFlowState" bigint,
	"fkNewFlowState" bigint,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."TaskFlowChange"
    OWNER to postgres;

DROP TABLE public."TaskTypeAccumulator";
	
CREATE TABLE public."TaskTypeAccumulator"
(
    id bigserial NOT NULL,
	"bEstimate" boolean,
	"stName" character varying(30),
	"fkTaskType" bigint,
	"fkTaskAccType" bigint,
	"fkTaskFlow" bigint,
	"fkTaskCategory" bigint,	
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."TaskTypeAccumulator"
    OWNER to postgres;

DROP TABLE public."TaskAccumulatorValue";

CREATE TABLE public."TaskAccumulatorValue"
(
    id bigserial NOT NULL,
	"dtLog" timestamp without time zone,
	"nuValue" bigint,
	"nuHourValue" bigint,
	"nuMinValue" bigint,		
	"fkTask" bigint,
	"fkTaskAcc" bigint,
	"fkUser" bigint,	
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."TaskAccumulatorValue"
    OWNER to postgres;

DROP TABLE public."AuditLog";

CREATE TABLE public."AuditLog"
(
    id bigserial NOT NULL,
	"dtLog" timestamp without time zone,
	"fkUser" bigint,
	"fkActionLog" bigint,
	"stLog" character varying(999),
	"stDetailLog" character varying(3999),	
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."AuditLog"
    OWNER to postgres;
