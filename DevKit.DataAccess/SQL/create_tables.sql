
CREATE TABLE IF NOT EXISTS public."Estado" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Estado" OWNER to postgres;
ALTER TABLE public."Estado" ADD COLUMN if not exists "stSigla" character varying(20);
ALTER TABLE public."Estado" ADD COLUMN if not exists "stNome" character varying(200);

CREATE TABLE IF NOT EXISTS public."Cidade" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Cidade" OWNER to postgres;
ALTER TABLE public."Cidade" ADD COLUMN if not exists "fkEstado" bigint;
ALTER TABLE public."Cidade" ADD COLUMN if not exists "stNome" character varying(200);

CREATE TABLE IF NOT EXISTS public."Setup" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Setup" OWNER to postgres;
ALTER TABLE public."Setup" ADD COLUMN if not exists "stPhoneMask" character varying(99);
ALTER TABLE public."Setup" ADD COLUMN if not exists "stDateFormat" character varying(99);
ALTER TABLE public."Setup" ADD COLUMN if not exists "stProtocolFormat" character varying(20);

CREATE TABLE IF NOT EXISTS public."Profile" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Profile" OWNER to postgres;
ALTER TABLE public."Profile" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."Profile" ADD COLUMN if not exists "stName" character varying(200);
ALTER TABLE public."Profile" ADD COLUMN if not exists "stPermissions" character varying(9999);

CREATE TABLE IF NOT EXISTS public."User" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."User" OWNER to postgres;
ALTER TABLE public."User" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."User" ADD COLUMN if not exists "bActive" boolean;
ALTER TABLE public."User" ADD COLUMN if not exists "stLogin" character varying(200);
ALTER TABLE public."User" ADD COLUMN if not exists "stName" character varying(200);
ALTER TABLE public."User" ADD COLUMN if not exists "stPassword" character varying(30);
ALTER TABLE public."User" ADD COLUMN if not exists "fkProfile" bigint;
ALTER TABLE public."User" ADD COLUMN if not exists "fkPerson" bigint;
ALTER TABLE public."User" ADD COLUMN if not exists "dtLastLogin" timestamp without time zone;
ALTER TABLE public."User" ADD COLUMN if not exists "dtCreation" timestamp without time zone;
ALTER TABLE public."User" ADD COLUMN if not exists "stCurrentSession" character varying(20);
   
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
ALTER TABLE public."Project" ADD COLUMN if not exists "fkEmpresa" bigint;
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
ALTER TABLE public."TaskType" ADD COLUMN if not exists "fkEmpresa" bigint;
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

CREATE TABLE IF NOT EXISTS public."TaskCheckPoint" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."TaskCheckPoint" OWNER to postgres;
ALTER TABLE public."TaskCheckPoint" ADD COLUMN if not exists "stName" character varying(50);
ALTER TABLE public."TaskCheckPoint" ADD COLUMN if not exists "fkCategory" bigint;
ALTER TABLE public."TaskCheckPoint" ADD COLUMN if not exists "bMandatory" boolean;

CREATE TABLE IF NOT EXISTS public."TaskCheckPointMark" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."TaskCheckPointMark" OWNER to postgres;
ALTER TABLE public."TaskCheckPointMark" ADD COLUMN if not exists "fkCheckPoint" bigint;
ALTER TABLE public."TaskCheckPointMark" ADD COLUMN if not exists "fkTask" bigint;
ALTER TABLE public."TaskCheckPointMark" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."TaskCheckPointMark" ADD COLUMN if not exists "dtLog" timestamp without time zone;

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
ALTER TABLE public."Task" ADD COLUMN if not exists "fkEmpresa" bigint;
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
ALTER TABLE public."AuditLog" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."AuditLog" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."AuditLog" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."AuditLog" ADD COLUMN if not exists "fkActionLog" bigint;
ALTER TABLE public."AuditLog" ADD COLUMN if not exists "nuType" bigint;
ALTER TABLE public."AuditLog" ADD COLUMN if not exists "fkTarget" bigint;
ALTER TABLE public."AuditLog" ADD COLUMN if not exists "stLog" character varying(999);
ALTER TABLE public."AuditLog" ADD COLUMN if not exists "stDetailLog" character varying(3999);

CREATE TABLE IF NOT EXISTS public."Survey" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Survey" OWNER to postgres;
ALTER TABLE public."Survey" ADD COLUMN if not exists "fkEmpresa" bigint;
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
ALTER TABLE public."CompanyNews" ADD COLUMN if not exists "fkEmpresa" bigint;
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
	
CREATE TABLE IF NOT EXISTS public."TaskDependency" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."TaskDependency" OWNER to postgres;
ALTER TABLE public."TaskDependency" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."TaskDependency" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."TaskDependency" ADD COLUMN if not exists "fkMainTask" bigint;
ALTER TABLE public."TaskDependency" ADD COLUMN if not exists "fkSubTask" bigint;

CREATE TABLE IF NOT EXISTS public."TaskQuestion" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."TaskQuestion" OWNER to postgres;
ALTER TABLE public."TaskQuestion" ADD COLUMN if not exists "dtOpen" timestamp without time zone;
ALTER TABLE public."TaskQuestion" ADD COLUMN if not exists "dtClosed" timestamp without time zone;
ALTER TABLE public."TaskQuestion" ADD COLUMN if not exists "stStatement" character varying(2000);
ALTER TABLE public."TaskQuestion" ADD COLUMN if not exists "stAnswer" character varying(2000);
ALTER TABLE public."TaskQuestion" ADD COLUMN if not exists "fkTask" bigint;
ALTER TABLE public."TaskQuestion" ADD COLUMN if not exists "fkUserOpen" bigint;
ALTER TABLE public."TaskQuestion" ADD COLUMN if not exists "fkUserDirected" bigint;
ALTER TABLE public."TaskQuestion" ADD COLUMN if not exists "bFinal" boolean;

CREATE TABLE IF NOT EXISTS public."TaskCustomStep" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."TaskCustomStep" OWNER to postgres;
ALTER TABLE public."TaskCustomStep" ADD COLUMN if not exists "fkTask" bigint;
ALTER TABLE public."TaskCustomStep" ADD COLUMN if not exists "bSelected" boolean;
ALTER TABLE public."TaskCustomStep" ADD COLUMN if not exists "stName" character varying(150);
ALTER TABLE public."TaskCustomStep" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."TaskCustomStep" ADD COLUMN if not exists "dtLog" timestamp without time zone;

---------------------------------
-- portal saude
---------------------------------

CREATE TABLE IF NOT EXISTS public."Empresa" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Empresa" OWNER to postgres;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "nuEmpresa" bigint;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "nuDiaFech" bigint;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stSigla" character varying(20);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stNome" character varying(200);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stCnpj" character varying(20);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "nuMaxConsultas" bigint;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "nuCarenciaMeses" bigint;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "vrMaxProcSemAut" bigint;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "nuDiasReconsulta" bigint;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "vrCobMensalidade" bigint;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "vrCobCartaoBase" bigint;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "vrCobCartaoAtivo" bigint;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "vrCobAutorizacao" bigint;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "vrCobNovoCartao" bigint;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "vrCobServBloq" bigint;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "vrCobServDesbloq" bigint;

CREATE TABLE IF NOT EXISTS public."EmpresaSecao" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."EmpresaSecao" OWNER to postgres;
ALTER TABLE public."EmpresaSecao" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."EmpresaSecao" ADD COLUMN if not exists "nuEmpresa" bigint;
ALTER TABLE public."EmpresaSecao" ADD COLUMN if not exists "stDesc" character varying(150);

CREATE TABLE IF NOT EXISTS public."EmpresaEmail" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."EmpresaEmail" OWNER to postgres;
ALTER TABLE public."EmpresaEmail" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."EmpresaEmail" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."EmpresaEmail" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."EmpresaEmail" ADD COLUMN if not exists "stEmail" character varying(250);
ALTER TABLE public."EmpresaEmail" ADD COLUMN if not exists "stContato" character varying(250);

CREATE TABLE IF NOT EXISTS public."EmpresaConsultaAno" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."EmpresaConsultaAno" OWNER to postgres;
ALTER TABLE public."EmpresaConsultaAno" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."EmpresaConsultaAno" ADD COLUMN if not exists "nuAno" bigint;
ALTER TABLE public."EmpresaConsultaAno" ADD COLUMN if not exists "vrPreco1" bigint;
ALTER TABLE public."EmpresaConsultaAno" ADD COLUMN if not exists "vrPreco2" bigint;
ALTER TABLE public."EmpresaConsultaAno" ADD COLUMN if not exists "vrPreco3" bigint;
ALTER TABLE public."EmpresaConsultaAno" ADD COLUMN if not exists "vrPreco4" bigint;
ALTER TABLE public."EmpresaConsultaAno" ADD COLUMN if not exists "vrPreco5" bigint;
ALTER TABLE public."EmpresaConsultaAno" ADD COLUMN if not exists "vrPreco6" bigint;
ALTER TABLE public."EmpresaConsultaAno" ADD COLUMN if not exists "vrPreco7" bigint;
ALTER TABLE public."EmpresaConsultaAno" ADD COLUMN if not exists "vrPreco8" bigint;
ALTER TABLE public."EmpresaConsultaAno" ADD COLUMN if not exists "vrPreco9" bigint;

CREATE TABLE IF NOT EXISTS public."EmpresaTelefone" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."EmpresaTelefone" OWNER to postgres;
ALTER TABLE public."EmpresaTelefone" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."EmpresaTelefone" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."EmpresaTelefone" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."EmpresaTelefone" ADD COLUMN if not exists "stTelefone" character varying(50);
ALTER TABLE public."EmpresaTelefone" ADD COLUMN if not exists "stContato" character varying(50);
ALTER TABLE public."EmpresaTelefone" ADD COLUMN if not exists "stCargo" character varying(50);
ALTER TABLE public."EmpresaTelefone" ADD COLUMN if not exists "stDesc" character varying(50);

CREATE TABLE IF NOT EXISTS public."EmpresaEndereco" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."EmpresaEndereco" OWNER to postgres;
ALTER TABLE public."EmpresaEndereco" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."EmpresaEndereco" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."EmpresaEndereco" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."EmpresaEndereco" ADD COLUMN if not exists "fkEstado" bigint;
ALTER TABLE public."EmpresaEndereco" ADD COLUMN if not exists "fkCidade" bigint;
ALTER TABLE public."EmpresaEndereco" ADD COLUMN if not exists "stRua" character varying(150);
ALTER TABLE public."EmpresaEndereco" ADD COLUMN if not exists "stNumero" character varying(50);
ALTER TABLE public."EmpresaEndereco" ADD COLUMN if not exists "stComplemento" character varying(50);
ALTER TABLE public."EmpresaEndereco" ADD COLUMN if not exists "stReferencia" character varying(150);
ALTER TABLE public."EmpresaEndereco" ADD COLUMN if not exists "stCEP" character varying(50);

CREATE TABLE IF NOT EXISTS public."Associado" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Associado" OWNER to postgres;
ALTER TABLE public."Associado" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."Associado" ADD COLUMN if not exists "fkSecao" bigint;
ALTER TABLE public."Associado" ADD COLUMN if not exists "nuMatricula" bigint;
ALTER TABLE public."Associado" ADD COLUMN if not exists "nuTitularidade" bigint;
ALTER TABLE public."Associado" ADD COLUMN if not exists "nuViaCartao" bigint;
ALTER TABLE public."Associado" ADD COLUMN if not exists "dtStart" timestamp without time zone;
ALTER TABLE public."Associado" ADD COLUMN if not exists "dtLastUpdate" timestamp without time zone;
ALTER TABLE public."Associado" ADD COLUMN if not exists "dtLastContact" timestamp without time zone;
ALTER TABLE public."Associado" ADD COLUMN if not exists "fkUserAdd" bigint;
ALTER TABLE public."Associado" ADD COLUMN if not exists "fkUserLastUpdate" bigint;
ALTER TABLE public."Associado" ADD COLUMN if not exists "fkUserLastContact" bigint;
ALTER TABLE public."Associado" ADD COLUMN if not exists "stName" character varying(150);
ALTER TABLE public."Associado" ADD COLUMN if not exists "stAlias" character varying(150);
ALTER TABLE public."Associado" ADD COLUMN if not exists "nuMonthAniversary" bigint;
ALTER TABLE public."Associado" ADD COLUMN if not exists "nuDayAniversary" bigint;
ALTER TABLE public."Associado" ADD COLUMN if not exists "nuYearBirth" bigint;
ALTER TABLE public."Associado" ADD COLUMN if not exists "bDeceased" boolean;
ALTER TABLE public."Associado" ADD COLUMN if not exists "bHomem" boolean;
ALTER TABLE public."Associado" ADD COLUMN if not exists "stCPF" character varying(20);
ALTER TABLE public."Associado" ADD COLUMN if not exists "tgStatus" bigint;
ALTER TABLE public."Associado" ADD COLUMN if not exists "tgExpedicao" bigint;
ALTER TABLE public."Associado" ADD COLUMN if not exists "tgFaltaEnd" bigint;
ALTER TABLE public."Associado" ADD COLUMN if not exists "tgFaltaTelefone" bigint;
ALTER TABLE public."Associado" ADD COLUMN if not exists "stSenha" character varying(4);
ALTER TABLE public."Associado" ADD COLUMN if not exists "stPaciente" character varying(4000);
ALTER TABLE public."Associado" ADD COLUMN if not exists "nuMatSaude" bigint;

CREATE TABLE IF NOT EXISTS public."AssociadoEmail" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."AssociadoEmail" OWNER to postgres;
ALTER TABLE public."AssociadoEmail" ADD COLUMN if not exists "fkAssociado" bigint;
ALTER TABLE public."AssociadoEmail" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."AssociadoEmail" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."AssociadoEmail" ADD COLUMN if not exists "stEmail" character varying(250);

CREATE TABLE IF NOT EXISTS public."AssociadoTelefone" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."AssociadoTelefone" OWNER to postgres;
ALTER TABLE public."AssociadoTelefone" ADD COLUMN if not exists "fkAssociado" bigint;
ALTER TABLE public."AssociadoTelefone" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."AssociadoTelefone" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."AssociadoTelefone" ADD COLUMN if not exists "stPhone" character varying(50);
ALTER TABLE public."AssociadoTelefone" ADD COLUMN if not exists "stDescription" character varying(50);

CREATE TABLE IF NOT EXISTS public."AssociadoEndereco" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."AssociadoEndereco" OWNER to postgres;
ALTER TABLE public."AssociadoEndereco" ADD COLUMN if not exists "fkAssociado" bigint;
ALTER TABLE public."AssociadoEndereco" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."AssociadoEndereco" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."AssociadoEndereco" ADD COLUMN if not exists "fkEstado" bigint;
ALTER TABLE public."AssociadoEndereco" ADD COLUMN if not exists "fkCidade" bigint;
ALTER TABLE public."AssociadoEndereco" ADD COLUMN if not exists "stRua" character varying(150);
ALTER TABLE public."AssociadoEndereco" ADD COLUMN if not exists "stNumero" character varying(50);
ALTER TABLE public."AssociadoEndereco" ADD COLUMN if not exists "stComplemento" character varying(50);
ALTER TABLE public."AssociadoEndereco" ADD COLUMN if not exists "stReferencia" character varying(150);
ALTER TABLE public."AssociadoEndereco" ADD COLUMN if not exists "stCEP" character varying(50);
ALTER TABLE public."AssociadoEndereco" ADD COLUMN if not exists "bPrincipal" boolean;

CREATE TABLE IF NOT EXISTS public."AssociadoDependente" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."AssociadoDependente" OWNER to postgres;
ALTER TABLE public."AssociadoDependente" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."AssociadoDependente" ADD COLUMN if not exists "fkAssociado" bigint;
ALTER TABLE public."AssociadoDependente" ADD COLUMN if not exists "fkCartao" bigint;
ALTER TABLE public."AssociadoDependente" ADD COLUMN if not exists "stNome" character varying(35);
ALTER TABLE public."AssociadoDependente" ADD COLUMN if not exists "stCPF" character varying(30);
ALTER TABLE public."AssociadoDependente" ADD COLUMN if not exists "dtNasc" timestamp without time zone;
ALTER TABLE public."AssociadoDependente" ADD COLUMN if not exists "fkTipoCoberturaDependente" bigint;

CREATE TABLE IF NOT EXISTS public."Especialidade" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Especialidade" OWNER to postgres;
ALTER TABLE public."Especialidade" ADD COLUMN if not exists "stNome" character varying(150);

CREATE TABLE IF NOT EXISTS public."Credenciado" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Credenciado" OWNER to postgres;
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "nuCodigo" bigint;
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "stNome" character varying(150);
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "stCnpj" character varying(30);
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "dtStart" timestamp without time zone;
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "dtLastUpdate" timestamp without time zone;
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "dtLastContact" timestamp without time zone;
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "fkUserAdd" bigint;
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "fkUserLastUpdate" bigint;
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "fkUserLastContact" bigint;
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "nuMonthAniversary" bigint;
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "nuDayAniversary" bigint;
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "nuYearBirth" bigint;
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "fkEspecialidade" bigint;
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "tgMasculino" boolean;
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "nuTipo" bigint;
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "stSenha" character varying(20);
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "stConselho" character varying(4000);
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "stBanco" character varying(20);
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "stAgencia" character varying(20);
ALTER TABLE public."Credenciado" ADD COLUMN if not exists "stConta" character varying(20);

CREATE TABLE IF NOT EXISTS public."CredenciadoEmpresa" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."CredenciadoEmpresa" OWNER to postgres;
ALTER TABLE public."CredenciadoEmpresa" ADD COLUMN if not exists "fkCredenciado" bigint;
ALTER TABLE public."CredenciadoEmpresa" ADD COLUMN if not exists "fkEmpresa" bigint;

CREATE TABLE IF NOT EXISTS public."CredenciadoEmail" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."CredenciadoEmail" OWNER to postgres;
ALTER TABLE public."CredenciadoEmail" ADD COLUMN if not exists "fkCredenciado" bigint;
ALTER TABLE public."CredenciadoEmail" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."CredenciadoEmail" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."CredenciadoEmail" ADD COLUMN if not exists "stEmail" character varying(250);
ALTER TABLE public."CredenciadoEmail" ADD COLUMN if not exists "stContato" character varying(250);

CREATE TABLE IF NOT EXISTS public."CredenciadoTelefone" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."CredenciadoTelefone" OWNER to postgres;
ALTER TABLE public."CredenciadoTelefone" ADD COLUMN if not exists "fkCredenciado" bigint;
ALTER TABLE public."CredenciadoTelefone" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."CredenciadoTelefone" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."CredenciadoTelefone" ADD COLUMN if not exists "stPhone" character varying(50);
ALTER TABLE public."CredenciadoTelefone" ADD COLUMN if not exists "stDescription" character varying(50);

CREATE TABLE IF NOT EXISTS public."CredenciadoEndereco" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."CredenciadoEndereco" OWNER to postgres;
ALTER TABLE public."CredenciadoEndereco" ADD COLUMN if not exists "fkCredenciado" bigint;
ALTER TABLE public."CredenciadoEndereco" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."CredenciadoEndereco" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."CredenciadoEndereco" ADD COLUMN if not exists "fkEstado" bigint;
ALTER TABLE public."CredenciadoEndereco" ADD COLUMN if not exists "fkCidade" bigint;
ALTER TABLE public."CredenciadoEndereco" ADD COLUMN if not exists "stRua" character varying(150);
ALTER TABLE public."CredenciadoEndereco" ADD COLUMN if not exists "stNumero" character varying(50);
ALTER TABLE public."CredenciadoEndereco" ADD COLUMN if not exists "stComplemento" character varying(50);
ALTER TABLE public."CredenciadoEndereco" ADD COLUMN if not exists "stReferencia" character varying(150);
ALTER TABLE public."CredenciadoEndereco" ADD COLUMN if not exists "stCEP" character varying(50);
ALTER TABLE public."CredenciadoEndereco" ADD COLUMN if not exists "bPrincipal" boolean;

CREATE TABLE IF NOT EXISTS public."Autorizacao" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Autorizacao" OWNER to postgres;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "dtSolicitacao" timestamp without time zone;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "fkCredenciado" bigint;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "fkProcedimento" bigint;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "fkAssociado" bigint;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "nuMes" bigint;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "nuAno" bigint;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "nuNSU" bigint;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "tgSituacao" bigint;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "fkAutOriginal" bigint;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "vrProcedimento" bigint;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "vrCoPart" bigint;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "vrParcela" bigint;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "vrParcelaCoPart" bigint;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "nuTotParcelas" bigint;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "nuIndice" bigint;
ALTER TABLE public."Autorizacao" ADD COLUMN if not exists "fkAssociadoPortador" bigint;

CREATE TABLE IF NOT EXISTS public."LoteGrafica" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."LoteGrafica" OWNER to postgres;
ALTER TABLE public."LoteGrafica" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."LoteGrafica" ADD COLUMN if not exists "nuCodigo" bigint;
ALTER TABLE public."LoteGrafica" ADD COLUMN if not exists "tgAtivo" bigint;

CREATE TABLE IF NOT EXISTS public."LoteGraficaCartao" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."LoteGraficaCartao" OWNER to postgres;
ALTER TABLE public."LoteGraficaCartao" ADD COLUMN if not exists "fkLoteGrafica" bigint;
ALTER TABLE public."LoteGraficaCartao" ADD COLUMN if not exists "fkAssociado" bigint;
ALTER TABLE public."LoteGraficaCartao" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."LoteGraficaCartao" ADD COLUMN if not exists "nuVia" bigint;
ALTER TABLE public."LoteGraficaCartao" ADD COLUMN if not exists "nuTit" bigint;

CREATE TABLE IF NOT EXISTS public."TUSS" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."TUSS" OWNER to postgres;
ALTER TABLE public."TUSS" ADD COLUMN if not exists "nuCodTUSS" bigint;
ALTER TABLE public."TUSS" ADD COLUMN if not exists "stDescricaoGP" character varying(800);
ALTER TABLE public."TUSS" ADD COLUMN if not exists "stDescricaoSubGP" character varying(800);
ALTER TABLE public."TUSS" ADD COLUMN if not exists "stProcedimento" character varying(2000);

CREATE TABLE IF NOT EXISTS public."CredenciadoEmpresaTuss" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."CredenciadoEmpresaTuss" OWNER to postgres;
ALTER TABLE public."CredenciadoEmpresaTuss" ADD COLUMN if not exists "fkCredenciado" bigint;
ALTER TABLE public."CredenciadoEmpresaTuss" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."CredenciadoEmpresaTuss" ADD COLUMN if not exists "nuTUSS" bigint;
ALTER TABLE public."CredenciadoEmpresaTuss" ADD COLUMN if not exists "vrProcedimento" bigint;
ALTER TABLE public."CredenciadoEmpresaTuss" ADD COLUMN if not exists "vrCoPart" bigint;
ALTER TABLE public."CredenciadoEmpresaTuss" ADD COLUMN if not exists "nuMaxMes" bigint;
ALTER TABLE public."CredenciadoEmpresaTuss" ADD COLUMN if not exists "nuMaxAno" bigint;
ALTER TABLE public."CredenciadoEmpresaTuss" ADD COLUMN if not exists "nuParcelas" bigint;
ALTER TABLE public."CredenciadoEmpresaTuss" ADD COLUMN if not exists "tgCob" boolean;

CREATE TABLE IF NOT EXISTS public."TipoCoberturaDependente" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."TipoCoberturaDependente" OWNER to postgres;
ALTER TABLE public."TipoCoberturaDependente" ADD COLUMN if not exists "stDesc" character varying(150);

CREATE TABLE IF NOT EXISTS public."LogAutorizaProc" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."LogAutorizaProc" OWNER to postgres;
ALTER TABLE public."LogAutorizaProc" ADD COLUMN if not exists "stLog" character varying(999);
ALTER TABLE public."LogAutorizaProc" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."LogAutorizaProc" ADD COLUMN if not exists "tgErro" boolean;

CREATE TABLE IF NOT EXISTS public."SaudeUnidadeEmpresa" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SaudeUnidadeEmpresa" ADD COLUMN if not exists "stNome" character varying(99);
ALTER TABLE public."SaudeUnidadeEmpresa" ADD COLUMN if not exists "fkEmpresa" bigint;

CREATE TABLE IF NOT EXISTS public."SaudeFabricanteMaterialEmpresa" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SaudeFabricanteMaterialEmpresa" ADD COLUMN if not exists "stNome" character varying(99);
ALTER TABLE public."SaudeFabricanteMaterialEmpresa" ADD COLUMN if not exists "fkEmpresa" bigint;

CREATE TABLE IF NOT EXISTS public."SaudeFabricanteMedicamentoEmpresa" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SaudeFabricanteMedicamentoEmpresa" ADD COLUMN if not exists "stNome" character varying(99);
ALTER TABLE public."SaudeFabricanteMedicamentoEmpresa" ADD COLUMN if not exists "fkEmpresa" bigint;

CREATE TABLE IF NOT EXISTS public."SaudeOPMEClassificacaoEmpresa" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SaudeOPMEClassificacaoEmpresa" ADD COLUMN if not exists "stNome" character varying(99);
ALTER TABLE public."SaudeOPMEClassificacaoEmpresa" ADD COLUMN if not exists "fkEmpresa" bigint;

CREATE TABLE IF NOT EXISTS public."SaudeOPMEEspecialidadeEmpresa" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SaudeOPMEEspecialidadeEmpresa" ADD COLUMN if not exists "stNome" character varying(99);
ALTER TABLE public."SaudeOPMEEspecialidadeEmpresa" ADD COLUMN if not exists "fkEmpresa" bigint;

CREATE TABLE IF NOT EXISTS public."SaudePorteProcedimentoEmpresa" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SaudePorteProcedimentoEmpresa" ADD COLUMN if not exists "stNome" character varying(99);
ALTER TABLE public."SaudePorteProcedimentoEmpresa" ADD COLUMN if not exists "fkEmpresa" bigint;

CREATE TABLE IF NOT EXISTS public."SaudeValorDiaria" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SaudeValorDiaria" OWNER to postgres;
ALTER TABLE public."SaudeValorDiaria" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."SaudeValorDiaria" ADD COLUMN if not exists "nuAnoVigencia" bigint;
ALTER TABLE public."SaudeValorDiaria" ADD COLUMN if not exists "nuCodInterno" bigint;
ALTER TABLE public."SaudeValorDiaria" ADD COLUMN if not exists "stDesc" character varying(999);
ALTER TABLE public."SaudeValorDiaria" ADD COLUMN if not exists "vrNivel1" bigint;
ALTER TABLE public."SaudeValorDiaria" ADD COLUMN if not exists "vrNivel2" bigint;
ALTER TABLE public."SaudeValorDiaria" ADD COLUMN if not exists "vrNivel3" bigint;
ALTER TABLE public."SaudeValorDiaria" ADD COLUMN if not exists "vrNivel4" bigint;
ALTER TABLE public."SaudeValorDiaria" ADD COLUMN if not exists "vrNivel5" bigint;

CREATE TABLE IF NOT EXISTS public."SaudeValorMaterial" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SaudeValorMaterial" OWNER to postgres;
ALTER TABLE public."SaudeValorMaterial" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."SaudeValorMaterial" ADD COLUMN if not exists "nuAnoVigencia" bigint;
ALTER TABLE public."SaudeValorMaterial" ADD COLUMN if not exists "nuCodInterno" bigint;
ALTER TABLE public."SaudeValorMaterial" ADD COLUMN if not exists "stDesc" character varying(999);
ALTER TABLE public."SaudeValorMaterial" ADD COLUMN if not exists "stComercial" character varying(999);
ALTER TABLE public."SaudeValorMaterial" ADD COLUMN if not exists "fkFabricanteMaterial" bigint;
ALTER TABLE public."SaudeValorMaterial" ADD COLUMN if not exists "fkUnidade" bigint;
ALTER TABLE public."SaudeValorMaterial" ADD COLUMN if not exists "vrFracao" bigint;
ALTER TABLE public."SaudeValorMaterial" ADD COLUMN if not exists "bFracionar" boolean;
ALTER TABLE public."SaudeValorMaterial" ADD COLUMN if not exists "vrValor" bigint;

CREATE TABLE IF NOT EXISTS public."SaudeValorMedicamento" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SaudeValorMedicamento" OWNER to postgres;
ALTER TABLE public."SaudeValorMedicamento" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."SaudeValorMedicamento" ADD COLUMN if not exists "nuAnoVigencia" bigint;
ALTER TABLE public."SaudeValorMedicamento" ADD COLUMN if not exists "nuCodInterno" bigint;
ALTER TABLE public."SaudeValorMedicamento" ADD COLUMN if not exists "stDesc" character varying(999);
ALTER TABLE public."SaudeValorMedicamento" ADD COLUMN if not exists "stComercial" character varying(999);
ALTER TABLE public."SaudeValorMedicamento" ADD COLUMN if not exists "fkFabricanteMedicamento" bigint;
ALTER TABLE public."SaudeValorMedicamento" ADD COLUMN if not exists "vrFracao" bigint;
ALTER TABLE public."SaudeValorMedicamento" ADD COLUMN if not exists "fkUnidade" bigint;
ALTER TABLE public."SaudeValorMedicamento" ADD COLUMN if not exists "bFracionar" boolean;
ALTER TABLE public."SaudeValorMedicamento" ADD COLUMN if not exists "vrValor" bigint;

CREATE TABLE IF NOT EXISTS public."SaudeValorNaoMedico" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SaudeValorNaoMedico" OWNER to postgres;
ALTER TABLE public."SaudeValorNaoMedico" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."SaudeValorNaoMedico" ADD COLUMN if not exists "nuAnoVigencia" bigint;
ALTER TABLE public."SaudeValorNaoMedico" ADD COLUMN if not exists "nuCodInterno" bigint;
ALTER TABLE public."SaudeValorNaoMedico" ADD COLUMN if not exists "stDesc" character varying(999);
ALTER TABLE public."SaudeValorNaoMedico" ADD COLUMN if not exists "vrValor" bigint;

CREATE TABLE IF NOT EXISTS public."SaudeValorOPME" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SaudeValorOPME" OWNER to postgres;
ALTER TABLE public."SaudeValorOPME" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."SaudeValorOPME" ADD COLUMN if not exists "nuAnoVigencia" bigint;
ALTER TABLE public."SaudeValorOPME" ADD COLUMN if not exists "nuCodInterno" bigint;
ALTER TABLE public."SaudeValorOPME" ADD COLUMN if not exists "stDesc" character varying(999);
ALTER TABLE public."SaudeValorOPME" ADD COLUMN if not exists "stTecnica" character varying(999);
ALTER TABLE public."SaudeValorOPME" ADD COLUMN if not exists "fkClassificacao" bigint;
ALTER TABLE public."SaudeValorOPME" ADD COLUMN if not exists "fkEspecialidade" bigint;
ALTER TABLE public."SaudeValorOPME" ADD COLUMN if not exists "vrValor" bigint;

CREATE TABLE IF NOT EXISTS public."SaudeValorPacote" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SaudeValorPacote" OWNER to postgres;
ALTER TABLE public."SaudeValorPacote" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."SaudeValorPacote" ADD COLUMN if not exists "nuAnoVigencia" bigint;
ALTER TABLE public."SaudeValorPacote" ADD COLUMN if not exists "nuCodInterno" bigint;
ALTER TABLE public."SaudeValorPacote" ADD COLUMN if not exists "stDesc" character varying(999);
ALTER TABLE public."SaudeValorPacote" ADD COLUMN if not exists "vrValor" bigint;

CREATE TABLE IF NOT EXISTS public."SaudeValorProcedimento" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SaudeValorProcedimento" OWNER to postgres;
ALTER TABLE public."SaudeValorProcedimento" ADD COLUMN if not exists "fkEmpresa" bigint;
ALTER TABLE public."SaudeValorProcedimento" ADD COLUMN if not exists "nuAnoVigencia" bigint;
ALTER TABLE public."SaudeValorProcedimento" ADD COLUMN if not exists "nuCodInterno" bigint;
ALTER TABLE public."SaudeValorProcedimento" ADD COLUMN if not exists "stDesc" character varying(999);
ALTER TABLE public."SaudeValorProcedimento" ADD COLUMN if not exists "vrTotalHMCO" bigint;
ALTER TABLE public."SaudeValorProcedimento" ADD COLUMN if not exists "fkSaudePorteProcedimento" bigint;
ALTER TABLE public."SaudeValorProcedimento" ADD COLUMN if not exists "vrValorHM" bigint;
ALTER TABLE public."SaudeValorProcedimento" ADD COLUMN if not exists "vrValorCO" bigint;
ALTER TABLE public."SaudeValorProcedimento" ADD COLUMN if not exists "nuAux" bigint;
ALTER TABLE public."SaudeValorProcedimento" ADD COLUMN if not exists "nuAnestesistas" bigint;
ALTER TABLE public."SaudeValorProcedimento" ADD COLUMN if not exists "vrPorteAnestesista" bigint;
ALTER TABLE public."SaudeValorProcedimento" ADD COLUMN if not exists "nuFilme4C" bigint;
ALTER TABLE public."SaudeValorProcedimento" ADD COLUMN if not exists "vrFilme" bigint;

CREATE TABLE IF NOT EXISTS public."NSU" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."NSU" OWNER to postgres;
ALTER TABLE public."NSU" ADD COLUMN if not exists "dtLog" timestamp without time zone;
ALTER TABLE public."NSU" ADD COLUMN if not exists "fkEmpresa" bigint;
