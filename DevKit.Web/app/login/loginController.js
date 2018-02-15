'use strict';

angular.module('app.controllers').controller('LoginController',
['$scope', '$rootScope', '$location', 'AuthService', 'version', 'Api', '$state', '$stateParams',
function ($scope, $rootScope, $location, AuthService, version, Api, $state, $stateParams)
{
    $rootScope.exibirMenu = false;

    init();

    function init()
    {
        $scope.version = version;
        $scope.loading = false;
        $scope.loginOK = false;
        $scope.mensagem = "";

        $scope.loginData =
            {
                userName: "",
                password: ""
            };

        var tipoId = 5;//($stateParams.tipo) ? parseInt($stateParams.tipo) : 0;

        console.log($stateParams);

        if (tipoId != 0)
            $rootScope.tipo = tipoId;
    }

    $scope.redirLogin = function (tiporedir) {
        $rootScope.tipo = tiporedir;
    }

    $scope.voltar = function () {
        $rootScope.tipo = undefined;
    }

    $scope.login = function ()
    {
    	$scope.loading = true;

        if ($scope.loginData.password == '')
    	{
    		$scope.loading = false;
    		$scope.mensagem = 'Entre com credenciais válidas!';
    	}
    	else
        {
            if ($rootScope.tipo == undefined)
                $rootScope.tipo = 1;

            var lData = {
                password: $scope.loginData.password,
            }

            if ($rootScope.tipo == 1) {
                lData.userName = $rootScope.tipo + ":" + $scope.loginData.nuCredenciado;
            }
            else if ($rootScope.tipo == 4) {
                lData.userName = $rootScope.tipo + ":" + $scope.loginData.nuEmpresa + ":" + $scope.loginData.userName;
            }
            else if ($rootScope.tipo == 5) {
                lData.userName = $rootScope.tipo + ":" + $scope.loginData.userName;
            }

            AuthService.login(lData).then(function (response)
    		{
                $scope.loginOK = true;
                $rootScope.exibirMenu = true;

                if ($rootScope.tipo == 1)
                {
                    if ($scope.loginData.nuCredenciado == $scope.loginData.password)
                    {
                        $state.go('credenciadopass');
                        toastr.error('Sua senha expirou.', 'Controle de senha');
                    }
                    else
                        $state.go('autorizacaoProc');
                }
                else if ($rootScope.tipo == 4)
                {
                    if ($scope.loginData.userName == $scope.loginData.password)
                    {
                        $state.go('userChangePass');
                        toastr.error('Sua senha expirou.', 'Controle de senha');
                    }
                    else
                        $location.path('/');
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
