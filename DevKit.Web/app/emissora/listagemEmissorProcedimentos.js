
angular.module('app.controllers').controller('ListagemEmissorProcedimentosController',
['$scope', '$rootScope', '$state', 'Api', 'ngSelects', 
function ($scope, $rootScope, $state, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
    $scope.loading = false;

    function CheckPermissions() {
        Api.Permission.get({ id: $scope.permID }, function (data) {
            $scope.permModel = data;

            if (!$scope.permModel.listagem) {
                toastr.error('Acesso negado para relatório de autorizações!', 'Permissão');
                $state.go('home');
            }
        },
            function (response) { });
    }
    
	init();

	function init()
    {
        $scope.campos = {
            ativo: 'true',
        };

        $scope.itensporpagina = 15;
        $scope.permModel = {};
        $scope.permID = 602;

        $scope.selectSecao = ngSelects.obterConfiguracao(Api.EmpresaSecaoCombo, {});

        CheckPermissions();
	}

	$scope.search = function ()
	{
		$scope.load(0, $scope.itensporpagina);
		$scope.paginador.reiniciar();
	}

	$scope.load = function (skip, take)
	{
		$scope.loading = true;

        var opcoes = {
            skip: skip,
            take: take,
            fkSecao: $scope.campos.fkSecao,
            tuss: $scope.campos.tuss,
            espec: $scope.campos.espec,
            nomeAssociado: $scope.campos.nomeAssociado,
            nomeCredenciado: $scope.campos.nomeCredenciado,
            codCredenciado: $scope.campos.codCredenciado,
            matricula: $scope.campos.matricula,
            dtInicial: $scope.campos.dtInicial,
            dtFim: $scope.campos.dtFim,
        };

		Api.EmissorListagemAutorizacao.listPage(opcoes, function (data)
		{
			$scope.list = data.results;
			$scope.total = data.count;
			$scope.loading = false;
		});
	}

}]);
