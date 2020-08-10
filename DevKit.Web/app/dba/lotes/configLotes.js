
angular.module('app.controllers').controller('ConfigLotesController',
    ['$scope',  '$state', 'Api', 'ngSelects',
        function ($scope, $state, Api, ngSelects) {
            $scope.loading = false;

            $scope.salvar = function (mdl) {

                var opt = $scope.viewModel;

                opt.op = '301';

                Api.AdmOper.listPage(opt, function (data) {
                    toastr.success('Dados alterados com sucesso', 'Sucesso');
                },
                function (response) {
                    if (response.status === 404) { toastr.error('Invalid ID', 'Erro'); }
                });

            };

            function init() {

                $scope.viewModel = {
                    
                };   

                Api.AdmOper.listPage({ op: '300' }, function (data) {                    
                    $scope.viewModel = data;
                },
                function (response) {
                    if (response.status === 404) { toastr.error('Invalid ID', 'Erro'); }
                });
            }

            init();

        }]);
