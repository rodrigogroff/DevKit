
angular.module('app.controllers').controller('EmissoraExtratosUsuariosCCController',
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

                if ($scope.campos.mat === undefined || $scope.campos.mat === '') {
                    toastr.error('Informar matrícula!', 'Erro');
                    return;
                }
                
                if ($scope.campos.mes_inicial !== undefined &&
                    $scope.campos.ano_inicial !== undefined ) {

                    var opcoes = {
                        mat: $scope.campos.mat,
                        mes: $scope.campos.mes_inicial,
                        ano: $scope.campos.ano_inicial,
                    };

                    $scope.loading = true;

                    Api.EmissoraLancCCExtratoUsuario.listPage(opcoes, function (data) {
                        $scope.result = data;

                        if (data.length == 0)
                            $scope.result = null;

                        $scope.loading = false;
                    });
                }
                else
                    $scope.result = null;
            };

            $scope.imprimir = function () {

                $scope.loading = true;

                var opcoes = {
                    mat: $scope.campos.mat,
                    mes: $scope.campos.mes_inicial,
                    ano: $scope.campos.ano_inicial,
                };

                Api.EmissoraLancCCExtratoUsuario.listPage(opcoes, function (data) {
                    $scope.result = data;                        
                    $scope.loading = false;

                    var printContents = '';

                    var mes = $scope.campos.mes_inicial;
                    var ano = $scope.campos.ano_inicial;

                    printContents = "<style> table, th, td { border: 1px solid black; border-collapse: collapse; } th, td { padding: 5px; text-align: left; } </style>" +
                        "<div align='center'><img src='/images/convey2020.png' style='height:50px' /><p align='center'>" +
                        "<h4>Extrato Completo Associado (Cartão e Despesas Associativas)</h4></p>";

                    printContents += "<table><tr><td>Cartão Mat.: <b>" + data.mat + "</b></td>" +
                        "<td width='20px'></td>" +
                        "<td>Cd. Folha: <b>" + data.fopa + "</b></td></tr>" +
                        "<tr>" +
                        "<td colspan='2'>Nome: <b>" + data.nome + "</b></td>" +
                        "</tr>" +
                        "<tr>" +
                        "<td>Mês / Ano Ref.: <b>" + mes + " / " + ano + "</b></td>" +
                        "</tr>" +
                        "</table>" + 
                        "<br>" + 
                        "<h4>Cartão Convênio</h4>" +
                        "<table class='table table-hover'><thead>" +
                        "<tr><th>Código</th><th>Estabelecimento</th><th>NSU</th><th>Vlr. Total</th><th>Vlr. Parcela</th><th>Parcela</th></tr></thead>";

                    for (var i = 0; i < data.listConvenio.length; i++) {
                        var m = data.listConvenio[i];
                        printContents += "<tr><td>" + m.cod + "</td><td>" + m.estab + "</td><td>" + m.nsu + "</td><td>" + m.vlrTot + "</td><td>" + m.vlrParc + "</td><td>" + m.parc + "</td></tr>";
                    }

                    printContents += "</table><p>Subtotal: " + data.vlrCartao + "</p>";
                    printContents += "<br>";
                    printContents += "<h4>Despesas Associativas</h4>";
                    printContents += "<table class='table table-hover'><thead><tr><th>Código</th><th>Descrição</th><th>Vlr. Parcela</th><th>Parcela</th></tr></thead>";

                    for (var i = 0; i < data.listDespCC.length; i++) {
                        var m = data.listDespCC[i];
                        printContents += "<tr><td>" + m.cod + "</td><td>" + m.desc + "</td><td>" + m.vlrParc + "</td><td>" + m.parc + "</td></tr>";
                    }

                    printContents += "</table>";
                    printContents += "<p>Subtotal: " + data.vlrDespCC + "</p>";
                    printContents += "<br>";
                    printContents += "<h4>Total Associado: " + data.vlrTotCC + "</h4>";

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
