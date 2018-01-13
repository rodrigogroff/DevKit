
angular.module('app.controllers').controller('NewsController',
['$scope', '$state', '$stateParams', '$rootScope', 'Api', 'ngSelects', 
function ($scope, $state, $stateParams, $rootScope, Api, ngSelects )
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

		$scope.selectProject = ngSelects.obterConfiguracao(Api.ProjectCombo, {});
		
		if (id > 0)
        {
            if ($scope.loaded == undefined)
			    $scope.loading = true;

            Api.News.get({ id: id }, function (data)
			{
				$scope.viewModel = data;
                $scope.loading = false;
                $scope.loaded = true;
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

					Api.News.update({ id: id }, $scope.viewModel, function (data)
					{
						toastr.success('Notícia salva!', 'Sucesso');
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
                        toastr.success('Notícia adicionada!', 'Sucesso');
                        $state.go('newsListing');
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
            toastr.error('Acesso negado!', 'Permissão');
		else
		{
            Api.News.remove({ id: id }, function (data)
			{
				toastr.success('Notícia removedida!', 'Sucesso');
				$scope.list();
			},
			function (response) {
				toastr.error(response.data.message, 'Permission');
			});
		}
	}		
	
}]);
