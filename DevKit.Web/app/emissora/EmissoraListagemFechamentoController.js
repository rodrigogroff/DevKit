
angular.module('app.controllers').controller('EmissoraListagemFechamentoController',
['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects',
function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects)
{
    var invalidCheck = function (element) {
        if (element == undefined)
            return true;
        else
            if (element.length == 0)
                return true;

        return false;
    }

    $rootScope.exibirMenu = true;
    $scope.loading = false;

    $scope.sintetico = false;
    $scope.tipoFech = 1;    
    $scope.date = new Date();

    $scope.campos = {
        mes_inicial: $scope.date.getMonth() + 1,
        ano_inicial: $scope.date.getFullYear(),
        selects: {
            mes: ngSelects.obterConfiguracao(Api.MonthCombo, { tamanhoPagina: 15 }),
        }
    };
    
    $scope.search = function ()
    {
        if (invalidCheck($scope.campos.mes_inicial) ||
            invalidCheck($scope.campos.ano_inicial) )
        {
            toastr.error('Informe os filtros corretamente', 'Erro');
            return;
        }
        
        $scope.loading = true;
        $scope.tipoFechSel = undefined;

        var opcoes = {
            tipoFech: $scope.tipoFech,
            mes: $scope.campos.mes_inicial,
            ano: $scope.campos.ano_inicial,
        };

        if ($scope.tipoFech == '1') {
            Api.EmissoraFechamento.listPage(opcoes, function (data) {
                $scope.tipoFechSel = $scope.tipoFech;
                $scope.list = data.results;
                $scope.totalFechamento = data.totalFechamento;
                $scope.totalCartoes = data.totalCartoes;
                $scope.convenio = data.convenio;
                $scope.loading = false;
            });
        }
        else if ($scope.tipoFech == '2') {
            Api.EmissoraFechamento.listPage(opcoes, function (data) {
                $scope.tipoFechSel = $scope.tipoFech;
                $scope.list = data.results;
                $scope.totalFechamento = data.totalFechamento;
                $scope.totalRepasse = data.totalRepasse;
                $scope.totalBonus = data.totalBonus;
                $scope.totalLojistas = data.totalLojistas;
                $scope.convenio = data.convenio;
                $scope.loading = false;
            });
        }
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
                printContents += "<table><thead><tr><th align='left'>Estabelecimento</th><th align='left'>Data compra</th><th align='left'>Valor Parcela</th><th align='left'>Parcela</th></tr></thead>";

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
        else if ($scope.tipoFech == '2') {
            var printContents = "<h2>CONVEYNET BENEFÍCIOS</h2>";

            printContents += "<h3>Relatório de Fechamento por lojistas</h3>";
            printContents += "Data de emissão: " + $scope.date.getDate() + "/" + ($scope.date.getMonth() + 1) + "/" + $scope.date.getFullYear();
            printContents += "<table>";

            //<h3>ConveyNET Benefícios</h3>
            //Convênio: <b> {{ convenio }}</b> <br />
            //Período: <b> {{ campos.mes_inicial }} / {{ campos.ano_inicial }}</b> <br />
            //Total fechamento convênio: <b>R$ {{ totalFechamento }}</b> <br />
            //Total repasse fornecedores: <b>R$ {{ totalRepasse }}</b> <br />
            //Total bonificação: <b>R$ {{ totalBonus }}</b> <br />
            //Quantidade lojistas: <b> {{ totalLojistas }}</b> <br />
            
            printContents += "<tr><td>Convênio: " + $scope.convenio + "</td></tr>";
            printContents += "<tr><td>Período: " + $scope.campos.mes_inicial + " / " + $scope.campos.ano_inicial + "</td></tr>";
            printContents += "<tr><td>Total fechamento convênio: R$ " + $scope.totalFechamento + "</td></tr>";
            printContents += "<tr><td>Total repasse lojistas: R$ " + $scope.totalRepasse + "</td></tr>";
            printContents += "<tr><td>Total bonificação: R$ " + $scope.totalBonus + "</td></tr>";
            printContents += "<tr><td>Quantidade lojistas: " + $scope.totalLojistas + "</td></tr>";

            printContents += "</table>";

            for (var i = 0; i < $scope.list.length; ++i)
            {
                var mdl = $scope.list[i];

                printContents += "<br>Fechamento para: <b>" + mdl.lojista + " - " + mdl.sigla + "</b> , Endereco: " + mdl.endereco + "<br />";

                printContents += "<table><thead><tr>";
                printContents += "<th width='50px' align='left'>ID</th>"
                printContents += "<th width='50px' align='left'>NSU</th>"
                printContents += "<th width='270px' align='left'>Associado</th>"
                printContents += "<th width='90px' align='left'>Cartão</th>"
                printContents += "<th width='120px' align='left'>Data Compra</th>"
                printContents += "<th width='90px' align='left'>Valor</th>"
                printContents += "<th width='70px' align='left'>Parcela</th>"
                printContents += "<th width='60px' align='left'>Terminal</th>"
                printContents += "</tr ></thead > ";

                for (var t = 0; t < mdl.vendas.length; ++t)
                {
                    var venda = mdl.vendas[t];

                    printContents += "<tr>";
                    printContents += "<td>" + venda.id + "</td>";
                    printContents += "<td>" + venda.nsu + "</td>";
                    printContents += "<td>" + venda.associado + "</td>";
                    printContents += "<td>" + venda.matricula + "</td>";
                    printContents += "<td>" + venda.dtCompra + "</td>";
                    printContents += "<td>" + venda.valor + "</td>";
                    printContents += "<td>" + venda.parcela + "</td>";
                    printContents += "<td>" + venda.terminal + "</td>";
                    
                    printContents += "</tr>";
                }

                printContents += "</table>";
                printContents += "<br>Bonificação: " + mdl.taxa;
                printContents += "<br>Valor total: " + mdl.total;
                printContents += "<br>Valor bonificação: " + mdl.bonus;
                printContents += "<br>Valor a repassar: " + mdl.repasse;
                printContents += "<br><br>";
            }

            var popupWin = window.open('', '_blank', 'width=900,height=600');
            popupWin.document.open();
            popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
            popupWin.document.close();
        }     
    }

}]);
