﻿'use strict';

angular.module('app.controllers').controller('LojaController',
    ['$scope', '$state', '$stateParams', '$rootScope', 'Api', 'ngSelects',
        function ($scope, $state, $stateParams, $rootScope, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.date = new Date();

            $scope.viewModel = {};

            $scope.selectMes = ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 });

            var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

            init();

            function init()
            {
                $scope.viewModelMensagem =
                    {
                        mes_final: $scope.date.getMonth() + 1,
                        ano_final: $scope.date.getFullYear(),
                        dia_final: $scope.date.getDate(),
                        ativa: true
                    };

                if (id > 0) {
                    $scope.loading = true;

                    Api.Loja.get({ id: id }, function (data) {
                        $scope.viewModel = data;
                        $scope.loading = false;
                    },
                        function (response) {
                            if (response.status === 404) { toastr.error('Invalid ID', 'Error'); }
                            $scope.list();
                        });
                }
            }

            var invalidCheck = function (element) {
                if (element == undefined)
                    return true;
                else
                    if (element.length == 0)
                        return true;

                return false;
            };

            $scope.save = function () {
                $scope.nome_fail = invalidCheck($scope.viewModel.st_nome);

                $scope.viewModel.novaMensagem = $scope.viewModelMensagem;

                if (!$scope.nome_fail) {
                    if (id > 0) {
                        Api.Loja.update({ id: id }, $scope.viewModel, function (data) {
                            toastr.success('Loja salva!', 'Sucesso');
                            init(); //recarrega mensagens
                        },
                            function (response) {
                                toastr.error(response.data.message, 'Error');
                            });
                    }
                    else {
                        Api.Loja.add($scope.viewModel, function (data) {
                            toastr.success('Loja adicionada!', 'Sucesso');
                            $state.go('lojas');
                        },
                            function (response) {
                                toastr.error(response.data.message, 'Error');
                            });
                    }
                }
            };

            $scope.showMsg = function (mdl) {
                $scope.viewModelMensagem = mdl;
            };

            $scope.list = function () {
                $state.go('lojas');
            };

        }]);
