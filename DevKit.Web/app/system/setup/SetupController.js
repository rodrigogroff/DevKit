'use strict';

angular.module('app.controllers').controller('SetupController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$scope.loading = false;

	$scope.permModel = {};
	$scope.permID = 100;

	function CheckPermissions()
	{
		Api.Permission.get({ id: $scope.permID }, function (data)
		{
			$scope.permModel = data;

			if (!$scope.permModel.visualizar)
			{
				toastr.error('Access denied!', 'Permission');
				$state.go('home', {});
			}

		}, function (response) { });
	}

	init();

	function init()
	{
		CheckPermissions();

		$scope.loading = true;

		Api.Setup.get({ id: 1 }, function (data)
		{
			$scope.viewModel = data;
			$scope.loading = false;
		},
		function (response)
		{
			if (response.status === 404) { toastr.error('Invalid ID', 'Error'); }
		});
	}

	$scope.save = function ()
	{
		$scope.stPhoneMask_fail = false;
		$scope.stDateFormat_fail = false;

		if (!$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			if ($scope.viewModel.stPhoneMask != undefined && $scope.viewModel.stPhoneMask.length == 0)
				$scope.stPhoneMask_fail = true;

			if ($scope.viewModel.stDateFormat != undefined && $scope.viewModel.stDateFormat.length == 0)
				$scope.stDateFormat_fail = true;

			if (!$scope.stPhoneMask_fail && !$scope.stDateFormat_fail)
			{
				Api.Setup.update({ id: 1 }, $scope.viewModel, function (data)
				{
					toastr.success('Setup preferences saved!', 'Success');
				},
				function (response)
				{
					toastr.error(response.data.message, 'Error');
				});
			}
		}
	};

}]);
