
angular.module('app.controllers').controller('ListingLojistaTransController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;

    $scope.date = new Date();

    $scope.loading = false;

    $scope.campos = {
        idOrdem: '1',
        confirmada: true,
        cancelada: false,
        mes_inicial: $scope.date.getMonth()+ 1,
        mes_final: $scope.date.getMonth() + 1,
        selects: {
            mes: ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 }),            
            terminal: ngSelects.obterConfiguracao(Api.TerminalLoja, { tamanhoPagina: 15 }),
            ordem: ngSelects.obterConfiguracao(Api.OrdemRelLojistaTrans, { tamanhoPagina: 15 }),
        }
    };

    $scope.itensporpagina = 15;

    init();

    function init()
    {
        if (ngHistoricoFiltro.filtro)
            ngHistoricoFiltro.filtro.exibeFiltro = false;

        console.log($scope.date);

        $scope.campos.ano_inicial = $scope.date.getFullYear();
        $scope.campos.ano_final = $scope.date.getFullYear();
        
        $scope.campos.dia_inicial = $scope.date.getDay() + 1;
        $scope.campos.dia_final = $scope.date.getDay() + 1;
    }

    $scope.search = function ()
    {
        $scope.load(0, $scope.itensporpagina);
        $scope.paginador.reiniciar();
    }

    $scope.load = function (skip, take)
    {
        $scope.loading = true;

        var opcoes = {
            skip: skip,
            take: take,
            idEmpresa: $scope.campos.idEmpresa
        };

        angular.extend(opcoes, $scope.campos);

        delete opcoes.selects;

        Api.RelLojistaTrans.listPage(opcoes, function (data)
        {
            $scope.list = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        });
    }

}]);
