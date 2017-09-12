angular.module('app.controllers').controller('VendaController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects', 
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects )
{
    $rootScope.exibirMenu = true;

    init();

    function init()
    {
        $scope.loading = false;
        $scope.modoVenda = '';
        $scope.viewModel = {};
    }

    $scope.conferirCartao = function ()
    {
        $scope.viewModel.error = '';

        $scope.stEmpresa_fail = invalidCheck($scope.viewModel.stEmpresa);
        $scope.stMatricula_fail = invalidCheck($scope.viewModel.stMatricula);
        $scope.stAcesso_fail = invalidCheck($scope.viewModel.stAcesso);
        $scope.stVencimento_fail = invalidCheck($scope.viewModel.stVencimento);

        if ($scope.stEmpresa_fail ||
            $scope.stMatricula_fail ||
            $scope.stAcesso_fail ||
            $scope.stVencimento_fail) {
            return;
        }

        var tag = $scope.viewModel.stEmpresa +
            $scope.viewModel.stMatricula +
            $scope.viewModel.stAcesso +
            $scope.viewModel.stVencimento;

        if (tag == $scope.lastTag)
            return;

        $scope.lastTag = tag;
        $scope.modoVenda = '';

        $scope.loading = true;

        Api.Associado.listPage({
            empresa: $scope.viewModel.stEmpresa,
            matricula: $scope.viewModel.stMatricula,
            acesso: $scope.viewModel.stAcesso,
            vencimento: $scope.viewModel.stVencimento,
        },
        function (data) {
            $scope.viewModel.data = data.results[0];
            $scope.loading = false;
        },
        function (response) {                
            $scope.viewModel.error = "Cartão Inválido";
            $scope.loading = false;

            $scope.viewModel.data = undefined;
            $scope.viewModel.parcelas = '';
            $scope.viewModel.valor = '';
            $scope.simulacao = undefined;
            $scope.modoVenda = '';
            $scope.erro = '';
            $scope.lastTag = '';
        });
    }
    
    var invalidCheck = function (element) {
        if (element == undefined)
            return true;
        else
            if (element.length == 0)
                return true;

        return false;
    }
    
    $scope.cancelarSimula = function ()
    {
        $scope.viewModel.stEmpresa = '';
        $scope.viewModel.stMatricula = ''; 
        $scope.viewModel.stAcesso = '';
        $scope.viewModel.stVencimento = '';
        $scope.viewModel.data = undefined;
        $scope.viewModel.parcelas = '';
        $scope.viewModel.valor = '';
        $scope.simulacao = undefined;
        $scope.modoVenda = '';
        $scope.erro = '';
        $scope.lastTag = '';
    }
    
    $scope.parcelar = function ()
    {
        $scope.valor_fail = invalidCheck($scope.viewModel.valor);
        $scope.parcelas_fail = invalidCheck($scope.viewModel.parcelas);

        if ($scope.valor_fail || $scope.parcelas_fail)
            return;

        $scope.loading = true;
        $scope.erro = '';

        if ($scope.viewModel.parcelas != $scope.viewModel.parcelasSim)
        {
            $scope.viewModel.p1 = ''; $scope.viewModel.p2 = ''; $scope.viewModel.p3 = '';
            $scope.viewModel.p4 = ''; $scope.viewModel.p5 = ''; $scope.viewModel.p6 = '';
            $scope.viewModel.p7 = ''; $scope.viewModel.p8 = ''; $scope.viewModel.p9 = '';
            $scope.viewModel.p10 = ''; $scope.viewModel.p11 = ''; $scope.viewModel.p12 = '';
        }

        Api.SimulaParcelada.listPage(
        {
            cartao: $scope.viewModel.data.id,
            valor: $scope.viewModel.valor,
            parcelas: $scope.viewModel.parcelas,
            p1: $scope.viewModel.p1,
            p2: $scope.viewModel.p2,
            p3: $scope.viewModel.p3,
            p4: $scope.viewModel.p4,
            p5: $scope.viewModel.p5,
            p6: $scope.viewModel.p6,
            p7: $scope.viewModel.p7,
            p8: $scope.viewModel.p8,
            p9: $scope.viewModel.p9,
            p10: $scope.viewModel.p10,
            p11: $scope.viewModel.p11,
            p12: $scope.viewModel.p12,
        },
        function (data)
        {
            $scope.modoVenda = 'simulacao';

            $scope.viewModel.p1 = ''; $scope.viewModel.p2 = ''; $scope.viewModel.p3 = '';
            $scope.viewModel.p4 = ''; $scope.viewModel.p5 = ''; $scope.viewModel.p6 = '';
            $scope.viewModel.p7 = ''; $scope.viewModel.p8 = ''; $scope.viewModel.p9 = '';
            $scope.viewModel.p10 = ''; $scope.viewModel.p11 = ''; $scope.viewModel.p12 = '';
            $scope.viewModel.p1m = ''; $scope.viewModel.p2m = ''; $scope.viewModel.p3m = '';
            $scope.viewModel.p4m = ''; $scope.viewModel.p5m = ''; $scope.viewModel.p6m = '';
            $scope.viewModel.p7m = ''; $scope.viewModel.p8m = ''; $scope.viewModel.p9m = '';
            $scope.viewModel.p10m = ''; $scope.viewModel.p11m = ''; $scope.viewModel.p12m = '';

            if ($scope.viewModel.parcelas >= 1) $scope.viewModel.p1 = data.results[0].valor;
            if ($scope.viewModel.parcelas >= 2) $scope.viewModel.p2 = data.results[1].valor;
            if ($scope.viewModel.parcelas >= 3) $scope.viewModel.p3 = data.results[2].valor;
            if ($scope.viewModel.parcelas >= 4) $scope.viewModel.p4 = data.results[3].valor;
            if ($scope.viewModel.parcelas >= 5) $scope.viewModel.p5 = data.results[4].valor;
            if ($scope.viewModel.parcelas >= 6) $scope.viewModel.p6 = data.results[5].valor;
            if ($scope.viewModel.parcelas >= 7) $scope.viewModel.p7 = data.results[6].valor;
            if ($scope.viewModel.parcelas >= 8) $scope.viewModel.p8 = data.results[7].valor;
            if ($scope.viewModel.parcelas >= 9) $scope.viewModel.p9 = data.results[8].valor;
            if ($scope.viewModel.parcelas >= 10) $scope.viewModel.p10 = data.results[9].valor;
            if ($scope.viewModel.parcelas >= 11) $scope.viewModel.p11 = data.results[10].valor;
            if ($scope.viewModel.parcelas >= 12) $scope.viewModel.p12 = data.results[11].valor;
            
            if ($scope.viewModel.parcelas >= 1) $scope.viewModel.p1m = data.results[0].valorMax;
            if ($scope.viewModel.parcelas >= 2) $scope.viewModel.p2m = data.results[1].valorMax;
            if ($scope.viewModel.parcelas >= 3) $scope.viewModel.p3m = data.results[2].valorMax;
            if ($scope.viewModel.parcelas >= 4) $scope.viewModel.p4m = data.results[3].valorMax;
            if ($scope.viewModel.parcelas >= 5) $scope.viewModel.p5m = data.results[4].valorMax;
            if ($scope.viewModel.parcelas >= 6) $scope.viewModel.p6m = data.results[5].valorMax;
            if ($scope.viewModel.parcelas >= 7) $scope.viewModel.p7m = data.results[6].valorMax;
            if ($scope.viewModel.parcelas >= 8) $scope.viewModel.p8m = data.results[7].valorMax;
            if ($scope.viewModel.parcelas >= 9) $scope.viewModel.p9m = data.results[8].valorMax;
            if ($scope.viewModel.parcelas >= 10) $scope.viewModel.p10m = data.results[9].valorMax;
            if ($scope.viewModel.parcelas >= 11) $scope.viewModel.p11m = data.results[10].valorMax;
            if ($scope.viewModel.parcelas >= 12) $scope.viewModel.p12m = data.results[11].valorMax;

            $scope.viewModel.valor = data.results[$scope.viewModel.parcelas].valor;
            $scope.viewModel.parcelasSim = $scope.viewModel.parcelas;

            $scope.loading = false;
        },
        function (response)
        {
            $scope.loading = false;
            $scope.modoVenda = '';
            
            $scope.erro = response.data.message;
        });
    }
    
    $scope.efetuarVenda = function ()
    {
        $scope.erroSoma = '';

        Api.SomaParcelada.listPage(
        {
            valor: $scope.viewModel.valor,
            parcelas: $scope.viewModel.parcelas,
            p1: $scope.viewModel.p1,
            p2: $scope.viewModel.p2,
            p3: $scope.viewModel.p3,
            p4: $scope.viewModel.p4,
            p5: $scope.viewModel.p5,
            p6: $scope.viewModel.p6,
            p7: $scope.viewModel.p7,
            p8: $scope.viewModel.p8,
            p9: $scope.viewModel.p9,
            p10: $scope.viewModel.p10,
            p11: $scope.viewModel.p11,
            p12: $scope.viewModel.p12,
        },
        function (data)
        {
            $scope.loading = false;
            $scope.modoVenda = 'confirmacao';
            $scope.viewModel.requerSenha = data.results[0];
        },
        function (response)
        {
            $scope.loading = false;
            $scope.erroSoma = response.data.message;
        });
    }

    $scope.cancelar = function () {
        $scope.modoVenda = 'simulacao';
    }

    $scope.confirmar = function ()
    {
        Api.EfetuaVenda.listPage(
            {
                cartao: $scope.viewModel.data.id,
                empresa: $scope.viewModel.stEmpresa,
                matricula: $scope.viewModel.stMatricula,
                codAcesso: $scope.viewModel.stAcesso,
                stVencimento: $scope.viewModel.stVencimento,
                valor: $scope.viewModel.valor,
                parcelas: $scope.viewModel.parcelas,
                p1: $scope.viewModel.p1,
                p2: $scope.viewModel.p2,
                p3: $scope.viewModel.p3,
                p4: $scope.viewModel.p4,
                p5: $scope.viewModel.p5,
                p6: $scope.viewModel.p6,
                p7: $scope.viewModel.p7,
                p8: $scope.viewModel.p8,
                p9: $scope.viewModel.p9,
                p10: $scope.viewModel.p10,
                p11: $scope.viewModel.p11,
                p12: $scope.viewModel.p12,
            },
            function (data)
            {                
                $scope.viewModel.cupom = data.results;                
            },
            function (response) {
                $scope.loading = false;
                toastr.error(response.data.message, 'Error');
            });
    }
    
    $scope.printDiv = function (divName) {
        var printContents = "<table>";

        printContents += "<tr><td>" + $scope.viewModel.cupom[0] + "</td></tr>";
        printContents += "<tr><td>" + $scope.viewModel.cupom[1] + "</td></tr>";
        printContents += "<tr><td>" + $scope.viewModel.cupom[2] + "</td></tr>";
        printContents += "<tr><td>" + $scope.viewModel.cupom[3] + "</td></tr>";
        printContents += "<tr><td>" + $scope.viewModel.cupom[4] + "</td></tr>";
        printContents += "<tr><td>" + $scope.viewModel.cupom[5] + "</td></tr>";
        printContents += "<tr><td>" + $scope.viewModel.cupom[6] + "</td></tr>";
        printContents += "<tr><td>" + $scope.viewModel.cupom[7] + "</td></tr>";
        printContents += "<tr><td>" + $scope.viewModel.cupom[8] + "</td></tr>";
        printContents += "<tr><td>" + $scope.viewModel.cupom[9] + "</td></tr>";

        var pos = 10;

        for (var i = 0; i < $scope.viewModel.parcelas; ++i)
        {
            printContents += "<tr><td>" + $scope.viewModel.cupom[pos] + "</td></tr>";
            pos = pos + 1;
        }
               
        printContents += "<tr><td>" + $scope.viewModel.cupom[pos] + "</td></tr>"; pos = pos + 1;
        printContents += "<tr><td>" + $scope.viewModel.cupom[pos] + "</td></tr>"; pos = pos + 1;
        printContents += "<tr><td>" + $scope.viewModel.cupom[pos] + "</td></tr>"; pos = pos + 1;
        printContents += "<tr><td>" + $scope.viewModel.cupom[pos] + "</td></tr>"; pos = pos + 1;

        printContents += "</table>"

        var popupWin = window.open('', '_blank', 'width=800,height=600');
        popupWin.document.open();
        popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
        popupWin.document.close();
    }

    $scope.novaVenda = function ()
    {
        init();
    }

}]);

