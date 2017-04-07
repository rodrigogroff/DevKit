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

		$scope.selectTaskCategory = ngSelects.obterConfiguracao(Api.TaskCategory, { scope: $scope, filtro: { campo: 'fkTaskType', valor: 'viewModel.id' } });
		$scope.selectAccType = ngSelects.obterConfiguracao(Api.AccType, { });
		$scope.selectProject = ngSelects.obterConfiguracao(Api.Project, { });
		$scope.selectTaskFlow = ngSelects.obterConfiguracao(Api.TaskFlow, { scope: $scope, filtro: { campo: 'fkTaskCategory', valor: 'newAcc.fkTaskCategory' } });

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
	
			if (!$scope.stName_fail && 
				!$scope.fkProject_fail )
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

	$scope.editCategorie = function (mdl) {
		$scope.addCategorie = true;
		$scope.newCategorie = mdl;
	}

	$scope.newCategorie =
		{
			fkTaskType: undefined,
			stName: ''			
		};

	$scope.saveNewCategorie = function ()
	{
		$scope.stCategorieName_fail = invalidCheck($scope.newCategorie.stName);

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

				toastr.success('Category saved', 'Success');
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

	$scope.$watch('newFlow.fkTaskCategory', function (newState, oldState)
	{
		if (newState !== oldState)
			$scope.loadFlows();
	});

	$scope.flows = [];

	$scope.loadFlows = function ()
	{
		var opcoes =
			{
				fkTaskCategory: $scope.newFlow.fkTaskCategory
			};

		$scope.flows = [];

		Api.TaskFlow.listPage(opcoes, function (data)
		{
			$scope.flows = data.results;
		});
	}

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
				$scope.loadFlows();
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

	$scope.newFlow = { };

	$scope.saveNewFlow = function ()
	{
		$scope.stFlowName_fail = invalidCheck ($scope.newFlow.stName);
		$scope.stFlowOrder_fail = invalidCheck ($scope.newFlow.nuOrder);

		if (!$scope.stFlowName_fail && !$scope.stFlowOrder_fail)
		{
			var tmp = $scope.newFlow.fkTaskCategory;

			$scope.viewModel.updateCommand = "newFlow";
			$scope.viewModel.anexedEntity = $scope.newFlow;

			Api.TaskType.update({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('Flow saved', 'Success');

				$scope.newFlow = {};
				$scope.newFlow.fkTaskCategory = tmp;

				$scope.loadFlows();

				$scope.addFlow = false;

			}, function (response) {
				toastr.error(response.data.message, 'Error');
			});
		}
	}

	// ---------------------------------
	// accs
	// ---------------------------------

	$scope.$watch('newAcc.fkTaskCategory', function (newState, oldState) {
		if (newState !== oldState)
			$scope.loadAccs();
	});

	$scope.accs = [];

	$scope.loadAccs = function () {
		var opcoes =
			{
				fkTaskCategory: $scope.newAcc.fkTaskCategory
			};

		$scope.accs = [];

		Api.TaskAccumulator.listPage(opcoes, function (data) {
			$scope.accs = data.results;
		});
	}

	$scope.addAcc = false;

	$scope.editAcc = function (mdl)
	{
		$scope.addAcc = true;
		$scope.newAcc = mdl;
	}

	$scope.removeAcc = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else {
			$scope.viewModel.updateCommand = "removeAcc";
			$scope.viewModel.anexedEntity = lista[index];

			Api.TaskType.update({ id: id }, $scope.viewModel, function (data) {
				toastr.success('Accumulator removed', 'Success');
				$scope.loadAccs();
			});
		}
	}

	$scope.addNewAcc = function () {
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
			$scope.addAcc = !$scope.addAcc;
	}

	$scope.newAcc = {};

	$scope.saveNewAcc = function ()
	{
		$scope.stAccName_fail = invalidCheck($scope.newAcc.stName);
		$scope.selectAccType_fail = $scope.newAcc.fkTaskAccType == undefined;
		$scope.selectAccFlow_fail = $scope.newAcc.fkTaskFlow == undefined;

		if (!$scope.stAccName_fail &&
			!$scope.selectAccType_fail &&
			!$scope.selectAccFlow_fail)
		{
			var tmp = $scope.newAcc.fkTaskCategory;

			$scope.viewModel.updateCommand = "newAcc";
			$scope.viewModel.anexedEntity = $scope.newAcc;

			Api.TaskType.update({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('Accumulator saved', 'Success');

				$scope.newAcc = {};
				$scope.newAcc.fkTaskCategory = tmp;

				$scope.loadAccs();

				$scope.addAcc = false;

			}, function (response) {
				toastr.error(response.data.message, 'Error');
			});
		}
	}

}]);
