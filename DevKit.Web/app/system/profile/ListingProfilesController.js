angular.module('app.controllers').controller('ListingProfilesController',
['$scope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api',
function ($scope, AuthService, $state, ngHistoricoFiltro, Api)
{
	$scope.permModel = {};
	$scope.loading = false;
	$scope.permID = 101;

	function CheckPermissions() { Api.Permission.get({ id: $scope.permID }, function (data) { $scope.permModel = data; }, function (response) { }); }

	init();

	function init()
	{
		CheckPermissions();

		if (ngHistoricoFiltro.filtro)
			ngHistoricoFiltro.filtro.exibeFiltro = false;
	}

	$scope.campos = { ativo: 'true' };
	$scope.itensporpagina = 15;
	
	$scope.load = function (skip, take)
	{
		$scope.loading = true;

		var opcoes = { ativo: 'true', skip: skip, take: take };
		var filtro = ngHistoricoFiltro.filtro.filtroGerado;
		if (filtro)
			angular.extend(opcoes, filtro);

		Api.Profile.listPage(opcoes, function (data) {
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
			$state.go('profile', { id: mdl.id });
	}

	$scope.new = function ()
	{
		if (!$scope.permModel.novo)
			toastr.error('Access denied!', 'Permission');
		else
			$state.go('profile-new');
	}

	$scope.search = function () {
		$scope.load(0, $scope.itensporpagina);
		$scope.paginador.reiniciar();
	}
	
}]);
