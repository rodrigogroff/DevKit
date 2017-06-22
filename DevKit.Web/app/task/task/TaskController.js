'use strict';

angular.module('app.controllers').controller('TaskController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$rootScope.exibirMenu = true;

	$scope.selectPriority = ngSelects.obterConfiguracao(Api.Priority, {});
	$scope.selectProjects = ngSelects.obterConfiguracao(Api.Project, {});
	$scope.selectClients = ngSelects.obterConfiguracao(Api.Client, {});
	$scope.selectClientGroups = ngSelects.obterConfiguracao(Api.ClientGroup, {});
	$scope.selectPhases = ngSelects.obterConfiguracao(Api.Phase, { scope: $scope, filtro: { campo: 'fkProject', valor: 'viewModel.fkProject' } });
	$scope.selectSprints = ngSelects.obterConfiguracao(Api.Sprint, { scope: $scope, filtro: { campo: 'fkPhase', valor: 'viewModel.fkPhase' } });
	$scope.selectVersions = ngSelects.obterConfiguracao(Api.Version, { scope: $scope, filtro: { campo: 'fkSprint', valor: 'viewModel.fkSprint' } });
	$scope.selectUsers = ngSelects.obterConfiguracao(Api.User, { campoNome: 'stLogin' });
	$scope.selectTaskType = ngSelects.obterConfiguracao(Api.TaskType, { scope: $scope, filtro: { campo: 'fkProject', valor: 'viewModel.fkProject' } });
	$scope.selectTaskCategory = ngSelects.obterConfiguracao(Api.TaskCategory, { scope: $scope, filtro: { campo: 'fkTaskType', valor: 'viewModel.fkTaskType' } });
	$scope.selectTaskFlow = ngSelects.obterConfiguracao(Api.TaskFlow, { scope: $scope, filtro: { campo: 'fkTaskCategory', valor: 'viewModel.fkTaskCategory' } });
	$scope.selectTaskAcc = ngSelects.obterConfiguracao(Api.TaskTypeAccumulator, { scope: $scope, filtro: { campo: 'fkTaskCategory', valor: 'viewModel.fkTaskCategory' } });

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
        Api.Setup.get({ id: 1 }, function (data) {
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
						init();
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
            Api.Task.remove({ id: id }, function (data)
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
        else
        {
			$scope.viewModel.updateCommand = "removeAcc";
			$scope.viewModel.anexedEntity = $scope.viewModel.accs[index];

			Api.Task.update({ id: id }, $scope.viewModel, function (data) {
				toastr.success('Accumulator value removed', 'Success');
				init();
			});
		}
	}

	$scope.$watch('newAcc.fkTaskAcc', function (newState, oldState)
	{
		if (newState !== oldState)
		{
            Api.TaskTypeAccumulator.get({ id: $scope.newAcc.fkTaskAcc }, function (data) {
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

	$scope.cancelAcc = function () {
		$scope.addAcc = false;
		$scope.newAcc = { fkTask: id };
	}

	$scope.newAcc = { fkTask: id };

	$scope.saveNewAcc = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.newAcc_fkTaskAcc_fail = $scope.newAcc.fkTaskAcc == undefined;

			if ($scope.fkTaskAccType == 1)
				$scope.newAcc_val_fail = invalidCheck($scope.newAcc.sMoneyVal);
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
					$scope.newAcc = { fkTask: id };

					toastr.success('Accumulator saved', 'Success');
					init();

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
			init();

		}, function (response) {
			toastr.error(response.data.message, 'Error');
		});
	}

    // ============================================
    // Clients 
    // ============================================

	$scope.addClient = false;

	$scope.removeClient = function (entity) {
	    if (!$scope.permModel.novo && !$scope.permModel.edicao)
	        toastr.error('Access denied!', 'Permission');
        else
        {
	        $scope.viewModel.updateCommand = "removeClient";
	        $scope.viewModel.anexedEntity = entity;

	        Api.Task.update({ id: id }, $scope.viewModel, function (data) {
	            toastr.success('Client removed', 'Success');
	            init();
	        });
	    }
	}

	$scope.addNewClient= function () {
	    if (!$scope.permModel.novo && !$scope.permModel.edicao)
	        toastr.error('Access denied!', 'Permission');
	    else
	        $scope.addClient = !$scope.addClient;
	}

	$scope.cancelClient = function () {
	    $scope.addClient = false;
	    $scope.newClient = {};
	}

	$scope.newClient = {};

	$scope.saveNewClient = function ()
	{
	    if (!$scope.permModel.novo && !$scope.permModel.edicao)
	        toastr.error('Access denied!', 'Permission');
        else
        {
	        $scope.newClient_fail = $scope.newClient.fkClient == undefined;

            if (!$scope.newClient_fail)
            {
	            $scope.addClient = false;

	            $scope.viewModel.updateCommand = "newClient";
	            $scope.viewModel.anexedEntity = $scope.newClient;

	            Api.Task.update({ id: id }, $scope.viewModel, function (data)
	            {
	                $scope.newClient = {};
	                toastr.success('Client added', 'Success');
	                init();
	            },
				function (response) {
				    toastr.error(response.data.message, 'Error');
				});
	        }
	    }
	}

    // ============================================
    // Client Groups 
    // ============================================

	$scope.addClientGroup = false;

	$scope.removeClientGroup = function (entity) {
	    if (!$scope.permModel.novo && !$scope.permModel.edicao)
	        toastr.error('Access denied!', 'Permission');
        else
        {
	        $scope.viewModel.updateCommand = "removeClientGroup";
	        $scope.viewModel.anexedEntity = entity;

	        Api.Task.update({ id: id }, $scope.viewModel, function (data) {
	            toastr.success('Client Group removed', 'Success');
	            init();
	        });
	    }
	}

	$scope.addNewClientGroup = function () {
	    if (!$scope.permModel.novo && !$scope.permModel.edicao)
	        toastr.error('Access denied!', 'Permission');
	    else
	        $scope.addClientGroup = !$scope.addClientGroup;
	}

	$scope.cancelClientGroup = function () {
	    $scope.addClientGroup = false;
	    $scope.newClientGroup = {};
	}

	$scope.newClientGroup = {};

	$scope.saveNewClientGroup = function () {
	    if (!$scope.permModel.novo && !$scope.permModel.edicao)
	        toastr.error('Access denied!', 'Permission');
        else
        {
	        $scope.newClientGroup_fail = $scope.newClientGroup.fkClientGroup == undefined;

            if (!$scope.newClientGroup_fail)
            {
	            $scope.addClientGroup = false;

	            $scope.viewModel.updateCommand = "newClientGroup";
	            $scope.viewModel.anexedEntity = $scope.newClientGroup;

                Api.Task.update({ id: id }, $scope.viewModel, function (data)
                {
	                $scope.newClientGroup = {};
	                toastr.success('Client group added', 'Success');
	                init();
	            },
				function (response) {
				    toastr.error(response.data.message, 'Error');
				});
	        }
	    }
	}

	// ============================================
	// Subtask 
	// ============================================

	$scope.addSubtask = false;

	$scope.removeSubtask = function (entity)
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
        {
			$scope.viewModel.updateCommand = "removeSubtask";
			$scope.viewModel.anexedEntity = entity;

			Api.Task.update({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('Sub-task removed', 'Success');
				init();
			});
		}
	}

	$scope.addNewSubtask = function () {
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
			$scope.addSubtask = !$scope.addSubtask;
	}

	$scope.cancelSubtask = function () {
		$scope.addSubtask = false;
		$scope.newSubtask = { };
	}

	$scope.newSubtask = {};

	$scope.saveNewSubtask = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.newSubtask_fail = invalidCheck($scope.newSubtask.stProtocol);

			if (!$scope.newSubtask_fail)
			{
				$scope.addSubtask = false;

				$scope.viewModel.updateCommand = "newSubtask";
				$scope.viewModel.anexedEntity = $scope.newSubtask;

				Api.Task.update({ id: id }, $scope.viewModel, function (data)
				{
					$scope.newSubtask = { };
					toastr.success('Sub-task saved', 'Success');
					init();
				},
				function (response) {
					toastr.error(response.data.message, 'Error');
				});
			}
		}
	}

    // ============================================
    // CustomStep 
    // ============================================

	$scope.addCustomStep = false;

	$scope.removeCustomStep = function (entity) {
	    if (!$scope.permModel.novo && !$scope.permModel.edicao)
	        toastr.error('Access denied!', 'Permission');
        else
        {
	        $scope.viewModel.updateCommand = "removeCustomStep";
	        $scope.viewModel.anexedEntity = entity;

	        Api.Task.update({ id: id }, $scope.viewModel, function (data) {
	            toastr.success('Custom step removed', 'Success');
	            init();
	        });
	    }
	}

	$scope.addNewCustomStep = function () {
	    if (!$scope.permModel.novo && !$scope.permModel.edicao)
	        toastr.error('Access denied!', 'Permission');
	    else
	        $scope.addCustomStep = !$scope.addCustomStep;
	}

	$scope.cancelCustomStep = function () {
	    $scope.addCustomStep = false;
	    $scope.newCustomStep = {};
	}

	$scope.newCustomStep = {};

	$scope.saveNewCustomStep = function () {
	    if (!$scope.permModel.novo && !$scope.permModel.edicao)
	        toastr.error('Access denied!', 'Permission');
        else
        {
	        $scope.newCustomStep_fail = invalidCheck($scope.newCustomStep.stName);

            if (!$scope.newCustomStep_fail)
            {
	            $scope.addCustomStep = false;

	            $scope.viewModel.updateCommand = "newCustomStep";
	            $scope.viewModel.anexedEntity = $scope.newCustomStep;

                Api.Task.update({ id: id }, $scope.viewModel, function (data)
                {
	                $scope.newCustomStep = {};
	                toastr.success('Custom step saved', 'Success');
	                init();
	            },
				function (response) {
				    toastr.error(response.data.message, 'Error');
				});
	        }
	    }
	}

	$scope.selectCustomStep = function (mdl)
	{
	    mdl.bSelected = !mdl.bSelected;

	    $scope.viewModel.updateCommand = "entity";

        Api.Task.update({ id: id }, $scope.viewModel, function (data)
        {
	        toastr.success('Custom step saved!', 'Success');
	        init();
	    },
		function (response) {
		    toastr.error(response.data.message, 'Error');
		});
	}

	// ============================================
	// checkpoints
	// ============================================

	$scope.selectCheckpoint = function (mdl)
	{
	    mdl.bSelected = !mdl.bSelected;

        $scope.viewModel.updateCommand = "entity";

        Api.Task.update({ id: id }, $scope.viewModel, function (data)
        {
	        toastr.success('Check point saved!', 'Success');
	        init();
	    },
		function (response) {
			toastr.error(response.data.message, 'Error');
		});
	}

	// ============================================
	// questions 
	// ============================================

	$scope.addQuestion = false;

	$scope.removeQuestion = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
        {
			$scope.viewModel.updateCommand = "removeQuestion";
			$scope.viewModel.anexedEntity = $scope.viewModel.questions[index];

			Api.Task.update({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('Question removed', 'Success');
				init();
			});
		}
	}

	$scope.addNewQuestion = function () {
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
			$scope.addQuestion = !$scope.addQuestion;
	}

	$scope.newQuestion = {};

	$scope.editQuestion = function (mdl) {
		$scope.addQuestion = true;
		$scope.newQuestion = mdl;
	}

	$scope.cancelQuestion = function () {
		$scope.addQuestion = false;
		$scope.newQuestion = {};
	}

	$scope.saveNewQuestion = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.newQuestion_Statement_fail = invalidCheck($scope.newQuestion.stStatement);
		
			if (!$scope.newQuestion_Statement_fail)
			{
				$scope.addQuestion = false;

				$scope.viewModel.updateCommand = "newQuestion";
				$scope.viewModel.anexedEntity = $scope.newQuestion;

				Api.Task.update({ id: id }, $scope.viewModel, function (data) {
					$scope.newQuestion = {};
					toastr.success('Question saved', 'Success');
					init();
				},
				function (response) {
					toastr.error(response.data.message, 'Error');
				});
			}
		}
	}

}]);
