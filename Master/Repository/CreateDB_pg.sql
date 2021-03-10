
--CREATE TABLE IF NOT EXISTS public."User" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
--ALTER TABLE public."User" OWNER to postgres;
--ALTER TABLE public."User" ADD COLUMN if not exists "stEmail" character varying(200);
--ALTER TABLE public."User" ADD COLUMN if not exists "nuTrig1_Email" int;
--CREATE INDEX IF NOT EXISTS trigs_email ON public."User" ("nuTrig1_Email", "nuTrig2_Email", "nuTrig3_Email", "nuTrig4_Email", "nuTrig5_Email", "nuTrig6_Email", "nuTrig7_Email", "nuTrig8_Email", "nuTrig9_Email", "nuTrig10_Email");
--CREATE INDEX IF NOT EXISTS trigs_sid ON public."User" ("nuTrig1_SocialID", "nuTrig2_SocialID", "nuTrig3_SocialID", "nuTrig4_SocialID");

