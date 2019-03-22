
angular.module('app.controllers').controller('EmissoraDBAListagemFaturamentoController',
    ['$scope', '$state', 'Api', 
        function ($scope, $state, Api ) {
            $scope.loading = false;

            $scope.search = function () {
                $scope.load(0, $scope.itensporpagina);
                $scope.paginador.reiniciar();
            };

            $scope.load = function (skip, take) {
                /*
                $scope.loading = true;

                var opcoes = {
                    skip: skip,
                    take: take,
                };

                Api.EmpresaDBA.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
                */
            };

            function init() {
                $scope.campos = {
                    tipoDemonstrativo: '2'
                };                
            }

            init();

        }]);
