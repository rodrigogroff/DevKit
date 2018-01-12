
angular.module('app.controllers').controller('ClientController',
['$scope', '$state', '$stateParams', '$rootScope', 'Api', 'ngSelects',
function ($scope, $state, $stateParams, $rootScope, Api, ngSelects)
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;

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
        $scope.estado = ngSelects.obterConfiguracao(Api.EstadoCombo, {});
        $scope.cidade = ngSelects.obterConfiguracao(Api.CidadeCombo, { scope: $scope, filtro: { campo: 'fkEstado', valor: 'viewModel.fkEstado' } });

        $scope.setupModel = { stPhoneMask: '' }
        $scope.viewModel = {};
        $scope.permModel = {};
        $scope.permID = 120;

		CheckPermissions();

        Api.Setup.get({ id: 1 }, function (data) {
			$scope.setupModel = data;
		});

		if (id > 0)
		{
            if ($scope.loaded == undefined)
                $scope.loading = true;

            Api.Client.get({ id: id }, function (data)
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

	var invalidEmail = function (element) {
		if (element == undefined)
			return true;
		else {
			if (element.length == 0)
				return true;

			if (element.indexOf('@') < 1)
				return true;
		}

		return false;
	}

	$scope.save = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
		{
			$scope.stName_fail = invalidCheck($scope.viewModel.stName);
			$scope.stEmail_fail = invalidEmail($scope.viewModel.stContactEmail);
				
			if (!$scope.stName_fail && !$scope.stEmail_fail )
            {
				if (id > 0)
				{
					$scope.viewModel.updateCommand = "entity";

                    Api.Client.update({ id: id }, $scope.viewModel, function (data)
					{
						toastr.success('Cliente salvo!', 'Success');
						$scope.viewModel.logs = data.logs;
					},
					function (response)
					{
						toastr.error(response.data.message, 'Error');
					});
				}
				else
				{
                    Api.Client.add($scope.viewModel, function (data)
					{
						toastr.success('Cliente salvo!', 'Success');
                        $state.go('clients');
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
		$state.go('clients');
	}

	$scope.remove = function ()
	{
		if (!$scope.permModel.remover)
            toastr.error('Acesso negado!', 'Permissão');
		else
		{
            Api.Client.remove({ id: id }, function (data)
			{
				toastr.success('Client removido!', 'Sucesso');
				$scope.list();
			},
			function (response) {
				toastr.error(response.data.message, 'Permission');
			});
		}
	}
		
}]);
