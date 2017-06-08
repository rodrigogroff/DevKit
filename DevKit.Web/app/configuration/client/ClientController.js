'use strict';

angular.module('app.controllers').controller('ClientController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$rootScope.exibirMenu = true;

	$scope.loading = false;

	$scope.setupModel = { stPhoneMask: '' }
	$scope.viewModel = {};
	$scope.permModel = {};	
	$scope.permID = 120;
	
	function CheckPermissions()
	{
        Api.Permission.get({ id: $scope.permID, login: $rootScope.loginInfo }, function (data)
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

        Api.Setup.get({ id: 1, login: $rootScope.loginInfo }, function (data) {
			$scope.setupModel = data;
		});

		if (id > 0)
		{
			$scope.loading = true;

            Api.Client.get({ id: id, login: $rootScope.loginInfo }, function (data)
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

	var invalidEmail = function (element) {
		if (element == undefined)
			return true;
		else {
			if (element.length == 0)
				return true;

			if (element.indexOf('@') > 1)
				return true;
		}

		return false;
	}

	$scope.save = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.stName_fail = invalidCheck($scope.viewModel.stName);
			$scope.stEmail_fail = invalidEmail($scope.viewModel.stContactEmail);
				
			if (!$scope.stName_fail && !$scope.stEmail_fail )
            {
                $scope.viewModel.login = $rootScope.loginInfo;

				if (id > 0)
				{
					$scope.viewModel.updateCommand = "entity";

                    Api.Client.update({ id: id }, $scope.viewModel, function (data)
					{
						toastr.success('Client saved!', 'Success');
						$scope.viewModel.logs = data.logs;
					},
					function (response)
					{
						toastr.error(response.data.message, 'Error');
					});
				}
				else
				{
                    Api.Client.add($scope.viewModel, function (data)
					{
						toastr.success('Client added!', 'Success');
						$state.go('client', { id: data.id });
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
		$state.go('clients');
	}

	$scope.remove = function ()
	{
		if (!$scope.permModel.remover)
			toastr.error('Access denied!', 'Permission');
		else
		{
            Api.Client.remove({ id: id, login: $rootScope.loginInfo }, function (data)
			{
				toastr.success('Client removed!', 'Success');
				$scope.list();
			},
			function (response) {
				toastr.error(response.data.message, 'Permission');
			});
		}
	}
		
}]);
