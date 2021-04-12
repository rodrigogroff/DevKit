
angular.module('app.controllers').controller('EmissoraListagemLancCCController',
    ['$scope', '$rootScope', '$state', 'ngHistoricoFiltro', 'Api', 
        function ($scope, $rootScope, $state, ngHistoricoFiltro, Api ) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.tipo = $rootScope.tipo;

            $scope.itensporpagina = 15;

            $scope.campos = { };
            
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
                    nome: $scope.campos.nome,
                };

                Api.EmissoraLancCC.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            };

            $scope.show = function (mdl) {
                $state.go('empDespesa', { id: mdl.id });
            };

            $scope.new = function () {
                $state.go('empDespesa', { });
            };

        }]);
