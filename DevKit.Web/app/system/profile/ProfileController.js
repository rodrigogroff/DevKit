'use strict';

angular.module('app.controllers').controller('ProfileController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$scope.style_error = { 'background-color': 'goldenrod' }
	$scope.permModel = {};
	$scope.loading = false;
	$scope.permID = 101;

	function CheckPermissions() { Api.Permission.get({ id: $scope.permID }, function (data) { $scope.permModel = data; }, function (response) { }); }

	$scope.viewModel =
		{
			// ## System #########################

			// profiles
			tg1011: false, tg1012: false, tg1013: false, tg1014: false, tg1015: false,
			// users
			tg1021: false, tg1022: false, tg1023: false, tg1024: false, tg1025: false,
		};
	
	var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;
	
	init();

	function init()
	{
		CheckPermissions();

		if (id > 0)
		{
			$scope.loading = true;

			Api.Profile.get({ id: id }, function (data)
			{
				// profiles
				if (data.stPermissions.indexOf('|1011|') >= 0) data.tg1011 = true; else data.tg1011 = false;
				if (data.stPermissions.indexOf('|1012|') >= 0) data.tg1012 = true; else data.tg1012 = false;
				if (data.stPermissions.indexOf('|1013|') >= 0) data.tg1013 = true; else data.tg1013 = false;
				if (data.stPermissions.indexOf('|1014|') >= 0) data.tg1014 = true; else data.tg1014 = false;
				if (data.stPermissions.indexOf('|1015|') >= 0) data.tg1015 = true; else data.tg1015 = false;

				// users
				if (data.stPermissions.indexOf('|1021|') >= 0) data.tg1021 = true; else data.tg1021 = false;
				if (data.stPermissions.indexOf('|1022|') >= 0) data.tg1022 = true; else data.tg1022 = false;
				if (data.stPermissions.indexOf('|1023|') >= 0) data.tg1023 = true; else data.tg1023 = false;
				if (data.stPermissions.indexOf('|1024|') >= 0) data.tg1024 = true; else data.tg1024 = false;
				if (data.stPermissions.indexOf('|1025|') >= 0) data.tg1025 = true; else data.tg1025 = false;
				
				$scope.viewModel = data;

				$scope.loading = false;

			},
			function (response)
			{
				if (response.status === 404) { toastr.error('Invalid profile', 'Erro'); }
				$scope.list();
			});
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
				_stName = $scope.viewModel.stName.length < 3;

			if (_stName) 
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

				var perms = ''; var _mdl = $scope.viewModel;

				// profiles
				if (_mdl.tg1011 == true) perms += '|1011|'; if (_mdl.tg1012 == true) perms += '|1012|'; if (_mdl.tg1013 == true) perms += '|1013|';
				if (_mdl.tg1014 == true) perms += '|1014|'; if (_mdl.tg1015 == true) perms += '|1015|';

				// users
				if (_mdl.tg1021 == true) perms += '|1021|'; if (_mdl.tg1022 == true) perms += '|1022|'; if (_mdl.tg1023 == true) perms += '|1023|';
				if (_mdl.tg1024 == true) perms += '|1024|'; if (_mdl.tg1025 == true) perms += '|1025|';

				$scope.viewModel.stPermissions = perms;

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
						toastr.success('Profile saved!', 'Success');
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

	$scope.list = function () {
		$state.go('profiles');
	}

	$scope.remove = function ()
	{
		if (!$scope.permModel.remover)
			toastr.error('Access denied!', 'Permission');
		else
		{
			Api.Profile.remove({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('Profile removed!', 'Success');
				$scope.list();
			},
			function (response)
			{
				toastr.error(response.data.message, 'Error');
			});
		}
	}

	$scope.showUser = function (mdl)
	{
		$state.go('user', { id: mdl.id });
	}	

}]);
