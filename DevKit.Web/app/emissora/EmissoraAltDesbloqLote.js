
angular.module('app.controllers').controller('EmissoraAltDesbloqueioLoteController',
    ['$scope', '$rootScope', 'Api',
        function ($scope, $rootScope, Api) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;

            $scope.modal = false;

            $scope.campos = {
                mat: '',
            };

            $scope.confirmar = function ()
            {
                $scope.loading = true;

                var lines = $scope.campos.mat.split('\n');

                if (lines.length > 0)
                    for (var i = 0; i < lines.length; i++)
                    {
                        var opcoes = {
                            mat: lines[i],
                            modo: 'Desbloqueio',
                        };

                        Api.LoteCartao.update({ id: null }, opcoes, function (data) { }, function (response) { toastr.error(response.data.message, 'Erro'); });
                    }

                $scope.loading = false;               
            };

        }]);
