angular.module('app.controllers').controller('ListingLojasController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
	$rootScope.exibirMenu = true;

	$scope.loading = false;

    $scope.campos = {
        bloqueada: 'false',
        comSenha: 'undefined'
	};

	$scope.itensporpagina = 15;
	
	init();

	function init()
    {
        $scope.selectEmpresa = ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 });

		if (ngHistoricoFiltro.filtro)
			ngHistoricoFiltro.filtro.exibeFiltro = false;
	}

	$scope.search = function ()
	{
		$scope.load(0, $scope.itensporpagina, false);
		$scope.paginador.reiniciar();
	}

	$scope.load = function (skip, take, exportar)
	{
        var opcoes =
            {
                skip: skip,
                take: take,
                busca: $scope.campos.busca,
                terminal: $scope.campos.terminal,
                cidade: $scope.campos.cidade,
                estado: $scope.campos.estado,
                bloqueada: $scope.campos.bloqueada,
                comSenha: $scope.campos.comSenha,
                idEmpresa: $scope.campos.idEmpresa
            };

        if (exportar)
        {
            toastr.warning('Aguarde, solicitação em andamento', 'Exportar');
            window.location.href = "/api/loja/exportar?" + $.param(opcoes);
        }
        else
        {
            $scope.loading = true;

            Api.Loja.listPage(opcoes, function (data) {
                $scope.list = data.results;
                $scope.total = data.count;
                $scope.loading = false;
            });
        }
	}

    $scope.exportar = function () {
        $scope.load(0, $scope.itensporpagina, true);
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
