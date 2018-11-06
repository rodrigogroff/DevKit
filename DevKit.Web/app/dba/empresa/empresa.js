
angular.module('app.controllers').controller('EmpresaController',
    ['$scope', '$state', '$stateParams', 'Api', 'ngSelects',
        function ($scope, $state, $stateParams, Api, ngSelects) {
            $scope.loading = false;

            var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

            function loadEntity() {
                if (id > 0) {
                    $scope.loading = true;

                    Api.EmpresaDBA.get({ id: id }, function (data) {
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
                    if (element.length == 0)
                        return true;

                return false;
            };

            var invalidEmail = function (element) {
                if (element == undefined)
                    return true;
                else {
                    if (element.length == 0)
                        return true;

                    if (element.indexOf('@') < 1)
                        return true;
                }

                return false;
            };

            $scope.save = function () {
                $scope.st_fantasia_fail = invalidCheck($scope.viewModel.st_fantasia);                
                $scope.cnpj_fail = invalidCheck($scope.viewModel.nu_CNPJ);
                $scope.st_empresa_fail = invalidCheck($scope.viewModel.st_empresa);

                if ($scope.st_fantasia_fail == false &&
                    $scope.cnpj_fail == false &&
                    $scope.st_empresa_fail == false)
                {
                    if (id > 0) {
                        $scope.viewModel.updateCommand = "entity";

                        Api.EmpresaDBA.update({ id: id }, $scope.viewModel, function (data) {
                            toastr.success('Dados atualizados!', 'Sucesso');
                            $scope.msgErro = '';
                        },
                            function (response) {
                                toastr.error(response.data.message, 'Erro');
                            });
                    }
                    else {
                        Api.EmpresaDBA.add($scope.viewModel, function (data) {
                            toastr.success('Empresa adicionada!', 'Sucesso');
                            $state.go('empresa', { id: data.i_unique });
                        },
                            function (response) {
                                toastr.error(response.data.message, 'Erro');
                            });
                    }
                }
                else {
                    if ($scope.st_fantasia_fail || $scope.cnpj_fail || $scope.st_empresa_fail )
                        $scope.tabCadastro_fail = "(!)";

                    //if ($scope.mens_fail)
                      //  $scope.tabCobranca_fail = "(!)";

                    $scope.msgErro = 'Verificar pendências de campos mandatórios';
                }
            };

            $scope.list = function () {
                $state.go('empresas');
            };

            // ============================================
            // phone 
            // ============================================

            //$scope.addPhone = false;

            //$scope.removePhone = function (index, lista) {
            //    $scope.modalPhone = true;
            //    $scope.delPhone = $scope.viewModel.phones[index];
            //};

            //$scope.closeModalPhone = function () {
            //    $scope.modalPhone = false;
            //};

            //$scope.removerPhoneModal = function () {
            //    $scope.viewModel.updateCommand = "removePhone";
            //    $scope.viewModel.anexedEntity = $scope.delPhone;
            //    Api.Empresa.update({ id: id }, $scope.viewModel, function (data) {
            //        loadEntity();
            //    });
            //};

            //$scope.addNewPhone = function () {
            //    $scope.addPhone = !$scope.addPhone;
            //};

            //$scope.newPhone = {};

            //$scope.editPhone = function (mdl) {
            //    $scope.addPhone = true;
            //    $scope.newPhone = mdl;
            //};

            //$scope.cancelPhone = function () {
            //    $scope.addPhone = false;
            //    $scope.newPhone = {};
            //};

            //$scope.saveNewPhone = function () {
            //    $scope.stPhone_fail = invalidCheck($scope.newPhone.stTelefone);
            //    $scope.stDescription_fail = invalidCheck($scope.newPhone.stDesc);

            //    if (!$scope.stPhone_fail &&
            //        !$scope.stDescription_fail) {
            //        $scope.addPhone = false;

            //        $scope.viewModel.updateCommand = "newPhone";
            //        $scope.viewModel.anexedEntity = $scope.newPhone;

            //        Api.Empresa.update({ id: id }, $scope.viewModel, function (data) {
            //            $scope.newPhone = {};
            //            loadEntity();
            //        },
            //            function (response) {
            //                toastr.error(response.data.message, 'Erro');
            //            });
            //    }
            //};

            // ============================================
            // email 
            // ============================================

            //$scope.addEmail = false;

            //$scope.removeEmail = function (index, lista) {
            //    $scope.modalEmail = true;
            //    $scope.delEmail = $scope.viewModel.emails[index];
            //};

            //$scope.closeModalEmail = function () {
            //    $scope.modalEmail = false;
            //};

            //$scope.removerEmailModal = function () {
            //    $scope.viewModel.updateCommand = "removeEmail";
            //    $scope.viewModel.anexedEntity = $scope.delEmail;
            //    Api.Empresa.update({ id: id }, $scope.viewModel, function (data) {
            //        loadEntity();
            //    });
            //};

            //$scope.addNewEmail = function () {
            //    $scope.addEmail = !$scope.addEmail;
            //};

            //$scope.newEmail = {};

            //$scope.editEmail = function (mdl) {
            //    $scope.addEmail = true;
            //    $scope.newEmail = mdl;
            //};

            //$scope.cancelEmail = function () {
            //    $scope.addEmail = false;
            //    $scope.newEmail = {};
            //};

            //$scope.saveNewEmail = function () {
            //    $scope.stEmail_fail = invalidEmail($scope.newEmail.stEmail);
            //    $scope.stEmailContato_fail = invalidCheck($scope.newEmail.stContato);

            //    if (!$scope.stEmail_fail) {
            //        $scope.addEmail = false;

            //        $scope.viewModel.updateCommand = "newEmail";
            //        $scope.viewModel.anexedEntity = $scope.newEmail;

            //        Api.Empresa.update({ id: id }, $scope.viewModel, function (data) {
            //            $scope.newEmail = {};
            //            loadEntity();
            //        },
            //            function (response) {
            //                toastr.error(response.data.message, 'Erro');
            //            });
            //    }
            //};

            // ============================================
            // endereco
            // ============================================

            //$scope.addEnd = false;

            //$scope.removeEnd = function (index, lista) {
            //    $scope.modalEnd = true;
            //    $scope.delEnd = $scope.viewModel.enderecos[index];
            //};

            //$scope.closeModalEnd = function () {
            //    $scope.modalEnd = false;
            //};

            //$scope.removerEndModal = function () {
            //    $scope.viewModel.updateCommand = "removeEnd";
            //    $scope.viewModel.anexedEntity = $scope.delEnd;
            //    Api.Empresa.update({ id: id }, $scope.viewModel, function (data) {
            //        loadEntity();
            //    });
            //};

            //$scope.addNewEnd = function () {
            //    $scope.addEnd = !$scope.addEnd;
            //};

            //$scope.newEnd = {};

            //$scope.editEnd = function (mdl) {
            //    $scope.addEnd = true;
            //    $scope.newEnd = mdl;
            //};

            //$scope.cancelEnd = function () {
            //    $scope.addEnd = false;
            //    $scope.newEnd = {};
            //};

            //$scope.saveNewEnd = function () {
            //    $scope.stRua_fail = invalidCheck($scope.newEnd.stRua);
            //    $scope.est_fail = $scope.newEnd.fkEstado == undefined;
            //    $scope.cid_fail = $scope.newEnd.fkCidade == undefined;

            //    if (!$scope.stRua_fail && !$scope.est_fail && !$scope.cid_fail) {
            //        $scope.addEnd = false;

            //        $scope.viewModel.updateCommand = "newEnd";
            //        $scope.viewModel.anexedEntity = $scope.newEnd;

            //        Api.Empresa.update({ id: id }, $scope.viewModel, function (data) {
            //            $scope.newEnd = {};
            //            loadEntity();
            //        },
            //            function (response) {
            //                toastr.error(response.data.message, 'Erro');
            //            });
            //    }
            //};

            //$scope.testeEmail = function () {
            //    var opcoes = { skip: 0, take: 1, testeEmail: $scope.viewModel.id, mailDest: $scope.viewModel.stEmailTeste };

            //    Api.Empresa.listPage(opcoes, function (data) {
            //        toastr.success('Email enviado!', 'Aviso');
            //    },
            //        function (response) {
            //            toastr.error(response.data.message, 'Erro');
            //        });
            //};

            function init()
            {
                $scope.viewModel =
                    {
                        svrMensalidade: '0,00',
                        svrCartaoAtivo: '0,00',
                        svrMinimo: '0,00',
                        svrTransacao: '0,00',
                        snuFranquia: '0'
                    };

                $scope.permModel = {};

                $scope.tipocob = ngSelects.obterConfiguracao(Api.TipoCob, {});
                $scope.selectDayMonths = ngSelects.obterConfiguracao(Api.DayMonthCombo, {});
                $scope.selectMonths = ngSelects.obterConfiguracao(Api.MonthCombo, {});
                $scope.estado = ngSelects.obterConfiguracao(Api.EstadoCombo, { scope: $scope, filtro: { campo: 'fkPais', valor: '1' } });
                $scope.cidade = ngSelects.obterConfiguracao(Api.CidadeCombo, { scope: $scope, filtro: { campo: 'fkEstado', valor: 'newEnd.fkEstado' } });

                loadEntity();
            }

            init();

        }]);
