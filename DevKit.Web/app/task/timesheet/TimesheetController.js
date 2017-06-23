angular.module('app.controllers').controller('TimesheetController',
['$window', '$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($window, $scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
	$rootScope.exibirMenu = true;

	$scope.loading = false;

	$scope.windowWidth = 900; var w = angular.element($window);
	$scope.$watch( function () { return $window.innerWidth; },
	  function (value) { $scope.availWidth = value; if (value > 1400) $scope.windowWidth = 1630; else $scope.windowWidth = 900; },
	  true ); w.bind('resize', function () { $scope.$apply();	});

	$scope.permModel = {};
	$scope.viewModel = undefined;
	$scope.adminTimesheet = false;
	$scope.permID = 109;
	$scope.userId = 0;
	$scope.stMessage = '';
	
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

        Api.Permission.get({ id: 110 }, function (data) {
			
			if ($scope.permModel.listagem) {
				$scope.adminTimesheet = true;
			}
		},
		function (response) { });
	}

	init();

	function init()
	{
		CheckPermissions();

		var currentDate = new Date();

		$scope.nuYear = currentDate.getFullYear();
		$scope.nuMonth = currentDate.getMonth() + 1;

		$scope.selectMonths = ngSelects.obterConfiguracao(Api.Month, { });
		$scope.selectUsers = ngSelects.obterConfiguracao(Api.UserCombo, { campoNome: 'stLogin' });
	}
	
	$scope.load = function()
	{
		$scope.viewModel = undefined;
		$scope.stMessage = '';

		if ($scope.nuYear == '' ||
			$scope.nuMonth == undefined )
		{
			$scope.stMessage = 'Please inform all date fields';
		}
		else
		{
			$scope.loading = true;

            Api.Timesheet.listPage({ nuYear: $scope.nuYear, nuMonth: $scope.nuMonth, fkUser: $scope.userId }, function (data)
			{
				if (data.fail == true)
				{
					$scope.viewModel = undefined;
					$scope.stMessage = 'No records found';
				}					
				else
					$scope.viewModel = data;
			});

			$scope.loading = false;
		}
	}

	$scope.show = function (id) {
		$state.go('task', { id: id });
	}

}]);
