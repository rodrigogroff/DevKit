'use strict';

angular.module('app.controllers').controller('SetupController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$scope.style_error = { 'background-color': 'goldenrod' }
	$scope.permModel = {};
	$scope.loading = false;
	$scope.permID = 100;

	function CheckPermissions()
	{
		Api.Permission.get({ id: $scope.permID }, function (data)
		{
			$scope.permModel = data;

			if (!$scope.permModel.visualizar)
			{
				toastr.error('Access denied!', 'Permission');
				$state.go('home', { });
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
			$scope.list();
		});
	}

	$scope.errorMain = false;
	$scope.errorMainMsg = '';

	$scope.save = function ()
	{
		if (!$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			var _stPhoneMask = true;
			var _stDateFormat = true;

			if ($scope.viewModel.stPhoneMask != undefined) // when new...
				if ($scope.viewModel.stPhoneMask.length < 5)
					_stPhoneMask = false;

			if ($scope.viewModel.stDateFormat != undefined) // when new...
				if ($scope.viewModel.stDateFormat.length < 5)
					_stDateFormat = false;

			if (!_stPhoneMask || !_stDateFormat)
			{
				if (!_stPhoneMask)
					$scope.style_stPhoneMask = $scope.style_error; else $scope.style_stPhoneMask = {};

				if (!_stDateFormat)
					$scope.style_stDateFormat = $scope.style_error; else $scope.style_stDateFormat = {};

				$scope.errorMain = true;
				$scope.errorMainMsg = 'Fill the form with all the required fields';
			}
			else
			{
				$scope.errorMain = false;
				$scope.errorMainMsg = '';

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
