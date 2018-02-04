'use strict';

var app = angular.module('app', ['ui.bootstrap', 'chieffancypants.loadingBar', 'ngAnimate', 'ui.router', 'angularSpinner', 'perfect_scrollbar', 'ngSanitize', 'ng-currency', 'ui.select', 'ui.select2', 'pasvaz.bindonce', 'app.filters', 'app.services', 'app.directives', 'app.controllers', 'ui.sortable', 'ui.keypress', 'ui.tree', 'ui.mask' ])
     
.config(['$stateProvider', '$locationProvider', function ($stateProvider, $locationProvider)
{
    $stateProvider

	.state('login', { url: '/login', templateUrl: 'app/login/login.html', controller: 'LoginController', data: {} })
    .state('home', { url: '/', templateUrl: 'app/home/home.html', controller: 'HomeController', data: {} })

    .state('cache', { url: '/cache', templateUrl: 'app/home/cache.html', controller: 'CacheController', data: {} })

	.state('setup', { url: '/system/setup/:id', templateUrl: 'app/system/setup/setup.html', controller: 'SetupController' })

    .state('users', { url: '/system/user', templateUrl: 'app/system/user/listingUsers.html', controller: 'ListingUsersController' })
    .state('user-new', { url: '/system/user/new', templateUrl: 'app/system/user/user.html', controller: 'UserController' })
    .state('user', { url: '/system/user/:id', templateUrl: 'app/system/user/user.html', controller: 'UserController' })
	.state('userChangePass', { url: '/system/userChangePass/:id', templateUrl: 'app/system/user/userPassword.html', controller: 'UserPasswordController' })

	.state('profiles', { url: '/system/profiles', templateUrl: 'app/system/profile/listingProfiles.html', controller: 'ListingProfilesController' })
    .state('profile-new', { url: '/system/profile/new', templateUrl: 'app/system/profile/profile.html', controller: 'ProfileController' })
    .state('profile', { url: '/system/profile/:id', templateUrl: 'app/system/profile/profile.html', controller: 'ProfileController' })

	.state('projects', { url: '/configuration/projects', templateUrl: 'app/configuration/project/listingProjects.html', controller: 'ListingProjectsController' })
    .state('project-new', { url: '/configuration/project/new', templateUrl: 'app/configuration/project/project.html', controller: 'ProjectController' })
    .state('project', { url: '/configuration/project/:id', templateUrl: 'app/configuration/project/project.html', controller: 'ProjectController' })

	.state('sprints', { url: '/configuration/sprints', templateUrl: 'app/configuration/sprint/listingSprints.html', controller: 'ListingSprintsController' })
    .state('sprint-new', { url: '/configuration/sprint/new', templateUrl: 'app/configuration/sprint/sprint.html', controller: 'SprintController' })
    .state('sprint', { url: '/configuration/sprint/:id', templateUrl: 'app/configuration/sprint/sprint.html', controller: 'SprintController' })

	.state('taskTypes', { url: '/configuration/taskTypes', templateUrl: 'app/configuration/taskType/listingTaskTypes.html', controller: 'ListingTaskTypesController' })
    .state('taskType-new', { url: '/configuration/taskType/new', templateUrl: 'app/configuration/taskType/taskType.html', controller: 'TaskTypeController' })
    .state('taskType', { url: '/configuration/taskType/:id', templateUrl: 'app/configuration/taskType/taskType.html', controller: 'TaskTypeController' })

	.state('tasks', { url: '/task/tasks', templateUrl: 'app/task/task/listingTasks.html', controller: 'ListingTasksController' })
    .state('task-new', { url: '/task/task/new', templateUrl: 'app/task/task/task.html', controller: 'TaskController' })
    .state('task', { url: '/task/task/:id', templateUrl: 'app/task/task/task.html', controller: 'TaskController' })

	.state('userKanban', { url: '/task/kanban', templateUrl: 'app/task/kanban/listingUserKanban.html', controller: 'ListingUserKanbanController' })
	.state('management', { url: '/task/management', templateUrl: 'app/task/management/management.html', controller: 'ManagementController' })
	.state('timesheet', { url: '/task/timesheet', templateUrl: 'app/task/timesheet/timesheet.html', controller: 'TimesheetController' })

	.state('newsListing', { url: '/configuration/newsListing', templateUrl: 'app/configuration/news/listingNews.html', controller: 'ListingNewsController' })
    .state('news-new', { url: '/configuration/news/new', templateUrl: 'app/configuration/news/news.html', controller: 'NewsController' })
    .state('news', { url: '/configuration/news/:id', templateUrl: 'app/configuration/news/news.html', controller: 'NewsController' })

	.state('surveysListing', { url: '/configuration/surveysListing', templateUrl: 'app/configuration/surveys/listingSurveys.html', controller: 'ListingSurveysController' })
    .state('survey-new', { url: '/configuration/survey/new', templateUrl: 'app/configuration/surveys/survey.html', controller: 'SurveyController' })
    .state('survey', { url: '/configuration/survey/:id', templateUrl: 'app/configuration/surveys/survey.html', controller: 'SurveyController' })

	.state('clients', { url: '/configuration/clients', templateUrl: 'app/configuration/client/listingClients.html', controller: 'ListingClientsController' })
    .state('client-new', { url: '/configuration/client/new', templateUrl: 'app/configuration/client/client.html', controller: 'ClientController' })
    .state('client', { url: '/configuration/client/:id', templateUrl: 'app/configuration/client/client.html', controller: 'ClientController' })

	.state('clientgroups', { url: '/configuration/clientgroups', templateUrl: 'app/configuration/clientgroup/listingClientGroups.html', controller: 'ListingClientGroupsController' })
    .state('clientgroup-new', { url: '/configuration/clientgroup/new', templateUrl: 'app/configuration/clientgroup/clientGroup.html', controller: 'ClientGroupController' })
    .state('clientgroup', { url: '/configuration/clientgroup/:id', templateUrl: 'app/configuration/clientgroup/clientGroup.html', controller: 'ClientGroupController' })

    .state('persons', { url: '/emissora/associados', templateUrl: 'app/emissora/listagemAssociados.html', controller: 'ListagemAssociadosController' })
    .state('person', { url: '/emissora/associados/:id', templateUrl: 'app/emissora/associado.html', controller: 'AssociadoController' })
    .state('person-new', { url: '/emissora/associados/new', templateUrl: 'app/emissora/associado.html', controller: 'AssociadoController' })

    .state('lotesgrafica', { url: '/dba/lotes', templateUrl: 'app/dba/listingLotes.html', controller: 'ListingLotesController' })
    .state('novolote', { url: '/dba/lotes/novo', templateUrl: 'app/dba/novoLote.html', controller: 'NovoLoteController' })

    .state('empresas', { url: '/dba/empresas', templateUrl: 'app/dba/listingEmpresas.html', controller: 'ListingEmpresasController' })
    .state('medicos', { url: '/dba/medicos', templateUrl: 'app/dba/listingMedicos.html', controller: 'ListingMedicosController' })
        
    .state('medico',
    {
        url: '/dba/medico/:id',
        templateUrl: 'app/dba/medico.html',
        controller: 'MedicoController'
    })

    .state('tuss', { url: '/dba/tuss', templateUrl: 'app/dba/listingTUSS.html', controller: 'ListingTUSSController' })
    .state('especialidades', { url: '/dba/especialidades', templateUrl: 'app/dba/listingEspecialidades.html', controller: 'ListingEspecialidadesController' })

    .state('altSenha', { url: '/emissora/altSenha', templateUrl: 'app/emissora/altSenha.html', controller: 'EmissoraAltSenhaController' })
    .state('emissorprocs', { url: '/emissora/listprocs', templateUrl: 'app/emissora/listagemEmissorProcedimentos.html', controller: 'ListagemEmissorProcedimentosController' })

    .state('medicopass', { url: '/medico/medicoTrocaSenha', templateUrl: 'app/medico/medicoPassword.html', controller: 'MedicoPasswordController' })    
    .state('autorizacaoProc', { url: '/medico/autorizacaoProc', templateUrl: 'app/medico/autorizacaoProc.html', controller: 'AutorizacaoProcController' })
    .state('medicoprocs', { url: '/medico/procs', templateUrl: 'app/medico/listagemMedicoProcedimentos.html', controller: 'ListagemMedicoProcedimentosController' })

    .state('listemissormedicos', { url: '/emissora/medicos', templateUrl: 'app/emissora/listagemMedicos.html', controller: 'ListagemEmissorMedicosController' })

    .state('emissormedico',
    {
        url: '/emissora/medico/:id',
        templateUrl: 'app/emissora/medico.html',
        controller: 'EmissorMedicoController'
    })

    .state('otherwise', { url: '*path', templateUrl: 'app/_shared/404.html', controller: 'Erro404Controller' });

    $locationProvider.html5Mode(true);

}]);
 
angular.module('app').config(function ($httpProvider) {

    $httpProvider.defaults.useXDomain = true;

	$httpProvider.interceptors.push('AuthInterceptorService');

	if (!$httpProvider.defaults.headers.get) {
		$httpProvider.defaults.headers.get = {};
	}

	$httpProvider.defaults.headers.get['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
	$httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache no-store';
	$httpProvider.defaults.headers.get['Pragma'] = 'no-cache no-store';

});
