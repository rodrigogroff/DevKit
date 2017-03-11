'use strict';

angular.module('app.controllers').controller('TaskController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
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

	$scope.save = function ()
	{
		$scope.stTitle_fail = false;
		$scope.stLocalization_fail = false;
		$scope.stDescription_fail = false;

		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			if ($scope.viewModel.stTitle != undefined && $scope.viewModel.stTitle.length == 0)
				$scope.stTitle_fail = true;

			if ($scope.viewModel.stLocalization != undefined && $scope.viewModel.stLocalization.length == 0)
				$scope.stLocalization_fail = true;

			if ($scope.viewModel.stDescription != undefined && $scope.viewModel.stDescription.length == 0)
				$scope.stDescription_fail = true;
	
			if (!$scope.stTitle_fail &&
				!$scope.stLocalization_fail &&
				!$scope.stDescription_fail)
			{
				if (id > 0)
				{
					$scope.viewModel.updateCommand = "entity";

					Api.Task.update({ id: id }, $scope.viewModel, function (data)
					{
						toastr.success('Task saved!', 'Success');
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
			Api.Task.remove({ id: id }, {}, function (data)
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

}]);
