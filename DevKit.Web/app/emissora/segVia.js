
angular.module('app.controllers').controller('EmissoraSegViaController',
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
                toastr.error('Acesso negado para segunda via!', 'Permissão');
                $state.go('home');
            }
        },
            function (response) { });
    }

    $rootScope.exibirMenu = true;
    $scope.loading = false;

    $scope.modal = false;

    $scope.campos = {
        mat: '',
        nomeCartao: '',
        id: 0,
        novaSenha: ''
    };

    init();

    function init() {
        $scope.permModel = {};
        $scope.permID = 402;

        CheckPermissions();
    }

    $scope.ativar = function (mdl)
    {
        mdl.selecionado = !mdl.selecionado;

        if (mdl.selecionado == true)
        {
            $scope.campos.idSel = mdl.id;
            for (var i = 0; i < $scope.list.length; i++)
            {
                if ($scope.list[i].id != mdl.id)
                    $scope.list[i].selecionado = false;
            }
        }            
    }

    $scope.buscar = function ()
    {
        $scope.campos.id = 0;

        $scope.mat_fail = invalidCheck($scope.campos.mat);

        if (!$scope.mat_fail)
        {
            $scope.loading = true;

            var opcoes = {
                matricula: $scope.campos.mat
            };

            Api.Associado.listPage(opcoes, function (data)
            {
                if (data.results.length > 0)
                {
                    $scope.list = data.results;    
                    for (var i = 0; i < length; i++) {
                        $scope.list[i].selecionado = false;
                    }
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
        $scope.loading = true;

        var opcoes = {
            id: $scope.campos.idSel,
            updateCommand: 'novaVia',
            anexedEntity: $scope.campos.novaSenha
        };

        $scope.modal = false;

        Api.Associado.update({ id: $scope.campos.idSel }, opcoes, function (data)
        {
            $scope.modal = true;
            $scope.loading = false;
        },
        function (response) {
            toastr.error(response.data.message, 'Erro');
            $scope.loading = false;
        });
    }

    $scope.fecharModal = function () {
        $scope.modal = false;
    }

}]);
