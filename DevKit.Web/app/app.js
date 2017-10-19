'use strict';

var app = angular.module('app', ['ui.bootstrap', 'chieffancypants.loadingBar', 'ngAnimate', 'ui.router', 'angularSpinner', 'perfect_scrollbar', 'ngSanitize', 'ng-currency', 'ui.select', 'ui.select2', 'pasvaz.bindonce', 'app.filters', 'app.services', 'app.directives', 'app.controllers', 'ui.sortable', 'ui.keypress', 'ui.tree', 'ui.mask' ])
     
.config(['$stateProvider', '$locationProvider', function ($stateProvider, $locationProvider)
{
    $stateProvider

	.state('login', { url: '/login', templateUrl: 'app/login/login.html', controller: 'LoginController', data: {} })
    .state('home', { url: '/', templateUrl: 'app/home/home.html', controller: 'HomeController', data: {} })
    .state('cache', { url: '/cache', templateUrl: 'app/home/cache.html', controller: 'CacheController', data: {} })
        
    .state('associado', { url: '/system/associado', templateUrl: 'app/system/associado/listingAssociados.html', controller: 'ListingAssociadosController' })
    .state('venda', { url: '/system/venda', templateUrl: 'app/system/venda/venda.html', controller: 'VendaController' })
    .state('cancelamento', { url: '/system/cancelamento', templateUrl: 'app/system/cancelamento/cancelamento.html', controller: 'CancelamentoController' })

    .state('relAssociados', { url: '/system/relAssociados', templateUrl: 'app/system/relatorios/listingRelAssociados.html', controller: 'ListingRelAssociadosController' })
    .state('relLojistaTrans', { url: '/system/relLojistaTrans', templateUrl: 'app/system/relatorios/listingRelLojistaTrans.html', controller: 'ListingLojistaTransController' })

    .state('mensagens', { url: '/system/lojasMensagens', templateUrl: 'app/system/loja/lojaMensagens.html', controller: 'LojaMensagensController' })
    .state('lojas', { url: '/system/lojas', templateUrl: 'app/system/loja/listingLojas.html', controller: 'ListingLojasController' })
    .state('loja-new', { url: '/system/loja/new', templateUrl: 'app/system/loja/loja.html', controller: 'LojaController' })
    .state('loja', { url: '/system/loja/:id', templateUrl: 'app/system/loja/loja.html', controller: 'LojaController' })

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
