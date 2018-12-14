﻿
angular.module('app.controllers').controller('ListingRelAssociadosController',
    ['$scope', '$rootScope', 'ngHistoricoFiltro', 'Api', 'ngSelects',
        function ($scope, $rootScope, ngHistoricoFiltro, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;

            $scope.campos = {
                bloqueado: 'false',
                expedicao: 'T',
                matricula: '',
                selects: {
                    empresa: ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 }),
                }
            };

            $scope.itensporpagina = 15;

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
                    skip: skip,
                    take: take,
                    matricula: $scope.campos.matricula,
                    idEmpresa: $scope.campos.idEmpresa,
                    bloqueado: $scope.campos.bloqueado,
                    expedicao: $scope.campos.expedicao
                };

                Api.RelAssociados.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            };

        }]);
