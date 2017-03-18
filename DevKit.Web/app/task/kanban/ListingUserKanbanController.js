angular.module('app.controllers').controller('ListingUserKanbanController',
['$scope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects', 
function ($scope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects )
{
	$scope.loading = false;
	$scope.fail = false;
	$scope.campos = {
		complete: 'false',
		selects: {
			user: ngSelects.obterConfiguracao(Api.User, { tamanhoPagina: 15, campoNome: 'stLogin' }),
			priority: ngSelects.obterConfiguracao(Api.Priority, { tamanhoPagina: 15, campoNome: 'stName' }),
			project: ngSelects.obterConfiguracao(Api.Project, { tamanhoPagina: 15, campoNome: 'stName' }),

			phase: ngSelects.obterConfiguracao(Api.Phase, {
				tamanhoPagina: 15, campoNome: 'stName',
				scope: $scope, filtro: { campo: 'fkProject', valor: 'campos.fkProject' }
			}),

			sprint: ngSelects.obterConfiguracao(Api.Sprint, {
				tamanhoPagina: 15, campoNome: 'stName',
				scope: $scope, filtro: { campo: 'fkPhase', valor: 'campos.fkPhase' }
			}),

			tasktype: ngSelects.obterConfiguracao(Api.TaskType, { tamanhoPagina: 15, campoNome: 'stName' }),

			taskcategory: ngSelects.obterConfiguracao(Api.TaskCategory, {
				tamanhoPagina: 15, campoNome: 'stName',
				scope: $scope, filtro: { campo: 'fkTaskType', valor: 'campos.fkTaskType' }
			}),
		}
	};

	$scope.permModel = {};
	$scope.viewModel = {};	
	$scope.permID = 107;

	function CheckPermissions()
	{
		Api.Permission.get({ id: $scope.permID }, function (data) {
			$scope.permModel = data;

			if (!$scope.permModel.listagem) {
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

		$scope.loading = true;

		var options = { };

		var filter = ngHistoricoFiltro.filtro.filtroGerado;

		if (filter)
			angular.extend(options, filter);

		delete options.selects;

		Api.UserKanban.listPage(options, function (data)
		{
			$scope.viewModel = data;
			$scope.loading = false;
			$scope.fail = false;
		},
		function (response)
		{
			$scope.loading = false;
			$scope.fail = true;
		});
	}

	$scope.search = function () {
		init();
	}

}]);
