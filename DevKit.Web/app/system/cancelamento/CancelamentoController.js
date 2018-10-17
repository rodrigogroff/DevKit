
angular.module('app.controllers').controller('CancelamentoController',
    ['$scope', '$rootScope', 'Api', 
        function ($scope, $rootScope, Api) {

            $rootScope.exibirMenu = true;

            init();

            function init() {
                $scope.loading = false;
                $scope.viewModel = {};
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
                    nsu: $scope.viewModel.stNSU
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

                Api.CancelaVenda.listPage({
                    nsu: $scope.viewModel.stNSU
                },
                    function (data) {
                        $scope.viewModel.cupom = data.results;
                        $scope.loading = false;
                    },
                    function (response) {
                        $scope.viewModel.cupom = undefined;
                        $scope.viewModel.erroCancelamento = response.data.message;
                        $scope.loading = false;
                    });
            };

            $scope.printDiv = function (divName) {
                var printContents = "<table>";

                for (var i = 0; i < $scope.viewModel.cupom.length; i = i + 1)
                    printContents += "<tr><td>" + $scope.viewModel.cupom[i] + "&nbsp;</td></tr>";

                printContents += "</table>"

                var popupWin = window.open('', '_blank', 'width=800,height=600');
                popupWin.document.open();
                popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
                popupWin.document.close();
            };

        }]);
