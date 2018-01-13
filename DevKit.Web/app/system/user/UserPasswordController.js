
angular.module('app.controllers').controller('UserPasswordController',
['$scope', '$state', '$stateParams', '$rootScope', 'Api', 'ngSelects',
function ($scope, $state, $stateParams, $rootScope, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;

	function CheckPermissions()
	{
        Api.Permission.get({ id: $scope.permID }, function (data)
        {
			$scope.permModel = data;
			
			if (!$scope.permModel.listagem) {
				toastr.error('Access denied!', 'Permission');
				$state.go('home');
			}
			else
            {
				$scope.loading = true;
                Api.User.get({ id: 0 }, function (data)
                {
					$scope.viewModel = data;
					$scope.loading = false;
				},
				function (response) {
					if (response.status === 404) { toastr.error('Invalid ID', 'Error'); }
					$scope.list();
				});
			}
		},
		function (response) { });
	}

    var id = ($stateParams.id) ? parseInt($stateParams.id) : 0;

	init();

	function init()
    {
        $scope.viewModel = {};
        $scope.permID = 200;

        $scope.changePassModel =
            {
                stCurrentPassword: '',
                stNewPassword: '',
                stConfirmation: '',
            };

		CheckPermissions();
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
		$state.go('users');
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

            Api.User.update({ id: id }, $scope.viewModel, function (data)
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
