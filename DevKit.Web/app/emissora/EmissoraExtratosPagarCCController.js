
angular.module('app.controllers').controller('EmissoraExtratosPagarCCController',
    ['$scope', '$rootScope', 'ngSelects', 'Api', 
        function ($scope, $rootScope, ngSelects, Api) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.tipo = $rootScope.tipo;

            $scope.itensporpagina = 15;

            $scope.date = new Date();

            $scope.result = null;

            $scope.campos = {
                mes_inicial: $scope.date.getMonth() + 1,
                ano_inicial: $scope.date.getFullYear(),
                selects: {
                    mes: ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 }),                    
                }
            };

            $scope.search = function () {

                $scope.loading = true;
                $scope.result = null;

                if ($scope.campos.mes_inicial !== undefined &&
                    $scope.campos.ano_inicial !== undefined) {

                    var opcoes = {
                        mat: $scope.campos.mat,
                        mes: $scope.campos.mes_inicial,
                        ano: $scope.campos.ano_inicial,
                    };

                    Api.EmissoraLancCCExtratoPagar.listPage(opcoes, function (data) {
                        $scope.result = data;                        
                        $scope.loading = false;
                    });
                }
            };

            $scope.imprimir = function () {

                $scope.loading = true;

                var opcoes = {
                    mes: $scope.campos.mes_inicial,
                    ano: $scope.campos.ano_inicial,
                };

                Api.EmissoraLancCCExtratoPagar.listPage(opcoes, function (data) {
                    $scope.result = data;                        
                    $scope.loading = false;

                    var printContents = '';

                    var mes = $scope.campos.mes_inicial;
                    var ano = $scope.campos.ano_inicial;

                    printContents = "<style> table, th, td { border: 1px solid black; border-collapse: collapse; } th, td { padding: 5px; text-align: left; } </style>" +
                        "<div align='center'><img src='/images/convey2020.png' style='height:50px' /><p align='center'>" +
                        "<h4>RELATÓRIO CONTAS A PAGAR</h4></p>" +
                        "<p>Mês / Ano Ref.: <b>" + mes + " / " + ano + "</b><p>";

                    printContents += "<table class='table table-hover'><thead>" +
                        "<tr><th>Código</th><th>Razão Social</th><th>Lojista</th><th>CNPJ</th><th>Dados Bancários</th><th>Valor Total</th><th>Valor Comissão</th><th>Valor Repasse</th></tr></thead>";

                    for (var i = 0; i < data.listPagarCC.length; i++) {
                        var m = data.listPagarCC[i];
                        printContents += "<tr>" +
                            "<td>" + m.codLojista + "</td>" +
                            "<td>" + m.razSoc + "</td>" +
                            "<td>" + m.lojista + "</td>" +
                            "<td>" + m.cnpj + "</td>" +
                            "<td>" + m.banco + "<br>" + m.agencia + "<br>" + m.conta + "</td>" +
                            "<td>" + m.vlrTot + "</td>" +
                            "<td>" + m.vlrComissao + "</td>" +
                            "<td>" + m.vlrRepasse + "</td></tr>";
                    }

                    printContents += "<br>";
                    printContents += "<h4>Total: " + data.vlrTotal + "</h4>";
                    printContents += "<h4>Total Comissão: " + data.vlrTotComissao + "</h4>";
                    printContents += "<br>";
                    printContents += "<h4>Total contas a pagar: " + data.vlrTotRep + "</h4>";

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

        }]);
