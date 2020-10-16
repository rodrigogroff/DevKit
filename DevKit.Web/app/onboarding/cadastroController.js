'use strict';

angular.module('app.controllers').controller('CadastroController',
    ['$scope', '$rootScope', '$location', '$state', 'AuthService', 'version', 'Api', 
        function ($scope, $rootScope, $location, $state, AuthService, version, Api) {

            $rootScope.exibirMenu = false;
            $scope.loading = false;

            $scope.stepAtual = 1;

            $scope.onboardingData =
                {
                    email: "",
                    senha: "",
                };

            init();

            var invalidCheck = function (element) {

                if (element == undefined)
                    return true;
                else
                    if (element.trim().length === 0)
                        return true;

                return false;
            };

            var invalidEmail = function (element) {
                if (element == undefined)
                    return true;
                else {
                    if (element.trim().length === 0)
                        return true;

                    var indexArroba = element.indexOf('@');

                    if (indexArroba < 1 || indexArroba >= element.length - 3)
                        return true;                    
                }

                return false;
            };

            function init() {
                setTimeout(function wait() {
                    switch ($scope.stepAtual) {
                        case 1: document.getElementById("email").focus(); break;
                    }
                }, 200)
            }

            $scope.proximo = function () {
                $scope.fail_email = false;
                $scope.fail_senha = false;
                $scope.fail_conf_senha = false;
                $scope.fail_cnpj = false;

                switch ($scope.stepAtual) {

                    case 1: $scope.fail_email = invalidEmail($scope.onboardingData.email);
                            if (!$scope.fail_email )
                                $scope.stepAtual = $scope.stepAtual + 1;
                            break;

                    case 2: $scope.fail_senha = invalidCheck($scope.onboardingData.senha);
                            $scope.fail_conf_senha = invalidCheck($scope.onboardingData.senhaConf);

                            if ($scope.onboardingData.senha !== $scope.onboardingData.senhaConf)
                                $scope.fail_conf_senha = true;

                            if (!$scope.fail_senha && !$scope.fail_conf_senha)
                                $scope.stepAtual = $scope.stepAtual + 1;
                        break;

                    case 3: $scope.fail_cnpj = invalidCheck($scope.onboardingData.cnpj);
                            if (!$scope.fail_cnpj)
                                $scope.stepAtual = $scope.stepAtual + 1;
                            break;
                }

                $scope.setaFocus();
            }

            $scope.setaFocus = function () {
                setTimeout(function wait() {
                    switch ($scope.stepAtual) {
                        case 1: document.getElementById("email").focus(); break;
                        case 2: document.getElementById("senha").focus(); break;
                        case 3: document.getElementById("cnpj").focus(); break;
                    }
                }, 200)               
            }

            $scope.volta = function () {
                $scope.setaFocus();   
            }

            $scope.anterior = function () {
                $scope.stepAtual = $scope.stepAtual - 1;
            }
        }]);

