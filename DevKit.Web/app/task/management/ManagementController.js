angular.module('app.controllers').controller('ManagementController',
['$window', '$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($window, $scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
	$scope.loading = false;

	$scope.windowWidth = 900; var w = angular.element($window);
	$scope.$watch( function () { return $window.innerWidth; },
	  function (value) { $scope.availWidth = value; if (value > 1400) $scope.windowWidth = 1630; else $scope.windowWidth = 900; },
	  true ); w.bind('resize', function () { $scope.$apply();	});

	$scope.permModel = {};	
	$scope.permID = 107;

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

	$scope.fkProject = 0;
	$scope.viewModel = undefined;

	$scope.$watch('fkProject', function (newState, oldState)
	{
		if (newState == undefined)
		{
			$scope.viewModel = undefined;
		}
		else
			if (newState != oldState)
				load();		
	});

	init();

	function init()
	{
		CheckPermissions();

		$scope.selectProjects = ngSelects.obterConfiguracao(Api.Project, { tamanhoPagina: 15, campoNome: 'stName' });
	}
	
	function load()
	{		
		$scope.viewModel = undefined;
		$scope.loading = true;

		var options = { fkProject: $scope.fkProject };

		Api.Management.listPage(options, function (data)
		{
			if (data.fail == true)
				$scope.viewModel = undefined;
			else
				$scope.viewModel = data;
		},
		function (response)
		{
			$scope.viewModel = undefined;
		});

		$scope.loading = false;
	}

}]);
