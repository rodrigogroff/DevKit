'use strict';

angular.module('app.controllers').controller('MenuController',
    ['$scope', '$rootScope', 'AuthService', 'version', '$window',
        function ($scope, $rootScope, AuthService, version, $window) {
            $scope.version = version;
            $scope.searchParam = '';

            init();

            function init()
            {
                var tipo = $rootScope.tipo;

                $rootScope.mobileVersion = false;// $window.innerWidth < 1000;

                AuthService.fillAuthData();

                $scope.authentication = AuthService.authentication;

                $scope.resizeReady = false;
                $scope.width = $window.innerWidth;
                
                $scope.resizeReady = true;
            }

            $scope.logOut = function (mTipo)
            {                
                AuthService.logOut();

                $rootScope.loginOK = false;
                $rootScope.exibirMenu = false;

                window.location = 'https://meuconvey.conveynet.com.br/login?tipo=' + mTipo;
            };

        }]);
