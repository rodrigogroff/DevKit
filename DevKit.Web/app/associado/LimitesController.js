
angular.module('app.controllers').controller('LimitesController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $rootScope.mobileVersion = true;

	$scope.loading = false;

	init();

	function init()
    {
        $scope.loading = true;

        Api.LimiteAssociado.listPage({}, function (data)
        {
            $scope.list = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        });
    }

    $scope.menu = function ()
    {
        $state.go('menuUsr', {});
    }
	
}]);
