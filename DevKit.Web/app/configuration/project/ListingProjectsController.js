angular.module('app.controllers').controller('ListingProjectsController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects', '$http',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects, $http)
{
	$rootScope.exibirMenu = true;

	$scope.loading = false;
	$scope.campos = {
		selects: {
			user: ngSelects.obterConfiguracao(Api.UserCombo, { }),
		}
	};
	$scope.itensporpagina = 15;

	$scope.permModel = {};	
	$scope.permID = 103;

	function CheckPermissions()
	{
        Api.Permission.get({ id: $scope.permID }, function (data)
		{
			$scope.permModel = data;

			if (!$scope.permModel.listagem)
			{
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

		if (ngHistoricoFiltro.filtro)
            ngHistoricoFiltro.filtro.exibeFiltro = false;

      // loadAPICore(0, 15);
	}
	
	$scope.search = function ()
    {        
		$scope.load(0, $scope.itensporpagina);
		$scope.paginador.reiniciar();
	}

    function loadAPICore(skip, take)
    {
        $http({
            method: "GET",
            url: "http://localhost:52851/api/project",
            params: { skip: skip, take: take, login: $rootScope.loginInfo },
            withCredentials: true,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': '*'
            }

        }).success(function (data) {

            console.log(data);

        }).error(function (data) {

            console.log(data);

        });;
    };

	$scope.load = function (skip, take)
	{
		$scope.loading = true;

        var opcoes = { skip: skip, take: take };

		var filtro = ngHistoricoFiltro.filtro.filtroGerado;

		if (filtro)
			angular.extend(opcoes, filtro);

		delete opcoes.selects;

		Api.Project.listPage(opcoes, function (data) {
			$scope.list = data.results;
			$scope.total = data.count;		
			$scope.loading = false;
		});
	}

	$scope.show = function (mdl)
	{
		if (!$scope.permModel.visualizar) 
			toastr.error('Access denied!', 'Permission');
		else
			$state.go('project', { id: mdl.id });
	}

	$scope.new = function ()
	{
		if (!$scope.permModel.novo)
			toastr.error('Access denied!', 'Permission');
		else
			$state.go('project-new');
	}
	
}]);
