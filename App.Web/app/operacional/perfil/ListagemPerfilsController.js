angular.module('app.controllers').controller('ListagemPerfilsController',
['$scope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api',
function ($scope, AuthService, $state, ngHistoricoFiltro, Api)
{
	$scope.permModel = {};

	function ObterPermissoes() { Api.Permissao.obter({ id: 101 }, function (data) { $scope.permModel = data; }, function (response) { }); }

	init();

	function init()
	{
		ObterPermissoes();

		if (ngHistoricoFiltro.filtro)
			ngHistoricoFiltro.filtro.exibeFiltro = false;
	}

	$scope.campos = { ativo: 'true' };
	$scope.itensporpagina = 15;
	
	$scope.carregar = function (skip, take)
	{
		var opcoes = { ativo: 'true', skip: skip, take: take };
		var filtro = ngHistoricoFiltro.filtro.filtroGerado;
		if (filtro)
			angular.extend(opcoes, filtro);

		Api.Perfil.listarPaginado(opcoes, function (data) {
			$scope.lista = data.results;
			$scope.total = data.count;
		});
	}

	$scope.exibir = function (mdl)
	{
		if (!$scope.permModel.visualizar) 
			toastr.error('Você não tem permissão para acessar esta página', 'Erro');
		else
			$state.go('perfil', { id: mdl.id });
	}

	$scope.adicionar = function ()
	{
		if (!$scope.permModel.novo)
			toastr.error('Você não tem permissão para acessar esta página', 'Erro');
		else
			$state.go('perfil-novo');
	}

	$scope.pesquisar = function () {
		$scope.carregar(0, $scope.itensporpagina);
		$scope.paginador.reiniciar();
	}
	
}]);
