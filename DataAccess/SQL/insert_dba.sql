
INSERT INTO public."Profile"( "stName", "stPermissions") VALUES ( 'Administrator', '|1011||1012||1013||1014||1015||1021||1022||1023||1024||1025|');
INSERT INTO public."User"( "stLogin", "stPassword", "bActive", "fkProfile", "dtLastLogin", "dtCreation") VALUES ( 'dba','dba', true, 1, null, null);
