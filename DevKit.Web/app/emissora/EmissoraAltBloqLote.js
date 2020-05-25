
angular.module('app.controllers').controller('EmissoraAltBloqueioLoteController',
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
                {
                    var opcoes = { mat: '', modo: 'Bloqueio', };

                    for (var i = 0; i < lines.length; i++)
                        opcoes.mat += lines[i] + ';';

                    toastr.warning('Aguarde bloqueamento das matriculas...', 'aviso');

                    Api.LoteCartao.update({ id: null }, opcoes, function (data)
                    {
                        toastr.success('Matriculas bloqueadas com sucesso!', 'aviso');
                        $scope.loading = false;
                    },
                    function (response) { toastr.error(response.data.message, 'Erro'); });                    
                }
            };

        }]);
