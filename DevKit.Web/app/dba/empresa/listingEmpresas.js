
angular.module('app.controllers').controller('ListingEmpresasController',
    ['$scope',  '$state', 'Api', 'ngSelects', 
        function ($scope,  $state, Api, ngSelects) {
            $scope.loading = false;

            $scope.search = function () {
                $scope.load(0, 999);
                $scope.paginador.reiniciar();
            };

            $scope.export = function () {
                window.location.href = "/api/EmpresaDBA/exportar";
            };

            $scope.load = function (skip, take) {
                $scope.loading = true;

                var opcoes = {
                    skip: skip,
                    take: take,
                    busca: $scope.campos.codigo,
                    cnpj: $scope.campos.cnpj,
                    cidade: $scope.campos.cidade,
                    estado: $scope.campos.estado,
                    parceiro: $scope.campos.fkParceiro,
                };
              
                Api.EmpresaDBA.listPage(opcoes, function (data) {
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
                $state.go('empresa', { id: mdl.i_unique });
            };

            $scope.new = function () {
                $state.go('novaempresa');
            };

            function init() {

                $scope.selectParceiro = ngSelects.obterConfiguracao(Api.Parceiro, {});

                $scope.campos = {
                    codigo: ''
                };                
            }

            init();

        }]);
