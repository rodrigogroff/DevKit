
angular.module('app.controllers').controller('GLDemonstrativoController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;

    $scope.date = new Date();

    $scope.campos = {
        idEmpresa: '',
        idTerminal: '',
        mes_inicial: $scope.date.getMonth() + 1,
        ano: $scope.date.getFullYear(),
        porTerminal: false
    };

    $scope.convenios = ngSelects.obterConfiguracao(Api.ConveniosCombo, { tamanhoPagina: 15 });
    $scope.terminais = ngSelects.obterConfiguracao(Api.TerminalLoja, { tamanhoPagina: 15 });
    $scope.mes = ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 });

    $scope.fecharPopUp = function ()
    {
        $scope.transacoes = undefined;
    };

    $scope.show = function (mdl)
    {
        $scope.transacoes = mdl.lst;
        $scope.mdlSelecionado = mdl;
    }

    $scope.search = function ()
    {
        $scope.loading = true;

        var opcoes = {
            tipoDemonstrativo: $scope.tipoDemonstrativo,
            idEmpresa: $scope.campos.idEmpresa,
            idTerminal: $scope.campos.idTerminal,
            mes_inicial: $scope.campos.mes_inicial,
            ano: $scope.campos.ano,
            porTerminal: $scope.campos.porTerminal,
        };

        $scope.tipoDemonstrativoSel = undefined;
        $scope.porTerminalSel = undefined;

        Api.GLDemonstrativo.listPage(opcoes,
        function (data)
        {
            $scope.tipoDemonstrativoSel = $scope.tipoDemonstrativo;
            $scope.porTerminalSel = $scope.campos.porTerminal;

            $scope.list = data.results;
            $scope.total = data.count;            

            $scope.totAtual = data.totAtual;
            $scope.totFuturo = data.totFuturo;
            $scope.totAtualRepasse = data.totAtualRepasse;
            $scope.totFuturoRepasse = data.totFuturoRepasse;

            $scope.loading = false;
        });
    }

    $scope.$watch("$scope.campos.porTerminal", function (novo, anterior)
    {
        $scope.tipoDemonstrativoSel = undefined;
        $scope.list = undefined;
        $scope.total = undefined;
        $scope.totAtual = undefined;
        $scope.totFuturo = undefined;
        $scope.totAtualRepasse = undefined;
        $scope.totFuturoRepasse = undefined;
    }, true);
   
}]);
