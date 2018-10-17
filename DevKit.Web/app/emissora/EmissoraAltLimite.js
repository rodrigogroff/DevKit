
angular.module('app.controllers').controller('EmissoraAltLimiteController',
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
                            $scope.campos.limMes = data.results[0].limM;
                            $scope.campos.limTot = data.results[0].limT;
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

                $scope.limMes_fail = invalidCheck($scope.campos.limMes);
                $scope.limTot_fail = invalidCheck($scope.campos.limTot);

                if (!$scope.limMes_fail &&
                    !$scope.limTot_fail) {
                    $scope.loading = true;

                    var opcoes = {
                        id: $scope.campos.id,
                        modo: 'altLim',
                        valor: $scope.campos.limMes + "|" + $scope.campos.limTot
                    };

                    $scope.modal = false;

                    Api.EmissoraCartao.update({ id: $scope.campos.id }, opcoes, function (data) {
                        $scope.modal = true;
                        $scope.loading = false;
                    },
                        function (response) {
                            toastr.error(response.data.message, 'Erro');
                            $scope.loading = false;
                        });
                }
            };

            $scope.fecharModal = function () {
                $scope.modal = false;
            };

        }]);
