angular.module('app.controllers').controller('ListingRelatoriosController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;

    $scope.loading = false;

    $scope.associados = function ()
    {
        $state.go('relAssociados', { });
    }

}]);
