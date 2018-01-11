
angular.module('app.controllers').controller('EmissoraRelExtratTransController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;

    $scope.pesquisa =
        {
            tipo: 1,
        };

    $scope.date = new Date();

    $scope.campos = {
        mes_inicial: $scope.date.getMonth() + 1,
        ano_inicial: $scope.date.getFullYear(),
        mes_final: undefined,
        ano_final: undefined,
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
        $scope.list = undefined;

        $scope.mat_fail = invalidCheck($scope.campos.mat);
        $scope.mes_fail = invalidCheck($scope.campos.mes_inicial);
        $scope.ano_fail = invalidCheck($scope.campos.ano_inicial);

        var tAnoFim = invalidCheck($scope.campos.ano_final);

        if (!tAnoFim)
        {
            $scope.mesf_fail = invalidCheck($scope.campos.mes_final);
            $scope.anof_fail = tAnoFim;
        }

        if ($scope.mat_fail)
            return;

        if (!$scope.mes_fail && !$scope.ano_fail)
        {
            $scope.loading = true;

            var opcoes = {
                mat: $scope.campos.mat,
                mes: $scope.campos.mes_inicial,
                ano: $scope.campos.ano_inicial,
                mesf: $scope.campos.mes_final,
                anof: $scope.campos.ano_final
            };

            Api.EmissoraRelExtratoTrans.listPage(opcoes, function (data)
            {
                $scope.list = data.results;
                $scope.dtEmissao = data.dtEmissao;
                $scope.cartao = data.cartao;
                $scope.loading = false;
            },
            function (response) {
                $scope.loading = false;
                $scope.list = [];
            });
        }  
    }

}]);
