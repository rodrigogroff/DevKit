
angular.module('app.controllers').controller('DBAAuditoriaController',
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

            $scope.load = function (skip, take) {
                
                $scope.loading = true;

                var opcoes = {
                    idEmpresa: $scope.campos.idEmpresa,
                    oper: $scope.campos.oper,
                    cont: $scope.campos.cont,
                    skip: skip,
                    take: take,
                    dtInicial: $scope.campos.dtInicial,
                    dtFinal: $scope.campos.dtFinal,
                };

                Api.Auditoria.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            };

            $scope.show = function (mdl) {
                $scope.cartaoSelecionado = mdl;
                $scope.modal = true;
            };

        }]);
