
angular.module('app.controllers').controller('EmissoraNovoCartaoController',
    ['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
        function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            
        }]);
