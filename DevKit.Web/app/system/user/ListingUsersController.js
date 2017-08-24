angular.module('app.controllers').controller('ListingUsersController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects', 
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
	$scope.loading = true;

	init();

    function init()
    {

    }

}]);
