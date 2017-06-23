angular.module('app.controllers').controller('ListingUserKanbanController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects )
{
	$rootScope.exibirMenu = true;

	$scope.loading = false;

	$scope.fail = false;

	$scope.campos = {
		complete: 'false',
		selects: {
			user: ngSelects.obterConfiguracao(Api.UserCombo, { campoNome: 'stLogin' }),
            priority: ngSelects.obterConfiguracao(Api.PriorityCombo, { }),
            project: ngSelects.obterConfiguracao(Api.ProjectCombo, { }),
            phase: ngSelects.obterConfiguracao(Api.PhaseCombo, { scope: $scope, filtro: { campo: 'fkProject', valor: 'campos.fkProject' } }),
            sprint: ngSelects.obterConfiguracao(Api.SprintCombo, { scope: $scope, filtro: { campo: 'fkPhase', valor: 'campos.fkPhase' } }),
            tasktype: ngSelects.obterConfiguracao(Api.TaskTypeCombo, { scope: $scope, filtro: { campo: 'fkProject', valor: 'campos.fkProject' } }),
            taskcategory: ngSelects.obterConfiguracao(Api.TaskCategoryCombo, { scope: $scope, filtro: { campo: 'fkTaskType', valor: 'campos.fkTaskType' } }),
		}
	};

	$scope.permModel = {};
	$scope.viewModel = {};	
	$scope.permID = 107;

	function CheckPermissions()
	{
        Api.Permission.get({ id: $scope.permID }, function (data)
		{
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
			$scope.fail = data.fail;
		});
	}

	$scope.search = function () {
		init();
	}

	$scope.open = function (mdl) {
		$state.go('task', { id: mdl.id });
	}

}]);
