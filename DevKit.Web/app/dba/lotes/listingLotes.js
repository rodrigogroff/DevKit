
angular.module('app.controllers').controller('ListingLotesController',
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
                };

                Api.LoteDBA.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            };

            $scope.show = function (mdl)
            {
                $scope.modalCartoes = true;
                $scope.loading = true;

                Api.LoteDetalhesDBA.listPage({ idLote: mdl.i_unique }, function (data) {
                    $scope.listDet = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            };

            $scope.closeModalCartoes = function (mdl) {
                $scope.modalCartoes = false;              
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
