
angular.module('app.controllers').controller('EmissoraDespesaRecController',
    ['$scope', '$rootScope', '$state', '$stateParams', 'Api',
        function ($scope, $rootScope, $state, $stateParams, Api ) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;

            $scope.viewModel = {  };
            $scope.campos = {  };

            var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

            function init() {
                if (id > 0) {
                    $scope.loading = true;
                    $scope.id = id;
                    Api.EmissoraDespesaRec.get({ id: id }, function (data) {
                        $scope.viewModel = data;
                        $scope.loading = false;
                    },
                        function (response) {
                            if (response.status === 404) { toastr.error('Invalid ID', 'Erro'); }
                            $scope.list();
                        });
                }
                else {
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

            $scope.save = function () {

                $scope.loading = true;

                $scope.cod_fail = invalidCheck($scope.viewModel.stCodigo);
                $scope.desc_fail = invalidCheck($scope.viewModel.stDescricao);

                if (!$scope.cod_fail && !$scope.desc_fail)
                {
                    if (id > 0) {
                        Api.EmissoraDespesaRec.update({ id: id }, $scope.viewModel, function (data) {
                            toastr.success('Despesa rec. salva!', 'Sucesso');
                            $scope.loading = false;
                        },
                            function (response) {
                                toastr.error(response.data.message, 'Erro');
                                $scope.loading = false;
                            });
                    }
                    else {
                        Api.EmissoraDespesaRec.add($scope.viewModel, function (data) {
                            toastr.success('Despesa rec. salva!', 'Sucesso');
                            $state.go('empListagemDespesa');
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

            $scope.list = function () {
                $state.go('empListagemDespesaRec');
            };

            init();

        }]);
