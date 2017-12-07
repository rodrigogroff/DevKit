
angular.module('app.controllers').controller('EmissoraListagemCartaoController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;

    $scope.itensporpagina = 15;

    $scope.campos = {
        idSit: '',
        idExp: '',
        idOrdem: 1,
    };
        
    $scope.situacoes = ngSelects.obterConfiguracao(Api.SituacoesCombo, { tamanhoPagina: 15 });
    $scope.expedicoes = ngSelects.obterConfiguracao(Api.ExpedicoesCombo, { tamanhoPagina: 15 });
    $scope.ordem = ngSelects.obterConfiguracao(Api.OrdemEmissorManutCartoes, { tamanhoPagina: 15 });
    
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
        $scope.cartaoSelecionado = mdl;
        $scope.modal = true;
    }

    $scope.editar = function () {
        $state.go('empManutCartao', { id: $scope.cartaoSelecionado.id });
    }

    $scope.fecharModal = function () {
        $scope.modal = false;
    }

    var invalidCheck = function (element) {
        if (element == undefined)
            return true;
        else
            if (element.length == 0)
                return true;

        return false;
    }

    $scope.confirmar = function ()
    {
        $scope.limMes_fail = invalidCheck($scope.cartaoSelecionado.limM);
        $scope.limTot_fail = invalidCheck($scope.cartaoSelecionado.limT);

        if (!$scope.limMes_fail &&
            !$scope.limTot_fail)
        {
            $scope.loading = true;

            var opcoes = {
                id: $scope.cartaoSelecionado.id,
                modo: 'altLim',
                valor: $scope.cartaoSelecionado.limM + "|" + $scope.cartaoSelecionado.limT
            };

            $scope.modal = false;

            Api.EmissoraCartao.update({ id: $scope.cartaoSelecionado.id }, opcoes, function (data)
            {
                toastr.success('Limite alterado com sucesso', 'Sucesso');
            },
            function (response)
            {
                toastr.error(response.data.message, 'Erro');
                $scope.loading = false;
            });
        }
    }

}]);
