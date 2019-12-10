
angular.module('app.controllers').controller('EmissoraRelExtratTransController',
    ['$scope', '$rootScope', 'Api', 'ngSelects',
        function ($scope, $rootScope, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.tipo = $rootScope.tipo;

            function init()
            {
                $scope.campos = {
                    sit: '1',
                    tipo: '3',
                    selects: {
                        empresa: ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 })
                    }
                };

                Api.DataServer.listPage({}, function (data) {
                        $scope.campos.dtInicial = data.dt;
                        $scope.campos.dtFinal = data.dt;
                });
            }

            init();

            var invalidCheck = function (element) {
                if (element == undefined)
                    return true;
                else
                    if (element.length == 0)
                        return true;

                return false;
            };

            $scope.search = function () {

                $scope.dt_ini_fail = $scope.campos.dtInicial == undefined;
                $scope.dt_fim_fail = $scope.campos.dtFinal == undefined;

                if ($scope.dt_ini_fail == true || $scope.dt_fim_fail == true)
                    return;
            
                $scope.list = undefined;

                $scope.dtIni_fail = invalidCheck($scope.campos.dtInicial);
                $scope.dtFim_fail = invalidCheck($scope.campos.dtFinal);

                $scope.loading = true;

                var opcoes = {
                    idEmpresa: $scope.campos.idEmpresa,
                    mat: $scope.campos.mat,
                    dtInicial: $scope.campos.dtInicial,
                    dtFinal: $scope.campos.dtFinal,
                    sit: $scope.campos.sit,
                    nsu: $scope.campos.nsu,
                    terminal: $scope.campos.terminal,
                    codLoja: $scope.campos.codLoja,
                    cnpjLoja: $scope.campos.cnpjLoja,
                    valorVenda: $scope.campos.valorVenda,
                    parcelas: $scope.campos.parcelas,
                    operacao: $scope.campos.operacao,
                    tipo: $scope.campos.tipo,
                };

                Api.EmissoraRelExtratoTrans.listPage(opcoes, function (data) {
                    if (data.fail == true) {
                        $scope.list = [];
                    }
                    else {
                        $scope.list = data.results;
                        $scope.dtEmissao = data.dtEmissao;
                        $scope.cartao = data.cartao;
                        $scope.periodo = data.periodo;
                        $scope.total = data.total;

                        $scope.listaFiltrada = [];

                        console.log($scope.campos.tipo);
                        console.log(' total ' + $scope.list[0].itens.length);

                        for (var i = 0; i < $scope.list[0].itens.length; i++) {

                            var item = $scope.list[0].itens[i];

                            if ($scope.campos.sit == '1') // todas
                                $scope.listaFiltrada.push(item);
                            else {
                                if ($scope.campos.sit == '2') { // confirmadas
                                    if (item.idstatus == '1')
                                        $scope.listaFiltrada.push(item);
                                }
                                else if ($scope.campos.sit == '3') { // canceladas
                                    if (item.idstatus == '5')
                                        $scope.listaFiltrada.push(item);
                                }
                                else if ($scope.campos.sit == '4') { // negadas
                                    if (item.idstatus == '2' || item.idstatus == '3')
                                        $scope.listaFiltrada.push(item);
                                }
                                else if ($scope.campos.sit == '5') { // pendentes
                                    if (item.idstatus == '0')
                                        $scope.listaFiltrada.push(item);
                                }
                            }
                        }                        
                    }

                    $scope.loading = false;
                },
                    function (response) {
                        $scope.loading = false;
                        toastr.error(response.data.message, 'Erro');
                        $scope.list = [];
                    });
            };

        }]);
