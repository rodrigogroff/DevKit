
angular.module('app.controllers').controller('EmissoraAltSenhaController',
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

        CheckPermissions();
    }

    $scope.campos = {
        mat: '',
        nomeCartao: '',
        id: 0,
        novaSenha: ''
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
    
    $scope.confirmar = function ()
    {
        $scope.senha_fail = invalidCheck($scope.campos.novaSenha);

        if (!$scope.senha_fail)
        {
            if ($scope.campos.novaSenha.length != 4)
            {
                toastr.error('Senha precisa ter 4 caracteres!', 'Erro');
                return;
            }
        }

        if (!$scope.senha_fail)
        {
            $scope.loading = true;

            var opcoes = $scope.campos.mdl;

            opcoes.updateCommand = 'altSenha';
            opcoes.anexedEntity = $scope.campos.novaSenha;

            $scope.modal = false;

            Api.Associado.update({ id: $scope.campos.id }, opcoes, function (data)
            {
                $scope.campos.mat = '';
                $scope.campos.nomeCartao = '';
                $scope.campos.novaSenha = '';
                $scope.campos.id = 0;
                $scope.modal = true;
                $scope.loading = false;
            },
            function (response) {
                toastr.error(response.data.message, 'Erro');
                $scope.loading = false;
            });
        }        
    }

    $scope.fecharModal = function () {
        $scope.modal = false;
    }

}]);
