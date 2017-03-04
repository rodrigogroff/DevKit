'use strict';
angular.module('app.controllers').controller('MenuController', ['$scope', '$rootScope','$location', 'AuthService', function ($scope, $rootScope, $location, AuthService) {

    $rootScope.exibirMenu = $rootScope.exibirMenu || true;

    AuthService.fillAuthData();
    $scope.authentication = AuthService.authentication;

    if (!AuthService.authentication.isAuth)
        $location.path('/login');

    $scope.logOut = function () {
        AuthService.logOut();
        $location.path('/login');
    };

}]);