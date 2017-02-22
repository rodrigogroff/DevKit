'use strict';

var app = angular.module('app', ['ui.bootstrap', 'chieffancypants.loadingBar', 'ngAnimate', 'ui.router', 'angularSpinner', 'perfect_scrollbar', 'ngSanitize', 'ng-currency', 'ui.select', 'ui.select2', 'pasvaz.bindonce', 'app.filters', 'app.services', 'app.directives', 'app.controllers', 'ui.sortable', 'ui.keypress', 'xtForm', 'uploadModule', 'ui.tree', 'nvd3ChartDirectives', 'angularTreeview', 'ui.mask', 'ngCsv'])

.config(['$stateProvider', '$locationProvider', function ($stateProvider, $locationProvider)
{
    $stateProvider

    .state('home', { url: '/', templateUrl: 'app/home/home.html' })
	.state('login', {
		url: '/login', templateUrl: 'app/_shared/login/login.html', controller: 'LoginController', data: { } })

    .state('usuarios', { url: '/operacional/usuario', templateUrl: 'app/operacional/usuario/listagemUsuarios.html', controller: 'ListagemUsuariosController' })
    .state('usuario-novo', { url: '/operacional/usuario/novo', templateUrl: 'app/operacional/usuario/usuario.html', controller: 'UsuarioController' })
    .state('usuario', { url: '/operacional/usuario/:id', templateUrl: 'app/operacional/usuario/usuario.html', controller: 'UsuarioController' })

	.state('perfils', { url: '/operacional/perfil', templateUrl: 'app/operacional/perfil/listagemPerfils.html', controller: 'ListagemPerfilsController' })
    .state('perfil-novo', { url: '/operacional/perfil/novo', templateUrl: 'app/operacional/perfil/perfil.html', controller: 'PerfilController' })
    .state('perfil', { url: '/operacional/perfil/:id', templateUrl: 'app/operacional/perfil/perfil.html', controller: 'PerfilController' })

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
