
angular.module('app.controllers').controller('HomeController',
['$window', '$scope', '$rootScope', '$state', 'Api', 'ngSelects',
function ($window, $scope, $rootScope, $state, Api, ngSelects)
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;
	
	init();

    function loadHomeView () {
        $scope.loading = true;

        Api.HomeView.listPage({}, function (data) {
            $scope.viewModel = data;
            $scope.loading = false;
        });
    }

	function init()
    {
        $scope.windowWidth = 900; var w = angular.element($window);
        $scope.$watch(function () { return $window.innerWidth; },
            function (value) { $scope.availWidth = value; if (value > 1400) $scope.windowWidth = 1630; else $scope.windowWidth = 900; },
            true); w.bind('resize', function () { $scope.$apply(); });

        $scope.viewModel = undefined;

        loadHomeView();
    }    

	$scope.showTask = function (mdl) {
		$state.go('task', { id: mdl.id });
	}

	$scope.showQuestion = function (mdl) {
		$state.go('task', { id: mdl.fkTask });
	}

	$scope.markAsRead = function(mdl)
    {
        mdl.updateCommand = 'maskAsRead';

        Api.News.update({ id: mdl.id }, mdl, function (data)
		{
            loadHomeView();
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
            $scope.currentOption = undefined;

            loadHomeView();			
		},
		function (response) {
			toastr.error(response.data.message, 'Error');
		});
	}

}]);
