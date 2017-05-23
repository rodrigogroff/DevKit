'use strict';

angular.module('app.directives', [])

.directive('appVersion', ['version', function (version) {
	return function (scope, elm, attrs) {
		elm.text(version);
	};
}])

    /*
    .directive('maskMoney', function($timeout, $locale) {
        return {
            restrict: 'A',
            require: 'ngModel',
            scope: {
                model: '=ngModel',
                mmOptions: '=?',
                prefix: '@',
                suffix: '@',
                affixesStay: '=',
                thousands: '@',
                decimal: '@',
                precision: '=',
                allowZero: '=',
                allowNegative: '='
            },
            link: function(scope, el, attr, ctrl) {

                scope.$watch(checkOptions, init, true);

                scope.$watch(attr.ngModel, eventHandler, true);
                //el.on('keyup', eventHandler); //change to $watch or $observe

                function checkOptions() {
                    return scope.mmOptions;
                }

                function checkModel() {
                    return scope.model;
                }

                //this parser will unformat the string for the model behid the scenes
                function parser() {
                    return $(el).maskMoney('unmasked')[0];
                }
                
                ctrl.$parsers.push(parser);
                
                ctrl.$formatters.push(function(value){
                    $timeout(function(){
                        init();
                    });
                    return parseFloat(value).toFixed(2);
                });

                function eventHandler() {
                    $timeout(function() {
                        scope.$apply(function() {
                            ctrl.$setViewValue($(el).val());
                        });
                    });
                }

                function init(options) {
                    $timeout(function() {
                        elOptions = {
                            prefix: scope.prefix || '',
                            suffix: scope.suffix || '',
                            affixesStay: scope.affixesStay,
                            //thousands: scope.thousands || $locale.NUMBER_FORMATS.GROUP_SEP,
                            //decimal: scope.decimal || $locale.NUMBER_FORMATS.DECIMAL_SEP,
                            thousands: scope.thousands || '.',
                            decimal: scope.decimal || ',',
                            precision: scope.precision,
                            allowZero: scope.allowZero,
                            allowNegative: scope.allowNegative
                        };

                        if (!scope.mmOptions) {
                            scope.mmOptions = {};
                        }

                        for (var elOption in elOptions) {
                            if (elOptions[elOption]) {
                                scope.mmOptions[elOption] = elOptions[elOption];
                            }
                        }

                        $(el).maskMoney(scope.mmOptions);
                        $(el).maskMoney('mask');
                        eventHandler();

                    }, 0);

                    $timeout(function() {
                        scope.$apply(function() {
                            ctrl.$setViewValue($(el).val());
                        });
                    });

                }
            }
        };
    })
    */
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
