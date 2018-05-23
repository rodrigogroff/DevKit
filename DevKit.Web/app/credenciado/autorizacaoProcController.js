
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
            codigo: '',
            aut:true
        };

        $scope.itensporpagina = 15;
        $scope.selecionouProc = null;
        $scope.associado = undefined;
        $scope.pedirSenha = false;
        $scope.mostraGuia = false;
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
            tuss: $scope.selecionouProc,
        };
        
        Api.AutorizaProc.listPage(opcoes, function (data)
        {
            $scope.cupom = data;
            $scope.closeModalSenha();
            
            $scope.mostraGuia = true;
        },
        function (response) {
            toastr.error(response.data.message, 'Autorização');
        });
    }

    $scope.imprimirGuia = function ()
    {
        $scope.mostraGuia = false;

        var printContents = "<h2>GUIA DE AUTORIZAÇÃO</h2>";

        printContents += "Validade: " + $scope.cupom.validade + "<br>";
        printContents += "<table border='1'>";
        printContents += "<thead><tr><td width='300px'></td><td width='300px'></td><td width='150px'></td><td width='150px'></td></tr></thead>";

        printContents += "<tr height='30px'><td></td ><td><b>" + $scope.cupom.empresa + "</b></td><td colspan='2'>Emissão guia: <b>" + $scope.cupom.emissao + "</b></td></tr >";
        printContents += "<tr height='30px'><td></td><td>Autorização/NSU: <b>" + $scope.cupom.autorizacao + "</b></td><td colspan='2'></td></tr>";
        printContents += "<tr height='15px'></tr>";
        printContents += "<tr height='30px'><td>Associado: <br /><b>(" + $scope.cupom.associadoMat + ") " + $scope.cupom.associadoNome + " - Tit.: " + $scope.cupom.associadoTit + "</b></td><td>Credenciado: <br /><b>" + $scope.cupom.credenciado + "</b></td><td colspan='2'>Vínculo/secao: <br /><b>" + $scope.cupom.secao + "</b></td></tr>";
        printContents += "<tr height='15px'></tr>";
        printContents += "<tr height='30px'><td><b>TUSS</b></td><td><b>PROCEDIMENTO</b></td><td><b>VALOR INTEGRAL</b></td><td><b>VALOR CO-PARTICIPAÇÃO</b></td></tr>";
        printContents += "<tr height='15px'></tr>";
        printContents += "<tr height='30px'><td>" + $scope.cupom.tuss + "</td><td>" + $scope.cupom.procedimento + "</td><td>R$ " + $scope.cupom.vrIntegral + "</td><td>R$ " + $scope.cupom.vrCoPart + "</td></tr>";
        printContents += "<tr height='15px'></tr>";
        printContents += "</table>";
        printContents += "<br /><br />Através do presente reconheço expressamente que possuo esta autorização para ser descontado <br />em minha folha de pagamento, conforme co-participação de R$ " + $scope.cupom.vrCoPart + "<br /><br />_______________________________________<br />" + $scope.cupom.associadoNome + "<br />";

        var popupWin = window.open('', '_blank', 'width=900,height=600');
        popupWin.document.open();
        popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
        popupWin.document.close();

        init();
    }

}]);
