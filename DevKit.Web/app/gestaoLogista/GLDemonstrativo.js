
angular.module('app.controllers').controller('GLDemonstrativoController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;
    $scope.idEmpresa = undefined;

    $scope.convenios = ngSelects.obterConfiguracao(Api.ConveniosCombo, { tamanhoPagina: 15 });

    $scope.search = function ()
    {
        $scope.loading = true;

        Api.GLDemonstrativo.listPage({
            tipoDemonstrativo: $scope.tipoDemonstrativo,
            idEmpresa: $scope.idEmpresa },
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
