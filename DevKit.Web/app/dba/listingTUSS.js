
angular.module('app.controllers').controller('ListingTUSSController',
['$scope', '$rootScope', '$state', 'Api', 'ngSelects', 
function ($scope, $rootScope, $state, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;

	init();

	function init()
    {
        $scope.campos = {
            codigo: ''
        };

        $scope.itensporpagina = 15;
	}

	$scope.search = function ()
	{
		$scope.load(0, $scope.itensporpagina);
		$scope.paginador.reiniciar();
	}

	$scope.load = function (skip, take)
	{
		$scope.loading = true;

        var opcoes = {
            skip: skip,
            take: take,
            codigo: $scope.campos.codigo,
            procedimento: $scope.campos.procedimento,
        };

		Api.TUSS.listPage(opcoes, function (data)
		{
			$scope.list = data.results;
			$scope.total = data.count;
			$scope.loading = false;
		});
	}
        
	$scope.new = function ()
	{
		//$state.go('novaempresa');
	}

}]);
