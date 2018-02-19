-- 01/01/2018

INSERT INTO public."Setup"( "stPhoneMask", "stDateFormat", "stProtocolFormat") VALUES ( '(99) 999999999', 'dd/MM/yyyy HH:mm', '9999.9999' );
INSERT INTO public."Empresa"("nuEmpresa", "nuDiaFech", "stSigla", "stNome") VALUES ( '1800', 5, 'FUMAM', 'FUMAM Tramandaí');
INSERT INTO public."Profile"( "fkEmpresa", "stName", "stPermissions") VALUES (1, 'Administrador', '|3011||3012||3013||3014||4013||4023||4033||4043||5011||5012||6011||6021||1011||1012||1013||1014||1021||1022||1023||1024||1025||2001||2011|');
INSERT INTO public."User"( "fkEmpresa", "stLogin", "stPassword", "bActive", "fkProfile", "dtLastLogin", "dtCreation") VALUES ( 1, 'adm1800','adm1800', true, 1, null, null);
