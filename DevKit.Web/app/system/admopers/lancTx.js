
angular.module('app.controllers').controller('AdmOpersLancTxController',
    ['$scope', 'Api', 'ngSelects',
        function ($scope, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.mostraModal = false;

            $scope.campos = {
                tipoOper: 1,
                selects: {
                    empresa: ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 }),
                }
            };

            $scope.pesquisar = function () {
                $scope.emp_fail = $scope.campos.idEmpresa == undefined;
                $scope.vlr_fail = $scope.campos.valor == undefined;
                $scope.dt_ini_fail = $scope.campos.dtInicial == undefined;
                $scope.dt_fim_fail = $scope.campos.dtFinal == undefined;

                if ($scope.emp_fail == false &&
                    $scope.dt_ini_fail == false &&
                    $scope.dt_fim_fail == false &&
                    $scope.vlr_fail == false) {
                    $scope.loading = true;

                    Api.AdmOper.listPage({
                        op: '10',
                        id_emp: $scope.campos.idEmpresa,
                        dtInicial: $scope.campos.dtInicial,
                        dtFinal: $scope.campos.dtFinal,
                        valor: $scope.campos.valor
                    },
                        function (data) {
                            $scope.list = data.results;
                            $scope.loading = false;
                        });
                }
            };

            $scope.cancelaModal = function () {
                $scope.mostraModal = false;
            };

            $scope.editar = function (mdl) {
                $scope.mostraModal = true;
                $scope.lanc = mdl;
            };

            $scope.salvar = function () {
                $scope.mostraModal = false;
            };

            $scope.lancar = function () {
                $scope.mostraModalConf = true;
            };

            $scope.nao = function () {
                $scope.mostraModalConf = false;
            };

            $scope.sim = function () {
                $scope.mostraModalConf = false;

                toastr.warning('Aguarde.... processando autorizações!', 'Sistema');

                var lst = '';

                for (var i = 0; i < $scope.list.length; i++) {
                    var v = $scope.list[i];

                    lst += v.fkCartao + '|' + v.valor + ';';
                }

                Api.AdmOper.listPage({
                    op: '11',
                    id_emp: $scope.campos.idEmpresa,
                    lista: lst
                },
                    function (data) {
                        toastr.success('Lançamentos efetuados!', 'Sistema');
                    });
            };

        }]);
