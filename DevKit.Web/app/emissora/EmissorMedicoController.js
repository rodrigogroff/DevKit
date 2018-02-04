
angular.module('app.controllers').controller('EmissorMedicoController',
['$scope', '$state', '$stateParams', '$rootScope', 'Api', 'ngSelects', 
function ($scope, $state, $stateParams, $rootScope, Api, ngSelects)
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;
    
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

        Api.Permission.get({ id: $scope.auditLogPerm }, function (data)
        {
			$scope.auditLogView = $scope.permModel.visualizar;
		},
		function (response) { });
	}

	//function loadSetup()
	//{
 //       Api.Setup.get({ id: 1 }, function (data)
	//	{
	//		$scope.setupModel = data;
	//	});
	//}
	
	var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;
    
	init();

    function init()
    {
        //$scope.setupModel = { stPhoneMask: '' }
        $scope.viewModel = {};
        //$scope.permModel = {};
        //$scope.permID = 102;
        //$scope.auditLogPerm = 112;

        $scope.selectDayMonths = ngSelects.obterConfiguracao(Api.DayMonthCombo, {});
        $scope.selectMonths = ngSelects.obterConfiguracao(Api.MonthCombo, {});
        $scope.estado = ngSelects.obterConfiguracao(Api.EstadoCombo, {});
        $scope.cidade = ngSelects.obterConfiguracao(Api.CidadeCombo, { scope: $scope, filtro: { campo: 'fkEstado', valor: 'newEnd.fkEstado' } });

        //CheckPermissions();
        //loadSetup();
        loadEntity();
    }

    function loadEntity()
    {
		if (id > 0)
        {
            if ($scope.loaded == undefined)
                $scope.loading = true;

            Api.Medico.get({ id: id }, function (data)
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
		{
            $scope.viewModel = { bActive: true, nuYearBirth: 1980 };
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

    $scope.list = function ()
    {
        $state.go('listemissormedicos');
    }

    // ============================================
    // procedimentos
    // ============================================

    $scope.addEmpresa = false;

    $scope.removeEmpresa = function (index, lista) {
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
        //  toastr.error('Acesso negado!', 'Permissão');
        //else
        {
            $scope.modalEmpresa = true;
            $scope.delEmpresa = $scope.viewModel.empresas[index];
        }
    }

    $scope.closeModalEmpresa = function () {
        $scope.modalEmpresa = false;
    }

    $scope.removerEmpresaModal = function () {
        $scope.viewModel.updateCommand = "removeEmpresa";
        $scope.viewModel.anexedEntity = $scope.delEmpresa;

        Api.Medico.update({ id: id }, $scope.viewModel, function (data) {
            loadEntity();
        });
    }

    $scope.addNewEmpresa = function () {
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
        //  toastr.error('Acesso negado!', 'Permissão');
        //else
        $scope.addEmpresa = !$scope.addEmpresa;
    }

    $scope.newEmpresa = {};

    $scope.editEmpresa = function (mdl) {
        $scope.addEmpresa = true;
        $scope.newEmpresa = mdl;
    }

    $scope.cancelEmpresa = function () {
        $scope.addEmpresa = false;
        $scope.newEmpresa = {};
    }

    $scope.saveNewEmpresa = function () {
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
        //  toastr.error('Acesso negado!', 'Permissão');
        //else
        {
            //if (!$scope.stPhone_fail &&
              //  !$scope.stDescription_fail)
            {
                $scope.addEmpresa = false;

                $scope.viewModel.updateCommand = "newEmpresa";
                $scope.viewModel.anexedEntity = $scope.newEmpresa;

                Api.Medico.update({ id: id }, $scope.viewModel, function (data) {
                    $scope.newPhone = {};
                    loadEntity();
                },
                    function (response) {
                        toastr.error(response.data.message, 'Error');
                    });
            }
        }
    }

}]);
