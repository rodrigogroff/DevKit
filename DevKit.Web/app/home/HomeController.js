
angular.module('app.controllers').controller('HomeController',
    ['$window', '$scope', '$rootScope', '$state', 'Api', 
        function ($window, $scope, $rootScope, $state, Api) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;

            var w = angular.element($window);

            $scope.$watch(function () { return $window.innerWidth; },
                function (value) { $scope.width = $window.innerWidth + ", " + $window.innerHeight; }, true);

            w.bind('resize', function () { $scope.$apply(); });

            $scope.viewModel = undefined;

            init();

            function init() {
                $scope.loading = true;

                Api.HomeView.listPage({}, function (data) {
                    $scope.viewModel = data;
                    $scope.loading = false;
                });
            }

            $scope.showTask = function (mdl) {
                $state.go('task', { id: mdl.id });
            };

            $scope.showQuestion = function (mdl) {
                $state.go('task', { id: mdl.fkTask });
            };

            $scope.markAsRead = function (mdl) {
                mdl.updateCommand = 'maskAsRead';

                Api.News.update({ id: mdl.id }, mdl, function (data) {
                    init();
                },
                    function (response) {
                        toastr.error(response.data.message, 'Erro');
                    });
            };

            $scope.currentOption = undefined;

            $scope.surveySelectOption = function (option) {
                $scope.currentOption = option.id;
            };

            $scope.confirmChoiceSurvey = function (mdl) {
                mdl.updateCommand = 'optionSelect';
                mdl.anexedEntity = { id: $scope.currentOption };

                Api.Survey.update({ id: mdl.id }, mdl, function (data) {
                    init();
                    $scope.currentOption = undefined;
                },
                    function (response) {
                        toastr.error(response.data.message, 'Erro');
                    });
            };

        }]);
