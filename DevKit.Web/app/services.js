'use strict';

// Demonstrate how to register services
// In this case it is a simple value service.
angular.module('app.services', ['ngResource'])

.service('$confirmacao', ['$modal', '$rootScope', '$q', function ($modal, $rootScope, $q) {

	var deferred;
	var scope = $rootScope.$new();

	scope.resposta = function (res) {
		deferred.resolve(res);
		confirmacao.hide();
	}

	var confirmacao = $modal({ template: 'app/_shared/templateConfirmacao.html', scope: scope, show: false });
	var parentShow = confirmacao.show;

	confirmacao.exibir = function (titulo, mensagem) {
		scope.titulo = titulo;
		scope.mensagem = mensagem;
		deferred = $q.defer();
		parentShow();
		return deferred.promise;
	}

	return confirmacao;
}])

.factory('Api', ['$resource', function ($resource)
{
	var opcoes = {
		'add': { method: 'POST' },
		'list': { method: 'GET', isArray: true },
		'listPage': { method: 'GET', isArray: false },
		'get': { method: 'GET', isArray: false },
		'update': { method: 'PUT' },
		'remove': { method: 'DELETE' }
	};

    return {

        Setup: $resource('api/setup/:id', {}, opcoes),
        Associado: $resource('api/associado/:id', {}, opcoes),
        Loja: $resource('api/loja/:id', {}, opcoes),
        Empresa: $resource('api/empresa/:id', {}, opcoes),
        EmpresaDBA: $resource('api/empresaDBA/:id', {}, opcoes),
        LoteDBA: $resource('api/loteDBA/:id', {}, opcoes),
        DataServer: $resource('api/dataServer/:id', {}, opcoes),
        ExpedicaoDBA: $resource('api/ExpedicaoDBA/:id', {}, opcoes),
        FaturamentoDBA: $resource('api/FaturamentoDBA/:id', {}, opcoes),
        LoteDetalhesDBA: $resource('api/loteDetalhesDBA/:id', {}, opcoes),
        LimiteAssociado: $resource('api/limiteassociado/:id', {}, opcoes),
        AdmOper: $resource('api/admoper/:id', {}, opcoes),
        ExtratoAssociado: $resource('api/extratoassociado/:id', {}, opcoes),
        TerminalLoja: $resource('api/terminalLoja/:id', {}, opcoes),
        SimulaParcelada: $resource('api/simulaparcelada/:id', {}, opcoes),
        SomaParcelada: $resource('api/somaparcelada/:id', {}, opcoes),
        EfetuaVenda: $resource('api/efetuavenda/:id', {}, opcoes),
        LojistasAssociado: $resource('api/lojistasassociado/:id', {}, opcoes),
        LojistaMensagens: $resource('api/lojistamensagens/:id', {}, opcoes),
        CancelaVenda: $resource('api/cancelavenda/:id', {}, opcoes),
        CancelaVendaDBA: $resource('api/cancelavendadba/:id', {}, opcoes),
        Cache: $resource('api/cache/:id', {}, opcoes),
        ListagemRastreamento: $resource('api/listagemRastreamento/:id', {}, opcoes),
        OrdemRelLojistaTrans: $resource('api/ordemrellojistaTrans/:id', {}, opcoes),
        OrdemEmissorManutCartoes: $resource('api/ordememissormanutcartoes/:id', {}, opcoes),
        ConfereNSU: $resource('api/conferensu/:id', {}, opcoes),
        HomeView: $resource('api/homeView/:id', {}, opcoes),
        Auditoria: $resource('api/auditoria', {}, opcoes),
        EmissoraCartao: $resource('api/emissoracartao/:id', {}, opcoes),
        EmissoraLoja: $resource('api/emissoraloja/:id', {}, opcoes),
        EmissoraUsuario: $resource('api/emissorausuario/:id', {}, opcoes),
        EmissoraRelRepFinanc: $resource('api/emissorarelrepfinanc/:id', {}, opcoes),
        EmissoraRelExtratos: $resource('api/emissorarelextratos/:id', {}, opcoes),
        EmissoraRelExtratoForn: $resource('api/emissorarelextratoforn/:id', {}, opcoes),
        EmissoraRelExtratoTrans: $resource('api/emissorarelextratotrans/:id', {}, opcoes),
        EmissoraRelExtratoTransLojas: $resource('api/emissorarelextratotranslojas/:id', {}, opcoes),
        EmissoraFechamento: $resource('api/emissorafechamento/:id', {}, opcoes),
        RelAssociados: $resource('api/relAssociados/:id', {}, opcoes),
        RelLojistaTrans: $resource('api/relLojistaTrans/:id', {}, opcoes),
        GLDemonstrativo: $resource('api/glDemonstrativo/:id', {}, opcoes),
        MonthCombo: $resource('api/month/:id', {}, opcoes),
        SituacoesCombo: $resource('api/situacoes/:id', {}, opcoes),
        ExpedicoesCombo: $resource('api/expedicoes/:id', {}, opcoes),
        ConveniosCombo: $resource('api/convenios/:id', {}, opcoes),
        EnvioEmail: $resource('api/envioemail/:id', {}, opcoes),
	};
}]);
