
angular.module('app.controllers').controller('ListingParceiroUsuariosController',
    ['$scope', '$state', 'Api', 'AuthService', 'ngSelects',
        function ($scope, $state, Api, AuthService, ngSelects ) {
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
                    fkParceiro: $scope.campos.fkParceiro,
                };

                Api.UsuarioParceiroDBA.listPage(opcoes, function (data) {
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
                $state.go('parceirousuario', { id: mdl.id });
            };

            $scope.new = function () {
                $state.go('novoparceirousuario');
            };

            function init() {

                $scope.selectParceiro = ngSelects.obterConfiguracao(Api.Parceiro, {});

                AuthService.fillAuthData();

                $scope.authentication = AuthService.authentication;

                $scope.campos = {
                    codigo: '',
                    fkParceiro: AuthService.authentication.parceiro
                };           
            }

            init();

        }]);
