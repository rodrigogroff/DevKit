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

    //.directive('myEnter', function ()
    //{
    //    return {
    //        require: 'ngModel',
    //        link: function (scope, element, attrs) {
    //            element.bind("keydown keypress", function (event) {
    //                if (event.which === 13) {
    //                    scope.$apply(function () {
    //                        scope.$eval(attrs.myEnter);
    //                    });

    //                    event.preventDefault();
    //                }
    //            });
    //        }
    //    };
    //})


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
})
