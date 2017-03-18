angular.module('app.controllers').controller('ListingSprintsController',
['$scope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
	$scope.loading = false;
	$scope.campos = {
		selects: {
			project: ngSelects.obterConfiguracao(Api.Project, { tamanhoPagina: 15, campoNome: 'stName' }),
			phase: ngSelects.obterConfiguracao(Api.Phase, {
				tamanhoPagina: 15, campoNome: 'stName',
				scope: $scope, filtro: { campo: 'fkProject', valor: 'campos.fkProject' }
			}),
		}
	};
	$scope.itensporpagina = 15;

	$scope.permModel = {};	
	$scope.permID = 104;

	function CheckPermissions()
	{
		Api.Permission.get({ id: $scope.permID }, function (data)
		{
			$scope.permModel = data;

			if (!$scope.permModel.listagem)
			{
				toastr.error('Access denied!', 'Permission');
				$state.go('home');
			}				
		},
		function (response) { });
	}

	init();

	function init()
	{
		CheckPermissions();

		if (ngHistoricoFiltro.filtro)
			ngHistoricoFiltro.filtro.exibeFiltro = false;
	}

	$scope.search = function ()
	{
		$scope.load(0, $scope.itensporpagina);
		$scope.paginador.reiniciar();
	}

	$scope.load = function (skip, take)
	{
		$scope.loading = true;

		var opcoes = { skip: skip, take: take };

		var filtro = ngHistoricoFiltro.filtro.filtroGerado;

		if (filtro)
			angular.extend(opcoes, filtro);

		Api.Sprint.listPage(opcoes, function (data) {
			$scope.list = data.results;
			$scope.total = data.count;		
			$scope.loading = false;
		});
	}

	$scope.show = function (mdl)
	{
		if (!$scope.permModel.visualizar) 
			toastr.error('Access denied!', 'Permission');
		else
			$state.go('sprint', { id: mdl.id });
	}

	$scope.new = function ()
	{
		if (!$scope.permModel.novo)
			toastr.error('Access denied!', 'Permission');
		else
			$state.go('sprint-new');
	}
	
}]);
