
angular.module('app.controllers').controller('ListingRastreamentoController',
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
                    todos: $scope.campos.todos,
                    idEmpresa: $scope.campos.idEmpresa,
                };

                Api.ListagemRastreamento.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            };

            function init() {

                $scope.selectEmpresa = ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 });

                $scope.campos = {
                    codigo: ''
                };                
            }

            init();

        }]);
