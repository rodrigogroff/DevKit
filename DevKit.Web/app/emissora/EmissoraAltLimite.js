
angular.module('app.controllers').controller('EmissoraAltLimiteController',
['$scope', '$rootScope', 'AuthService', '$state', '$stateParams', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, $stateParams, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;

    $scope.campos = {
        mat: '',
        nomeCartao: '',
        id: 0,
    };

    $scope.buscar = function ()
    {
        $scope.campos.id = 0;

        if ($scope.campos.mat != '')
        {
            $scope.loading = true;

            var opcoes = { matricula: $scope.campos.mat };

            Api.EmissoraCartao.listPage(opcoes, function (data) {
                $scope.campos.nomeCartao = data.results[0].associado;
                $scope.campos.id = data.results[0].id;
                $scope.campos.limMes = data.results[0].limM;
                $scope.campos.limTot = data.results[0].limT;
                $scope.loading = false;
            },
            function (response) {
                toastr.error(response.data.message, 'Erro');
                $scope.loading = false;
            });
        }
    }

    $scope.confirmar = function ()
    {
        $scope.limMes_fail = invalidCheck($scope.viewModel.limMes);
        $scope.limTot_fail = invalidCheck($scope.viewModel.limTot);

        if (!$scope.limMes_fail && $scope.limTot_fail)
        {
            $scope.loading = true;

            var opcoes = {
                id: $scope.campos.id,
                modo: 'altLim',
                valor: $scope.campos.limMes + "|" + $scope.campos.limTot
            };

            Api.EmissoraCartao.update({ id: $scope.campos.id }, opcoes, function (data) {
                toastr.success('Limites atualizados!', 'Sucesso');
                $scope.loading = false;
            },
                function (response) {
                    toastr.error(response.data.message, 'Erro');
                    $scope.loading = false;
                });
        }
    }

}]);
