
angular.module('app.controllers').controller('ListingLojistaPendsController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;

    $scope.search = function ()
    {
        $scope.load(0, $scope.itensporpagina);        
    }

    $scope.load = function (skip, take)
    {
        $scope.loading = true;

        var opcoes = {
            skip: 0,
            take: 1000,
            pends: true
        };

        Api.RelLojistaTrans.listPage(opcoes, function (data)
        {
            $scope.list = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        });
    }

    $scope.cancelar = function () {
        $scope.mostraModal = true;
    }

    $scope.cancelaModal = function () {
        $scope.mostraModal = false;
    }

    $scope.cancelaModal = function () {
        $scope.mostraModal = false;
    }

    $scope.confirmar = function () {

        $scope.loading = true;

        var opcoes = {
            skip: 0,
            take: 1000,
            pends: true,
            pendsConf: true
        };

        Api.RelLojistaTrans.listPage(opcoes, function (data)
        {
            toastr.success('Todas as transações foram canceladas', 'Sistema');
            $scope.loading = false;
        });        
    }    

    init();

    function init() {
        $scope.search();
    }

}]);
