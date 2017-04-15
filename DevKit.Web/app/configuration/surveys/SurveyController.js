'use strict';

angular.module('app.controllers').controller('SurveyController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects', 
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects )
{
	$scope.selectProject = ngSelects.obterConfiguracao(Api.Project, {});

	$scope.loading = false;

	$scope.viewModel = {};
	$scope.permModel = {};	
	$scope.permID = 119;
	
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
	
	var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

	init();

	function init()
	{
		CheckPermissions();

		if (id > 0)
		{
			$scope.loading = true;

			Api.Survey.get({ id: id }, function (data)
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
			$scope.viewModel = { bActive: true };
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
			$scope.stTitle_fail = invalidCheck($scope.viewModel.stTitle);
			$scope.stMessage_fail = invalidCheck($scope.viewModel.stMessage);
	
			if (!$scope.stTitle_fail &&
				!$scope.stMessage_fail)
			{
				if (id > 0)
				{
					$scope.viewModel.updateCommand = "entity";

					Api.Survey.update({ id: id }, $scope.viewModel, function (data)
					{
						toastr.success('Survey saved!', 'Success');
					},
					function (response)
					{
						toastr.error(response.data.message, 'Error');
					});
				}
				else
				{
					Api.Survey.add($scope.viewModel, function (data)
					{
						toastr.success('Survey added!', 'Success');
						$state.go('survey', { id: data.id });
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
		$state.go('surveysListing');
	}

	$scope.remove = function ()
	{
		if (!$scope.permModel.remover)
			toastr.error('Access denied!', 'Permission');
		else
		{
			Api.Survey.remove({ id: id }, {}, function (data)
			{
				toastr.success('Survey removed!', 'Success');
				$scope.list();
			},
			function (response) {
				toastr.error(response.data.message, 'Permission');
			});
		}
	}		
	
	// ============================================
	// options 
	// ============================================

	$scope.addOption = false;

	$scope.removeOption = function (index, lista) {
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else {
			$scope.viewModel.updateCommand = "removeOption";
			$scope.viewModel.anexedEntity = $scope.viewModel.options[index];

			Api.Survey.update({ id: id }, $scope.viewModel, function (data) {
				toastr.success('Phone removed', 'Success');
				$scope.viewModel.options = data.options;
			});
		}
	}

	$scope.addNewOption = function () {
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
			$scope.addOption = !$scope.addOption;
	}

	$scope.newOption = { nuOrder: '', stOption: '' };

	$scope.editOption = function (mdl) {
		$scope.addOption = true;
		$scope.newOption = mdl;
	}

	$scope.saveNewOption = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			$scope.stOrder_fail = invalidCheck($scope.newOption.nuOrder);
			$scope.stOption_fail = invalidCheck($scope.newOption.stOption);

			if (!$scope.stOrder_fail &&
				!$scope.stOption_fail)
			{
				$scope.addOption = false;

				$scope.viewModel.updateCommand = "newOption";
				$scope.viewModel.anexedEntity = $scope.newOption;

				Api.Survey.update({ id: id }, $scope.viewModel, function (data)
				{
					$scope.newOption = { nuOrder: '', stOption: '' };
					toastr.success('Option saved', 'Success');
					$scope.viewModel.options = data.options;
				},
				function (response) {
					toastr.error(response.data.message, 'Error');
				});
			}
		}
	}

}]);
