
angular.module('app.controllers').controller('EmissoraListagemLojaController',
    ['$scope', '$rootScope', 'ngHistoricoFiltro', 'Api', 'ngSelects',
        function ($scope, $rootScope, ngHistoricoFiltro, Api, ngSelects) {

            $rootScope.exibirMenu = true;
            $scope.loading = false;
            $scope.tipo = $rootScope.tipo;

            $scope.itensporpagina = 15;

            $scope.campos = {
                nome: '',
                codigo: '',
                cnpj: '',
                endereco: '',
                selects: {
                    empresa: ngSelects.obterConfiguracao(Api.Empresa, { tamanhoPagina: 15 }),
                }
            };

            $scope.date = new Date();

            init();

            function init() {
                if (ngHistoricoFiltro.filtro)
                    ngHistoricoFiltro.filtro.exibeFiltro = false;
            }

            $scope.search = function () {
                $scope.load(0, $scope.itensporpagina);
                $scope.paginador.reiniciar();
            };

            $scope.load = function (skip, take) {
                if ($scope.tipo == '5') {
                    $scope.emp_fail = $scope.campos.idEmpresa == undefined;
                    if ($scope.emp_fail == true)
                        return;
                }

                $scope.loading = true;

                var opcoes = {
                    idEmpresa: $scope.campos.idEmpresa,
                    skip: skip,
                    take: take,
                    nome: $scope.campos.nome,
                    codigo: $scope.campos.codigo,
                    cnpj: $scope.campos.cnpj,
                    endereco: $scope.campos.endereco,
                };

                Api.EmissoraLoja.listPage(opcoes, function (data) {
                    $scope.list = data.results;
                    $scope.total = data.count;
                    $scope.loading = false;
                });
            };

            $scope.imprimir = function () {
                if ($scope.tipo == '5') {
                    $scope.emp_fail = $scope.campos.idEmpresa == undefined;
                    if ($scope.emp_fail == true)
                        return;
                }

                $scope.loading = true;

                var opcoes = {
                    idEmpresa: $scope.campos.idEmpresa,
                    skip: 0,
                    take: 90000,
                    nome: $scope.campos.nome,
                    codigo: $scope.campos.codigo,
                    cnpj: $scope.campos.cnpj,
                    endereco: $scope.campos.endereco,
                };

                Api.EmissoraLoja.listPage(opcoes, function (data) {
                    $scope.listPrint = data.results;
                    $scope.loading = false;

                    var printContents = "<h2>CONVEYNET BENEFÍCIOS</h2>";

                    printContents += "<h3>Relatório de lojistas</h3>";
                    printContents += "Data de emissão: " + $scope.date.getDate() + "/" + ($scope.date.getMonth() + 1) + "/" + $scope.date.getFullYear();

                    printContents += "<table style='font-size:x-small;'><thead><tr><th align='left'>Código</th><th align='left'>Razão Social</th><th align='left'>CNPJ</th><th align='left'>Endereço</th><th align='left'>Telefone</th><th align='left'>Tx. Admin</th></tr></thead>";

                    for (var i = 0; i < $scope.listPrint.length; ++i) {
                        var mdl = $scope.listPrint[i];

                        printContents += "<tr height='60px'>";
                        printContents += "<td width='80px'  valign='top'>" + mdl.codigo + "</td>";
                        printContents += "<td width='400px' valign='top'>" + mdl.razSoc + "<br>" + mdl.nome + "</td>";
                        printContents += "<td width='120px'  valign='top'>" + mdl.cnpj + "</td>";
                        printContents += "<td width='400px'  valign='top'>" + mdl.end + "<br />" + mdl.cidade + " - " + mdl.estado + "</td>";
                        printContents += "<td width='120px'  valign='top'>" + mdl.telefone + "</td>";
                        printContents += "<td width='90px'  valign='top'>" + mdl.txAdmin + "</td>";
                        printContents += "</tr>";
                    }

                    printContents += "</table>";

                    var popupWin = window.open('', '_blank', 'width=800,height=600');
                    popupWin.document.open();
                    popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                });
            };

        }]);
