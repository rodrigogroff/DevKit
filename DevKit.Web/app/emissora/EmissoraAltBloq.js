﻿
angular.module('app.controllers').controller('EmissoraAltBloqueioController',
['$scope', '$rootScope', 'AuthService', '$state', '$stateParams', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, $stateParams, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;

    $scope.campos = {
        mat: '',
        nomeCartao: '',
        id: 0,
    };

    $scope.buscar = function ()
    {
        $scope.campos.id = 0;

        if ($scope.campos.mat != '')
        {
            $scope.loading = true;

            var opcoes = { matricula: $scope.campos.mat };

            Api.EmissoraCartao.listPage(opcoes, function (data)
            {
                if (data.results.length > 0)
                {
                    $scope.campos.nomeCartao = data.results[0].associado;
                    $scope.campos.id = data.results[0].id;
                    $scope.campos.situacao = data.results[0].situacao;               
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
            id: $scope.campos.id,
            modo: 'altBloq',            
        };

        Api.EmissoraCartao.update({ id: $scope.campos.id }, opcoes, function (data) {
            toastr.success('Cartão bloqueado!', 'Sucesso');
            $scope.loading = false;
        },
        function (response) {
            toastr.error(response.data.message, 'Erro');
            $scope.loading = false;
        });       
    }

}]);
