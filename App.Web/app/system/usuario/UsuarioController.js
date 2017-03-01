'use strict';

angular.module('app.controllers').controller('UsuarioController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$scope.style_error = { 'background-color': 'goldenrod' }
	$scope.maskTelefone = '(99) 999999999';
	
	$scope.viewModel = {};
	$scope.permModel = {};
	
	$scope.loading = false;

	function ObterPermissoes() {
		Api.Permissao.obter({ id: 102 }, function (data) { $scope.permModel = data; }, function (response) { });
	}
	
	var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

	init();

	function init()
	{
		ObterPermissoes();

		$scope.selectPerfis = ngSelects.obterConfiguracao(Api.Perfil, { tamanhoPagina: 15, campoNome: 'stNome' });

		if (id > 0) {
			$scope.loading = true;
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
		$scope.listar();
	}

	function showError(errorMessage) {
		toastr.error(errorMessage, 'Validação');
	}

	$scope.salvar = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Você não tem permissão de salvar o registro', 'Permissão');
		else
		if ($scope.formBase.$valid) {
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
		$state.go('usuarios');
	}

	$scope.excluir = function ()
	{
		if (!$scope.permModel.remover)
			toastr.error('Você não tem permissão de remover o registro', 'Permissão');
		else
			Api.Usuario.remover({ id: id }, $scope.viewModel, function (data) {
				toastr.success('Usuário removido', 'Sucesso');
				$scope.listar();
			}, function (response) {
				showError(response.data.message);
			});		
	}

	// ============================================
	// telefone 
	// ============================================

	$scope.addTelefone = false;
	$scope.novoTelefone = {
		stTelefone: '',
		stLocal: ''
	};
	$scope.errorTel = false;
	$scope.errorTelMsg = '';
	
	$scope.removerTelefone = function (index, lista) {
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Você não tem permissão de salvar o registro', 'Permissão');
		else {
			Api.Usuario.atualizar({ id: id }, $scope.viewModel, function (data) {
				toastr.success('Lista de telefones salva', 'Sucesso');
				$scope.viewModel.telefones.splice(index, 1);
			});
		}
	}

	$scope.adicionarTelefone = function () {
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Você não tem permissão de salvar o registro', 'Permissão');
		else
			$scope.addTelefone = !$scope.addTelefone;
	}

	$scope.salvarNovoTelefone = function ()
	{
		var _stTelefone = $scope.novoTelefone.stTelefone.length != 11;
		var _stLocal = $scope.novoTelefone.stLocal.length < 3;
		
		if (_stTelefone || _stLocal)
		{
			if (_stTelefone) $scope.style_stTelefone = $scope.style_error; else $scope.style_stTelefone = {};
			if (_stLocal) $scope.style_stLocal = $scope.style_error; else $scope.style_stLocal = {};

			$scope.errorTel = true;
			$scope.errorTelMsg = 'Preencha os campos corretamente';
		}
		else
		{
			$scope.errorTel = false;
			
			$scope.addTelefone = false;
			$scope.novoTelefone.FkUsuario = id;
			$scope.viewModel.telefones.push($scope.novoTelefone);

			Api.Usuario.atualizar({ id: id }, $scope.viewModel, function (data)
			{				
				$scope.style_stTelefone = {}
				$scope.style_stLocal = {}
				$scope.novoTelefone = {
					stTelefone: '',
					stLocal: ''
				};
				toastr.success('Lista de telefones salva', 'Sucesso');

				$scope.viewModel.telefones = data.telefones;

			}, function (response) {
				showError(response.data.message);
			});
		}
	}

	// ============================================
	// email 
	// ============================================

	$scope.addEmail = false;
	$scope.novoEmail =
		{
			stEmail: ''
		};
	$scope.errorEmail = false;
	$scope.errorEmailMsg = '';
	$scope.style_stEmail = {};
	
	$scope.removerEmail = function (index, lista)
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
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
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Você não tem permissão de salvar o registro', 'Permissão');
		else
			$scope.addEmail = !$scope.addEmail;
	}

	$scope.salvarNovoEmail = function ()
	{
		var _stEmail = false;

		if ($scope.novoEmail.stEmail != undefined)
			_stEmail = !($scope.novoEmail.stEmail.indexOf('@') > 0);
		
		if (_stEmail)
		{
			if (_stEmail)
				$scope.style_stEmail = $scope.style_error; else $scope.style_stEmail = {};

			$scope.errorEmail = true;
			$scope.errorEmailMsg = 'Preencha os campos corretamente';
		}
		else
		{
			$scope.errorEmail = false;
			$scope.addEmail = false;

			$scope.novoEmail.FkUsuario = id;
			$scope.novoEmail.dtCriacao = new Date();
			$scope.viewModel.emails.push($scope.novoEmail);

			Api.Usuario.atualizar({ id: id }, $scope.viewModel, function (data)
			{
				toastr.success('Lista de emails salva', 'Sucesso');
				$scope.novoEmail =
					{
						stEmail: ''
					};
				
				$scope.viewModel.emails = data.emails;

			}, function (response) {
				showError(response.data.message);
			});
		}
	}

}]);
