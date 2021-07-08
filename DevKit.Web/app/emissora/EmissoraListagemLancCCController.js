
angular.module('app.controllers').controller('EmissoraListagemLancCCController',
    ['$scope', '$rootScope', 'ngHistoricoFiltro', 'ngSelects', 'AuthService', 'Api', 
        function ($scope, $rootScope, ngHistoricoFiltro, ngSelects, AuthService, Api) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.tipo = $rootScope.tipo;

            $scope.itensporpagina = 15;

            $scope.date = new Date();

            $scope.campos = {
                mes_inicial: $scope.date.getMonth() + 1,
                ano_inicial: $scope.date.getFullYear(),
                selects: {
                    mes: ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 }),
                    despesa: ngSelects.obterConfiguracao(Api.Despesa, { tamanhoPagina: 15 }),
                    cartao: ngSelects.obterConfiguracao(Api.CartaoCombo, { tamanhoPagina: 15 }),
                }
            };

            init();

            function init() {
                if (ngHistoricoFiltro.filtro)
                    ngHistoricoFiltro.filtro.exibeFiltro = false;
            }

            $scope.search = function () {
                $scope.load(0, $scope.itensporpagina);
                $scope.paginador.reiniciar();
            };

            $scope.$watch("newLanc._fkCartao", function (novo, antigo) {                                
                Api.EmissoraCartao.get({ id: novo }, function (data) {                    
                    $scope.newLanc.stFOPA = data.stCodigoFOPA === "" ? data.matricula : data.stCodigoFOPA;
                    $scope.loading = false;
                },
                function (response) {
                    if (response.status === 404) { toastr.error('Invalid ID', 'Erro'); }
                });                    
            });

            $scope.audit = function () {

                $scope.loading = true;
                $scope.list = null;

                var opcoes = {
                    ano: $scope.campos.ano_inicial,
                    mes: $scope.campos.mes_inicial,
                };

                Api.EmissoraBaixaCCAudit.listPage(opcoes, function (data) {
                    $scope.listAudit = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            }

            $scope.load = function (skip, take) {

                $scope.loading = true;
                $scope.listAudit = null;

                if ($scope.campos.mes_inicial !== undefined &&
                    $scope.campos.ano_inicial !== undefined) {
                    var opcoes = {
                        skip: 0,
                        take: 60000,
                        nome: $scope.campos.nome,
                        mat: $scope.campos.mat,
                        mes: $scope.campos.mes_inicial,
                        ano: $scope.campos.ano_inicial,
                    };

                    Api.EmissoraLancCC.listPage(opcoes, function (data) {
                        $scope.list = data.results;
                        $scope.total = data.count;
                        $scope.loading = false;
                    });
                }
            };

            $scope.editar = function (mdl) {
                $scope.editLanc = mdl;
                $scope.modalEdit = true;                
            };

            $scope.closeModalEdit = function () {
                $scope.modalEdit = false;
            };
            
            $scope.editarLancModal = function () {
                Api.EmissoraLancCC.update({ id: 1 }, $scope.editLanc, function (data) {
                    toastr.success('Lançamento alterado!', 'Sucesso');
                    $scope.loading = false;
                    $scope.modalEdit = false;
                    $scope.search();
                }, function (response) { toastr.error(response.data.message, 'Erro'); });
            };

            $scope.remover = function (mdl) {
                $scope.delLanc = mdl;
                $scope.modalRemove = true;
            };

            $scope.closeModalRemover = function () {
                $scope.modalRemove = false;
            };       

            $scope.removerLancModal = function () {

                var opcoes = {
                    del_item: $scope.delLanc.id,                    
                };

                Api.EmissoraLancCC.listPage(opcoes, function (data) {
                    toastr.success('Lançamento removido com sucesso', 'Sucesso');
                    $scope.search();
                }, function (response) { toastr.error(response.data.message, 'Erro'); });

                $scope.modalRemove = false;
            };

            $scope.novoLanc = function () {

                $scope.newLanc = {
                };

                $scope.newLanc.nuMes = $scope.date.getMonth() + 1;
                $scope.newLanc.nuAno = $scope.date.getFullYear();
                $scope.newLanc._totParcelas = 1;
                $scope.newLanc.vrValor = '0,00';

                $scope.modalNew = true;                
            };

            $scope.closeModalNew = function () {
                $scope.modalNew = false;
                $scope.newLanc = { };
            }            

            $scope.newLancModal = function () {
                Api.EmissoraLancCC.add($scope.newLanc, function (data) {
                    toastr.success('Lançamento inserido!', 'Sucesso');
                    $scope.loading = false;
                    
                    $scope.newLanc._fkTipo = null;
                    $scope.newLanc._totParcelas = null;
                    $scope.newLanc.vrValor = null;

                    $scope.search();
                }, function (response) { toastr.error(response.data.message, 'Erro'); });
            }

            $scope.relatorio = function () {

                $scope.loading = true;

                var opcoes = {
                    mes: $scope.campos.mes_inicial,
                    ano: $scope.campos.ano_inicial
                };

                Api.EmissoraRelLancCC.listPage(opcoes, function (data)
                {                    
                    $scope.loading = false;

                    var printContents = '';

                    var mes = $scope.campos.mes_inicial;
                    var ano = $scope.campos.ano_inicial;

                    var emissao = data.emissao;
                    var dtEnvio = data.envio;
                    var totRegistros = data.totRegistros;
                    var totalRemessa = data.totalRemessa;
                    var lancs = data.lancs;

                    printContents = "<style> table, th, td { border: 1px solid black; border-collapse: collapse; } th, td { padding: 5px; text-align: left; } </style>" +
                        "<div align='center'><img src='/images/convey2020.png' style='height:50px' /><p align='center'><h3>RELATÓRIO DE REMESSA FOLHA DE PAGAMENTO</h3></p>" +
                        "<p align='center'>FILTRO: MÊS " + mes + " ANO " + ano + "<br>EMISSÃO: " + emissao + "<br>DT. ENVIO: " + dtEnvio + "<br>TOTAL REGISTROS: <b>" + totRegistros + "</b> TOTAL REMESSA: <b>R$ " + totalRemessa + "</b></p>";

                    if (lancs.length > 0) {
                        printContents += "<table align='center' width='100%'>";
                        printContents +=
                            "<tr>" +
                            "<td ><div align='center'><b>ID</b></div></td>" +
                            "<td ><div align='center'><b>CARTÃO</b></div></td>" +
                            "<td ><div align='center'><b>CD. FOLHA</b></div></td>" +
                            "<td ><div align='center'><b>ASSOCIADO</b></div></td>" +
                            "<td ><div align='center'><b>VALOR CARTAO</b></div></td>" +
                            "<td ><div align='center'><b>VALOR LANC.</b></div></td>" +
                            "<td ><div align='center'><b>TOTAL</b></div></td>" +
                            "</tr>";

                        for (var i = 0; i < lancs.length; i++) {
                            var lanc = lancs[i];
                            printContents +=
                                "<tr>" +
                                "<td ><div align='center'>" + (i + 1).toString() + "</div></td>" +
                                "<td ><div align='center'>" + lanc.cartao + "</div></td>" +
                                "<td ><div align='center'>" + lanc.folha + "</div></td>" +
                                "<td ><div align='center'>" + lanc.associado + "</div></td>" +
                                "<td ><div align='center'>" + lanc.valor + "</div></td>" +
                                "<td ><div align='center'>" + lanc.valorLanc + "</div></td>" +
                                "<td ><div align='center'>" + lanc.total + "</div></td>" +
                                "</tr>";
                        }

                        printContents += "</table>";
                    }

                    var popupWin = window.open('', '_blank', 'width=800,height=600');
                    popupWin.document.open();
                    popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                },
                    function (response) {
                        toastr.error('Parâmetros inválidos!', 'Erro');
                        $scope.loading = false;                        
                    });
            }

            $scope.exportar = function () {
                
                var t_emp = '';

                AuthService.fillAuthData();
                $scope.authentication = AuthService.authentication;
                t_emp = $scope.authentication.m1;
                
                if ($scope.campos.mes_inicial === undefined &&
                    $scope.campos.ano_inicial === undefined) {
                    toastr.error('Informe os filtros corretamente', 'Erro');
                    return;
                }

                window.location.href = "/api/EmissoraLancCC/exportar?" + $.param({
                    emp: t_emp,
                    mes: $scope.campos.mes_inicial,
                    ano: $scope.campos.ano_inicial,
                });
            };

        }]);
