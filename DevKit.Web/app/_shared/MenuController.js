'use strict';

angular.module('app.controllers').controller('MenuController',
['$scope', '$rootScope', '$location', 'AuthService', 'Api', 'version','$state',
function ($scope, $rootScope, $location, AuthService, Api, version, $state)
{
	init();

	function init()
    {
        $scope.goHome = function () {
            $state.go('home');
        }

        $scope.logOut = function () {
            AuthService.logOut();
            $location.path('/login');
        };

        $scope.version = version;

        AuthService.fillAuthData();        
		$scope.authentication = AuthService.authentication;

		if (!AuthService.authentication.isAuth)
			$location.path('/login');
	}

}]);