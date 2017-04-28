'use strict';

angular.module('app.controllers').controller('TaskController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$rootScope.exibirMenu = true;

	$scope.selectPriority = ngSelects.obterConfiguracao(Api.Priority, {});
	$scope.selectProjects = ngSelects.obterConfiguracao(Api.Project, {});
	$scope.selectPhases = ngSelects.obterConfiguracao(Api.Phase, { scope: $scope, filtro: { campo: 'fkProject', valor: 'viewModel.fkProject' } });
	$scope.selectSprints = ngSelects.obterConfiguracao(Api.Sprint, { scope: $scope, filtro: { campo: 'fkPhase', valor: 'viewModel.fkPhase' } });
	$scope.selectVersions = ngSelects.obterConfiguracao(Api.Version, { scope: $scope, filtro: { campo: 'fkSprint', valor: 'viewModel.fkSprint' } });
	$scope.selectUsers = ngSelects.obterConfiguracao(Api.User, { campoNome: 'stLogin' });
	$scope.selectTaskType = ngSelects.obterConfiguracao(Api.TaskType, { scope: $scope, filtro: { campo: 'fkProject', valor: 'viewModel.fkProject' } });
	$scope.selectTaskCategory = ngSelects.obterConfiguracao(Api.TaskCategory, { scope: $scope, filtro: { campo: 'fkTaskType', valor: 'viewModel.fkTaskType' } });
	$scope.selectTaskFlow = ngSelects.obterConfiguracao(Api.TaskFlow, { scope: $scope, filtro: { campo: 'fkTaskCategory', valor: 'viewModel.fkTaskCategory' } });
	$scope.selectTaskAcc = ngSelects.obterConfiguracao(Api.TaskAccumulator, { scope: $scope, filtro: { campo: 'fkTaskCategory', valor: 'viewModel.fkTaskCategory' } });

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
			$scope.fkVersion_fail = $scope.viewModel.fkVersion == undefined;
			$scope.nuPriority_fail = $scope.viewModel.nuPriority == undefined;
			$scope.fkTaskType_fail = $scope.viewModel.fkTaskType == undefined;
			$scope.fkTaskCategory_fail = $scope.viewModel.fkTaskCategory == undefined;
	
			if (!$scope.stTitle_fail &&
				!$scope.stLocalization_fail &&
				!$scope.stDescription_fail &&
				!$scope.fkProject_fail &&
				!$scope.fkPhase_fail && 
				!$scope.fkSprint_fail &&
				!$scope.fkVersion_fail &&
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

				$rootScope.$broadcast('updateCounters');
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

	// ============================================
	// acc´s 
	// ============================================

	$scope.addAcc = false;

	$scope.removeAcc = function (index, lista) {
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else {
			$scope.viewModel.updateCommand = "removeAcc";
			$scope.viewModel.anexedEntity = $scope.viewModel.accs[index];

			Api.Task.update({ id: id }, $scope.viewModel, function (data) {
				toastr.success('Accumulator value removed', 'Success');
				$scope.viewModel.accs = data.accs;
			});
		}
	}

	$scope.$watch('newAcc.fkTaskAcc', function (newState, oldState)
	{
		if (newState !== oldState)
		{
			Api.TaskAccumulator.get({ id: $scope.newAcc.fkTaskAcc }, function (data)
			{
				$scope.fkTaskAccType = data.fkTaskAccType;
			});
		}
	});

	$scope.addNewAcc = function () {
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
			$scope.addAcc = !$scope.addAcc;
	}

	$scope.newAcc = { fkTask: id, fkTaskAcc: undefined, nuValue: '', nuHourValue : '', nuMinValue: '', fkUser: undefined };

	$scope.saveNewAcc = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.newAcc_fkTaskAcc_fail = $scope.newAcc.fkTaskAcc == undefined;

			if ($scope.fkTaskAccType == 1)
				$scope.newAcc_val_fail = invalidCheck($scope.newAcc.nuValue);
			else
				if ($scope.fkTaskAccType == 2)
					$scope.newAcc_val_fail = invalidCheck($scope.newAcc.nuHourValue) && invalidCheck($scope.newAcc.nuMinValue);

			if (!$scope.newAcc_fkTaskAcc_fail &&
				!$scope.newAcc_val_fail)
			{
				$scope.addAcc = false;

				$scope.viewModel.updateCommand = "newAcc";
				$scope.viewModel.anexedEntity = $scope.newAcc;

				Api.Task.update({ id: id }, $scope.viewModel, function (data) 
				{
					$scope.newAcc = { fkTask: id, fkTaskAcc: undefined, nuValue: '', nuHourValue : '', nuMinValue: '', fkUser: undefined };

					toastr.success('Accumulator saved', 'Success');

					$scope.viewModel.accs = data.accs;

				}, function (response) {
					toastr.error(response.data.message, 'Error');
				});
			}
		}
	}

	$scope.removeAccValue = function (accVal)
	{
		$scope.viewModel.updateCommand = "removeAccValue";
		$scope.viewModel.anexedEntity = accVal;

		Api.Task.update({ id: id }, $scope.viewModel, function (data)
		{
			toastr.success('Accumulator removed', 'Success');

			$scope.viewModel.accs = data.accs;

		}, function (response) {
			toastr.error(response.data.message, 'Error');
		});
	}	

}]);
