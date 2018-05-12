
angular.module('app.controllers').controller('AssociadoMenuController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $scope.limites = function ()
    {
        $state.go('limitesUsrMobile', {});
    }

    $scope.extratos = function ()
    {
        $state.go('extratosUsrMobile', {});
    }

    $scope.lojistasUsrMobile = function () {
        $state.go('lojistasUsrMobile', {});
    }

    $scope.logOut = function ()
    {
        window.location = '/login?tipo=2';
    };

}]);
