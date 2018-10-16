
angular.module('app.controllers').controller('DashboardController',
    ['$scope', '$rootScope', 'Api', 
        function ($scope, $rootScope, Api) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;

            function init()
            {
                Api.AdmOper.listPage({ op: '100' }, function (data) {
                    $scope.a = data.a;
                    $scope.b = data.b;
                    $scope.c = data.c;
                    $scope.d = data.d;
                    $scope.e = data.e;
                    $scope.f = data.f;
                    $scope.g = data.g;
                });
            }

            $scope.refresh = function () {
                init();
            };

            init();

        }]);
