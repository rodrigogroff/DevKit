
CREATE TABLE public."Setup"
(
    id bigserial NOT NULL,
    "stPhoneMask" character varying(99),
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

