'use strict';

angular.module('app.directives').directive('ngPaginacao', function () {

	return {
		restrict: 'AE',
		replace: true,

		scope: {
			carregar: '=',
			list: '=',
			total: '=',
			itensporpagina: '=',
			paginador: '=',
			ehModalParam: '@?',
			carregarAutomaticamente: '@?'
		},

		templateUrl: 'app/_shared/paginacao/templateNgPaginacao.html',

		controller: ['$scope', 'ngHistoricoFiltro', function ($scope, ngHistoricoFiltro) {

			$scope.paginador = $scope.paginador || {};
			$scope.paginaAtual = ngHistoricoFiltro.filtro.paginaAtual || 0;
			$scope.itensporpagina = ngHistoricoFiltro.filtro.itensporpaginastored || $scope.itensporpagina;

			$scope.range = function () {
				var ret = [];
				var inicio, fim;
				var rangeSize = 5;

				inicio = $scope.paginaAtual;

				if (inicio > $scope.pageCount() - rangeSize) {
					inicio = $scope.pageCount() - rangeSize;
				}

				if (inicio < 0)
					inicio = 0;

				for (var i = inicio; i < inicio + rangeSize; i++) {
					ret.push(i);
				}

				return ret;
			};

			$scope.firstPage = function () {
				$scope.paginaAtual = 0;
			}

			$scope.lastPage = function () {
				$scope.paginaAtual = $scope.pageCount() - 1;
			}

			$scope.prevPage = function () {
				if ($scope.paginaAtual > 0) {
					$scope.paginaAtual--;
				}
			};

			$scope.prevPageDisabled = function () {
				return $scope.paginaAtual === 0 ? "" : "";
			};

			$scope.nextPage = function () {
				if ($scope.paginaAtual < $scope.pageCount() - 1) {
					$scope.paginaAtual++;
				}
			};

			$scope.nextPageDisabled = function () {
				return $scope.paginaAtual === $scope.pageCount() - 1 ? "" : "";
			};

			$scope.isCurrentPage = function (p) {
				return $scope.paginaAtual === p ? "btn btn-xs btn-primary" : "btn btn-xs btn-default";
			};

			$scope.pageCount = function () {
				return Math.ceil($scope.total / $scope.itensporpagina);
			};

			$scope.setPage = function (n) {
				if (n >= 0 && n < $scope.pageCount()) {
					$scope.paginaAtual = n;
				}
			};

			$scope.$watch("itensporpagina", function (valorNovo, valorAntigo) {
				if (valorNovo != valorAntigo) {
					$scope.paginaAtual = parseInt($scope.paginaAtual * (valorAntigo / valorNovo));
					$scope.carregar($scope.paginaAtual * valorNovo, valorNovo);
					ngHistoricoFiltro.filtro.itensporpaginastored = valorNovo;
				} else {
					if ($scope.ehModalParam) {
						$scope.itensporpagina = 15;
						ngHistoricoFiltro.filtro.itensporpaginastored = valorNovo;
					}
				}
			});

			$scope.$watch("paginaAtual", function (valorNovo, valorAntigo) {
				$scope.carregarAutomaticamente = $scope.carregarAutomaticamente || true;
				if (valorNovo != valorAntigo || $scope.carregarAutomaticamente == true) {
					ngHistoricoFiltro.filtro.paginaAtual = valorNovo;
					$scope.carregar(valorNovo * $scope.itensporpagina, $scope.itensporpagina);
				}
			});

			$scope.$watch("list", function (valorNovo, valorAntigo) {
				$scope.itemInicial = $scope.paginaAtual * $scope.itensporpagina + 1;
				$scope.itemFinal = Math.min(($scope.paginaAtual + 1) * $scope.itensporpagina, $scope.total);
			});

			$scope.paginador.reiniciar = function () {
				$scope.paginaAtual = 0;
				ngHistoricoFiltro.filtro.paginaAtual = 0;
			}

			$scope.paginador.alterarPagina = $scope.setPage;
		}]
	};
});