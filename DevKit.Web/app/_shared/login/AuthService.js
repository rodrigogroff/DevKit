'use strict';
angular.module('app.services').factory('AuthService', ['$http', '$q', function ($http, $q) {

    var serviceBase = '/';
    var authServiceFactory = {};

    var _authentication = {
        isAuth: false,
        nomeUsuario: null,
        idPerfil: null
    };


    var _login = function (loginData) {

        var data = "grant_type=password&username=" + encodeURIComponent(loginData.userName) + "&password=" + encodeURIComponent(loginData.password);

        var deferred = $q.defer();

        $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {
            localStorage.setItem('authorizationData', JSON.stringify({ token: response.access_token, nomeUsuario: response.nomeUsuario, idPerfil: response.idPerfil }));

            _authentication.isAuth = true;
            _authentication.nomeUsuario = response.nomeUsuario;
            _authentication.idPerfil = response.idPerfil;

            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    var _logOut = function () {

        localStorage.removeItem('authorizationData');

        _authentication.isAuth = false;
        _authentication.nomeUsuario = null;
        _authentication.idPerfil = null;

    };

    var _fillAuthData = function () {
        var authData = JSON.parse(localStorage.getItem('authorizationData'));
        if (authData) {
            _authentication.isAuth = true;
            _authentication.nomeUsuario = authData.nomeUsuario;
            _authentication.idPerfil = authData.idPerfil;
        }
    }

    

    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;
    

    return authServiceFactory;
}]);