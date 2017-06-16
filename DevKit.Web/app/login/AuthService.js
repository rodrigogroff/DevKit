'use strict';
angular.module('app.services').factory('AuthService',
['$http', '$q',
function ($http, $q)
{
    var serviceBase = '/';
    var authServiceFactory = {};

    var _authentication = {
        isAuth: false,
        nameUser: null,
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
                }));

            _authentication.isAuth = true;
            _authentication.nameUser = response.nameUser;

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
        _authentication.idProfile = null;
    };

    var _fillAuthData = function ()
    {
        var authData = JSON.parse(localStorage.getItem('authorizationData'));
        if (authData)
        {
            _authentication.isAuth = true;
            _authentication.nameUser = authData.nameUser;
        }
    }
	
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;
    
    return authServiceFactory;

}]);
