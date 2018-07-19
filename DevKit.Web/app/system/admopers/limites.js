angular.module('app.controllers').controller('AdmOpersLimitesController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects', 
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
    $scope.loading = false;
    $scope.mostraModal = false;

    $scope.campos = {

        tipoLim: 1,
        tipoOper: 1,

        selects: {
            empresa: ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 }),
        }
    };

    $scope.limpaPendenciaModal = function () {
        $scope.mostraModal = true;
    }

    $scope.cancelaModal = function () {
        $scope.mostraModal = false;
    }

    $scope.confirmar = function ()
    {
           
    }

}]);
