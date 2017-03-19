﻿'use strict';

angular.module('app.controllers').controller('TaskController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$scope.loading = false;

	$scope.viewModel = {};
	$scope.permModel = {};	
	$scope.permID = 106;

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
	}

	function loadSetup()
	{
		Api.Setup.get({ id: 1 }, function (data)
		{
			$scope.setupModel = data;
		});
	}
	
	var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

	init();

	function init()
	{
		CheckPermissions();
		loadSetup();

		$scope.selectPriority = ngSelects.obterConfiguracao(Api.Priority, { tamanhoPagina: 15, campoNome: 'stName' });

		$scope.selectProjects = ngSelects.obterConfiguracao(Api.Project, { tamanhoPagina: 15, campoNome: 'stName' });

		$scope.selectPhases = ngSelects.obterConfiguracao(Api.Phase, {
				tamanhoPagina: 15, scope: $scope,
				filtro: { campo: 'fkProject', valor: 'viewModel.fkProject' } });

		$scope.selectSprints = ngSelects.obterConfiguracao(Api.Sprint, {
				tamanhoPagina: 15, scope: $scope,
				filtro: { campo: 'fkPhase', valor: 'viewModel.fkPhase' } });

		$scope.selectVersions = ngSelects.obterConfiguracao(Api.Version, {
				tamanhoPagina: 15, scope: $scope,
				filtro:	{ campo: 'fkSprint', valor: 'viewModel.fkSprint' } });

		$scope.selectUsers = ngSelects.obterConfiguracao(Api.User, { tamanhoPagina: 15, campoNome: 'stLogin' });

		$scope.selectTaskType = ngSelects.obterConfiguracao(Api.TaskType, { tamanhoPagina: 15, campoNome: 'stName' });

		$scope.selectTaskCategory = ngSelects.obterConfiguracao(Api.TaskCategory, {
				tamanhoPagina: 15, scope: $scope,
				filtro: { campo: 'fkTaskType', valor: 'viewModel.fkTaskType' } });

		$scope.selectTaskFlow = ngSelects.obterConfiguracao(Api.TaskFlow, {
				tamanhoPagina: 15, scope: $scope,
				filtro: { campo: 'fkTaskCategory', valor: 'viewModel.fkTaskCategory' }
		});

		if (id > 0)
		{
			$scope.loading = true;

			Api.Task.get({ id: id }, function (data)
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
		{
			$scope.viewModel = { bActive: true };
		}
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
			$scope.stTitle_fail = invalidCheck($scope.viewModel.stTitle);
			$scope.stLocalization_fail = invalidCheck($scope.viewModel.stLocalization);
			$scope.stDescription_fail = invalidCheck($scope.viewModel.stDescription);
			$scope.fkProject_fail = $scope.viewModel.fkProject == undefined;
			$scope.fkPhase_fail = $scope.viewModel.fkPhase == undefined;
			$scope.fkSprint_fail = $scope.viewModel.fkSprint == undefined;
			$scope.nuPriority_fail = $scope.viewModel.nuPriority == undefined;
			$scope.fkTaskType_fail = $scope.viewModel.fkTaskType == undefined;
			$scope.fkTaskCategory_fail = $scope.viewModel.fkTaskCategory == undefined;
	
			if (!$scope.stTitle_fail &&
				!$scope.stLocalization_fail &&
				!$scope.stDescription_fail &&
				!$scope.fkProject_fail &&
				!$scope.fkPhase_fail && 
				!$scope.fkSprint_fail && 
				!$scope.nuPriority_fail)
			{
				if (id > 0)
				{
					$scope.viewModel.updateCommand = "entity";

					Api.Task.update({ id: id }, $scope.viewModel, function (data)
					{
						toastr.success('Task saved!', 'Success');
						$scope.viewModel = data;
					},
					function (response)
					{
						toastr.error(response.data.message, 'Error');
					});
				}
				else
				{
					Api.Task.add($scope.viewModel, function (data)
					{
						toastr.success('Task added!', 'Success');
						$state.go('task', { id: data.id });
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
		$state.go('tasks');
	}

	$scope.remove = function ()
	{
		if (!$scope.permModel.remover)
			toastr.error('Access denied!', 'Permission');
		else
		{
			Api.Task.remove({ id: id }, {}, function (data)
			{
				toastr.success('Task removed!', 'Success');
				$scope.list();
			},
			function (response)
			{
				toastr.error(response.data.message, 'Permission');
			});
		}			
	}

}]);
