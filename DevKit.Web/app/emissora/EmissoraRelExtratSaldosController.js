
angular.module('app.controllers').controller('EmissoraRelExtratSaldosController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;

    $scope.tipo = 1;    
    $scope.date = new Date();

    $scope.campos = {
        mes_inicial: undefined,
        ano_inicial: $scope.date.getFullYear(),
        selects: {
            mes: ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 }),
        }
    };

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
        
        if ($scope.tipo == 1)
        {
            $scope.mes_fail = invalidCheck($scope.campos.mes_inicial);
            $scope.ano_fail = invalidCheck($scope.campos.ano_inicial);

            if (!$scope.mat_fail && !$scope.mes_fail && !$scope.ano_fail)
            {
                $scope.loading = true;

                var opcoes = {
                    tipo: $scope.tipo,
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
                });
            }
        }        
    }

}]);
