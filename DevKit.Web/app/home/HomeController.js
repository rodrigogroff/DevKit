angular.module('app.controllers').controller('HomeController',
['$window', '$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($window, $scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
	$rootScope.exibirMenu = true;

	$scope.loading = false;

	$scope.windowWidth = 900; var w = angular.element($window);
	$scope.$watch( function () { return $window.innerWidth; },
	  function (value) { $scope.availWidth = value; if (value > 1400) $scope.windowWidth = 1630; else $scope.windowWidth = 900; },
	  true ); w.bind('resize', function () { $scope.$apply();	});

	$scope.viewModel = undefined;
	
	init();

	function init()
	{
		$scope.loading = true;

		Api.HomeView.listPage({ }, function (data)
		{
			$scope.viewModel = data;
			$scope.loading = false;
		});
	}

	$scope.markAsRead = function(mdl)
	{
		mdl.updateCommand = 'maskAsRead';

		Api.News.update({ id: mdl.id }, mdl, function (data)
		{
			init();
		},
		function (response) {
			toastr.error(response.data.message, 'Error');
		});
	}

	$scope.currentOption = undefined;

	$scope.surveySelectOption = function (option)
	{		
		$scope.currentOption = option.id;
	}

	$scope.confirmChoiceSurvey = function (mdl)
	{		
		mdl.updateCommand = 'optionSelect';
		mdl.anexedEntity = { id: $scope.currentOption };

		Api.Survey.update({ id: mdl.id }, mdl, function (data)
		{
			init();
			$scope.currentOption = undefined;
		},
		function (response) {
			toastr.error(response.data.message, 'Error');
		});
	}

}]);
