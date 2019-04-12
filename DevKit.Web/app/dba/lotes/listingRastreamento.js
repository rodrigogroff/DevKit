
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
                    nome: $scope.campos.nome,
                    mat: $scope.campos.mat,
                    dtInicial: $scope.campos.dtInicial,
                    dtFinal: $scope.campos.dtFinal,                    
                    sedex: $scope.campos.sedex,         
                    dtEnvio: $scope.campos.dtEnvio,         
                    idEmpresa: $scope.campos.idEmpresa,
                };

                Api.ListagemRastreamento.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            };

            $scope.abreModal = function (mdl) {
                $scope.pedidoCartao = mdl;
                $scope.modalEdit = true;
            };

            $scope.closeModal = function () {
                $scope.modalEdit = false;
            };

            function init() {

                $scope.selectEmpresa = ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 });

                $scope.modalEdit = false;
                $scope.pedidoCartao = null;

                $scope.campos = {
                    codigo: ''
                };                
            }

            init();

        }]);
