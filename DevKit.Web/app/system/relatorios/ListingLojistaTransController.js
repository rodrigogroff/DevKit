
angular.module('app.controllers').controller('ListingLojistaTransController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;

    $scope.date = new Date();

    $scope.loading = false;

    $scope.campos = {
        idOrdem: '1',
        confirmada: true,
        cancelada: false,
        mes_inicial: $scope.date.getMonth() + 1,
        mes_final: $scope.date.getMonth() + 1,
        ano_inicial: $scope.date.getFullYear(),
        ano_final: $scope.date.getFullYear(),
        dia_inicial: $scope.date.getDate(),
        dia_final: $scope.date.getDate(),
        selects: {
            mes: ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 }),            
            terminal: ngSelects.obterConfiguracao(Api.TerminalLoja, { tamanhoPagina: 15 }),
            ordem: ngSelects.obterConfiguracao(Api.OrdemRelLojistaTrans, { tamanhoPagina: 15 }),
        }
    };

    $scope.itensporpagina = 15;

    init();

    function init()
    {
        if (ngHistoricoFiltro.filtro)
            ngHistoricoFiltro.filtro.exibeFiltro = false;
    }

    $scope.search = function () {
        $scope.load(0, $scope.itensporpagina);
        $scope.paginador.reiniciar();
    };

    $scope.load = function (skip, take) {
        $scope.loading = true;

        var opcoes = {
            skip: skip,
            take: take,
            idEmpresa: $scope.campos.idEmpresa
        };

        angular.extend(opcoes, $scope.campos);

        delete opcoes.selects;

        Api.RelLojistaTrans.listPage(opcoes, function (data) {
            $scope.list = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        });
    };

    $scope.exportar = function ()
    {
        var opcoes =
        {
            skip: 0,
            take: 999999,
            idEmpresa: $scope.campos.idEmpresa
        };

        angular.extend(opcoes, $scope.campos);

        delete opcoes.selects;

        window.location.href = "/api/relLojistaTrans/exportar?" + $.param(opcoes);
    };

    $scope.printDiv = function (parcelas, cupom) {
        var printContents = "<table>";

        printContents += "<tr><td>" + cupom[0] + "</td></tr>";
        printContents += "<tr><td>" + cupom[1] + "</td></tr>";
        printContents += "<tr><td>" + cupom[2] + "</td></tr>";
        printContents += "<tr><td>" + cupom[3] + "</td></tr>";
        printContents += "<tr><td>" + cupom[4] + "</td></tr>";
        printContents += "<tr><td>" + cupom[5] + "</td></tr>";
        printContents += "<tr><td>" + cupom[6] + "</td></tr>";
        printContents += "<tr><td>" + cupom[7] + "</td></tr>";
        printContents += "<tr><td>" + cupom[8] + "</td></tr>";
        printContents += "<tr><td>" + cupom[9] + "</td></tr>";

        var pos = 10;

        for (var i = 0; i < parcelas; ++i) {
            printContents += "<tr><td>" + cupom[pos] + "</td></tr>";
            pos = pos + 1;
        }

        printContents += "<tr><td>" + cupom[pos] + "</td></tr>"; pos = pos + 1;
        printContents += "<tr><td>" + cupom[pos] + "</td></tr>"; pos = pos + 1;
        printContents += "<tr><td>" + cupom[pos] + "</td></tr>"; pos = pos + 1;
        printContents += "<tr><td>" + cupom[pos] + "</td></tr>"; pos = pos + 1;

        printContents += "</table>"

        var popupWin = window.open('', '_blank', 'width=800,height=600');
        popupWin.document.open();
        popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
        popupWin.document.close();
    };

}]);
