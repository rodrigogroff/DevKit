
angular.module('app.controllers').controller('GraficosController',
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

            $scope.campos = {
                mat: '',
                nomeCartao: '',
                id: 0,
            };

            $scope.selectEmpresa = ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 });

            $scope.buscar = function () {
                
                $scope.campos.data = null;

                $scope.dt_ini_fail = invalidCheck($scope.campos.dtInicial);
                $scope.dt_fim_fail = invalidCheck($scope.campos.dtFinal);

                if (!$scope.mat_fail && !$scope.dt_fim_fail) {
                    $scope.loading = true;

                    var opcoes = { op: '200', dtInicial: $scope.campos.dtInicial, dtFinal: $scope.campos.dtFinal, idEmpresa: $scope.campos.idEmpresa };

                    Api.AdmOper.listPage(opcoes, function (data) {

                        $scope.campos.data = data;

                        var data120_a = []; var data120_b = []; var data120_c = [];

                        for (var it = 0; it < data.list.length; it++) {
                            var item = data.list[it]
                            data120_a.push([item.x, item.y]);
                        }

                        for (var it = 0; it < data.listOld.length; it++) {

                            var item = data.listOld[it]
                            data120_b.push([item.x, item.y]);
                        }

                        data120_c.push([0, '']);

                        for (var it = 0; it < data.ticks.length; it++) {
                            var item = data.ticks[it]
                            data120_c.push([item.x, item.label]);
                        }

                        var datasets =
                            [{
                                label: "&nbsp;" + data.label_a,
                                data: data120_a
                            },
                            {

                                label: "&nbsp;" + data.label_b,
                                data: data120_b
                            }
                            ];

                        $.plot("#placeholder120", datasets, {
                            legend: {
                                show: true
                            },
                            series: {
                                lines: {
                                    fill: false
                                }
                            },
                            xaxis: {
                                tickLength: 10,
                                ticks: data120_c
                            },
                        });

                        $scope.loading = false;
                    },
                        function (response) {
                            toastr.error('Falha em processamento', 'Erro');
                            $scope.loading = false;
                        });
                }
            };

        }]);
