
angular.module('app.controllers').controller('DBAAltCotaController',
    ['$scope', '$rootScope', 'Api', 'ngSelects',
        function ($scope, $rootScope, Api, ngSelects) {

            var invalidCheck = function (element) {
                if (element == undefined)
                    return true;
                else
                    if (element.length == 0)
                        return true;

                return false;
            };

            $rootScope.exibirMenu = true;
            $scope.loading = false;

            $scope.modal = false;
            $scope.modalConf = false;
            $scope.confirmado = false;

            $scope.campos = {
                mat: '',
                nomeCartao: '',
                id: 0,
            };

            $scope.selectEmpresa = ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 });

            $scope.buscar = function () {
                $scope.campos.id = 0;

                $scope.mat_fail = invalidCheck($scope.campos.mat);

                if (!$scope.mat_fail) {
                    $scope.loading = true;

                    var opcoes = { matricula: $scope.campos.mat, idEmpresa: $scope.campos.idEmpresa };

                    Api.EmissoraCartao.listPage(opcoes, function (data) {
                        if (data.results.length > 0) {
                            $scope.campos.nomeCartao = data.results[0].associado;
                            $scope.campos.id = data.results[0].id;
                            $scope.campos.limMes = data.results[0].limM;
                            $scope.campos.limTot = data.results[0].limT;
                            $scope.campos.limCota = data.results[0].limCota;
                        }
                        else
                            toastr.error('matrícula inválida', 'Erro');

                        $scope.loading = false;
                    },
                        function (response) {
                            toastr.error(response.data.message, 'Erro');
                            $scope.loading = false;
                        });
                }
            };

            $scope.confirmar = function () {

                $scope.novaCota_fail = invalidCheck($scope.campos.novaCota);

                if (!$scope.novaCota_fail) {
                    $scope.loading = true;

                    var opcoes = {
                        id: $scope.campos.id,
                        modo: 'altCota',
                        valor: $scope.campos.novaCota
                    };

                    $scope.modal = false;

                    Api.EmissoraCartao.update({ id: $scope.campos.id }, opcoes, function (data) {
                        $scope.modal = true;
                        $scope.loading = false;

                        $scope.campos.novaCota = '';

                        var opcoes = { matricula: $scope.campos.mat };

                        Api.EmissoraCartao.listPage(opcoes, function (data) {
                            if (data.results.length > 0) {
                                $scope.campos.limCota = data.results[0].limCota;
                            }
                        },
                            function (response) {
                                $scope.loading = false;
                            });
                    },
                        function (response) {
                            toastr.error(response.data.message, 'Erro');
                            $scope.loading = false;
                        });
                }
            };

            $scope.listar = function () {
                $scope.confirmado = false;
                $scope.loading = true;

                var opcoes = { skip: 0, take: 9000, cota: true, idOrdem: 1 };

                Api.EmissoraCartao.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            };

            $scope.marcarTodos = function () {
                for (var i = 0; i < $scope.list.length; i++) {
                    $scope.list[i].selecionado = true;
                }
            };

            $scope.desmarcarTodos = function () {
                for (var i = 0; i < $scope.list.length; i++) {
                    $scope.list[i].selecionado = false;
                }
            };

            $scope.marcar = function (mdl) {
                mdl.selecionado = !mdl.selecionado;
            };

            $scope.confirmarTodos = function () {
                $scope.novaCota_fail = invalidCheck($scope.campos.novaCota);

                if (!$scope.novaCota_fail) {
                    if (!$scope.confirmado) {
                        $scope.modalConf = true;
                        return;
                    }
                }
            };

            $scope.confirmarGeral = function () {
                $scope.loading = true;

                var array = '';

                for (var i = 0; i < $scope.list.length; i++)
                    if ($scope.list[i].selecionado == true)
                        array += $scope.list[i].matricula + ',';

                var opcoes = {
                    id: $scope.campos.id,
                    modo: 'altCotaGeral',
                    valor: $scope.campos.novaCota,
                    array: array
                };

                $scope.modal = false;

                Api.EmissoraCartao.update({ id: 1 }, opcoes, function (data) {
                    $scope.modal = true;
                    $scope.loading = false;
                    $scope.list = undefined;
                    $scope.campos.novaCota = undefined;
                },
                    function (response) {
                        toastr.error(response.data.message, 'Erro');
                        $scope.loading = false;
                    });
            };

            $scope.fecharModal = function () {
                $scope.modal = false;
            };

            $scope.fecharModalConf = function () {
                $scope.modalConf = false;
                $scope.confirmado = true;
                $scope.confirmarGeral();
            };

            $scope.cancelarModalConf = function () {
                $scope.modalConf = false;
            };

        }]);
