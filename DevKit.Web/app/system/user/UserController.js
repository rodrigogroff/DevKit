'use strict';

angular.module('app.controllers').controller('UserController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects', 
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$rootScope.exibirMenu = true;

	$scope.selectPerfis = ngSelects.obterConfiguracao(Api.ProfileCombo, {});

	$scope.loading = false;

	$scope.setupModel = { stPhoneMask: '' }	
	$scope.viewModel = {};
	$scope.permModel = {};	
	$scope.permID = 102;
	$scope.auditLogPerm = 112;

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

	var invalidEmail = function (element) {
		if (element == undefined)
			return true;
		else
		{
			if (element.length == 0)
				return true;

			if (element.indexOf('@') > 1)
				return true;
		}			

		return false;
	}

	$scope.save = function ()
	{
		$scope.resetPassword = undefined;

		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.stLogin_fail = invalidCheck($scope.viewModel.stLogin);
			$scope.fkProfile_fail = $scope.viewModel.fkProfile == undefined;
	
            if (!$scope.stLogin_fail &&
                !$scope.fkProfile_fail)
            {
				if (id > 0)
                {
					$scope.viewModel.updateCommand = "entity";

                    Api.User.update({ id: id }, $scope.viewModel, function (data)
					{
						toastr.success('User saved!', 'Success');
                        init();
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
                        $state.go('users');
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

	$scope.resetPass = function ()
    {
		$scope.viewModel.updateCommand = "resetPassword";

        Api.User.update({ id: id }, $scope.viewModel, function (data)
		{			
			$scope.resetPassword = data.resetPassword;
		},
		function (response) {
			toastr.error(response.data.message, 'Error');
		});
	}

	$scope.remove = function ()
	{
		if (!$scope.permModel.remover)
			toastr.error('Access denied!', 'Permission');
		else
		{
            Api.User.remove({ id: id }, function (data)
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

            Api.User.update({ id: id }, $scope.viewModel, function (data) {
		        toastr.success('Phone removed', 'Success');
                init();
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

	$scope.newPhone = { };

	$scope.editPhone = function (mdl)
	{
		$scope.addPhone = true;
		$scope.newPhone = mdl;
	}

	$scope.cancelPhone = function () {
		$scope.addPhone = false;
		$scope.newPhone = {};
	}

	$scope.saveNewPhone = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.stPhone_fail = invalidCheck($scope.newPhone.stPhone);
			$scope.stDescription_fail = invalidCheck($scope.newPhone.stDescription);
	
			if (!$scope.stPhone_fail &&
				!$scope.stDescription_fail)
			{
				$scope.addPhone = false;
                
				$scope.viewModel.updateCommand = "newPhone";
				$scope.viewModel.anexedEntity = $scope.newPhone;

                Api.User.update({ id: id }, $scope.viewModel, function (data)
				{
					$scope.newPhone = {};
					toastr.success('Phone saved', 'Success');					
                    init();
				},
				function (response) {
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
                init();
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

	$scope.newEmail = { };

	$scope.editEmail = function (mdl) {
		$scope.addEmail = true;
		$scope.newEmail = mdl;
	}

	$scope.cancelEmail = function () {
		$scope.addEmail = false;
		$scope.newEmail = {};
	}

	$scope.saveNewEmail = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.stEmail_fail = invalidEmail($scope.newEmail.stEmail) ;

			if (!$scope.stEmail_fail)
			{
				$scope.addEmail = false;
                
				$scope.viewModel.updateCommand = "newEmail";
				$scope.viewModel.anexedEntity = $scope.newEmail;

                Api.User.update({ id: id }, $scope.viewModel, function (data)
                {
                    $scope.newEmail = {};
                    toastr.success('Email saved', 'Success');
                    init();
				},
				function (response) {
					toastr.error(response.data.message, 'Error');
				});
			}
		}
	}

}]);
