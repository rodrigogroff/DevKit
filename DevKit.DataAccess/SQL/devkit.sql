--
-- PostgreSQL database dump
--

-- Dumped from database version 9.6.1
-- Dumped by pg_dump version 9.6.2

-- Started on 2017-04-27 18:36:33

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

SET search_path = public, pg_catalog;

--
-- TOC entry 2384 (class 0 OID 19052)
-- Dependencies: 224
-- Data for Name: AuditLog; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "AuditLog" VALUES (1, '2017-04-27 14:45:32.099684', 1, 1, 5, 0, '', '');
INSERT INTO "AuditLog" VALUES (2, '2017-04-27 14:45:38.014867', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 0');
INSERT INTO "AuditLog" VALUES (3, '2017-04-27 14:46:00.640391', 1, 34, 5, 2, 'New user: r.groff', '');
INSERT INTO "AuditLog" VALUES (4, '2017-04-27 14:46:21.894641', 1, 34, 5, 3, 'New user: c.maciel', '');
INSERT INTO "AuditLog" VALUES (5, '2017-04-27 14:46:41.223506', 1, 34, 5, 4, 'New user: s.helen', '');
INSERT INTO "AuditLog" VALUES (6, '2017-04-27 14:48:08.438129', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 0');
INSERT INTO "AuditLog" VALUES (7, '2017-04-27 14:48:40.243309', 1, 3, 1, 1, 'type: Software Maintenance', '');
INSERT INTO "AuditLog" VALUES (8, '2017-04-27 14:49:01.397424', 1, 5, 1, 1, 'New user: c.maciel;Role: Manager', '');
INSERT INTO "AuditLog" VALUES (9, '2017-04-27 14:49:10.186303', 1, 5, 1, 1, 'New user: r.groff;Role: Programmer', '');
INSERT INTO "AuditLog" VALUES (10, '2017-04-27 14:49:26.646949', 1, 5, 1, 1, 'New user: s.helen;Role: Analyst', '');
INSERT INTO "AuditLog" VALUES (11, '2017-04-27 14:49:35.35782', 1, 8, 1, 1, 'Phase added: Fase dois', '');
INSERT INTO "AuditLog" VALUES (12, '2017-04-27 14:49:44.56274', 1, 9, 1, 1, 'Phase edited: Fase dois', '');
INSERT INTO "AuditLog" VALUES (13, '2017-04-27 14:50:09.509235', 1, 11, 1, 1, 'New sprint: Mudança Requisitos', '');
INSERT INTO "AuditLog" VALUES (14, '2017-04-27 14:50:09.510235', 1, 12, 2, 0, '', '');
INSERT INTO "AuditLog" VALUES (15, '2017-04-27 14:50:27.069991', 1, 15, 2, 1, 'Added version: 7.0.1', '');
INSERT INTO "AuditLog" VALUES (16, '2017-04-27 14:52:06.420925', 1, 28, 3, 2, 'Edit accumulator: Coding Hours', '');
INSERT INTO "AuditLog" VALUES (17, '2017-04-27 14:52:44.598742', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (18, '2017-04-27 14:53:36.352917', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (19, '2017-04-27 14:54:43.032584', 1, 37, 4, 1, '', '');
INSERT INTO "AuditLog" VALUES (20, '2017-04-27 14:55:38.848165', 2, 1, 5, 0, '', '');
INSERT INTO "AuditLog" VALUES (21, '2017-04-27 14:56:02.136494', 2, 50, 5, 2, 'Password changed by user', '');
INSERT INTO "AuditLog" VALUES (22, '2017-04-27 14:56:14.362717', 2, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (23, '2017-04-27 14:58:31.931472', 1, 1, 5, 0, '', '');
INSERT INTO "AuditLog" VALUES (24, '2017-04-27 15:00:32.715549', 1, 1, 5, 0, '', '');
INSERT INTO "AuditLog" VALUES (25, '2017-04-27 15:01:37.249002', 2, 1, 5, 0, '', '');
INSERT INTO "AuditLog" VALUES (26, '2017-04-27 15:01:49.151192', 2, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (27, '2017-04-27 15:02:09.416218', 2, 40, 4, 1, 'New assigned: r.groff', '');
INSERT INTO "AuditLog" VALUES (28, '2017-04-27 15:02:09.419219', 2, 39, 4, 1, '', '');
INSERT INTO "AuditLog" VALUES (29, '2017-04-27 15:02:21.479425', 2, 43, 4, 1, 'New time added: 5:', '');
INSERT INTO "AuditLog" VALUES (30, '2017-04-27 17:05:45.793096', 1, 1, 5, 0, '', '');
INSERT INTO "AuditLog" VALUES (31, '2017-04-27 17:06:38.587375', 1, 40, 4, 1, 'State changed -> Estimating', '');
INSERT INTO "AuditLog" VALUES (32, '2017-04-27 17:06:38.591375', 1, 39, 4, 1, '', '');
INSERT INTO "AuditLog" VALUES (33, '2017-04-27 17:10:51.756273', 1, 1, 5, 0, '', '');
INSERT INTO "AuditLog" VALUES (34, '2017-04-27 17:21:54.857107', 1, 1, 5, 0, '', '');
INSERT INTO "AuditLog" VALUES (35, '2017-04-27 17:23:17.922107', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (36, '2017-04-27 17:23:23.964107', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (37, '2017-04-27 17:23:41.130107', 1, 16, 2, 1, 'Updated version: 4.4.0', '');
INSERT INTO "AuditLog" VALUES (38, '2017-04-27 17:25:15.071107', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (39, '2017-04-27 17:26:24.581107', 1, 37, 4, 2, '', '');
INSERT INTO "AuditLog" VALUES (40, '2017-04-27 17:26:57.256107', 1, 43, 4, 2, 'New time added: 16:', '');
INSERT INTO "AuditLog" VALUES (41, '2017-04-27 17:28:46.384107', 1, 40, 4, 1, 'New message', '');
INSERT INTO "AuditLog" VALUES (42, '2017-04-27 17:28:46.386107', 1, 39, 4, 1, '', '');
INSERT INTO "AuditLog" VALUES (43, '2017-04-27 17:29:38.968107', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (44, '2017-04-27 17:31:47.057107', 1, 37, 4, 3, '', '');
INSERT INTO "AuditLog" VALUES (45, '2017-04-27 17:32:03.369107', 1, 43, 4, 3, 'New time added: 3:', '');
INSERT INTO "AuditLog" VALUES (46, '2017-04-27 17:37:56.200107', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (47, '2017-04-27 17:50:01.665343', 1, 1, 5, 0, '', '');
INSERT INTO "AuditLog" VALUES (48, '2017-04-27 17:50:08.208105', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (49, '2017-04-27 17:50:26.253657', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (50, '2017-04-27 17:51:59.805426', 1, 37, 4, 4, '', '');
INSERT INTO "AuditLog" VALUES (51, '2017-04-27 17:52:11.991769', 1, 43, 4, 4, 'New time added: 1:', '');
INSERT INTO "AuditLog" VALUES (52, '2017-04-27 17:52:56.255486', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (53, '2017-04-27 17:53:26.895291', 1, 37, 4, 5, '', '');
INSERT INTO "AuditLog" VALUES (54, '2017-04-27 17:53:43.063439', 1, 43, 4, 5, 'New time added: 1:', '');
INSERT INTO "AuditLog" VALUES (55, '2017-04-27 17:54:09.856399', 1, 39, 4, 5, 'Description: RF002003 - Cadastro Conjunto de PDV => RF002003 - Cadastro Conjunto de PDV
Exportar -> Incluir sigla do Conjunto
Incluir o campo "Sigla" (NM_SIGLA - tabela "ConjuntoPDVs") imediatamente após o nome reduzido na rotina de exportação;', '');
INSERT INTO "AuditLog" VALUES (56, '2017-04-27 17:55:35.108816', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (57, '2017-04-27 17:55:56.819301', 1, 37, 4, 6, '', '');
INSERT INTO "AuditLog" VALUES (58, '2017-04-27 17:56:16.501394', 1, 43, 4, 6, 'New time added: 3:', '');
INSERT INTO "AuditLog" VALUES (59, '2017-04-27 18:01:00.475355', 1, 39, 4, 6, 'Description: RF002003 - Cadastro Conjunto de PDV => RF002003 - Cadastro Conjunto de PDV

Exportar -> Não está funcionando código e nome da unidade de negócio (tem que vir somente relativo a aquele cliente/unidade de negócio selecionado)


• Alterar a tabela "ConjuntoPDVs" incluindo ID_UNIDADENEGOCIO que será referência a tabela "UnidadeNegocio".
• Fazer script para alterar a tabela “ConjuntoPDVs”
• Alterar a aba “Conjunto”:
o Incluir o campo “Código” – ID_CONJUNTOPDV, somente consulta, antes de “Nome”
o Incluir um campo tipo combo box após “Cliente” para “Unidade de Negócio”, permitir a seleção somente de Unidades de Negócio relacionadas ao Cliente. Após a seleção este campo deverá atualizar a tabela “ConjuntoPDVs”;Localization: Exportar => Exportar -> Não está funcionando código e nome da unidade de negócio;', '');
INSERT INTO "AuditLog" VALUES (60, '2017-04-27 18:01:54.90486', 1, 1, 5, 0, '', '');
INSERT INTO "AuditLog" VALUES (61, '2017-04-27 18:02:02.63186', 1, 39, 4, 6, '', '');
INSERT INTO "AuditLog" VALUES (62, '2017-04-27 18:03:45.27731', 1, 1, 5, 0, '', '');
INSERT INTO "AuditLog" VALUES (63, '2017-04-27 18:05:50.764152', 1, 1, 5, 0, '', '');
INSERT INTO "AuditLog" VALUES (64, '2017-04-27 18:06:02.371313', 1, 39, 4, 6, 'Description: RF002003 - Cadastro Conjunto de PDV

Exportar -> Não está funcionando código e nome da unidade de negócio (tem que vir somente relativo a aquele cliente/unidade de negócio selecionado)


• Alterar a tabela "ConjuntoPDVs" incluindo ID_UNIDADENEGOCIO que será referência a tabela "UnidadeNegocio".
• Fazer script para alterar a tabela “ConjuntoPDVs”
• Alterar a aba “Conjunto”:
o Incluir o campo “Código” – ID_CONJUNTOPDV, somente consulta, antes de “Nome”
o Incluir um campo tipo combo box após “Cliente” para “Unidade de Negócio”, permitir a seleção somente de Unidades de Negócio relacionadas ao Cliente. Após a seleção este campo deverá atualizar a tabela “ConjuntoPDVs” => RF002003 - Cadastro Conjunto de PDV

Exportar -> Não está funcionando código e nome da unidade de negócio (tem que vir somente relativo a aquele cliente/unidade de negócio selecionado)

• Alterar a tabela "ConjuntoPDVs" incluindo ID_UNIDADENEGOCIO que será referência a tabela "UnidadeNegocio".
• Fazer scri...', '');
INSERT INTO "AuditLog" VALUES (65, '2017-04-27 18:07:47.774852', 1, 39, 4, 5, 'Localization: Exportar => Exportar -> Incluir sigla do Conjunto;', '');
INSERT INTO "AuditLog" VALUES (66, '2017-04-27 18:08:13.55643', 1, 39, 4, 4, 'Localization: Ferramentas de Pesquisa => Ferramentas de Pesquisa -> Tirar região e código da região.;', '');
INSERT INTO "AuditLog" VALUES (67, '2017-04-27 18:08:34.929567', 1, 39, 4, 3, 'Localization: Ferramentas de pesquisa => Ferramentas de pesquisa - “Selecionar” (combo) unidade de negócio;', '');
INSERT INTO "AuditLog" VALUES (68, '2017-04-27 18:12:37.514823', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (69, '2017-04-27 18:12:53.913463', 1, 37, 4, 7, '', '');
INSERT INTO "AuditLog" VALUES (70, '2017-04-27 18:13:09.404012', 1, 43, 4, 7, 'New time added: 1:', '');
INSERT INTO "AuditLog" VALUES (71, '2017-04-27 18:14:41.578228', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (72, '2017-04-27 18:14:52.49632', 1, 37, 4, 8, '', '');
INSERT INTO "AuditLog" VALUES (73, '2017-04-27 18:15:09.877058', 1, 43, 4, 8, 'New time added: 1:', '');
INSERT INTO "AuditLog" VALUES (74, '2017-04-27 18:19:23.369405', 1, 1, 5, 0, '', '');
INSERT INTO "AuditLog" VALUES (75, '2017-04-27 18:24:22.300545', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (76, '2017-04-27 18:24:37.247545', 1, 37, 4, 9, '', '');
INSERT INTO "AuditLog" VALUES (77, '2017-04-27 18:28:45.022545', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');
INSERT INTO "AuditLog" VALUES (78, '2017-04-27 18:28:57.937545', 1, 37, 4, 10, '', '');
INSERT INTO "AuditLog" VALUES (79, '2017-04-27 18:29:49.084545', 1, 43, 4, 10, 'New time added: 2:', '');
INSERT INTO "AuditLog" VALUES (80, '2017-04-27 18:30:09.948545', 1, 2, 1, NULL, 'Skip: 0 take: 15 ', 'count: 1');


--
-- TOC entry 2399 (class 0 OID 0)
-- Dependencies: 223
-- Name: AuditLog_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"AuditLog_id_seq"', 80, true);


--
-- TOC entry 2392 (class 0 OID 19090)
-- Dependencies: 232
-- Data for Name: CompanyNews; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 2400 (class 0 OID 0)
-- Dependencies: 231
-- Name: CompanyNews_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"CompanyNews_id_seq"', 1, false);


--
-- TOC entry 2348 (class 0 OID 18893)
-- Dependencies: 188
-- Data for Name: Profile; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "Profile" VALUES (1, 'Administrator', '|1002||1003||1011||1012||1013||1014||1015||1021||1022||1023||1024||1025||1031||1032||1033||1034||1035||1041||1042||1043||1044||1045||1051||1052||1053||1054||1055||1061||1062||1063||1064||1065||1071||1081||1091||1101||2001||2011||1181||1182||1183||1184||1185||1191||1192||1193||1194||1195|');
INSERT INTO "Profile" VALUES (2, 'Project Manager', '|1002||1003||1011||1012||1013||1014||1015||1021||1022||1023||1024||1025||1031||1032||1033||1034||1035||1041||1042||1043||1044||1045||1051||1052||1053||1054||1055||1061||1062||1063||1064||1065||1071||1081||1091||1101||2001||2011||1181||1182||1183||1184||1185||1191||1192||1193||1194||1195|');
INSERT INTO "Profile" VALUES (3, 'Analyst', '|1002||1003||1011||1012||1013||1014||1015||1021||1022||1023||1024||1025||1031||1032||1033||1034||1035||1041||1042||1043||1044||1045||1051||1052||1053||1054||1055||1061||1062||1063||1064||1065||1071||1081||1091||1101||2001||2011||1181||1182||1183||1184||1185||1191||1192||1193||1194||1195|');
INSERT INTO "Profile" VALUES (4, 'Developer', '|1002||1003||1011||1012||1013||1014||1015||1021||1022||1023||1024||1025||1031||1032||1033||1034||1035||1041||1042||1043||1044||1045||1051||1052||1053||1054||1055||1061||1062||1063||1064||1065||1071||1081||1091||1101||2001||2011||1181||1182||1183||1184||1185||1191||1192||1193||1194||1195|');
INSERT INTO "Profile" VALUES (5, 'Tester', '|1002||1003||1011||1012||1013||1014||1015||1021||1022||1023||1024||1025||1031||1032||1033||1034||1035||1041||1042||1043||1044||1045||1051||1052||1053||1054||1055||1061||1062||1063||1064||1065||1071||1081||1091||1101||2001||2011||1181||1182||1183||1184||1185||1191||1192||1193||1194||1195|');


--
-- TOC entry 2401 (class 0 OID 0)
-- Dependencies: 187
-- Name: Profile_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"Profile_id_seq"', 5, true);


--
-- TOC entry 2356 (class 0 OID 18928)
-- Dependencies: 196
-- Data for Name: Project; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "Project" VALUES (1, 1, 'Pipeline', 3, '2017-04-27 14:48:40.131298');


--
-- TOC entry 2360 (class 0 OID 18944)
-- Dependencies: 200
-- Data for Name: ProjectPhase; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "ProjectPhase" VALUES (1, 'Fase dois', 1);


--
-- TOC entry 2402 (class 0 OID 0)
-- Dependencies: 199
-- Name: ProjectPhase_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"ProjectPhase_id_seq"', 1, true);


--
-- TOC entry 2362 (class 0 OID 18952)
-- Dependencies: 202
-- Data for Name: ProjectSprint; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "ProjectSprint" VALUES (1, 'Mudança Requisitos', NULL, 1, 1);


--
-- TOC entry 2370 (class 0 OID 18990)
-- Dependencies: 210
-- Data for Name: ProjectSprintVersion; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "ProjectSprintVersion" VALUES (1, '4.4.0', 1, 1);


--
-- TOC entry 2403 (class 0 OID 0)
-- Dependencies: 209
-- Name: ProjectSprintVersion_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"ProjectSprintVersion_id_seq"', 1, true);


--
-- TOC entry 2404 (class 0 OID 0)
-- Dependencies: 201
-- Name: ProjectSprint_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"ProjectSprint_id_seq"', 1, true);


--
-- TOC entry 2358 (class 0 OID 18936)
-- Dependencies: 198
-- Data for Name: ProjectUser; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "ProjectUser" VALUES (1, 1, 1, 'Creator', '2017-04-27 14:48:40.137299');
INSERT INTO "ProjectUser" VALUES (2, 3, 1, 'Manager', '2017-04-27 14:49:01.326417');
INSERT INTO "ProjectUser" VALUES (3, 2, 1, 'Programmer', '2017-04-27 14:49:10.184303');
INSERT INTO "ProjectUser" VALUES (4, 4, 1, 'Analyst', '2017-04-27 14:49:26.645949');


--
-- TOC entry 2405 (class 0 OID 0)
-- Dependencies: 197
-- Name: ProjectUser_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"ProjectUser_id_seq"', 4, true);


--
-- TOC entry 2406 (class 0 OID 0)
-- Dependencies: 195
-- Name: Project_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"Project_id_seq"', 1, true);


--
-- TOC entry 2346 (class 0 OID 18885)
-- Dependencies: 186
-- Data for Name: Setup; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "Setup" VALUES (1, '(99) 9999999', 'dd/MM/yyyy HH:mm', '9999.9999');


--
-- TOC entry 2407 (class 0 OID 0)
-- Dependencies: 185
-- Name: Setup_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"Setup_id_seq"', 1, true);


--
-- TOC entry 2386 (class 0 OID 19063)
-- Dependencies: 226
-- Data for Name: Survey; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 2388 (class 0 OID 19074)
-- Dependencies: 228
-- Data for Name: SurveyOption; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 2408 (class 0 OID 0)
-- Dependencies: 227
-- Name: SurveyOption_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"SurveyOption_id_seq"', 1, false);


--
-- TOC entry 2390 (class 0 OID 19082)
-- Dependencies: 230
-- Data for Name: SurveyUserOption; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 2409 (class 0 OID 0)
-- Dependencies: 229
-- Name: SurveyUserOption_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"SurveyUserOption_id_seq"', 1, false);


--
-- TOC entry 2410 (class 0 OID 0)
-- Dependencies: 225
-- Name: Survey_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"Survey_id_seq"', 1, false);


--
-- TOC entry 2372 (class 0 OID 18998)
-- Dependencies: 212
-- Data for Name: Task; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "Task" VALUES (8, false, '2017-04-27 18:14:52.487319', NULL, '5042.0214', 'RF002003', 'Visualização tela inicial -> Acrescentar Código do Conjunto PDV', 'RF002003 - Cadastro Conjunto de PDV

Visualização tela inicial -> Acrescentar Código do Conjunto PDV

Alterar a Visualização Tela Inicial para incluir a coluna "Código" (ID_CONJUNTOPDVS" antes de "Nome Conjunto"', 3, 1, 1, 1, 1, 1, 2, 5, 26, NULL, NULL, NULL);
INSERT INTO "Task" VALUES (9, false, '2017-04-27 18:24:37.237545', NULL, '3010.5348', 'RF002003', 'Visualização tela inicial -> Não está funcionando código e nome da unidade de negócio', 'RF002003 - Cadastro Conjunto de PDV

Visualização tela inicial -> Não está funcionando código e nome da unidade de negócio (tem que vir somente relativo a aquele cliente/unidade de negócio selecionado)', 3, 1, 1, 1, 1, 1, 2, 5, 26, NULL, NULL, NULL);
INSERT INTO "Task" VALUES (2, false, '2017-04-27 17:26:24.544107', NULL, '0570.6736', 'Congelar cabeçalho nas pesquisas', 'Funcionalidades Gerais', '"Congelar" a linha de cabeçalho de todas as telas de visualização da pesquisa', 3, 1, 1, 1, 1, 1, 2, 5, 26, NULL, NULL, NULL);
INSERT INTO "Task" VALUES (1, false, '2017-04-27 14:54:43.000581', NULL, '8342.3802', 'Orçamento', 'Mudanças requisitos fase dois', 'Orçar todos os artefatos de mudança da fase 2', 3, 1, 1, 1, 1, 1, 1, 3, 17, NULL, 2, NULL);
INSERT INTO "Task" VALUES (10, false, '2017-04-27 18:28:57.928545', NULL, '8241.7475', 'RF002003', 'Visualização tela inicial -> Tirar região', 'RF002003 - Cadastro Conjunto de PDV

Visualização tela inicial -> Tirar região

Alterar a Visualização Tela Inicial para retirar as colunas "Região", na coluna "Sigla" considerar a sigla do Conjunto de PDVs (NM_SIGLA - tabela "ConjuntoPDVS - ver ID 37)

• Alterar a tabela "ConjuntoPDVs" incluindo ID_UNIDADENEGOCIO que será referência a tabela "UnidadeNegocio".
• Fazer script para alterar a tabela “ConjuntoPDVs”
• Alterar a aba “Conjunto”:
o Incluir o campo “Código” – ID_CONJUNTOPDV, somente consulta, antes de “Nome”
o Incluir um campo tipo combo box após “Cliente” para “Unidade de Negócio”, permitir a seleção somente de Unidades de Negócio relacionadas ao Cliente. Após a seleção este campo deverá atualizar a tabela “ConjuntoPDVs”', 3, 1, 1, 1, 1, 1, 2, 5, 26, NULL, NULL, NULL);
INSERT INTO "Task" VALUES (6, false, '2017-04-27 17:55:56.809304', NULL, '6212.5704', 'RF002003', 'Exportar -> Não está funcionando código e nome da unidade de negócio', 'RF002003 - Cadastro Conjunto de PDV

Exportar -> Não está funcionando código e nome da unidade de negócio (tem que vir somente relativo a aquele cliente/unidade de negócio selecionado)

• Alterar a tabela "ConjuntoPDVs" incluindo ID_UNIDADENEGOCIO que será referência a tabela "UnidadeNegocio".
• Fazer script para alterar a tabela “ConjuntoPDVs”
• Alterar a aba “Conjunto”:
o Incluir o campo “Código” – ID_CONJUNTOPDV, somente consulta, antes de “Nome”
o Incluir um campo tipo combo box após “Cliente” para “Unidade de Negócio”, permitir a seleção somente de Unidades de Negócio relacionadas ao Cliente. Após a seleção este campo deverá atualizar a tabela “ConjuntoPDVs”', 3, 1, 1, 1, 1, 1, 2, 5, 26, NULL, NULL, NULL);
INSERT INTO "Task" VALUES (5, false, '2017-04-27 17:53:26.885294', NULL, '8563.1483', 'RF002003', 'Exportar -> Incluir sigla do Conjunto', 'RF002003 - Cadastro Conjunto de PDV
Exportar -> Incluir sigla do Conjunto
Incluir o campo "Sigla" (NM_SIGLA - tabela "ConjuntoPDVs") imediatamente após o nome reduzido na rotina de exportação', 3, 1, 1, 1, 1, 1, 2, 5, 26, NULL, NULL, NULL);
INSERT INTO "Task" VALUES (4, false, '2017-04-27 17:51:59.775435', NULL, '1272.3380', 'RF002003', 'Ferramentas de Pesquisa -> Tirar região e código da região.', 'RF002003 - Cadastro Conjunto de PDV
Ferramentas de Pesquisa -> Tirar região e código da região.

Retirar os campos Região e Sigla da Região da Ferramenta de Pesquisa.', 3, 1, 1, 1, 1, 1, 2, 5, 26, NULL, NULL, NULL);
INSERT INTO "Task" VALUES (3, false, '2017-04-27 17:31:47.047107', NULL, '0030.4623', 'RF002003', 'Ferramentas de pesquisa - “Selecionar” (combo) unidade de negócio', 'RF002003 - Cadastro Conjunto de PDV

Ferramentas de Pesquisa -> “Selecionar” (combo) unidade de negócio e não “escrever”, mesmo escrevendo não está selecionando os conjuntos somente daquela unidade

Análise: Alterar o formato do campo "Unidade de Negócio" para combo box e se o campo Cliente estiver preenchido, trazer somente as unidades de negócio relacionadas a este cliente', 3, 1, 1, 1, 1, 1, 2, 5, 26, NULL, NULL, NULL);
INSERT INTO "Task" VALUES (7, false, '2017-04-27 18:12:53.891461', NULL, '0805.4016', 'RF002003', 'Exportar -> Tirar região, código da região, sigla da região', 'RF002003 - Cadastro Conjunto de PDV

Exportar -> Tirar região, código da região, sigla da região

Alterar a rotina de exportação para retirar as colunas "Região", Código da Região" e "Sigla" (sigla da região)', 3, 1, 1, 1, 1, 1, 2, 5, 26, NULL, NULL, NULL);


--
-- TOC entry 2382 (class 0 OID 19044)
-- Dependencies: 222
-- Data for Name: TaskAccumulatorValue; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "TaskAccumulatorValue" VALUES (1, '2017-04-27 15:02:21.474424', NULL, 5, NULL, 1, 5, 2);
INSERT INTO "TaskAccumulatorValue" VALUES (2, '2017-04-27 17:26:57.236107', NULL, 16, NULL, 2, 8, 1);
INSERT INTO "TaskAccumulatorValue" VALUES (3, '2017-04-27 17:32:03.368107', NULL, 3, NULL, 3, 8, 1);
INSERT INTO "TaskAccumulatorValue" VALUES (4, '2017-04-27 17:52:11.946783', NULL, 1, NULL, 4, 8, 1);
INSERT INTO "TaskAccumulatorValue" VALUES (5, '2017-04-27 17:53:43.06244', NULL, 1, NULL, 5, 8, 1);
INSERT INTO "TaskAccumulatorValue" VALUES (6, '2017-04-27 17:56:16.500394', NULL, 3, NULL, 6, 8, 1);
INSERT INTO "TaskAccumulatorValue" VALUES (7, '2017-04-27 18:13:09.396011', NULL, 1, NULL, 7, 8, 1);
INSERT INTO "TaskAccumulatorValue" VALUES (8, '2017-04-27 18:15:09.875058', NULL, 1, NULL, 8, 8, 1);
INSERT INTO "TaskAccumulatorValue" VALUES (9, '2017-04-27 18:29:49.082545', NULL, 2, NULL, 10, 8, 1);


--
-- TOC entry 2411 (class 0 OID 0)
-- Dependencies: 221
-- Name: TaskAccumulatorValue_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"TaskAccumulatorValue_id_seq"', 9, true);


--
-- TOC entry 2366 (class 0 OID 18971)
-- Dependencies: 206
-- Data for Name: TaskCategory; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "TaskCategory" VALUES (1, 'Design Docs', '', '', 1, NULL, NULL, NULL, NULL);
INSERT INTO "TaskCategory" VALUES (2, 'Change Requirement', '', '', 1, NULL, NULL, NULL, NULL);
INSERT INTO "TaskCategory" VALUES (3, 'Task Estimation', '', '', 1, NULL, NULL, NULL, NULL);
INSERT INTO "TaskCategory" VALUES (4, 'Resource Build', '', '', 2, NULL, NULL, NULL, NULL);
INSERT INTO "TaskCategory" VALUES (5, 'Resource Refactory', '', '', 2, NULL, NULL, NULL, NULL);
INSERT INTO "TaskCategory" VALUES (6, 'Construction Bugs', '', '', 3, NULL, NULL, NULL, NULL);
INSERT INTO "TaskCategory" VALUES (7, 'Homologation Bugs', '', '', 3, NULL, NULL, NULL, NULL);
INSERT INTO "TaskCategory" VALUES (8, 'Production Bugs', '', '', 3, NULL, NULL, NULL, NULL);
INSERT INTO "TaskCategory" VALUES (9, 'Everyone', '', '', 4, NULL, NULL, NULL, NULL);
INSERT INTO "TaskCategory" VALUES (10, 'Analysts', '', '', 4, NULL, NULL, NULL, NULL);
INSERT INTO "TaskCategory" VALUES (11, 'Developers', '', '', 4, NULL, NULL, NULL, NULL);
INSERT INTO "TaskCategory" VALUES (12, 'Client and Analysts', '', '', 4, NULL, NULL, NULL, NULL);


--
-- TOC entry 2412 (class 0 OID 0)
-- Dependencies: 205
-- Name: TaskCategory_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"TaskCategory_id_seq"', 12, true);


--
-- TOC entry 2368 (class 0 OID 18982)
-- Dependencies: 208
-- Data for Name: TaskFlow; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "TaskFlow" VALUES (1, false, true, 'Open', 0, 1, 1);
INSERT INTO "TaskFlow" VALUES (2, false, true, 'Re-Open', 1, 1, 1);
INSERT INTO "TaskFlow" VALUES (3, false, false, 'Construction', 2, 1, 1);
INSERT INTO "TaskFlow" VALUES (4, false, false, 'Peer Review', 3, 1, 1);
INSERT INTO "TaskFlow" VALUES (5, false, false, 'Approval', 4, 1, 1);
INSERT INTO "TaskFlow" VALUES (6, true, false, 'Done', 5, 1, 1);
INSERT INTO "TaskFlow" VALUES (7, true, false, 'Cancelled', 6, 1, 1);
INSERT INTO "TaskFlow" VALUES (8, false, true, 'Open', 0, 1, 2);
INSERT INTO "TaskFlow" VALUES (9, false, true, 'Re-Open', 1, 1, 2);
INSERT INTO "TaskFlow" VALUES (10, false, false, 'Construction', 2, 1, 2);
INSERT INTO "TaskFlow" VALUES (11, false, false, 'Peer Review', 3, 1, 2);
INSERT INTO "TaskFlow" VALUES (12, false, false, 'Approval', 4, 1, 2);
INSERT INTO "TaskFlow" VALUES (13, true, false, 'Done', 5, 1, 2);
INSERT INTO "TaskFlow" VALUES (14, true, false, 'Cancelled', 6, 1, 2);
INSERT INTO "TaskFlow" VALUES (15, false, true, 'Open', 0, 1, 3);
INSERT INTO "TaskFlow" VALUES (16, false, true, 'Re-Open', 1, 1, 3);
INSERT INTO "TaskFlow" VALUES (17, false, false, 'Estimating', 2, 1, 3);
INSERT INTO "TaskFlow" VALUES (18, true, false, 'Done', 3, 1, 3);
INSERT INTO "TaskFlow" VALUES (19, true, false, 'Cancelled', 4, 1, 3);
INSERT INTO "TaskFlow" VALUES (20, false, true, 'Open', 0, 2, 4);
INSERT INTO "TaskFlow" VALUES (21, false, true, 'Re-Open', 1, 2, 4);
INSERT INTO "TaskFlow" VALUES (22, false, false, 'Development', 2, 2, 4);
INSERT INTO "TaskFlow" VALUES (23, false, false, 'Testing', 3, 2, 4);
INSERT INTO "TaskFlow" VALUES (24, true, false, 'Done', 4, 2, 4);
INSERT INTO "TaskFlow" VALUES (25, true, false, 'Cancelled', 5, 2, 4);
INSERT INTO "TaskFlow" VALUES (26, false, true, 'Open', 0, 2, 5);
INSERT INTO "TaskFlow" VALUES (27, false, true, 'Re-Open', 1, 2, 5);
INSERT INTO "TaskFlow" VALUES (28, false, false, 'Development', 2, 2, 5);
INSERT INTO "TaskFlow" VALUES (29, false, false, 'Testing', 3, 2, 5);
INSERT INTO "TaskFlow" VALUES (30, true, false, 'Done', 4, 2, 5);
INSERT INTO "TaskFlow" VALUES (31, true, false, 'Cancelled', 5, 2, 5);
INSERT INTO "TaskFlow" VALUES (32, false, true, 'Open', 0, 3, 6);
INSERT INTO "TaskFlow" VALUES (33, false, true, 'Re-Open', 1, 3, 6);
INSERT INTO "TaskFlow" VALUES (34, false, false, 'Development', 2, 3, 6);
INSERT INTO "TaskFlow" VALUES (35, false, false, 'Testing', 3, 3, 6);
INSERT INTO "TaskFlow" VALUES (36, true, false, 'Done', 4, 3, 6);
INSERT INTO "TaskFlow" VALUES (37, true, false, 'Cancelled', 5, 3, 6);
INSERT INTO "TaskFlow" VALUES (38, false, true, 'Open', 0, 3, 7);
INSERT INTO "TaskFlow" VALUES (39, false, true, 'Re-Open', 1, 3, 7);
INSERT INTO "TaskFlow" VALUES (40, false, false, 'Development', 2, 3, 7);
INSERT INTO "TaskFlow" VALUES (41, false, false, 'Testing', 3, 3, 7);
INSERT INTO "TaskFlow" VALUES (42, true, false, 'Done', 4, 3, 7);
INSERT INTO "TaskFlow" VALUES (43, true, false, 'Cancelled', 5, 3, 7);
INSERT INTO "TaskFlow" VALUES (44, false, true, 'Open', 0, 3, 8);
INSERT INTO "TaskFlow" VALUES (45, false, true, 'Re-Open', 1, 3, 8);
INSERT INTO "TaskFlow" VALUES (46, false, false, 'Development', 2, 3, 8);
INSERT INTO "TaskFlow" VALUES (47, false, false, 'Validation', 3, 3, 8);
INSERT INTO "TaskFlow" VALUES (48, true, false, 'Done', 4, 3, 8);
INSERT INTO "TaskFlow" VALUES (49, true, false, 'Cancelled', 5, 3, 8);
INSERT INTO "TaskFlow" VALUES (50, false, true, 'Planned', 0, 4, 9);
INSERT INTO "TaskFlow" VALUES (51, true, false, 'Done', 1, 4, 9);
INSERT INTO "TaskFlow" VALUES (52, true, false, 'Cancelled', 2, 4, 9);
INSERT INTO "TaskFlow" VALUES (53, false, true, 'Planned', 0, 4, 10);
INSERT INTO "TaskFlow" VALUES (54, true, false, 'Done', 1, 4, 10);
INSERT INTO "TaskFlow" VALUES (55, true, false, 'Cancelled', 2, 4, 10);
INSERT INTO "TaskFlow" VALUES (56, false, true, 'Planned', 0, 4, 11);
INSERT INTO "TaskFlow" VALUES (57, true, false, 'Done', 1, 4, 11);
INSERT INTO "TaskFlow" VALUES (58, true, false, 'Cancelled', 2, 4, 11);
INSERT INTO "TaskFlow" VALUES (59, false, true, 'Planned', 0, 4, 12);
INSERT INTO "TaskFlow" VALUES (60, true, false, 'Done', 1, 4, 12);
INSERT INTO "TaskFlow" VALUES (61, true, false, 'Cancelled', 2, 4, 12);


--
-- TOC entry 2378 (class 0 OID 19028)
-- Dependencies: 218
-- Data for Name: TaskFlowChange; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "TaskFlowChange" VALUES (1, 'Inicio dos trabalhos', '2017-04-27 17:06:38.574373', 1, 1, 15, 17);


--
-- TOC entry 2413 (class 0 OID 0)
-- Dependencies: 217
-- Name: TaskFlowChange_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"TaskFlowChange_id_seq"', 1, true);


--
-- TOC entry 2414 (class 0 OID 0)
-- Dependencies: 207
-- Name: TaskFlow_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"TaskFlow_id_seq"', 61, true);


--
-- TOC entry 2376 (class 0 OID 19017)
-- Dependencies: 216
-- Data for Name: TaskMessage; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "TaskMessage" VALUES (1, 'sprint 1 a 5 estimados', '2017-04-27 17:28:46.380107', 1, 1, 17);


--
-- TOC entry 2415 (class 0 OID 0)
-- Dependencies: 215
-- Name: TaskMessage_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"TaskMessage_id_seq"', 1, true);


--
-- TOC entry 2374 (class 0 OID 19009)
-- Dependencies: 214
-- Data for Name: TaskProgress; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "TaskProgress" VALUES (1, 1, 2, '2017-04-27 15:02:09.409218');


--
-- TOC entry 2416 (class 0 OID 0)
-- Dependencies: 213
-- Name: TaskProgress_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"TaskProgress_id_seq"', 1, true);


--
-- TOC entry 2364 (class 0 OID 18963)
-- Dependencies: 204
-- Data for Name: TaskType; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "TaskType" VALUES (1, true, true, false, 'Software Analysis', 1);
INSERT INTO "TaskType" VALUES (2, true, true, false, 'Software Development', 1);
INSERT INTO "TaskType" VALUES (3, true, true, false, 'Software Bugs', 1);
INSERT INTO "TaskType" VALUES (4, false, false, false, 'Team Meetings', 1);


--
-- TOC entry 2380 (class 0 OID 19036)
-- Dependencies: 220
-- Data for Name: TaskTypeAccumulator; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "TaskTypeAccumulator" VALUES (6, true, 'Estimate Coding Hours', 2, 2, 22, 4);
INSERT INTO "TaskTypeAccumulator" VALUES (8, true, 'Estimate Coding Hours', 2, 2, 28, 5);
INSERT INTO "TaskTypeAccumulator" VALUES (9, false, 'Coding Hours', 2, 2, 28, 5);
INSERT INTO "TaskTypeAccumulator" VALUES (7, false, 'Coding Hours', 2, 2, 22, 4);
INSERT INTO "TaskTypeAccumulator" VALUES (1, false, 'Design Construction Hours', 1, 2, 3, 1);
INSERT INTO "TaskTypeAccumulator" VALUES (2, false, 'Design Peer Review Hours', 1, 2, 4, 1);
INSERT INTO "TaskTypeAccumulator" VALUES (3, false, 'Change Analysis Hours', 1, 2, 10, 2);
INSERT INTO "TaskTypeAccumulator" VALUES (4, false, 'Change Peer Review Hours', 1, 2, 11, 2);
INSERT INTO "TaskTypeAccumulator" VALUES (5, false, 'Estimating Hours', 1, 2, 17, 3);
INSERT INTO "TaskTypeAccumulator" VALUES (10, false, 'Coding Hours', 3, 2, 34, 6);
INSERT INTO "TaskTypeAccumulator" VALUES (11, false, 'Coding Hours', 3, 2, 40, 7);
INSERT INTO "TaskTypeAccumulator" VALUES (12, false, 'Coding Hours', 3, 2, 46, 8);


--
-- TOC entry 2417 (class 0 OID 0)
-- Dependencies: 219
-- Name: TaskTypeAccumulator_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"TaskTypeAccumulator_id_seq"', 12, true);


--
-- TOC entry 2418 (class 0 OID 0)
-- Dependencies: 203
-- Name: TaskType_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"TaskType_id_seq"', 4, true);


--
-- TOC entry 2419 (class 0 OID 0)
-- Dependencies: 211
-- Name: Task_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"Task_id_seq"', 10, true);


--
-- TOC entry 2350 (class 0 OID 18904)
-- Dependencies: 190
-- Data for Name: User; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "User" VALUES (3, true, 'c.maciel', 'c.maciel', 2, NULL, '2017-04-27 14:46:21.893641');
INSERT INTO "User" VALUES (4, true, 's.helen', 's.helen', 3, NULL, '2017-04-27 14:46:41.222506');
INSERT INTO "User" VALUES (2, true, 'r.groff', 'gustavo', 4, '2017-04-27 15:01:37.300007', '2017-04-27 14:46:00.63739');
INSERT INTO "User" VALUES (1, true, 'dba', 'dba', 1, '2017-04-27 18:19:23.415409', NULL);


--
-- TOC entry 2352 (class 0 OID 18912)
-- Dependencies: 192
-- Data for Name: UserEmail; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 2420 (class 0 OID 0)
-- Dependencies: 191
-- Name: UserEmail_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"UserEmail_id_seq"', 1, false);


--
-- TOC entry 2394 (class 0 OID 19101)
-- Dependencies: 234
-- Data for Name: UserNewsRead; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 2421 (class 0 OID 0)
-- Dependencies: 233
-- Name: UserNewsRead_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"UserNewsRead_id_seq"', 1, false);


--
-- TOC entry 2354 (class 0 OID 18920)
-- Dependencies: 194
-- Data for Name: UserPhone; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 2422 (class 0 OID 0)
-- Dependencies: 193
-- Name: UserPhone_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"UserPhone_id_seq"', 1, false);


--
-- TOC entry 2423 (class 0 OID 0)
-- Dependencies: 189
-- Name: User_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"User_id_seq"', 4, true);


-- Completed on 2017-04-27 18:36:35

--
-- PostgreSQL database dump complete
--

