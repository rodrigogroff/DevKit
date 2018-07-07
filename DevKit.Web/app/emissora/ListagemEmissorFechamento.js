
angular.module('app.controllers').controller('ListagemEmissorFechamentoController',
['$scope', '$rootScope', '$state', 'Api', 'ngSelects', 
function ($scope, $rootScope, $state, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;

    function CheckPermissions() {
        Api.Permission.get({ id: $scope.permID }, function (data) {
            $scope.permModel = data;

            if (!$scope.permModel.listagem) {
                toastr.error('Acesso negado para relatório de fechamento!', 'Permissão');
                $state.go('home');
            }
        },
            function (response) { });
    }

	init();

	function init()
    {
        $scope.campos = { tgSituacao: 1};

        $scope.selectMonths = ngSelects.obterConfiguracao(Api.MonthCombo, {});
        $scope.selectSituacaoAutorizacao = ngSelects.obterConfiguracao(Api.TipoSituacaoAutorizacaoCombo, {});
        $scope.selectSecao = ngSelects.obterConfiguracao(Api.EmpresaSecaoCombo, {});

        $scope.itensporpagina = 15;

        $scope.permModel = {};
        $scope.permID = 601;

        CheckPermissions();
	}

	$scope.search = function ()
	{
        $scope.load(0, $scope.itensporpagina);
        if ($scope.paginador != undefined)
		    $scope.paginador.reiniciar();
	}

	$scope.load = function (skip, take)
    {
        if ($scope.campos.mes > 0 && $scope.campos.ano > 2000 && $scope.campos.tipo > 0 && $scope.campos.modo > 0)

		$scope.loading = true;

        var opcoes = {
            mes: $scope.campos.mes,
            ano: $scope.campos.ano,
            tipo: $scope.campos.tipo,
            modo: $scope.campos.modo,
            tgSituacao: $scope.campos.tgSituacao,
            fkSecao: $scope.campos.fkSecao,
            codCred: $scope.campos.codCred,
            mat: $scope.campos.mat,
        };
        
		Api.EmissorListagemFechamento.listPage(opcoes, function (data)
		{
			$scope.list = data.results;
            $scope.result = data;
            $scope.tipoSel = $scope.campos.tipo;
            $scope.modoSel = $scope.campos.modo;

			$scope.loading = false;
		});
    }

    $scope.marca = function (m)
    {
        if (m.selecionado == undefined)
            m.selecionado = false;

        m.selecionado = !m.selecionado;
    }

    $scope.operacoesLote = function ()
    {
        $scope.listLote = [];

        for (var i = 0; i < $scope.list.length; i++)
        {
            var mdl = $scope.list[i];

            for (var t = 0; t < mdl.results.length; t++)
            {
                var m = mdl.results[t];

                if (m.selecionado == true)
                    $scope.listLote.push(m);            
            }            
        }

        if ($scope.listLote.length > 0)
            $scope.mostraLote = true;
    }

    $scope.cancelarModal = function () {
        $scope.mostraLote = false;
    }

    $scope.confirmaLote = function ()
    {
        var nsus = '';

        for (var i = 0; i < $scope.listLote.length; i++) {
            var m = $scope.listLote[i];
            if (m.selecionado == true) {
                nsus += m.nsu + ','
            }
        }

        $scope.loading = true;

        var opcoes = {
            oper: 'mudaSituacao',
            nsu: nsus,
            tgSituacaoLote: $scope.campos.tgSituacaoLote,            
        };

        Api.EmissorFechamentoOper.listPage(opcoes, function (data)
        {
            toastr.success('Autorizações convertidas com sucesso!', 'Sistema');

            $scope.mostraLote = false;
            $scope.loading = false;            
        });       
    }
    
}]);
