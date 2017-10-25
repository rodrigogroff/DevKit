'use strict';
angular.module('app.controllers').controller('LoginController',
['$scope', '$rootScope', '$location', '$state', 'AuthService', 'version', 'Api', '$stateParams',
function ($scope, $rootScope, $location, $state, AuthService, version, Api, $stateParams)
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

    init();

    function init()
    {
        $scope.tipo = $location.search().tipo;

        if ($scope.tipo == '' || $scope.tipo == undefined)
            $scope.tipo = 1;
    }

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
            var lData = { };
            
            if ($scope.tipo == 2)
            {
                // usuarios

                lData.userName = "2." +
                    $scope.loginData.userName + "." +
                    $scope.loginData.userNameMat + "." +
                    $scope.loginData.userNameAcesso + "." +
                    $scope.loginData.userNameVenc;
            }
            else
            {
                // lojista

                lData.userName = "1" + $scope.loginData.userName;
            }

            lData.password = $scope.loginData.password;
            lData.tipo = $scope.tipo;

            AuthService.login(lData).then(function (response)
    		{
                $scope.loginOK = true;
                $rootScope.exibirMenu = true;              

                if ($scope.tipo == 2)
                {
                    // usuarios

                    $state.go('limitesUsr', {});
                }
                else 
                {
                    // lojistas

                    if ($scope.loginData.userName == "DBA") {
                        $rootScope.lojistaLogado = "DBA";
                        $rootScope.lojistaEnd = "Modo de configuração do portal";

                        $state.go('relatorios', {});
                    }
                    else {

                        Api.LojistaMensagens.listPage({}, function (data) {
                            if (data.count == 0)
                                $state.go('venda', {});
                            else
                                $state.go('mensagens', {});
                        });
                    }
                }
                                
    		},
			function (err)
			{
				$scope.loading = false;
				$scope.mensagem = err.error_description;
			});
    	}
    };

}]);
