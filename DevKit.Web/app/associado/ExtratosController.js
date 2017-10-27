
angular.module('app.controllers').controller('ExtratosController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
	$rootScope.exibirMenu = true;

    $scope.date = new Date();

    $scope.loading = false;

    $scope.selectMeses = ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 });

    $scope.opcoes =
    {
        extrato_fech_mes: '',
        extrato_fech_ano_inicial: $scope.date.getFullYear()
    };

	init();

    function init()
    {
        $scope.loading = false;
     //   $scope.extrato_fech_mes = undefined;
        $scope.extrato_fech_ano_inicial = $scope.date.getFullYear();
    }

    $scope.pesquisar = function()
    {
        $scope.opcoes.tipo = $scope.tipoExtrato;

        Api.ExtratoAssociado.listPage($scope.opcoes, function (data)
        {
            $scope.list = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        });
	}
	
}]);
