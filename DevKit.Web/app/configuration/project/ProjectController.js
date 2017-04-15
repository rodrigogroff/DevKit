'use strict';

angular.module('app.controllers').controller('ProjectController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$scope.selectUsers = ngSelects.obterConfiguracao(Api.User, { campoNome: 'stLogin' });
	$scope.selectProjectTemplate = ngSelects.obterConfiguracao(Api.ProjectTemplate, {});
	$scope.selectPhases = ngSelects.obterConfiguracao(Api.Phase, { scope: $scope, filtro: { campo: 'fkProject', valor: 'viewModel.id' } });

	$scope.loading = false;

	$scope.viewModel = {};
	$scope.permModel = {};	
	$scope.permID = 103;
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

			Api.Project.get({ id: id }, function (data)
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
			$scope.fkTemplate_fail = $scope.viewModel.fkProjectTemplate == undefined;
	
			if (!$scope.stName_fail && 
				!$scope.fkTemplate_fail )
			{
				if (id > 0)
				{
					$scope.viewModel.updateCommand = "entity";

					Api.Project.update({ id: id }, $scope.viewModel, function (data)
					{
						toastr.success('Project saved!', 'Success');
						$scope.viewModel.logs = data.logs;
					},
					function (response)
					{
						toastr.error(response.data.message, 'Error');
					});
				}
				else
				{
					Api.Project.add($scope.viewModel, function (data)
					{
						toastr.success('Project added!', 'Success');
						$state.go('project', { id: data.id });
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
		$state.go('projects');
	}

	$scope.remove = function ()
	{
		if (!$scope.permModel.remover)
			toastr.error('Access denied!', 'Permission');
		else
		{
			Api.Project.remove({ id: id }, {}, function (data)
			{
				toastr.success('Project removed!', 'Success');
				$scope.list();
			},
			function (response) {
				toastr.error(response.data.message, 'Permission');
			});
		}
	}
		
	// ---------------------------------
	// users
	// ---------------------------------

	$scope.addUser = false;

	$scope.removeUser = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.viewModel.updateCommand = "removeUser";
			$scope.viewModel.anexedEntity = lista[index];

			Api.Project.update({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('User removed', 'Success');
				$scope.viewModel.users = data.users;
				$scope.viewModel.logs = data.logs;
			});
		}
	}

	$scope.addNewUser = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
			$scope.addUser = !$scope.addUser;
	}

	$scope.editUser = function (mdl)
	{
		$scope.addUser = true;
		$scope.newUser = mdl;
	}

	$scope.newUser =
		{
			fkUser: undefined,
			stRole: '', 
			fkProject: undefined
		};

	$scope.saveNewUser = function ()
	{
		$scope.fkUser_fail = $scope.newUser.fkUser == undefined;
		$scope.stRole_fail = invalidCheck($scope.newUser.stRole);
	
		if (!$scope.fkUser_fail && !$scope.stRole_fail)
		{
			$scope.viewModel.updateCommand = "newUser";
			$scope.viewModel.anexedEntity = $scope.newUser;

			Api.Project.update({ id: id }, $scope.viewModel, function (data)
			{
				$scope.newUser = { fkUser: undefined, stRole: '', fkProject: undefined };

				toastr.success('User added', 'Success');
				$scope.viewModel.users = data.users;
				$scope.viewModel.logs = data.logs;

				$scope.addUser = false;

			}, function (response)
			{
				toastr.error(response.data.message, 'Error');
			});
		}
	}

	// ---------------------------------
	// phases
	// ---------------------------------

	$scope.addPhase = false;

	$scope.removePhase = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.viewModel.updateCommand = "removePhase";
			$scope.viewModel.anexedEntity = lista[index];

			Api.Project.update({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('Phase removed', 'Success');
				$scope.viewModel.phases = data.phases;
				$scope.viewModel.logs = data.logs;
			});
		}
	}

	$scope.addNewPhase = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
			$scope.addPhase = !$scope.addPhase;
	}

	$scope.editPhase = function (mdl)
	{
		$scope.addPhase = true;
		$scope.newPhase = mdl;
	}

	$scope.newPhase =
		{
			stName: '',
			dtStart: '',
			dtEnd: '',
			bComplete: false,
			fkProject: undefined
		};

	$scope.saveNewPhase = function ()
	{		
		$scope.newphase_stName_fail = invalidCheck($scope.newPhase.stName);

		if (!$scope.newphase_stName_fail)
		{
			$scope.viewModel.updateCommand = "newPhase";
			$scope.viewModel.anexedEntity = $scope.newPhase;

			Api.Project.update({ id: id }, $scope.viewModel, function (data)
			{
				$scope.newPhase =
					{
						stName: '',
						dtStart: '',
						dtEnd: '',
						bComplete: false,
						fkProject: undefined
					};

				toastr.success('Phase added', 'Success');
				$scope.viewModel.phases = data.phases;
				$scope.viewModel.logs = data.logs;

				$scope.addPhase = false;

			}, function (response) {
				toastr.error(response.data.message, 'Error');
			});
		}
	}

	// ---------------------------------
	// sprints
	// ---------------------------------

	$scope.addSprint = false;

	$scope.removeSprint = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else {
			$scope.viewModel.updateCommand = "removeSprint";
			$scope.viewModel.anexedEntity = lista[index];

			Api.Project.update({ id: id }, $scope.viewModel, function (data) {
				toastr.success('Sprint removed', 'Success');
				$scope.viewModel.sprints = data.sprints;
				$scope.viewModel.logs = data.logs;
			});
		}
	}

	$scope.addNewSprint = function () {
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
			$scope.addSprint = !$scope.addSprint;
	}

	$scope.editSprint = function (mdl) {
		$scope.addSprint = true;
		$scope.newSprint = mdl;
	}

	$scope.goSprint = function (mdl)
	{
		$state.go('sprint', { id: mdl.id });
	}

	$scope.newSprint =
		{
			fkProject: undefined,
			fkPhase: undefined,
			stName: ''			
		};

	$scope.saveNewSprint = function ()
	{
		$scope.fkSprintPhase_fail = $scope.newSprint.fkPhase == undefined;
		$scope.stSprint_fail = invalidCheck($scope.newSprint.stName);

		if (!$scope.fkSprintPhase_fail &&
			!$scope.stSprint_fail)
		{
			$scope.viewModel.updateCommand = "newSprint";
			$scope.viewModel.anexedEntity = $scope.newSprint;

			Api.Project.update({ id: id }, $scope.viewModel, function (data)
			{				
				$scope.newSprint =
				{
					fkProject: undefined,
					fkPhase: undefined,
					stName: ''
					
				};

				toastr.success('Sprint saved', 'Success');
				$scope.viewModel.sprints = data.sprints;
				$scope.viewModel.logs = data.logs;

				$scope.addSprint = false;

			}, function (response) {
				toastr.error(response.data.message, 'Error');
			});
		}
	}

}]);
