
angular.module('app.controllers').controller('GLDemonstrativoController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;

    $scope.search = function ()
    {
        $scope.loading = true;

        Api.GLDemonstrativo.listPage({ tipoDemonstrativo: $scope.tipoDemonstrativo}, function (data) {
            $scope.list = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        });
    }
   
}]);
