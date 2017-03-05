'use strict';

angular.module('app.controllers').controller('UserController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$scope.style_error = { 'background-color': 'goldenrod' }
	$scope.setupModel = { stPhoneMask: '' }
	
	$scope.viewModel = {};
	$scope.permModel = {};
	$scope.setupModel = {};

	$scope.loading = false;
	$scope.permID = 102;

	function CheckPermissions() { Api.Permission.get({ id: $scope.permID }, function (data) { $scope.permModel = data; }, function (response) { }); }
	function loadSetup() { Api.Setup.get({ id: 1 }, function (data) { $scope.setupModel = data; }, function (response) { }); }
	
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

	$scope.errorMain = false;
	$scope.errorMainMsg = '';

	$scope.save = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			var _stLogin = true;

			if ($scope.viewModel.stLogin != undefined) // when new...
			{
				console.log($scope.viewModel.stLogin.length);

				if ($scope.viewModel.stLogin.length < 3)
					_stLogin = false;
			}				

			if (!_stLogin)
			{
				if (_stLogin)
					$scope.style_stLogin = $scope.style_error; else $scope.style_stLogin = {};

				$scope.errorMain = true;
				$scope.errorMainMsg = 'Fill the form with all the required fields';
			}
			else
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
	$scope.newPhone = { stPhone: '', stDescription: '' };

	$scope.errorPhone = false;
	$scope.errorPhoneMsg = '';
	
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

	$scope.saveNewPhone = function ()
	{
		var _stPhone = $scope.newPhone.stPhone != '';
		var _stDescription = $scope.newPhone.stDescription.length > 3;
		
		if (!_stPhone || !_stDescription)
		{
			if (!_stPhone) $scope.style_stPhone = $scope.style_error; else $scope.style_stPhone = {};
			if (!_stDescription) $scope.style_stDescription = $scope.style_error; else $scope.style_stDescription = {};

			$scope.errorPhone = true;
			$scope.errorPhoneMsg = 'Field validation failed';
		}
		else
		{
			$scope.errorPhone = false;
			
			$scope.addPhone = false;
			$scope.newPhone.fkUser = id;
			
			$scope.viewModel.updateCommand = "newPhone";
			$scope.viewModel.anexedEntity = $scope.newPhone;

			Api.User.update({ id: id }, $scope.viewModel, function (data)
			{				
				$scope.style_stPhone = {}
				$scope.style_stLocal = {}

				$scope.newPhone = { stPhone: '', stLocal: '' };

				toastr.success('Phone added', 'Success');

				$scope.viewModel.phones = data.phones;

			}, function (response) {
				toastr.error(response.data.message, 'Error');
			});
		}
	}

	// ============================================
	// email 
	// ============================================

	$scope.addEmail = false;
	$scope.newEmail = { stEmail: '' };

	$scope.errorEmail = false;
	$scope.errorEmailMsg = '';

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

	$scope.saveNewEmail = function ()
	{
		var _stEmail = ($scope.newEmail.stEmail.indexOf('@') > 0);

		if (!_stEmail)
		{
			if (!_stEmail) $scope.style_stEmail = $scope.style_error; else $scope.style_stEmail = {};

			$scope.errorEmail = true;
			$scope.errorEmailMsg = 'Field validation failed';
		}
		else
		{
			$scope.errorEmail = false;

			$scope.addEmail = false;
			$scope.newEmail.fkUser = id;

			$scope.viewModel.updateCommand = "newEmail";
			$scope.viewModel.anexedEntity = $scope.newEmail;

			Api.User.update({ id: id }, $scope.viewModel, function (data)
			{
				$scope.style_stEmail = {}
				
				$scope.newEmail = { stEmail: '' };

				toastr.success('Email added', 'Success');

				$scope.viewModel.emails = data.emails;

			}, function (response) {
				toastr.error(response.data.message, 'Error');
			});
		}
	}

}]);
