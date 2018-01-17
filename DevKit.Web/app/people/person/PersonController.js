
angular.module('app.controllers').controller('PersonController',
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

	function loadSetup()
	{
        Api.Setup.get({ id: 1 }, function (data)
		{
			$scope.setupModel = data;
		});
	}
	
	var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;
    
	init();

    function init() {
        $scope.setupModel = { stPhoneMask: '' }
        $scope.viewModel = {};
        $scope.permModel = {};
        $scope.permID = 102;
        $scope.auditLogPerm = 112;

        $scope.selectDayMonths = ngSelects.obterConfiguracao(Api.DayMonthCombo, {});
        $scope.selectMonths = ngSelects.obterConfiguracao(Api.MonthCombo, {});
        $scope.estado = ngSelects.obterConfiguracao(Api.EstadoCombo, {});
        $scope.cidade = ngSelects.obterConfiguracao(Api.CidadeCombo, { scope: $scope, filtro: { campo: 'fkEstado', valor: 'newEnd.fkEstado' } });

        CheckPermissions();
        loadSetup();
        loadEntity();
    }

    function loadEntity()
    {
		if (id > 0)
        {
            if ($scope.loaded == undefined)
                $scope.loading = true;

            Api.Person.get({ id: id }, function (data)
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

	var invalidEmail = function (element) {
		if (element == undefined)
			return true;
		else
		{
			if (element.length == 0)
				return true;

			if (element.indexOf('@') < 1)
				return true;
		}			

		return false;
	}

	$scope.save = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
		{
			$scope.stName_fail = invalidCheck($scope.viewModel.stName);
            $scope.mat_fail = invalidCheck($scope.viewModel.nuMatricula);
            $scope.cpf_fail = invalidCheck($scope.viewModel.stCPF);
	
            if (!$scope.stName_fail)
            {
				if (id > 0)
                {
					$scope.viewModel.updateCommand = "entity";

                    Api.Person.update({ id: id }, $scope.viewModel, function (data)
					{
                        toastr.success('Cadastro atualizado!', 'Sucesso');
					},
					function (response)
					{
						toastr.error(response.data.message, 'Error');
					});
				}
				else
				{
                    Api.Person.add($scope.viewModel, function (data)
					{
                        toastr.success('Cadastro adicionado!', 'Sucesso');                       
                        $state.go('person', { id: data.id });
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
		$state.go('persons');
	}

	$scope.remove = function ()
	{
		if (!$scope.permModel.remover)
            toastr.error('Acesso negado!', 'Permissão');
		else
		{
            Api.Person.remove({ id: id }, function (data)
			{
                toastr.success('Cadastro removido!', 'Sucesso');
				$scope.list();
			},
			function (response)
			{
				toastr.error(response.data.message, 'Permission');
			});
		}			
	}

	// ============================================
	// phone 
	// ============================================

	$scope.addPhone = false;
		
    $scope.removePhone = function (index, lista)
    {
        if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
        else
        {
            $scope.modalPhone = true;
            $scope.delPhone = $scope.viewModel.phones[index];
        }
    }

    $scope.closeModalPhone = function () {        
        $scope.modalPhone = false;
    }

    $scope.removerPhoneModal = function ()
    {
        $scope.viewModel.updateCommand = "removePhone";
        $scope.viewModel.anexedEntity = $scope.delPhone;

        Api.Person.update({ id: id }, $scope.viewModel, function (data) {
            loadEntity();
		});
	}

	$scope.addNewPhone = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
			$scope.addPhone = !$scope.addPhone;
	}

	$scope.newPhone = { };

	$scope.editPhone = function (mdl)
	{
		$scope.addPhone = true;
		$scope.newPhone = mdl;
	}

	$scope.cancelPhone = function () {
		$scope.addPhone = false;
		$scope.newPhone = {};
	}

	$scope.saveNewPhone = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
		{
			$scope.stPhone_fail = invalidCheck($scope.newPhone.stPhone);
			$scope.stDescription_fail = invalidCheck($scope.newPhone.stDescription);
	
			if (!$scope.stPhone_fail &&
				!$scope.stDescription_fail)
			{
				$scope.addPhone = false;
                
				$scope.viewModel.updateCommand = "newPhone";
				$scope.viewModel.anexedEntity = $scope.newPhone;

                Api.Person.update({ id: id }, $scope.viewModel, function (data)
				{
					$scope.newPhone = {};
                    loadEntity();
				},
				function (response) {
					toastr.error(response.data.message, 'Error');
				});
			}
		}
	}

	// ============================================
	// email 
	// ============================================

	$scope.addEmail = false;

    $scope.removeEmail = function (index, lista) {
        if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
        else {
            $scope.modalEmail = true;
            $scope.delEmail = $scope.viewModel.emails[index];
        }
    }

    $scope.closeModalEmail = function () {
        $scope.modalEmail = false;
    }

    $scope.removerEmailModal = function () {
        $scope.viewModel.updateCommand = "removeEmail";
        $scope.viewModel.anexedEntity = $scope.delEmail;

        Api.Person.update({ id: id }, $scope.viewModel, function (data) {
            loadEntity();
        });
    }

	$scope.addNewEmail = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
			$scope.addEmail = !$scope.addEmail;
	}

	$scope.newEmail = { };

	$scope.editEmail = function (mdl) {
		$scope.addEmail = true;
		$scope.newEmail = mdl;
	}

	$scope.cancelEmail = function () {
		$scope.addEmail = false;
		$scope.newEmail = {};
	}

	$scope.saveNewEmail = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
		{
			$scope.stEmail_fail = invalidEmail($scope.newEmail.stEmail) ;

			if (!$scope.stEmail_fail)
			{
				$scope.addEmail = false;
                
				$scope.viewModel.updateCommand = "newEmail";
				$scope.viewModel.anexedEntity = $scope.newEmail;

                Api.Person.update({ id: id }, $scope.viewModel, function (data)
                {
                    $scope.newEmail = {};
                    loadEntity();
				},
				function (response) {
					toastr.error(response.data.message, 'Error');
				});
			}
		}
    }

    // ============================================
    // endereco
    // ============================================

    $scope.addEnd = false;

    $scope.removeEnd = function (index, lista) {
        if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
        else {
            $scope.modalEnd = true;
            $scope.delEnd = $scope.viewModel.enderecos[index];
        }
    }

    $scope.closeModalEnd = function () {
        $scope.modalEnd = false;
    }

    $scope.removerEndModal = function () {
        $scope.viewModel.updateCommand = "removeEnd";
        $scope.viewModel.anexedEntity = $scope.delEnd;

        Api.Person.update({ id: id }, $scope.viewModel, function (data) {
            loadEntity();
        });
    }

    $scope.addNewEnd = function () {
        if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
        else
            $scope.addEnd = !$scope.addEnd;
    }

    $scope.newEnd = {};

    $scope.editEnd = function (mdl) {
        $scope.addEnd = true;
        $scope.newEnd = mdl;
    }

    $scope.cancelEnd = function () {
        $scope.addEnd = false;
        $scope.newEnd = {};
    }

    $scope.saveNewEnd = function () {
        if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
        else
        {
            $scope.stRua_fail = invalidCheck($scope.newEnd.stRua);
            $scope.est_fail = $scope.newEnd.fkEstado == undefined;
            $scope.cid_fail = $scope.newEnd.fkCidade == undefined;

            if (!$scope.stRua_fail && !$scope.est_fail && !$scope.cid_fail)
            {
                $scope.addEnd = false;

                $scope.viewModel.updateCommand = "newEnd";
                $scope.viewModel.anexedEntity = $scope.newEnd;

                Api.Person.update({ id: id }, $scope.viewModel, function (data) {
                    $scope.newEnd = {};
                    loadEntity();
                },
                function (response) {
                    toastr.error(response.data.message, 'Error');
                });
            }
        }
    }

}]);
