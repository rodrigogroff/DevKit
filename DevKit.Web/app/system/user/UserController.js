'use strict';

angular.module('app.controllers').controller('UserController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$scope.loading = false;

	$scope.setupModel = { stPhoneMask: '' }	
	$scope.viewModel = {};
	$scope.permModel = {};	
	$scope.permID = 102;

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

		$scope.selectPerfis = ngSelects.obterConfiguracao(Api.Profile, { tamanhoPagina: 15, campoNome: 'stName' });

		if (id > 0)
		{
			$scope.loading = true;
			Api.User.get({ id: id }, function (data)
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
			$scope.stLogin_fail = invalidCheck($scope.viewModel.stLogin);
			$scope.fkProfile_fail = $scope.viewModel.fkProfile == undefined;
	
			if (!$scope.stLogin_fail && !$scope.fkProfile_fail)
			{
				if (id > 0)
				{
					$scope.viewModel.updateCommand = "entity";

					Api.User.update({ id: id }, $scope.viewModel, function (data)
					{
						toastr.success('User saved!', 'Success');
					},
					function (response)
					{
						toastr.error(response.data.message, 'Error');
					});
				}
				else
				{
					Api.User.add($scope.viewModel, function (data)
					{
						toastr.success('User added!', 'Success');
						$state.go('user', { id: data.id });
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
		$state.go('users');
	}

	$scope.remove = function ()
	{
		if (!$scope.permModel.remover)
			toastr.error('Access denied!', 'Permission');
		else
		{
			Api.User.remove({ id: id }, {}, function (data)
			{
				toastr.success('User removed!', 'Success');
				$scope.list();
			},
			function (response)
			{
				toastr.error(response.data.message, 'Permission');
			});
		}			
	}

	// ============================================
	// phone 
	// ============================================

	$scope.addPhone = false;
		
	$scope.removePhone = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.viewModel.updateCommand = "removePhone";
			$scope.viewModel.anexedEntity = $scope.viewModel.phones[index];

			Api.User.update({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('Phone removed', 'Success');
				$scope.viewModel.phones = data.phones;
			});
		}
	}

	$scope.addNewPhone = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
			$scope.addPhone = !$scope.addPhone;
	}

	$scope.newPhone = { stPhone: '', stDescription: '' };

	$scope.saveNewPhone = function ()
	{
		$scope.stPhone_fail = false;
		$scope.stDescription_fail = false;

		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			if ($scope.newPhone.stPhone != undefined && $scope.newPhone.stPhone.length == 0)
				$scope.stPhone_fail = true;

			if ($scope.newPhone.stDescription != undefined && $scope.newPhone.stDescription.length == 0)
				$scope.stDescription_fail = true;
	
			if (!$scope.stPhone_fail && !$scope.stDescription_fail)
			{
				$scope.addPhone = false;

				$scope.viewModel.updateCommand = "newPhone";
				$scope.viewModel.anexedEntity = $scope.newPhone;

				Api.User.update({ id: id }, $scope.viewModel, function (data)
				{
					$scope.newPhone = { stPhone: '', stDescription: '' };
					toastr.success('Phone added', 'Success');
					$scope.viewModel.phones = data.phones;

				}, function (response) {
					toastr.error(response.data.message, 'Error');
				});
			}
		}
	}

	// ============================================
	// email 
	// ============================================

	$scope.addEmail = false;
	
	$scope.removeEmail = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.viewModel.updateCommand = "removeEmail";
			$scope.viewModel.anexedEntity = $scope.viewModel.emails[index];

			Api.User.update({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('Email removed', 'Success');
				$scope.viewModel.emails = data.emails;
			});
		}
	}

	$scope.addNewEmail = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
			$scope.addEmail = !$scope.addEmail;
	}

	$scope.newEmail = { stEmail: '' };

	$scope.saveNewEmail = function ()
	{
		$scope.stEmail_fail = false;
		
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			if ($scope.newEmail.stEmail != undefined && $scope.newEmail.stEmail.indexOf('@') <= 0)
				$scope.stEmail_fail = true;
	
			if (!$scope.stEmail_fail)
			{
				$scope.addEmail = false;

				$scope.viewModel.updateCommand = "newEmail";
				$scope.viewModel.anexedEntity = $scope.newEmail;

				Api.User.update({ id: id }, $scope.viewModel, function (data)
				{
					$scope.newEmail = { stEmail: '' };
					toastr.success('Email added', 'Success');
					$scope.viewModel.emails = data.emails;

				}, function (response) {
					toastr.error(response.data.message, 'Error');
				});
			}
		}
	}

}]);
