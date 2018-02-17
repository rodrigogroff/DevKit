
angular.module('app.controllers').controller('EmissorCredenciadoController',
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
                toastr.error('Acesso negado para visualização de credenciado!', 'Permissão');
				$state.go('home');
			}
		},
		function (response) { });
	}
	
	var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;
    
	init();

    function init()
    {
        $scope.viewModel = {};
        $scope.permModel = {};
        $scope.permID = 501;
        
        $scope.nomeTuss = '';

        CheckPermissions();
        loadEntity();
    }

    function loadEntity()
    {
		if (id > 0)
        {
            if ($scope.loaded == undefined)
                $scope.loading = true;

            Api.Credenciado.get({ id: id }, function (data)
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
        $state.go('listemissorcredenciados');
    }

    $scope.buscaTUSS = function ()
    {
        Api.TUSS.listPage({ codigo: $scope.newProcedimento.nuCodTUSS }, function (data)
        {
            if (data.results.length > 0)
                $scope.nomeTuss = data.results[0].stProcedimento;
        });
    }

    // ============================================
    // procedimentos
    // ============================================

    $scope.addProcedimento = false;

    $scope.removeProcedimento = function (index, lista) {
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
        //  toastr.error('Acesso negado!', 'Permissão');
        //else
        {
            $scope.modalProcedimento = true;
            $scope.delProcedimento = $scope.viewModel.procedimentos[index];
        }
    }

    $scope.closeModalProcedimento = function () {
        $scope.modalProcedimento = false;
    }

    $scope.removerProcedimentoModal = function () {
        $scope.viewModel.updateCommand = "removeProcedimentoViaEmissor";
        $scope.viewModel.anexedEntity = $scope.delProcedimento;

        Api.Credenciado.update({ id: id }, $scope.viewModel, function (data) {
            loadEntity();
        });
    }

    $scope.addNewProcedimento = function () {
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
        //  toastr.error('Acesso negado!', 'Permissão');
        //else
        $scope.addProcedimento = !$scope.addProcedimento;
    }

    $scope.newProcedimento = {};

    $scope.cancelProcedimento = function () {
        $scope.addProcedimento = false;
        $scope.newProcedimento = {};
    }

    $scope.saveNewProcedimento = function () {
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
        //  toastr.error('Acesso negado!', 'Permissão');
        //else
        {
            //if (!$scope.stPhone_fail &&
              //  !$scope.stDescription_fail)
            {
                $scope.addProcedimento = false;

                $scope.viewModel.updateCommand = "newProcedimentoViaEmissor";
                $scope.viewModel.anexedEntity = $scope.newProcedimento;

                Api.Credenciado.update({ id: id }, $scope.viewModel, function (data) {
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
