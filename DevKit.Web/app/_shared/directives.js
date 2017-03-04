'use strict';

angular.module('app.directives', [])

.directive('appVersion', ['version', function (version) {
	return function (scope, elm, attrs) {
		elm.text(version);
	};
}])

.directive("contenteditable", function () {
	return {
		restrict: "A",
		require: "ngModel",
		link: ['scope', 'element', 'attrs', 'ngModel', function (scope, element, attrs, ngModel) {

			function read() {
				ngModel.$setViewValue(element.html());
			}

			ngModel.$render = function () {
				element.html(ngModel.$viewValue || "");
			};

			element.bind("blur keyup change", function () {
				scope.$apply(read);
			});
		}]
	};
})

.directive('checkboxAll', function () {
	return function (scope, iElement, iAttrs) {
		var parts = iAttrs.checkboxAll.split(':');
		iElement.attr('type', 'checkbox');
		iElement.bind('change', function (evt) {
			scope.$apply(function () {
				var setValue = iElement.prop('checked');
				angular.forEach(scope.$eval(parts[0]), function (v) {
					var ignorar = v['ignorarCheckAll'] || false;
					if (!ignorar) {
						v[parts[1]] = setValue;
					}
				});
			});
		});
		scope.$watch(parts[0], function (newVal) {
			var hasTrue, hasFalse;
			angular.forEach(newVal, function (v) {
				if (v[parts[1]]) {
					hasTrue = true;
				} else {
					hasFalse = true;
				}
			});
			if (hasTrue && hasFalse) {
				iElement.attr('checked', false);
				iElement.addClass('greyed');
			} else {
				iElement.attr('checked', hasTrue);
				iElement.removeClass('greyed');
			}
		}, true);
	};
})

.directive('contatos', function () {
	return {
		restrict: 'AE',

		scope: {
			contatos: '=dados',
			contexto: '@'
		},

		controller: ['$scope', 'AuthService', '$state', '$rootScope', '$location', '$confirmacao', 'Api', function ($scope, AuthService, $state, $rootScope, $location, $confirmacao, Api) {

			
			$scope.modo = "lista";

			$scope.editar = function (contato) {
				$scope.novoContato = contato || {};
				$scope.modo = "edicao";
			};

			$scope.buscar = function () {
				$scope.modo = "busca";
			};

			$scope.remover = function (contato) {
				$confirmacao.exibir('Confirmação', 'Você tem certeza que deseja remover este contato?').then(function (aceito) {
					if (aceito) {
						var index = $scope.contatos.indexOf(contato)
						$scope.contatos.splice(index, 1);
						return false;
					}
				});
			}

			$scope.cancelar = function () {
				$scope.modo = "lista";
			}

			function showError(errorMessage) {
				toastr.error(errorMessage, 'Validação');
			}

			$scope.salvar = function () {
				if ($scope.formContato.$valid) {

					if ($scope.contatos == undefined)
						$scope.contatos = [];

					if ($scope.novoContato.id > 0) {
						Api.Contato.atualizar({ id: $scope.novoContato.id }, $scope.novoContato, function (data) {
							$scope.modo = "lista";
						}, function (response) {
							showError(response.data.message);
						});
					}
					else {
						Api.Contato.adicionar($scope.novoContato, function (contatoSalvo) {
							$scope.contatos.push(contatoSalvo);
							$scope.modo = "lista";
						}, function (response) {
							showError(response.data.message);
						});
					}
				}
				else {
					toastr.error('Campos Inválidos! Verifique os campos destacados em vermelho em todas as abas.', 'Validação');
				}
			};

			$scope.itensporpagina = 15;
			$scope.filtros = { Valor: '' };
			$scope.paginador = {};

			$scope.carregar = function (skip, take) {

				var opcoes = {
					skip: skip,
					take: take,
					busca: $scope.filtros.Valor,
					contexto: $scope.contexto
				};

				Api.Contato.listarPaginado(opcoes, function (data) {
					$scope.lista = data.results;
					$scope.total = data.count;
				});

			}

			$scope.associarContato = function (contato) {

				var contatoJaAdicionado = false;

				for (var i = 0; i < $scope.contatos.length; i++) {
					if (contato.id == $scope.contatos[i].id) {
						contatoJaAdicionado = true;
						toastr.info('Este contato já foi adicionado anteriormente.');
						break;
					}
				}

				if (!contatoJaAdicionado) {
					$scope.contatos.push(contato)
				}

				$scope.modo = "lista";
			}

			$scope.pesquisar = function () {
				$scope.carregar(0, $scope.itensporpagina);
				$scope.paginador.reiniciar();
			}

		}],
		templateUrl: 'app/cadastros/contato/templateContato.html'
	};
})

.directive('endereco', function () {

	return {

		restrict: 'AE',

		scope: {
			endereco: '=dados',
			disabled: '=' || false,
			obrigatorio: '=' || false,
		},

		controller: ['$scope', 'AuthService', '$state', 'ngSelects', 'Api', function ($scope, AuthService, $state, ngSelects, Api) {

			

			$scope.selectPaises = ngSelects.obterConfiguracao(Api.Pais, { tamanhoPagina: 15 });
			$scope.selectEstados = ngSelects.obterConfiguracao(Api.Estado, { tamanhoPagina: 15, scope: $scope, filtro: { campo: 'idPais', valor: 'endereco.idPais' } });
			$scope.selectCidades = ngSelects.obterConfiguracao(Api.Cidade, { tamanhoPagina: 50, scope: $scope, filtro: { campo: 'idUf', valor: 'endereco.idEstado' } });
			$scope.selectBairros = ngSelects.obterConfiguracao(Api.Bairro, { tamanhoPagina: 50, scope: $scope, filtro: { campo: 'idCidade', valor: 'endereco.idCidade' } });

			$scope.selectTiposLogradouro = ngSelects.obterConfiguracao(Api.TipoLogradouro, { tamanhoPagina: 15 });

			$scope.$watch('endereco', function (valorNovo, valorAntigo) {

				if ($scope.endereco == null)
					return;

				if ($scope.endereco.cep == '')
					$scope.endereco.cep = null;
				else
					if ($scope.endereco.cep.length == 8)
						$scope.buscarCep();

				if ($scope.endereco.idPais == 0)
					$scope.endereco.idPais = null;

				if ($scope.endereco.idEstado == 0)
					$scope.endereco.idEstado = null;

				if ($scope.endereco.idCidade == 0)
					$scope.endereco.idCidade = null;

				if ($scope.endereco.idBairro == 0)
					$scope.endereco.idBairro = null;

				if ($scope.endereco.idTipoLogradouro == 0)
					$scope.endereco.idTipoLogradouro = null;

				if (!$scope.endereco.logradouro)
					$scope.endereco.logradouro = '';

				var vazio = true;

				for (var key in $scope.endereco) {
					if ($scope.endereco[key]) {
						vazio = false;
						break;
					}
				}

				if (vazio)
					$scope.endereco = null;

			}, true);

			$scope.limparUf = function () {
				$scope.endereco.idEstado = null;
				$scope.limparCidade();
			};

			$scope.limparCidade = function () {
				$scope.endereco.idCidade = null;
				$scope.limparBairro();
			};

			$scope.limparBairro = function () {
				$scope.endereco.idBairro = null;
			};

			$scope.buscarCep = function () {

				var cep = '';

				if ($scope.endereco.cep) {
					cep = $scope.endereco.cep;
				}

				cep = cep.replace(/[^\d]+/g, '');

				if (!cep)
					cep = '';

				if (cep) {
					Api.CEP.obter({ cep: cep }, function (data) {

						$scope.endereco.idPais = data.id_pais;
						$scope.endereco.idEstado = data.id_uf;
						$scope.endereco.idCidade = data.id_cidade;
						$scope.endereco.logradouro = data.logradouro;
						$scope.endereco.idBairro = data.id_bairro;
						$scope.endereco.idTipoLogradouro = data.id_tipo_logradouro;

					}, function (response) {
						if (response.status === 404) {
							toastr.warning('CEP não localizado! Digite o endereço', 'Atenção');
						} else {
							toastr.error('Erro ao buscar CEP!', 'Erro');
						}
					});
				}
			};

		}],
		templateUrl: 'app/_shared/endereco/templateEndereco.html'
	};

})

.directive('ngUpdateHidden', function () {
	return function (scope, el, attr) {
		var model = attr['ngModel'];
		scope.$watch(model, function (nv) {
			el.val(nv);
		});
	};
})

.directive('uploads', function () {
	return {
		restrict: 'AE',
		transclude: true,
		scope: {
			context: '@',
			entidade: '=',
			identificador: '='
		},
		templateUrl: 'app/cadastros/templateUploads.html'
	};
})

	.directive('onlynumber', function () {
		return {
			require: 'ngModel',
			link: function (scope, element, attrs, modelCtrl) {
				var uppercase = function (inputValue) {
					if (inputValue == undefined) inputValue = '';

					var uppercased = inputValue.match(/\d/g);
					uppercased = uppercased.join("");

					if (uppercased !== inputValue) {
						modelCtrl.$setViewValue(uppercased);
						modelCtrl.$render();
					}

					return uppercased;
				}
				modelCtrl.$parsers.push(uppercase);
				modelCtrl.$formatters.push(uppercase);
				uppercase(scope[attrs.ngModel]);
			}
		};
	})


.directive('uppercase', function () {
	return {
		require: 'ngModel',
		link: function (scope, element, attrs, ngModel)
		{
			ngModel.$parsers.push(function (input) { return input ? input.toUpperCase() : ""; });
			element.css("text-transform", "uppercase");
		}
	};
})

.directive('focus', function ($timeout) {
	return {
		scope: {
			trigger: '=focus'
		},
		link: function (scope, element) {
			scope.$watch('trigger', function (value) {
				if (value) {
					$timeout(function () {
						element[0].focus();
						element[0].select();
					});
				}
			});
		}
	};
});
