
angular.module('app.controllers').controller('AtivaCartaoController',
    ['$scope', 'Api', 
        function ($scope, Api) {

            $scope.loading = false;

            $scope.$watch("viewModel.cartao", function (novo, anterior)
            {
                var indice = novo.indexOf("826766");

                if (novo.length - indice >= 27)
                {
                    $scope.loading = true;

                    Api.AdmOper.listPage({ op: '20', cartao: $scope.viewModel.cartao }, function (data) {
                        toastr.success('Cartão ativado com sucesso!', 'Sistema');
                        $scope.viewModel.cartao = '';
                        $scope.loading = false;
                    },
                    function (response) {
                        toastr.error(response.data.message, 'Sistema');
                        $scope.loading = false;
                    });
                }

            }, true);
            
            function init()
            {
                $scope.viewModel =
                    {
                        cartao: ''
                    };                                
            }

            init();

        }]);
