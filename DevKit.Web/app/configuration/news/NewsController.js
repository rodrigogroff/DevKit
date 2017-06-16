'use strict';

angular.module('app.controllers').controller('NewsController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects', 
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects )
{
	$rootScope.exibirMenu = true;

	$scope.loading = false;

	$scope.viewModel = {};
	$scope.permModel = {};	
	$scope.permID = 118;
	
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

		$scope.selectProject = ngSelects.obterConfiguracao(Api.Project, {});
		
		if (id > 0)
		{
			$scope.loading = true;

            Api.News.get({ id: id }, function (data)
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

					Api.News.update({ id: id }, $scope.viewModel, function (data)
					{
						toastr.success('News saved!', 'Success');
					},
					function (response)
					{
						toastr.error(response.data.message, 'Error');
					});
				}
				else
				{
					Api.News.add($scope.viewModel, function (data)
					{
						toastr.success('News added!', 'Success');
						$state.go('news', { id: data.id });
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
		$state.go('newsListing');
	}

	$scope.remove = function ()
	{
		if (!$scope.permModel.remover)
			toastr.error('Access denied!', 'Permission');
		else
		{
            Api.News.remove({ id: id }, function (data)
			{
				toastr.success('News removed!', 'Success');
				$scope.list();
			},
			function (response) {
				toastr.error(response.data.message, 'Permission');
			});
		}
	}		
	
}]);
