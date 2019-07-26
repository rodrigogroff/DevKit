
angular.module('app.controllers').controller('EmissoraRelExtratTransLojasController',
    ['$scope', '$rootScope', 'Api', 'ngSelects',
        function ($scope, $rootScope, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.tipo = $rootScope.tipo;

            $scope.campos = {
                sit: '1',
                selects: {
                    empresa: ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 }),
                }
            };

            var invalidCheck = function (element) {
                if (element == undefined)
                    return true;
                else
                    if (element.length == 0)
                        return true;

                return false;
            };

            $scope.search = function () {

                $scope.list = undefined;

                $scope.dtIni_fail = invalidCheck($scope.campos.dtInicial);
                $scope.dtFim_fail = invalidCheck($scope.campos.dtFinal);

                $scope.loading = true;

                var opcoes = {
                    idEmpresa: $scope.campos.idEmpresa,
                    codLoja: $scope.campos.codLoja,
                    dtInicial: $scope.campos.dtInicial,
                    dtFinal: $scope.campos.dtFinal,
                    sit: $scope.campos.sit,
                };

                Api.EmissoraRelExtratoTransLojas.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.dtEmissao = data.dtEmissao;
                    $scope.empresa = data.empresa;
                    $scope.periodo = data.periodo;
                    $scope.vendasConf = data.vendasConf;
                    $scope.qtdConf = data.qtdConf;
                    $scope.qtdPend = data.qtdPend;
                    $scope.qtdCanc = data.qtdCanc;
                    $scope.qtdErro = data.qtdErro;
                    $scope.loading = false;
                },
                    function (response) {
                        $scope.loading = false;
                        $scope.list = [];
                    });
            };

        }]);
