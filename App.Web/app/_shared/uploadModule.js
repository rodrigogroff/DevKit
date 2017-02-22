/*
 * jQuery File Upload Plugin Angular JS Example 1.0.3
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2013, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */

/*jslint regexp: true */
/*global window, angular, navigator */

(function () {
	'use strict';

	var url = '/Backload/FileHandler';

	angular.module('uploadModule', ['blueimp.fileupload'])

        .config([
            '$httpProvider', 'fileUploadProvider',
            function ($httpProvider, fileUploadProvider) {
            	angular.extend(fileUploadProvider.defaults, {
            		maxFileSize: 5000000,
            		acceptFileTypes: /(\.|\/)(gif|jpe?g|png|pdf)$/i,
            		disableImageResize: /Android(?!.*Chrome)|Opera/.test(window.navigator && navigator.userAgent),
            	});
            }
        ])

        .controller('DemoFileUploadController', ['$rootScope', '$scope', '$http', function ($rootScope, $scope, $http) {

        	$scope.loadingFiles = true;

        	$scope.options = {
        		url: url
        	};

        	$scope.handleClick = function (event) {
        		event.preventDefault();
        	};

        	$scope.download = function (file) {
        		console.log(file);
        		window.document.location.href = file.url;
        	};

        	$http.get(url, { params: { objectContext: $scope.context, uploadContext: $scope.identificador } }).then(
				function (response) {
					$scope.loadingFiles = false;
					$scope.queue = response.data.files || [];
					$scope.entidade.files = $scope.queue;
				},
				function () {
					$scope.loadingFiles = false;
				}
			);

        }])

        .controller('FileDestroyController', ['$scope', '$http', function ($scope, $http) {

        	var file = $scope.file, state;

        	if (file.url) {
        		file.$state = function () {
        			return state;
        		};
        		file.$destroy = function () {
        			state = 'pending';
        			return $http({
        				url: file.deleteUrl,
        				method: file.deleteType
        			}).then(
						function () {
							state = 'resolved';
							$scope.clear(file);
						},
						function () {
							state = 'rejected';
						}
					);
        		};
        	} else if (!file.$cancel) {
        		file.$cancel = function () {
        			$scope.clear(file);
        		};
        	}
        }]);

}());