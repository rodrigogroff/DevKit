
angular.module('app.controllers').controller('EmissoraEmitCartVirtualController',
    ['$scope', '$rootScope', 'Api',
        function ($scope, $rootScope, Api) {

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

            $scope.buscar = function () {
                $scope.campos.id = 0;

                $scope.mat_fail = invalidCheck($scope.campos.mat);

                if (!$scope.mat_fail) {
                    $scope.loading = true;

                    var opcoes = { matricula: $scope.campos.mat };

                    Api.EmissoraCartao.listPage(opcoes, function (data) {
                        if (data.results.length > 0) {
                            $scope.campos.nomeCartao = data.results[0].associado;
                            $scope.campos.id = data.results[0].id;
                            $scope.campos.situacao = data.results[0].situacao;

                            Api.EmissoraCartao.get({ id: data.results[0].id }, function (data_2) {
                                $scope.campos.digitos = data_2.digitos;
                                $scope.loading = false;
                            },
                                function (response) {
                                    if (response.status === 404) { toastr.error('Invalid ID', 'Erro'); }
                                    $scope.list();
                                });
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

            $scope.imprimir = function () {
            }

        }]);
