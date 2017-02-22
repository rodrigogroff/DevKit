
-- Table: public."Perfil"

-- DROP TABLE public."Perfil";

CREATE TABLE public."Perfil"
(
    "Id" bigint NOT NULL DEFAULT nextval('"Perfil_Id_seq"'::regclass),
    "StNome" character varying(150) COLLATE pg_catalog."default",
    "StPermissoes" character varying(4000) COLLATE pg_catalog."default",
    CONSTRAINT "Perfil_pkey" PRIMARY KEY ("Id")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Perfil"
    OWNER to postgres;

-- Table: public."Usuario"

-- DROP TABLE public."Usuario";

CREATE TABLE public."Usuario"
(
    "Id" bigint NOT NULL DEFAULT nextval('"Usuario_Id_seq"'::regclass),
    "StLogin" character varying(100) COLLATE pg_catalog."default",
    "StPassword" character varying(100) COLLATE pg_catalog."default",
    "bAtivo" boolean,
    "FkPerfil" bigint,
    "DtUltimoLogin" timestamp without time zone,
    CONSTRAINT "Usuario_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "Perfil" FOREIGN KEY ("FkPerfil")
        REFERENCES public."Perfil" ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."Usuario"
    OWNER to postgres;

-- Index: fki_UsuarioPerfil

-- DROP INDEX public."fki_UsuarioPerfil";

CREATE INDEX "fki_UsuarioPerfil"
    ON public."Usuario" USING btree
    (FkPerfil)
    TABLESPACE pg_default;

