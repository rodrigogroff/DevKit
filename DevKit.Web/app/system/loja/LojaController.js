'use strict';

angular.module('app.controllers').controller('LojaController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;

	$scope.viewModel = { };
	
	var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;
	
	init();

	function init()
	{
		if (id > 0)
        {
            $scope.loading = true;

            Api.Loja.get({ id: id }, function (data)
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
		$scope.nome_fail = invalidCheck($scope.viewModel.nome);

		if (!$scope.nome_fail) 
		{
			if (id > 0)
            {
				Api.Loja.update({ id: id }, $scope.viewModel, function (data) {
					toastr.success('Loja salva!', 'Sucesso');
				},
				function (response) {
					toastr.error(response.data.message, 'Error');
				});
			}
			else
			{
				/*Api.Profile.add($scope.viewModel, function (data)
				{
					toastr.success('Perfil adicionado!', 'Suceso');
                    $state.go('profiles'); $state.go('profiles');
				},
				function (response)
				{
					toastr.error(response.data.message, 'Error');
				});*/
			}
		}
	
	};

	$scope.list = function () {
		$state.go('lojas');
	}

}]);
