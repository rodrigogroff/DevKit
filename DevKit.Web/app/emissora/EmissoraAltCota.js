
angular.module('app.controllers').controller('EmissoraAltCotaController',
['$scope', '$rootScope', 'AuthService', '$state', '$stateParams', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, $stateParams, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;
    $scope.modal = false;

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

            Api.EmissoraCartao.listPage(opcoes, function (data)
            {
                if (data.results.length > 0) {
                    $scope.campos.nomeCartao = data.results[0].associado;
                    $scope.campos.id = data.results[0].id;
                    $scope.campos.limMes = data.results[0].limM;
                    $scope.campos.limTot = data.results[0].limT;
                    $scope.campos.limCota = data.results[0].limCota;
                }
                else
                    toastr.error('matrícula inválida', 'Erro');

                $scope.loading = false;
            },
            function (response) {
                toastr.error(response.data.message, 'Erro');
                $scope.loading = false;
            });
        }
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
        $scope.novaCota_fail = invalidCheck($scope.campos.novaCota);
        
        if (!$scope.limCota_fail)
        {
            $scope.loading = true;

            var opcoes = {
                id: $scope.campos.id,
                modo: 'altCota',
                valor: $scope.campos.novaCota
            };

            $scope.modal = false;

            Api.EmissoraCartao.update({ id: $scope.campos.id }, opcoes, function (data)
            {
                $scope.modal = true;                
                $scope.loading = false;  

                $scope.campos.novaCota = '';

                var opcoes = { matricula: $scope.campos.mat };

                Api.EmissoraCartao.listPage(opcoes, function (data) {
                    if (data.results.length > 0) {
                        $scope.campos.limCota = data.results[0].limCota;
                    }
                },
                function (response) {
                    $scope.loading = false;
                });
            },
            function (response) {
                toastr.error(response.data.message, 'Erro');
                $scope.loading = false;
            });
        }
    }

    $scope.confirmarTodos = function ()
    {
        $scope.novaCota_fail = invalidCheck($scope.campos.novaCota);

        if (!$scope.limCota_fail)
        {
            $scope.loading = true;

            var opcoes = {
                id: $scope.campos.id,
                modo: 'altCotaGeral',
                valor: $scope.campos.novaCota
            };

            $scope.modal = false;

            Api.EmissoraCartao.update({ id: 1 }, opcoes, function (data) {
                $scope.modal = true;
                $scope.loading = false;
            },
                function (response) {
                    toastr.error(response.data.message, 'Erro');
                    $scope.loading = false;
                });
        }
    }

    $scope.fecharModal = function () {
        $scope.modal = false;        
    }

}]);
