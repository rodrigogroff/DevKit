
angular.module('app.controllers').controller('EmissoraExpFechamentoController',
    ['$scope', '$rootScope', 'AuthService', 'Api', 'ngSelects',
        function ($scope, $rootScope, AuthService, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.tipo = $rootScope.tipo;

            var invalidCheck = function (element) {
                if (element == undefined)
                    return true;
                else
                    if (element.length == 0)
                        return true;

                return false;
            };

            $scope.date = new Date();

            $scope.campos = {
                mes_inicial: $scope.date.getMonth() + 1,
                ano_inicial: $scope.date.getFullYear(),
                selects: {
                    mes: ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 }),
                    empresa: ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 }),
                }
            };

            $scope.search = function () {
                if ($scope.tipo == '5') {
                    $scope.emp_fail = $scope.campos.idEmpresa == undefined;
                    if ($scope.emp_fail == true)
                        return;
                }

                var t_emp = '';

                if ($scope.tipo != '5') {
                    AuthService.fillAuthData();
                    $scope.authentication = AuthService.authentication;
                    t_emp = $scope.authentication.m1;
                }

                if (invalidCheck($scope.campos.mes_inicial) ||
                    invalidCheck($scope.campos.ano_inicial)) {
                    toastr.error('Informe os filtros corretamente', 'Erro');
                    return;
                }

                if ($scope.tipo == '5')
                    t_emp = $scope.campos.idEmpresa;

                window.location.href = "/api/EmissoraFechamentoExp/exportar?" + $.param({
                    emp: t_emp,
                    mes: $scope.campos.mes_inicial,
                    ano: $scope.campos.ano_inicial,
                });
            };

        }]);
