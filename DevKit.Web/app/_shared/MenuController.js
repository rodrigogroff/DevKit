'use strict';

angular.module('app.controllers').controller('MenuController',
    ['$scope', '$rootScope', '$location', 'AuthService', 'Api', 'version', '$state', '$window',
        function ($scope, $rootScope, $location, AuthService, Api, version, $state, $window) {
            $scope.version = version;
            $scope.searchParam = '';

            init();

            function init()
            {
                var tipo = $rootScope.tipo;

                $rootScope.mobileVersion = $window.innerWidth < 1000;

                AuthService.fillAuthData();

                $scope.authentication = AuthService.authentication;

                if (!AuthService.authentication.isAuth)
                    $location.path('login');

                $scope.resizeReady = false;
                $scope.width = $window.innerWidth;
                
                $scope.resizeReady = true;
            }

            $scope.logOut = function ()
            {
                AuthService.fillAuthData();

                $scope.authentication = AuthService.authentication;

                var tipo = $scope.authentication.tipo;

                if (tipo == 2 || tipo == 1)
                    AuthService.logOut();
                else
                    window.location = '/login?tipo=' + tipo;
            };

        }]);
