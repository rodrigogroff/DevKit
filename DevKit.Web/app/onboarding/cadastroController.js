'use strict';

angular.module('app.controllers').controller('CadastroController',
    ['$scope', '$rootScope', 'Api', '$window', 'ngSelects',
        function ($scope, $rootScope, Api, $window, ngSelects ) {

            $rootScope.exibirMenu = false;
            $scope.loading = false;

            $scope.stepAtual = 1;

            $scope.MyWidth = $window.innerWidth - 15;

            $scope.onboardingData =
                {
                    email: '',
                    senha: '',                    
                };

            var invalidCheck = function (element) {

                if (element == undefined)
                    return true;
                else
                    if (element.trim().length === 0)
                        return true;

                return false;
            };

            var invalidCheckName = function (element, minLen) {

                if (element == undefined)
                    return true;
                else
                    if (element.trim().length === 0)
                        return true;

                if (element.length < minLen)
                    return true;

                var testChars = '@!#$%¨&*()-=+_0123456789<>:;?/'

                for (var i = 0; i < testChars.length; i++) {
                    if (element.indexOf(testChars[i]) > 0)
                        return true;
                }

                return false;
            };

            var invalidEmail = function (element) {
                if (element == undefined)
                    return true;
                else {
                    if (element.trim().length === 0)
                        return true;

                    var testChars = '!"#$%¨&*()-=+ '

                    for (var i = 0; i < testChars.length; i++)
                        if (element.indexOf(testChars[i]) > 0)
                            return true;

                    var indexArroba = element.indexOf('@');

                    if (indexArroba < 1 || indexArroba >= element.length - 3)
                        return true;    

                    var ponto = element.indexOf('.', indexArroba);

                    if (ponto < 1) return true;
                    if (ponto > element.length - 3) return true;
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

            $scope.confereValores = function () {
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
                $scope.fail_assInst = false;

                console.log($scope.stepAtual)

                switch ($scope.stepAtual) {

                    case 1: $scope.fail_email = invalidEmail($scope.onboardingData.email);
                        if (!$scope.fail_email)
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

                    case 4: $scope.fail_razSoc = invalidCheckName($scope.onboardingData.razSoc, 15);
                        $scope.fail_fantasia = invalidCheckName($scope.onboardingData.fantasia, 15);
                        if (!$scope.fail_razSoc && !$scope.fail_fantasia)
                            $scope.stepAtual = $scope.stepAtual + 1;
                        break;

                    case 5: $scope.fail_cep = invalidCheck($scope.onboardingData.cep);
                        $scope.fail_numero = invalidCheck($scope.onboardingData.numero);

                        $scope.validaCep();
                        if (!$scope.fail_cep && !$scope.fail_numero && $scope.onboardingData.cepStr != undefined) {
                            if ($scope.onboardingData.cepStr.length > 0)
                                $scope.stepAtual = $scope.stepAtual + 1;
                        }
                        break;

                    case 6: $scope.fail_cepInst = invalidCheck($scope.onboardingData.cepInst);
                        $scope.fail_numeroInst = invalidCheck($scope.onboardingData.numeroInst);

                        $scope.validaCepInst();
                        if (!$scope.fail_cepInst && !$scope.fail_numeroInst && $scope.onboardingData.cepInstStr != undefined) {
                            if ($scope.onboardingData.cepInstStr.length > 0)
                                $scope.stepAtual = $scope.stepAtual + 1;
                        }
                        break;

                    case 7: $scope.fail_tel_cel = invalidCheck($scope.onboardingData.telCel);
                        if (!$scope.fail_tel_cel)
                            $scope.stepAtual = $scope.stepAtual + 1;
                        break;

                    case 8: $scope.fail_resp = invalidCheck($scope.onboardingData.resp);
                        if (!$scope.fail_resp)
                            $scope.stepAtual = $scope.stepAtual + 1;
                        break;

                    case 9: $scope.fail_sitef = $scope.onboardingData.sitef == undefined;
                        if (!$scope.fail_sitef)
                            $scope.stepAtual = $scope.stepAtual + 1;
                        break;

                    case 10: $scope.fail_assInst = $scope.onboardingData.assInst == undefined;
                        if (!$scope.fail_assInst)
                            $scope.stepAtual = $scope.stepAtual + 1;
                        break;

                    case 11:
                        $scope.stepAtual = $scope.stepAtual + 1;
                        $scope.modalConf = true;
                        break;
                }
            }

            var getJSON = function (url, callback) {
                var xhr = new XMLHttpRequest();
                xhr.open('GET', url, true);
                xhr.responseType = 'json';
                xhr.onload = function () {
                    var status = xhr.status;
                    if (status === 200) {
                        callback(null, xhr.response);
                    } else {
                        callback(status, xhr.response);
                    }
                };
                xhr.send();
            };

            $scope.mesmoCep = function () {                
                $scope.onboardingData.cepInst = $scope.onboardingData.cep;
                $scope.onboardingData.numeroInst = $scope.onboardingData.numero;
                $scope.validaCepInst();
            }

            $scope.validaCep = function () {    
                setTimeout(function wait() { 
                    getJSON('https://viacep.com.br/ws/' + $scope.onboardingData.cep + '/json/',
                        function (err, data) {
                            if (err !== null) { } else {

                                if (data.logradouro == undefined)
                                    $scope.onboardingData.cepStr = '';
                                else
                                    $scope.onboardingData.cepStr = data.logradouro + ", " + data.localidade + ", " + data.uf

                                $scope.$apply() 
                            }
                        });

                }, 200)
            }

            $scope.validaCepInst = function () {
                setTimeout(function wait() {
                    getJSON('https://viacep.com.br/ws/' + $scope.onboardingData.cepInst + '/json/',
                        function (err, data) {
                            if (err !== null) { } else {

                                if (data.logradouro == undefined)
                                    $scope.onboardingData.cepInstStr = '';
                                else
                                    $scope.onboardingData.cepInstStr = data.logradouro + ", " + data.localidade + ", " + data.uf

                                $scope.$apply()
                            }
                        });

                }, 200)
            }

            $scope.proximo = function () {
                $scope.confereValores();
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

            $scope.confirmar = function () {
                Api.OnboardingLojista.add($scope.onboardingData, function (data) {                    
                    $scope.modalConf = false;
                    $scope.modalAviso = true;                    
                },
                    function (response) {
                        toastr.error(response.data.message, 'Erro');
                    });
            }

            init();

            function init() {

                $scope.selectBanco = ngSelects.obterConfiguracao(Api.BancosCombo, { tamanhoPagina: 15 });

                $scope.modalConf = false;
                $scope.modalAviso = false;

                setTimeout(function wait() {
                    $scope.MyWidth = $window.innerWidth - 15;
                    $scope.stepAtual = 1;
                    $scope.setaFocus();
                }, 200)
            }
        }]);

