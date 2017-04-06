
TRUNCATE TABLE public."Profile" RESTART IDENTITY;
TRUNCATE TABLE public."Project" RESTART IDENTITY;
TRUNCATE TABLE public."ProjectPhase" RESTART IDENTITY;
TRUNCATE TABLE public."ProjectSprint" RESTART IDENTITY;
TRUNCATE TABLE public."ProjectSprintVersion" RESTART IDENTITY;
TRUNCATE TABLE public."ProjectUser" RESTART IDENTITY;
TRUNCATE TABLE public."Setup" RESTART IDENTITY;
TRUNCATE TABLE public."Task" RESTART IDENTITY;
TRUNCATE TABLE public."TaskAccumulatorValue" RESTART IDENTITY;
TRUNCATE TABLE public."TaskCategory" RESTART IDENTITY;
TRUNCATE TABLE public."TaskFlow" RESTART IDENTITY;
TRUNCATE TABLE public."TaskFlowChange" RESTART IDENTITY;
TRUNCATE TABLE public."TaskMessage" RESTART IDENTITY;
TRUNCATE TABLE public."TaskProgress" RESTART IDENTITY;
TRUNCATE TABLE public."TaskType" RESTART IDENTITY;
TRUNCATE TABLE public."TaskTypeAccumulator" RESTART IDENTITY;
TRUNCATE TABLE public."User" RESTART IDENTITY;
TRUNCATE TABLE public."UserEmail" RESTART IDENTITY;
TRUNCATE TABLE public."UserPhone" RESTART IDENTITY;

INSERT INTO public."Setup"( "stPhoneMask", "stDateFormat", "stProtocolFormat") VALUES ( '(99) 9999999', 'dd/MM/yyyy HH:mm', '9999.9999' );
INSERT INTO public."Profile"( "stName", "stPermissions") VALUES ( 'Administrator', '|1002||1003||1011||1012||1013||1014||1015||1021||1022||1023||1024||1025||1031||1032||1033||1034||1035||1041||1042||1043||1044||1045||1051||1052||1053||1054||1055||1061||1062||1063||1064||1065||1071||1081||1091|');
INSERT INTO public."User"( "stLogin", "stPassword", "bActive", "fkProfile", "dtLastLogin", "dtCreation") VALUES ( 'dba','dba', true, 1, null, null);
