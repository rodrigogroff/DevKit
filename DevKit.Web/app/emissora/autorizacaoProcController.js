
angular.module('app.controllers').controller('EmissoraAutorizacaoProcController',
['$scope', '$rootScope', '$state', 'Api', 'ngSelects', 
function ($scope, $rootScope, $state, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;

	init();

	function init()
    {
        $scope.campos = {
            codigo: '',
            aut:true
        };

        $scope.itensporpagina = 15;
        $scope.selecionouProc = null;
        $scope.associado = undefined;
        $scope.pedirSenha = false;
	}

    $scope.buscarCartao = function () {
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
                $scope.associado = undefined;
                $scope.loading = false;
                toastr.error(response.message, 'Verificação');
            });
    }

    $scope.buscarCred = function () {
        $scope.loadCred(0, 50);
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
            aut: $scope.campos.aut,
            emp: $scope.campos.cartEmp,
            codigo: $scope.campos.codigo,
            codigoCred: $scope.selecionouCred,
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

    $scope.loadCred = function (skip, take)
    {
        $scope.loading = true;

        var opcoes = {
            skip: skip,
            take: take,
            especialidade: $scope.campos.credEspec,
            nome: $scope.campos.nomeCred,
        };

        Api.Credenciado.listPage(opcoes, function (data) {
            $scope.listCred = data.results;
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

    $scope.marcarCred = function (mdl) {
        if (mdl.selecionado == undefined)
            mdl.selecionado = false;

        mdl.selecionado = !mdl.selecionado;

        for (var t = 0; t < $scope.listCred.length; ++t) {
            var item = $scope.listCred[t];
            if (item.nuCodigo != mdl.nuCodigo)
                item.selecionado = false;
        }

        if (mdl.selecionado == true)
            $scope.selecionouCred = mdl.nuCodigo;
        else
            $scope.selecionouCred = undefined;
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
            senha: $scope.campos.senhaCartao,
            codigoCred: $scope.selecionouCred,
            tuss: $scope.selecionouProc,
        };
        
        Api.AutorizaProc.listPage(opcoes, function (data)
        {
            $scope.closeModalSenha();
            toastr.success('Procedimento autorizado com sucesso!', 'Autorização');
        },
        function (response) {
            toastr.error(response.data.message, 'Autorização');
        });
    }

}]);
