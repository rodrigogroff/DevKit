'use strict';
angular.module('app.controllers').controller('LoginController', ['$scope', '$rootScope', '$location', 'AuthService', 'version', function ($scope, $rootScope, $location, AuthService, version) {

	$scope.version = version;
    $rootScope.exibirMenu = false;

    $scope.loginData = {
        userName: "",
        password: ""
    };

    $scope.mensagem = "";

    $scope.login = function () {

        AuthService.login($scope.loginData).then(function (response) {
            $rootScope.exibirMenu = true;
            $location.path('/');

        },
         function (err) {
             $scope.mensagem = err.error_description;
         });
    };

}]);