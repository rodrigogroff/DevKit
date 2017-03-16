angular.module('app.controllers').controller('ListingUserKanbanController',
['$scope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects', 
function ($scope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects )
{
	$scope.loading = false;

	$scope.permModel = {};
	$scope.viewModel = {};	
	$scope.permID = 107;

	function CheckPermissions()
	{
		Api.Permission.get({ id: $scope.permID }, function (data) {
			$scope.permModel = data;

			if (!$scope.permModel.listagem) {
				toastr.error('Access denied!', 'Permission');
				$state.go('home');
			}
		},
		function (response) { });
	}

	init();

	function init()
	{
		CheckPermissions();

		$scope.loading = true;

		var options = { };

		var filter = ngHistoricoFiltro.filtro.filtroGerado;

		if (filter)
			angular.extend(options, filter);

		Api.UserKanban.listPage(options, function (data)
		{
			$scope.viewModel = data;
			$scope.loading = false;
		});
	}

	$scope.search = function () {
		init();
	}

}]);
