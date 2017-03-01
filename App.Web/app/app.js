'use strict';

var app = angular.module('app', ['ui.bootstrap', 'chieffancypants.loadingBar', 'ngAnimate', 'ui.router', 'angularSpinner', 'perfect_scrollbar', 'ngSanitize', 'ng-currency', 'ui.select', 'ui.select2', 'pasvaz.bindonce', 'app.filters', 'app.services', 'app.directives', 'app.controllers', 'ui.sortable', 'ui.keypress', 'xtForm', 'uploadModule', 'ui.tree', 'nvd3ChartDirectives', 'angularTreeview', 'ui.mask', 'ngCsv'])

.config(['$stateProvider', '$locationProvider', function ($stateProvider, $locationProvider)
{
    $stateProvider

    .state('home', { url: '/', templateUrl: 'app/home/home.html' })
	.state('login', {
		url: '/login', templateUrl: 'app/_shared/login/login.html', controller: 'LoginController', data: { } })

    .state('usuarios', { url: '/system/usuario', templateUrl: 'app/system/usuario/listagemUsuarios.html', controller: 'ListagemUsuariosController' })
    .state('usuario-novo', { url: '/system/usuario/novo', templateUrl: 'app/system/usuario/usuario.html', controller: 'UsuarioController' })
    .state('usuario', { url: '/system/usuario/:id', templateUrl: 'app/system/usuario/usuario.html', controller: 'UsuarioController' })

	.state('perfils', { url: '/system/perfil', templateUrl: 'app/system/perfil/listagemPerfils.html', controller: 'ListagemPerfilsController' })
    .state('perfil-novo', { url: '/system/perfil/novo', templateUrl: 'app/system/perfil/perfil.html', controller: 'PerfilController' })
    .state('perfil', { url: '/system/perfil/:id', templateUrl: 'app/system/perfil/perfil.html', controller: 'PerfilController' })

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
