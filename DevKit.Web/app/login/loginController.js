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

    init();

    function init() {
        if ($rootScope.tipo == undefined)
            $rootScope.tipo = $location.search().tipo;
    }

    $scope.login = function ()
    {
    	$scope.loading = true;

        if ($scope.loginData.nuEmpresa == '' ||
            $scope.loginData.userName == '' ||
			$scope.loginData.password == '')
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
                lData.userName = $rootScope.tipo + ":" + $scope.loginData.nuMedico;
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

                if ($rootScope.tipo == 1) {
                    $location.path('/');
                }
                else {
                    if ($scope.loginData.userName == $scope.loginData.password) {
                        $location.path('/system/userChangePass/');
                        toastr.error('Sua senha expirou.', 'Controle de senha');
                    }
                    else
                        $location.path('/');
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
