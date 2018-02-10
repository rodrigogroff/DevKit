
angular.module('app.controllers').controller('ListingCredenciadosController',
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
            nome: $scope.campos.nome,
        };

        Api.Credenciado.listPage(opcoes, function (data)
		{
			$scope.list = data.results;
			$scope.total = data.count;
			$scope.loading = false;
		});
    }

    $scope.show = function (mdl) {
       // if (!$scope.permModel.visualizar)
         //   toastr.error('Acesso negado!', 'Permissão');
        //else
            $state.go('credenciado', { id: mdl.id });
    }
        
	$scope.new = function () {
		$state.go('novocredenciado');
	}

}]);
