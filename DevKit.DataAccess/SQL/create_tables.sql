
CREATE TABLE public."Setup"
(
    id bigserial NOT NULL,
    "stPhoneMask" character varying(99),
	"stDateFormat" character varying(99),
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Setup"
    OWNER to postgres;

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

CREATE TABLE public."User"
(
    id bigserial NOT NULL,
    "stLogin" character varying(200),
    "stPassword" character varying(30),
    "bActive" boolean,
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

CREATE TABLE public."UserEmail"
(
    id bigserial NOT NULL,
    "fkUser" bigint,
    "stEmail" character varying(250),
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."UserEmail"
    OWNER to postgres;

CREATE TABLE public."UserPhone"
(
    id bigserial NOT NULL,
    "fkUser" bigint,
    "stPhone" character varying(50),
    "stDescription" character varying(50),
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."UserPhone"
    OWNER to postgres;

CREATE TABLE public."Project"
(
    id bigserial NOT NULL,
    "stName" character varying(99),
	"fkUser" bigint,
	"dtCreation" timestamp without time zone,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Project"
    OWNER to postgres;

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

CREATE TABLE public."ProjectPhase"
(
    id bigserial NOT NULL,
    "fkProject" bigint,
	"stName" character varying(99),
	"dtStart" timestamp without time zone,
	"dtEnd" timestamp without time zone,
	"bComplete" boolean,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."ProjectPhase"
    OWNER to postgres;

CREATE TABLE public."ProjectSprint"
(
    id bigserial NOT NULL,
    "fkProject" bigint,
	"fkPhase" bigint,
	"stName" character varying(200),
	"stDescription" character varying(1000),
	"dtStart" timestamp without time zone,
	"dtEnd" timestamp without time zone,
	"bComplete" boolean,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."ProjectSprint"
    OWNER to postgres;
	
CREATE TABLE public."TaskType"
(
    id bigserial NOT NULL,
    "stName" character varying(200),
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."TaskType"
    OWNER to postgres;


CREATE TABLE public."TaskCategory"
(
    id bigserial NOT NULL,
	"fkTaskType" bigint,
    "stName" character varying(200),
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."TaskCategory"
    OWNER to postgres;


CREATE TABLE public."TaskFlow"
(
    id bigserial NOT NULL,
	"fkTaskType" bigint,
	"fkTaskCategory" bigint,
    "stName" character varying(200),
	"nuOrder" bigint,
	"bForceComplete" boolean,
	"bForceOpen" boolean,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."TaskFlow"
    OWNER to postgres;
	

CREATE TABLE public."ProjectSprintVersion"
(
    id bigserial NOT NULL,
    "fkSprint" bigint,
	"stName" character varying(20),
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."ProjectSprintVersion"
    OWNER to postgres;

CREATE TABLE public."Task"
(
    id bigserial NOT NULL,
	"stTitle" character varying(200),
	"stLocalization" character varying(200),
	"stDescription" character varying(4000),
	"fkProject" bigint,
	"nuPriority" bigint,
	"fkPhase" bigint,
    "fkSprint" bigint,
	"fkUserStart" bigint,
	"fkVersion" bigint,
	"fkTaskType" bigint,
	"fkTaskCategory" bigint,
	"fkTaskFlowCurrent" bigint,
	"fkReleaseVersion" bigint,
	"fkUserResponsible" bigint,
	"dtStart" timestamp without time zone,	
	"dtLastEdit" timestamp without time zone,	
	"bComplete" boolean,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Task"
    OWNER to postgres;


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

CREATE TABLE public."TaskMessage"
(
    id bigserial NOT NULL,
	"fkTask" bigint,
	"fkUser" bigint,
	"fkCurrentFlow" bigint,
	"stMessage" character varying(999),
	"dtLog" timestamp without time zone,	
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."TaskMessage"
    OWNER to postgres;


CREATE TABLE public."TaskFlowChange"
(
    id bigserial NOT NULL,
	"fkTask" bigint,
	"fkUser" bigint,
	"fkOldFlowState" bigint,
	"fkNewFlowState" bigint,
	"dtLog" timestamp without time zone,	
	"stMessage" character varying(300),
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."TaskFlowChange"
    OWNER to postgres;

	
CREATE TABLE public."TaskTypeAccumulator"
(
    id bigserial NOT NULL,
	"fkTaskType" bigint,
	"fkTaskAccType" bigint,
	"fkTaskFlow" bigint,
	"fkTaskCategory" bigint,
	"stName" character varying(30),
	"stDisplay" character varying(30),
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."TaskTypeAccumulator"
    OWNER to postgres;
