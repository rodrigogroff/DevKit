
angular.module('app.controllers').controller('EmissoraListagemLojaController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    var invalidCheck = function (element) {
        if (element == undefined)
            return true;
        else
            if (element.length == 0)
                return true;

        return false;
    }

    $rootScope.exibirMenu = true;
    $scope.loading = false;

    $scope.itensporpagina = 15;

    $scope.campos = {
        nome: '',
        codigo: '',
        cnpj: '',
        endereco: ''
    };
    
    init();

    function init()
    {
        if (ngHistoricoFiltro.filtro)
            ngHistoricoFiltro.filtro.exibeFiltro = false;
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
            nome: $scope.campos.nome,
            codigo: $scope.campos.codigo,
            cnpj: $scope.campos.cnpj,
            endereco: $scope.campos.endereco,
        };

        Api.EmissoraLoja.listPage(opcoes, function (data)
        {
            $scope.list = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        });
    }

}]);
