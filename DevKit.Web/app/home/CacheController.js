
angular.module('app.controllers').controller('CacheController',
    ['$scope', '$rootScope', 'Api', 
        function ($scope, $rootScope, Api) {

            $rootScope.exibirMenu = true;
            $scope.viewModel = [];

            function init() {
                Api.Cache.listPage({}, function (data) {
                    $scope.viewModel = data;
                });
            }

            $scope.refresh = function () {
                init();
            };

            init();

        }]);
