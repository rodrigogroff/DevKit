'use strict';

angular.module('app.controllers').controller('LoginController',
    ['$scope', '$rootScope', '$location', '$state', 'AuthService', 'version', 'Api', 
        function ($scope, $rootScope, $location, $state, AuthService, version, Api) {

            $rootScope.exibirMenu = false;
            $scope.loading = false;
            $scope.loginOK = false;

            $scope.version = version;
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

            $scope.esqueci = function () {
                $scope.loadingSenha = true;

                if ($scope.loginData.userName === '') {
                    $scope.loading = false;
                    $scope.mensagem = 'Favor entre com um email válido';
                }
                else {
                    var opcoes = {
                        email: $scope.loginData.userName,
                    };

                    Api.UsuarioParceiroDBAResetSenha.listPage(opcoes, function (data) {
                        $scope.loadingSenha = false;
                        $scope.mensagem = 'Email enviado com sua nova senha';
                    },
                        function (response) {
                            $scope.loadingSenha = false;
                            $scope.mensagem = 'Email não registrado no sistema';                            
                        });
                }
            };

            $scope.login = function () {
                $scope.loading = true;

                if ($scope.loginData.userName == '' ||
                    $scope.loginData.password == '') {
                    $scope.loading = false;
                    $scope.mensagem = 'Favor entre com um email válido';
                }
                else {
                    var lData = {};

                    if ($rootScope.tipo == undefined) {
                        $rootScope.tipo = 1;
                    }
                    else if ($rootScope.tipo == 2) {
                        var stA = $scope.loginData.userNameAcesso.toString();
                        var stV = $scope.loginData.userNameVenc.toString();

                        if (stA.length == 3) stA = "0" + stA;
                        if (stV.length == 3) stV = "0" + stV;

                        lData.userName = "2." +
                            $scope.loginData.userName.toString() + "." +
                            $scope.loginData.userNameMat.toString() + "." +
                            stA + "." +
                            stV;
                    }
                    else if ($rootScope.tipo == 4) {
                        // emissoras
                        lData.userName = "4." +
                            $scope.loginData.userEmp + "." +
                            $scope.loginData.userName;

                        $rootScope.stEmpresa = $scope.loginData.m1;
                    }
                    else if ($rootScope.tipo == 1 ||
                        $rootScope.tipo == 3 ||
                        $rootScope.tipo == 5) {
                        lData.userName = $rootScope.tipo + $scope.loginData.userName.toString();
                    }

                    lData.password = $scope.loginData.password;
                    lData.tipo = $rootScope.tipo;

                    AuthService.login(lData).then(function (response) {
                        $scope.loginOK = true;
                        $rootScope.exibirMenu = true;

                        if ($rootScope.tipo == 5) {
                            $rootScope.lojistaLogado = "DBA";
                            $rootScope.lojistaEnd = "Modo de configuração do portal";

                            AuthService.fillAuthData();

                            if (AuthService.authentication.parceiro != '1')
                                $rootScope.modoParceiro = "true";

                            $state.go('dashboard', {});
                        }
                        else if ($rootScope.tipo == 4) {

                            //AuthService.fillAuthData();
                            //console.log(AuthService.authentication);

                        }
                        else if ($rootScope.tipo == 2) {
                            if ($rootScope.mobileVersion == true)
                                $state.go('limitesUsrMobile', {});
                            else
                                $state.go('limitesUsr', {});
                        }
                        else if ($rootScope.tipo == 1 || $rootScope.tipo == 3) {
                            Api.LojistaMensagens.listPage({}, function (data) {
                                if ($rootScope.mobileVersion == true) {
                                    $state.go('vendamobile', {});
                                }
                                else {
                                    if (data.count == 0 && $rootScope.tipo == 1) {
                                        $state.go('venda', {});
                                    }
                                    else if (data.count > 0) {
                                        $state.go('mensagens', {});
                                    }
                                }
                            });
                        }

                    },
                        function (err) {
                            $scope.loading = false;
                            $scope.mensagem = err.error_description;
                        });
                }
            };

        }]);
