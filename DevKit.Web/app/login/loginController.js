'use strict';

angular.module('app.controllers').controller('LoginController',
['$scope', '$rootScope', '$location', 'AuthService', 'version','Api',
function ($scope, $rootScope, $location, AuthService, version, Api)
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
    		$scope.mensagem = 'Entre com credenciais válidas!';
    	}
    	else
    	{
    		AuthService.login($scope.loginData).then(function (response)
    		{
                $scope.loginOK = true;
                $rootScope.exibirMenu = true;
                
                if ($scope.loginData.userName == $scope.loginData.password)
    			{
    				$location.path('/system/userChangePass/');
    				toastr.error('Sua senha expirou.', 'Controle de senha');
    			}
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