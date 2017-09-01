'use strict';

angular.module('app.directives', [])

    /*
.directive('priceOnly', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {

            modelCtrl.$parsers.push(function (inputValue) {
                var transformedInput = inputValue ? inputValue.replace(/[^\d,-]/g, '') : null;

                if (transformedInput != inputValue) {
                    modelCtrl.$setViewValue(transformedInput);
                    modelCtrl.$render();
                }

                return transformedInput;
            });
        }
    };
})       
*/
    .directive('numericOnly', function ()
    {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, modelCtrl) {

            modelCtrl.$parsers.push(function (inputValue) {
                var transformedInput = inputValue ? inputValue.replace(/[^\d.-]/g, '') : null;

                if (transformedInput != inputValue) {
                    modelCtrl.$setViewValue(transformedInput);
                    modelCtrl.$render();
                }

                return transformedInput;
            });
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
})
