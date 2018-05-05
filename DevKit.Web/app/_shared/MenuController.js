'use strict';

angular.module('app.controllers').controller('MenuController',
['$scope', '$rootScope', '$location', 'AuthService', 'Api', 'version', '$state', '$window',
function ($scope, $rootScope, $location, AuthService, Api, version, $state, $window)
{
	$scope.version = version;
	$scope.searchParam = '';

    $scope.mobileVersion = false;
    $scope.resizeReady = false;

    var w = angular.element($window);

    $scope.$watch(function () { return $window.innerWidth; },
        function (value)
        {
            $scope.resizeReady = false;
            $scope.width = $window.innerWidth;
            $scope.mobileVersion = $window.innerWidth < 1000;
            $scope.resizeReady = true;
        },
        true);

    w.bind('resize', function () { $scope.$apply(); $scope.resizeReady = true; });

	init();

	function init()
	{
		AuthService.fillAuthData();

		$scope.authentication = AuthService.authentication;

        var tipo = $rootScope.tipo;

		if (!AuthService.authentication.isAuth)
            $location.path('login');    
	}

//	$scope.searchSystem = function () {
	//	$location.path('/task/tasks').search({ searchSystem: $scope.searchParam });
	//}

    $scope.logOut = function ()
    {
        AuthService.fillAuthData();

        $scope.authentication = AuthService.authentication;

        console.log($scope.authentication)

        var tipo = $scope.authentication.tipo;

        if (tipo == 2 || tipo == 1)
            AuthService.logOut();
        else
            window.location = '/login?tipo=' + tipo;        
    };

}]);