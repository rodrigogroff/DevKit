
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

            $scope.save = function () {

                $scope.st_nome_fail = invalidCheck($scope.viewModel.st_nome);                

                if ( !$scope.st_nome_fail )
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
                    $scope.msgErro = 'Verificar pendências de campos mandatórios';
                }
            };


            function init()
            {
                $scope.viewModel =
                    {
                        
                    };
                
                $scope.tipocob = ngSelects.obterConfiguracao(Api.TipoCob, {});
                $scope.selectDayMonths = ngSelects.obterConfiguracao(Api.DayMonthCombo, {});
                $scope.selectEmpresa = ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 });
                
                loadEntity();
            }

            init();

        }]);
