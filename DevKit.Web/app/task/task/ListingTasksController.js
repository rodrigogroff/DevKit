angular.module('app.controllers').controller('ListingTasksController',
['$window', '$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($window, $scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
	$scope.loading = false;

	$scope.windowWidth = 900; var w = angular.element($window);
	$scope.$watch( function () { return $window.innerWidth; },
	  function (value) { $scope.availWidth = value; if (value > 1400) $scope.windowWidth = 1630; else $scope.windowWidth = 900; },
	  true ); w.bind('resize', function () { $scope.$apply();	});

	$scope.campos = {
		complete: false,
		kpa: false,
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

			tasktype: ngSelects.obterConfiguracao(Api.TaskType, {
				tamanhoPagina: 15, campoNome: 'stName',
				scope: $scope, filtro: { campo: 'fkProject', valor: 'campos.fkProject' }
			}),

			taskcategory: ngSelects.obterConfiguracao(Api.TaskCategory, {
				tamanhoPagina: 15, campoNome: 'stName',
				scope: $scope, filtro: { campo: 'fkTaskType', valor: 'campos.fkTaskType' }
			}),
		}
	};
	$scope.itensporpagina = 15;

	$scope.permModel = {};	
	$scope.permID = 106;

	function CheckPermissions() {
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

		$rootScope.$broadcast('updateCounters');

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

		var options = { skip: skip, take: take };

		var filter = ngHistoricoFiltro.filtro.filtroGerado;

		if (filter)
			angular.extend(options, filter);

		delete options.selects;

		Api.Task.listPage(options, function (data) {
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
			$state.go('task', { id: mdl.id });
	}

	$scope.new = function ()
	{
		if (!$scope.permModel.novo)
			toastr.error('Access denied!', 'Permission');
		else
			$state.go('task-new');
	}

}]);
