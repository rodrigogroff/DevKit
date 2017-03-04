'use strict';

var validatorModule = angular.module('xtForm')

.directive('xtValidateCep', function () {
	return {
		require: 'ngModel',
		link: function (scope, element, attrs, ngModel) {

			var isValid = function (cep) {

				if (!cep)
					cep = '';

				if (cep == '')
					return true;

				cep = cep.replace(/[^\d]+/g, '');

				if (cep.length != 8)
					return false;

				return parseInt(cep) > 0;
			};

			ngModel.$parsers.unshift(function (value) {
				var valid = isValid(value);
				ngModel.$setValidity('cep', valid);
				return valid ? value : undefined;
			});

			ngModel.$formatters.unshift(function (value) {
				var valid = isValid(value);
				ngModel.$setValidity('cep', valid);
				return value;
			});
		}
	};
})

.directive('xtValidateCnpj', function () {
	return {
		require: 'ngModel',
		link: function (scope, element, attrs, ngModel) {

			var isValid = function (cnpj) {

				if (!cnpj)
					cnpj = '';

				if (cnpj == '')
					return true;

				cnpj = cnpj.replace(/[^\d]+/g, '');

				if (cnpj.length != 14)
					return false;

				var soma = 0;
				var tamanho = cnpj.length - 2
				var numeros = cnpj.substring(0, tamanho);
				var digitos = cnpj.substring(tamanho);

				var pos = tamanho - 7;

				for (var i = tamanho; i >= 1; i--) {
					soma += numeros.charAt(tamanho - i) * pos--;
					if (pos < 2)
						pos = 9;
				}

				var resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;

				if (resultado != digitos.charAt(0))
					return false;

				soma = 0;
				tamanho = tamanho + 1;
				numeros = cnpj.substring(0, tamanho);
				pos = tamanho - 7;

				for (var i = tamanho; i >= 1; i--) {
					soma += numeros.charAt(tamanho - i) * pos--;
					if (pos < 2)
						pos = 9;
				}

				resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;

				if (resultado != digitos.charAt(1))
					return false;

				return true;
			};

			ngModel.$parsers.unshift(function (value) {
				var valid = isValid(value);
				ngModel.$setValidity('cnpj', valid);
				return valid ? value : undefined;
			});

			ngModel.$formatters.unshift(function (value) {
				var valid = isValid(value);
				ngModel.$setValidity('cnpj', valid);
				return value;
			});
		}
	};
});