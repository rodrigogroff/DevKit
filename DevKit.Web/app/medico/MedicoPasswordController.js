
angular.module('app.controllers').controller('MedicoPasswordController',
    ['$scope', '$state', '$stateParams', '$rootScope', 'Api', 'ngSelects', 'AuthService',
        function ($scope, $state, $stateParams, $rootScope, Api, ngSelects, AuthService)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;

    init();

	function init()
    {
        AuthService.fillAuthData();
        $scope.authentication = AuthService.authentication;

        $scope.viewModel = {};
        
        $scope.changePassModel =
            {
                stCurrentPassword: '',
                stNewPassword: '',
                stConfirmation: '',
            };
	}

	var invalidCheck = function (element) {
		if (element == undefined)
			return true;
		else
			if (element.length == 0)
				return true;

		return false;
	}

	$scope.list = function () {
		$state.go('home');
	}

	$scope.changePass = function ()
	{
		$scope.stCurrentPasswordFail = invalidCheck($scope.changePassModel.stCurrentPassword);
        $scope.stNewPasswordFail = invalidCheck($scope.changePassModel.stNewPassword);

        $scope.stConfirmationFail = $scope.changePassModel.stNewPassword != $scope.changePassModel.stConfirmation ||
                                    $scope.changePassModel.stNewPassword.length == 0
	
		if (!$scope.stCurrentPasswordFail &&
			!$scope.stCurrentPasswordFail &&
			!$scope.stConfirmationFail)
		{
            $scope.viewModel.updateCommand = "changePassword";
			$scope.viewModel.anexedEntity = $scope.changePassModel;

            Api.Medico.update({ id: 0 }, $scope.viewModel, function (data)
			{			
				$scope.viewModel.anexedEntity = undefined;
                toastr.success('Senha trocada com sucesso!', 'Sucesso');
			},
			function (response) {
				toastr.error(response.data.message, 'Error');
			});
		}
	}

}]);
