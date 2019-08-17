
angular.module('app.controllers').controller('DBAEditUsuarioController',
    ['$scope', '$state', '$stateParams', 'Api', 'ngSelects',
        function ($scope, $state, $stateParams, Api, ngSelects) {
            $scope.loading = false;

            var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

            function loadEntity() {
                if (id > 0) {
                    $scope.loading = true;

                    Api.DBAUsuario.get({ id: id }, function (data) {
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

            $scope.trocaSenha = function () {
                $scope.viewModel.modal = true;
            };

            $scope.fecharModal = function () {
                $scope.viewModel.modal = false;
            };

            var invalidCheck = function (element) {
                if (element == undefined)
                    return true;
                else
                    if (element.length == 0)
                        return true;

                return false;
            };

            $scope.save = function () {
                if (id > 0) {

                    $scope.viewModel.id = id;

                    Api.DBAUsuario.update({ id: id }, $scope.viewModel, function (data) {
                        toastr.success('Dados do usuário atualizados!', 'Sucesso');
                        $scope.viewModel.modal = false;
                    },
                    function (response) {
                        toastr.error(response.data.message, 'Erro');
                    });
                }
            };

            $scope.list = function () {
                $state.go('dbaUsuarios');
            };
            
            function init()
            {
                $scope.modal = false;
                $scope.viewModel =
                    {
                        
                    };
                
                $scope.selectEmpresa = ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 });
                
                loadEntity();
            }

            init();

        }]);
