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

}]);
