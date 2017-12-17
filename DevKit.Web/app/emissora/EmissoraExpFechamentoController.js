
angular.module('app.controllers').controller('EmissoraExpFechamentoController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;
    
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
        AuthService.fillAuthData();

        $scope.authentication = AuthService.authentication;

        if (invalidCheck($scope.campos.mes_inicial) ||
            invalidCheck($scope.campos.ano_inicial) )
        {
            toastr.error('Informe os filtros corretamente', 'Erro');
            return;
        }
        
        window.location.href = "/api/EmissoraFechamentoExp/exportar?" + $.param({
            emp: $scope.authentication.m1,
            mes: $scope.campos.mes_inicial,
            ano: $scope.campos.ano_inicial,
        });
    }

}]);
