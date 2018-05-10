
angular.module('app.controllers').controller('AssociadoMenuController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $scope.limites = function () {
        $state.go('limitesUsr', {});
    }

    $scope.extratos = function () {
        $state.go('extratosUsr', {});
    }

    $scope.logOut = function () {
        AuthService.fillAuthData();
        window.location = '/login?tipo=2';
    };

}]);
