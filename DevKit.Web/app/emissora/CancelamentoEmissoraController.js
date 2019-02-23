
angular.module('app.controllers').controller('CancelamentoEmissoraController',
    ['$scope', '$rootScope', 'Api', 
        function ($scope, $rootScope, Api) {

            $rootScope.exibirMenu = true;

            init();

            function init() {
                $scope.loading = false;
                $scope.viewModel = {};

                Api.DataServer.listPage({}, function (data) {
                    $scope.viewModel.dt = data.dt;
                });
            }

            $scope.limpar = function () {
                $scope.viewModel = {};
            };

            $scope.conferirNSU = function () {
                $scope.viewModel.error = '';

                $scope.stNSU_fail = invalidCheck($scope.viewModel.stNSU);

                if ($scope.stNSU_fail)
                    return;

                $scope.loading = true;

                Api.ConfereNSU.listPage({
                    nsu: $scope.viewModel.stNSU,
                    dt: $scope.viewModel.dt
                },
                    function (data) {
                        $scope.viewModel.cupom = data.results;
                        $scope.loading = false;
                    },
                    function (response) {
                        $scope.viewModel.error = response.data.message;
                        $scope.loading = false;
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

            $scope.confirmar = function () {
                $scope.loading = true;

                Api.CancelaVendaDBA.listPage({
                    nsu: $scope.viewModel.stNSU,
                    dt: $scope.viewModel.dt
                },
                function (data) {
                    $scope.loading = false;
                    $scope.viewModel.cupom = null;
                    toastr.success('Transação cancelada!', 'Sucesso');
                },
                function (response) {
                    $scope.loading = false;
                    toastr.error('Transação não cancelada!', 'Erro');
                });
            };

        }]);
