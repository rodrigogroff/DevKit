
angular.module('app.controllers').controller('ListingRelatoriosController',
    ['$scope', '$rootScope', '$state',
        function ($scope, $rootScope, $state) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;

            $scope.associados = function () {
                $state.go('relAssociados', {});
            };

        }]);
