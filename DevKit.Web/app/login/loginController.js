'use strict';
angular.module('app.controllers').controller('LoginController',
['$scope', '$rootScope', '$location', 'AuthService', 'version',
function ($scope, $rootScope, $location, AuthService, version)
{
	$rootScope.exibirMenu = false;

	$scope.version = version;	
	$scope.loading = false;
	$scope.loginOK = false;
	$scope.mensagem = "";

	$scope.loginData =
		{
			userName: "",
			password: ""
		};

    $scope.login = function ()
    {
    	$scope.loading = true;

    	if ($scope.loginData.userName == '' ||
			$scope.loginData.password == '')
    	{
    		$scope.loading = false;
    		$scope.mensagem = 'Please enter valid credentials';
    	}
    	else
    	{
    		AuthService.login($scope.loginData).then(function (response)
    		{
    			$scope.loginOK = true;
    			$rootScope.exibirMenu = true;
    			$rootScope.$broadcast('updateCounters');

    			if ($scope.loginData.userName == $scope.loginData.password)
    				$location.path('/system/userChangePass/');
    			else
    				$location.path('/');    				
    		},
			function (err)
			{
				$scope.loading = false;
				$scope.mensagem = err.error_description;
			});
    	}
    };

}]);