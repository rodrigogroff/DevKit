
CREATE TABLE IF NOT EXISTS public."Cartao" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Cartao" OWNER to postgres;

ALTER TABLE public."Cartao" ADD COLUMN if not exists "fkEmpresa" int;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "nuMatricula" int;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "nuTitularidade" int;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stSenha" character varying(500);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "nuTipoCartao" int;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stVenctoCartao" character varying(4);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "nuStatus" int;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "nuSenhaErrada" int;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "dtInclusao" timestamp without time zone;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "dtBloqueio" timestamp without time zone;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "nuMotivoBloqueio" int;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stBanco" character varying(20);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stAgencia" character varying(20);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stConta" character varying(20);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stMatExtra" character varying(20);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stCelCartao" character varying(20);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stCpf" character varying(20);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stNome" character varying(200);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stEndereco" character varying(500);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stNumero" character varying(50);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stCompl" character varying(50);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stBairro" character varying(200);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stEstado" character varying(200);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stCidade" character varying(500);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stCEP" character varying(20);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stDDD" character varying(3);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stTelefone" character varying(20);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "dtNasc" timestamp without time zone;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "stEmail" character varying(20);
ALTER TABLE public."Cartao" ADD COLUMN if not exists "vrRenda" int;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "nuViaCartao" int;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "vrLimiteTotal" int;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "vrLimiteMensal" int;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "vrLimiteRotativo" int;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "vrCotaExtra" int;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "nuEmitido" int;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "vrSaldoConvenio" int;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "dtPedidoCartao" timestamp without time zone;
ALTER TABLE public."Cartao" ADD COLUMN if not exists "bConvenioComSaldo" boolean;

CREATE INDEX IF NOT EXISTS idx_cartao ON public."Cartao" USING btree
    ("fkEmpresa" ASC NULLS LAST, "nuMatricula" ASC NULLS LAST, "nuTitularidade" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public."ConfigPlasticoEnvio" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."ConfigPlasticoEnvio" OWNER to postgres;
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "stDias" character varying(500);
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "stHorario" character varying(99);
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "stEmails" character varying(999);
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "bAtivo" boolean;
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "dom" boolean;
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "seg" boolean;
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "ter" boolean;
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "qua" boolean;
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "qui" boolean;
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "sex" boolean;
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "sab" boolean;
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "stEmailSmtp" character varying(999);
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "stSenhaSmtp" character varying(999);
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "stHostSmtp" character varying(999);
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "nuPortSmtp" int;
ALTER TABLE public."ConfigPlasticoEnvio" ADD COLUMN if not exists "stStatus" character varying(999);

CREATE TABLE IF NOT EXISTS public."DashboardGrafico" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."DashboardGrafico" OWNER to postgres;
ALTER TABLE public."DashboardGrafico" ADD COLUMN if not exists "nuTotalTransacoes" int;
ALTER TABLE public."DashboardGrafico" ADD COLUMN if not exists "nuTotalCartoes" int;
ALTER TABLE public."DashboardGrafico" ADD COLUMN if not exists "nuTotalFinanc" int;
ALTER TABLE public."DashboardGrafico" ADD COLUMN if not exists "nuDia" int;
ALTER TABLE public."DashboardGrafico" ADD COLUMN if not exists "nuMes" int;
ALTER TABLE public."DashboardGrafico" ADD COLUMN if not exists "nuAno" int;
ALTER TABLE public."DashboardGrafico" ADD COLUMN if not exists "nuTotalLojas" int;
ALTER TABLE public."DashboardGrafico" ADD COLUMN if not exists "dtDia" timestamp without time zone;

CREATE INDEX IF NOT EXISTS idx_dashboardgrafico ON public."DashboardGrafico" USING btree
    ("nuDia" ASC NULLS LAST, "nuMes" ASC NULLS LAST, "nuAno" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public."Empresa" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Empresa" OWNER to postgres;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "nuEmpresa" int;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stCNPJ" character varying(20);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stFantasia" character varying(999);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stSocial" character varying(999);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stEndereco" character varying(999);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stCidade" character varying(999);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stEstado" character varying(2);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stCEP" character varying(20);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stTelefone" character varying(20);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "nuParcelas" int;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "fkAdmin" int;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stContaDeb" character varying(20);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "vrMensalidade" int;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "nuPctValor" int;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "vrTransacao" int;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "vrMinimo" int;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "nuFranquiaTrans" int;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "nuPeriodoFat" int;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "nuDiaVenc" int;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stBancoFat" character varying(20);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "vrCartaoAtivo" int;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stObs" character varying(500);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stHomepage" character varying(500);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "nuDiaFech" int;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stHoraFech" character varying(500);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "bConvenioSaldo" boolean;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "fkParceiro" int;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "stEmailPlastico" character varying(999);
ALTER TABLE public."Empresa" ADD COLUMN if not exists "bBlocked" boolean;
ALTER TABLE public."Empresa" ADD COLUMN if not exists "bIsentoFat" boolean;

CREATE INDEX IF NOT EXISTS idx_empresa ON public."Empresa" USING btree
    ("nuEmpresa" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public."Faturamento" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Faturamento" OWNER to postgres;
ALTER TABLE public."Faturamento" ADD COLUMN if not exists "fkEmpresa" int;
ALTER TABLE public."Faturamento" ADD COLUMN if not exists "fkLoja" int;
ALTER TABLE public."Faturamento" ADD COLUMN if not exists "vrCobranca" int;
ALTER TABLE public."Faturamento" ADD COLUMN if not exists "dtVencimento" timestamp without time zone;
ALTER TABLE public."Faturamento" ADD COLUMN if not exists "dtBaixa" timestamp without time zone;
ALTER TABLE public."Faturamento" ADD COLUMN if not exists "nuSituacao" int;
ALTER TABLE public."Faturamento" ADD COLUMN if not exists "nuRetBanco" int;

CREATE TABLE IF NOT EXISTS public."FaturamentoDetalhe" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."FaturamentoDetalhe" OWNER to postgres;
ALTER TABLE public."FaturamentoDetalhe" ADD COLUMN if not exists "fkFatura" int;
ALTER TABLE public."FaturamentoDetalhe" ADD COLUMN if not exists "nuTipoFat" int;
ALTER TABLE public."FaturamentoDetalhe" ADD COLUMN if not exists "nuQuantidade" int;
ALTER TABLE public."FaturamentoDetalhe" ADD COLUMN if not exists "vrCobranca" int;
ALTER TABLE public."FaturamentoDetalhe" ADD COLUMN if not exists "bDesconto" boolean;
ALTER TABLE public."FaturamentoDetalhe" ADD COLUMN if not exists "stExtras" character varying(100);
ALTER TABLE public."FaturamentoDetalhe" ADD COLUMN if not exists "fkEmpresa" int;
ALTER TABLE public."FaturamentoDetalhe" ADD COLUMN if not exists "fkLoja" int;

CREATE INDEX IF NOT EXISTS idx1_fatdet ON public."FaturamentoDetalhe" USING btree
    ("fkFatura" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE INDEX IF NOT EXISTS idx2_fatdet ON public."FaturamentoDetalhe" USING btree
    ("fkFatura" ASC NULLS LAST, "fkEmpresa" ASC NULLS LAST, "fkLoja" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public."JobFechamento" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."JobFechamento" OWNER to postgres;
ALTER TABLE public."JobFechamento" ADD COLUMN if not exists "fkEmpresa" int;
ALTER TABLE public."JobFechamento" ADD COLUMN if not exists "dtInicio" timestamp without time zone;
ALTER TABLE public."JobFechamento" ADD COLUMN if not exists "dtFim" timestamp without time zone;
ALTER TABLE public."JobFechamento" ADD COLUMN if not exists "nuMes" int;
ALTER TABLE public."JobFechamento" ADD COLUMN if not exists "nuAno" int;

CREATE INDEX IF NOT EXISTS idx1_jobfech ON public."JobFechamento" USING btree
    ("fkEmpresa" ASC NULLS LAST, "nuMes" ASC NULLS LAST, "nuAno" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public."LogAudit" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."LogAudit" OWNER to postgres;
ALTER TABLE public."LogAudit" ADD COLUMN if not exists "fkUsuario" int;
ALTER TABLE public."LogAudit" ADD COLUMN if not exists "nuOperacao" int;
ALTER TABLE public."LogAudit" ADD COLUMN if not exists "dtOperacao" timestamp without time zone;
ALTER TABLE public."LogAudit" ADD COLUMN if not exists "fkGeneric" int;
ALTER TABLE public."LogAudit" ADD COLUMN if not exists "fkEmpresa" int;
ALTER TABLE public."LogAudit" ADD COLUMN if not exists "stLog" character varying(4000);

CREATE INDEX IF NOT EXISTS idx_logaudit ON public."LogAudit" USING btree
    ("fkEmpresa" ASC NULLS LAST, "dtOperacao" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public."LogFechamento" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."LogFechamento" OWNER to postgres;
ALTER TABLE public."LogFechamento" ADD COLUMN if not exists "nuMes" int;
ALTER TABLE public."LogFechamento" ADD COLUMN if not exists "nuAno" int;
ALTER TABLE public."LogFechamento" ADD COLUMN if not exists "vrValor" int;
ALTER TABLE public."LogFechamento" ADD COLUMN if not exists "dtFechamento" timestamp without time zone;
ALTER TABLE public."LogFechamento" ADD COLUMN if not exists "fkEmpresa" int;
ALTER TABLE public."LogFechamento" ADD COLUMN if not exists "fkLoja" int;
ALTER TABLE public."LogFechamento" ADD COLUMN if not exists "fkCartao" int;
ALTER TABLE public."LogFechamento" ADD COLUMN if not exists "fkParcela" int;
ALTER TABLE public."LogFechamento" ADD COLUMN if not exists "dtCompra" timestamp without time zone;
ALTER TABLE public."LogFechamento" ADD COLUMN if not exists "nuParcela" int;
ALTER TABLE public."LogFechamento" ADD COLUMN if not exists "stCartao" character varying(500);

CREATE INDEX IF NOT EXISTS idx_logfechamento ON public."LogFechamento" USING btree
    ("nuMes" ASC NULLS LAST, "nuAno" ASC NULLS LAST, "fkEmpresa" ASC NULLS LAST, "fkLoja" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public."LogNsu" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."LogNsu" OWNER to postgres;
ALTER TABLE public."LogNsu" ADD COLUMN if not exists "dtLog" timestamp without time zone;

CREATE TABLE IF NOT EXISTS public."LogTransacao" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."LogTransacao" OWNER to postgres;
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "fkTerminal" int;
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "dtTransacao" timestamp without time zone;
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "nuNsu" int;
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "fkEmpresa" int;
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "fkCartao" int;
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "vrTotal" int;
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "nuParcelas" int;
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "nuCodErro" int;
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "nuConfirmada" int;
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "nuNsuOrig" int;
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "nuOperacao" int;
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "stMsg" character varying(50);
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "bContabil" boolean;
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "fkLoja" int;
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "stDoc" character varying(50);

CREATE TABLE IF NOT EXISTS public."LogTransacao" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."LogTransacao" OWNER to postgres;
ALTER TABLE public."LogTransacao" ADD COLUMN if not exists "fkTerminal" int;

CREATE TABLE IF NOT EXISTS public."Loja" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Loja" OWNER to postgres;
ALTER TABLE public."Loja" ADD COLUMN if not exists "stCNPJ" character varying(50);
ALTER TABLE public."Loja" ADD COLUMN if not exists "stNome" character varying(999);
ALTER TABLE public."Loja" ADD COLUMN if not exists "stSocial" character varying(999);
ALTER TABLE public."Loja" ADD COLUMN if not exists "stEndereco" character varying(999);
ALTER TABLE public."Loja" ADD COLUMN if not exists "stEnderecoInst" character varying(999);
ALTER TABLE public."Loja" ADD COLUMN if not exists "stInscEst" character varying(999);
ALTER TABLE public."Loja" ADD COLUMN if not exists "stCidade" character varying(999);
ALTER TABLE public."Loja" ADD COLUMN if not exists "stEstado" character varying(999);
ALTER TABLE public."Loja" ADD COLUMN if not exists "stCEP" character varying(20);    
ALTER TABLE public."Loja" ADD COLUMN if not exists "stTelefone" character varying(20);    
ALTER TABLE public."Loja" ADD COLUMN if not exists "stFax" character varying(20);        
ALTER TABLE public."Loja" ADD COLUMN if not exists "stContato" character varying(999);
ALTER TABLE public."Loja" ADD COLUMN if not exists "vrMensalidade" int;
ALTER TABLE public."Loja" ADD COLUMN if not exists "stContaDeb" character varying(99);
ALTER TABLE public."Loja" ADD COLUMN if not exists "stObs" character varying(999);    
ALTER TABLE public."Loja" ADD COLUMN if not exists "stLoja" character varying(90);        
ALTER TABLE public."Loja" ADD COLUMN if not exists "bBlocked" boolean;
ALTER TABLE public."Loja" ADD COLUMN if not exists "nuPctValor" int;
ALTER TABLE public."Loja" ADD COLUMN if not exists "vrTransacao" int;
ALTER TABLE public."Loja" ADD COLUMN if not exists "vrMinimo" int;        
ALTER TABLE public."Loja" ADD COLUMN if not exists "nuFranquia" int;            
ALTER TABLE public."Loja" ADD COLUMN if not exists "nuPeriodoFat" int;            
ALTER TABLE public."Loja" ADD COLUMN if not exists "nuDiaVenc" int;                
ALTER TABLE public."Loja" ADD COLUMN if not exists "nuTipoCob" int;                
ALTER TABLE public."Loja" ADD COLUMN if not exists "nuBancoFat" int;                    
ALTER TABLE public."Loja" ADD COLUMN if not exists "bIsentoFat" boolean;                    
ALTER TABLE public."Loja" ADD COLUMN if not exists "stSenha" character varying(50);
ALTER TABLE public."Loja" ADD COLUMN if not exists "bCancel" boolean;
ALTER TABLE public."Loja" ADD COLUMN if not exists "bPortalSenha" boolean;
ALTER TABLE public."Loja" ADD COLUMN if not exists "stEmail" character varying(200);
ALTER TABLE public."Loja" ADD COLUMN if not exists "stCelular" character varying(20);
ALTER TABLE public."Loja" ADD COLUMN if not exists "stAgencia" character varying(20);
ALTER TABLE public."Loja" ADD COLUMN if not exists "stConta" character varying(20);
ALTER TABLE public."Loja" ADD COLUMN if not exists "fkBanco" int;                   
ALTER TABLE public."Loja" ADD COLUMN if not exists "stCPFResp" character varying(20);
ALTER TABLE public."Loja" ADD COLUMN if not exists "stDataResp" character varying(20);

CREATE INDEX IF NOT EXISTS idx1_loja ON public."Loja" USING btree
    ("stLoja" COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public."LojaEmpresa" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."LojaEmpresa" OWNER to postgres;
ALTER TABLE public."LojaEmpresa" ADD COLUMN if not exists "fkLoja" int;
ALTER TABLE public."LojaEmpresa" ADD COLUMN if not exists "fkEmpresa" int;
ALTER TABLE public."LojaEmpresa" ADD COLUMN if not exists "nuTxAdmin" int;
ALTER TABLE public."LojaEmpresa" ADD COLUMN if not exists "nuDiasRepasse" int;
ALTER TABLE public."LojaEmpresa" ADD COLUMN if not exists "stAgencia" character varying(20);
ALTER TABLE public."LojaEmpresa" ADD COLUMN if not exists "stConta" character varying(20);
ALTER TABLE public."LojaEmpresa" ADD COLUMN if not exists "stBanco" character varying(20);

CREATE INDEX IF NOT EXISTS idx_lojaempresa ON public."LojaEmpresa" USING btree
    ("fkLoja" ASC NULLS LAST, "fkEmpresa" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public."LojaMsg" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."LojaMsg" OWNER to postgres;
ALTER TABLE public."LojaMsg" ADD COLUMN if not exists "fkLoja" int;
ALTER TABLE public."LojaMsg" ADD COLUMN if not exists "stMsg" character varying(2000);
ALTER TABLE public."LojaMsg" ADD COLUMN if not exists "stLink" character varying(999);
ALTER TABLE public."LojaMsg" ADD COLUMN if not exists "dtValidade" timestamp without time zone;
ALTER TABLE public."LojaMsg" ADD COLUMN if not exists "dtCriacao" timestamp without time zone;
ALTER TABLE public."LojaMsg" ADD COLUMN if not exists "bAtiva" boolean;

CREATE INDEX IF NOT EXISTS idx1_lojamsg ON public."LojaMsg" USING btree
    ("fkLoja" ASC NULLS LAST, "dtValidade" ASC NULLS LAST, "bAtiva" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public."LoteCartao" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."LoteCartao" OWNER to postgres;
ALTER TABLE public."LoteCartao" ADD COLUMN if not exists "nuCartoes" int;
ALTER TABLE public."LoteCartao" ADD COLUMN if not exists "fkEmpresa" int;
ALTER TABLE public."LoteCartao" ADD COLUMN if not exists "nuSitLote" int;
ALTER TABLE public."LoteCartao" ADD COLUMN if not exists "dtAbertura" timestamp without time zone;
ALTER TABLE public."LoteCartao" ADD COLUMN if not exists "dtEnvioGrafica" timestamp without time zone;
ALTER TABLE public."LoteCartao" ADD COLUMN if not exists "dtAtivacao" timestamp without time zone;

CREATE INDEX IF NOT EXISTS idx1_lotecartao ON public."LoteCartao" USING btree
    ("fkEmpresa" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public."LoteCartaoDetalhe" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."LoteCartaoDetalhe" OWNER to postgres;
ALTER TABLE public."LoteCartaoDetalhe" ADD COLUMN if not exists "fkLote" int;
ALTER TABLE public."LoteCartaoDetalhe" ADD COLUMN if not exists "fkEmpresa" int;
ALTER TABLE public."LoteCartaoDetalhe" ADD COLUMN if not exists "fkCartao" int;
ALTER TABLE public."LoteCartaoDetalhe" ADD COLUMN if not exists "nuMatricula" int;
ALTER TABLE public."LoteCartaoDetalhe" ADD COLUMN if not exists "nuViaOriginal" int;
ALTER TABLE public."LoteCartaoDetalhe" ADD COLUMN if not exists "nuTitularidade" int;
ALTER TABLE public."LoteCartaoDetalhe" ADD COLUMN if not exists "stCPF" character varying(20);
ALTER TABLE public."LoteCartaoDetalhe" ADD COLUMN if not exists "stNomeCartao" character varying(30);
ALTER TABLE public."LoteCartaoDetalhe" ADD COLUMN if not exists "dtAtivacao" timestamp without time zone;
ALTER TABLE public."LoteCartaoDetalhe" ADD COLUMN if not exists "dtPedido" timestamp without time zone;

CREATE INDEX IF NOT EXISTS idx1_lotecartaodet ON public."LoteCartaoDetalhe" USING btree
    ("fkLote" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE INDEX IF NOT EXISTS idx2_lotecartaodet ON public."LoteCartaoDetalhe" USING btree
    ("fkLote" ASC NULLS LAST, "fkEmpresa" ASC NULLS LAST, "fkCartao" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public."Parceiro" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Parceiro" OWNER to postgres;
ALTER TABLE public."Parceiro" ADD COLUMN if not exists "stNome" character varying(500);
ALTER TABLE public."Parceiro" ADD COLUMN if not exists "dtCadastro" timestamp without time zone;
ALTER TABLE public."Parceiro" ADD COLUMN if not exists "stTelefone" character varying(50);
ALTER TABLE public."Parceiro" ADD COLUMN if not exists "stCelular" character varying(50);
ALTER TABLE public."Parceiro" ADD COLUMN if not exists "stResp" character varying(500);
ALTER TABLE public."Parceiro" ADD COLUMN if not exists "stSocial" character varying(500);
ALTER TABLE public."Parceiro" ADD COLUMN if not exists "stCNPJ" character varying(50);
ALTER TABLE public."Parceiro" ADD COLUMN if not exists "stEstado" character varying(2);
ALTER TABLE public."Parceiro" ADD COLUMN if not exists "stCidade" character varying(500);
ALTER TABLE public."Parceiro" ADD COLUMN if not exists "stEndereco" character varying(500);
ALTER TABLE public."Parceiro" ADD COLUMN if not exists "stObs" character varying(500);
ALTER TABLE public."Parceiro" ADD COLUMN if not exists "stCEP" character varying(20);

CREATE TABLE IF NOT EXISTS public."Parcela" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Parcela" OWNER to postgres;
ALTER TABLE public."Parcela" ADD COLUMN if not exists "nuNsu" int;
ALTER TABLE public."Parcela" ADD COLUMN if not exists "fkEmpresa" int;
ALTER TABLE public."Parcela" ADD COLUMN if not exists "fkCartao" int;
ALTER TABLE public."Parcela" ADD COLUMN if not exists "dtInclusao" timestamp without time zone;
ALTER TABLE public."Parcela" ADD COLUMN if not exists "nuParcela" int;
ALTER TABLE public."Parcela" ADD COLUMN if not exists "vrValor" int;
ALTER TABLE public."Parcela" ADD COLUMN if not exists "nuIndice" int;
ALTER TABLE public."Parcela" ADD COLUMN if not exists "fkLoja" int;
ALTER TABLE public."Parcela" ADD COLUMN if not exists "nuTotParcelas" int;
ALTER TABLE public."Parcela" ADD COLUMN if not exists "fkTerminal" int;
ALTER TABLE public."Parcela" ADD COLUMN if not exists "fkLogTransacao" int;

CREATE INDEX IF NOT EXISTS idx1_parcela ON public."Parcela" USING btree
    ("fkLogTransacao" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public."SaldoConvenio" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SaldoConvenio" OWNER to postgres;
ALTER TABLE public."SaldoConvenio" ADD COLUMN if not exists "fkCartao" int;
ALTER TABLE public."SaldoConvenio" ADD COLUMN if not exists "vrSaldo" int;
ALTER TABLE public."SaldoConvenio" ADD COLUMN if not exists "nuMes" int;
ALTER TABLE public."SaldoConvenio" ADD COLUMN if not exists "nuAno" int;

CREATE TABLE IF NOT EXISTS public."SolicitacaoVenda" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."SolicitacaoVenda" OWNER to postgres;
ALTER TABLE public."SolicitacaoVenda" ADD COLUMN if not exists "fkCartao" int;
ALTER TABLE public."SolicitacaoVenda" ADD COLUMN if not exists "fkLoja" int;
ALTER TABLE public."SolicitacaoVenda" ADD COLUMN if not exists "vrValor" int;
ALTER TABLE public."SolicitacaoVenda" ADD COLUMN if not exists "nuParcelas" int;
ALTER TABLE public."SolicitacaoVenda" ADD COLUMN if not exists "bAberto" boolean;
ALTER TABLE public."SolicitacaoVenda" ADD COLUMN if not exists "dtSolic" timestamp without time zone;
ALTER TABLE public."SolicitacaoVenda" ADD COLUMN if not exists "dtConf" timestamp without time zone;
ALTER TABLE public."SolicitacaoVenda" ADD COLUMN if not exists "fkTerminal" int;
ALTER TABLE public."SolicitacaoVenda" ADD COLUMN if not exists "fkLogTrans" int;
ALTER TABLE public."SolicitacaoVenda" ADD COLUMN if not exists "stErro" character varying(500);
ALTER TABLE public."SolicitacaoVenda" ADD COLUMN if not exists "stParcelas" character varying(999);

CREATE TABLE IF NOT EXISTS public."Terminal" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."Terminal" OWNER to postgres;
ALTER TABLE public."Terminal" ADD COLUMN if not exists "stTerminal" character varying(12);
ALTER TABLE public."Terminal" ADD COLUMN if not exists "fkLoja" int;
ALTER TABLE public."Terminal" ADD COLUMN if not exists "stLocal" character varying(250);

CREATE INDEX IF NOT EXISTS idx1_terminal ON public."Terminal" USING btree
    ("stTerminal" COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE INDEX IF NOT EXISTS idx2_terminal ON public."Terminal" USING btree
    ("fkLoja" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE INDEX IF NOT EXISTS idx3_terminal ON public."Terminal" USING btree
    ("fkLoja" ASC NULLS LAST, "stTerminal" COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public."UsuarioEmissor" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."UsuarioEmissor" OWNER to postgres;
ALTER TABLE public."UsuarioEmissor" ADD COLUMN if not exists "nuNivel" int;
ALTER TABLE public."UsuarioEmissor" ADD COLUMN if not exists "dtTrocaSenha" timestamp without time zone;
ALTER TABLE public."UsuarioEmissor" ADD COLUMN if not exists "dtUltUso" timestamp without time zone;
ALTER TABLE public."UsuarioEmissor" ADD COLUMN if not exists "nuSenhaErrada" int;
ALTER TABLE public."UsuarioEmissor" ADD COLUMN if not exists "bTrocaSenha" boolean;
ALTER TABLE public."UsuarioEmissor" ADD COLUMN if not exists "fkEmpresa" int;
ALTER TABLE public."UsuarioEmissor" ADD COLUMN if not exists "stSenha" character varying(250);
ALTER TABLE public."UsuarioEmissor" ADD COLUMN if not exists "bBloqueio" boolean;
ALTER TABLE public."UsuarioEmissor" ADD COLUMN if not exists "stNome" character varying(250);
ALTER TABLE public."UsuarioEmissor" ADD COLUMN if not exists "bOperador" boolean;
ALTER TABLE public."UsuarioEmissor" ADD COLUMN if not exists "bAviso" boolean;

CREATE INDEX IF NOT EXISTS idx1_usuario ON public."UsuarioEmissor" USING btree
    ("stNome" COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public."UsuarioParceiro" ( id bigserial NOT NULL, PRIMARY KEY (id)) WITH (OIDS = FALSE);
ALTER TABLE public."UsuarioParceiro" OWNER to postgres;
ALTER TABLE public."UsuarioParceiro" ADD COLUMN if not exists "stEmail" character varying(250);
ALTER TABLE public."UsuarioParceiro" ADD COLUMN if not exists "stSenha" character varying(250);
ALTER TABLE public."UsuarioParceiro" ADD COLUMN if not exists "nuTipo" int;
ALTER TABLE public."UsuarioParceiro" ADD COLUMN if not exists "fkParceiro" int;
ALTER TABLE public."UsuarioParceiro" ADD COLUMN if not exists "bAtivo" boolean;
ALTER TABLE public."UsuarioParceiro" ADD COLUMN if not exists "stNome" character varying(250);
ALTER TABLE public."UsuarioParceiro" ADD COLUMN if not exists "dtCadastro" timestamp without time zone;
ALTER TABLE public."UsuarioParceiro" ADD COLUMN if not exists "dtUltLogin" timestamp without time zone;
