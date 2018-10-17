
angular.module('app.controllers').controller('EmissoraAltSenhaUsuarioController',
    ['$scope', '$rootScope', 'Api',
        function ($scope, $rootScope, Api) {

            var invalidCheck = function (element) {
                if (element == undefined)
                    return true;
                else
                    if (element.length == 0)
                        return true;

                return false;
            };

            $rootScope.exibirMenu = true;
            $scope.loading = false;

            $scope.modal = false;

            $scope.campos = {
                nome: '',
                id: 0,
                novaSenha: ''
            };

            $scope.buscar = function () {
                $scope.campos.id = 0;

                $scope.nome_fail = invalidCheck($scope.campos.nome);

                if (!$scope.nome_fail) {
                    $scope.loading = true;

                    var opcoes = { nome: $scope.campos.nome };

                    Api.EmissoraUsuario.listPage(opcoes, function (data) {
                        if (data.results.length > 0) {
                            $scope.campos.nome = data.results[0].nome;
                            $scope.campos.id = data.results[0].id;
                        }
                        else
                            toastr.error('Nome de usuário inválido', 'Erro');

                        $scope.loading = false;
                    },
                        function (response) {
                            toastr.error(response.data.message, 'Erro');
                            $scope.loading = false;
                        });
                }
            };

            $scope.confirmar = function () {
                $scope.senha_fail = invalidCheck($scope.campos.novaSenha);

                if (!$scope.senha_fail) {
                    if ($scope.campos.novaSenha.length != 4) {
                        toastr.error('Senha precisa ter 4 caracteres!', 'Erro');
                        return;
                    }
                }

                if (!$scope.senha_fail) {
                    $scope.loading = true;

                    var opcoes = {
                        id: $scope.campos.id,
                        modo: 'altSenhaUsuario',
                        valor: $scope.campos.novaSenha
                    };

                    $scope.modal = false;

                    Api.EmissoraUsuario.update({ id: $scope.campos.id }, opcoes, function (data) {
                        $scope.campos.nome = '';
                        $scope.campos.novaSenha = '';
                        $scope.campos.id = 0;
                        $scope.modal = true;
                        $scope.loading = false;
                    },
                        function (response) {
                            toastr.error(response.data.message, 'Erro');
                            $scope.loading = false;
                        });
                }
            };

            $scope.fecharModal = function () {
                $scope.modal = false;
            };

        }]);
