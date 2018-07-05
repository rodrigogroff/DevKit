
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
        $scope.selectMonths = ngSelects.obterConfiguracao(Api.MonthCombo, {});
        $scope.selectSituacaoAutorizacao = ngSelects.obterConfiguracao(Api.TipoSituacaoAutorizacaoCombo, {});
        
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
            mes: $scope.campos.mes,
            ano: $scope.campos.ano,
            nomeAssociado: $scope.campos.nomeAssociado,
            nomeCredenciado: $scope.campos.nomeCredenciado,
            codCredenciado: $scope.campos.codCredenciado,
            matricula: $scope.campos.matricula,
            tgSituacao: $scope.campos.tgSituacao,
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

    $scope.show = function (mdl) {
        $scope.cupom = mdl.cupom;
        $scope.mostraGuia = true;
    }

    $scope.marca = function (mdl) {
        if (mdl.selecionado == undefined)
            mdl.selecionado = true;
        else
            mdl.selecionado = !mdl.selecionado;
    }

    $scope.cancelar = function () {

        $scope.listCanc = [];
        var num = 0;
        for (var i = 0; i < $scope.list.length; i++)
            if ($scope.list[i].selecionado == true) {
                $scope.listCanc.push($scope.list[i]);
                num++;
            }
                
        if (num > 0)
            $scope.mostraCancel = true;
    }

    $scope.cancelarModal = function () {
        $scope.mostraCancel = false;
    }

    $scope.cancelarAutorizacoes = function ()
    {
        for (var i = 0; i < $scope.listCanc.length; i++)
        {
            var f = $scope.listCanc[i];

            $scope.loading = true;

            var opcoes = {
                codCredenciado: f.snuCodigoCredenciado,
                matricula: f.snuMatriculaAssociado,
                dt: f.sdtSolicitacao,
                fkSecao: f.sfkSecao,
                nsu: f.nuNSU,
            };

            Api.EmissorCancelamento.listPage(opcoes, function (data) {
                $scope.loading = false;
            });
        }

        $scope.list = undefined;
        $scope.total = undefined;

        toastr.success('Cancelamentos efetuados com sucesso!', 'Sistema');

        $scope.mostraCancel = false;
    }

    $scope.cancelarGuia = function () {
        $scope.mostraGuia = false;
    }

    $scope.imprimirGuia = function () {
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
        printContents += "<br /><br /><table><tr><td><img src='../../images/fumam.png' style='max-height:180px' /></td><td width='20px'></td><td>Através do presente reconheço expressamente que possuo esta autorização para ser descontado <br />em minha folha de pagamento, conforme coparticipação de R$ " + $scope.cupom.vrCoPart + "<br /><br />_______________________________________<br />" + $scope.cupom.associadoNome + "<br /><br /></td ></tr ></table >";

        var popupWin = window.open('', '_blank', 'width=900,height=600');
        popupWin.document.open();
        popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
        popupWin.document.close();

        init();
    }

}]);
