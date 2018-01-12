angular.module('app.controllers').controller('ListingUsersController',
['$scope', '$rootScope', '$state', 'Api', 'ngSelects', 
function ($scope, $rootScope, $state, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;
	
    function CheckPermissions()
    {
        Api.Permission.get({ id: $scope.permID }, function (data)
        {
			$scope.permModel = data;

            if (!$scope.permModel.listagem)
            {
                toastr.error('Accesso negado!', 'Permissão');
				$state.go('home');
			}
		},
		function (response) { });
	}

	init();

	function init()
    {
        $scope.campos = {
            ativo: 'true',
            selects: {
                perfil: ngSelects.obterConfiguracao(Api.ProfileCombo, {}),
            }
        };

        $scope.itensporpagina = 15;
        $scope.permModel = {};
        $scope.permID = 102;

		CheckPermissions();
	}

	$scope.search = function ()
	{
		$scope.load(0, $scope.itensporpagina);
		$scope.paginador.reiniciar();
	}

	$scope.load = function (skip, take)
	{
		$scope.loading = true;

        var opcoes = {            
            skip: skip,
            take: take,
            fkPerfil: $scope.campos.ativo,
            ativo: $scope.campos.ativo,
            busca: $scope.campos.busca,
            email: $scope.campos.email,
            phone: $scope.campos.phone,
        };

		Api.User.listPage(opcoes, function (data)
		{
			$scope.list = data.results;
			$scope.total = data.count;
			$scope.loading = false;
		});
	}

	$scope.show = function (mdl)
	{
		if (!$scope.permModel.visualizar) 
            toastr.error('Accesso negado!', 'Permissão');
		else
			$state.go('user', { id: mdl.id });
	}

	$scope.new = function ()
	{
		if (!$scope.permModel.novo)
            toastr.error('Accesso negado!', 'Permissão');
		else
			$state.go('user-new');
	}

}]);
