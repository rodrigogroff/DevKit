
angular.module('app.controllers').controller('ListingLotesController',
    ['$scope',  '$state', 'Api', 'ngSelects',
        function ($scope, $state, Api, ngSelects) {
            $scope.loading = false;

            $scope.search = function () {
                $scope.load(0, $scope.itensporpagina);
                $scope.paginador.reiniciar();
            };

            $scope.load = function (skip, take) {
                $scope.loading = true;

                var opcoes = {
                    skip: skip,
                    take: take,
                    todos: $scope.campos.todos,
                    idEmpresa: $scope.campos.idEmpresa,
                    matricula: $scope.campos.matricula,
                };

                Api.LoteDBA.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            };

            $scope.show = function (mdl)
            {
                $scope.modalCartoes = true;
                $scope.loading = true;

                Api.LoteDetalhesDBA.listPage({ idLote: mdl.i_unique }, function (data) {
                    $scope.listDet = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                   
                });
            };

            $scope.remover = function (mdl) {
                $scope.modalRemocao = true;
                $scope.loteAtual = mdl;
            };

            $scope.closeModalRemover = function () {
                $scope.modalRemocao = false;
            };

            $scope.removerLote = function () {
                $scope.loading = true;
                Api.AdmOper.listPage({ op: '22', lote: $scope.loteAtual.i_unique }, function (data) {
                    toastr.success('Lote removido com sucesso!', 'Sistema');
                    $scope.modalRemocao = false;                    
                    $scope.loading = false;
                    $scope.search();
                },
                function (response) {
                    $scope.loading = false;
                });
            };

            $scope.closeModalCartoes = function (mdl) {
                $scope.modalCartoes = false;              
            };

            $scope.arquivo = function (mdl) {
                window.location.href = "/api/LoteDBA/exportar?" + $.param({
                    idLote: mdl.i_unique
                });
            };

            $scope.ativar = function (mdl) {
                $scope.loading = true;
                Api.AdmOper.listPage({ op: '21', lote: mdl.i_unique }, function (data)
                {
                    toastr.success('Lote ativado com sucesso!', 'Sistema');
                    $scope.search();

                    $scope.loading = false;
                },
                function (response) {
                    $scope.loading = false;
                });
            };

            $scope.new = function () {
                $state.go('novolote');
            };

            function init() {

                $scope.selectEmpresa = ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 });

                $scope.campos = {
                    codigo: ''
                };                
            }

            init();

        }]);
