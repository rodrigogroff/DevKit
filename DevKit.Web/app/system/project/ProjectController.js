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
			if ($scope.viewModel.stName != undefined && $scope.viewModel.stName.length == 0)
				$scope.stName_fail = true;
	
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
			$scope.viewModel.anexedEntity = $scope.viewModel.users[index];

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
		$scope.fkUser_fail = false;
		$scope.stRole_fail = false;

		if ($scope.newUser.fkUser == undefined )
			$scope.fkUser_fail = true;

		if ($scope.newUser.stRole != undefined && $scope.newUser.stRole.length == 0)
			$scope.stRole_fail = true;
	
		if (!$scope.fkUser_fail && !$scope.stRole_fail)
		{
			$scope.viewModel.updateCommand = "newUser";
			$scope.viewModel.anexedEntity = $scope.newUser;

			Api.Project.update({ id: id }, $scope.viewModel, function (data)
			{
				$scope.style_stRole = {}

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

}]);
