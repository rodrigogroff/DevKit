
angular.module('app.controllers').controller('ProjectController',
['$scope', '$state', '$stateParams', '$rootScope', 'Api', 'ngSelects',
function ($scope, $state, $stateParams, $rootScope, Api, ngSelects)
{
	$rootScope.exibirMenu = true;

	$scope.selectUsers = ngSelects.obterConfiguracao(Api.UserCombo, { });
	$scope.selectProjectTemplate = ngSelects.obterConfiguracao(Api.ProjectTemplate, {});
	$scope.selectPhases = ngSelects.obterConfiguracao(Api.PhaseCombo, { scope: $scope, filtro: { campo: 'fkProject', valor: 'viewModel.id' } });

	$scope.loading = false;

	$scope.viewModel = {};
	$scope.permModel = {};	
	$scope.permID = 103;
	$scope.auditLogPerm = 111;

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
	
	var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

	init();

    function init() {
        CheckPermissions();
        loadEntity();
    }

    function loadEntity()
    {
		if (id > 0)
        {
            if ($scope.loaded == undefined)
			    $scope.loading = true;

            Api.Project.get({ id: id }, function (data)
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
			$scope.viewModel = { };
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
			$scope.stName_fail = invalidCheck($scope.viewModel.stName);
			$scope.fkTemplate_fail = $scope.viewModel.fkProjectTemplate == undefined;
	
			if (!$scope.stName_fail && 
				!$scope.fkTemplate_fail )
            {
				if (id > 0)
				{
					$scope.viewModel.updateCommand = "entity";

					Api.Project.update({ id: id }, $scope.viewModel, function (data)
					{
                        toastr.success('Projeto salvo!', 'Sucesso');
                        init();
					},
					function (response)
					{
						toastr.error(response.data.message, 'Error');
					});
				}
				else
				{
					Api.Project.add($scope.viewModel, function (data)
					{
                        toastr.success('Projeto adicionado!', 'Sucesso');
                        $state.go('projects');
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
		$state.go('projects');
	}

	$scope.remove = function ()
	{
		if (!$scope.permModel.remover)
            toastr.error('Acesso negado!', 'Permissão');
		else
		{
            Api.Project.remove({ id: id }, function (data)
			{
                toastr.success('Projeto removido!', 'Sucesso');
				$scope.list();
			},
			function (response) {
				toastr.error(response.data.message, 'Permission');
			});
		}
	}
		
	// ---------------------------------
	// users
	// ---------------------------------

	$scope.addUser = false;

    $scope.removeUser = function (index, lista) {
        if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
        else {
            $scope.modalUser = true;
            $scope.delUser = $scope.viewModel.users[index];
        }
    }

    $scope.closeModalUser = function () {
        $scope.modalUser = false;
    }

    $scope.removerUserModal = function () {
        $scope.viewModel.updateCommand = "removeUser";
        $scope.viewModel.anexedEntity = $scope.delUser;

        Api.Project.update({ id: id }, $scope.viewModel, function (data) {
            loadEntity();
        });
    }

	$scope.addNewUser = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
			$scope.addUser = !$scope.addUser;
	}

	$scope.editUser = function (mdl)
	{
		$scope.addUser = true;
		$scope.newUser = mdl;
	}

	$scope.cancelUser = function (mdl) {
		$scope.addUser = false;
		$scope.newUser = {};
	}

	$scope.newUser = { };

	$scope.saveNewUser = function ()
	{
		$scope.fkUser_fail = $scope.newUser.fkUser == undefined;
		$scope.stRole_fail = invalidCheck($scope.newUser.stRole);
	
		if (!$scope.fkUser_fail && !$scope.stRole_fail)
        {
			$scope.viewModel.updateCommand = "newUser";
			$scope.viewModel.anexedEntity = $scope.newUser;

			Api.Project.update({ id: id }, $scope.viewModel, function (data)
			{
				$scope.newUser = {};
                $scope.addUser = false;

                toastr.success('Usuário adicionado', 'Sucesso');
                loadEntity();

			}, function (response)
			{
				toastr.error(response.data.message, 'Error');
			});
		}
	}

	// ---------------------------------
	// phases
	// ---------------------------------

	$scope.addPhase = false;

    $scope.removePhase = function (index, lista) {
        if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
        else {
            $scope.modalPhase = true;
            $scope.delPhase = $scope.viewModel.phases[index];
        }
    }

    $scope.closeModalPhase = function () {
        $scope.modalPhase = false;
    }

    $scope.removerPhaseModal = function () {
        $scope.viewModel.updateCommand = "removePhase";
        $scope.viewModel.anexedEntity = $scope.delPhase;

        Api.Project.update({ id: id }, $scope.viewModel, function (data) {
            loadEntity();
        });
    }

	$scope.addNewPhase = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
			$scope.addPhase = !$scope.addPhase;
	}

	$scope.cancelPhase = function (mdl) {
		$scope.addPhase = false;
		$scope.newPhase = {};
	}

	$scope.editPhase = function (mdl)
	{
		$scope.addPhase = true;
		$scope.newPhase = mdl;
	}

	$scope.newPhase = { bComplete: false };

	$scope.saveNewPhase = function ()
	{		
		$scope.newphase_stName_fail = invalidCheck($scope.newPhase.stName);

		if (!$scope.newphase_stName_fail)
        {            
            $scope.viewModel.updateCommand = "newPhase";
			$scope.viewModel.anexedEntity = $scope.newPhase;

			Api.Project.update({ id: id }, $scope.viewModel, function (data)
			{
				$scope.newPhase = { bComplete: false };

				toastr.success('Fase adicionada', 'Sucesso');
                loadEntity();

            }, function (response)
            {
				toastr.error(response.data.message, 'Error');
			});
		}
	}

	// ---------------------------------
	// sprints
	// ---------------------------------

	$scope.addSprint = false;

    $scope.removeSprint = function (index, lista) {
        if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
        else {
            $scope.modalSprint = true;
            $scope.delSprint = $scope.viewModel.sprints[index];
        }
    }

    $scope.closeModalSprint = function () {
        $scope.modalSprint = false;
    }

    $scope.removerSprintModal = function () {
        $scope.viewModel.updateCommand = "removeSprint";
        $scope.viewModel.anexedEntity = $scope.delSprint;

        Api.Project.update({ id: id }, $scope.viewModel, function (data) {
            loadEntity();
        });
    }

	$scope.addNewSprint = function () {
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
            toastr.error('Acesso negado!', 'Permissão');
		else
			$scope.addSprint = !$scope.addSprint;
	}

	$scope.cancelSprint = function () {
		$scope.addSprint = false;
		$scope.newSprint = {};
	}

	$scope.editSprint = function (mdl) {
		$scope.addSprint = true;
		$scope.newSprint = mdl;
	}

	$scope.goSprint = function (mdl)
	{
		$state.go('sprint', { id: mdl.id });
	}

	$scope.newSprint = { };

	$scope.saveNewSprint = function ()
	{
		$scope.fkSprintPhase_fail = $scope.newSprint.fkPhase == undefined;
		$scope.stSprint_fail = invalidCheck($scope.newSprint.stName);

		if (!$scope.fkSprintPhase_fail &&
			!$scope.stSprint_fail)
        {
			$scope.viewModel.updateCommand = "newSprint";
			$scope.viewModel.anexedEntity = $scope.newSprint;

			Api.Project.update({ id: id }, $scope.viewModel, function (data)
			{				
				$scope.newSprint = {};
                $scope.addSprint = false;

				toastr.success('Sprint salvo', 'Sucesso');

                loadEntity();

			}, function (response) {
				toastr.error(response.data.message, 'Error');
			});
		}
	}

}]);
