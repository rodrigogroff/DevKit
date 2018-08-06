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

            Api.AdmOper.listPage({
                op: '3',
                id_emp: $scope.campos.idEmpresa,
            },
                function (data) {

                    if (data.nuCartoes != undefined)
                        $scope.msgEmpresa = "Foram encontrados " + data.nuCartoes + " cartões";
                    else
                        $scope.msgEmpresa = "Nenhum cartão encontrado!";

                $scope.loading = false;
            });
        }            
    });

    $scope.executar = function ()
    {
        $scope.loading = true;

        Api.AdmOper.listPage({
            op: '4',
            id_emp: $scope.campos.idEmpresa,            
        },
        function (data) {
            toastr.success('Cartões ativados com sucesso!', 'Sistema');
            $scope.loading = false;
        });
    }

}]);
