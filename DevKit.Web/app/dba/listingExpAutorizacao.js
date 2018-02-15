
angular.module('app.controllers').controller('ListingExpAutorizacaoController',
['$scope', '$rootScope', '$state', 'Api', 'ngSelects', 
function ($scope, $rootScope, $state, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;

	init();

	function init()
    {
        $scope.selectEmpresa = ngSelects.obterConfiguracao(Api.EmpresaCombo, {});

        $scope.campos = {
            mes: '',
            ano: '',
            fkEmpresa: undefined,
        };

        $scope.itensporpagina = 15;
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
            fkEmpresa: $scope.campos.fkEmpresa,
            nuMes: $scope.campos.nuMes,
            nuAno: $scope.campos.nuAno,
        };

		Api.LotesExpAutorizacao.listPage(opcoes, function (data)
		{
			$scope.list = data.results;
			$scope.total = data.count;
			$scope.loading = false;
		});
	}

    $scope.ativar = function (mdl) {
        mdl.selecionado = !mdl.selecionado;
    }
    
    $scope.exportarLote = function ()
    {
        var lote = undefined;

        for (var i = 0; i < $scope.list.length; ++i) {
            if ($scope.list[i].selecionado == true) {
                lote = $scope.list[i];
                break;
            }                
        }

        if (lote == undefined) {
            toastr.error('Escolha algum lote para exportação', 'Exportar lote');
        }
        else
            window.location.href = "/api/lotesexpautorizacao/exportar?" + $.param(
                {
                    fkEmpresa: lote.fkEmpresa,
                    nuMes: lote.nuMes,
                    nuAno: lote.nuAno,
                });
    }

}]);
