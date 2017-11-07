angular.module('app.controllers').controller('LojaMensagensController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
	$rootScope.exibirMenu = true;

    $scope.loading = false;
    $scope.aceite = false;
	
	init();

    function init()
	{
		$scope.loading = true;

        Api.LojistaMensagens.listPage({}, function (data)
        {
            $scope.list = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        });
    }

    $scope.confirmar = function ()
    {
        if ($scope.aceite == true)
            if ($rootScope != 3)
                $state.go('venda', {});
    }
	
}]);
