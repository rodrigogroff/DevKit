angular.module('app.controllers').controller('AdmOpersAtivaGraficaController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects', 
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
    $scope.loading = false;
    $scope.mostraModal = false;

    $scope.campos = {
        selects: {
            empresa: ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 }),
        }
    };

    $scope.$watch("campos.idEmpresa", function (novo, antigo)
    {
        if (novo != antigo)
        {
            $scope.loading = true;
            $scope.list = undefined;

            Api.AdmOper.listPage({
                op: '3',
                id_emp: $scope.campos.idEmpresa,
            },
                function (data) {

                    if (data.nuCartoes != undefined)
                        $scope.msgEmpresa = "Foram encontrados " + data.nuCartoes + " cartões";
                    else
                        $scope.msgEmpresa = "Nenhum cartão encontrado!";

                    $scope.list = data.results;

                    for (var i = 0; i < $scope.list.length; i++) {
                        $scope.list[i].selecionado = true;
                    }

                $scope.loading = false;
            });
        }            
    });

    $scope.confirmar = function (mdl) {
        mdl.selecionado = !mdl.selecionado;
    }

    $scope.executar = function ()
    {
        $scope.loading = true;

        var ids = '';

        for (var i = 0; i < $scope.list.length; i++) {
            if ($scope.list[i].selecionado == true)
                ids += $scope.list[i].id + ',';
        }
 
        Api.AdmOper.listPage({
            op: '4',
            id_emp: $scope.campos.idEmpresa,   
            ids: ids,
        },
        function (data) {
            toastr.success('Cartões ativados com sucesso!', 'Sistema');
            $scope.loading = false;
        });
    }

}]);
