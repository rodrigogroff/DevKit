'use strict';
angular.module('app.controllers').controller('LoginController',
['$scope', '$rootScope', '$location', '$state', 'AuthService', 'version', 'Api', '$stateParams', '$window',
function ($scope, $rootScope, $location, $state, AuthService, version, Api, $stateParams, $window)
{
    $rootScope.exibirMenu = false;
    $scope.mobileVersion = false;
    
    var w = angular.element($window);

    $scope.$watch(function () { return $window.innerWidth; },
        function (value) { $scope.width = $window.innerWidth + ", " + $window.innerHeight; $scope.mobileVersion = $window.innerWidth < 1000; }, true);

    w.bind('resize', function () { $scope.$apply(); });
    
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
        if ($rootScope.tipo == undefined)
            $rootScope.tipo = $location.search().tipo;
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

            if ($rootScope.tipo == undefined)
                $rootScope.tipo = 1;

            if ($rootScope.tipo == 2)
            {
                // usuarios
                lData.userName = "2." +
                    $scope.loginData.userName + "." +
                    $scope.loginData.userNameMat + "." +
                    $scope.loginData.userNameAcesso + "." +
                    $scope.loginData.userNameVenc;
            }
            else if ($rootScope.tipo == 4)
            {
                // emissoras
                lData.userName = "4." +
                    $scope.loginData.userEmp + "." +
                    $scope.loginData.userName;
            }
            else if ($rootScope.tipo == 1 || $rootScope.tipo == 3)
            {
                // lojista
                lData.userName = $rootScope.tipo + $scope.loginData.userName;
            }

            lData.password = $scope.loginData.password;
            lData.tipo = $scope.tipo;

            AuthService.login(lData).then(function (response)
    		{
                $scope.loginOK = true;
                $rootScope.exibirMenu = true;              

                if ($rootScope.tipo == 2)
                {
                    // usuarios
                    $rootScope.mobileVersion = true;

                    $state.go('limitesUsr', {});
                }
                else if ($rootScope.tipo == 1 || $rootScope.tipo == 3)
                {
                    // lojistas

                    if ($scope.loginData.userName == "DBA") {
                        $rootScope.lojistaLogado = "DBA";
                        $rootScope.lojistaEnd = "Modo de configuração do portal";

                        $state.go('relAssociados', {});
                    }
                    else
                    {
                        Api.LojistaMensagens.listPage({}, function (data)
                        {
                            if (data.count == 0 && $rootScope.tipo == 1)
                            {
                                $state.go('venda', {});
                            }
                            else if (data.count > 0)
                            {
                                $state.go('mensagens', {});
                            }                                
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
