
angular.module('app.controllers').controller('EmissoraNovoCartaoController',
    ['$scope', '$rootScope', 'AuthService', '$state', '$stateParams', 'ngHistoricoFiltro', 'Api', 'ngSelects',
        function ($scope, $rootScope, AuthService, $state, $stateParams, ngHistoricoFiltro, Api, ngSelects) {
            $rootScope.exibirMenu = true;
            $scope.loading = false;

            $scope.viewModel =
                {
                    limMes: '0,00',
                    limTot: '0,00',
                    via: '01',
                    situacao: 'Em cadastramento',
                    cdAcesso: 'Indefinido',
                };

            var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

            init();

            function init() {
                if (id > 0) {
                    $scope.loading = true;

                    Api.EmissoraCartao.get({ id: id }, function (data) {
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
            }

            $scope.save = function () {
                $scope.mat_fail = invalidCheck($scope.viewModel.matricula);
                $scope.limMes_fail = invalidCheck($scope.viewModel.limMes);
                $scope.venc_fail = invalidCheck($scope.viewModel.vencMes) || invalidCheck($scope.viewModel.vencAno);
                $scope.nome_fail = invalidCheck($scope.viewModel.nome);
                $scope.cpf_fail = invalidCheck($scope.viewModel.cpf);
                $scope.dtNasc_fail = invalidCheck($scope.viewModel.dtNasc);
                $scope.limMes_fail = invalidCheck($scope.viewModel.limMes);
                $scope.limTot_fail = invalidCheck($scope.viewModel.limTot);
                $scope.banco_fail = invalidCheck($scope.viewModel.banco);
                $scope.bancoAg_fail = invalidCheck($scope.viewModel.bancoAg);
                $scope.bancoCta_fail = invalidCheck($scope.viewModel.bancoCta);
                $scope.tel_fail = invalidCheck($scope.viewModel.tel);
                $scope.email_fail = invalidCheck($scope.viewModel.email);
                
                if (!$scope.mat_fail &&
                    !$scope.nome_fail &&
                    !$scope.cpf_fail &&
                    !$scope.dtNasc_fail &&
                    !$scope.limMes_fail &&
                    !$scope.limTot_fail &&
                    !$scope.banco_fail &&
                    !$scope.bancoAg_fail &&
                    !$scope.bancoCta_fail &&
                    !$scope.tel_fail &&
                    !$scope.email_fail )
                {
                    if (id > 0) {
                        Api.EmissoraCartao.update({ id: id }, $scope.viewModel, function (data) {
                            toastr.success('Cartão salvo!', 'Sucesso');                            
                        },
                            function (response) {
                                toastr.error(response.data.message, 'Erro');
                            });
                    }
                    else {
                        Api.EmissoraCartao.add($scope.viewModel, function (data) {
                            toastr.success('Cartão salvo!', 'Sucesso');
                            $state.go('empListagemCartao');
                        },
                            function (response) {
                                toastr.error(response.data.message, 'Erro');
                            });
                    }
                }
                else
                {
                    toastr.error('Existem pendências de cadastro', 'Erro');
                }
            };

        }]);
