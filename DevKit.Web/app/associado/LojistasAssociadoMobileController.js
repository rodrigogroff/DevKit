
angular.module('app.controllers').controller('LojistasAssociadoMobileController',
    ['$scope', '$rootScope', 'ngHistoricoFiltro', 'Api', '$window',
        function ($scope, $rootScope, ngHistoricoFiltro, Api, $window) {

            $rootScope.exibirMenu = true;

            $scope.itensporpagina = 15;
            $scope.loading = false;

            init();

            function init()
            {
                $scope.setWidth = $window.innerWidth - 15;

                if (ngHistoricoFiltro.filtro)
                    ngHistoricoFiltro.filtro.exibeFiltro = false;
            }

            $scope.search = function () {
                $scope.setWidth = $window.innerWidth - 15;

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
