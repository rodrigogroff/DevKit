
angular.module('app.controllers').controller('LojaMensagensController',
    ['$scope', '$rootScope', '$state', 'Api',
        function ($scope, $rootScope, $state, Api) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.aceite = false;

            init();

            function init() {
                $scope.loading = true;

                Api.LojistaMensagens.listPage({}, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            }

            $scope.confirmar = function () {
                $state.go('venda', {});
            };

        }]);
