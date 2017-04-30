'use strict';

angular.module('app.controllers').controller('SprintController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$rootScope.exibirMenu = true;

	$scope.selectProjects = ngSelects.obterConfiguracao(Api.Project, {});
	$scope.selectVersionStates = ngSelects.obterConfiguracao(Api.VersionState, {});
	$scope.selectPhases = ngSelects.obterConfiguracao(Api.Phase, { scope: $scope, filtro: { campo: 'fkProject', valor: 'viewModel.fkProject' } });

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
				toastr.error('Access denied!', 'Permission');
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
			$scope.loading = true;
			Api.Sprint.get({ id: id }, function (data)
			{
				$scope.viewModel = data;
				$scope.loading = false;
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
			toastr.error('Access denied!', 'Permission');
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
						toastr.success('Sprint saved!', 'Success');
						$scope.viewModel.logs = data.logs;
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
						toastr.success('Sprint added!', 'Success');
						$state.go('sprint', { id: data.id });
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
			toastr.error('Access denied!', 'Permission');
		else
		{
			Api.Sprint.remove({ id: id }, {}, function (data)
			{
				toastr.success('Sprint removed!', 'Success');
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
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.viewModel.updateCommand = "removeVersion";
			$scope.viewModel.anexedEntity = lista[index];

			Api.Sprint.update({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('Version removed', 'Success');
				$scope.viewModel.versions = data.versions;
				$scope.viewModel.logs = data.logs;
			});
		}
	}

	$scope.addNewVersion = function () {
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
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

				toastr.success('Version saved', 'Success');
				$scope.viewModel.versions = data.versions;
				$scope.viewModel.logs = data.logs;

				$scope.addVersion = false;

			},
			function (response)
			{
				toastr.error(response.data.message, 'Error');
			});
		}
	}
	
}]);
