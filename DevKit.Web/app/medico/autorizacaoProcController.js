
angular.module('app.controllers').controller('AutorizacaoProcController',
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
        $scope.selecionouProc = null;

        $scope.pedirSenha = false;
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
            procedimento: $scope.campos.procedimento,
        };

		Api.TUSS.listPage(opcoes, function (data)
		{
			$scope.list = data.results;
			$scope.total = data.count;
			$scope.loading = false;
        },
            function (response) {
                $scope.loading = false;
            });
	}
        
	$scope.marcar = function (mdl)
    {
        if (mdl.selecionado == undefined)
            mdl.selecionado = false;

        mdl.selecionado = !mdl.selecionado;

        for (var t = 0; t < $scope.list.length; ++t)
        {
            var item = $scope.list[t];
            if (item.nuCodTUSS != mdl.nuCodTUSS)
                item.selecionado = false;
        }

        if (mdl.selecionado == true)
            $scope.selecionouProc = mdl.nuCodTUSS;
        else
            $scope.selecionouProc = null;
    }

    $scope.buscarCartao = function ()
    {
        $scope.loading = true;
        $scope.associado = undefined;

        var opcoes = {
            emp: $scope.campos.cartEmp,
            mat: $scope.campos.cartMat,
            ca: $scope.campos.cartCA,
            titVia: $scope.campos.cartTitVia,
        };

        Api.Cartao.listPage(opcoes, function (data) {
            $scope.associado = data;
            $scope.loading = false;
        },
            function (response) {
                $scope.loading = false;
                toastr.error(response.message, 'Verificação');
        });
    }

    $scope.abreSenha = function ()
    {
        $scope.pedirSenha = true;
    }

    $scope.closeModalSenha = function () {
        $scope.pedirSenha = false;
    }

    $scope.autorizar = function ()
    {
        var opcoes = {
            emp: $scope.campos.cartEmp,
            mat: $scope.campos.cartMat,
            ca: $scope.campos.cartCA,            
            titVia: $scope.campos.cartTitVia,
            senha: $scope.campos.senha,
            tuss: $scope.selecionouProc,
        };
        
        Api.AutorizaProc.listPage(opcoes, function (data)
        {
            $scope.closeModalSenha();
        },
        function (response) {
            toastr.error(response.message, 'Autorização');
        });
    }

}]);
