
angular.module('app.controllers').controller('NovoLoteController',
['$scope', '$state', '$stateParams', '$rootScope', 'Api', 'ngSelects', 
function ($scope, $state, $stateParams, $rootScope, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;

    init();

    function init()
    {
        $scope.campos = {
            codigo: ''
        };

        $scope.itensporpagina = 15;

        $scope.search = function () {
            $scope.load(0, $scope.itensporpagina);
        }

        $scope.load = function (skip, take) {
            $scope.loading = true;

            var opcoes = {
                skip: skip,
                take: take,
                novoLote: true
            };

            Api.LotesGrafica.listPage(opcoes, function (data) {
                $scope.list = data.results;
                $scope.loading = false;
            });
        }

        $scope.cancelar = function ()
        {
            $state.go('lotesgrafica');
        }

        $scope.ativar = function (mdl)
        {
            mdl.selecionado = !mdl.selecionado;
        }

        $scope.criarLote = function ()
        {
            $scope.loading = true;

            var emps = '';

            for (var i = 0; i < $scope.list.length; ++i)
            {
                if ($scope.list[i].selecionado == true)
                    emps += $scope.list[i].id + ',';
            }

            if (emps != '') {
                var opcoes = {
                    criarLote: true,
                    empresas: emps,
                };

                Api.LotesGrafica.listPage(opcoes, function (data) {
                    toastr.error('Lote ' + data.codigo + ' criado com sucesso!', 'Novo lote');
                    $state.go('lotesgrafica');
                });
            }
        }

        $scope.search();
    }

}]);
