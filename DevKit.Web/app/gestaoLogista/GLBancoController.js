
angular.module('app.controllers').controller('GLBancoController',
    ['$scope', '$rootScope', 'ngHistoricoFiltro', 'Api', 'ngSelects',
        function ($scope, $rootScope, ngHistoricoFiltro, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;

            init();

            function init() {

                $scope.selectBanco = ngSelects.obterConfiguracao(Api.BancosCombo, { tamanhoPagina: 15 });

                $scope.loading = true;

                Api.Loja.get({ id: 0 }, function (data) {
                    $scope.viewModel = data;
                    $scope.loading = false;
                },
                    function (response) {
                        if (response.status === 404) { toastr.error('Invalid ID', 'Erro'); }
                        $scope.list();
                    });
            }

            $scope.atualizar = function () {
                
                Api.Loja.update({ id: $scope.viewModel.id }, $scope.viewModel, function (data) {
                    toastr.success('Cadastro atualizado!', 'Sucesso');
                    init();
                },
                    function (response) {
                        toastr.error(response.data.message, 'Erro');
                    });
                
            };

        }]);
