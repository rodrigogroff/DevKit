'use strict';

angular.module('app.controllers').controller('MenuController',
['$scope', '$rootScope', '$location', 'AuthService', 'Api', 'version','$state',
function ($scope, $rootScope, $location, AuthService, Api, version, $state)
{
    $scope.goHome = function () {
        $state.go('home');
    }

    $scope.logOut = function () {
        $rootScope.tipo = undefined;
        AuthService.logOut();
    };

	function init()
    {
        $scope.version = version;

        AuthService.fillAuthData();        

        $scope.authentication = AuthService.authentication;
        $rootScope.tipo = $scope.authentication.tipo;

        if (!AuthService.authentication.isAuth)
            $location.path('/login');
    }

    init();

}]);