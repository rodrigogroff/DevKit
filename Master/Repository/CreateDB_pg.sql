
CREATE TABLE IF NOT EXISTS public."User" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."User" OWNER to postgres;
ALTER TABLE public."User" ADD COLUMN if not exists "stEmail" character varying(200);
ALTER TABLE public."User" ADD COLUMN if not exists "nuTrig1_Email" int;
ALTER TABLE public."User" ADD COLUMN if not exists "nuTrig2_Email" int;
ALTER TABLE public."User" ADD COLUMN if not exists "nuTrig3_Email" int;
ALTER TABLE public."User" ADD COLUMN if not exists "nuTrig4_Email" int;
ALTER TABLE public."User" ADD COLUMN if not exists "nuTrig5_Email" int;
ALTER TABLE public."User" ADD COLUMN if not exists "nuTrig6_Email" int;
ALTER TABLE public."User" ADD COLUMN if not exists "nuTrig7_Email" int;
ALTER TABLE public."User" ADD COLUMN if not exists "nuTrig8_Email" int;
ALTER TABLE public."User" ADD COLUMN if not exists "nuTrig9_Email" int;
ALTER TABLE public."User" ADD COLUMN if not exists "nuTrig10_Email" int;
ALTER TABLE public."User" ADD COLUMN if not exists "nuTrig1_SocialID" int;
ALTER TABLE public."User" ADD COLUMN if not exists "nuTrig2_SocialID" int;
ALTER TABLE public."User" ADD COLUMN if not exists "nuTrig3_SocialID" int;
ALTER TABLE public."User" ADD COLUMN if not exists "nuTrig4_SocialID" int;
ALTER TABLE public."User" ADD COLUMN if not exists "stName" character varying(50);
ALTER TABLE public."User" ADD COLUMN if not exists "bActive" boolean;
ALTER TABLE public."User" ADD COLUMN if not exists "bAdmin" boolean;
ALTER TABLE public."User" ADD COLUMN if not exists "bTokenized" boolean;
ALTER TABLE public."User" ADD COLUMN if not exists "stSocialID" character varying(20);
ALTER TABLE public."User" ADD COLUMN if not exists "stPassword" character varying(20);
ALTER TABLE public."User" ADD COLUMN if not exists "dtLastLogin" timestamp without time zone;
ALTER TABLE public."User" ADD COLUMN if not exists "dtCreation" timestamp without time zone;
ALTER TABLE public."User" ADD COLUMN if not exists "dtTokenExpires" timestamp without time zone;
ALTER TABLE public."User" ADD COLUMN if not exists "stToken" character varying(20);
CREATE INDEX IF NOT EXISTS trigs_email ON public."User" ("nuTrig1_Email", "nuTrig2_Email", "nuTrig3_Email", "nuTrig4_Email", "nuTrig5_Email", "nuTrig6_Email", "nuTrig7_Email", "nuTrig8_Email", "nuTrig9_Email", "nuTrig10_Email");
CREATE INDEX IF NOT EXISTS trigs_sid ON public."User" ("nuTrig1_SocialID", "nuTrig2_SocialID", "nuTrig3_SocialID", "nuTrig4_SocialID");

CREATE TABLE IF NOT EXISTS public."Brand" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Brand" OWNER to postgres;
ALTER TABLE public."Brand" ADD COLUMN if not exists "stName" character varying(200);
ALTER TABLE public."Brand" ADD COLUMN if not exists "fkCountry" bigint;

CREATE TABLE IF NOT EXISTS public."Category" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Category" OWNER to postgres;
ALTER TABLE public."Category" ADD COLUMN if not exists "stName" character varying(200);
ALTER TABLE public."Category" ADD COLUMN if not exists "fkMainCategory" bigint;
ALTER TABLE public."Category" ADD COLUMN if not exists "fkCountry" bigint;

CREATE TABLE IF NOT EXISTS public."Country" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Country" OWNER to postgres;
ALTER TABLE public."Country" ADD COLUMN if not exists "stName" character varying(200);

CREATE TABLE IF NOT EXISTS public."State" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."State" OWNER to postgres;
ALTER TABLE public."State" ADD COLUMN if not exists "stName" character varying(200);
ALTER TABLE public."State" ADD COLUMN if not exists "fkCountry" bigint;

CREATE TABLE IF NOT EXISTS public."Product" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Product" OWNER to postgres;
ALTER TABLE public."Product" ADD COLUMN if not exists "stName" character varying(200);
ALTER TABLE public."Product" ADD COLUMN if not exists "fkCountry" bigint;
ALTER TABLE public."Product" ADD COLUMN if not exists "fkBrand" bigint;
ALTER TABLE public."Product" ADD COLUMN if not exists "fkCategory" bigint;
ALTER TABLE public."Product" ADD COLUMN if not exists "nuPrice" bigint;
ALTER TABLE public."Product" ADD COLUMN if not exists "stDesc" character varying(2000);

CREATE TABLE IF NOT EXISTS public."ProductPromotion" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."ProductPromotion" OWNER to postgres;
ALTER TABLE public."ProductPromotion" ADD COLUMN if not exists "fkProduct" bigint;
ALTER TABLE public."ProductPromotion" ADD COLUMN if not exists "stName" character varying(200);
ALTER TABLE public."ProductPromotion" ADD COLUMN if not exists "nuPctDesc" bigint;
ALTER TABLE public."ProductPromotion" ADD COLUMN if not exists "dtCreation" timestamp without time zone;
ALTER TABLE public."ProductPromotion" ADD COLUMN if not exists "dtExpires" timestamp without time zone;
ALTER TABLE public."ProductPromotion" ADD COLUMN if not exists "bPaused" boolean;

CREATE TABLE IF NOT EXISTS public."Store" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Store" OWNER to postgres;
ALTER TABLE public."Store" ADD COLUMN if not exists "fkCountry" bigint;
ALTER TABLE public."Store" ADD COLUMN if not exists "stName" character varying(500);
ALTER TABLE public."Store" ADD COLUMN if not exists "stLocalization" character varying(900);

CREATE TABLE IF NOT EXISTS public."ProductStock" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."ProductStock" OWNER to postgres;
ALTER TABLE public."ProductStock" ADD COLUMN if not exists "fkCountry" bigint;
ALTER TABLE public."ProductStock" ADD COLUMN if not exists "fkProduct" bigint;
ALTER TABLE public."ProductStock" ADD COLUMN if not exists "fkStore" bigint;

CREATE TABLE IF NOT EXISTS public."CategoryPromotion" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."CategoryPromotion" OWNER to postgres;
ALTER TABLE public."CategoryPromotion" ADD COLUMN if not exists "fkCategory" bigint;
ALTER TABLE public."CategoryPromotion" ADD COLUMN if not exists "stName" character varying(200);
ALTER TABLE public."CategoryPromotion" ADD COLUMN if not exists "nuPctDesc" bigint;
ALTER TABLE public."CategoryPromotion" ADD COLUMN if not exists "dtCreation" timestamp without time zone;
ALTER TABLE public."CategoryPromotion" ADD COLUMN if not exists "dtExpires" timestamp without time zone;
ALTER TABLE public."CategoryPromotion" ADD COLUMN if not exists "bPaused" boolean;

CREATE TABLE IF NOT EXISTS public."ProductSale" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."ProductSale" OWNER to postgres;
ALTER TABLE public."ProductSale" ADD COLUMN if not exists "fkCountry" bigint;
ALTER TABLE public."ProductSale" ADD COLUMN if not exists "fkProduct" bigint;
ALTER TABLE public."ProductSale" ADD COLUMN if not exists "fkCategory" bigint;
ALTER TABLE public."ProductSale" ADD COLUMN if not exists "fkUser" bigint;
ALTER TABLE public."ProductSale" ADD COLUMN if not exists "fkStore" bigint;
ALTER TABLE public."ProductSale" ADD COLUMN if not exists "nuPrice" bigint;
ALTER TABLE public."ProductSale" ADD COLUMN if not exists "nuYear" int;
ALTER TABLE public."ProductSale" ADD COLUMN if not exists "nuMonth" int;
ALTER TABLE public."ProductSale" ADD COLUMN if not exists "nuDay" int;
ALTER TABLE public."ProductSale" ADD COLUMN if not exists "dtSale" timestamp without time zone;

CREATE TABLE IF NOT EXISTS public."ProductCatalog" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."ProductCatalog" OWNER to postgres;
ALTER TABLE public."ProductCatalog" ADD COLUMN if not exists "stTag" character varying(25); -- Livro
CREATE INDEX IF NOT EXISTS tags ON public."ProductCatalog" USING hash ("stTag");

CREATE TABLE IF NOT EXISTS public."ProductCatalogLink" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."ProductCatalogLink" OWNER to postgres;
ALTER TABLE public."ProductCatalogLink" ADD COLUMN if not exists "fkProduct" bigint; -- 1
ALTER TABLE public."ProductCatalogLink" ADD COLUMN if not exists "fkProductCatalog" bigint; -- 1
CREATE INDEX IF NOT EXISTS fks ON public."ProductCatalogLink" ("fkProduct", "fkProductCatalog" );
CREATE INDEX IF NOT EXISTS prods ON public."ProductCatalogLink" USING hash ("fkProduct");
