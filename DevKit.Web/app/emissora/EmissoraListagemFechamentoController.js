
angular.module('app.controllers').controller('EmissoraListagemFechamentoController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    $rootScope.exibirMenu = true;
    $scope.loading = false;
    $scope.tipoFech = 1;
    $scope.date = new Date();

    $scope.campos = {
        mes_inicial: $scope.date.getMonth() + 1,
        ano_inicial: $scope.date.getFullYear(),
        selects: {
            mes: ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 }),
        }
    };

    var invalidCheck = function (element) {
        if (element == undefined)
            return true;
        else
            if (element.length == 0)
                return true;

        return false;
    }

    $scope.search = function ()
    {
        if (invalidCheck($scope.campos.mes_inicial) ||
            invalidCheck($scope.campos.ano_inicial) )
        {
            toastr.error('Informe os filtros corretamente', 'Erro');
            return;
        }
        
        $scope.loading = true;

        var opcoes = {
            tipoFech: $scope.tipoFech,
            mes: $scope.campos.mes_inicial,
            ano: $scope.campos.ano_inicial,
        };

        Api.EmissoraFechamento.listPage(opcoes, function (data)
        {
            $scope.list = data.results;
            $scope.totalFechamento = data.totalFechamento;
            $scope.totalCartoes = data.totalCartoes;
            $scope.convenio = data.convenio;
            $scope.loading = false;
        });
    }

    $scope.imprimir = function ()
    {
        if ($scope.tipoFech == '1')
        {
            var printContents = "<h2>CONVEYNET BENEFÍCIOS</h2>";

            printContents += "<h3>Relatório de Fechamento por cartões</h3>";
            printContents += "Data de emissão: " + $scope.date.getDate() + "/" + ($scope.date.getMonth() + 1) + "/" + $scope.date.getFullYear();
            printContents += "<table>";

            printContents += "<tr><td>Convênio: " + $scope.convenio + "</td></tr>";
            printContents += "<tr><td>Período: " + $scope.campos.mes_inicial + " / " + $scope.campos.ano_inicial + "</td></tr>";
            printContents += "<tr><td>Total fechamento: R$ " + $scope.totalFechamento + "</td></tr>";
            printContents += "<tr><td>Total cartões: " + $scope.totalCartoes + "</td></tr>";

            printContents += "</table>";

            for (var i = 0; i < $scope.list.length; ++i)
            {
                var mdl = $scope.list[i];

                printContents += "<br>Fechamento para: <b>" + mdl.associado + "</b> , Matrícula: " + mdl.matricula + "<br />";

                printContents += "<table><thead><tr><th align='left'>Estabelecimento</th><th align='left'>Data compra</th><th align='left'>Valor</th><th align='left'>Parcela</th></tr></thead>";

                for (var t = 0; t < mdl.vendas.length; ++t)
                {
                    var venda = mdl.vendas[t];
                
                    printContents += "<tr>";
                    printContents += "<td width='350px'>" + venda.lojista + "</td>";
                    printContents += "<td width='120px'>" + venda.dtCompra + "</td>";
                    printContents += "<td width='120px'>" + venda.valor + "</td>";
                    printContents += "<td width='120px'>" + venda.parcela + "</td>";
                    printContents += "</tr>";
                }

                printContents += "</table>";
                printContents += "<br>Total: R$ " + mdl.total + "<br><br>";
            }
            
            var popupWin = window.open('', '_blank', 'width=800,height=600');
            popupWin.document.open();
            popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
            popupWin.document.close();
        }     
    }

}]);
