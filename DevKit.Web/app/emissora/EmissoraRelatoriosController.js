
angular.module('app.controllers').controller('EmissoraRelatoriosController',
    ['$scope', '$rootScope', 
        function ($scope, $rootScope) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;

        }]);
