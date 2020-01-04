
angular.module('app.controllers').controller('ListingParceirosController',
    ['$scope',  '$state', 'Api', 
        function ($scope,  $state, Api) {
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
                    busca: $scope.campos.codigo,                    
                };

                Api.ParceiroDBA.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                },
                    function (response) {
                        $scope.loading = false;
                        toastr.error('Acesso não autorizado!', 'Erro');
                    });
            };

            $scope.show = function (mdl) {
                $state.go('parceiro', { id: mdl.id });
            };

            $scope.new = function () {
                $state.go('parceiro');
            };

            function init() {

                $scope.campos = {
                    codigo: ''
                };                
            }

            init();

        }]);
