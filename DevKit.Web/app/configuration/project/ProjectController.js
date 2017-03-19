'use strict';

angular.module('app.controllers').controller('ProjectController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$scope.loading = false;

	$scope.viewModel = {};
	$scope.permModel = {};	
	$scope.permID = 103;

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

		$scope.selectUsers = ngSelects.obterConfiguracao(Api.User, { tamanhoPagina: 15, campoNome: 'stLogin' });

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
	
			if (!$scope.stName_fail)
			{
				if (id > 0)
				{
					$scope.viewModel.updateCommand = "entity";

					Api.Project.update({ id: id }, $scope.viewModel, function (data)
					{
						toastr.success('Project saved!', 'Success');
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

				$scope.addPhase = false;

			}, function (response) {
				toastr.error(response.data.message, 'Error');
			});
		}
	}

}]);
