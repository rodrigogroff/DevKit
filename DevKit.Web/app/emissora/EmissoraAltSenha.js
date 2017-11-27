
angular.module('app.controllers').controller('EmissoraAltSenhaController',
['$scope', '$rootScope', 'AuthService', '$state', '$stateParams', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, $stateParams, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;

    $scope.campos = {
        mat: '',
        nomeCartao: '',
        id: '',
        novaSenha: ''
    };

    $scope.buscar = function ()
    {
        $scope.loading = true;

        var opcoes = { matricula: $scope.campos.mat };

        Api.EmissoraCartao.listPage(opcoes, function (data)
        {
            $scope.campos.nomeCartao = data.results[0].associado;
            $scope.campos.id = data.results[0].id;
            $scope.loading = false;
        },
        function (response) { toastr.error(response.data.message, 'Erro'); });
    }

    $scope.confirmar = function ()
    {
        $scope.loading = true;

        var opcoes = {
            id: $scope.campos.id,
            modo: 'altSenha',
            valor: $scope.campos.novaSenha
        };
        
        Api.EmissoraCartao.update({ id: $scope.campos.id }, opcoes, function (data)
        {
            toastr.success('Cartão Atualizado!', 'Sucesso');
            $scope.loading = false;
        },
        function (response) { toastr.error(response.data.message, 'Erro'); });
    }

}]);
