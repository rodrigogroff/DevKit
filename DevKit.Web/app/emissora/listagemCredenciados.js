
angular.module('app.controllers').controller('ListagemEmissorCredenciadosController',
['$scope', '$rootScope', '$state', 'Api', 'ngSelects', 
function ($scope, $rootScope, $state, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
    $scope.loading = false;

    function CheckPermissions() {
        Api.Permission.get({ id: $scope.permID }, function (data) {
            $scope.permModel = data;

            if (!$scope.permModel.listagem) {
                toastr.error('Acesso negado para consulta de credenciados!', 'Permissão');
                $state.go('home');
            }
        },
            function (response) { });
    }

	init();

	function init()
    {
        $scope.campos = {
            codigo: ''
        };

        $scope.itensporpagina = 15;
        $scope.permModel = {};
        $scope.permID = 501;

        CheckPermissions();
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
            especialidade: $scope.campos.especialidade,
        };

        Api.EmissorListagemCredenciado.listPage(opcoes, function (data)
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
        $state.go('emissorcredenciado', { id: mdl.id });
    }
        
	$scope.new = function () {
		$state.go('novocredenciado');
	}

}]);
