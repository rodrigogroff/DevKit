
angular.module('app.controllers').controller('ListingEmpresasController',
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
                    cnpj: $scope.campos.cnpj,
                    cidade: $scope.campos.cidade,
                    estado: $scope.campos.estado
                };

                Api.EmpresaDBA.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            };

            $scope.show = function (mdl) {
                $state.go('empresa', { id: mdl.i_unique });
            };

            $scope.new = function () {
                $state.go('novaempresa');
            };

            function init() {
                $scope.campos = {
                    codigo: ''
                };                
            }

            init();

        }]);
