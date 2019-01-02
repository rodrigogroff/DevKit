
angular.module('app.controllers').controller('DBAAltSegViaController',
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
                            $scope.campos.situacao = data.results[0].situacao;
                            $scope.campos.via = data.results[0].via;
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
                $scope.loading = true;

                var opcoes = {
                    id: $scope.campos.id,
                    modo: 'altSegVia',
                };

                $scope.modal = false;

                Api.EmissoraCartao.update({ id: $scope.campos.id }, opcoes, function (data) {
                    $scope.campos.mat = '';
                    $scope.campos.nomeCartao = '';
                    $scope.campos.situacao = '';
                    $scope.campos.id = 0;
                    $scope.campos.via = 0;
                    $scope.modal = true;
                    $scope.loading = false;
                },
                    function (response) {
                        toastr.error(response.data.message, 'Erro');
                        $scope.loading = false;
                    });
            };

            $scope.fecharModal = function () {
                $scope.modal = false;
            };

        }]);
