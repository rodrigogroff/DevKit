
angular.module('app.controllers').controller('CacheController',
[ '$scope', '$rootScope', 
function ( $scope, $rootScope)
{
	$rootScope.exibirMenu = true;
    $scope.viewModel = [];

	function init()
	{
        Api.Cache.listPage({ }, function (data)
        {
            $scope.viewModel = data;
		});
	}

    $scope.refresh = function ()
    {
        init();
    }
    
    init();

}]);
