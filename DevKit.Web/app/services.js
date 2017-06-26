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
		User: $resource('api/user/:id', {}, opcoes),
		Profile: $resource('api/profile/:id', {}, opcoes),
		Permission: $resource('api/permission/:id', {}, opcoes),
		Project: $resource('api/project/:id', {}, opcoes),
		Client: $resource('api/client/:id', {}, opcoes),
		ClientGroup: $resource('api/clientGroup/:id', {}, opcoes),
		Phase: $resource('api/phase/:id', {}, opcoes),
		Version: $resource('api/version/:id', {}, opcoes),
		Sprint: $resource('api/sprint/:id', {}, opcoes),
		TaskType: $resource('api/taskType/:id', {}, opcoes),
		TaskCategory: $resource('api/taskCategory/:id', {}, opcoes),
		TaskFlow: $resource('api/taskFlow/:id', {}, opcoes),
		TaskCount: $resource('api/taskCount/:id', {}, opcoes),
		TaskTypeAccumulator: $resource('api/taskAccumulator/:id', {}, opcoes),
		TaskCheckPoint: $resource('api/taskCheckPoint/:id', {}, opcoes),
		Task: $resource('api/task/:id', {}, opcoes),
		Priority: $resource('api/priority/:id', {}, opcoes),
		UserKanban: $resource('api/userKanbanView/:id', {}, opcoes),
		Management: $resource('api/managementView/:id', {}, opcoes),
		AccType: $resource('api/accumulatorType/:id', {}, opcoes),
		Month: $resource('api/month/:id', {}, opcoes),		
		VersionState: $resource('api/versionState/:id', {}, opcoes),
		ProjectTemplate: $resource('api/projectTemplate/:id', {}, opcoes),
		Timesheet: $resource('api/timesheetView/:id', {}, opcoes),
		News: $resource('api/news/:id', {}, opcoes),
        Survey: $resource('api/survey/:id', {}, opcoes),
        Cache: $resource('api/cache/:id', {}, opcoes),
        HomeView: $resource('api/homeView/:id', {}, opcoes),
        ClientCombo: $resource('api/clientcombo/:id', {}, opcoes),
        ClientGroupCombo: $resource('api/clientgroupcombo/:id', {}, opcoes),
        ProfileCombo: $resource('api/profilecombo/:id', {}, opcoes),
        ProjectCombo: $resource('api/projectcombo/:id', {}, opcoes),
        TaskTypeCombo: $resource('api/tasktypecombo/:id', {}, opcoes),
        UserCombo: $resource('api/usercombo/:id', {}, opcoes),
        PhaseCombo: $resource('api/phasecombo/:id', {}, opcoes),        
        TaskCategoryCombo: $resource('api/taskcategorycombo/:id', {}, opcoes),
        TaskFlowCombo: $resource('api/taskflowcombo/:id', {}, opcoes),
        SprintCombo: $resource('api/sprintcombo/:id', {}, opcoes),        
        VersionCombo: $resource('api/versioncombo/:id', {}, opcoes)
	};
}]);
