
angular.module('app.controllers').controller('DBAListUsuariosController',
    ['$scope', '$rootScope', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
        function ($scope, $rootScope, $state, ngHistoricoFiltro, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.tipo = $rootScope.tipo;

            $scope.itensporpagina = 15;

            $scope.campos = {
                oper: '',
                cont: '',
                selects: {
                    empresa: ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 }),
                }
            };

            init();

            function init() {
                if (ngHistoricoFiltro.filtro)
                    ngHistoricoFiltro.filtro.exibeFiltro = false;
            }

            $scope.search = function () {
                $scope.load(0, $scope.itensporpagina);
                $scope.paginador.reiniciar();
            };

            $scope.show = function (mdl) {
                console.log(mdl);
                $state.go('usuario', { id: mdl.id });
            };

            $scope.load = function (skip, take) {
                
                $scope.loading = true;

                var opcoes = {
                    idEmpresa: $scope.campos.idEmpresa,
                    busca: $scope.campos.nome,
                    skip: skip,
                    take: take,
                };

                Api.DBAUsuario.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            };

        }]);
