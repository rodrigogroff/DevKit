
angular.module('app.controllers').controller('EmissoraNovoCartaoController',
    ['$scope', '$rootScope', '$state', '$stateParams', 'Api','ngSelects',
        function ($scope, $rootScope, $state, $stateParams, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;

            $scope.viewModel =
                {
                    limMes: '0,00',
                    limTot: '0,00',
                    via: '01',
                    situacao: 'Em cadastramento',
                };

            $scope.campos = {
                matOk: true,
                selects: {
                    empresa: ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 }),
                }
            };

            var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

            init();

            function init() {
                if (id > 0) {
                    $scope.loading = true;

                    $scope.id = id;

                    Api.EmissoraCartao.get({ id: id }, function (data) {
                        $scope.viewModel = data;
                        $scope.loading = false;
                    },
                        function (response) {
                            if (response.status === 404) { toastr.error('Invalid ID', 'Error'); }
                            $scope.list();
                        });
                }
                else {
                    $scope.viewModel =
                        {
                            limMes: '0,00',
                            limTot: '0,00',
                            via: '01',
                            situacao: 'Em cadastramento',
                        };

                    id = 0;
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

            $scope.altLim = function () {
                $scope.limMes_fail = invalidCheck($scope.viewModel.limMes);
                $scope.limTot_fail = invalidCheck($scope.viewModel.limTot);

                if ($scope.limMes_fail || $scope.limTot_fail) {
                    toastr.error('Dados inconsistentes para mudança de limites!', 'Erro');
                    return;
                }

                $scope.loading = true;

                var opcoes = {
                    id: id,
                    modo: 'altLim',
                    valor: $scope.viewModel.limMes + "|" + $scope.viewModel.limTot
                };

                Api.EmissoraCartao.update({ id: id }, opcoes, function (data) {
                    toastr.success('Limites trocados com sucesso!', 'Sucesso');
                    $scope.loading = false;
                },
                    function (response) {
                        toastr.error(response.data.message, 'Erro');
                        $scope.loading = false;
                    });
            };

            $scope.criaDep = function () {
                $scope.novoDep_fail = invalidCheck($scope.viewModel.novoDep);

                if ($scope.novoDep_fail)
                    return;

                $scope.loading = true;

                var opcoes = {
                    id: id,
                    modo: 'criaDep',
                    valor: $scope.viewModel.novoDep
                };

                Api.EmissoraCartao.update({ id: id }, opcoes, function (data) {
                    toastr.success('Dependente criado com sucesso!', 'Sucesso');
                    $scope.loading = false;
                    init();
                },
                    function (response) {
                        toastr.error(response.data.message, 'Erro');
                        $scope.loading = false;
                    });
            };

            $scope.checkMat = function () {

                $scope.loading = true;

                var opcoes = {
                    idEmpresa: $scope.viewModel.fkEmpresa,
                    skip: 0,
                    take: 1,
                    matricula: $scope.viewModel.matricula,
                };

                Api.RelAssociados.listPage(opcoes, function (data) {
                    $scope.loading = false;

                    $scope.mat_ok = false;
                    $scope.mat_fail = false;

                    if (data.count == 0) {
                        $scope.mat_ok = true;
                    }
                    else {
                        $scope.mat_fail = true;                    
                    }                        
                });
            };

            $scope.altSenha = function () {

                $scope.senha_fail = invalidCheck($scope.viewModel.senhaAtual);
                $scope.senhaConf_fail = invalidCheck($scope.viewModel.senhaConf);

                if ($scope.senha_fail == false && $scope.senhaConf_fail == false) {
                    if ($scope.viewModel.senhaAtual.length != 4) {
                        $scope.senhaConf_fail = true;
                        toastr.error('Senha precisa ter 4 catacteres!', 'Erro');
                        return;
                    }

                    if ($scope.viewModel.senhaAtual != $scope.viewModel.senhaConf) {
                        $scope.senhaConf_fail = true;
                        toastr.error('Confirmação de senha inválida!', 'Erro');
                        return;
                    }
                }

                if ($scope.senha_fail || $scope.senhaConf_fail) {
                    toastr.error('Dados inconsistentes para troca de senha!', 'Erro');
                    return;
                }

                $scope.loading = true;

                var opcoes = {
                    id: id,
                    modo: 'altSenha',
                    valor: $scope.viewModel.senhaAtual
                };

                Api.EmissoraCartao.update({ id: id }, opcoes, function (data) {
                    toastr.success('Senha trocada com sucesso!', 'Sucesso');
                    $scope.viewModel.senhaAtual = undefined;
                    $scope.viewModel.senhaConf = undefined;
                    $scope.loading = false;
                },
                    function (response) {
                        toastr.error(response.data.message, 'Erro');
                        $scope.loading = false;
                    });
            };

            $scope.save = function () {

                $scope.loading = true;

                $scope.mat_fail = invalidCheck($scope.viewModel.matricula);

                $scope.checkMat();

                $scope.venc_fail = invalidCheck($scope.viewModel.vencMes) || invalidCheck($scope.viewModel.vencAno);
                $scope.nome_fail = invalidCheck($scope.viewModel.nome);
                $scope.cpf_fail = invalidCheck($scope.viewModel.cpf);

                /*
                $scope.dtNasc_fail = invalidCheck($scope.viewModel.dtNasc);
                $scope.limMes_fail = invalidCheck($scope.viewModel.limMes);
                $scope.limTot_fail = invalidCheck($scope.viewModel.limTot);
                $scope.banco_fail = invalidCheck($scope.viewModel.banco);
                $scope.bancoAg_fail = invalidCheck($scope.viewModel.bancoAg);
                $scope.bancoCta_fail = invalidCheck($scope.viewModel.bancoCta);
                $scope.tel_fail = invalidCheck($scope.viewModel.tel);
                $scope.email_fail = invalidCheck($scope.viewModel.email);
                */

                if ($scope.tipo == 5)
                    if ($scope.viewModel.fkEmpresa == undefined) {
                        $scope.emp_fail = true;
                        toastr.error('Selecione a empresa do novo cartão', 'Erro');
                        $scope.loading = false;
                    }

                if (!$scope.mat_fail
                    && !$scope.nome_fail
                    && !$scope.venc_fail

               ////         && !$scope.cpf_fail
                   ////     && !$scope.dtNasc_fail
                       // && $scope.limMes_fail
                       // && !$scope.limTot_fail
                        //&& !$scope.banco_fail
                        //&& !$scope.bancoAg_fail
                        //&& $scope.bancoCta_fail
                        //&& !$scope.tel_fail &&
                        //!$scope.email_fail
                    )
                {
                    if (id > 0) {
                        Api.EmissoraCartao.update({ id: id }, $scope.viewModel, function (data) {
                            toastr.success('Cartão salvo!', 'Sucesso');
                            $scope.loading = false;
                        },
                            function (response) {
                                toastr.error(response.data.message, 'Erro');
                                $scope.loading = false;
                            });
                    }
                    else {
                        Api.EmissoraCartao.add($scope.viewModel, function (data) {
                            toastr.success('Cartão salvo!', 'Sucesso');

                            $scope.retCartaoAdd = data;
                            $scope.modalSave = true;

                            //$state.go('empListagemCartao');
                        },
                            function (response) {
                                toastr.error(response.data.message, 'Erro');
                                $scope.loading = false;
                            });
                    }
                }
                else {
                    toastr.error('Existem pendências de cadastro', 'Erro');
                    $scope.loading = false;
                }
            };

            $scope.editarCartao = function () {
                $state.go('empManutCartao', { id: $scope.retCartaoAdd.i_unique });
            };

            $scope.novoCartao = function () {
                $scope.modalSave = false;
            };

        }]);
