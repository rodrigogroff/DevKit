
angular.module('app.controllers').controller('DashboardController',
    ['$scope', '$rootScope', 'Api', 'ngSelects',
        function ($scope, $rootScope, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;

        }]);
