angular.module('app.controllers').controller('VendaController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects', 
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects )
{
	$rootScope.exibirMenu = true;
    $scope.loading = false;
    
    $scope.viewModel = {};
    
    var invalidCheck = function (element) {
        if (element == undefined)
            return true;
        else
            if (element.length == 0)
                return true;

        return false;
    }

    $scope.parcelar = function ()
    {
        $scope.simulacao = false;

        $scope.parcelas_fail = invalidCheck($scope.viewModel.parcelas);
        $scope.valor_fail = invalidCheck($scope.viewModel.valor);

        if ($scope.parcelas_fail || $scope.valor_fail)
        {
            return;
        }

        $scope.simulacao = true;

        $scope.p1 = ''; $scope.p2 = ''; $scope.p3 = ''; $scope.p4 = '';
        $scope.p5 = ''; $scope.p6 = ''; $scope.p7 = ''; $scope.p8 = '';

        var valor = $scope.viewModel.valor;
        var posVirg = $scope.viewModel.valor.indexOf(",");
        var parcs = $scope.viewModel.parcelas;

        var valorFloat = 0;              

        if (posVirg >= 0)
        {
            valorFloat = $scope.viewModel.valor.substring(posVirg + 1);

            if (valorFloat.length == 1)
                valorFloat = valorFloat + "0";

            valor = valor.substring(0, posVirg);
        }            
        
        var vlrParc        = valor / parcs;
        var vlrParcDecimal = valorFloat / parcs;
        var quebra         = vlrParcDecimal.indexOf(".");

        if (quebra >= 0)
        {
            vlrParcDecimal = vlrParcDecimal.substring(0, quebra + 1);

            if (vlrParcDecimal.length > 2)
                vlrParcDecimal = vlrParcDecimal.substring(0, 2);

            var total = 0;

            if (parcs >= 1) $scope.p1 = vlrParc + "," + vlrParcDecimal;
            if (parcs >= 2) $scope.p2 = vlrParc + "," + vlrParcDecimal;
            if (parcs >= 3) $scope.p3 = vlrParc + "," + vlrParcDecimal;
            if (parcs >= 4) $scope.p4 = vlrParc + "," + vlrParcDecimal;
            if (parcs >= 5) $scope.p5 = vlrParc + "," + vlrParcDecimal;
            if (parcs >= 6) $scope.p6 = vlrParc + "," + vlrParcDecimal;
            if (parcs >= 7) $scope.p7 = vlrParc + "," + vlrParcDecimal;
            if (parcs >= 8) $scope.p8 = vlrParc + "," + vlrParcDecimal;  
        }
        else
        {
            if (parcs >= 1) $scope.p1 = vlrParc + "," + vlrParcDecimal;
            if (parcs >= 2) $scope.p2 = vlrParc + "," + vlrParcDecimal;
            if (parcs >= 3) $scope.p3 = vlrParc + "," + vlrParcDecimal;
            if (parcs >= 4) $scope.p4 = vlrParc + "," + vlrParcDecimal;
            if (parcs >= 5) $scope.p5 = vlrParc + "," + vlrParcDecimal;
            if (parcs >= 6) $scope.p6 = vlrParc + "," + vlrParcDecimal;
            if (parcs >= 7) $scope.p7 = vlrParc + "," + vlrParcDecimal;
            if (parcs >= 8) $scope.p8 = vlrParc + "," + vlrParcDecimal;           
        }        
    }
    
    $scope.conferir = function ()
    {
        $scope.stEmpresa_fail = invalidCheck($scope.viewModel.stEmpresa);
        $scope.stMatricula_fail = invalidCheck($scope.viewModel.stMatricula);
        $scope.stAcesso_fail = invalidCheck($scope.viewModel.stAcesso);
        $scope.stVencimento_fail = invalidCheck($scope.viewModel.stVencimento);

        if ($scope.stEmpresa_fail ||
            $scope.stMatricula_fail ||
            $scope.stAcesso_fail ||
            $scope.stVencimento_fail)
        {
            return;
        }

        $scope.loading = true;

        Api.Associado.listPage({
            empresa: $scope.viewModel.stEmpresa,
            matricula: $scope.viewModel.stMatricula,
            acesso: $scope.viewModel.stAcesso,
            vencimento: $scope.viewModel.stVencimento,
        },
        function (data)
        {
            $scope.viewModel.data = data.results[0];
            $scope.loading = false;
        },
        function (response)
        {
            $scope.viewModel.data = {};
            $scope.viewModel.data.error = "Cartão Inválido";
            $scope.loading = false;
        });
    }

}]);

