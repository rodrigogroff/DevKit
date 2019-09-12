'use strict';

angular.module('app.controllers').controller('LojaController',
    ['$scope', '$state', '$stateParams', '$rootScope', 'Api', 'ngSelects',
        function ($scope, $state, $stateParams, $rootScope, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.date = new Date();

            $scope.viewModel = {};
            $scope.campos = {};

            var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

            init();

            function init()
            {
                $scope.selectMes = ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 });
                $scope.selectEmpresa = ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 });    

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
                $scope.cnpj_fail = invalidCheck($scope.viewModel.nu_CNPJ);

                $scope.viewModel.novaMensagem = $scope.viewModelMensagem;

                if (!$scope.nome_fail && !$scope.cnpj_fail)
                {
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
                            $state.go('loja', { id: data.i_unique });                            
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

            // --------------------
            // -- terminal --
            // --------------------

            $scope.editarTerminal = function (mdl) {
                $scope.viewModelTerm = mdl;
                $scope.modalEditarTerminal = true;
            };

            $scope.closeModalEditTerminal = function () {
                $scope.viewModelTerm = null;
                $scope.modalEditarTerminal = false;
            };

            $scope.saveEditTerminal = function () {

                $scope.viewModel.editTerminal = $scope.viewModelTerm;

                Api.Loja.update({ id: id }, $scope.viewModel, function (data) {
                    toastr.success('Terminal salvo!', 'Sucesso');
                    $scope.viewModel.editTerminal = null;
                    $scope.modalEditarTerminal = false;
                },
                    function (response) {
                        toastr.error(response.data.message, 'Error');
                    });
            };

            $scope.salvarNovoTerminal = function () {

                if ($scope.campos.texto.length == 0)
                    return;

                $scope.viewModel.novoTerminal =
                    {
                        texto: $scope.campos.texto
                    };

                Api.Loja.update({ id: id }, $scope.viewModel, function (data) {
                    toastr.success('Terminal adicionado!', 'Sucesso');

                    $scope.viewModel.novoTerminal = null;

                    $scope.viewModel.lstTerminais.push(data);
                    $scope.campos =
                        {
                        };
                },
                    function (response) {
                        toastr.error(response.data.message, 'Error');
                    });
            };

            // --------------------
            // -- convênio --
            // --------------------

            $scope.editarConvenio = function (mdl) {
                $scope.viewModelConv = mdl;
                $scope.modalEditarConvenio = true;
            };

            $scope.removerConvenio = function (mdl) {
                $scope.viewModelConv = mdl;

                $scope.viewModel.editConvenio = $scope.viewModelConv;
                $scope.viewModel.editConvenio.remover = 'true';

                Api.Loja.update({ id: id }, $scope.viewModel, function (data) {
                    toastr.success('Convênio removido!', 'Sucesso');
                    init();
                },
                function (response) {
                    toastr.error(response.data.message, 'Error');
                });
            };

            $scope.closeModalEditConvenio = function () {
                $scope.viewModelConv = null;
                $scope.modalEditarConvenio = false;
            };

            $scope.saveEditConvenio = function () {

                $scope.viewModel.editConvenio = $scope.viewModelConv;

                Api.Loja.update({ id: id }, $scope.viewModel, function (data) {
                    toastr.success('Convênio salvo!', 'Sucesso');

                    $scope.viewModel.editConvenio = null;
                    $scope.modalEditarConvenio = false;
                },
                function (response) {
                    toastr.error(response.data.message, 'Error');
                });
            };

            $scope.salvarNovoConvenio = function ()
            {
                if ($scope.campos.idEmpresa == undefined ||
                    $scope.campos.tx_admin == undefined)
                    return;

                $scope.viewModel.novoConvenio =
                    {
                        idEmpresa: $scope.campos.idEmpresa,
                        tx_admin: $scope.campos.tx_admin
                    };

                Api.Loja.update({ id: id }, $scope.viewModel, function (data) {
                    toastr.success('Convênio adicionado!', 'Sucesso');
                    $scope.viewModel.lstConvenios.push(data);
                    $scope.viewModel.novoConvenio = null;
                    $scope.campos =
                        {
                        };                    
                },
                function (response) {
                    toastr.error(response.data.message, 'Error');
                });
            };

        }]);
