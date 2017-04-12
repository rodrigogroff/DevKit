angular.module('app.controllers').controller('HomeController',
['$window', '$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($window, $scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
	$scope.loading = false;

	$scope.windowWidth = 900; var w = angular.element($window);
	$scope.$watch( function () { return $window.innerWidth; },
	  function (value) { $scope.availWidth = value; if (value > 1400) $scope.windowWidth = 1630; else $scope.windowWidth = 900; },
	  true ); w.bind('resize', function () { $scope.$apply();	});

	$scope.viewModel = undefined;
	
	init();

	function init()
	{
		CheckPermissions();

		$scope.loading = true;

		Api.HomeView.listPage({ }, function (data)
		{
			$scope.viewModel = data;
		});

		$scope.loading = false;
	}

}]);
