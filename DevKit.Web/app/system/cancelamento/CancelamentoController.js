angular.module('app.controllers').controller('CancelamentoController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects', 
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects )
{
    $rootScope.exibirMenu = true;

    init();

    function init()
    {
        $scope.loading = false;        
        $scope.viewModel = {};        
    }

    $scope.limpar = function ()
    {
        $scope.viewModel = {};
    }

    $scope.conferirNSU = function ()
    {
        $scope.viewModel.error = '';

        $scope.stNSU_fail = invalidCheck($scope.viewModel.stNSU);

        if ($scope.stNSU_fail )
        {
            return;
        }

        $scope.loading = true;
        
        Api.ConfereNSU.listPage({
            nsu: $scope.viewModel.stNSU
        },
        function (data)
        {
            $scope.viewModel.cupom = data.results;
            $scope.loading = false;
        },
        function (response)
        {
            $scope.viewModel.error = response.data.message;
            $scope.loading = false;
        });
    }
    
    var invalidCheck = function (element) {
        if (element == undefined)
            return true;
        else
            if (element.length == 0)
                return true;

        return false;
    }
    
    
}]);

