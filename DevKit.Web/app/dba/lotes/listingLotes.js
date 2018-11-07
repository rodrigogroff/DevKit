
angular.module('app.controllers').controller('ListingLotesController',
    ['$scope', '$rootScope', '$state', 'Api', 'ngSelects',
        function ($scope, $rootScope, $state, Api, ngSelects) {
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
                };

                Api.LoteDBA.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            };

            $scope.show = function (mdl) {
                $state.go('lote', { id: mdl.i_unique });
            };

            $scope.new = function () {
                $state.go('novolote');
            };

            function init() {
                $scope.campos = {
                    codigo: ''
                };                
            }

            init();

        }]);
