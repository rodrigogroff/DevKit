'use strict';

var app = angular.module('app', ['ui.bootstrap', 'chieffancypants.loadingBar', 'ngAnimate', 'ui.router', 'angularSpinner', 'perfect_scrollbar', 'ngSanitize', 'ng-currency', 'ui.select', 'ui.select2', 'pasvaz.bindonce', 'app.filters', 'app.services', 'app.directives', 'app.controllers', 'ui.sortable', 'ui.keypress', 'ui.tree', 'ui.mask' ])
     
.config(['$stateProvider', '$locationProvider', function ($stateProvider, $locationProvider)
{
    $stateProvider

	.state('login', { url: '/login', templateUrl: 'app/login/login.html', controller: 'LoginController', data: {} })
    .state('home', { url: '/', templateUrl: 'app/home/home.html', controller: 'HomeController', data: {} })
    .state('cache', { url: '/cache', templateUrl: 'app/home/cache.html', controller: 'CacheController', data: {} })
        
    //.state('users', { url: '/system/user', templateUrl: 'app/system/user/listingUsers.html', controller: 'ListingUsersController' })
    //.state('user-new', { url: '/system/user/new', templateUrl: 'app/system/user/user.html', controller: 'UserController' })
    //.state('user', { url: '/system/user/:id', templateUrl: 'app/system/user/user.html', controller: 'UserController' })
	//.state('userChangePass', { url: '/system/userChangePass/:id', templateUrl: 'app/system/user/userPassword.html', controller: 'UserPasswordController' })
        
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
