﻿
angular.module('app.controllers').controller('ParceiroUsuarioController',
    ['$scope', '$state', '$stateParams', 'Api', 'ngSelects',
        function ($scope, $state, $stateParams, Api, ngSelects) {
            $scope.loading = false;

            var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

            function loadEntity() {
                if (id > 0) {
                    $scope.loading = true;

                    Api.UsuarioParceiroDBA.get({ id: id }, function (data) {
                        $scope.viewModel = data;
                        $scope.loading = false;
                    },
                        function (response) {
                            if (response.status === 404) { toastr.error('Invalid ID', 'Erro'); }
                            $scope.list();
                        });
                }
                else {
                    $scope.viewModel = { bActive: true, nuYearBirth: 1980 };
                }
            }

            var invalidCheck = function (element) {
                if (element == undefined)
                    return true;
                else
                    if (element.length === 0)
                        return true;

                return false;
            };

            $scope.save = function () {

                $scope.msgErro = '';

                $scope.st_email_fail = invalidCheck($scope.viewModel.stEmail);
                $scope.st_nome_fail = invalidCheck($scope.viewModel.stNome);

                if ($scope.st_email_fail === false && $scope.st_nome_fail === false )
                {
                    if (id > 0) {
                        $scope.viewModel.updateCommand = "entity";

                        Api.UsuarioParceiroDBA.update({ id: id }, $scope.viewModel, function (data) {
                            toastr.success('Dados atualizados!', 'Sucesso');
                            $scope.modalConf = false;
                        },
                            function (response) {
                                toastr.error(response.data.message, 'Erro');
                            });
                    }
                    else {
                        Api.UsuarioParceiroDBA.add($scope.viewModel, function (data) {
                            toastr.success('Parceiro adicionado!', 'Sucesso');
                            $scope.list();
                        },
                            function (response) {
                                toastr.error(response.data.message, 'Erro');
                            });
                    }
                }
                else {

                    if ($scope.st_fantasia_fail)
                        $scope.tabCadastro_fail = '(*)';

                    $scope.msgErro = 'Verificar pendências de campos mandatórios';
                }
            };

            $scope.list = function () {
                $state.go('parceirousuarios');
            };
            
            function init()
            {
                $scope.viewModel =
                    {
                        
                    };

                $scope.selectTipoUsuario = ngSelects.obterConfiguracao(Api.TipoUsuario, {});

                loadEntity();
            }

            init();

        }]);
