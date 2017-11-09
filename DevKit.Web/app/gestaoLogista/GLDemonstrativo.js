
angular.module('app.controllers').controller('GLDemonstrativoController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;

    $scope.campos = {
        idEmpresa: '',
        idTerminal: '',
        porTerminal: true
    };

    $scope.convenios = ngSelects.obterConfiguracao(Api.ConveniosCombo, { tamanhoPagina: 15 });
    $scope.terminais = ngSelects.obterConfiguracao(Api.TerminalLoja, { tamanhoPagina: 15 });

    $scope.search = function ()
    {
        $scope.loading = true;

        var opcoes = {
            tipoDemonstrativo: $scope.tipoDemonstrativo,
            idEmpresa: $scope.campos.idEmpresa,
            idTerminal: $scope.campos.idTerminal,
            porTerminal: $scope.campos.porTerminal,
        };

        console.log('updated 3');
        console.log(opcoes);

        Api.GLDemonstrativo.listPage(opcoes,
        function (data)
        {
            $scope.list = data.results;
            $scope.total = data.count;            

            $scope.totAtual = data.totAtual;
            $scope.totFuturo = data.totFuturo;
            $scope.totAtualRepasse = data.totAtualRepasse;
            $scope.totFuturoRepasse = data.totFuturoRepasse;

            $scope.loading = false;
        });
    }
   
}]);
