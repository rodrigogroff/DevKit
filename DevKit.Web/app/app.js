'use strict';

var app = angular.module('app', ['ui.bootstrap', 'chieffancypants.loadingBar', 'ngAnimate', 'ui.router', 'angularSpinner', 'perfect_scrollbar', 'ngSanitize', 'ng-currency', 'ui.select', 'ui.select2', 'pasvaz.bindonce', 'app.filters', 'app.services', 'app.directives', 'app.controllers', 'ui.sortable', 'ui.keypress', 'xtForm', 'ui.tree', 'nvd3ChartDirectives', 'angularTreeview', 'ui.mask', 'ngCsv'])

.config(['$stateProvider', '$locationProvider', function ($stateProvider, $locationProvider)
{
    $stateProvider

    .state('home', { url: '/', templateUrl: 'app/home/home.html' })
	.state('login', { url: '/login', templateUrl: 'app/_shared/login/login.html', controller: 'LoginController', data: {} })

	.state('setup', { url: '/system/setup/:id', templateUrl: 'app/system/setup/setup.html', controller: 'SetupController' })

    .state('users', { url: '/system/user', templateUrl: 'app/system/user/listingUsers.html', controller: 'ListingUsersController' })
    .state('user-new', { url: '/system/user/new', templateUrl: 'app/system/user/user.html', controller: 'UserController' })
    .state('user', { url: '/system/user/:id', templateUrl: 'app/system/user/user.html', controller: 'UserController' })

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

	.state('otherwise', { url: '*path', templateUrl: 'app/_shared/404.html', controller: 'Erro404Controller' });

    $locationProvider.html5Mode(true);

}]);
 
angular.module('app').config(function ($httpProvider) {

	$httpProvider.interceptors.push('AuthInterceptorService');

	if (!$httpProvider.defaults.headers.get) {
		$httpProvider.defaults.headers.get = {};
	}

	$httpProvider.defaults.headers.get['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
	$httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache no-store';
	$httpProvider.defaults.headers.get['Pragma'] = 'no-cache no-store';

});
