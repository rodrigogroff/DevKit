angular.module('app.controllers').controller('ListingTasksController',
['$window', '$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects','$stateParams', '$location',
function ($window, $scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects, $stateParams, $location)
{
	$scope.loading = false;

	$scope.windowWidth = 900; var w = angular.element($window);
	$scope.$watch( function () { return $window.innerWidth; },
	  function (value) { $scope.availWidth = value; if (value > 1400) $scope.windowWidth = 1630; else $scope.windowWidth = 900; },
	  true ); w.bind('resize', function () { $scope.$apply();	});

	$scope.campos = {
	  	kpa: 'false',
	  	complete: 'false',
		selects: {
			user: ngSelects.obterConfiguracao(Api.User, { campoNome: 'stLogin' }),
			priority: ngSelects.obterConfiguracao(Api.Priority, { }),
			project: ngSelects.obterConfiguracao(Api.Project, { }),
			phase: ngSelects.obterConfiguracao(Api.Phase, { scope: $scope, filtro: { campo: 'fkProject', valor: 'campos.fkProject' } }),
			sprint: ngSelects.obterConfiguracao(Api.Sprint, { scope: $scope, filtro: { campo: 'fkPhase', valor: 'campos.fkPhase' } }),
			tasktype: ngSelects.obterConfiguracao(Api.TaskType, { scope: $scope, filtro: { campo: 'fkProject', valor: 'campos.fkProject' } }),
			taskcategory: ngSelects.obterConfiguracao(Api.TaskCategory, { scope: $scope, filtro: { campo: 'fkTaskType', valor: 'campos.fkTaskType' } }),
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

		var urlParams = $location.search();

		if (urlParams.searchSystem != undefined)
			$scope.campos.busca = urlParams.searchSystem;
			
		var options = {
			skip: skip,
			take: take,
			kpa: $scope.campos.kpa,
			complete: $scope.campos.complete
		};

		var filter = ngHistoricoFiltro.filtro.filtroGerado;

		if (filter)
			angular.extend(options, filter);

		if (urlParams.searchSystem != undefined)
			angular.extend(options, $scope.campos );
		
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
