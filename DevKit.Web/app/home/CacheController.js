angular.module('app.controllers').controller('CacheController',
['$window', '$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($window, $scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
	$rootScope.exibirMenu = true;
    $scope.viewModel = [];

	function init()
	{
        Api.Cache.listPage({ }, function (data)
        {
            $scope.viewModel = data.results;
		});
	}

    $scope.refresh = function ()
    {
        init();
    }
    
    init();

}]);
