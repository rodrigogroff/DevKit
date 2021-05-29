
angular.module('app.controllers').controller('EmissoraExtratosLojistaCCController',
    ['$scope', '$rootScope', 'ngSelects', 'Api', 
        function ($scope, $rootScope, ngSelects, Api) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.tipo = $rootScope.tipo;

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

                $scope.result = null;

                if ($scope.campos.mes_inicial !== undefined &&
                    $scope.campos.ano_inicial !== undefined &&
                    $scope.campos.codigo !== undefined &&
                    $scope.campos.codigo !== '') {

                    var opcoes = {
                        codigo: $scope.campos.codigo,
                        mes: $scope.campos.mes_inicial,
                        ano: $scope.campos.ano_inicial,
                    };

                    $scope.loading = true;

                    Api.EmissoraLancCCExtratoLojista.listPage(opcoes, function (data) {
                        $scope.result = data;                        
                        $scope.loading = false;
                    });
                }
            };

            $scope.imprimir = function () {

                $scope.loading = true;

                var opcoes = {
                    codigo: $scope.campos.codigo,
                    mes: $scope.campos.mes_inicial,
                    ano: $scope.campos.ano_inicial,
                };

                Api.EmissoraLancCCExtratoLojista.listPage(opcoes, function (data) {
                    $scope.result = data;                        
                    $scope.loading = false;

                    var printContents = '';

                    var mes = $scope.campos.mes_inicial;
                    var ano = $scope.campos.ano_inicial;

                    printContents = "<style> table, th, td { border: 1px solid black; border-collapse: collapse; } th, td { padding: 5px; text-align: left; } </style>" +
                        "<div align='center'><img src='/images/convey2020.png' style='height:50px' /><p align='center'>" +
                        "<h4>Extrato Completo Lojista</h4></p>";

                    printContents += "<b>CNPJ</b>: " + data.cnpj + "<br>" +
                        "<b>Razão Social</b>: " + data.social + "<br>" +
                        "<b>Nome / Fantasia</b>: " + data.fantasia + "<br>" + 
                        "<b>Mês / Ano Ref</b>: " + mes + " / " + ano + "<br>" + 
                        "<b>Total</b>: " + data.valor + "<br>";


                    printContents += "<table class='table table-hover'><thead><tr><th>Ano / Mês</th><th>Vlr. Total</th><th>Situação</th></thead>";

                    for (var i = 0; i < data.list.length; i++) {
                        var m = data.list[i];
                        printContents += "<tr><td>" + m.ano + " / " + m.mes + "</td><td>" + m.valor + "</td><td>" + m.situacao + "</td></tr>";
                    }

                    printContents += "</table><br>";

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
