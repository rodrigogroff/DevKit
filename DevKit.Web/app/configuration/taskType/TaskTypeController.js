'use strict';

angular.module('app.controllers').controller('TaskTypeController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$scope.loading = false;

	$scope.viewModel = {};
	$scope.permModel = {};	
	$scope.permID = 105;

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
	
	var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

	init();

	function init()
	{
		CheckPermissions();

		if (id > 0)
		{
			$scope.loading = true;
			Api.TaskType.get({ id: id }, function (data)
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
			$scope.viewModel = { };
		}
	}

	$scope.save = function ()
	{
		$scope.stName_fail = false;

		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			if ($scope.viewModel.stName != undefined) {
				if ($scope.viewModel.stName.length < 5)
					$scope.stName_fail = true;
			}
			else
				$scope.stName_fail = true;
	
			if (!$scope.stName_fail)
			{
				if (id > 0)
				{
					$scope.viewModel.updateCommand = "entity";

					Api.TaskType.update({ id: id }, $scope.viewModel, function (data)
					{
						toastr.success('Task type saved!', 'Success');
					},
					function (response)
					{
						toastr.error(response.data.message, 'Error');
					});
				}
				else
				{
					Api.TaskType.add($scope.viewModel, function (data)
					{
						toastr.success('Task type added!', 'Success');
						$state.go('taskType', { id: data.id });
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
		$state.go('taskTypes');
	}

	$scope.remove = function ()
	{
		if (!$scope.permModel.remover)
			toastr.error('Access denied!', 'Permission');
		else
		{
			Api.TaskType.remove({ id: id }, {}, function (data)
			{
				toastr.success('Task Type removed!', 'Success');
				$scope.list();
			},
			function (response) {
				toastr.error(response.data.message, 'Permission');
			});
		}
	}

	// ---------------------------------
	// categories
	// ---------------------------------

	$scope.addCategorie = false;

	$scope.removeCategorie = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.viewModel.updateCommand = "removeCategorie";
			$scope.viewModel.anexedEntity = lista[index];

			Api.TaskType.update({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('Category removed', 'Success');
				$scope.viewModel.categories = data.categories;
			});
		}
	}

	$scope.addNewCategorie = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
			$scope.addCategorie = !$scope.addCategorie;
	}

	$scope.newCategorie =
		{
			fkTaskType: undefined,
			stName: ''			
		};

	$scope.saveNewCategorie = function ()
	{
		$scope.stCategorieName_fail = false;

		if ($scope.newCategorie.stName != undefined && $scope.newCategorie.stName.length == 0)
			$scope.stCategorieName_fail = true;

		if (!$scope.stCategorieName_fail)
		{
			$scope.viewModel.updateCommand = "newCategorie";
			$scope.viewModel.anexedEntity = $scope.newCategorie;

			Api.TaskType.update({ id: id }, $scope.viewModel, function (data)
			{
				$scope.newCategorie =
				{
					fkTaskType: undefined,
					stName: ''
				};

				toastr.success('Category added', 'Success');
				$scope.viewModel.categories = data.categories;

				$scope.addCategorie = false;

			}, function (response)
			{
				toastr.error(response.data.message, 'Error');
			});
		}
	}

	// ---------------------------------
	// flows
	// ---------------------------------

	$scope.addFlow = false;

	$scope.editFlow = function (mdl)
	{
		$scope.addFlow = true;
		$scope.newFlow = mdl;
	}

	$scope.removeFlow = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.viewModel.updateCommand = "removeFlow";
			$scope.viewModel.anexedEntity = lista[index];

			Api.TaskType.update({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('Flow removed', 'Success');
				$scope.viewModel.flows = data.flows;
			});
		}
	}

	$scope.addNewFlow = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
			$scope.addFlow = !$scope.addFlow;
	}

	$scope.newFlow =
		{
			fkTaskType: undefined,
			stName: '',
			nuOrder: ''
		};

	$scope.saveNewFlow = function ()
	{
		$scope.stFlowName_fail = false;
		$scope.stFlowOrder_fail = false;

		if ($scope.newFlow.stName != undefined && $scope.newFlow.stName.length == 0)
			$scope.stFlowName_fail = true;

		if ($scope.newFlow.nuOrder != undefined && $scope.newFlow.nuOrder.length == 0)
			$scope.stFlowOrder_fail = true;

		if (!$scope.stFlowName_fail && !$scope.stFlowOrder_fail)
		{
			$scope.viewModel.updateCommand = "newFlow";
			$scope.viewModel.anexedEntity = $scope.newFlow;

			Api.TaskType.update({ id: id }, $scope.viewModel, function (data)
			{
				$scope.newFlow =
					{
						fkTaskType: undefined,
						stName: '',
						nuOrder: ''
					};

				toastr.success('Flow added', 'Success');
				$scope.viewModel.flows = data.flows;

				$scope.addFlow = false;

			}, function (response) {
				toastr.error(response.data.message, 'Error');
			});
		}
	}

}]);
