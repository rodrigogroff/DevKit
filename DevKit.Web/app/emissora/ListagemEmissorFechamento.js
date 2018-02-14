
angular.module('app.controllers').controller('ListagemEmissorFechamentoController',
['$scope', '$rootScope', '$state', 'Api', 'ngSelects', 
function ($scope, $rootScope, $state, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;
    
	init();

	function init()
    {
        $scope.campos = { };

        $scope.selectMonths = ngSelects.obterConfiguracao(Api.MonthCombo, {});

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
            mes: $scope.campos.mes,
            ano: $scope.campos.ano,
            tipo: $scope.campos.tipo,
            modo: $scope.campos.modo,
        };
        
		Api.EmissorListagemFechamento.listPage(opcoes, function (data)
		{
			$scope.list = data.results;
            $scope.result = data;
            $scope.tipoSel = $scope.campos.tipo;
            $scope.modoSel = $scope.campos.modo;

			$scope.loading = false;
		});
	}

}]);
