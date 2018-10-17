
angular.module('app.controllers').controller('EmissoraRelRepFinancController',
    ['$scope', '$rootScope', 'Api', 'ngSelects',
        function ($scope, $rootScope, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.tipo = $rootScope.tipo;

            $scope.pesquisa =
                {
                    tipo: 1,
                    tipoSel: 1,
                    fornecedor: false,
                };

            $scope.date = new Date();

            $scope.campos = {
                mes_inicial: undefined,
                ano_inicial: $scope.date.getFullYear(),
                lojista: '',
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
                $scope.list = [];
                $scope.pesquisa.tipoSel = 0;

                if ($scope.tipo == '5') {
                    $scope.emp_fail = $scope.campos.idEmpresa == undefined;
                    if ($scope.emp_fail == true)
                        return;
                }

                if ($scope.pesquisa.tipo == 1) {
                    $scope.loading = true;

                    var opcoes = {
                        tipo: $scope.pesquisa.tipo,
                        idEmpresa: $scope.campos.idEmpresa,
                    };

                    Api.EmissoraRelRepFinanc.listPage(opcoes, function (data) {
                        $scope.list = data.results;
                        $scope.dtEmissao = data.dtEmissao;
                        $scope.empresa = data.empresa;
                        $scope.pesquisa.tipoSel = 1;
                        $scope.total = data.total;
                        $scope.totalBonus = data.totalBonus;
                        $scope.totalRep = data.totalRep;
                        $scope.loading = false;
                    },
                        function (response) {
                            $scope.loading = false;
                            $scope.list = [];
                        });
                }
                else {
                    $scope.mes_fail = invalidCheck($scope.campos.mes_inicial);
                    $scope.ano_fail = invalidCheck($scope.campos.ano_inicial);

                    $scope.pesquisa.fornecedor = invalidCheck($scope.campos.lojista);

                    if ($scope.mes_fail || $scope.ano_fail)
                        return;

                    $scope.loading = true;

                    var opcoes = {
                        tipo: $scope.pesquisa.tipo,
                        mes: $scope.campos.mes_inicial,
                        ano: $scope.campos.ano_inicial,
                        lojista: $scope.campos.lojista,
                        idEmpresa: $scope.campos.idEmpresa,
                    };

                    Api.EmissoraRelRepFinanc.listPage(opcoes, function (data) {
                        $scope.list = data.results;
                        $scope.empresa = data.empresa;
                        $scope.lojista = data.lojista;
                        $scope.dtEmissao = data.dtEmissao;
                        $scope.pesquisa.tipoSel = 2;
                        $scope.total = data.total;
                        $scope.totalBonus = data.totalBonus;
                        $scope.totalRep = data.totalRep;
                        $scope.loading = false;
                    },
                        function (response) {
                            $scope.loading = false;
                            $scope.list = [];
                        });
                }
            };

        }]);
