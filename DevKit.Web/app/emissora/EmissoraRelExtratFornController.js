
angular.module('app.controllers').controller('EmissoraRelExtratFornController',
    ['$scope', '$rootScope', 'Api', 'ngSelects',
        function ($scope, $rootScope, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.tipo = $rootScope.tipo;

            $scope.pesquisa =
                {
                    tipo: 1
                };

            $scope.date = new Date();

            $scope.campos = {
                mes_inicial: undefined,
                ano_inicial: $scope.date.getFullYear(),
                selects: {
                    mes: ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 }),
                    empresa: ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 }),
                }
            };

            $scope.$watch("pesquisa.tipo", function (novo, antigo) {
                $scope.list = undefined;
            });

            var invalidCheck = function (element) {
                if (element == undefined)
                    return true;
                else
                    if (element.length == 0)
                        return true;

                return false;
            };

            $scope.search = function () {

                if ($scope.tipo == '5') {
                    $scope.emp_fail = $scope.campos.idEmpresa == undefined;
                    if ($scope.emp_fail == true)
                        return;
                }

                $scope.cod_fail = invalidCheck($scope.campos.codigo);
                $scope.mes_fail = invalidCheck($scope.campos.mes_inicial);
                $scope.ano_fail = invalidCheck($scope.campos.ano_inicial);

                if ($scope.cod_fail)
                    return;

                if (!$scope.mes_fail && !$scope.ano_fail) {
                    $scope.loading = true;

                    var opcoes = {
                        idEmpresa: $scope.campos.idEmpresa,
                        tipo: $scope.pesquisa.tipo,
                        codigo: $scope.campos.codigo,
                        mes: $scope.campos.mes_inicial,
                        ano: $scope.campos.ano_inicial
                    };

                    Api.EmissoraRelExtratoForn.listPage(opcoes, function (data) {
                        $scope.list = data.results;
                        $scope.tipoSel = $scope.pesquisa.tipo;
                        $scope.dtEmissao = data.dtEmissao;
                        $scope.referencia = data.referencia;
                        $scope.lojista = data.lojista;
                        $scope.total = data.total;
                        $scope.totalBonus = data.totalBonus;
                        $scope.totalRep = data.totalRep;
                        $scope.txAdmin = data.txAdmin;
                        $scope.loading = false;
                    },
                        function (response) {
                            $scope.loading = false;
                            $scope.list = [];
                        });
                }
            };

        }]);
