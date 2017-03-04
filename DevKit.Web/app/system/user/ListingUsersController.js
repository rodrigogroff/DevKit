angular.module('app.controllers').controller('ListingUsersController',
['$scope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
	$scope.permModel = {};
	$scope.loading = false;

	function CheckPermissions() { Api.Permission.obter({ id: 102 }, function (data) { $scope.permModel = data; }, function (response) { }); }

	init();

	function init() {
		CheckPermissions();

		if (ngHistoricoFiltro.filtro)
			ngHistoricoFiltro.filtro.exibeFiltro = false;		
	}

	$scope.fields = {
		active: 'true',
		selects: {
			perfil: ngSelects.obterConfiguracao(Api.Perfil, { tamanhoPagina: 15, campoNome: 'stName' }),
		}
	};
	
	$scope.itensporpagina = 15;
	
	$scope.load = function (skip, take)
	{
		$scope.loading = true;

		var options = { active: 'true', skip: skip, take: take };
		var filter = ngHistoricoFiltro.filtro.filtroGerado;
		if (filter)
			angular.extend(options, filter);

		Api.User.listarPaginado(options, function (data) {
			$scope.list = data.results;
			$scope.total = data.count;
			$scope.loading = false;
		});
	}

	$scope.show = function (mdl)
	{
		if (!$scope.permModel.visualizar) 
			toastr.error('Você não tem permissão para acessar esta página', 'Erro');
		else
			$state.go('usuario', { id: mdl.id });
	}

	$scope.add = function ()
	{
		if (!$scope.permModel.novo)
			toastr.error('Você não tem permissão para acessar esta página', 'Erro');
		else
			$state.go('usuario-novo');
	}

	$scope.search = function ()
	{
		$scope.load(0, $scope.itensporpagina);
		$scope.paginador.reiniciar();
	}
}]);