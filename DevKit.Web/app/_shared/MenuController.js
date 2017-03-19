'use strict';

angular.module('app.controllers').controller('MenuController',
['$scope', '$rootScope', '$location', 'AuthService', 'Api',
function ($scope, $rootScope, $location, AuthService, Api)
{
	$scope.userTasks = 0;

	init();

	function init()
	{
		AuthService.fillAuthData();

		$scope.authentication = AuthService.authentication;

		if (!AuthService.authentication.isAuth)
			$location.path('/login');

		Api.TaskCount.listPage({}, function (data)
		{
			$scope.userTasks = data.count_user_tasks;
		});
	}

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