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

		$scope.selectPriority = ngSelects.obterConfiguracao(Api.Priority, { tamanhoPagina: 15, campoNome: 'stName' });

		$scope.selectProjects = ngSelects.obterConfiguracao(Api.Project, { tamanhoPagina: 15, campoNome: 'stName' });

		$scope.selectPhases = ngSelects.obterConfiguracao(Api.Phase,
			{
				tamanhoPagina: 15,
				scope: $scope,
				filtro:
					{
						campo: 'idProject',
						valor: 'viewModel.fkProject'
					}
			});

		$scope.selectSprints = ngSelects.obterConfiguracao(Api.Sprint,
			{
				tamanhoPagina: 15,
				scope: $scope,
				filtro:
					{
						campo: 'idPhase',
						valor: 'viewModel.fkPhase'
					}
			});

		$scope.selectVersions = ngSelects.obterConfiguracao(Api.Version,
			{
				tamanhoPagina: 15,
				scope: $scope,
				filtro:
					{
						campo: 'idSprint',
						valor: 'viewModel.fkSprint'
					}
			});

		$scope.selectUsers = ngSelects.obterConfiguracao(Api.User, { tamanhoPagina: 15, campoNome: 'stLogin' });

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
		$scope.fkProject_fail = false;
		$scope.fkPhase_fail = false;
		$scope.fkSprint_fail = false;
		$scope.fkVersion_fail = false;

		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Access denied!', 'Permission');
		else
		{
			if ($scope.viewModel.stTitle == undefined) $scope.stTitle_fail = true;
			else if ($scope.viewModel.stTitle.length == 0) $scope.stTitle_fail = true;

			if ($scope.viewModel.stLocalization == undefined) $scope.stLocalization_fail = true;
			else if ($scope.viewModel.stLocalization.length == 0) $scope.stLocalization_fail = true;

			if ($scope.viewModel.stDescription == undefined) $scope.stDescription_fail = true;
			else if ($scope.viewModel.stDescription.length == 0) $scope.stDescription_fail = true;

			if ($scope.viewModel.fkProject == undefined) $scope.fkProject_fail = true;
			if ($scope.viewModel.fkPhase == undefined) $scope.fkPhase_fail = true;
			if ($scope.viewModel.fkSprint == undefined) $scope.fkSprint_fail = true;
			if ($scope.viewModel.fkVersion == undefined) $scope.fkVersion_fail = true;
	
			if (!$scope.stTitle_fail &&
				!$scope.stLocalization_fail &&
				!$scope.stDescription_fail &&
				!$scope.fkProject_fail &&
				!$scope.fkPhase_fail && 
				!$scope.fkSprint_fail && 
				!$scope.fkVersion_fail )
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
