angular.module('app.controllers').controller('AdmOpersController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects', 
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
    $scope.loading = false;

    $scope.limpaPendencia = function () {

        $scope.loading = true;

        Api.AdmOper.listPage({ op: '1' },
            function (data)
            {
                toastr.success( data.resp + ' pendências resolvidas', 'Sucesso');
                $scope.loading = false;
            },
            function (response) {
                $scope.viewModel.data = {};
                $scope.viewModel.data.error = "Cartão Inválido";
                $scope.loading = false;
            });

    }

}]);
