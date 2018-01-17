﻿
INSERT INTO public."Setup"( "stPhoneMask", "stDateFormat", "stProtocolFormat") VALUES ( '(99) 999999999', 'dd/MM/yyyy HH:mm', '9999.9999' );

INSERT INTO public."Empresa"("nuEmpresa", "nuDiaFech", "stSigla", "stNome") VALUES ( '1801', 5, 'FUMAM Ativos', 'FUMAM Tramandaí Ativos');
INSERT INTO public."Empresa"("nuEmpresa", "nuDiaFech", "stSigla", "stNome") VALUES ( '1802', 5, 'FUMAM Inativos', 'FUMAM Tramandaí Inativos');

INSERT INTO public."Profile"( "fkEmpresa", "stName", "stPermissions") VALUES (1, 'Administrador', '|1002||1003||1011||1012||1013||1014||1015||1021||1022||1023||1024||1025||1031||1032||1033||1034||1035||1041||1042||1043||1044||1045||1051||1052||1053||1054||1055||1061||1062||1063||1064||1065||1181||1182||1183||1184||1185||1191||1192||1193||1194||1195||1201||1202||1203||1204||1205||1211||1212||1213||1214||1215||1071||1081||1091||1101||1111||1121||1131||1141||1151||1161||1171||2001||2011|');
INSERT INTO public."Profile"( "fkEmpresa","stName", "stPermissions") VALUES ( 1,'Gerente de Projeto', '|1002||1003||1011||1012||1013||1014||1015||1021||1022||1023||1024||1025||1031||1032||1033||1034||1035||1041||1042||1043||1044||1045||1051||1052||1053||1054||1055||1061||1062||1063||1064||1065||1071||1081||1091||1101||2001||2011||1181||1182||1183||1184||1185||1191||1192||1193||1194||1195|');
INSERT INTO public."Profile"( "fkEmpresa","stName", "stPermissions") VALUES (1, 'Analista', '|1002||1003||1011||1012||1013||1014||1015||1021||1022||1023||1024||1025||1031||1032||1033||1034||1035||1041||1042||1043||1044||1045||1051||1052||1053||1054||1055||1061||1062||1063||1064||1065||1071||1081||1091||1101||2001||2011||1181||1182||1183||1184||1185||1191||1192||1193||1194||1195|');
INSERT INTO public."Profile"( "fkEmpresa","stName", "stPermissions") VALUES (1, 'Desenvolvedor', '|1002||1003||1011||1012||1013||1014||1015||1021||1022||1023||1024||1025||1031||1032||1033||1034||1035||1041||1042||1043||1044||1045||1051||1052||1053||1054||1055||1061||1062||1063||1064||1065||1071||1081||1091||1101||2001||2011||1181||1182||1183||1184||1185||1191||1192||1193||1194||1195|');
INSERT INTO public."Profile"( "fkEmpresa","stName", "stPermissions") VALUES (1, 'Tester', '|1002||1003||1011||1012||1013||1014||1015||1021||1022||1023||1024||1025||1031||1032||1033||1034||1035||1041||1042||1043||1044||1045||1051||1052||1053||1054||1055||1061||1062||1063||1064||1065||1071||1081||1091||1101||2001||2011||1181||1182||1183||1184||1185||1191||1192||1193||1194||1195|');

INSERT INTO public."User"( "fkEmpresa", "stLogin", "stPassword", "bActive", "fkProfile", "dtLastLogin", "dtCreation") VALUES ( 0, 'dba','superdba@2018', true, 0, null, null);
INSERT INTO public."User"( "fkEmpresa", "stLogin", "stPassword", "bActive", "fkProfile", "dtLastLogin", "dtCreation") VALUES ( 1, 'adm1801','adm1801', true, 1, null, null);
INSERT INTO public."User"( "fkEmpresa", "stLogin", "stPassword", "bActive", "fkProfile", "dtLastLogin", "dtCreation") VALUES ( 2, 'adm1802','adm1802', true, 1, null, null);
