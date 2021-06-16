
angular.module('app.controllers').controller('EmissoraExtratosReceberCCController',
    ['$scope', '$rootScope', 'ngSelects', 'Api', 
        function ($scope, $rootScope, ngSelects, Api) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.tipo = $rootScope.tipo;

            $scope.itensporpagina = 15;

            $scope.date = new Date();

            $scope.result = {};

            $scope.campos = {
                mes_inicial: $scope.date.getMonth() + 1,
                ano_inicial: $scope.date.getFullYear(),
                selects: {
                    mes: ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 }),                    
                }
            };

            $scope.search = function () {

                $scope.loading = true;
                $scope.result = {};

                if ($scope.campos.mes_inicial !== undefined &&
                    $scope.campos.ano_inicial !== undefined) {

                    var opcoes = {
                        mat: $scope.campos.mat,
                        mes: $scope.campos.mes_inicial,
                        ano: $scope.campos.ano_inicial,
                    };

                    Api.EmissoraLancCCExtratoReceber.listPage(opcoes, function (data) {
                        $scope.result = data;                        
                        $scope.loading = false;
                    });
                }
                else
                    $scope.result = null;
            };

            $scope.imprimir = function () {

                $scope.loading = true;

                var opcoes = {
                    mes: $scope.campos.mes_inicial,
                    ano: $scope.campos.ano_inicial,
                };

                Api.EmissoraLancCCExtratoReceber.listPage(opcoes, function (data) {
                    $scope.result = data;                        
                    $scope.loading = false;

                    var printContents = '';

                    var mes = $scope.campos.mes_inicial;
                    var ano = $scope.campos.ano_inicial;

                    printContents = "<style> table, th, td { border: 1px solid black; border-collapse: collapse; } th, td { padding: 5px; text-align: left; } </style>" +
                        "<div align='center'><img src='/images/convey2020.png' style='height:50px' /><p align='center'>" +
                        "<h4>RELATORIO CONTAS A RECEBER / RECEITAS</h4></p>" +
                        "<p>Mês / Ano Ref.: <b>" + mes + " / " + ano + "</b><p>";

                    printContents += "<table class='table table-hover'><thead>" +
                        "<tr><th>ID</th><th>Cartão</th><th>Associado</th><th>Valor em Aberto</th></tr></thead>";

                    for (var i = 0; i < data.listDespCC.length; i++) {
                        var m = data.listDespCC[i];
                        printContents += "<tr><td>" + m.id + "</td><td>" + m.cartao + "</td><td>" + m.associado + "</td><td>" + m.vlrTotal + "</td></tr>";
                    }

                    printContents += "<br>";
                    printContents += "<h4>Total: " + data.vlrTotCC + "</h4>";

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
