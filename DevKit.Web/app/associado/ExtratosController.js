
angular.module('app.controllers').controller('ExtratosController',
    ['$scope', '$rootScope', 'AuthService', 'Api', 'ngSelects', 
        function ($scope, $rootScope, AuthService, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $rootScope.mobileVersion = false;

            $scope.authentication = AuthService.authentication;
            $scope.date = new Date();
            $scope.loading = false;

            $scope.selectMeses = ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 });

            $scope.opcoes =
                {
                    extrato_fech_mes: '',
                    extrato_fech_ano_inicial: $scope.date.getFullYear()
                };

            $scope.$watch("tipoExtrato", function (novo, antigo) {
                if (novo != antigo)
                    $scope.list = undefined;
            });

            init();

            function init() {
                $scope.loading = false;
                $scope.extrato_fech_ano_inicial = $scope.date.getFullYear();
            }

            $scope.pesquisar = function () {
                $scope.opcoes.tipo = $scope.tipoExtrato;

                Api.ExtratoAssociado.listPage($scope.opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.total;
                    $scope.mesAtual = data.mesAtual;
                    $scope.saldoDisp = data.saldoDisp;
                    $scope.loading = false;
                });
            };

            $scope.listaDetalheFuturo = undefined;

            $scope.showModal = function (mdl) {
                $scope.listaDetalheFuturo = mdl;
            };

            $scope.fecharModal = function () {
                $scope.listaDetalheFuturo = undefined;
            };

            $scope.imprimir = function () {

                var printContents = '';
                
                if ($scope.tipoExtrato == '1') {
                    printContents = "<h2>CONVEYNET BENEFÍCIOS</h2>";

                    printContents += "Data de emissão: " + $scope.date.getDate() + "/" + ($scope.date.getMonth() + 1) + "/" + $scope.date.getFullYear();
                    printContents += "<table>";

                    printContents += "<tr><td>Associado: " + $scope.authentication.m2 + "</td></tr>";
                    printContents += "<tr><td>Cartão: " + $scope.authentication.nameUser + "</td></tr>";
                    printContents += "<tr><td>Valor total: " + $scope.total + "</td></tr>";
                    printContents += "<tr><td>Mês " + $scope.mesAtual + " / " + $scope.opcoes.extrato_fech_ano_inicial + "</td></tr>";

                    printContents += "</table>";
                    printContents += "<table><thead><tr><th align='left'>Data venda</th><th align='left'>NSU</th><th align='left'>Valor</th><th align='left'>Parcela</th><th align='left'>Estabelecimento</th></tr></thead>";

                    if ($scope.list.length == 0) {
                        printContents += "<br>Nenhum registro encontrado.<br>";
                    }
                    else if ($scope.list.length > 0) {
                        printContents += "<table><thead><tr><th align='left'>Data venda</th><th align='left'>NSU</th><th align='left'>Valor</th><th align='left'>Parcela</th><th align='left'>Estabelecimento</th></tr></thead>";

                        for (var i = 0; i < $scope.list.length; ++i) {
                            var mdl = $scope.list[i];

                            printContents += "<tr>";
                            printContents += "<td width='90px'>" + mdl.dataHora + "</td>";
                            printContents += "<td width='90px'>" + mdl.nsu + "</td>";
                            printContents += "<td width='90px'>" + mdl.valor + "</td>";
                            printContents += "<td width='90px'>" + mdl.parcela + "</td>";
                            printContents += "<td width='300px'>" + mdl.estab + "</td>";
                            printContents += "</tr>";
                        }

                        printContents += "</table>"
                    }

                    var popupWin = window.open('', '_blank', 'width=800,height=600');
                    popupWin.document.open();
                    popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                }
                else if ($scope.tipoExtrato == '2') {

                    printContents = "<h2>CONVEYNET BENEFÍCIOS</h2>";

                    printContents += "Data de emissão: " + $scope.date.getDate() + "/" + ($scope.date.getMonth() + 1) + "/" + $scope.date.getFullYear();
                    printContents += "<table>";

                    printContents += "<tr><td>Associado: " + $scope.authentication.m2 + "</td></tr>";
                    printContents += "<tr><td>Mês atual: " + $scope.mesAtual + "</td></tr>";
                    printContents += "<tr><td>Cartão: " + $scope.authentication.nameUser + "</td></tr>";
                    printContents += "<tr><td>Valor total: " + $scope.total + "</td></tr>";

                    printContents += "</table>";

                    if ($scope.list.length == 0) {
                        printContents += "<br>Nenhum registro encontrado.<br>";
                    }
                    else if ($scope.list.length > 0) {
                        printContents += "<table><thead><tr><th align='left'>Data venda</th><th align='left'>NSU</th><th align='left'>Valor</th><th align='left'>Parcela</th><th align='left'>Estabelecimento</th></tr></thead>";

                        for (var i = 0; i < $scope.list.length; ++i) {
                            var mdl = $scope.list[i];

                            printContents += "<tr>";
                            printContents += "<td width='90px'>" + mdl.dataHora + "</td>";
                            printContents += "<td width='90px'>" + mdl.nsu + "</td>";
                            printContents += "<td width='90px'>" + mdl.valor + "</td>";
                            printContents += "<td width='90px'>" + mdl.parcela + "</td>";
                            printContents += "<td width='300px'>" + mdl.estab + "</td>";
                            printContents += "</tr>";
                        }

                        printContents += "</table>"
                    }

                    var popupWin = window.open('', '_blank', 'width=800,height=600');

                    popupWin.document.open();
                    popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                }
            };

        }]);
