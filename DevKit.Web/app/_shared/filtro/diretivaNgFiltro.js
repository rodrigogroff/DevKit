'use strict';

angular.module('app.services').factory('ngHistoricoFiltro', ['$rootScope', function ($rootScope) {

	var service = {

		filtro: {
			campos: {},
			paginaAtual: 0,
			exibeFiltro: false,
			itensporpaginastored: 15
		},

		salvarFiltro: function () {
			sessionStorage.ngfiltro = angular.toJson(service.filtro);
		},

		recuperarFiltro: function () {
			service.filtro = angular.fromJson(sessionStorage.ngfiltro);
		},

		paginaAtual: function (paginaAtual) {
			service.filtro.paginaAtual = paginaAtual;
			sessionStorage.ngfiltro = angular.toJson(service.filtro);
		},

		itensporpaginastored: function (itensporpaginastored) {
			service.filtro.itensporpaginastored = itensporpaginastored;
			sessionStorage.ngfiltro = angular.toJson(service.filtro);
		}

	}

	$rootScope.$on("salvarFiltro", service.salvarFiltro);
	$rootScope.$on("recuperarFiltro", service.recuperarFiltro);

	return service;

}]);

angular.module('app.directives').directive('ngFiltro', function () {

	return {

		replace: true,
		restrict: 'AE',

		scope: {
			tipo: '@',
			campos: '=',
			placeholder: '@',
			templateAvancado: '@',
			funcaoPesquisar: '=',
			funcaoLimpar: '='
		},

		controller: ['$scope', '$rootScope', '$timeout', 'ngHistoricoFiltro', function ($scope, $rootScope, $timeout, ngHistoricoFiltro) {

			$scope.filtro = ngHistoricoFiltro.filtro;

			$scope.ferramentaPesquisa = $scope.ferramentaPesquisa || true;
			$scope.filtro.filtroGerado = ngHistoricoFiltro.filtro.filtroGerado;
			
			if ($scope.tipo != $scope.filtro.tipo)
			{
				$scope.filtro.tipo = $scope.tipo;
				$scope.filtro.campos = $scope.campos;
			
				ngHistoricoFiltro.filtro.paginaAtual = 0;
				ngHistoricoFiltro.filtro.itensporpaginastored = 15;

				$scope.filtro.exibeFiltro = false;
				$scope.filtro.filtroBasico = '';
			}

			$scope.toggleFiltros = function () {
				$scope.filtro.exibeFiltro = !$scope.filtro.exibeFiltro;
			};

			$scope.constroiFiltro = function (opcoes) {
				if (opcoes.filtroBasico) {
					var fBasico = {
						busca: opcoes ? opcoes.valor : ''
					};
					return fBasico;
				}

				return $scope.filtro.campos;
			};

			$scope.$watch("filtro.campos", function (novo, anterior) {

				var opcoes = { filtroBasico: !$scope.filtro.exibeFiltro };

				$scope.filtro.filtroGerado = $scope.constroiFiltro(opcoes);

			}, true);

			$scope.alterarFiltroBasico = function () {
				var opcoes = { filtroBasico: !$scope.filtro.exibeFiltro, valor: $scope.filtro.filtroBasico };
				$scope.filtro.filtroGerado = $scope.constroiFiltro(opcoes);
			};

			var focarElemento = function (elemento) {
				if (elemento) {
					$timeout(function () {
						elemento.focus();
					}, 100);
				}
			}

			$scope.$watch("filtro.exibeFiltro", function (novo, anterior) {
				var elemento = null;
				if ($scope.filtro.exibeFiltro)
					elemento = angular.element('.form-control').first();
				else
					elemento = angular.element(document.querySelector('#filtroBasico'));
				focarElemento(elemento);

				var opcoes = { filtroBasico: !$scope.filtro.exibeFiltro, valor: $scope.filtro.filtroBasico };
				$scope.filtro.filtroGerado = $scope.constroiFiltro(opcoes);

			}, true);

			$scope.$watch("filtro.filtroGerado", function (novo, anterior) {
				$scope.filtroGerado = $scope.filtro.filtroGerado;
			});

			$scope.pesquisar = function () {

				$rootScope.$broadcast('salvarFiltro');

				$scope.funcaoPesquisar();

				if ($scope.filtro.exibeFiltro) {
					var elemento = angular.element(document.querySelector('#filtroBasico'));
					focarElemento(elemento);
				}

			}

			$scope.limpar = function () {
				
				if ($scope.funcaoLimpar)
				{
					$scope.funcaoLimpar($scope.filtro.campos);
				}
				else
				{
					var selects = null;

					if ($scope.filtro && $scope.filtro.campos && $scope.filtro.campos.selects)
						selects = $scope.filtro.campos.selects;

					$scope.filtro.campos = { }

					if (selects) 
						$scope.filtro.campos.selects = selects;
				}

			}

		}],
		templateUrl: 'app/_shared/filtro/templateNgFiltro.html'
	};
});
