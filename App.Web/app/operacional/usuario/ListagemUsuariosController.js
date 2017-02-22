angular.module('app.controllers').controller('ListagemUsuariosController',
['$scope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
	$scope.permModel = {};

	function ObterPermissoes() { Api.Permissao.obter({ id: 102 }, function (data) { $scope.permModel = data; }, function (response) { }); }

	init();

	function init() {
		ObterPermissoes();

		if (ngHistoricoFiltro.filtro)
			ngHistoricoFiltro.filtro.exibeFiltro = false;		
	}

	$scope.campos = {
		ativo: 'true',
		selects: {
			perfil: ngSelects.obterConfiguracao(Api.Perfil, { tamanhoPagina: 15, campoNome: 'stNome' }),
		}
	};
	
	$scope.itensporpagina = 15;
	
	$scope.carregar = function (skip, take)
	{
		var opcoes = { ativo: 'true', skip: skip, take: take };
		var filtro = ngHistoricoFiltro.filtro.filtroGerado;
		if (filtro)
			angular.extend(opcoes, filtro);

		Api.Usuario.listarPaginado(opcoes, function (data) {
			$scope.lista = data.results;
			$scope.total = data.count;
		});
	}

	$scope.exibir = function (mdl)
	{
		if (!$scope.permModel.visualizar) 
			toastr.error('Você não tem permissão para acessar esta página', 'Erro');
		else
			$state.go('usuario', { id: mdl.id });
	}

	$scope.adicionar = function ()
	{
		if (!$scope.permModel.novo)
			toastr.error('Você não tem permissão para acessar esta página', 'Erro');
		else
			$state.go('usuario-novo');
	}

	$scope.pesquisar = function ()
	{
		$scope.carregar(0, $scope.itensporpagina);
		$scope.paginador.reiniciar();
	}
}]);