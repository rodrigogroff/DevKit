
angular.module('app.controllers').controller('ExtratosMobileController',
    ['$scope', '$state', 'Api', 'ngSelects',
        function ($scope, $state, Api, ngSelects) {
                        
            $scope.date = new Date();
            $scope.loading = false;

            $scope.selectMeses = ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 });

            $scope.opcoes =
                {
                    extrato_fech_mes: '',
                    extrato_fech_ano_inicial: $scope.date.getFullYear()
                };

            $scope.$watch("tipoExtrato", function (novo, antigo) {
                if (novo != antigo)
                    $scope.list = undefined;
            });

            init();

            function init() {
                $scope.loading = false;
                $scope.extrato_fech_ano_inicial = $scope.date.getFullYear();
            }

            $scope.pesquisar = function () {
                $scope.opcoes.tipo = $scope.tipoExtrato;

                Api.ExtratoAssociado.listPage($scope.opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.total;
                    $scope.mesAtual = data.mesAtual;
                    $scope.saldoDisp = data.saldoDisp;
                    $scope.loading = false;
                });
            };

            $scope.listaDetalheFuturo = undefined;

            $scope.menu = function () {
                $state.go('menuUsr', {});
            };

        }]);
