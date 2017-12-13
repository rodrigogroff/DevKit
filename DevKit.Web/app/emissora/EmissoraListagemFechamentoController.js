
angular.module('app.controllers').controller('EmissoraListagemFechamentoController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;
    $scope.tipoFech = 1;
    $scope.date = new Date();

    $scope.campos = {
        mes_inicial: $scope.date.getMonth() + 1,
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
        if (invalidCheck($scope.campos.mes_inicial) ||
            invalidCheck($scope.campos.ano_inicial) )
        {
            toastr.error('Informe os filtros corretamente', 'Erro');
            return;
        }
        
        $scope.loading = true;

        var opcoes = {
            tipoFech: $scope.tipoFech,
            mes: $scope.campos.mes_inicial,
            ano: $scope.campos.ano_inicial,
        };

        Api.EmissoraFechamento.listPage(opcoes, function (data)
        {
            $scope.list = data.results;
            $scope.totalFechamento = data.totalFechamento;
            $scope.loading = false;
        });
    }

}]);
