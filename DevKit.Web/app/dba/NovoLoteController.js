
angular.module('app.controllers').controller('NovoLoteController',
['$scope', '$state', '$stateParams', '$rootScope', 'Api', 'ngSelects', 
function ($scope, $state, $stateParams, $rootScope, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;

    init();

    function init()
    {
        $scope.campos = {
            codigo: ''
        };

        $scope.itensporpagina = 15;
        $scope.search();
    }

    $scope.search = function () {
        $scope.load(0, $scope.itensporpagina);
        $scope.paginador.reiniciar();
    }

    $scope.load = function (skip, take)
    {
        $scope.loading = true;

        var opcoes = {
            skip: skip,
            take: take,
            novoLote: true
        };

        Api.LotesGrafica.listPage(opcoes, function (data) {
            $scope.list = data.results;
            $scope.loading = false;
        });
    }

}]);
