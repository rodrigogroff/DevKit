
angular.module('app.controllers').controller('ListingUserKanbanController',
['$scope', '$rootScope', '$state', 'Api', 'ngSelects',
function ($scope, $rootScope, $state, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;
	
	function CheckPermissions()
	{
        Api.Permission.get({ id: $scope.permID }, function (data)
		{
			$scope.permModel = data;			
			if (!$scope.permModel.listagem) {
                toastr.error('Acesso negado!', 'Permissão');
				$state.go('home');
			}
		},
		function (response) { });
	}

	init();

	function init()
    {
        $scope.fail = false;

        $scope.campos = {
            complete: 'false',
            expired: 'false',
            selects: {
                user: ngSelects.obterConfiguracao(Api.UserCombo, {}),
                priority: ngSelects.obterConfiguracao(Api.Priority, {}),
                project: ngSelects.obterConfiguracao(Api.ProjectCombo, {}),
                phase: ngSelects.obterConfiguracao(Api.PhaseCombo, { scope: $scope, filtro: { campo: 'fkProject', valor: 'campos.fkProject' } }),
                sprint: ngSelects.obterConfiguracao(Api.SprintCombo, { scope: $scope, filtro: { campo: 'fkPhase', valor: 'campos.fkPhase' } }),
                tasktype: ngSelects.obterConfiguracao(Api.TaskTypeCombo, { scope: $scope, filtro: { campo: 'fkProject', valor: 'campos.fkProject' } }),
                taskcategory: ngSelects.obterConfiguracao(Api.TaskCategoryCombo, { scope: $scope, filtro: { campo: 'fkTaskType', valor: 'campos.fkTaskType' } }),
            }
        };

        $scope.permModel = {};
        $scope.viewModel = {};
        $scope.permID = 107;

		CheckPermissions();
	}

    $scope.search = function ()
    {
        $scope.loading = true;

        var options = {
            busca: $scope.campos.busca,
            nuPriority: $scope.campos.nuPriority,
            fkProject: $scope.campos.fkProject,
            fkPhase: $scope.campos.fkPhase,
            fkSprint: $scope.campos.fkSprint,
            fkUserAssigned: $scope.campos.fkUserAssigned,
            fkUserStart: $scope.campos.fkUserStart,
            fkTaskType: $scope.campos.fkTaskType,
            fkTaskCategory: $scope.campos.fkTaskCategory,
            kpa: $scope.campos.kpa,
            expired: $scope.campos.expired,
            complete: $scope.campos.complete,
        };

        Api.UserKanban.listPage(options, function (data) {
            $scope.viewModel = data;
            $scope.loading = false;
            $scope.fail = data.fail;
        });		
	}

	$scope.open = function (mdl) {
		$state.go('task', { id: mdl.id });
	}

}]);
