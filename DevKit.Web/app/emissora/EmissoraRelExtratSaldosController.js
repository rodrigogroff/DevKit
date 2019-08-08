
angular.module('app.controllers').controller('EmissoraRelExtratSaldosController',
    ['$scope', '$rootScope', 'Api', 'ngSelects',
        function ($scope, $rootScope, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.tipo = $rootScope.tipo;

            $scope.pesquisa =
                {
                    tipo: 2,
                    tipoFut: 1,
                    tipoSel: 2,
                    tipoFutSel: 1,
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

            $scope.$watch("pesquisa.tipoFut", function (novo, antigo) {
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

                $scope.mat_fail = invalidCheck($scope.campos.matricula);
                $scope.mes_fail = invalidCheck($scope.campos.mes_inicial);
                $scope.ano_fail = invalidCheck($scope.campos.ano_inicial);

                if ($scope.mat_fail)
                    return;

                if ($scope.pesquisa.tipo == 1) {
                    if (!$scope.mes_fail && !$scope.ano_fail) {
                        $scope.loading = true;

                        var opcoes = {
                            tipo: $scope.pesquisa.tipo,
                            mat: $scope.campos.matricula,
                            mes: $scope.campos.mes_inicial,
                            ano: $scope.campos.ano_inicial,
                            idEmpresa: $scope.campos.idEmpresa,
                        };

                        Api.EmissoraRelExtratos.listPage(opcoes, function (data) {
                            $scope.list = data.results;
                            $scope.associado = data.associado;
                            $scope.cartao = data.cartao;
                            $scope.cpf = data.cpf;
                            $scope.total = data.total;
                            $scope.dtEmissao = data.dtEmissao;
                            $scope.pesquisa.tipoSel = 1;
                            $scope.mesSel = $scope.campos.mes_inicial;
                            $scope.anoSel = $scope.campos.ano_inicial;
                            $scope.loading = false;
                        },
                            function (response) {
                                $scope.loading = false;
                                $scope.list = [];
                            });
                    }
                }
                else if ($scope.pesquisa.tipo == 2) {
                    $scope.loading = true;

                    var opcoes = {
                        tipo: $scope.pesquisa.tipo,
                        mat: $scope.campos.matricula,
                        idEmpresa: $scope.campos.idEmpresa,
                    };

                    Api.EmissoraRelExtratos.listPage(opcoes, function (data) {
                        $scope.list = data.results;
                        $scope.associado = data.associado;
                        $scope.cartao = data.cartao;
                        $scope.cpf = data.cpf;
                        $scope.mesAtual = data.mesAtual;
                        $scope.total = data.total;
                        $scope.saldo = data.saldo;
                        $scope.saldoM = data.saldoM;
                        $scope.saldoT = data.saldoT;
                        $scope.saldoDT = data.saldoDT;
                        $scope.saldoCT = data.saldoCT;
                        $scope.saldoAcc = data.saldoAcc;
                        $scope.saldoMensAcc = data.saldoMensAcc;
                        $scope.dtEmissao = data.dtEmissao;
                        $scope.pesquisa.tipoSel = 2;
                        $scope.loading = false;
                    },
                        function (response) {
                            $scope.loading = false;
                            $scope.list = [];
                        });
                }
                else if ($scope.pesquisa.tipo == 3) {
                    if ($scope.pesquisa.tipoFut != undefined) {
                        if ($scope.pesquisa.tipoFut == 1) {
                            $scope.loading = true;

                            var opcoes = {
                                tipo: $scope.pesquisa.tipo,
                                tipoFut: $scope.pesquisa.tipoFut,
                                mat: $scope.campos.matricula,
                                idEmpresa: $scope.campos.idEmpresa,
                            };

                            Api.EmissoraRelExtratos.listPage(opcoes, function (data) {
                                $scope.list = data.results;
                                $scope.associado = data.associado;
                                $scope.cartao = data.cartao;
                                $scope.cpf = data.cpf;
                                $scope.dtEmissao = data.dtEmissao;
                                $scope.pesquisa.tipoSel = 3;
                                $scope.pesquisa.tipoFutSel = 1;
                                $scope.loading = false;
                            },
                                function (response) {
                                    $scope.loading = false;
                                    $scope.list = [];
                                });
                        }
                        else if ($scope.pesquisa.tipoFut == 2) {
                            if (!$scope.mes_fail && !$scope.ano_fail) {
                                $scope.loading = true;

                                var opcoes = {
                                    tipo: $scope.pesquisa.tipo,
                                    tipoFut: $scope.pesquisa.tipoFut,
                                    mat: $scope.campos.matricula,
                                    mes: $scope.campos.mes_inicial,
                                    ano: $scope.campos.ano_inicial,
                                    idEmpresa: $scope.campos.idEmpresa,
                                };

                                Api.EmissoraRelExtratos.listPage(opcoes, function (data) {
                                    $scope.list = data.results;
                                    $scope.total = data.total;
                                    $scope.associado = data.associado;
                                    $scope.cartao = data.cartao;
                                    $scope.cpf = data.cpf;
                                    $scope.dtEmissao = data.dtEmissao;
                                    $scope.mesSel = $scope.campos.mes_inicial;
                                    $scope.anoSel = $scope.campos.ano_inicial;
                                    $scope.pesquisa.tipoSel = 3;
                                    $scope.pesquisa.tipoFutSel = 2;
                                    $scope.loading = false;
                                },
                                    function (response) {
                                        $scope.loading = false;
                                        $scope.list = [];
                                    });
                            }
                        }
                    }
                }
            };

        }]);
