'use strict';

angular.module('app.controllers').controller('ProjectController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$scope.style_error = { 'background-color': 'goldenrod' }
	
	$scope.viewModel = {};
	$scope.permModel = {};

	$scope.loading = false;
	$scope.permID = 103;

	function CheckPermissions()
	{
		Api.Permission.get({ id: $scope.permID }, function (data) {
			$scope.permModel = data;

			if (!$scope.permModel.visualizar) {
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

	$scope.errorMain = false;
	$scope.errorMainMsg = '';

	$scope.save = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			var _stName = true;

			if ($scope.viewModel.stName != undefined) // when new...
			{
				console.log($scope.viewModel.stName.length);

				if ($scope.viewModel.stName.length < 3)
					_stName = false;
			}

			if (!_stName) {
				if (_stName)
					$scope.style_stName = $scope.style_error; else $scope.style_stName = {};

				$scope.errorMain = true;
				$scope.errorMainMsg = 'Fill the form with all the required fields';
			}
			else
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
	$scope.newUser = { fkUser: undefined, stRole: '', fkProject: undefined };

	$scope.errorUser = false;
	$scope.errorUserMsg = '';

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

	$scope.saveNewUser = function ()
	{
		var _stfkUser = ($scope.newUser.fkUser != undefined);
		var _stRole = ($scope.newUser.stRole.length > 2);

		console.log($scope.newUser.fkUser);
		console.log(_stfkUser);

		if (!_stRole || !_stfkUser)
		{
			if (!_stRole) $scope.style_stRole = $scope.style_error; else $scope.style_stRole = {};

			$scope.errorUser = true;
			$scope.errorUserMsg = 'Field validation failed';
		}
		else
		{
			$scope.errorUser = false;
			$scope.addUser = false;

			$scope.viewModel.updateCommand = "newUser";
			$scope.viewModel.anexedEntity = $scope.newUser;

			$scope.newUser.fkProject = id;

			Api.Project.update({ id: id }, $scope.viewModel, function (data)
			{
				$scope.style_stRole = {}

				$scope.newUser = { fkUser: undefined, stRole: '', fkProject: undefined };

				toastr.success('User added', 'Success');
				$scope.viewModel.users = data.users;

			}, function (response)
			{
				toastr.error(response.data.message, 'Error');
			});
		}
	}

}]);
