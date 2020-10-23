'use strict';

angular.module('app.controllers').controller('CadastroController',
    ['$scope', '$rootScope', '$location', '$state', 'AuthService', 'version', 'Api', '$window',
        function ($scope, $rootScope, $location, $state, AuthService, version, Api, $window ) {

            $rootScope.exibirMenu = false;
            $scope.loading = false;

            $scope.stepAtual = 1;

            $scope.MyWidth = $window.innerWidth - 15;

            $scope.onboardingData =
                {
                    email: "",
                    senha: "",
                };

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

            function validarCNPJ(cnpj) {

                cnpj = cnpj.replace(/[^\d]+/g, '');

                if (cnpj == '') return false;

                if (cnpj.length != 14)
                    return false;

                // Elimina CNPJs invalidos conhecidos
                if (cnpj == "00000000000000" ||
                    cnpj == "11111111111111" ||
                    cnpj == "22222222222222" ||
                    cnpj == "33333333333333" ||
                    cnpj == "44444444444444" ||
                    cnpj == "55555555555555" ||
                    cnpj == "66666666666666" ||
                    cnpj == "77777777777777" ||
                    cnpj == "88888888888888" ||
                    cnpj == "99999999999999")
                    return false;

                // Valida DVs
                var tamanho = cnpj.length - 2
                var numeros = cnpj.substring(0, tamanho);
                var digitos = cnpj.substring(tamanho);
                var soma = 0;
                var pos = tamanho - 7;

                for (let i = tamanho; i >= 1; i--) {
                    soma += numeros.charAt(tamanho - i) * pos--;
                    if (pos < 2)
                        pos = 9;
                }
                var resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != digitos.charAt(0))
                    return false;

                tamanho = tamanho + 1;
                numeros = cnpj.substring(0, tamanho);
                soma = 0;
                pos = tamanho - 7;
                for (let i = tamanho; i >= 1; i--) {
                    soma += numeros.charAt(tamanho - i) * pos--;
                    if (pos < 2)
                        pos = 9;
                }
                resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != digitos.charAt(1))
                    return false;

                return true;
            }

            $scope.proximo = function () {
                $scope.fail_email = false;
                $scope.fail_senha = false;
                $scope.fail_conf_senha = false;
                $scope.fail_cnpj = false;
                $scope.fail_razSoc = false;
                $scope.fail_fantasia = false;
                $scope.fail_cep = false;
                $scope.fail_cepInst = false;
                $scope.fail_tel_cel = false;
                $scope.fail_resp = false;

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

                    case 3: $scope.fail_cnpj = !validarCNPJ($scope.onboardingData.cnpj);
                            if (!$scope.fail_cnpj)
                                $scope.stepAtual = $scope.stepAtual + 1;
                        break;

                    case 4: $scope.fail_razSoc = invalidCheck($scope.onboardingData.razSoc);
                        $scope.fail_fantasia = invalidCheck($scope.onboardingData.fantasia);
                        if (!$scope.fail_razSoc && !$scope.fail_fantasia)
                            $scope.stepAtual = $scope.stepAtual + 1;
                        break;

                    case 5: $scope.fail_cep = invalidCheck($scope.onboardingData.cep);                        
                        if (!$scope.fail_cep)
                            $scope.stepAtual = $scope.stepAtual + 1;
                        break;

                    case 6: $scope.fail_cepInst = invalidCheck($scope.onboardingData.cepInst);
                        if (!$scope.fail_cepInst)
                            $scope.stepAtual = $scope.stepAtual + 1;
                        break;

                    case 7: $scope.fail_tel_cel = invalidCheck($scope.onboardingData.telCel);
                        if (!$scope.fail_tel_cel)
                            $scope.stepAtual = $scope.stepAtual + 1;
                        break;

                    case 8: $scope.fail_resp = invalidCheck($scope.onboardingData.resp);
                        if (!$scope.fail_resp)
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
                        case 4: document.getElementById("razSoc").focus(); break;
                        case 5: document.getElementById("cep").focus(); break;
                        case 6: document.getElementById("cepInst").focus(); break;
                        case 7: document.getElementById("telCel").focus(); break;
                        case 8: document.getElementById("resp").focus(); break;
                    }
                }, 200)               
            }

            $scope.volta = function () {
                $scope.setaFocus();   
            }

            $scope.anterior = function () {
                $scope.stepAtual = $scope.stepAtual - 1;
            }

            init();

            function init() {
                setTimeout(function wait() {
                    $scope.MyWidth = $window.innerWidth - 15;
                    $scope.stepAtual = 1;
                    $scope.setaFocus();
                }, 200)
            }
        }]);

