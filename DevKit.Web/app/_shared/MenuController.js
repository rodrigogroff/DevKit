'use strict';

angular.module('app.controllers').controller('MenuController',
['$scope', '$rootScope', '$location', 'AuthService', 'Api',
function ($scope, $rootScope, $location, AuthService, Api)
{
	$scope.userTasks = 0;
	$scope.projectTasks = 0;
	$scope.userDeadlineTasks = 0;

	init();

	function init()
	{
		AuthService.fillAuthData();

		$scope.authentication = AuthService.authentication;

		if (!AuthService.authentication.isAuth)
			$location.path('/login');

		Api.TaskCount.listPage({}, function (data) {
			$scope.projectTasks = data.count_project_tasks;
			$scope.userTasks = data.count_user_tasks;
		});
	}

	$rootScope.$on("updateCounters", function ()
	{
		Api.TaskCount.listPage({}, function (data)
		{
			$scope.projectTasks = data.count_project_tasks;
			$scope.userTasks = data.count_user_tasks;
		});
	});

    $scope.logOut = function ()
    {
        AuthService.logOut();
        $location.path('/login');
    };

    $scope.tasksClick = function ()
    {
    	$location.path('/task/kanban');
    };

}]);