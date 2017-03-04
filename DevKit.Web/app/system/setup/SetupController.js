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
			var _stName = true;

			if ($scope.viewModel.stName != undefined) // when new...
				if ($scope.viewModel.stName.length < 3)
					_stName = false;

			if (!_stName) 
			{
				if (_stName) 
					$scope.style_stName = $scope.style_error; else $scope.style_stName = {};

				$scope.errorMain = true;
				$scope.errorMainMsg = 'Fill the form with all the required fields';
			}
			else
			{
				$scope.errorMain = false;
				$scope.errorMainMsg = '';

				if (id > 0)
				{
					Api.Profile.update({ id: id }, $scope.viewModel, function (data)
					{
						toastr.success('Profile saved!', 'Success');
					},
					function (response)
					{
						toastr.error(response.data.message, 'Error');
					});
				}
				else
				{
					Api.Profile.add($scope.viewModel, function (data)
					{
						toastr.success('Profile added!', 'Success');
						$state.go('profile', { id: data.id });
					},
					function (response)
					{
						toastr.error(response.data.message, 'Error');
					});
				}
			}
		}
	};

}]);
