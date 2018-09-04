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

    $scope.executar = function () {
        $scope.emp_fail = $scope.campos.idEmpresa == undefined;
        $scope.vlr_fail = $scope.campos.valor == undefined;

        if ($scope.emp_fail == false && $scope.vlr_fail == false)
            $scope.mostraModal = true;
    };

    $scope.cancelaModal = function () {
        $scope.mostraModal = false;
    };

    $scope.confirmar = function () {
        $scope.mostraModal = false;

        $scope.loading = true;

        Api.AdmOper.listPage({
            op: '2',
            id_emp: $scope.campos.idEmpresa,
            tipoLim: $scope.campos.tipoLim,
            tipoOper: $scope.campos.tipoOper,
            valor: $scope.campos.valor
        },
            function (data) {
                toastr.success('Limites alterados com sucesso!', 'Sistema');
                $scope.loading = false;
            });
    };

}]);
