
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
        $scope.campos = {
            tgSituacao: 1,
            ano: (new Date()).getFullYear(),
            tipo: 1,
            modo: 2
        };

        $scope.selectMonths = ngSelects.obterConfiguracao(Api.MonthCombo, {});
        $scope.selectSituacaoAutorizacao = ngSelects.obterConfiguracao(Api.TipoSituacaoAutorizacaoCombo, {});
        $scope.selectSecao = ngSelects.obterConfiguracao(Api.EmpresaSecaoCombo, {});
        $scope.selectTipoAutorizacao = ngSelects.obterConfiguracao(Api.TipoAutorizacaoCombo, {});

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

    $scope.searchCredSelect = function (m) {

        if ($scope.campos.codCred != m.codCredenciado)
            $scope.campos.codCred = m.codCredenciado;
        else
            $scope.campos.codCred = '';

        $scope.search();
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

                for (var y = 0; y < m.resultsExtras.length; y++) {

                    var k = m.resultsExtras[y];

                    if (k.selecionado == true) {

                        k.codCred = mdl.codCredenciado;
                        k.credenciado = mdl.nomeCredenciado;
                        k.associado = k.portador;

                        $scope.listLote.push(k);   
                    }
                }
            }            
        }

        if ($scope.listLote.length > 0)
            $scope.mostraLote = true;
        else {
            // vai tudo

            for (var i = 0; i < $scope.list.length; i++) {
                var mdl = $scope.list[i];

                for (var t = 0; t < mdl.results.length; t++) {
                    var m = mdl.results[t];

                    m.selecionado = true;
                    m.codCred = mdl.codCredenciado;
                    m.credenciado = mdl.nomeCredenciado;

                    $scope.listLote.push(m);

                    for (var y = 0; y < m.resultsExtras.length; y++) {

                        var k = m.resultsExtras[y];

                        k.selecionado = true;
                        k.codCred = mdl.codCredenciado;
                        k.credenciado = mdl.nomeCredenciado;
                        k.associado = k.portador;
                        
                        $scope.listLote.push(k);
                    }
                }

                for (var t = 0; t < mdl.resultsExtras.length; t++) {
                    var m = mdl.resultsExtras[t];

                    m.selecionado = true;
                    m.codCred = mdl.codCredenciado;
                    m.credenciado = mdl.nomeCredenciado;

                    $scope.listLote.push(m);
                }
            }

            if ($scope.listLote.length > 0)
                $scope.mostraLote = true;
            else 
                toastr.error('Nenhuma autorização encontrada', 'Sistema');
        }
    }

    $scope.cancelarModal = function ()
    {
        for (var i = 0; i < $scope.list.length; i++) {
            var mdl = $scope.list[i];
            for (var t = 0; t < mdl.results.length; t++)
                mdl.results[t].selecionado = false;
            for (var t = 0; t < mdl.resultsExtras.length; t++)
                mdl.resultsExtras[t].selecionado = false;
        }

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

    $scope.lancDespModal = function (m, mdl) {
        $scope.mostraLancDesp = true;

        $scope.camposLanc =
            {
                matricula : m.matricula + " - " + m.associado,
                credenciado : mdl.codCredenciado + " - " + mdl.nomeCredenciado,
                dt: m.dtSolicitacao,
                parcelas: 1,
                nsu : m.nsu,
            };
    }

    $scope.$watch('camposLanc.tipo', function (newState, oldState)
    {
        if (newState !== oldState)
        {
            $scope.listLanc = undefined;
            $scope.camposLanc.selecionado = undefined;
            $scope.camposLanc.codigo = undefined;
            $scope.camposLanc.desc = undefined;

            if ($scope.paginadorMaterial != undefined) $scope.paginadorMaterial.reiniciar();
            if ($scope.paginadorDiaria != undefined) $scope.paginadorDiaria.reiniciar();
            if ($scope.paginadorMed != undefined) $scope.paginadorMed.reiniciar();
            if ($scope.paginadorNaoMed != undefined) $scope.paginadorNaoMed.reiniciar();
            if ($scope.paginadorOPME != undefined) $scope.paginadorOPME.reiniciar();
            if ($scope.paginadorPacote != undefined) $scope.paginadorPacote.reiniciar();
            if ($scope.paginadorProc != undefined) $scope.paginadorProc.reiniciar();

            if (ngHistoricoFiltro != undefined)
                ngHistoricoFiltro.filtro.paginaAtual = 0;

            $scope.camposLanc.tipo_desp_fail = false;
        }
    });

    $scope.cancelarModalLanc = function () {
        $scope.mostraLancDesp = false;
    }    

    var invalidCheck = function (element) {
        if (element == undefined)
            return true;
        else
            if (element.length == 0)
                return true;

        return false;
    }
    
    $scope.show = function (mdl) {
        $scope.camposLanc.selecionado = mdl;
        if ($scope.camposLanc.tipo == 2) 
            $scope.camposLanc.valor = mdl.svrNivel1;
        else 
            $scope.camposLanc.valor = mdl.svrValor;
    }

    $scope.confirmarModalLanc = function ()
    {
        $scope.camposLanc.vr_fail = invalidCheck($scope.camposLanc.valor);
        $scope.camposLanc.nu_parc_fail = invalidCheck($scope.camposLanc.parcelas);

        if ($scope.camposLanc.valor == "0,00")
            $scope.camposLanc.vr_fail = true;

        $scope.camposLanc.tipo_desp_fail = $scope.camposLanc.selecionado == undefined;

        if ($scope.camposLanc.vr_fail == false &&
            $scope.camposLanc.nu_parc_fail == false &&
            $scope.camposLanc.tipo_desp_fail == false)
        {
            $scope.loading = true;

            var opcoes = {
                nsuRef: $scope.camposLanc.nsu,
                vrValor: $scope.camposLanc.valor,
                nuTipo: $scope.camposLanc.tipo,
                fkPrecificacao: $scope.camposLanc.selecionado.id,
                nuParcelas: $scope.camposLanc.parcelas
            };

            Api.EmissorLancaDespesa.listPage(opcoes, function (data) {
                toastr.success('Despesa lançada com sucesso', 'Sistema');
                $scope.loading = false;
                $scope.search();
                $scope.camposLanc.valor = '';
                $scope.camposLanc.listLanc = undefined;
                $scope.camposLanc.tipo = undefined;
            },
            function (response) {
                $scope.loading = false;
            });
        }
    }

    // ------------------
    // DIARIA
    // ------------------

    $scope.searchDiaria = function () {
        $scope.listLanc = undefined;
        $scope.camposLanc.selecionado = undefined;
        $scope.loadDiaria(0, $scope.itensporpagina);
        if ($scope.paginadorDiaria != undefined)
            $scope.paginadorDiaria.reiniciar();

        $scope.paginaAtual = 1;
    }

    $scope.loadDiaria = function (skip, take) {
        $scope.loading = true;

        var opcoes = {
            skip: skip,
            take: take,
            codigo: $scope.campos.codigo,
            desc: $scope.campos.desc,
        };

        Api.PrecoDiaria.listPage(opcoes, function (data) {
            $scope.listLanc = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        },
            function (response) {
                $scope.loading = false;
            });
    }

    // ------------------
    // MATERIAL
    // ------------------

    $scope.searchMaterial = function () {
        $scope.listLanc = undefined;
        $scope.camposLanc.selecionado = undefined;
        $scope.loadMaterial(0, $scope.itensporpagina);
        if ($scope.paginadorMaterial != undefined)
            $scope.paginadorMaterial.reiniciar();
    }

    $scope.loadMaterial = function (skip, take) {
        $scope.loading = true;

        var opcoes = {
            skip: skip,
            take: take,
            codigo: $scope.campos.codigo,
            desc: $scope.campos.desc,
        };

        Api.PrecoMaterial.listPage(opcoes, function (data) {
            $scope.listLanc = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        },
            function (response) {
                $scope.loading = false;
            });
    }

    // ------------------
    // MEDICAMENTOS
    // ------------------

    $scope.searchMed = function () {
        $scope.listLanc = undefined;
        $scope.camposLanc.selecionado = undefined;
        $scope.loadMed(0, $scope.itensporpagina);
        if ($scope.paginadorMed != undefined)
            $scope.paginadorMed.reiniciar();
    }

    $scope.loadMed = function (skip, take) {
        $scope.loading = true;

        var opcoes = {
            skip: skip,
            take: take,
            codigo: $scope.campos.codigo,
            desc: $scope.campos.desc,
        };

        Api.PrecoMedicamento.listPage(opcoes, function (data) {
            $scope.listLanc = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        },
            function (response) {
                $scope.loading = false;
            });
    }

    // ------------------
    // Nao medicos
    // ------------------

    $scope.searchNaoMed = function () {
        $scope.listLanc = undefined;
        $scope.camposLanc.selecionado = undefined;
        $scope.loadNaoMed(0, $scope.itensporpagina);
        if ($scope.paginadorNaoMed != undefined)
            $scope.paginadorNaoMed.reiniciar();
    }

    $scope.loadNaoMed = function (skip, take) {
        $scope.loading = true;

        var opcoes = {
            skip: skip,
            take: take,
            codigo: $scope.campos.codigo,
            desc: $scope.campos.desc,
        };

        Api.PrecoNaoMedico.listPage(opcoes, function (data) {
            $scope.listLanc = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        },
            function (response) {
                $scope.loading = false;
            });
    }

    // ------------------
    // OPME
    // ------------------

    $scope.searchOPME = function () {
        $scope.listLanc = undefined;
        $scope.camposLanc.selecionado = undefined;
        $scope.loadOPME(0, $scope.itensporpagina);
        if ($scope.paginadorOPME != undefined)
            $scope.paginadorOPME.reiniciar();
    }

    $scope.loadOPME = function (skip, take) {
        $scope.loading = true;

        var opcoes = {
            skip: skip,
            take: take,
            codigo: $scope.campos.codigo,
            desc: $scope.campos.desc,
        };

        Api.PrecoOPME.listPage(opcoes, function (data) {
            $scope.listLanc = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        },
            function (response) {
                $scope.loading = false;
            });
    }

    // --------------
    // PACOTE
    // --------------

    $scope.searchPacote = function () {
        $scope.listLanc = undefined;
        $scope.camposLanc.selecionado = undefined;
        $scope.loadPacote(0, $scope.itensporpagina);
        if ($scope.paginadorPacote != undefined)
            $scope.paginadorPacote.reiniciar();
    }

    $scope.loadPacote = function (skip, take) {
        $scope.loading = true;

        var opcoes = {
            skip: skip,
            take: take,
            codigo: $scope.campos.codigo,
            desc: $scope.campos.desc,
        };

        Api.PrecoPacote.listPage(opcoes, function (data) {
            $scope.listLanc = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        },
            function (response) {
                $scope.loading = false;
            });
    }

}]);
