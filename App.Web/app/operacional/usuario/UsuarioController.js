'use strict';

angular.module('app.controllers').controller('UsuarioController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$scope.permModel = {};
	$scope.loading = false;

	function ObterPermissoes() { Api.Permissao.obter({ id: 102 }, function (data) { $scope.permModel = data; }, function (response) { }); }

	$scope.viewModel = {};

	var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

	init();

	function init()
	{
		$scope.loading = true;

		ObterPermissoes();

		$scope.selectPerfis = ngSelects.obterConfiguracao(Api.Perfil, { tamanhoPagina: 15, campoNome: 'stNome' });

		if (id > 0) {
			Api.Usuario.obter({ id: id }, function (data) {
				$scope.viewModel = data;
				$scope.loading = false;
			}, function (response) {
				if (response.status === 404) { toastr.error('Cadastro inexistente', 'Erro'); }
				$scope.listar();
			});
		}
		else {
			$scope.viewModel = { bAtivo: true };
		}
	}

	function showSuccessAndRedirect() {
		toastr.success('Usuário salvo', 'Sucesso');
	}

	function showError(errorMessage) {
		toastr.error(errorMessage, 'Validação');
	}

	$scope.salvar = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.editar)
			toastr.error('Você não tem permissão de salvar o registro', 'Permissão');
		else
		if ($scope.formEntidade.$valid) {
			if (id > 0) {
				Api.Usuario.atualizar({ id: id }, $scope.viewModel, function (data) {
					showSuccessAndRedirect();
				}, function (response) {
					showError(response.data.message);
				});
			}
			else 
				Api.Usuario.adicionar($scope.viewModel, function (data) {
					showSuccessAndRedirect();
				}, function (response) {
					showError(response.data.message);
				});
		}
		else 
			toastr.error('Campos Inválidos!', 'Validação');
	};

	$scope.listar = function () {
		$scope.entidadeAlterada = false;
		$state.go('usuarios');
	}

	// email --------------------------------------------------------------------------------------

	$scope.addEmail = false;
	$scope.novoEmail = { StEmail: '' };

	$scope.removerEmail = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.editar)
			toastr.error('Você não tem permissão de salvar o registro', 'Permissão');
		else
		{
			$scope.viewModel.emails.splice(index, 1);
			Api.Usuario.atualizar({ id: id }, $scope.viewModel, function (data) {
				toastr.success('Lista de emails salva', 'Sucesso');
			});
		}		
	}
	
	$scope.adicionarEmail = function () {
		if (!$scope.permModel.novo && !$scope.permModel.editar)
			toastr.error('Você não tem permissão de salvar o registro', 'Permissão');
		else
			$scope.addEmail = !$scope.addEmail;
	}

	$scope.salvarNovoEmail = function ()
	{
		$scope.addEmail = false;
		$scope.novoEmail.FkUsuario = id;
		$scope.viewModel.emails.push($scope.novoEmail);

		Api.Usuario.atualizar({ id: id }, $scope.viewModel, function (data) {
			toastr.success('Lista de emails salva', 'Sucesso');
		});

		$scope.novoEmail = { StEmail: '' };
	}

	// telefone --------------------------------------------------------------------------------------

	$scope.addTelefone = false;
	$scope.novoTelefone = { StTelefone: '', StLocal: '' };

	$scope.removerTelefone = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.editar)
			toastr.error('Você não tem permissão de salvar o registro', 'Permissão');
		else {
			$scope.viewModel.telefones.splice(index, 1);
			Api.Usuario.atualizar({ id: id }, $scope.viewModel, function (data) {
				toastr.success('Lista de telefones salva', 'Sucesso');
			});
		}
	}

	$scope.adicionarTelefone = function () {
		if (!$scope.permModel.novo && !$scope.permModel.editar)
			toastr.error('Você não tem permissão de salvar o registro', 'Permissão');
		else
			$scope.addTelefone = !$scope.addTelefone;
	}

	$scope.salvarNovoTelefone = function ()
	{
		$scope.addTelefone = false;
		$scope.novoTelefone.FkUsuario = id;
		$scope.viewModel.telefones.push($scope.novoTelefone);

		Api.Usuario.atualizar({ id: id }, $scope.viewModel, function (data) {
			toastr.success('Lista de telefones salva', 'Sucesso');
		});

		$scope.novoTelefone = { StTelefone: '', StLocal: '' };
		//	toastr.error('Falha', 'Campos');
	}

}]);
