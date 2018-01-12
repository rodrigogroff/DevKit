angular.module('app.controllers').controller('ListingTasksController',
['$window', '$scope', '$rootScope', '$state', 'Api', 'ngSelects',
function ($window, $scope, $rootScope, $state, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;

	function CheckPermissions() {
        Api.Permission.get({ id: $scope.permID }, function (data) {
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
        $scope.windowWidth = 900; var w = angular.element($window);
        $scope.$watch(function () { return $window.innerWidth; },
            function (value) { $scope.availWidth = value; if (value > 1400) $scope.windowWidth = 1630; else $scope.windowWidth = 900; },
            true); w.bind('resize', function () { $scope.$apply(); });

        $scope.campos = {
            kpa: 'false',
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

        $scope.itensporpagina = 15;
        $scope.permModel = {};
        $scope.permID = 106;

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

		var options = {
			skip: skip,
            take: take,
            busca: $scope.campos.busca,
            nuPriority: $scope.campos.nuPriority,
            fkProject: $scope.campos.fkProject,
            fkPhase: $scope.campos.fkPhase,
            fkSprint: $scope.campos.fkSprint,
            fkUserResponsible: $scope.campos.fkUserResponsible,
            fkUserStart: $scope.campos.fkUserStart,
            fkTaskType: $scope.campos.fkTaskType,
            fkTaskCategory: $scope.campos.fkTaskCategory,
            kpa: $scope.campos.kpa,
            expired: $scope.campos.expired,
            complete: $scope.campos.complete,
		};

		Api.Task.listPage(options, function (data) {
			$scope.list = data.results;
			$scope.total = data.count;
			$scope.loading = false;
		});
	}

	$scope.show = function (mdl)
    {
        console.log(mdl);
		if (!$scope.permModel.visualizar) 
            toastr.error('Acesso negado!', 'Permissão');
		else
			$state.go('task', { id: mdl.id });
	}

	$scope.new = function ()
	{
		if (!$scope.permModel.novo)
            toastr.error('Acesso negado!', 'Permissão');
		else
			$state.go('task-new');
	}

}]);
