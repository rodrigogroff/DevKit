
angular.module('app.controllers').controller('EmissoraAutorizacaoCancController',
['$scope', '$rootScope', 'AuthService', '$state', '$stateParams', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, $stateParams, ngHistoricoFiltro, Api, ngSelects)
{
    var invalidCheck = function (element) {
        if (element == undefined)
            return true;
        else
            if (element.length == 0)
                return true;

        return false;
    }

    function CheckPermissions() {
        Api.Permission.get({ id: $scope.permID }, function (data) {
            $scope.permModel = data;

            if (!$scope.permModel.edicao) {
                toastr.error('Acesso negado para alteração de senha!', 'Permissão');
                $state.go('home');
            }
        },
            function (response) { });
    }

    $rootScope.exibirMenu = true;
    $scope.loading = false;
    $scope.modal = false;

    init();

    function init()
    {        
        $scope.permModel = {};
        $scope.permID = 401;

        $scope.selectSecao = ngSelects.obterConfiguracao(Api.EmpresaSecaoCombo, {});

        //CheckPermissions();
    }

    $scope.campos = {
        
    };

    $scope.buscar = function ()
    {
        $scope.campos.id = 0;

        $scope.mat_fail = invalidCheck($scope.campos.mat);
        $scope.secao_fail = $scope.campos.fkSecao == undefined;

        if (!$scope.mat_fail && !$scope.secao_fail)
        {
            $scope.loading = true;

            var opcoes = {
                fkSecao: $scope.campos.fkSecao,
                matricula: $scope.campos.mat,
                titularidade: '1',
            };

            Api.Associado.listPage(opcoes, function (data)
            {
                if (data.results.length > 0) {
                    $scope.campos.mdl = data.results[0];
                    $scope.campos.nomeCartao = data.results[0].stName;
                    $scope.campos.id = data.results[0].id;                    
                }
                else
                    toastr.error('matrícula inválida', 'Erro');

                $scope.loading = false;
            },
            function (response) {
                toastr.error(response.data.message, 'Erro');
                $scope.loading = false;
            });
        }        
    }

    $scope.buscarCred = function ()
    {
        $scope.campos.idCred = 0;

        $scope.cred_fail = invalidCheck($scope.campos.cred);
        $scope.secao_fail = $scope.campos.fkSecao == undefined;

        if ($scope.cred_fail == false && $scope.secao_fail == false)
        {
            $scope.loading = true;

            var opcoes = {
                skip: 0,
                take: 1,
                codigo: $scope.campos.cred,
                fkSecao: $scope.campos.fkSecao,
            };

            Api.EmissorListagemCredenciado.listPage(opcoes, function (data)
            {
                if (data.results.length > 0)
                {
                    $scope.campos.nomeCredenciado = data.results[0].stNome;
                    $scope.campos.idCred = data.results[0].id;
                }
                else
                    toastr.error('código de credenciado inválido', 'Erro');
                
                $scope.loading = false;
            });
        }
    }

    $scope.cancelar = function ()
    {
        $scope.secao_fail = $scope.campos.fkSecao == undefined;
        $scope.mat_fail = invalidCheck($scope.campos.nomeCartao);
        $scope.cred_fail = invalidCheck($scope.campos.nomeCredenciado);
        $scope.dt_fail = invalidCheck($scope.campos.dt);
        $scope.nsu_fail = invalidCheck($scope.campos.nsu);

        if ($scope.secao_fail == false &&
            $scope.mat_fail == false &&
            $scope.cred_fail == false &&
            $scope.dt_fail == false &&
            $scope.nsu_fail == false)
        {
            $scope.loading = true;

            var opcoes = {
                skip: 0,
                take: 1,
                codCredenciado: $scope.campos.cred,
                matricula: $scope.campos.mat,
                fkSecao: $scope.campos.fkSecao,
                dtInicial: $scope.campos.dt,
                dtFim: $scope.campos.dt,
                nsu: $scope.campos.nsu,
            };

            Api.EmissorListagemAutorizacao.listPage(opcoes, function (data) {
                if (data.results.length > 0) {
                    $scope.modal = true;
                }
                else
                    toastr.error('Valores inválidos!', 'Erro');

                $scope.loading = false;
            });
        }        
    }
    
    $scope.confirmar = function ()
    {
        $scope.loading = true;

        var opcoes = {
            codCredenciado: $scope.campos.cred,
            matricula: $scope.campos.mat,
            fkSecao: $scope.campos.fkSecao,
            dt: $scope.campos.dt,
            nsu: $scope.campos.nsu,
        };

        Api.EmissorCancelamento.listPage(opcoes, function (data) {
            $scope.loading = false;
            $scope.campos = {};
            $scope.modal = false; 

            toastr.success('Cancelamento efetuado com sucesso!', 'Sistema');
        });
    }

    $scope.fecharModal = function () {
        $scope.modal = false;
    }

}]);
