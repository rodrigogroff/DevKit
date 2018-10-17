
angular.module('app.controllers').controller('AssociadoMenuController',
    ['$scope', '$state',
        function ($scope, $state) {
            $scope.limites = function () {
                $state.go('limitesUsrMobile', {});
            };

            $scope.extratos = function () {
                $state.go('extratosUsrMobile', {});
            };

            $scope.lojistasUsrMobile = function () {
                $state.go('lojistasUsrMobile', {});
            };

            $scope.logOut = function () {
                window.location = '/login?tipo=2';
            };

        }]);
