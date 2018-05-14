angular.module('app.controllers').controller('VendaMobileController',
    ['$scope', '$rootScope', 'AuthService', '$state', 'ngHistoricoFiltro', 'Api', 'ngSelects', '$window',
        function ($scope, $rootScope, AuthService, $state, ngHistoricoFiltro, Api, ngSelects, $window) {

            init();

            function init()
            {
                $rootScope.exibirMenu = true;

                $scope.loading = false;
                $scope.mostraModalMobile = false;
                $scope.mostraModalMobileAutorizado = false;
                $scope.processandoVenda = false;
                                
                $scope.lastTag = '';
                $scope.modoVenda = '';
                $scope.autorizando = false;

                $scope.viewModel =
                    {
                        parcelas: 1,
                        valor: '0,00'
                    };

                $scope.viewModel.senhaPortadorCartao = undefined;
                $scope.viewModel.cupom = undefined;
            }

            $scope.confirmarMobile = function () {
                $scope.efetuarVenda();
                $scope.mostraModalMobile = false;
            }

            $scope.$watch("viewModel.stEmpresa", function (novo, anterior) {

                if (novo != undefined)
                    if (novo.length == 6)
                        document.getElementById("cartMat").focus();

            }, true);

            $scope.$watch("viewModel.stMatricula", function (novo, anterior) {

                if (novo != undefined)
                    if (novo.length == 6)
                        document.getElementById("cartAcesso").focus();

            }, true);


            $scope.acesso_zero = false;

            $scope.$watch("viewModel.stAcesso", function (novo, anterior) {

                if (novo == '0')
                    $scope.acesso_zero = true;

                if ($scope.acesso_zero == true)
                    novo = '0' + novo;

                if (novo != undefined)
                    if (novo.length == 4)
                        document.getElementById("cartVenc").focus();


            }, true);

            $scope.pulaMatricula = function () { document.getElementById("cartMat").focus(); }
            $scope.pulaAcesso = function () { document.getElementById("cartAcesso").focus(); }
            $scope.pulaVenc = function () { document.getElementById("cartVenc").focus(); }

            $scope.conferirCartao = function ()
            {
                var stA = $scope.viewModel.stAcesso.toString();
                var stV = $scope.viewModel.stVencimento.toString();

                $scope.viewModel.error = '';
                $scope.valorVelho = '';
                $scope.parcelaVelho = '';

                if (stA.length == 3) stA = "0" + stA;
                if (stV.length == 3) stV = "0" + stV;

                $scope.stEmpresa_fail = invalidCheck($scope.viewModel.stEmpresa);
                $scope.stMatricula_fail = invalidCheck($scope.viewModel.stMatricula);
                $scope.stAcesso_fail = invalidCheck(stA);
                $scope.stVencimento_fail = invalidCheck(stV);

                if ($scope.stEmpresa_fail ||
                    $scope.stMatricula_fail ||
                    $scope.stAcesso_fail ||
                    $scope.stVencimento_fail) {
                    return;
                }

                var tag = $scope.viewModel.stEmpresa +
                    $scope.viewModel.stMatricula +
                    stA +
                    stV;

                if (tag == $scope.lastTag)
                    return;

                $scope.lastTag = tag;
                $scope.modoVenda = '';

                $scope.loading = true;

                Api.Associado.listPage({
                    empresa: $scope.viewModel.stEmpresa.toString(),
                    matricula: $scope.viewModel.stMatricula.toString(),
                    acesso: stA,
                    vencimento: stV,
                },
                function (data) {
                    $scope.viewModel.data = data.results[0];
                    $scope.viewModel.emailCupom = $scope.viewModel.data.email;
                    
                    $scope.loading = false;
                },
                function (response) {                        
                    $scope.loading = false;
                    $scope.viewModel.error = "Cartão Inválido";
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

            $scope.cancelarSimula = function () {
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
                $scope.falhaVendaMsg = '';
            }

            $scope.parcelar = function ()
            {
                $scope.valor_fail = invalidCheck($scope.viewModel.valor);
                $scope.parcelas_fail = invalidCheck($scope.viewModel.parcelas);

                if ($scope.valor_fail || $scope.parcelas_fail)
                    return;

                $scope.loading = true;
                $scope.erro = '';

                if ($scope.viewModel.parcelas != $scope.viewModel.parcelasSim) {
                    $scope.viewModel.p1 = ''; $scope.viewModel.p2 = ''; $scope.viewModel.p3 = '';
                    $scope.viewModel.p4 = ''; $scope.viewModel.p5 = ''; $scope.viewModel.p6 = '';
                    $scope.viewModel.p7 = ''; $scope.viewModel.p8 = ''; $scope.viewModel.p9 = '';
                    $scope.viewModel.p10 = ''; $scope.viewModel.p11 = ''; $scope.viewModel.p12 = '';
                }

                if ($scope.viewModel.parcelas.length == 2)
                    if ($scope.viewModel.parcelas != 10)
                        if ($scope.viewModel.parcelas[0] == '0')
                            $scope.viewModel.parcelas = $scope.viewModel.parcelas[1];

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
                    function (data) {
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

                        $scope.valorVelho = $scope.viewModel.valor;
                        $scope.parcelaVelho = $scope.viewModel.parcelas;

                        $scope.mostraModalMobile = true;
                    },
                    function (response) {
                        $scope.falhaVendaMsg = response.data.message;
                        $scope.erro = response.data.message;
                        $scope.loading = false;
                    });

            }

            $scope.efetuarVenda = function () {
                
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
                    function (data) {
                        $scope.modoVenda = 'confirmacao';
                        $scope.viewModel.requerSenha = data.results[0].requerSenha;
                    },
                    function (response) {
                        $scope.loading = false;
                        $scope.erroSoma = response.data.message;
                    });

                if ($scope.loading == true)
                {
                    var stA = $scope.viewModel.stAcesso.toString();
                    var stV = $scope.viewModel.stVencimento.toString();

                    if (stA.length == 3) stA = "0" + stA;
                    if (stV.length == 3) stV = "0" + stV;

                    Api.EfetuaVenda.listPage(
                        {
                            cartao: $scope.viewModel.data.id,
                            empresa: $scope.viewModel.stEmpresa,
                            matricula: $scope.viewModel.stMatricula,
                            codAcesso: stA,
                            stVencimento: stV,
                            valor: $scope.viewModel.valor,
                            senha: $scope.viewModel.senhaPortadorCartao,
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
                            tipoWeb: 'mobile'
                        },
                        function (data) {
                            $scope.viewModel.cupom = data.results;
                            $scope.loading = false;
                            $scope.mostraModalMobileAutorizado = true;
                        },
                        function (response) {
                            $scope.falhaVendaMsg = response.data.message;
                            $scope.loading = false;
                        });
                }
            }

            $scope.closeModalMobile = function () {
                $scope.mostraModalMobile = undefined;
                $scope.loading = false;
                $scope.processandoVenda = false;
            }

            $scope.closeModalMobileAutorizado = function () {
                $scope.mostraModalMobileAutorizado = false;
                $scope.mostraModalMobileEnviaEmail = true;
                $scope.processandoVenda = false;
            }

            $scope.closeModalMobileEnviaEmail = function () {

                $scope.mostraModalMobileEnviaEmail = false;
                $scope.cancelarSimula();
            }

            $scope.confirmarMobileEnviarEmail = function ()
            {
                Api.EnvioEmail.listPage(
                {
                    cartao: $scope.viewModel.data.id,
                    email: $scope.viewModel.emailCupom
                },
                function (data) { $scope.closeModalMobileEnviaEmail(); },
                function (response) { });
            }

            // ---------------------------------------
            // controle de dinheiro
            // ---------------------------------------

            function trimStart(character, string) {
                var startIndex = 0;
                while (string[startIndex] === character) {
                    startIndex++;
                }
                return string.substr(startIndex);
            }

            function cleanup(strMoney) {
                var ret = '';
                var i = 0;
                for (; i < strMoney.length; i++) {
                    var va = strMoney[i];
                    if (va == '0') { } else {
                        break;
                        ret += va;
                    }
                }
                for (; i < strMoney.length; i++) {
                    var va = strMoney[i];
                    ret += va;
                }
                return ret;
            }

            function extractNumbers(strMoney) {
                var ret = '';
                for (var i = 0; i < strMoney.length; i++) {
                    var va = strMoney[i];
                    if (va == '0' || va == '1' || va == '2' || va == '3' || va == '4' || va == '5' || va == '6' || va == '7' || va == '8' || va == '9')
                        ret += va;
                }
                return ret;
            }

            function Valor(v)
            {
                var extract = extractNumbers(v);
                var clean = cleanup(extract);

                var resp = '';

                switch (clean.length) {
                    case 1: resp = '0,0' + clean; break;
                    case 2: resp = '0,' + clean; break;
                    case 3:
                        {
                            var p1 = clean.substr(0, 1);
                            var p2 = clean.substr(1, 2);

                            resp = p1 + ',' + p2;
                            break;
                        }

                    case 4:
                        {
                            var p1 = clean.substr(0, 2);
                            var p2 = clean.substr(2, 2);

                            resp = p1 + ',' + p2;
                            break;
                        }

                    case 5:
                        {
                            var p1 = clean.substr(0, 3);
                            var p2 = clean.substr(3, 2);

                            resp = p1 + ',' + p2;
                            break;
                        }

                    case 6:
                        {
                            var p1 = clean.substr(0, 1);
                            var p2 = clean.substr(1, 3);
                            var p3 = clean.substr(4, 2);

                            resp = p1 + '.' + p2 + ',' + p3;
                            break;
                        }

                    case 7:
                        {
                            var p1 = clean.substr(0, 2);
                            var p2 = clean.substr(2, 3);
                            var p3 = clean.substr(5, 2);

                            resp = p1 + '.' + p2 + ',' + p3;
                            break;
                        }

                    case 8:
                        {
                            var p1 = clean.substr(0, 3);
                            var p2 = clean.substr(3, 3);
                            var p3 = clean.substr(6, 2);

                            resp = p1 + '.' + p2 + ',' + p3;

                            break;
                        }

                    case 9:
                        {
                            var p1 = clean.substr(0, 1);
                            var p2 = clean.substr(1, 3);
                            var p3 = clean.substr(4, 3);
                            var p4 = clean.substr(7, 2);

                            resp = p1 + '.' + p2 + '.' + p3 + ',' + p4;
                        }

                    default: break;
                }

                // console.log('resp: ' + resp);

                return resp;
            }

            // ----------------------
            // valor
            // ----------------------

            $scope.abreModalValor = function () { $scope.modalValor = true; }
            $scope.fechaModalValor = function () { $scope.modalValor = false; }
            $scope.btnOneValor = function () { if($scope.viewModel.valor.length < 12) $scope.viewModel.valor = Valor($scope.viewModel.valor + '1'); }
            $scope.btnTwoValor = function () { if ($scope.viewModel.valor.length < 12) $scope.viewModel.valor = Valor($scope.viewModel.valor + '2'); }
            $scope.btnThreeValor = function () { if ($scope.viewModel.valor.length < 12) $scope.viewModel.valor = Valor($scope.viewModel.valor + '3'); }
            $scope.btnFourValor = function () { if ($scope.viewModel.valor.length < 12) $scope.viewModel.valor = Valor($scope.viewModel.valor + '4'); }
            $scope.btnFiveValor = function () { if ($scope.viewModel.valor.length < 12) $scope.viewModel.valor = Valor($scope.viewModel.valor + '5'); }
            $scope.btnSixValor = function () { if ($scope.viewModel.valor.length < 12) $scope.viewModel.valor = Valor($scope.viewModel.valor + '6'); }
            $scope.btnSevenValor = function () { if ($scope.viewModel.valor.length < 12) $scope.viewModel.valor = Valor($scope.viewModel.valor + '7'); }
            $scope.btnEightValor = function () { if ($scope.viewModel.valor.length < 12) $scope.viewModel.valor = Valor($scope.viewModel.valor + '8'); }
            $scope.btnNineValor = function () { if ($scope.viewModel.valor.length < 12) $scope.viewModel.valor = Valor($scope.viewModel.valor + '9'); }
            $scope.btnZeroValor = function () {
                if ($scope.viewModel.valor.length < 12) $scope.viewModel.valor = Valor($scope.viewModel.valor + '0'); if ($scope.viewModel.valor == '')
                    $scope.viewModel.valor = '0,00';}
            $scope.btnCleanValor = function () { $scope.viewModel.valor = '0,00'; }
            $scope.btnDelValor = function () {
                if ($scope.viewModel.valor != '0,00') {
                    $scope.viewModel.valor = Valor($scope.viewModel.valor.substr(0, $scope.viewModel.valor.length - 1));
                    if ($scope.viewModel.valor == '')
                        $scope.viewModel.valor = '0,00';
                }
            }

        }]);
