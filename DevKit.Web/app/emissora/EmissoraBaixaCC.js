
angular.module('app.controllers').controller('EmissoraBaixaCCController',
    ['$scope', '$rootScope', 'ngSelects', 'AuthService', 'Api',
        function ($scope, $rootScope, ngSelects, AuthService, Api) {

            $rootScope.exibirMenu = true;
            $scope.authentication = AuthService.authentication;
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

                if (files.length > 0) {
                    const formData = new FormData()
                    formData.append('myFile', files[0])
                    formData.append('ano', $scope.campos.ano_inicial)
                    formData.append('mes', $scope.campos.mes_inicial)
                    formData.append('empresa', $scope.authentication.IdEmpresa)

                    fetch('/baixacc', {
                        method: 'POST',
                        body: formData,
                    })

                    toastr.success('Arquivo importado com sucesso!', 'Sucesso');
                }
                else
                    toastr.error('Informe um arquivo corretamente', 'Erro');
            };

            $scope.pesquisarHistorico = function () {

                $scope.loading = true;

                var opcoes = {
                    ano: $scope.campos.ano_inicial,
                    mes: $scope.campos.mes_inicial,
                };

                Api.EmissoraBaixaCCHist.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            }

            $scope.desfazerBaixa = function (mdl) {
                $scope.modalDesfaz = true;
                $scope.desfaz = mdl;
            }

            $scope.confirmaModalDesfaz = function () {                
                $scope.loading = true;
                Api.EmissoraBaixaCCHist.add($scope.desfaz, function (data) {
                    toastr.success('Baixa automática desfeita!', 'Sucesso');
                    $scope.loading = false;                    
                    $scope.modalDesfaz = false;
                    $scope.pesquisarHistorico();
                }, function (response) { toastr.error(response.data.message, 'Erro'); });
            }

            $scope.cancelaModalDesfaz = function () {
                $scope.modalDesfaz = false;
            }            

        }]);
