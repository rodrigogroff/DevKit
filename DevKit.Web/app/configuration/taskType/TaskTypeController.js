'use strict';

angular.module('app.controllers').controller('TaskTypeController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$rootScope.exibirMenu = true;

	$scope.selectAccType = ngSelects.obterConfiguracao(Api.AccType, {});
	$scope.selectProject = ngSelects.obterConfiguracao(Api.ProjectCombo, {});
	$scope.selectTaskCategory = ngSelects.obterConfiguracao(Api.TaskCategoryCombo, { scope: $scope, filtro: { campo: 'fkTaskType', valor: 'viewModel.id' } });
	$scope.selectTaskFlow = ngSelects.obterConfiguracao(Api.TaskFlowCombo, { scope: $scope, filtro: { campo: 'fkTaskCategory', valor: 'newAcc.fkTaskCategory' } });

	$scope.loading = false;

	$scope.viewModel = {};
	$scope.permModel = {};	
	$scope.permID = 105;
	$scope.auditLogPerm = 113;

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

        Api.Permission.get({ id: $scope.auditLogPerm }, function (data)
        {
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
						init();
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
                        $state.go('taskTypes');
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
            Api.TaskType.remove({ id: id }, function (data)
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
				init();
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

	$scope.cancelCategorie = function () {
		$scope.addCategorie = false;
		$scope.newCategorie = {};
	}

	$scope.newCategorie = { };

	$scope.saveNewCategorie = function ()
	{
		$scope.stCategorieName_fail = invalidCheck($scope.newCategorie.stName);

		if (!$scope.stCategorieName_fail)
        {
			$scope.viewModel.updateCommand = "newCategorie";
			$scope.viewModel.anexedEntity = $scope.newCategorie;

			Api.TaskType.update({ id: id }, $scope.viewModel, function (data)
			{
				$scope.newCategorie = {};
				$scope.addCategorie = false;
				toastr.success('Category saved', 'Success');
				init();
			},
            function (response)
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
				init();
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

	$scope.cancelFlow = function () {
		$scope.addFlow = false;
		$scope.newFlow = {};
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

				init();
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

	$scope.loadAccs = function ()
	{
		$scope.addAcc = false;

		var opcoes =
            {
				fkTaskCategory: $scope.newAcc.fkTaskCategory
			};

		$scope.accs = [];

		Api.TaskTypeAccumulator.listPage(opcoes, function (data) {
			$scope.accs = data.results;
		});
	}

	$scope.addAcc = false;

	$scope.editAcc = function (mdl)
	{
		if ($scope.addAcc == false)
		{
			$scope.addAcc = true;
			$scope.newAcc = mdl;
		}		
	}

	$scope.cancelAcc = function () {
		$scope.addAcc = false;
		$scope.newAcc = {};
	}

	$scope.removeAcc = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
        else
        {
			$scope.viewModel.updateCommand = "removeAcc";
			$scope.viewModel.anexedEntity = lista[index];

			Api.TaskType.update({ id: id }, $scope.viewModel, function (data) {
			    toastr.success('Accumulator removed', 'Success');
			    init();
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

                init();

				$scope.loadAccs();
				$scope.addAcc = false;

			},
			function (response) {
				toastr.error(response.data.message, 'Error');
			});
		}
	}

	// ---------------------------------
	// check points
	// ---------------------------------
		
	$scope.$watch('newCC.fkCategory', function (newState, oldState) {
		if (newState !== oldState)
			$scope.loadCCs();
	});

	$scope.checkpoints = [];

	$scope.loadCCs = function ()
	{
		$scope.addCC = false;

		var opcoes =
            {
				fkCategory: $scope.newCC.fkCategory
			};

		$scope.checkpoints = [];

		Api.TaskCheckPoint.listPage(opcoes, function (data) {
			$scope.checkpoints = data.results;
		});
	}

	$scope.addCC = false;

	$scope.editCC = function (mdl)
	{
		if ($scope.addCC == false) {
			$scope.addCC = true;
			$scope.newCC = mdl;
		}
	}

	$scope.cancelCC = function () {
		$scope.addCC = false;
		$scope.newCC = {};
	}

	$scope.removeCC = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
        else
        {
			$scope.viewModel.updateCommand = "removeCC";
			$scope.viewModel.anexedEntity = lista[index];

			Api.TaskType.update({ id: id }, $scope.viewModel, function (data) {
				toastr.success('Check Point removed', 'Success');
				$scope.loadCCs();
			});
		}
	}

	$scope.addNewCC = function () {
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
			$scope.addCC = !$scope.addCC;
	}

	$scope.newCC = {};

	$scope.saveNewCC = function () {
		$scope.stCCName_fail = invalidCheck($scope.newCC.stName);

		if (!$scope.stAccName_fail)
		{
			var tmp = $scope.newCC.fkCategory;

			$scope.viewModel.updateCommand = "newCC";
			$scope.viewModel.anexedEntity = $scope.newCC;

			Api.TaskType.update({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('Check Point saved', 'Success');
				$scope.newCC = {};
				$scope.newCC.fkCategory = tmp;
				init();
				$scope.loadCCs();
				$scope.addCC = false;
			},
			function (response) {
				toastr.error(response.data.message, 'Error');
			});
		}
	}

}]);
