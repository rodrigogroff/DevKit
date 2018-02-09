
angular.module('app.controllers').controller('ListagemEmissorProcedimentosController',
['$scope', '$rootScope', '$state', 'Api', 'ngSelects', 
function ($scope, $rootScope, $state, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;
    
	init();

	function init()
    {
        $scope.campos = {
            ativo: 'true',
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
            tuss: $scope.campos.tuss,
            espec: $scope.campos.espec,
            nomeAssociado: $scope.campos.nomeAssociado,
            nomeCredenciado: $scope.campos.nomeCredenciado,
            codMedico: $scope.campos.codMedico,
            matricula: $scope.campos.matricula,
            dtInicial: $scope.campos.dtInicial,
            dtFim: $scope.campos.dtFim,
        };

		Api.EmissorListagemAutorizacao.listPage(opcoes, function (data)
		{
			$scope.list = data.results;
			$scope.total = data.count;
			$scope.loading = false;
		});
	}

}]);
