
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

            $scope.atualizar = function () {
                Api.EmpresaDBA.get({ id: id }, function (data) {
                    $scope.viewModel = data;
                    $scope.loading = false;
                },
                    function (response) {
                        if (response.status === 404) { toastr.error('Invalid ID', 'Erro'); }
                        $scope.list();
                    });
            };

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

                $scope.msgErro = '';

                $scope.st_fantasia_fail = invalidCheck($scope.viewModel.st_fantasia);                
                $scope.cnpj_fail = invalidCheck($scope.viewModel.nu_CNPJ);
                $scope.st_empresa_fail = invalidCheck($scope.viewModel.st_empresa);
                $scope.fech_fail = false;
                
                if ($scope.viewModel.nu_diaFech > 0 && $scope.viewModel.st_horaFech.length > 0)
                    $scope.fech_fail = invalidCheck($scope.viewModel.nu_diaFech) || invalidCheck($scope.viewModel.st_horaFech);

                if ($scope.st_fantasia_fail == false &&
                    $scope.cnpj_fail == false &&
                    $scope.st_empresa_fail == false &&
                    $scope.fech_fail == false )
                {
                    if (id > 0) {
                        $scope.viewModel.updateCommand = "entity";
                        $scope.modalConf = true;
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
                        $scope.tabCadastro_fail = '(*)';

                    if ($scope.fech_fail == true)
                        $scope.tabFechamento_fail = '(*)';                    

                    $scope.msgErro = 'Verificar pendências de campos mandatórios';
                }
            };

            $scope.closeModalConf = function () {
                $scope.modalConf = false;
            };

            $scope.confirmar = function () {

                $scope.viewModel.updateCommand = "entity";

                Api.EmpresaDBA.update({ id: id }, $scope.viewModel, function (data) {
                    toastr.success('Dados atualizados!', 'Sucesso');
                    $scope.modalConf = false;
                },
                    function (response) {
                        toastr.error(response.data.message, 'Erro');
                    });

            };

            $scope.list = function () {
                $state.go('empresas');
            };

            // ============================================
            // despesa
            // ============================================

            $scope.addDespesa = false;

            $scope.removeDespesa = function (index, lista) {
                $scope.modalDespesa = true;
                $scope.delDespesa = $scope.viewModel.despesas[index];
            };

            $scope.closeModalDespesa = function () {
                $scope.modalDespesa = false;
            };

            $scope.removerDespesaModal = function () {
                $scope.viewModel.updateCommand = "removeDespesa";
                $scope.viewModel.anexedDespesa = $scope.delDespesa;
                Api.EmpresaDBA.update({ id: id }, $scope.viewModel, function (data) {
                    $scope.modalDespesa = false;
                    loadEntity();
                });
            };

            $scope.addNewDespesa = function () {
                $scope.addDespesa = !$scope.addDespesa;
            };

            $scope.newDespesa = {};

            $scope.editDespesa = function (mdl) {
                $scope.addDespesa = true;
                $scope.newDespesa = mdl;
            };

            $scope.cancelDespesa = function () {
                $scope.addDespesa = false;
                $scope.newDespesa = {};
            };

            $scope.saveNewDespesa = function () {
                $scope.desp_stCodigo_fail = invalidCheck($scope.newDespesa.stCodigo);
                $scope.desp_stDesc_fail = invalidCheck($scope.newDespesa.stDescricao);

                if (!$scope.desp_stCodigo_fail && !$scope.desp_stDesc_fail)
                {
                    $scope.addDespesa = false;

                    $scope.viewModel.updateCommand = "newDespesa";
                    $scope.viewModel.anexedDespesa = $scope.newDespesa;

                    Api.EmpresaDBA.update({ id: id }, $scope.viewModel, function (data) {
                        $scope.newDespesa = {};                        
                        loadEntity();
                    },
                    function (response) {
                        toastr.error(response.data.message, 'Erro');
                    });
                }
            };

            // ============================================
            // despesas recorrentes
            // ============================================

            $scope.addDespesaRec = false;

            $scope.removeDespesaRec = function (index, lista) {
                $scope.modalDespesaRec = true;
                $scope.delDespesaRec = $scope.viewModel.despesasRec[index];
            };

            $scope.closeModalDespesaRec = function () {
                $scope.modalDespesaRec = false;
            };

            $scope.removerDespesaRecModal = function () {
                $scope.viewModel.updateCommand = "removeDespesaRec";
                $scope.viewModel.anexedDespesaRec = $scope.delDespesaRec;
                Api.EmpresaDBA.update({ id: id }, $scope.viewModel, function (data) {
                    $scope.modalDespesa = false;
                    loadEntity();
                });
            };

            $scope.addNewDespesaRec = function () {
                $scope.addDespesaRec = !$scope.addDespesaRec;
            };

            $scope.newDespesaRec = {};

            $scope.editDespesaRec = function (mdl) {
                $scope.addDespesaRec = true;
                $scope.newDespesaRec = mdl;
            };

            $scope.cancelDespesaRec = function () {
                $scope.addDespesaRec = false;
                $scope.newDespesaRec = {};
            };

            $scope.saveNewDespesaRec = function () {
                $scope.despRec_stCodigo_fail = invalidCheck($scope.newDespesaRec.stCodigo);
                $scope.despRec_stDesc_fail = invalidCheck($scope.newDespesaRec.stDescricao);

                if (!$scope.despRec_stCodigo_fail && !$scope.despRec_stDesc_fail) {
                    $scope.addDespesaRec = false;

                    $scope.viewModel.updateCommand = "newDespesaRec";
                    $scope.viewModel.anexedDespesaRec = $scope.newDespesaRec;

                    Api.EmpresaDBA.update({ id: id }, $scope.viewModel, function (data) {
                        $scope.newDespesaRec = {};
                        loadEntity();
                    },
                        function (response) {
                            toastr.error(response.data.message, 'Erro');
                        });
                }
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

                $scope.selectParceiro = ngSelects.obterConfiguracao(Api.Parceiro, {});

                loadEntity();
            }

            init();

        }]);
