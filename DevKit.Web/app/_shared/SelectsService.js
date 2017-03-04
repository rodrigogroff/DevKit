angular.module('app.services').service('Selects', ['Api', function (Api) {

	var obterConfiguracao = function (api, parametros) {

		var campoNome = (parametros && parametros.campoNome) || 'stName';
		var campoId = (parametros && parametros.campoId) || 'id';
		var _campoId = campoId.toLowerCase();
		var _campoNome = campoNome.toLowerCase();
		var tamanhoPagina = 15;

		if (parametros && parametros.tamanhoPagina) {
			tamanhoPagina = parametros.tamanhoPagina;
		}

		var configuracao = {

			allowClear: true,
			placeholder: "Select",
			multiple: (parametros && parametros.selecaoMultipla) || false,
			funcaoDadosIniciais: parametros ? parametros.funcaoDadosIniciais : undefined,
			initSelection: function (element, callback) {
				if (parametros && parametros.funcaoDadosIniciais) {
					var dados = parametros.funcaoDadosIniciais();
					var valores = [];
					angular.forEach(dados, function (item) {
						valores.push({ id: item[_campoId], text: item[_campoNome] });
					});
					return callback(valores);
				}
				else {
					var id = $(element).select2('val');
					var opcoesObter = {};
					opcoesObter[_campoId] = id;
					if (parametros && parametros.opcoes)
						angular.extend(opcoesObter, parametros.opcoes);
					
					api.get(opcoesObter, function (selecionado) {
						return callback({ id: selecionado[_campoId], text: selecionado[_campoNome] });
					});
				}
			},
			formatSelection: function (item) {
				return item.text;
			},
			formatResult: function (item) {
				return item.text;
			},
			query: function (query) {

				var apenasAtivos = parametros.apenasAtivos != undefined ? parametros.apenasAtivos : true;

				var filtroInicial = apenasAtivos ? 'ativo eq true' : '';

				if (parametros && parametros.filtro) {
					var valor;
					if (parametros.scope)
						valor = parametros.scope.$apply(parametros.filtro.valor);
					else
						valor = parametros.filtro.valor;


					if (valor) {
						filtroInicial = parametros.filtro.campo + " eq " + valor;
					}
					else
						return query.callback({ results: [] });
				}

				if (api == Api.Pergunta)
					campoNome = 'Descricao';

				var filtro = '';

				if (filtroInicial) {
					filtro = filtroInicial;
					if (query.term)
						filtro += ' and ';
				}

				if (query.term) {
					filtro += "substringof('" + query.term + "'," + campoNome + ")";
				}

				var opcoesListar = { $inlinecount: 'allpages', flat: true, $select: campoId + ',' + campoNome };
				if (filtro)
					opcoesListar.$filter = filtro;

				if (parametros && parametros.opcoesApi) {
					opcoes[parametros.opcoesApi.campo] = parametros.opcoesApi.valor;
				}

				if (parametros && parametros.tamanhoPagina) {
					opcoesListar.$skip = (query.page - 1) * tamanhoPagina;
					opcoesListar.$top = parametros.tamanhoPagina;
				}

				if (parametros && parametros.opcoes)
					angular.extend(opcoesListar, parametros.opcoes);

				api.listPage(opcoesListar, function (data) {
					var lista = [];
					if (parametros && parametros.buscaPorTrecho && query.page == 1 && query.term) {
						lista.push({
							id: -1,
							text: 'Contendo "' + query.term + '"',
							buscaPorTrecho: query.term
						});
					}

					$.each(data.results, function (index, item) {
						lista.push({
							id: item[_campoId],
							text: item[_campoNome]
						});
					});
					var temMais = false;
					if (parametros && parametros.tamanhoPagina)
						temMais = (query.page * parametros.tamanhoPagina) < data.count;

					return query.callback({ results: lista, more: temMais });
				});
			}
		};

		return configuracao;
	};

	return {
		obterConfiguracao: obterConfiguracao
	}

}])
