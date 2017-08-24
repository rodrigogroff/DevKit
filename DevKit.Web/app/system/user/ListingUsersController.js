angular.module('app.controllers').controller('ListingUsersController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects', 
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
    $scope.loading = false;
    
    $scope.viewModel = {};
    
    var invalidCheck = function (element) {
        if (element == undefined)
            return true;
        else
            if (element.length == 0)
                return true;

        return false;
    }

    $scope.conferir = function ()
    {
        $scope.stEmpresa_fail = invalidCheck($scope.viewModel.stEmpresa);
        $scope.stMatricula_fail = invalidCheck($scope.viewModel.stMatricula);
        $scope.stAcesso_fail = invalidCheck($scope.viewModel.stAcesso);
        $scope.stVencimento_fail = invalidCheck($scope.viewModel.stVencimento);

        if ($scope.stEmpresa_fail ||
            $scope.stMatricula_fail ||
            $scope.stAcesso_fail ||
            $scope.stVencimento_fail)
        {
            return;
        }

        $scope.loading = true;

        Api.Associado.listPage({
            empresa: $scope.viewModel.stEmpresa,
            matricula: $scope.viewModel.stMatricula,
            acesso: $scope.viewModel.stAcesso,
            vencimento: $scope.viewModel.stVencimento,
        },
        function (data)
        {
            $scope.viewModel.data = data.results[0];
            $scope.loading = false;
        },
        function (response)
        {
            $scope.viewModel.data = {};
            $scope.viewModel.data.error = "Cartão Inválido";
            $scope.loading = false;
        });
    }

}]);
