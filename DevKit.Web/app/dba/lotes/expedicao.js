
angular.module('app.controllers').controller('ListingExpController',
    ['$scope',  '$state', 'Api', 'ngSelects',
        function ($scope, $state, Api, ngSelects) {

            $scope.loading = false;

            $scope.search = function () {
                $scope.load(0, $scope.itensporpagina);
                $scope.paginador.reiniciar();
            };

            $scope.load = function (skip, take) {
                $scope.loading = true;

                var opcoes = {
                    skip: skip,
                    take: take,
                    dtInicial: $scope.campos.dtInicial,
                    dtFinal: $scope.campos.dtFinal,
                    idEmpresa: $scope.campos.idEmpresa,
                    ordem: $scope.campos.ordem,
                };

                Api.ExpedicaoDBA.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            };

            function init() {
                $scope.campos = {
                    codigo: '',
                    ordem: '1',
                    selects: {
                        empresa: ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 })
                    }
                };                
            }

            init();

        }]);
