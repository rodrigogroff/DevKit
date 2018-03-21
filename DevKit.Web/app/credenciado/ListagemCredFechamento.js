
angular.module('app.controllers').controller('ListagemCredFechamentoController',
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
        if ($scope.campos.mes != undefined && $scope.campos.ano > 2000)
        {
            $scope.loading = true;

            var opcoes =
                {
                    mes: $scope.campos.mes,
                    ano: $scope.campos.ano,
                    tipo: 1,
                    modo: 2,
                };

            Api.EmissorListagemFechamento.listPage(opcoes, function (data) {
                $scope.list = data.results;
                $scope.result = data;

                $scope.loading = false;
            });
        }
    }

}]);
