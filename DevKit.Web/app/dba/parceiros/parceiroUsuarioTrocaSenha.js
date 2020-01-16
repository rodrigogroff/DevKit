﻿
angular.module('app.controllers').controller('ParceiroUsuarioTrocaSenhaController',
    ['$scope', '$state', '$stateParams', 'Api', 
        function ($scope, $state, $stateParams, Api) {
            $scope.loading = false;

            var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

            var invalidCheck = function (element) {
                if (element == undefined)
                    return true;
                else
                    if (element.length === 0)
                        return true;

                return false;
            };

            $scope.save = function () {

                $scope.msgErro = '';

                $scope.st_senha_atual_fail = invalidCheck($scope.viewModel.stSenha);
                $scope.st_senha_nova_fail = invalidCheck($scope.viewModel.stNova);
                $scope.st_conf_fail = invalidCheck($scope.viewModel.stConf);

                if ($scope.st_senha_atual_fail === false && $scope.st_senha_nova_fail === false && $scope.st_conf_fail === false)
                {
                    $scope.viewModel.updateCommand = "trocaSenha";

                    Api.UsuarioParceiroDBA.update({ id: id }, $scope.viewModel, function (data) {
                        toastr.success('Dados atualizados!', 'Sucesso');
                        $scope.modalConf = false;
                    },
                        function (response) {
                            toastr.error(response.data.message, 'Erro');
                        });
                }
                else {
                    $scope.msgErro = 'Verificar pendências de campos mandatórios';
                }
            };
            
            function init()
            {
                $scope.viewModel =
                    {
                        
                    };
            }

            init();

        }]);
