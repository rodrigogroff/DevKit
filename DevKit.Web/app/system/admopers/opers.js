
angular.module('app.controllers').controller('AdmOpersController',
    ['$scope', '$rootScope', 'Api', 
        function ($scope, $rootScope, Api) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.mostraModal = false;

            $scope.campos = {};

            $scope.limpaPendenciaModal = function () {
                $scope.mostraModal = true;
            };

            $scope.cancelaPendencia = function () {
                $scope.mostraModal = false;
            };

            $scope.limpaPendencia = function () {
                $scope.mostraModal = false;
                $scope.loading = true;
                $scope.campos.resultado = '';

                Api.AdmOper.listPage({
                    op: '1',
                    di: $scope.campos.dtInicial,
                    df: $scope.campos.dtFinal,
                    nsu: $scope.campos.nsu,
                },
                    function (data) {
                        $scope.campos.resultado = data.resp + ' pendências resolvidas';
                        $scope.loading = false;
                    },
                    function (response) {
                        toastr.success(response.data, 'Sucesso');
                        $scope.loading = false;
                    });
            };

            function init() {
                Api.AdmOper.listPage({
                    op: '0'
                },
                    function (data) {
                        $scope.campos.dtInicial = data.di;
                    },
                    function (response) {
                    });
            }

            init();

        }]);
