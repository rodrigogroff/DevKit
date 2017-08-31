angular.module('app.controllers').controller('ListingLojasController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
	$rootScope.exibirMenu = true;

	$scope.loading = false;

    $scope.campos = {
        bloqueada: 'false',
        comSenha: 'true'
	};

	$scope.itensporpagina = 15;
	
	init();

	function init()
    {
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

        var opcoes = { skip: skip, take: take };

		var filtro = ngHistoricoFiltro.filtro.filtroGerado;

		if (filtro)
            angular.extend(opcoes, filtro);

        opcoes.bloqueada = $scope.campos.bloqueada;
        opcoes.comSenha = $scope.campos.comSenha;

        Api.Loja.listPage(opcoes, function (data) {
            $scope.list = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        });
	}

	$scope.show = function (mdl)
	{
        $state.go('loja', { id: mdl.id });
	}

	$scope.new = function ()
	{
		//$state.go('profile-new');
	}
	
}]);
