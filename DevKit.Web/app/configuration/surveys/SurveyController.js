'use strict';

angular.module('app.controllers').controller('SurveyController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects', 
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects )
{
	$rootScope.exibirMenu = true;

	$scope.selectProject = ngSelects.obterConfiguracao(Api.ProjectCombo, {});

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
                toastr.error('Acesso negado!', 'Permissão');
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
            toastr.error('Acesso negado!', 'Permissão');
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
						toastr.success('Pesquisa salva!', 'Sucesso');
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
                        toastr.success('Pesquisa adicionada!', 'Sucesso');

                        $state.go('surveysListing');
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
            toastr.error('Acesso negado!', 'Permissão');
		else
		{
            Api.Survey.remove({ id: id }, function (data)
			{
				toastr.success('Pesquisa removida!', 'Sucesso');
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
            toastr.error('Acesso negado!', 'Permissão');
        else
        {
			$scope.viewModel.updateCommand = "removeOption";
			$scope.viewModel.anexedEntity = $scope.viewModel.options[index];

			Api.Survey.update({ id: id }, $scope.viewModel, function (data) {
                toastr.success('Opção removida', 'Sucesso');
                init();
			});
		}
	}

	$scope.addNewOption = function () {
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
			$scope.addOption = !$scope.addOption;
	}

	$scope.newOption = { };

	$scope.editOption = function (mdl) {
		$scope.addOption = true;
		$scope.newOption = mdl;
	}

	$scope.cancelOption = function () {
		$scope.addOption = false;
		$scope.newOption = {};
	}

	$scope.saveNewOption = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
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
					$scope.newOption = {};
					toastr.success('Opção salva', 'Sucesso');

                    init();
				},
				function (response) {
					toastr.error(response.data.message, 'Error');
				});
			}
		}
	}

}]);
