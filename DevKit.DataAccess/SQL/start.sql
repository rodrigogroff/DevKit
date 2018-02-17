-- 01/01/2018

INSERT INTO public."Setup"( "stPhoneMask", "stDateFormat", "stProtocolFormat") VALUES ( '(99) 999999999', 'dd/MM/yyyy HH:mm', '9999.9999' );

INSERT INTO public."Empresa"("nuEmpresa", "nuDiaFech", "stSigla", "stNome") VALUES ( '1801', 5, 'FUMAM Ativos', 'FUMAM Tramandaí Ativos');
INSERT INTO public."Empresa"("nuEmpresa", "nuDiaFech", "stSigla", "stNome") VALUES ( '1802', 5, 'FUMAM Inativos', 'FUMAM Tramandaí Inativos');

INSERT INTO public."Profile"( "fkEmpresa", "stName", "stPermissions") VALUES (1, 'Administrador', '|3011||3012||3013||3014||4013||4023||4033||4043||5011||5012||6011||6021||1011||1012||1013||1014||1021||1022||1023||1024||1025||2001||2011|');

INSERT INTO public."User"( "fkEmpresa", "stLogin", "stPassword", "bActive", "fkProfile", "dtLastLogin", "dtCreation") VALUES ( 0, 'dba','superdba@2018', true, 0, null, null);
INSERT INTO public."User"( "fkEmpresa", "stLogin", "stPassword", "bActive", "fkProfile", "dtLastLogin", "dtCreation") VALUES ( 1, 'adm1801','adm1801', true, 1, null, null);
INSERT INTO public."User"( "fkEmpresa", "stLogin", "stPassword", "bActive", "fkProfile", "dtLastLogin", "dtCreation") VALUES ( 2, 'adm1802','adm1802', true, 1, null, null);

-- 25/01/2018

INSERT INTO public."Empresa"("nuEmpresa", "nuDiaFech", "stSigla", "stNome") VALUES ( '1803', 5, 'FUMAM Vereadores', 'FUMAM Tramandaí Câmara Vereadores');
INSERT INTO public."Empresa"("nuEmpresa", "nuDiaFech", "stSigla", "stNome") VALUES ( '2222', 5, 'ConveyTESTES', 'CNET Testes');

INSERT INTO public."Profile"( "fkEmpresa", "stName", "stPermissions") VALUES (2, 'Administrador', '|3011||3012||3013||3014||4013||4023||4033||4043||5011||5012||6011||6021||1011||1012||1013||1014||1021||1022||1023||1024||1025||2001||2011|');
INSERT INTO public."Profile"( "fkEmpresa", "stName", "stPermissions") VALUES (3, 'Administrador', '|3011||3012||3013||3014||4013||4023||4033||4043||5011||5012||6011||6021||1011||1012||1013||1014||1021||1022||1023||1024||1025||2001||2011|');

INSERT INTO public."User"( "fkEmpresa", "stLogin", "stPassword", "bActive", "fkProfile", "dtLastLogin", "dtCreation") VALUES ( 3, 'adm1803','adm1803', true, 1, null, null);
INSERT INTO public."User"( "fkEmpresa", "stLogin", "stPassword", "bActive", "fkProfile", "dtLastLogin", "dtCreation") VALUES ( 4, 'adm1803','adm1803', true, 1, null, null);

INSERT INTO public."Profile"( "fkEmpresa", "stName", "stPermissions") VALUES (4, 'Administrador', '|3011||3012||3013||3014||4013||4023||4033||4043||5011||5012||6011||6021||1011||1012||1013||1014||1021||1022||1023||1024||1025||2001||2011|');
