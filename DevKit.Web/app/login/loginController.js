'use strict';
angular.module('app.controllers').controller('LoginController',
['$scope', '$rootScope', '$location', 'AuthService', 'version',
function ($scope, $rootScope, $location, AuthService, version)
{
	$scope.version = version;
	$rootScope.exibirMenu = false;
	$scope.loading = false;

    $scope.loginData = {
        userName: "",
        password: ""
    };

    $scope.mensagem = "";

    $scope.login = function () {

    	$scope.loading = true;

    	AuthService.login($scope.loginData).then(function (response)
    	{
        	$scope.loading = false;

        	$rootScope.exibirMenu = true;
        	$rootScope.$broadcast('updateCounters');

        	$location.path('/');
        },
		function (err)
		{
         	 $scope.loading = false;
             $scope.mensagem = err.error_description;
        });
    };
}]);