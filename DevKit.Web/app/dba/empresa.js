
angular.module('app.controllers').controller('EmpresaController',
['$scope', '$state', '$stateParams', '$rootScope', 'Api', 'ngSelects', 
function ($scope, $state, $stateParams, $rootScope, Api, ngSelects)
{
	$rootScope.exibirMenu = true;
	$scope.loading = false;
	
	var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;
    
	init();

    function init()
    {
        $scope.viewModel = {};
        $scope.permModel = {};

        $scope.selectDayMonths = ngSelects.obterConfiguracao(Api.DayMonthCombo, {});
        $scope.selectMonths = ngSelects.obterConfiguracao(Api.MonthCombo, {});
        $scope.estado = ngSelects.obterConfiguracao(Api.EstadoCombo, {});
        $scope.cidade = ngSelects.obterConfiguracao(Api.CidadeCombo, { scope: $scope, filtro: { campo: 'fkEstado', valor: 'newEnd.fkEstado' } });

        loadEntity();
    }

    function loadEntity()
    {
		if (id > 0)
        {
            if ($scope.loaded == undefined)
                $scope.loading = true;

            Api.Empresa.get({ id: id }, function (data)
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
		//if (!$scope.permModel.novo && !$scope.permModel.edicao)
          //  toastr.error('Acesso negado!', 'Permissão');
		//else
		{
            $scope.stNome_fail = invalidCheck($scope.viewModel.stNome);
            $scope.stSigla_fail = invalidCheck($scope.viewModel.stSigla);
            $scope.cnpj_fail = invalidCheck($scope.viewModel.stCnpj);
            $scope.nuEmpresa_fail = invalidCheck($scope.viewModel.nuEmpresa);

            $scope.nuFech_fail = invalidCheck($scope.viewModel.nuDiaFech);
            $scope.nuMaxC_fail = invalidCheck($scope.viewModel.nuMaxConsultas);
            $scope.nuCar_fail = invalidCheck($scope.viewModel.nuCarenciaMeses);
            $scope.vrMax_fail = invalidCheck($scope.viewModel.vrMaxProcSemAut);
	
            if (!$scope.stName_fail &&
                !$scope.stSigla_fail &&
                !$scope.cnpj_fail &&
                !$scope.nuEmpresa_fail && 
                !$scope.nuFech_fail && 
                !$scope.nuMaxC_fail && 
                !$scope.nuCar_fail && 
                !$scope.vrMax_fail )
            {
				if (id > 0)
                {
					$scope.viewModel.updateCommand = "entity";

                    Api.Empresa.update({ id: id }, $scope.viewModel, function (data)
					{
                        toastr.success('Dados atualizados!', 'Sucesso');
					},
					function (response) {
						toastr.error(response.data.message, 'Error');
					});
				}
				else
				{
                    Api.Empresa.add($scope.viewModel, function (data)
					{
                        toastr.success('Credenciado adicionado!', 'Sucesso');                       
                        $state.go('credenciado', { id: data.id });
					},
					function (response) {
						toastr.error(response.data.message, 'Error');
					});
				}
			}
		}
	};

    $scope.list = function ()
    {
		$state.go('empresas');
    }

    // ============================================
    // consultas 
    // ============================================

    $scope.addConsulta = false;

    $scope.removeConsulta = function (index, lista) {
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
        //  toastr.error('Acesso negado!', 'Permissão');
        //else 
        {
            $scope.modalConsulta = true;
            $scope.delConsulta = $scope.viewModel.consultas[index];
        }
    }

    $scope.closeModalConsulta = function () {
        $scope.modalConsulta = false;
    }

    $scope.removerConsultaModal = function () {
        $scope.viewModel.updateCommand = "removeConsulta";
        $scope.viewModel.anexedEntity = $scope.delConsulta;

        Api.Empresa.update({ id: id }, $scope.viewModel, function (data) {
            loadEntity();
        });
    }

    $scope.addNewConsulta = function () {
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
        //  toastr.error('Acesso negado!', 'Permissão');
        //else
        $scope.InitNewConsulta();
        $scope.addConsulta = !$scope.addConsulta;
    }

    $scope.InitNewConsulta = function () {
        $scope.newConsulta = { svrPreco1: '0,00', svrPreco2: '0,00', svrPreco3: '0,00', svrPreco4: '0,00', svrPreco5: '0,00', svrPreco6: '0,00', svrPreco7: '0,00', svrPreco8: '0,00', svrPreco9: '0,00' };
    }
    
    $scope.editConsulta = function (mdl) {
        $scope.addConsulta = true;
        $scope.newConsulta = mdl;
    }

    $scope.cancelConsulta = function () {
        $scope.addConsulta = false;
        $scope.newConsulta = {};
    }

    $scope.saveNewConsulta = function () {
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
        //  toastr.error('Acesso negado!', 'Permissão');
        //else
        {
            $scope.consulta_nuAno_fail = invalidCheck($scope.newConsulta.nuAno);

            if (!$scope.consulta_nuAno_fail)
            {
                $scope.addConsulta = false;

                $scope.viewModel.updateCommand = "newConsulta";
                $scope.viewModel.anexedEntity = $scope.newConsulta;

                Api.Empresa.update({ id: id }, $scope.viewModel, function (data) {
                    $scope.InitNewConsulta();
                    loadEntity();
                },
                    function (response) {
                        toastr.error(response.data.message, 'Error');
                    });
            }
        }
    }

    // ============================================
    // secoes 
    // ============================================

    $scope.addSecao = false;

    $scope.removeSecao = function (index, lista) {
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
        //  toastr.error('Acesso negado!', 'Permissão');
        //else 
        {
            $scope.modalSecao = true;
            $scope.delSecao = $scope.viewModel.secoes[index];
        }
    }

    $scope.closeModalSecao = function () {
        $scope.modalSecao = false;
    }

    $scope.removerSecaoModal = function () {
        $scope.viewModel.updateCommand = "removeSecao";
        $scope.viewModel.anexedEntity = $scope.delSecao;

        Api.Empresa.update({ id: id }, $scope.viewModel, function (data) {
            loadEntity();
        });
    }

    $scope.addNewSecao = function () {
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
        //  toastr.error('Acesso negado!', 'Permissão');
        //else
        $scope.addSecao = !$scope.addSecao;
    }

    $scope.newSecao = {};

    $scope.editSecao = function (mdl) {
        $scope.addSecao = true;
        $scope.newSecao = mdl;
    }

    $scope.cancelSecao = function () {
        $scope.addSecao = false;
        $scope.newSecao = {};
    }

    $scope.saveNewSecao = function () {
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
        //  toastr.error('Acesso negado!', 'Permissão');
        //else
        {
            

            {
                $scope.addSecao = false;

                $scope.viewModel.updateCommand = "newSecao";
                $scope.viewModel.anexedEntity = $scope.newSecao;

                Api.Empresa.update({ id: id }, $scope.viewModel, function (data) {
                    $scope.newSecao = {};
                    loadEntity();
                },
                function (response) {
                    toastr.error(response.data.message, 'Error');
                });
            }
        }
    }
        
	// ============================================
	// phone 
	// ============================================

	$scope.addPhone = false;
		
    $scope.removePhone = function (index, lista)
    {
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
          //  toastr.error('Acesso negado!', 'Permissão');
        //else
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

        Api.Empresa.update({ id: id }, $scope.viewModel, function (data) {
            loadEntity();
		});
	}

	$scope.addNewPhone = function ()
	{
		//if (!$scope.permModel.novo && !$scope.permModel.edicao)
          //  toastr.error('Acesso negado!', 'Permissão');
		//else
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
		//if (!$scope.permModel.novo && !$scope.permModel.edicao)
          //  toastr.error('Acesso negado!', 'Permissão');
		//else
		{
			$scope.stPhone_fail = invalidCheck($scope.newPhone.stTelefone);
			$scope.stDescription_fail = invalidCheck($scope.newPhone.stDesc);
	
			if (!$scope.stPhone_fail &&
				!$scope.stDescription_fail)
			{
				$scope.addPhone = false;
                
				$scope.viewModel.updateCommand = "newPhone";
				$scope.viewModel.anexedEntity = $scope.newPhone;

                Api.Empresa.update({ id: id }, $scope.viewModel, function (data)
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
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
          //  toastr.error('Acesso negado!', 'Permissão');
        //else 
        {
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

        Api.Empresa.update({ id: id }, $scope.viewModel, function (data) {
            loadEntity();
        });
    }

	$scope.addNewEmail = function ()
	{
		//if (!$scope.permModel.novo && !$scope.permModel.edicao)
          //  toastr.error('Acesso negado!', 'Permissão');
		//else
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
		//if (!$scope.permModel.novo && !$scope.permModel.edicao)
          //  toastr.error('Acesso negado!', 'Permissão');
		//else
		{
			$scope.stEmail_fail = invalidEmail($scope.newEmail.stEmail) ;
            $scope.stEmailContato_fail = invalidCheck($scope.newEmail.stContato);

			if (!$scope.stEmail_fail)
			{
				$scope.addEmail = false;
                
				$scope.viewModel.updateCommand = "newEmail";
				$scope.viewModel.anexedEntity = $scope.newEmail;

                Api.Empresa.update({ id: id }, $scope.viewModel, function (data)
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
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
          //  toastr.error('Acesso negado!', 'Permissão');
        //else 
        {
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

        Api.Empresa.update({ id: id }, $scope.viewModel, function (data) {
            loadEntity();
        });
    }

    $scope.addNewEnd = function () {
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
        //    toastr.error('Acesso negado!', 'Permissão');
        //else
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
        //if (!$scope.permModel.novo && !$scope.permModel.edicao)
          //  toastr.error('Acesso negado!', 'Permissão');
        //else
        {
            $scope.stRua_fail = invalidCheck($scope.newEnd.stRua);
            $scope.est_fail = $scope.newEnd.fkEstado == undefined;
            $scope.cid_fail = $scope.newEnd.fkCidade == undefined;

            if (!$scope.stRua_fail && !$scope.est_fail && !$scope.cid_fail)
            {
                $scope.addEnd = false;

                $scope.viewModel.updateCommand = "newEnd";
                $scope.viewModel.anexedEntity = $scope.newEnd;

                Api.Empresa.update({ id: id }, $scope.viewModel, function (data) {
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
