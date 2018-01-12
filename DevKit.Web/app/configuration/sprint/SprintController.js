
angular.module('app.controllers').controller('SprintController',
['$scope', '$state', '$stateParams', '$rootScope', 'Api', 'ngSelects',
function ($scope, $state, $stateParams, $rootScope, Api, ngSelects)
{
	$rootScope.exibirMenu = true;

	$scope.selectProjects = ngSelects.obterConfiguracao(Api.ProjectCombo, {});
	$scope.selectVersionStates = ngSelects.obterConfiguracao(Api.VersionState, {});
	$scope.selectPhases = ngSelects.obterConfiguracao(Api.PhaseCombo, { scope: $scope, filtro: { campo: 'fkProject', valor: 'viewModel.fkProject' } });

	$scope.loading = false;

	$scope.viewModel = {};
	$scope.permModel = {};	
	$scope.permID = 104;
	$scope.auditLogPerm = 111;

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

		Api.Permission.get({ id: $scope.auditLogPerm }, function (data) {
			$scope.auditLogView = $scope.permModel.visualizar;
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

            Api.Sprint.get({ id: id }, function (data)
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
			$scope.fkProject_fail = $scope.viewModel.fkProject == undefined;
			$scope.fkPhase_fail = $scope.viewModel.fkPhase == undefined;

			if (!$scope.stName_fail &&
				!$scope.fkProject_fail &&
				!$scope.fkPhase_fail)
            {
				if (id > 0)
				{
					$scope.viewModel.updateCommand = "entity";

					Api.Sprint.update({ id: id }, $scope.viewModel, function (data)
					{
						toastr.success('Sprint salvo!', 'Success');
                        init();
					},
					function (response)
					{
						toastr.error(response.data.message, 'Error');
					});
				}
				else
				{
					Api.Sprint.add($scope.viewModel, function (data)
					{
						toastr.success('Sprint adicionado!', 'Success');
                        $state.go('sprints');
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
		$state.go('sprints');
	}

	$scope.remove = function ()
	{
		if (!$scope.permModel.remover)
            toastr.error('Acesso negado!', 'Permissão');
		else
		{
            Api.Sprint.remove({ id: id, login: $rootScope.loginInfo }, {}, function (data)
			{
				toastr.success('Sprint removido!', 'Success');
				$scope.list();
			},
			function (response) {
				toastr.error(response.data.message, 'Permission');
			});
		}
	}

	// ---------------------------------
	// versions
	// ---------------------------------

	$scope.addVersion = false;

	$scope.removeVersion = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
        {
			$scope.viewModel.updateCommand = "removeVersion";
			$scope.viewModel.anexedEntity = lista[index];

			Api.Sprint.update({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('Versão removida', 'Success');
                init();
			});
		}
	}

    $scope.addNewVersion = function ()
    {
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
			$scope.addVersion = !$scope.addVersion;
	}

	$scope.cancelVersion = function () {
		$scope.addVersion = false;
		$scope.newVersion = {};
	}
	
	$scope.newVersion = { };

	$scope.editVersion = function (mdl) {
		$scope.addVersion = true;
		$scope.newVersion = mdl;
	}

	$scope.saveNewVersion = function ()
	{
		$scope.stVersion_fail = invalidCheck($scope.newVersion.stName);
		$scope.fkVersionState_fail = $scope.newVersion.fkVersionState == undefined;

		if (!$scope.stVersion_fail && 
			!$scope.fkVersionState_fail)
        {
			$scope.viewModel.updateCommand = "newVersion";
			$scope.viewModel.anexedEntity = $scope.newVersion;

			Api.Sprint.update({ id: id }, $scope.viewModel, function (data)
			{
				$scope.newVersion = {};
                $scope.addVersion = false;

				toastr.success('Versão salva', 'Sucesso');
                init();				
			},
			function (response)
			{
				toastr.error(response.data.message, 'Error');
			});
		}
	}
	
}]);
