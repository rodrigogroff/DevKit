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
		User: $resource('api/user/:id', {}, opcoes),
		Profile: $resource('api/profile/:id', {}, opcoes),
		Permission: $resource('api/permission/:id', {}, opcoes),
		Project: $resource('api/project/:id', {}, opcoes),
		Phase: $resource('api/phase/:id', {}, opcoes),
		Version: $resource('api/version/:id', {}, opcoes),
		Sprint: $resource('api/sprint/:id', {}, opcoes),
		TaskType: $resource('api/taskType/:id', {}, opcoes),
		TaskCategory: $resource('api/taskCategory/:id', {}, opcoes),
		TaskFlow: $resource('api/taskFlow/:id', {}, opcoes),
		TaskCount: $resource('api/taskCount/:id', {}, opcoes),
		TaskAccumulator: $resource('api/taskAccumulator/:id', {}, opcoes),
		Task: $resource('api/task/:id', {}, opcoes),
		Priority: $resource('api/priority/:id', {}, opcoes),
		UserKanban: $resource('api/userKanban/:id', {}, opcoes),
		Management: $resource('api/management/:id', {}, opcoes),
		AccType: $resource('api/accumulatorType/:id', {}, opcoes),
		Month: $resource('api/month/:id', {}, opcoes),		
		VersionState: $resource('api/versionState/:id', {}, opcoes),
		Timesheet: $resource('api/versionState/:id', {}, opcoes)

	};
}]);
