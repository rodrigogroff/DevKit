
CREATE TABLE "Perfil" (
    "Id" bigint NOT NULL,
    "StNome" character varying(150),
    "StPermissoes" character varying(4000)
);

ALTER TABLE "Perfil" OWNER TO postgres;

CREATE SEQUENCE "Perfil_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER TABLE "Perfil_Id_seq" OWNER TO postgres;

ALTER SEQUENCE "Perfil_Id_seq" OWNED BY "Perfil"."Id";

CREATE TABLE "UsuarioTelefone" (
    "Id" bigint NOT NULL,
    "StTelefone" character varying(20),
    "StLocal" character varying(100),
    "FkUsuario" bigint
);

ALTER TABLE "UsuarioTelefone" OWNER TO postgres;

CREATE SEQUENCE "Telefone_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER TABLE "Telefone_Id_seq" OWNER TO postgres;

ALTER SEQUENCE "Telefone_Id_seq" OWNED BY "UsuarioTelefone"."Id";

CREATE TABLE "Usuario" (
    "Id" bigint NOT NULL,
    "StLogin" character varying(100),
    "StPassword" character varying(100),
    "bAtivo" boolean,
    "FkPerfil" bigint,
    "DtUltimoLogin" timestamp without time zone
);

ALTER TABLE "Usuario" OWNER TO postgres;

CREATE TABLE "UsuarioEmail" (
    "Id" bigint NOT NULL,
    "StEmail" character varying(250),
    "FkUsuario" bigint
);

ALTER TABLE "UsuarioEmail" OWNER TO postgres;

CREATE SEQUENCE "UsuarioEmail_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER TABLE "UsuarioEmail_Id_seq" OWNER TO postgres;

ALTER SEQUENCE "UsuarioEmail_Id_seq" OWNED BY "UsuarioEmail"."Id";

CREATE SEQUENCE "Usuario_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER TABLE "Usuario_Id_seq" OWNER TO postgres;

ALTER SEQUENCE "Usuario_Id_seq" OWNED BY "Usuario"."Id";

ALTER TABLE ONLY "Perfil" ALTER COLUMN "Id" SET DEFAULT nextval('"Perfil_Id_seq"'::regclass);

ALTER TABLE ONLY "Usuario" ALTER COLUMN "Id" SET DEFAULT nextval('"Usuario_Id_seq"'::regclass);

ALTER TABLE ONLY "UsuarioEmail" ALTER COLUMN "Id" SET DEFAULT nextval('"UsuarioEmail_Id_seq"'::regclass);

ALTER TABLE ONLY "UsuarioTelefone" ALTER COLUMN "Id" SET DEFAULT nextval('"Telefone_Id_seq"'::regclass);

ALTER TABLE ONLY "Perfil"
    ADD CONSTRAINT "Perfil_pkey" PRIMARY KEY ("Id");

ALTER TABLE ONLY "UsuarioTelefone"
    ADD CONSTRAINT "Telefone_pkey" PRIMARY KEY ("Id");

ALTER TABLE ONLY "UsuarioEmail"
    ADD CONSTRAINT "UsuarioEmail_pkey" PRIMARY KEY ("Id");

ALTER TABLE ONLY "Usuario"
    ADD CONSTRAINT "Usuario_pkey" PRIMARY KEY ("Id");

CREATE INDEX "fki_Usuario" ON "UsuarioTelefone" USING btree ("FkUsuario");

CREATE INDEX "fki_UsuarioEmail" ON "UsuarioEmail" USING btree ("FkUsuario");

CREATE INDEX "fki_UsuarioPerfil" ON "Usuario" USING btree ("FkPerfil");

ALTER TABLE ONLY "Usuario"
    ADD CONSTRAINT "Perfil" FOREIGN KEY ("FkPerfil") REFERENCES "Perfil"("Id");

ALTER TABLE ONLY "UsuarioTelefone"
    ADD CONSTRAINT "Usuario" FOREIGN KEY ("FkUsuario") REFERENCES "Usuario"("Id");

ALTER TABLE ONLY "UsuarioEmail"
    ADD CONSTRAINT "fkUsuarioEmail" FOREIGN KEY ("FkUsuario") REFERENCES "Usuario"("Id");


ALTER TABLE public."UsuarioEmail"
    ADD COLUMN "DtCriacao" timestamp without time zone;
