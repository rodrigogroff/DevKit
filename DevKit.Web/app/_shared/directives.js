'use strict';

angular.module('app.directives', [])

    .directive('autoNext', function () {
        return {
            restrict: 'A',
            link: function (scope, element, attr, form) {
                var tabindex = parseInt(attr.tabindex);
                var maxLength = parseInt(attr.ngMaxlength);
                element.on('keypress', function (e) {
                    if (element.val().length > maxLength - 1) {
                        var next = angular.element(document.body).find('[tabindex=' + (tabindex + 1) + ']');
                        if (next.length > 0) {
                            next.focus();
                            return next.triggerHandler('keypress', { which: e.which });
                        }
                        else {
                            return false;
                        }
                    }
                    return true;
                });

            }
        }
    })

    .directive('numeric', function ()
    {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, modelCtrl)
            {
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
