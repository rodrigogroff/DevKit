
angular.module('app.controllers').controller('EmissoraBaixaCCController',
    ['$scope', '$rootScope', 'ngHistoricoFiltro', 'ngSelects', 'AuthService', 'Api',
        function ($scope, $rootScope, ngHistoricoFiltro, ngSelects, AuthService, Api) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;

            $scope.modal = false;
            $scope.modalConf = false;
            $scope.confirmado = false;

            $scope.date = new Date();

            $scope.campos = {
                mes_inicial: $scope.date.getMonth() + 1,
                ano_inicial: $scope.date.getFullYear(),
                selects: {
                    mes: ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 }),
                    despesa: ngSelects.obterConfiguracao(Api.Despesa, { tamanhoPagina: 15 }),
                    cartao: ngSelects.obterConfiguracao(Api.CartaoCombo, { tamanhoPagina: 15 }),
                }
            };

            $scope.baixaManual = function () {

                var mdl = {
                    idCartao: $scope.campos.fkCartao,
                    valor: $scope.campos.valor,
                    ano: $scope.campos.ano_inicial,
                    mes: $scope.campos.mes_inicial,
                };

                Api.EmissoraBaixaCC.add(mdl, function (data) {
                    toastr.success('Despesa salva!', 'Sucesso');
                    $scope.campos.valor = null;
                },
                    function (response) {
                        toastr.error(response.data.message, 'Erro');
                        $scope.loading = false;
                    });
            };

            $scope.importar = function () {
                var input = document.querySelector('input[type="file"]')
                const files = input.files
                const formData = new FormData()
                formData.append('myFile', files[0])

                fetch('/baixacc', {
                    method: 'POST',
                    body: formData,
                })
            };

        }]);
