'use strict';

angular.module('app.controllers').controller('SetupController',
    ['$scope', '$rootScope', '$state', 'Api',
        function ($scope, $rootScope, $state, Api) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;

            $scope.permModel = {};
            $scope.permID = 100;

            function CheckPermissions() {
                Api.Permission.get({ id: $scope.permID }, function (data) {
                    $scope.permModel = data;

                    if (!$scope.permModel.visualizar) {
                        toastr.error('Accesso negado!', 'Permissão');
                        $state.go('home', {});
                    }

                }, function (response) { });
            }

            init();

            function init() {
                CheckPermissions();

                if ($scope.loaded == undefined)
                    $scope.loading = true;

                Api.Setup.get({ id: 1 }, function (data) {
                    $scope.viewModel = data;
                    $scope.loading = false;
                    $scope.loaded = true;
                },
                    function (response) {
                        if (response.status === 404) { toastr.error('Invalid ID', 'Erro'); }
                    });
            }

            var invalidCheck = function (element) {
                if (element == undefined)
                    return true;
                else
                    if (element.length == 0)
                        return true;

                return false;
            };

            $scope.save = function () {
                if (!$scope.permModel.edicao)
                    toastr.error('Accesso negado!', 'Permissão');
                else {
                    $scope.stPhoneMask_fail = invalidCheck($scope.viewModel.stPhoneMask);
                    $scope.stDateFormat_fail = invalidCheck($scope.viewModel.stDateFormat);
                    $scope.stProtocolFormat_fail = invalidCheck($scope.viewModel.stProtocolFormat);

                    if (!$scope.stPhoneMask_fail &&
                        !$scope.stDateFormat_fail &&
                        !$scope.stProtocolFormat_fail) {
                        Api.Setup.update({ id: 1 }, $scope.viewModel, function (data) {
                            toastr.success('Configurações salvas!', 'Sucesso');
                            init();
                        },
                            function (response) {
                                toastr.error(response.data.message, 'Erro');
                            });
                    }
                }
            };

        }]);
