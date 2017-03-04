/// <reference path="_references.js" />
/// <reference path="controllers.js" />

'use strict';

describe('Controllers: HomeCtrl', function () {
    var $scope, ctrl;

    beforeEach(module('app.controllers'));

    beforeEach(inject(function ($rootScope, $controller) {
        $scope = $rootScope.$new();
        ctrl = $controller('HomeCtrl', { $scope: $scope });
    }));

    it('should set a page title', function () {
        expect($scope.$root.title).toBe('AngularJS SPA Template for Visual Studio');
    });
});

describe('Controllers: AboutCtrl', function () {
    var $scope, ctrl;

    beforeEach(module('app.controllers'));

    beforeEach(inject(function ($rootScope, $controller) {
        $scope = $rootScope.$new();
        ctrl = $controller('AboutCtrl', { $scope: $scope });
    }));

    it('should set a page title', function () {
        expect($scope.$root.title).toBe('AngularJS SPA | About');
    });
});

describe('Controllers: LoginCtrl', function () {
    var $scope, ctrl;

    beforeEach(module('app.controllers'));

    beforeEach(inject(function ($rootScope, $controller) {
        $scope = $rootScope.$new();
        ctrl = $controller('LoginCtrl', { $scope: $scope });
    }));

    it('should set a page title', function () {
        expect($scope.$root.title).toBe('AngularJS SPA | Sign In');
    });
});

describe('Controllers: Erro404Controller', function () {
    var $scope, ctrl;

    beforeEach(module('app.controllers'));

    beforeEach(inject(function ($rootScope, $controller) {
        $scope = $rootScope.$new();
        ctrl = $controller('Erro404Controller', { $scope: $scope });
    }));

    it('should set a page title', function () {
        expect($scope.$root.title).toBe('Error 404: Page Not Found');
    });
});