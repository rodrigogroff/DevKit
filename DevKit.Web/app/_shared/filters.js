'use strict';

angular.module('app.filters', ['ngResource'])
.filter('percentage', ['$filter', function ($filter) {
	return function (input, decimals) {
		return $filter('number')(input * 100, decimals) + '%';
	};
}])

.filter('cnpj', ['$filter', function ($filter) {
	return function (input) {
		if (!input) return '';
		return input.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, "$1.$2.$3/$4-$5");
	}
}])

.filter('cep', ['$filter', function ($filter) {
	return function (input) {
		if (!input) return '';
		return input.replace(/^(\d{5})(\d{3})/, "$1-$2");
	}
}])

    .filter('multicliente', ['$filter', function ($filter)
    {
		var verificaClienteNome = function (item, val)
		{
    		if (item.nomeCliente.indexOf(val) >= 0)
    			return true;

    		return false;
    	};

    	var verificaUNNome = function (item, val)
    	{
    		if (item.lstUn)
    		{
    			for (var idx = 0; idx < item.lstUn.length; idx++)
    			{
    				var un = item.lstUn[idx];
    				if (un.nome.indexOf(val) >= 0)
    					return true;
    			}
    		}

    		return false;
    	};

    	return function (itens, parametros)
    	{
    		if (!itens) return [];

    		var clienteNome = parametros['nome'];
    		var nomeUnidade = parametros['nomeUnidade'];

    		if (!clienteNome && !nomeUnidade) 
    			return itens;
    		
    		var result = [];

    		for (var idx = 0; idx < itens.length; idx++)
    		{
    			var item = itens[idx];
    			var adiciona = true;

    			if (clienteNome) adiciona = verificaClienteNome(item, clienteNome);
    			if (nomeUnidade) adiciona = adiciona && verificaUNNome(item, nomeUnidade);

    			if (adiciona)
    				result.push(item);
    		}

    		return result;
    	}
    }])

	.filter('multiclienteCateg', ['$filter', function ($filter)
	{
		var verificaClienteNome = function (item, val) {
			if (item.nome.indexOf(val) >= 0)
				return true;

			return false;
		};

		var verificaClienteCodigo = function (item, val)
		{
			if (item.id.indexOf(val) >= 0)
				return true;

			return false;
		};

		var verificaUNCodigo = function (item, val) {
			if (item.lstUn) {
				for (var idx = 0; idx < item.lstUn.length; idx++) {
					var un = item.lstUn[idx];
					if (un.id.indexOf(val) >= 0)
						return true;
				}
			}

			return false;
		};

		var verificaUNNome = function (item, val) {
			if (item.lstUn) {
				for (var idx = 0; idx < item.lstUn.length; idx++) {
					var un = item.lstUn[idx];
					if (un.nome.indexOf(val) >= 0)
						return true;
				}
			}

			return false;
		};

		return function (itens, parametros) {

			if (!itens) return [];

			var clienteCodigo = parametros['id'];
			var unCodigo = parametros['codigoUnidade'];
			var clienteNome = parametros['nome'];
			var nomeUnidade = parametros['nomeUnidade'];

			if (!clienteCodigo && !unCodigo && !clienteNome && !nomeUnidade)
			{
				return itens;
			}
				
			var result = [];

			for (var idx = 0; idx < itens.length; idx++)
			{
				var item = itens[idx];
				var adiciona = true;

				if (item.id.indexOf('2') > 0) adiciona = true; else adiciona = false;
				
				//if (clienteCodigo) adiciona = adiciona & verificaClienteCodigo(item, clienteCodigo);
				if (clienteNome) adiciona = adiciona & verificaClienteNome(item, clienteNome);				
				if (unCodigo) adiciona = adiciona & verificaUNCodigo(item, unCodigo);
				if (nomeUnidade) adiciona = adiciona && verificaUNNome(item, nomeUnidade);				

				if (adiciona)
					result.push(item);
			}

			console.log("idx -> FIM");

			return result;
		}
	}])

.filter('multiproduto', ['$filter', function ($filter) {
	var verificaNomeMP = function (item, nomeMP) {
		if (item.nome.indexOf(nomeMP) >= 0)
			return true;

		return false;
	};

	var verificaNomeP = function (item, nomeP) {
		var temCodigo = false;

		if (!item.produtos)
			return false;

		for (var idx = 0; idx < item.produtos.length; idx++) {
			var produto = item.produtos[idx];

			temCodigo = produto.nome.indexOf(nomeP) >= 0;

			if (temCodigo)
				break;
		}

		return temCodigo;
	};

	var verificaBarra = function (item, cdBarra) {
		var temCodigo = false;

		if (!item.produtos)
			return false;

		for (var idx = 0; idx < item.produtos.length; idx++) {
			var produto = item.produtos[idx];
			for (var idx2 = 0; idx2 < produto.codigosBarras.length; idx2++) {
				if (!produto.codigosBarras)
					break;
				var codigo = produto.codigosBarras[idx2];
				temCodigo = codigo.codigoBarras.indexOf(cdBarra) >= 0;
				if (temCodigo)
					break;
			};
			if (temCodigo)
				break;
		};

		return temCodigo;
	};

	return function (itens, parametros) {
		if (!itens) return [];

		var nomeMP = parametros['nomeMP'];
		var nomeP = parametros['nomeP'];
		var cdBarra = parametros['codbarra'];

		if (!nomeMP && !nomeP && !cdBarra)
			return itens;

		var result = [];

		for (var idx = 0; idx < itens.length; idx++) {
			var item = itens[idx];

			var adiciona = true;

			if (nomeMP) adiciona = verificaNomeMP(item, nomeMP);
			if (nomeP) adiciona = adiciona && verificaNomeP(item, nomeP);
			if (cdBarra) adiciona = adiciona && verificaBarra(item, cdBarra);

			if (adiciona)
				result.push(item);
		}

		return result;
	}

}])


.filter('mcliente', ['$filter', function ($filter) 
{
	var verificaNomeCli = function (item, nomeCli) 
	{
		if (item.nome.indexOf(nomeCli) >= 0)
			return true;

		return false;
	};

	var verificaNomeFor = function (item, nomeUN) {
		for (var idx = 0; idx < item.length; idx++) {
			if (item[idx].nome.indexOf(nomeUN) >= 0)
				return true;
		}

		return false;
	};

	return function (itens, parametros) 
	{
		if (!itens) return [];

		var nome = parametros['nome'];
		var nomeUnidade = parametros['nomeUnidade'];
		var nomeConj = parametros['nomeConj'];
		var nomeCat = parametros['nomeCat'];
		
		if (!nome && !nomeUnidade && !nomeConj && !nomeCat)
			return itens;

		var result = [];

		for (var idx = 0; idx < itens.length; idx++)
		{
			var item = itens[idx];
			var adiciona = true;

			if (nome) adiciona = verificaNomeCli(item, nome);
			if (nomeUnidade) adiciona = adiciona && verificaNomeFor(item.unidadesNegocio, nomeUnidade);
			if (nomeConj) adiciona = adiciona && verificaNomeFor(item.conjuntoProdutos, nomeConj);
			if (nomeCat) adiciona = adiciona && verificaNomeFor(item.categoria, nomeCat);
			
			if (adiciona)
				result.push(item);
		}

		return result;
	}

}]);
