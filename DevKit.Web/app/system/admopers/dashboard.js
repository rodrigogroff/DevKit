
angular.module('app.controllers').controller('DashboardController',
    ['$scope', '$rootScope', 'Api', 
        function ($scope, $rootScope, Api) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;

            function init()
            {
                Api.AdmOper.listPage({ op: '101' }, function (data) { $scope.a = data; });
                Api.AdmOper.listPage({ op: '102' }, function (data) { $scope.b = data; });
                Api.AdmOper.listPage({ op: '103' }, function (data) { $scope.c = data; });
                Api.AdmOper.listPage({ op: '104' }, function (data) { $scope.d = data; });
                Api.AdmOper.listPage({ op: '105' }, function (data) { $scope.e = data; });
                Api.AdmOper.listPage({ op: '106' }, function (data) { $scope.f = data; });
                Api.AdmOper.listPage({ op: '107' }, function (data) { $scope.g = data; });
            }

            init();

        }]);
