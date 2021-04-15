
angular.module('app.controllers').controller('EmissoraListagemLancCCController',
    ['$scope', '$rootScope', 'ngHistoricoFiltro', 'ngSelects',  'Api', 
        function ($scope, $rootScope, ngHistoricoFiltro, ngSelects, Api) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.tipo = $rootScope.tipo;

            $scope.itensporpagina = 15;

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

            init();

            function init() {
                if (ngHistoricoFiltro.filtro)
                    ngHistoricoFiltro.filtro.exibeFiltro = false;
            }

            $scope.search = function () {
                $scope.load(0, $scope.itensporpagina);
                $scope.paginador.reiniciar();
            };

            $scope.$watch("newLanc._fkCartao", function (novo, antigo) {
                Api.EmissoraCartao.get({ id: novo }, function (data) {
                    $scope.newLanc.stFOPA = data.stCodigoFOPA;
                    $scope.loading = false;
                },
                function (response) {
                    if (response.status === 404) { toastr.error('Invalid ID', 'Erro'); }
                });
            });

            $scope.load = function (skip, take) {

                $scope.loading = true;

                if ($scope.campos.mes_inicial !== undefined &&
                    $scope.campos.ano_inicial !== undefined) {
                    var opcoes = {
                        skip: 0,
                        take: 60000,
                        nome: $scope.campos.nome,
                        mat: $scope.campos.mat,
                        mes: $scope.campos.mes_inicial,
                        ano: $scope.campos.ano_inicial,
                    };

                    Api.EmissoraLancCC.listPage(opcoes, function (data) {
                        $scope.list = data.results;
                        $scope.total = data.count;
                        $scope.loading = false;
                    });
                }
            };

            $scope.editar = function (mdl) {
                $scope.editLanc = mdl;
                $scope.modalEdit = true;                
            };

            $scope.closeModalEdit = function () {
                $scope.modalEdit = false;
            };
            
            $scope.editarLancModal = function () {
                Api.EmissoraLancCC.update({ id: 1 }, $scope.editLanc, function (data) {
                    toastr.success('Lançamento alterado!', 'Sucesso');
                    $scope.loading = false;
                    $scope.modalEdit = false;
                    $scope.search();
                }, function (response) { toastr.error(response.data.message, 'Erro'); });
            };

            $scope.remover = function (mdl) {
                $scope.delLanc = mdl;
                $scope.modalRemove = true;
            };

            $scope.closeModalRemover = function () {
                $scope.modalRemove = false;
            };       

            $scope.removerLancModal = function () {

                var opcoes = {
                    del_item: $scope.delLanc.id,                    
                };

                Api.EmissoraLancCC.listPage(opcoes, function (data) {
                    toastr.success('Lançamento removido com sucesso', 'Sucesso');
                    $scope.search();
                }, function (response) { toastr.error(response.data.message, 'Erro'); });

                $scope.modalRemove = false;
            };

            $scope.novoLanc = function () {

                $scope.newLanc = {
                };

                $scope.newLanc.nuMes = $scope.date.getMonth() + 1;
                $scope.newLanc.nuAno = $scope.date.getFullYear();
                $scope.newLanc._totParcelas = 1;
                $scope.newLanc.vrValor = '0,00';

                $scope.modalNew = true;                
            };

            $scope.closeModalNew = function () {
                $scope.modalNew = false;
                $scope.newLanc = { };
            }            

            $scope.newLancModal = function () {
                Api.EmissoraLancCC.add($scope.newLanc, function (data) {
                    toastr.success('Lançamento inserido!', 'Sucesso');
                    $scope.loading = false;
                    
                    $scope.newLanc._fkTipo = null;
                    $scope.newLanc._totParcelas = null;
                    $scope.newLanc.vrValor = null;

                    $scope.search();
                }, function (response) { toastr.error(response.data.message, 'Erro'); });
            }

        }]);
