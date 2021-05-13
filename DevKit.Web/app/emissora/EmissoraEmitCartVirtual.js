
angular.module('app.controllers').controller('EmissoraEmitCartVirtualController',
    ['$scope', '$rootScope', 'Api',
        function ($scope, $rootScope, Api) {

            var invalidCheck = function (element) {
                if (element == undefined)
                    return true;
                else
                    if (element.length == 0)
                        return true;

                return false;
            };

            $rootScope.exibirMenu = true;
            $scope.loading = false;

            $scope.modal = false;

            $scope.campos = {
                mat: '',
                nomeCartao: '',
                id: 0,
            };

            $scope.buscar = function () {
                $scope.campos.id = 0;

                $scope.mat_fail = invalidCheck($scope.campos.mat);

                if (!$scope.mat_fail) {
                    $scope.loading = true;

                    var opcoes = { matricula: $scope.campos.mat };

                    Api.EmissoraCartao.listPage(opcoes, function (data) {
                        if (data.results.length > 0) {
                            $scope.campos.nomeCartao = data.results[0].associado;
                            $scope.campos.id = data.results[0].id;
                            $scope.campos.situacao = data.results[0].situacao;

                            Api.EmissoraCartao.get({ id: data.results[0].id }, function (data_2) {
                                $scope.campos.digitos = data_2.digitos;
                                $scope.loading = false;
                            },
                                function (response) {
                                    if (response.status === 404) { toastr.error('Invalid ID', 'Erro'); }
                                    $scope.list();
                                });
                        }
                        else
                            toastr.error('matrícula inválida', 'Erro');

                        $scope.loading = false;
                    },
                        function (response) {
                            toastr.error(response.data.message, 'Erro');
                            $scope.loading = false;
                        });
                }
            };

            $scope.imprimir = function () {

                var printContents = '';

                var ca = $scope.campos.digitos.split(' ');

                console.log(ca);

                printContents = "<style> table, th, td { border: 1px solid black; border-collapse: collapse; } th, td { padding: 5px; text-align: left; } </style>" + 

"<div align='center'><br><img src='/images/convey2020.png' style='height:50px' /><p align='center'>-------------------------------------------------------------------<br><h3>CARTÃO VIRTUAL</h3></p>" +

"<p align='center'>CONVEY BENEFÍCIOS</p>"+
"<p align='center'>Prezado Associado:</p>"+
"<table align='center'><tr><td width='100px'>TITULAR</td><td width='300px'><b>" + $scope.campos.nomeCartao + "</b></td></tr></table>"+ 
"<br>"+ 

"<p style='width:450px'>É um prazer tê-lo conosco, você está recebendo o cartão virtual <b>CONVEY Benefícios</b>, assim, que você cadastrar seu acesso seguro/senha, na sua entidade, já poderá utilizar para suas compras no modo DIGITADO.</p>"+
"<p align='center'>Este cartão estará disponível também, através do <b>APP MEU CONVEY</b> ou através do código abaixo, </p>"+
"<p align='center'>CONVEY VIRTUAL</p>"+

"<table align='center' width='400px'>"+
"<tr>" + 
"<td width='100px'><div align='center'><h1>" + ca[0] + "</h1></div></td>"+
                    "<td width='100px'><div align='center'><h1>" + ca[1] + "</h1></div></td>"+
                    "<td width='100px'><div align='center'><h1>" + ca[2] + "</h1></div></td>"+
                    "<td width='100px'><div align='center'><h1>" + ca[3] + "</h1></div></td>"+
"</tr>"+
"</table>"+

"<p align='center'>*Utilize sempre no modo DIGITADO.<br><br><b>SUA ENTIDADE SEMPRE JUNTO COM VOCÊ!</b></p><br>"+

"<table align='center' width='400px'><tr><td width='200px'><img src='/images/barra_1x.png' style='height:90px' /></td>"+
"<td width='200px'><img src='/images/conveyvirtual.png' style='height:90px' /></td></tr><tr><td>LOJISTA<br>CAPTURE A IMAGEM<br>CELULAR</td><td></td></table>" +
"</div>"

                var popupWin = window.open('', '_blank', 'width=800,height=600');
                popupWin.document.open();
                popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
                popupWin.document.close();
            }

        }]);
