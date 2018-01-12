angular.module('app.controllers').controller('ManagementController',
['$window', '$scope', '$rootScope', '$state', 'Api', 'ngSelects',
function ($window, $scope, $rootScope, $state, Api, ngSelects)
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

	$scope.$watch('fkProject', function (newState, oldState)
	{
		if (newState == undefined)
			$scope.viewModel = undefined;
		else
			if (newState != oldState)
				load();		
	});

	init();

	function init()
    {
        $scope.windowWidth = 900; var w = angular.element($window);
        $scope.$watch(function () { return $window.innerWidth; },
            function (value) { $scope.availWidth = value; if (value > 1400) $scope.windowWidth = 1630; else $scope.windowWidth = 900; },
            true); w.bind('resize', function () { $scope.$apply(); });

        $scope.selectProjects = ngSelects.obterConfiguracao(Api.ProjectCombo, {});

        $scope.permModel = {};
        $scope.permID = 108;
        $scope.fkProject = 0;
        $scope.viewModel = undefined;

		CheckPermissions();		
	}
	
	function load()
	{		
		$scope.viewModel = undefined;
		$scope.loading = true;

        Api.Management.listPage({ fkProject: $scope.fkProject }, function (data)
        {
            $scope.loading = false;

			if (data.fail == true)
				$scope.viewModel = undefined;
			else
				$scope.viewModel = data;
		});
	}

	$scope.showTask = function (id) {
		$state.go('task', { id: id });
	}

}]);
