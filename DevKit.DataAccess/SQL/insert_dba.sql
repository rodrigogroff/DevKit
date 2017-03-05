
INSERT INTO public."Setup"( "stPhoneMask") VALUES ( '(99) 9999999', 'dd/MM/yyyy HH:mm' );
INSERT INTO public."Profile"( "stName", "stPermissions") VALUES ( 'Administrator', '|1002||1003||1011||1012||1013||1014||1015||1021||1022||1023||1024||1025||1031||1032||1033||1034||1035|');
INSERT INTO public."User"( "stLogin", "stPassword", "bActive", "fkProfile", "dtLastLogin", "dtCreation") VALUES ( 'dba','dba', true, 1, null, null);
