angular.module('app.controllers').controller('TimesheetController',
['$window', '$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($window, $scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
	$scope.loading = false;

	$scope.windowWidth = 900; var w = angular.element($window);
	$scope.$watch( function () { return $window.innerWidth; },
	  function (value) { $scope.availWidth = value; if (value > 1400) $scope.windowWidth = 1630; else $scope.windowWidth = 900; },
	  true ); w.bind('resize', function () { $scope.$apply();	});

	$scope.permModel = {};	
	$scope.permID = 109;

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

	$scope.viewModel = undefined;

	init();

	function init()
	{
		CheckPermissions();

		var currentDate = new Date();

		$scope.nuYear = currentDate.getFullYear();
		$scope.nuMonth = currentDate.getMonth() + 1;

		$scope.selectMonths = ngSelects.obterConfiguracao(Api.Month, { tamanhoPagina: 99, campoNome: 'stName' });
	}
	
	$scope.load = function()
	{		
		$scope.viewModel = undefined;
		$scope.loading = true;

		var options = { nuYear: $scope.nuYear, nuMonth: $scope.nuMonth };

		Api.Timesheet.listPage(options, function (data)
		{
			if (data.fail == true)
				$scope.viewModel = undefined;
			else
				$scope.viewModel = data;
		});

		$scope.loading = false;
	}

	$scope.showTask = function (id) {
		$state.go('task', { id: id });
	}

}]);
