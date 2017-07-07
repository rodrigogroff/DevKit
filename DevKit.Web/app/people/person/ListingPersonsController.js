angular.module('app.controllers').controller('ListingPersonsController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects', 
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects )
{
	$rootScope.exibirMenu = true;

	$scope.loading = false;

	$scope.campos = {
		ativo: 'true',		
	};
	$scope.itensporpagina = 15;

	$scope.permModel = {};	
	$scope.permID = 102;

    function CheckPermissions()
    {
        // rever permissoes
        Api.Permission.get({ id: $scope.permID }, function (data)
        {
			$scope.permModel = data;

            if (!$scope.permModel.listagem)
            {
				toastr.error('Access denied!', 'Permission');
				$state.go('home');
			}
		},
		function (response) { });
	}

	init();

	function init()
	{
		CheckPermissions();

		if (ngHistoricoFiltro.filtro)
			ngHistoricoFiltro.filtro.exibeFiltro = false;		
	}

	$scope.search = function ()
	{
		$scope.load(0, $scope.itensporpagina);
		$scope.paginador.reiniciar();
	}

	$scope.load = function (skip, take)
	{
		$scope.loading = true;

        var opcoes = { active: 'true', skip: skip, take: take };

		var filter = ngHistoricoFiltro.filtro.filtroGerado;

		if (filter)
			angular.extend(opcoes, filter);

		delete opcoes.selects;

		Api.Person.listPage(opcoes, function (data)
		{
			$scope.list = data.results;
			$scope.total = data.count;
			$scope.loading = false;
		});
	}

	$scope.show = function (mdl)
	{
		if (!$scope.permModel.visualizar) 
			toastr.error('Access denied!', 'Permission');
		else
			$state.go('person', { id: mdl.id });
	}

	$scope.new = function ()
	{
		if (!$scope.permModel.novo)
			toastr.error('Access denied!', 'Permission');
		else
			$state.go('person-new');
	}

}]);
