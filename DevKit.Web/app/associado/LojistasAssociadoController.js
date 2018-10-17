
angular.module('app.controllers').controller('LojistasAssociadoController',
    ['$scope', '$rootScope', 'ngHistoricoFiltro', 'Api', 
        function ($scope, $rootScope, ngHistoricoFiltro, Api) {

            $rootScope.exibirMenu = true;
            $rootScope.mobileVersion = false;

            $scope.itensporpagina = 15;
            $scope.loading = false;

            init();

            function init() {
                if (ngHistoricoFiltro.filtro)
                    ngHistoricoFiltro.filtro.exibeFiltro = false;
            }

            $scope.search = function () {
                $scope.load(0, $scope.itensporpagina);
                $scope.paginador.reiniciar();
            };

            $scope.load = function (skip, take) {

                $scope.loading = true;

                var opcoes = {
                    skip: skip,
                    take: take,
                    busca: $scope.busca
                };

                angular.extend(opcoes, $scope.campos);

                delete opcoes.selects;

                Api.LojistasAssociado.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            };

        }]);
