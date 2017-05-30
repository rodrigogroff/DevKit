angular.module('app.services').service('ngSelects', ['Api', function (Api)
{
	var obterConfiguracao = function (api, parametros)
	{
		var tamanhoPagina = 15;
		var campoId = (parametros && parametros.campoId) || 'id';
		var campoNome = (parametros && parametros.campoNome) || 'stName';

		if (parametros && parametros.tamanhoPagina) 
			tamanhoPagina = parametros.tamanhoPagina;
		
		var configuracao =
		{
			allowClear: true,
			itemSelecionado: null,
			placeholder: "Select",

			multiple: (parametros && parametros.selecaoMultipla) || false,

			funcaoDadosIniciais: parametros ? parametros.funcaoDadosIniciais : undefined,

			initSelection: function (element, callback)
			{
				if (parametros && parametros.funcaoDadosIniciais)
				{
					var valores = [];
					var dados = parametros.funcaoDadosIniciais();

					angular.forEach(dados, function (item) {
						valores.push({ id: item[campoId], text: item[campoNome], source: item });
					});

					return callback(valores);
				}
				else
				{
					var opcoesObter = {};
					var id = $(element).select2('val');

					opcoesObter[campoId] = id;
					opcoesObter.combo = true;

					if (parametros && parametros.opcoes)
						angular.extend(opcoesObter, parametros.opcoes);

					api.get(opcoesObter, function (selecionado) {
						return callback({ id: selecionado[campoId], text: selecionado[campoNome], source: selecionado });
					});
				}
			},

			formatSelection: function (item)
			{
				configuracao.itemSelecionado = item.source;
				return item.text;
			},

			formatResult: function (item)
			{
				return item.text;
			},

			query: function (query)
			{
				var objFiltro = {};

				var apenasAtivos = parametros.apenasAtivos != undefined ? parametros.apenasAtivos : true;

				if (parametros && parametros.filtro)
				{
					var valor;

					if (parametros.scope) 
						valor = parametros.scope.$apply(parametros.filtro.valor);
					else 
						valor = parametros.filtro.valor;
					
					if (valor) 
						objFiltro[parametros.filtro.campo] = valor;
					else
						return query.callback({ results: [] });
				}

				if (query.term) 
					objFiltro.busca = query.term;
				
				if (parametros && parametros.opcoesApi) 
					opcoes[parametros.opcoesApi.campo] = parametros.opcoesApi.valor;
				
				if (parametros && parametros.tamanhoPagina)
				{
					objFiltro.skip = (query.page - 1) * tamanhoPagina;
					objFiltro.take = parametros.tamanhoPagina;
				}

				if (parametros && parametros.opcoes)
					angular.extend(objFiltro, parametros.opcoes);

				api.listPage(objFiltro, function (data)
				{
					var lista = [];

					$.each(data.results, function (index, item)
					{
						lista.push({
							id: item[campoId],
							text: item[campoNome],
							source: item
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

	return { obterConfiguracao: obterConfiguracao }
}])