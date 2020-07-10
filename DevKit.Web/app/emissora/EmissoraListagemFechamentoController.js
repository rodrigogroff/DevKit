
angular.module('app.controllers').controller('EmissoraListagemFechamentoController',
    ['$scope', '$rootScope', 'Api', 'ngSelects', 'AuthService',
        function ($scope, $rootScope, Api, ngSelects, AuthService) {

            var invalidCheck = function (element) {
                if (element == undefined)
                    return true;
                else
                    if (element.length == 0)
                        return true;

                return false;
            };

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.tipo = $rootScope.tipo;
            $scope.date = new Date();

            $scope.campos = {
                mes_inicial: $scope.date.getMonth() + 1,
                ano_inicial: $scope.date.getFullYear(),
                tipoFech: 1,
                detFech: 1,
                tipoFechSel: 0,
                selects: {
                    mes: ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 }),
                    empresa: ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 }),
                }
            };

            $scope.search = function () {

                if ($scope.tipo == '5') {
                    $scope.emp_fail = $scope.campos.idEmpresa == undefined;
                    if ($scope.emp_fail == true)
                        return;
                }

                if (invalidCheck($scope.campos.mes_inicial) ||
                    invalidCheck($scope.campos.ano_inicial)) {
                    toastr.error('Informe os filtros corretamente', 'Erro');
                    return;
                }

                $scope.loading = true;
                $scope.campos.tipoFechSel = 0;

                var opcoes = {
                    idEmpresa: $scope.campos.idEmpresa,
                    tipoFech: $scope.campos.tipoFech,
                    detFech: $scope.campos.detFech,
                    mes: $scope.campos.mes_inicial,
                    ano: $scope.campos.ano_inicial,
                };

                if (opcoes.tipoFech == 1) {

                    Api.EmissoraFechamento.listPage(opcoes, function (data) {
                        $scope.campos.tipoFechSel = 1;
                        $scope.list = data.results;
                        $scope.totalFechamento = data.totalFechamento;
                        $scope.totalCartoes = data.totalCartoes;
                        $scope.convenio = data.convenio;
                        $scope.loading = false;
                    });
                }
                else if (opcoes.tipoFech == 2) {

                    Api.EmissoraFechamento.listPage(opcoes, function (data) {
                        $scope.campos.tipoFechSel = 2;
                        $scope.list = data.results;
                        $scope.totalFechamento = data.totalFechamento;
                        $scope.totalRepasse = data.totalRepasse;
                        $scope.totalBonus = data.totalBonus;
                        $scope.totalLojistas = data.totalLojistas;
                        $scope.convenio = data.convenio;
                        $scope.loading = false;
                    });
                }
            };

            $scope.exportar = function () {

                toastr.warning('Aguarde, solicitação em andamento', 'Exportar');

                AuthService.fillAuthData();

                var opcoes = {
                    idEmpresa: $scope.campos.idEmpresa,
                    tipoFech: $scope.campos.tipoFech,
                    detFech: $scope.campos.detFech,
                    mes: $scope.campos.mes_inicial,
                    ano: $scope.campos.ano_inicial,                    
                };

                console.log(AuthService.authentication);

                if ($scope.tipo == 4) opcoes.idEmpresa = AuthService.authentication.IdEmpresa;                

                window.location.href = "/api/EmissoraFechamento/exportar?" + $.param(opcoes);
            }

            $scope.imprimir = function () {

                var printContents = "<h2>CONVEYNET BENEFÍCIOS</h2>";

                if ($scope.campos.tipoFech == '1') {

                    printContents += "<h3>Relatório de Fechamento por cartões</h3>";
                    printContents += "Data de emissão: " + $scope.date.getDate() + "/" + ($scope.date.getMonth() + 1) + "/" + $scope.date.getFullYear();
                    printContents += "<table>";

                    printContents += "<tr><td>Convênio: " + $scope.convenio + "</td></tr>";
                    printContents += "<tr><td>Período: " + $scope.campos.mes_inicial + " / " + $scope.campos.ano_inicial + "</td></tr>";
                    printContents += "<tr><td>Total fechamento: R$ " + $scope.totalFechamento + "</td></tr>";
                    printContents += "<tr><td>Total cartões: " + $scope.totalCartoes + "</td></tr>";

                    printContents += "</table>";

                    if ($scope.campos.detFech == "2") {

                        printContents += "<table><thead><tr>";
                        printContents += "<th align='left' width='450px'>Associado</th>";
                        printContents += "<th align='left' width='120px'>Matricula</th>";
                        printContents += "<th align='left' width='120px'>Total</th>";
                        printContents += "</thead>";

                        for (var i = 0; i < $scope.list.length; ++i) {
                            var mdl = $scope.list[i];

                            printContents += "<tr>";
                            printContents += "<td>" + mdl.associado + "</td>";
                            printContents += "<td>" + mdl.matricula + "</td>";
                            printContents += "<td>" + mdl.total + "</td>";
                            printContents += "</tr>";
                        
                        }

                        printContents += "</table>";
                    }
                    else {

                        for (var i = 0; i < $scope.list.length; ++i) {
                            var mdl = $scope.list[i];

                            printContents += "<br>Fechamento para: <b>" + mdl.associado + "</b>, Matrícula: " + mdl.matricula + "<br />";
                            printContents += "<table><thead><tr>";
                            printContents += "<th align='left' width='450px'>Estabelecimento</th>";
                            printContents += "<th align='left' width='120px'>Data compra</th>";
                            printContents += "<th align='left' width='120px'>Valor Parcela</th>";
                            printContents += "<th align='left' width='120px'>Parcela</th>";
                            printContents += "</tr></thead>";

                            for (var t = 0; t < mdl.vendas.length; ++t) {
                                var venda = mdl.vendas[t];
                                printContents += "<tr>";
                                printContents += "<td>" + venda.lojista + "</td>";
                                printContents += "<td>" + venda.dtCompra + "</td>";
                                printContents += "<td>" + venda.valor + "</td>";
                                printContents += "<td>" + venda.parcela + "</td>";
                                printContents += "</tr>";
                            }

                            printContents += "</table>";
                            printContents += "<br>Total: R$ " + mdl.total + "<br><br>";
                        }
                    }

                    var popupWin = window.open('', '_blank', 'width=800,height=600');
                    popupWin.document.open();
                    popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                }
                else if ($scope.campos.tipoFech == '2') {

                    printContents += "<h3>Relatório de Fechamento por lojistas</h3>";
                    printContents += "Data de emissão: " + $scope.date.getDate() + "/" + ($scope.date.getMonth() + 1) + "/" + $scope.date.getFullYear();
                    printContents += "<table>";

                    printContents += "<tr><td>Convênio: " + $scope.convenio + "</td></tr>";
                    printContents += "<tr><td>Período: " + $scope.campos.mes_inicial + " / " + $scope.campos.ano_inicial + "</td></tr>";
                    printContents += "<tr><td>Total fechamento convênio: R$ " + $scope.totalFechamento + "</td></tr>";
                    printContents += "<tr><td>Total repasse lojistas: R$ " + $scope.totalRepasse + "</td></tr>";
                    printContents += "<tr><td>Total bonificação: R$ " + $scope.totalBonus + "</td></tr>";
                    printContents += "<tr><td>Quantidade lojistas: " + $scope.totalLojistas + "</td></tr>";

                    printContents += "</table>";

                    if ($scope.campos.detFech == "2") {

                        printContents += "<table><thead><tr>";
                        printContents += "<th width='450px' align='left'>Lojista</th>"
                        printContents += "<th width='150px' align='left'>Fechamento</th>"
                        printContents += "<th width='150px' align='left'>Bonificação</th>"
                        printContents += "<th width='150px' align='left'>A Pagar</th>"
                        printContents += "</tr></thead> ";

                        for (var i = 0; i < $scope.list.length; ++i) {
                            var mdl = $scope.list[i];

                            printContents += "<tr>";
                            printContents += "<td>" + mdl.lojista + "</td>";
                            printContents += "<td>" + mdl.total + "</td>";
                            printContents += "<td>" + mdl.bonus + "</td>";
                            printContents += "<td>" + mdl.repasse + "</td>";
                            printContents += "</tr>";
                        }

                        printContents += "</table>";
                    }
                    else {

                        for (var i = 0; i < $scope.list.length; ++i) {
                            var mdl = $scope.list[i];

                            printContents += "<br>Fechamento para: <b>" + mdl.lojista + " - " + mdl.sigla + "</b> , Endereco: " + mdl.endereco + "<br />";

                            printContents += "<table><thead><tr>";
                            printContents += "<th width='50px' align='left'>ID</th>"
                            printContents += "<th width='50px' align='left'>NSU</th>"
                            printContents += "<th width='270px' align='left'>Associado</th>"
                            printContents += "<th width='90px' align='left'>Cartão</th>"
                            printContents += "<th width='120px' align='left'>Data Compra</th>"
                            printContents += "<th width='90px' align='left'>Valor</th>"
                            printContents += "<th width='70px' align='left'>Parcela</th>"
                            printContents += "<th width='60px' align='left'>Terminal</th>"
                            printContents += "</tr ></thead > ";

                            for (var t = 0; t < mdl.vendas.length; ++t) {
                                var venda = mdl.vendas[t];

                                printContents += "<tr>";
                                printContents += "<td>" + venda.id + "</td>";
                                printContents += "<td>" + venda.nsu + "</td>";
                                printContents += "<td>" + venda.associado + "</td>";
                                printContents += "<td>" + venda.matricula + "</td>";
                                printContents += "<td>" + venda.dtCompra + "</td>";
                                printContents += "<td>" + venda.valor + "</td>";
                                printContents += "<td>" + venda.parcela + "</td>";
                                printContents += "<td>" + venda.terminal + "</td>";

                                printContents += "</tr>";
                            }

                            printContents += "</table>";
                            printContents += "<br>Bonificação: " + mdl.taxa;
                            printContents += "<br>Valor total: " + mdl.total;
                            printContents += "<br>Valor bonificação: " + mdl.bonus;
                            printContents += "<br>Valor a repassar: " + mdl.repasse;
                            printContents += "<br><br>";
                        }
                    }

                    var popupWin = window.open('', '_blank', 'width=900,height=600');
                    popupWin.document.open();
                    popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                }
            };

        }]);
