angular.module('app.controllers').controller('ListingUsersController',
['$scope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
	$scope.permModel = {};
	$scope.loading = false;
	$scope.permID = 102;

	function CheckPermissions() { Api.Permission.get({ id: $scope.permID }, function (data) { $scope.permModel = data; }, function (response) { }); }

	init();

	function init()
	{
		CheckPermissions();

		if (ngHistoricoFiltro.filtro)
			ngHistoricoFiltro.filtro.exibeFiltro = false;		
	}

	$scope.campos = {
		ativo: 'true',
		selects: {
			perfil: ngSelects.obterConfiguracao(Api.Profile, { tamanhoPagina: 15, campoNome: 'stName' }),
		}
	};
	
	$scope.itensporpagina = 15;

	$scope.search = function ()
	{
		$scope.load(0, $scope.itensporpagina);
		$scope.paginador.reiniciar();
	}

	$scope.load = function (skip, take)
	{
		$scope.loading = true;

		var options = { active: 'true', skip: skip, take: take };
		var filter = ngHistoricoFiltro.filtro.filtroGerado;
		if (filter)
			angular.extend(options, filter);

		Api.User.listPage(options, function (data) {
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
			$state.go('user', { id: mdl.id });
	}

	$scope.new = function ()
	{
		if (!$scope.permModel.novo)
			toastr.error('Access denied!', 'Permission');
		else
			$state.go('user-new');
	}

}]);
