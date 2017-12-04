
angular.module('app.controllers').controller('EmissoraListagemCartaoController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;

    $scope.loading = false;

    $scope.campos = {
        idSit: '',
        idExp: '',
        idOrdem: 1,
    };
        
    $scope.situacoes = ngSelects.obterConfiguracao(Api.SituacoesCombo, { tamanhoPagina: 15 });
    $scope.expedicoes = ngSelects.obterConfiguracao(Api.ExpedicoesCombo, { tamanhoPagina: 15 });

    $scope.ordem = ngSelects.obterConfiguracao(Api.OrdemEmissorManutCartoes, { tamanhoPagina: 15 });

    $scope.itensporpagina = 15;

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
            matricula: $scope.campos.matricula,
            cpf: $scope.campos.cpf,
            idSit: $scope.campos.idSit,
            idExp: $scope.campos.idExp,
            idOrdem: $scope.campos.idOrdem,
        };

        Api.EmissoraCartao.listPage(opcoes, function (data)
        {
            $scope.list = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        });
    }

    $scope.show = function (mdl) {
        $state.go('empManutCartao', { id: mdl.id });
    }

}]);
