
angular.module('app.controllers').controller('DBADesbloqueioLoteController',
    ['$scope', '$rootScope', 'Api', 'ngSelects',
        function ($scope, $rootScope, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.modal = false;

            $scope.cartoes = [];

            $scope.campos = {
                mat: '',
                nomeCartao: '',
                id: 0,
            };

            $scope.selectEmpresa = ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 });

            $scope.buscar = function () {

                $scope.loading = true;
                $scope.cartoes = [];

                var opcoes = { idEmpresa: $scope.campos.idEmpresa };

                Api.Associado.listPage(opcoes, function (data) {
                    if (data.results.length > 0) {                        
                        for (var i = 0; i < data.results.length; i++) {
                            var mdl = data.results[i];
                            if (mdl.bloqueado === true) {
                                mdl.selecionado = true;
                                $scope.cartoes.push(mdl);
                            }
                        }
                    }
                    else
                        toastr.error('Nenhum registro encontrado', 'Erro');

                    $scope.loading = false;
                },
                    function (response) {
                        toastr.error('Nenhum registro encontrado', 'Erro');
                        $scope.loading = false;
                    });
            };

            $scope.marcar = function (mdl) {
                mdl.selecionado = !mdl.selecionado;
            }

            $scope.marcarTodos = function (mdl) {
                for (var i = 0; i < $scope.cartoes.length; i++) {
                    var mdl = $scope.cartoes[i];
                    mdl.selecionado = true;
                }
            }

            $scope.desmarcarTodos = function (mdl) {
                for (var i = 0; i < $scope.cartoes.length; i++) {
                    var mdl = $scope.cartoes[i];
                    mdl.selecionado = false;
                }
            }

            $scope.conf = function () {
                $scope.modalConfirma = true;
            }

            $scope.cancelar = function () {
                $scope.modalConfirma = false;
            }

            $scope.desbloquear = function () {
                $scope.loading = true;
                $scope.modalConfirma = false;

                var lote = '';

                for (var i = 0; i < $scope.cartoes.length; i++) {
                    var mdl = $scope.cartoes[i];
                    if (mdl.selecionado === true)
                        lote += mdl.id + ',';
                }

                var opcoes = {
                    array: lote,
                    modo: 'altDesbloqLote',
                };

                $scope.modal = false;

                Api.EmissoraCartao.update({ id: 1 }, opcoes, function (data) {
                    $scope.campos.mat = '';
                    $scope.campos.nomeCartao = '';
                    $scope.campos.situacao = '';
                    $scope.campos.id = 0;
                    $scope.modal = true;
                    $scope.loading = false;
                },
                    function (response) {
                        toastr.error('Falha no processo de desbloqueio', 'Erro');
                        $scope.loading = false;
                    });
            };

            $scope.fecharModal = function () {
                $scope.modal = false;
                $scope.buscar();
            };

        }]);
