
angular.module('app.controllers').controller('EmissoraDBAListagemFaturamentoController',
    ['$scope', '$state', 'Api', 'ngSelects',
        function ($scope, $state, Api, ngSelects ) {
            $scope.loading = false;

            $scope.search = function () {
                $scope.load(0, $scope.itensporpagina);
                $scope.paginador.reiniciar();
            };

            $scope.load = function (skip, take, exportar) {
                
                $scope.loading = true;

                var opcoes = {
                    skip: skip,
                    take: take,
                    codigo: $scope.campos.codigo,
                    tipoDemonstrativo: $scope.campos.tipoDemonstrativo,
                    ano: $scope.campos.ano,
                    mes: $scope.campos.mes
                };

                if (exportar) {
                    toastr.warning('Aguarde, solicitação em andamento', 'Exportar');
                    window.location.href = "/api/FaturamentoDBA/exportar?" + $.param(opcoes);
                }
                else {
                    Api.FaturamentoDBA.listPage(opcoes, function (data) {
                        $scope.list = data.results;
                        $scope.total = data.count;
                        $scope.loading = false;
                        $scope.dtEmissao = data.dtEmissao;
                        $scope.dtVencimento = data.dtVencimento;
                        $scope.perFatIni = data.perFatIni;
                        $scope.perFatFim = data.perFatFim;
                        $scope.totalFat = data.totalFat;
                    });
                }                
            };

            $scope.exportar = function () {
                $scope.load(0, $scope.itensporpagina, true);
            };

            function init() {

                $scope.selectMeses = ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 });
                
                Api.DataServer.listPage({}, function (data) {
                    $scope.campos = {
                        tipoDemonstrativo: '2',
                        ano: data.dt.substring(6, 10),
                        mes: data.dt.substring(3, 5)
                    };                
                });
            }

            init();

        }]);
