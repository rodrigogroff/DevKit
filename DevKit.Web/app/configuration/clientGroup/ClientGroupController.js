'use strict';

angular.module('app.controllers').controller('ClientGroupController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$rootScope.exibirMenu = true;

	$scope.loading = false;

	$scope.selectClients = ngSelects.obterConfiguracao(Api.ClientCombo, {});

	$scope.viewModel = {};
	$scope.permModel = {};	
	$scope.permID = 121;
	
	function CheckPermissions()
	{
        Api.Permission.get({ id: $scope.permID }, function (data)
		{
			$scope.permModel = data;

			if (!$scope.permModel.visualizar)
			{
				toastr.error('Acesso negado!', 'Permissão');
				$state.go('home');
			}
		},
		function (response) { });
	}
	
	var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

	init();

	function init()
	{
		CheckPermissions();

		if (id > 0)
		{
            if ($scope.loaded == undefined)
                $scope.loading = true;

            Api.ClientGroup.get({ id: id }, function (data)
			{
				$scope.viewModel = data;
                $scope.loading = false;
                $scope.loaded = true;
			},
			function (response)
			{
				if (response.status === 404) { toastr.error('Invalid ID', 'Error'); }
				$scope.list();
			});
		}
		else
			$scope.viewModel = { };
	}

	var invalidCheck = function (element) {
		if (element == undefined)
			return true;
		else
			if (element.length == 0)
				return true;

		return false;
	}

	$scope.save = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
		{
			$scope.stName_fail = invalidCheck($scope.viewModel.stName);
				
			if (!$scope.stName_fail)
            {
				if (id > 0)
				{
					$scope.viewModel.updateCommand = "entity";

					Api.ClientGroup.update({ id: id }, $scope.viewModel, function (data)
					{
						toastr.success('Grupo de clientes salvo!', 'Sucesso');
						$scope.viewModel.logs = data.logs;
					},
					function (response)
					{
						toastr.error(response.data.message, 'Error');
					});
				}
				else
				{
					Api.ClientGroup.add($scope.viewModel, function (data)
					{
						toastr.success('Grupo de clientes adicionado!', 'Sucesso');
                        $state.go('clientgroups');
					},
					function (response)
					{
						toastr.error(response.data.message, 'Error');
					});
				}
			}
		}
	};

	$scope.list = function () {
		$state.go('clientgroups');
	}

	$scope.remove = function ()
	{
		if (!$scope.permModel.remover)
            toastr.error('Acesso negado!', 'Permissão');
		else
		{
            Api.ClientGroup.remove({ id: id }, function (data)
			{
				toastr.success('Grupo de clientes removido!', 'Sucesso');
				$scope.list();
			},
			function (response) {
				toastr.error(response.data.message, 'Permission');
			});
		}
	}

	// ============================================
	// client 
	// ============================================

	$scope.addClient = false;

	$scope.removeClient = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
        {
			$scope.viewModel.updateCommand = "removeClient";
			$scope.viewModel.anexedEntity = $scope.viewModel.clients[index];

			Api.ClientGroup.update({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('Cliente removido', 'Sucesso');
                init();
			});
		}
	}

	$scope.addNewClient = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
			$scope.addClient = !$scope.addClient;
	}

	$scope.newClient = {};

	$scope.editClient = function (mdl) {
		$scope.addClient = true;
		$scope.newClient = mdl;
	}

	$scope.cancelClient = function () {
		$scope.addClient = false;
		$scope.newClient = {};
	}

	$scope.saveNewClient = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
		{			
			$scope.fkClient_fail = $scope.newClient.fkClient == undefined;

			if (!$scope.fkClient_fail)
            {
                for (var i = 0; i < $scope.viewModel.clients.length; i++)
                {
                    if ($scope.viewModel.clients[i].id == $scope.newClient.fkClient)
                    {
                        toastr.error('Cliente já adicionado', 'Validação');
                        $scope.fkClient_fail = true;
                        return;
                    }
                }               

				$scope.addClient = false;

				$scope.viewModel.updateCommand = "newClient";
				$scope.viewModel.anexedEntity = $scope.newClient;

				Api.ClientGroup.update({ id: id }, $scope.viewModel, function (data)
				{
					$scope.newClient = {};
                    toastr.success('Cliente salvo', 'Sucesso');

                    init();		
				},
				function (response) {
					toastr.error(response.data.message, 'Error');
				});
			}
		}
	}
		
}]);
