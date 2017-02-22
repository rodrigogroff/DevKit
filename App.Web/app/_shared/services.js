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

.service('$informacao', ['$modal', '$rootScope', '$q', function ($modal, $rootScope, $q) {

	var deferred;
	var scope = $rootScope.$new();

	scope.resposta = function (res) {
		deferred.resolve(res);
		confirmacao.hide();
	}

	var confirmacao = $modal({ template: 'app/_shared/templateInformacao.html', scope: scope, show: false });
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

.service('$mensagemErro', ['$modal', '$rootScope', '$q', function ($modal, $rootScope, $q) {

	var deferred;
	var scope = $rootScope.$new();

	scope.fechar = function () {
		janela.hide();
	}

	var janela = $modal({ template: 'app/_shared/templateMensagemErro.html', scope: scope, show: false });
	var parentShow = janela.show;

	janela.exibir = function (titulo, mensagem) {
		scope.titulo = titulo;
		scope.mensagem = mensagem;
		deferred = $q.defer();
		parentShow();
		return deferred.promise;
	}

	return janela;
}])

.factory('Api', ['$resource', function ($resource) {
	var opcoes = {
		'adicionar': { method: 'POST' },
		'listar': { method: 'GET', isArray: true },
		'listarPaginado': { method: 'GET', isArray: false },
		'obter': { method: 'GET', isArray: false },
		'atualizar': { method: 'PUT' },
		'remover': { method: 'DELETE' }
	};
	return {
		
		Usuario: $resource('api/usuario/:id', {}, opcoes),
		Perfil: $resource('api/perfil/:id', {}, opcoes),
		Permissao: $resource('api/permissao/:id', {}, opcoes)

	};
}]);
