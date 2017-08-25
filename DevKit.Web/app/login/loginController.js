﻿'use strict';
angular.module('app.controllers').controller('LoginController',
['$scope', '$rootScope', '$location', '$state', 'AuthService', 'version','Api',
function ($scope, $rootScope, $location, $state, AuthService, version, Api)
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

                Api.Lojista.listPage({
                    terminal: $scope.loginData.userName,
                    senha: $scope.loginData.password,
                },
                function (data)
                {
                    $rootScope.lojistaLogado = data.results[0].nome;
                    $rootScope.lojistaEnd = data.results[0].endereco;
                });

                $state.go('venda', {});
    		},
			function (err)
			{
				$scope.loading = false;
				$scope.mensagem = err.error_description;
			});
    	}
    };

}]);