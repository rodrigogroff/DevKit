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
        LimiteAssociado: $resource('api/limiteassociado/:id', {}, opcoes),
        TerminalLoja: $resource('api/terminalLoja/:id', {}, opcoes),
        SimulaParcelada: $resource('api/simulaparcelada/:id', {}, opcoes),
        SomaParcelada: $resource('api/somaparcelada/:id', {}, opcoes),
        EfetuaVenda: $resource('api/efetuavenda/:id', {}, opcoes),
        LojistaMensagens: $resource('api/lojistamensagens/:id', {}, opcoes),
        CancelaVenda: $resource('api/cancelavenda/:id', {}, opcoes),
        Cache: $resource('api/cache/:id', {}, opcoes),
        OrdemRelLojistaTrans: $resource('api/OrdemRelLojistaTrans/:id', {}, opcoes),        
        ConfereNSU: $resource('api/conferensu/:id', {}, opcoes),
        HomeView: $resource('api/homeView/:id', {}, opcoes),
        RelAssociados: $resource('api/relAssociados/:id', {}, opcoes),
        RelLojistaTrans: $resource('api/relLojistaTrans/:id', {}, opcoes),
        MonthCombo: $resource('api/month/:id', {}, opcoes),
	};
}]);
