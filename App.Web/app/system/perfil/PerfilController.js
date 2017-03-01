'use strict';

angular.module('app.controllers').controller('PerfilController',
['$scope', 'AuthService', '$state', '$stateParams', '$location', '$rootScope', 'Api', 'ngSelects',
function ($scope, AuthService, $state, $stateParams, $location, $rootScope, Api, ngSelects)
{
	$scope.permModel = {};
	$scope.loading = false;

	function ObterPermissoes() { Api.Permissao.obter({ id: 101 }, function (data) { $scope.permModel = data; }, function (response) { }); }

	$scope.viewModel =
		{
			// ## Sistema #########################
			// perfis
			tg1011: false, tg1012: false, tg1013: false, tg1014: false, tg1015: false,
			// usuarios
			tg1021: false, tg1022: false, tg1023: false, tg1024: false, tg1025: false,
		};
	
	var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;
	
	init();

	function init()
	{
		console.log('oi');
		ObterPermissoes();

		if (id > 0) {
			$scope.loading = true;
			Api.Perfil.obter({ id: id }, function (data)
			{
				// perfis
				if (data.stPermissoes.indexOf('|1011|') >= 0) data.tg1011 = true; else data.tg1011 = false;
				if (data.stPermissoes.indexOf('|1012|') >= 0) data.tg1012 = true; else data.tg1012 = false;
				if (data.stPermissoes.indexOf('|1013|') >= 0) data.tg1013 = true; else data.tg1013 = false;
				if (data.stPermissoes.indexOf('|1014|') >= 0) data.tg1014 = true; else data.tg1014 = false;
				if (data.stPermissoes.indexOf('|1015|') >= 0) data.tg1015 = true; else data.tg1015 = false;

				// usuarios
				if (data.stPermissoes.indexOf('|1021|') >= 0) data.tg1021 = true; else data.tg1021 = false;
				if (data.stPermissoes.indexOf('|1022|') >= 0) data.tg1022 = true; else data.tg1022 = false;
				if (data.stPermissoes.indexOf('|1023|') >= 0) data.tg1023 = true; else data.tg1023 = false;
				if (data.stPermissoes.indexOf('|1024|') >= 0) data.tg1024 = true; else data.tg1024 = false;
				if (data.stPermissoes.indexOf('|1025|') >= 0) data.tg1025 = true; else data.tg1025 = false;
				
				$scope.viewModel = data;

				$scope.loading = false;

			}, function (response) {
				if (response.status === 404) { toastr.error('Perfil inexistente', 'Erro'); }
				$scope.listar();
			});
		}
	}

	function showError(errorMessage) {
		toastr.error(errorMessage, 'Validação');
	}

	$scope.salvar = function ()
	{
		if (!$scope.permModel.novo && !$scope.permModel.edicao)
			toastr.error('Você não tem permissão de salvar o registro', 'Permissão');
		else
		if ($scope.formEntidade.$valid)
		{
			var perms = ''; var _mdl = $scope.viewModel;

			// perfis
			if (_mdl.tg1011 == true) perms += '|1011|'; if (_mdl.tg1012 == true) perms += '|1012|'; if (_mdl.tg1013 == true) perms += '|1013|';
			if (_mdl.tg1014 == true) perms += '|1014|'; if (_mdl.tg1015 == true) perms += '|1015|';

			// usuarios
			if (_mdl.tg1021 == true) perms += '|1021|'; if (_mdl.tg1022 == true) perms += '|1022|'; if (_mdl.tg1023 == true) perms += '|1023|';
			if (_mdl.tg1024 == true) perms += '|1024|'; if (_mdl.tg1025 == true) perms += '|1025|';
			
			$scope.viewModel.stPermissoes = perms;

			if (id > 0)
			{
				Api.Perfil.atualizar({ id: id }, $scope.viewModel, function (data) {
					toastr.success('Perfil salvo', 'Sucesso');
				}, function (response) {
					showError(response.data.message);
				});
			}
			else 
				Api.Perfil.adicionar($scope.viewModel, function (data) {
					toastr.success('Perfil salvo', 'Sucesso');
					$state.go('perfil', { id: data.id });
				}, function (response) {
					showError(response.data.message);
				});
		}
		else 
			toastr.error('Campos Inválidos!', 'Validação');
	};

	$scope.listar = function () {
		$state.go('perfils');
	}

	$scope.excluir = function () {
		if (!$scope.permModel.remover)
			toastr.error('Você não tem permissão de remover o registro', 'Permissão');
		else
			Api.Perfil.remover({ id: id }, $scope.viewModel, function (data) {
				toastr.success('Perfil removido', 'Sucesso');
				$scope.listar();
			}, function (response) {
				showError(response.data.message);
			});
	}

	$scope.exibirUsuario = function (mdl) {
		$state.go('usuario', { id: mdl.id });
	}	

}]);
