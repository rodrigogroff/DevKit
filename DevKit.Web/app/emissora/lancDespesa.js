
angular.module('app.controllers').controller('EmissoraLancDespesaController',
['$scope', '$rootScope', '$state', 'Api', 'ngSelects', 'ngHistoricoFiltro',
function ($scope, $rootScope, $state, Api, ngSelects, ngHistoricoFiltro )
{
	$rootScope.exibirMenu = true;
    $scope.loading = false;

	init();

	function init()
    {
        $scope.pesquisarCred = false;
        $scope.pesquisarAssoc = false;

        $scope.campos =
            {
                valor: '0,00',
                parcelas: '1',
                codigo: '',
                nomeAssociado: '',
                nomeCredenciado: '',
                tipo: '',
                selectTipoAutorizacao: ngSelects.obterConfiguracao(Api.TipoAutorizacaoCombo, {})
            };

        $scope.itensporpagina = 15;        
	}

    $scope.$watch('campos.tipo', function (newState, oldState)
    {
        if (newState !== oldState)
        {
            $scope.list = undefined;
            $scope.campos.selecionado = undefined;

            if ($scope.paginadorMaterial != undefined) $scope.paginadorMaterial.reiniciar();
            if ($scope.paginadorDiaria != undefined) $scope.paginadorDiaria.reiniciar();
            if ($scope.paginadorMed != undefined) $scope.paginadorMed.reiniciar();
            if ($scope.paginadorNaoMed != undefined) $scope.paginadorNaoMed.reiniciar();
            if ($scope.paginadorOPME != undefined) $scope.paginadorOPME.reiniciar();
            if ($scope.paginadorPacote != undefined) $scope.paginadorPacote.reiniciar();
            if ($scope.paginadorProc != undefined) $scope.paginadorProc.reiniciar();

            ngHistoricoFiltro.filtro.paginaAtual = 0;
        }            
    });

    // ---------------------------
    // buscar credenciados
    // ---------------------------

    $scope.buscaCredenciado = function ()
    {
        $scope.loading = true;
        $scope.campos.nomeCredenciado = '';

        var opcoes = { skip: 0, take: 1, codigo: $scope.campos.credenciado };

        Api.Credenciado.listPage(opcoes, function (data)
        {
            if (data.results.length > 0) 
                $scope.campos.nomeCredenciado = data.results[0].stNome;
            
            $scope.loading = false;
            $scope.cred_fail = invalidCheck($scope.campos.credenciado) || $scope.campos.nomeCredenciado == '';
        });
    }

    $scope.buscaCred = function ()
    {
        $scope.pesquisarCred = true;
        $scope.campos.nomeCredenciado = '';
    }

    $scope.fechar_buscaCred = function ()
    {
        $scope.pesquisarCred = false;
        $scope.list = undefined;
    }

    $scope.searchCredenciado = function ()
    {
        $scope.loadCredenciado(0, $scope.itensporpagina);        
        if ($scope.paginadorCredenciado != undefined) $scope.paginadorCredenciado.reiniciar();
    }

    $scope.loadCredenciado = function (skip, take)
    {
        $scope.loading = true;

        var opcoes = { skip: skip, take: take, nome: $scope.campos.buscacred_nome };

        Api.EmissorListagemCredenciado.listPage(opcoes, function (data) {
            $scope.list = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        });
    }

    $scope.selecionarCredenciado = function (mdl) {
        $scope.list = undefined;
        $scope.pesquisarCred = false;
        $scope.campos.credenciado = mdl.nuCodigo;
        $scope.buscaCredenciado();        
    }

    // ---------------------------
    // buscar asssociado
    // ---------------------------

    $scope.buscaAssoc = function () {
        $scope.pesquisarAssoc = true;
        $scope.campos.nomeAssociado = '';
        
    }

    $scope.fechar_buscaAssoc = function () {
        $scope.pesquisarAssoc = false;
        $scope.list = undefined;
    }

    $scope.searchAssociado = function () {
        $scope.loadAssociado(0, $scope.itensporpagina);
        if ($scope.paginadorCredenciado != undefined) $scope.paginadorCredenciado.reiniciar();
    }

    $scope.loadAssociado = function (skip, take) {
        $scope.loading = true;

        var opcoes = {
            skip: skip,
            take: take,
            busca: $scope.campos.busca,
        };

        Api.Associado.listPage(opcoes, function (data) {
            $scope.list = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        });
    }

    $scope.selecionarAssociado = function (mdl) {
        $scope.list = undefined;
        $scope.pesquisarAssoc = false;
        $scope.campos.matricula = mdl.nuMatricula;
        $scope.buscaCartao();
    }

    $scope.buscaCartao = function ()
    {
        $scope.loading = true;
        $scope.campos.nomeAssociado = '';

        var opcoes = { skip: 0, take: 1, matricula: $scope.campos.matricula };

        Api.Associado.listPage(opcoes, function (data) {
            if (data.results.length > 0) {
                $scope.campos.nomeAssociado = data.results[0].stName;
            }
            $scope.loading = false;

            $scope.assoc_fail = invalidCheck($scope.campos.matricula) || $scope.campos.nomeAssociado == '';
        });
    }

    // fim

    $scope.show = function (mdl) {
        $scope.campos.selecionado = mdl;
    }
    
    $scope.cancelar = function (mdl) {
        $scope.campos.selecionado = undefined;
    }

    var invalidCheck = function (element) {
        if (element == undefined)
            return true;
        else
            if (element.length == 0)
                return true;

        return false;
    }

    $scope.salvar = function ()
    {
        $scope.assoc_fail = invalidCheck($scope.campos.matricula) || $scope.campos.nomeAssociado == '';
        $scope.cred_fail = invalidCheck($scope.campos.credenciado) || $scope.campos.nomeCredenciado == '';
        $scope.dt_fail = invalidCheck($scope.campos.dt);
        $scope.vr_fail = invalidCheck($scope.campos.valor);
        $scope.nu_parc_fail = invalidCheck($scope.campos.parcelas);
                
        if ($scope.campos.valor == "0,00")
            $scope.vr_fail = true;

        if (!$scope.assoc_fail && !$scope.dt_fail && !$scope.vr_fail && !$scope.nu_parc_fail) {

        }
    }

    // ----------------------
    // Procedimentos
    // ----------------------

    $scope.searchProc = function () {
        $scope.loadProc(0, $scope.itensporpagina);
        $scope.paginador.reiniciar();
    }

    $scope.loadProc = function (skip, take) {
        $scope.loading = true;

        var opcoes = {
            skip: skip,
            take: take,
            codigo: $scope.campos.codigo,
        };

        Api.PrecoProcedimento.listPage(opcoes, function (data) {
            $scope.list = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        },
            function (response) {
                $scope.loading = false;
            });
    }
    
    // ------------------
    // DIARIA
    // ------------------

	$scope.searchDiaria = function ()
    {
        $scope.campos.selecionado = undefined;
        $scope.loadDiaria(0, $scope.itensporpagina);
        if ($scope.paginadorDiaria != undefined)
            $scope.paginadorDiaria.reiniciar();

        $scope.paginaAtual = 1;
	}
    
	$scope.loadDiaria = function (skip, take)
	{
		$scope.loading = true;

        var opcoes = {
            skip: skip,
            take: take,
            codigo: $scope.campos.codigo,
            desc: $scope.campos.desc,
        };

		Api.PrecoDiaria.listPage(opcoes, function (data)
		{
			$scope.list = data.results;
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
        $scope.campos.selecionado = undefined;
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
            $scope.list = data.results;
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
        $scope.campos.selecionado = undefined;
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
            $scope.list = data.results;
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
        $scope.campos.selecionado = undefined;
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
            $scope.list = data.results;
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
        $scope.campos.selecionado = undefined;
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
            $scope.list = data.results;
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
        $scope.campos.selecionado = undefined;
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
            $scope.list = data.results;
            $scope.total = data.count;
            $scope.loading = false;
        },
        function (response) {
            $scope.loading = false;
        });
    }

}]);
