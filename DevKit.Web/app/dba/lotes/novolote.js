
angular.module('app.controllers').controller('NovoLoteController',
    ['$scope', '$state', '$stateParams', 'Api', 
        function ($scope, $state, $stateParams, Api) {

            $scope.loading = false;

            $scope.executar = function () {

                $scope.loading = true;

                var str_list = '';

                for (var i = 0; i < $scope.list.length; i++) {
                    if ($scope.list[i].selecionado == true) {
                        str_list += $scope.list[i].sigla + ';';
                    }
                }

                Api.AdmOper.listPage({ op: '14', list: str_list }, function (data) {

                    toastr.success('Lote criado com sucesso!', 'Sistema');
                    $state.go('lotes');
                    
                },
                function (response) {
                    if (response.status === 404) { toastr.error('Invalid ID', 'Erro'); }
                    $scope.list();
                });

            };

            $scope.confirmar = function (mdl) {
                mdl.selecionado = !mdl.selecionado;
            };

            function init()
            {
                $scope.viewModel =
                    {
                    };

                $scope.loading = true;

                Api.AdmOper.listPage({ op: '13' }, function (data)
                {
                    $scope.list = data.results;
                    $scope.loading = false;

                    if (data.nuCartoes > 0) {
                        $scope.msgEmpresa = 'Foram encontrados ' + data.nuCartoes + ' cartões';
                    }
                },
                    function (response) {
                        if (response.status === 404) { toastr.error('Invalid ID', 'Erro'); }
                        $scope.list();
                    });
            }

            init();

        }]);
