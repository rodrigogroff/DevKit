
angular.module('app.controllers').controller('EmissoraRelExtratSaldosController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;

    $scope.pesquisa =
        {
            tipo: 1,    
            tipoFut: 1,    
            tipoSel: 1,
            tipoFutSel: 1,    
        };

    $scope.date = new Date();

    $scope.campos = {
        mes_inicial: undefined,
        ano_inicial: $scope.date.getFullYear(),
        selects: {
            mes: ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 }),
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
    }
    
    $scope.search = function ()
    {
        $scope.mat_fail = invalidCheck($scope.campos.matricula);
        $scope.mes_fail = invalidCheck($scope.campos.mes_inicial);
        $scope.ano_fail = invalidCheck($scope.campos.ano_inicial);

        if ($scope.mat_fail) 
            return;

        console.log('pesquisa');
        
        if ($scope.pesquisa.tipo == 1)
        {
            if (!$scope.mes_fail && !$scope.ano_fail)
            {
                $scope.loading = true;

                var opcoes = {
                    tipo: $scope.pesquisa.tipo,
                    mat: $scope.campos.matricula,
                    mes: $scope.campos.mes_inicial,
                    ano: $scope.campos.ano_inicial
                };

                Api.EmissoraRelExtratos.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.associado = data.associado;
                    $scope.cartao = data.cartao;
                    $scope.cpf = data.cpf;
                    $scope.total = data.total;
                    $scope.dtEmissao = data.dtEmissao;
                    $scope.loading = false;
                    $scope.pesquisa.tipoSel = 1;
                });
            }
        }        
        else if ($scope.pesquisa.tipo == 3)
        {
            console.log('futuro');

            if ($scope.pesquisa.tipoFut != undefined)
            {
                console.log('valido');
                console.log($scope.pesquisa.tipoFut);

                if ($scope.pesquisa.tipoFut == 1)
                {
                    console.log('sintetico');

                    // ------------------
                    // sintético
                    // ------------------

                    $scope.loading = true;

                    var opcoes = {
                        tipo: $scope.pesquisa.tipo,
                        tipoFut: $scope.pesquisa.tipoFut,
                        mat: $scope.campos.matricula
                    };

                    Api.EmissoraRelExtratos.listPage(opcoes, function (data) {
                        $scope.list = data.results;
                        $scope.associado = data.associado;
                        $scope.cartao = data.cartao;
                        $scope.cpf = data.cpf;
                        $scope.dtEmissao = data.dtEmissao;
                        $scope.loading = false;
                        $scope.pesquisa.tipoSel = 3;
                        $scope.pesquisa.tipoFutSel = 1;
                    });
                }
                else if ($scope.pesquisa.tipoFut == 2)
                {
                    console.log('detalhado');

                    // --------------
                    // detalhado
                    // --------------

                    if (!$scope.mes_fail && !$scope.ano_fail)
                    {
                        $scope.loading = true;

                        var opcoes = {
                            tipo: $scope.pesquisa.tipo,
                            tipoFut: $scope.pesquisa.tipoFut,
                            mat: $scope.campos.matricula,
                            mes: $scope.campos.mes_inicial,
                            ano: $scope.campos.ano_inicial,
                        };

                        Api.EmissoraRelExtratos.listPage(opcoes, function (data) {
                            $scope.list = data.results;
                            $scope.total = data.total;
                            $scope.associado = data.associado;
                            $scope.cartao = data.cartao;
                            $scope.cpf = data.cpf;
                            $scope.dtEmissao = data.dtEmissao;
                            $scope.loading = false;
                            $scope.pesquisa.tipoSel = 3;
                            $scope.pesquisa.tipoFutSel = 2;    
                        });
                    }
                }
                            
            }
        }   
    }

}]);
