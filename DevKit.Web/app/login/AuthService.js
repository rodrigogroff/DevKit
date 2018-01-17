'use strict';

angular.module('app.services').factory('AuthService',
['$http', '$q',
function ($http, $q)
{
    var serviceBase = '/';
    var authServiceFactory = {};

    var _authentication = {
        isAuth: false,
        nameUser: '',
        nuEmpresa: null,
        tipo: ''
    };

    var _login = function (loginData)
    {
        var data = "grant_type=password&username=" + encodeURIComponent(loginData.userName) + "&password=" + encodeURIComponent(loginData.password);

        var deferred = $q.defer();

        $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response)
        {
            localStorage.setItem('authorizationData',
                JSON.stringify({
                    token: response.access_token,
                    nameUser: response.nameUser,
                    nuEmpresa: response.nuEmpresa,
                    tipo: response.tipo
                }));

            _authentication.isAuth = true;
            _authentication.nameUser = response.nameUser;
            _authentication.nuEmpresa = response.nuEmpresa;
            _authentication.tipo = response.tipo;

            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;
    };

    var _logOut = function ()
    {
        localStorage.removeItem('authorizationData');
        
        _authentication.isAuth = false;
        _authentication.nameUser = null;
        _authentication.nuEmpresa = null;
        _authentication.tipo = null;
    };

    var _fillAuthData = function ()
    {
        var authData = JSON.parse(localStorage.getItem('authorizationData'));
        if (authData)
        {
            _authentication.isAuth = true;
            _authentication.nameUser = authData.nameUser;
            _authentication.nuEmpresa = authData.nuEmpresa;
            _authentication.tipo = authData.tipo;
        }
    }
	
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;
    
    return authServiceFactory;

}]);
