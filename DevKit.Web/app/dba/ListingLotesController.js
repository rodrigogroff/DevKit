
angular.module('app.controllers').controller('ListingLotesController',
['$scope', '$rootScope', '$state', 'Api', 'ngSelects', 
function ($scope, $rootScope, $state, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;

	init();

	function init()
    {
        $scope.campos = {
            codigo: ''
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
            skip: skip,
            take: take,
            codigo: $scope.campos.codigo,
        };

		Api.LotesGrafica.listPage(opcoes, function (data)
		{
			$scope.list = data.results;
			$scope.total = data.count;
			$scope.loading = false;
		});
	}

    $scope.ativar = function (mdl) {
        mdl.selecionado = !mdl.selecionado;
    }

    $scope.ativarTodos = function ()
    {
        var lotes = '';

        for (var i = 0; i < $scope.list.length; ++i) {
            if ($scope.list[i].selecionado == true)
                lotes += $scope.list[i].id + ',';
        }

        if (lotes == '') {
            toastr.error('Escolha algum lote para ativação', 'Ativar lote');
        }
        else 
        {
            var opcoes = {
                ativarLote: true,
                lotes: lotes,
            };

            Api.LotesGrafica.listPage(opcoes, function (data) {
                toastr.success('Lote(s) ativados com sucesso!', 'Ativar lote');
                $scope.search();
            });
        }
    }

    $scope.exportarLote = function ()
    {
        var lote = '';

        for (var i = 0; i < $scope.list.length; ++i) {
            if ($scope.list[i].selecionado == true) {
                lote = $scope.list[i].id;
                break;
            }                
        }

        if (lote == '') {
            toastr.error('Escolha algum lote para exportação', 'Exportar lote');
        }
        else
            window.location.href = "/api/lotesgrafica/exportar?" + $.param({ idLote: lote });
    }

	$scope.new = function ()
	{
		$state.go('novolote');
	}

}]);
