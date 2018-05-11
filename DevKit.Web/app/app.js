'use strict';

var app = angular.module('app', ['ui.bootstrap', 'chieffancypants.loadingBar', 'ngAnimate', 'ui.router', 'angularSpinner', 'perfect_scrollbar', 'ngSanitize', 'ng-currency', 'ui.select', 'ui.select2', 'pasvaz.bindonce', 'app.filters', 'app.services', 'app.directives', 'app.controllers', 'ui.sortable', 'ui.keypress', 'ui.tree', 'ui.mask' ])
     
.config(['$stateProvider', '$locationProvider', function ($stateProvider, $locationProvider)
{
    $stateProvider

	.state('login', { url: '/login', templateUrl: 'app/login/login.html', controller: 'LoginController', data: {} })
    .state('home', { url: '/', templateUrl: 'app/home/home.html', controller: 'HomeController', data: {} })
    .state('cache', { url: '/cache', templateUrl: 'app/home/cache.html', controller: 'CacheController', data: {} })
        
    .state('associado', { url: '/system/associado', templateUrl: 'app/system/associado/listingAssociados.html', controller: 'ListingAssociadosController' })
    .state('venda', { url: '/system/venda', templateUrl: 'app/system/venda/venda.html', controller: 'VendaController' })
    .state('vendamobile', { url: '/system/vendamobile', templateUrl: 'app/system/venda/vendamobile.html', controller: 'VendaMobileController' })
    .state('cancelamento', { url: '/system/cancelamento', templateUrl: 'app/system/cancelamento/cancelamento.html', controller: 'CancelamentoController' })

    .state('relAssociados', { url: '/system/relAssociados', templateUrl: 'app/system/relatorios/listingRelAssociados.html', controller: 'ListingRelAssociadosController' })
    .state('relLojistaTrans', { url: '/system/relLojistaTrans', templateUrl: 'app/system/relatorios/listingRelLojistaTrans.html', controller: 'ListingLojistaTransController' })
        
    .state('gestaoLojistaRelTrans',
        {
            url: '/gestaoLogista/transacoes',
            templateUrl: 'app/gestaoLogista/GLTrans.html',
            controller: 'GLTransController'
        })

    .state('gestaoLojistaDemonstrativo',
        {
            url: '/gestaoLogista/demonstrativos',
            templateUrl: 'app/gestaoLogista/GLDemonstrativo.html',
            controller: 'GLDemonstrativoController'
        })
        
    .state('limitesUsr', { url: '/associado/limites', templateUrl: 'app/associado/limites.html', controller: 'LimitesController' })
    .state('limitesUsrMobile', { url: '/associado/limitesMobile', templateUrl: 'app/associado/limitesMobile.html', controller: 'LimitesMobileController' })

        .state('extratosUsr', { url: '/associado/extratos', templateUrl: 'app/associado/extratos.html', controller: 'ExtratosController' })
        .state('extratosUsrMobile', { url: '/associado/extratosMobile', templateUrl: 'app/associado/extratosMobile.html', controller: 'ExtratosMobileController' })

    .state('lojistasUsr', { url: '/associado/lojistas', templateUrl: 'app/associado/lojistas.html', controller: 'LojistasAssociadoController' })

    .state('mensagens', { url: '/system/lojasMensagens', templateUrl: 'app/system/loja/lojaMensagens.html', controller: 'LojaMensagensController' })
    .state('lojas', { url: '/system/lojas', templateUrl: 'app/system/loja/listingLojas.html', controller: 'ListingLojasController' })
    .state('loja-new', { url: '/system/loja/new', templateUrl: 'app/system/loja/loja.html', controller: 'LojaController' })
    .state('loja', { url: '/system/loja/:id', templateUrl: 'app/system/loja/loja.html', controller: 'LojaController' })

    .state('menuUsr', { url: '/system/associadoMenu', templateUrl: 'app/associado/menu.html', controller: 'AssociadoMenuController' })

    .state('empNovoCartao', { url: '/emissora/novoCartao', templateUrl: 'app/emissora/novoCartao.html', controller: 'EmissoraNovoCartaoController' })
    .state('empManutCartao', { url: '/emissora/manutCartao/:id', templateUrl: 'app/emissora/novoCartao.html', controller: 'EmissoraNovoCartaoController' })
    .state('empListagemCartao', { url: '/emissora/listagemCartao', templateUrl: 'app/emissora/listagemCartao.html', controller: 'EmissoraListagemCartaoController' })
    .state('empListagemLoja', { url: '/emissora/listagemLoja', templateUrl: 'app/emissora/listagemLoja.html', controller: 'EmissoraListagemLojaController' })
    .state('empListagemUsuario', { url: '/emissora/listagemUsuario', templateUrl: 'app/emissora/listagemUsuario.html', controller: 'EmissoraListagemUsuarioController' })
    .state('empAltSenhaUsuario', { url: '/emissora/altsenhausuario', templateUrl: 'app/emissora/altSenhaUsuario.html', controller: 'EmissoraAltSenhaUsuarioController' })
    .state('empAltSenha', { url: '/emissora/altsenha', templateUrl: 'app/emissora/altSenha.html', controller: 'EmissoraAltSenhaController' })
    .state('empAltSegVia', { url: '/emissora/altsegvia', templateUrl: 'app/emissora/altSegVia.html', controller: 'EmissoraAltSegViaController' })
    .state('empAltLim', { url: '/emissora/altlim', templateUrl: 'app/emissora/altLimite.html', controller: 'EmissoraAltLimiteController' })
    .state('empAltCota', { url: '/emissora/altCota', templateUrl: 'app/emissora/altCota.html', controller: 'EmissoraAltCotaController' })
    .state('empAltBloq', { url: '/emissora/altBloq', templateUrl: 'app/emissora/altBloq.html', controller: 'EmissoraAltBloqueioController' })
    .state('empAltDesbloq', { url: '/emissora/altDesbloq', templateUrl: 'app/emissora/altDesbloq.html', controller: 'EmissoraAltDesbloqueioController' })
    .state('empListagemFechamento', { url: '/emissora/listagemFechamento', templateUrl: 'app/emissora/listagemFechamento.html', controller: 'EmissoraListagemFechamentoController' })
    .state('empExpFechamento', { url: '/emissora/expFechamento', templateUrl: 'app/emissora/expFechamento.html', controller: 'EmissoraExpFechamentoController' })
    .state('empRelatorios', { url: '/emissora/relatorios', templateUrl: 'app/emissora/relatorios.html', controller: 'EmissoraRelatoriosController' })
    .state('empRelRepFinanc', { url: '/emissora/repFinanc', templateUrl: 'app/emissora/relRepFinanc.html', controller: 'EmissoraRelRepFinancController' })
    .state('empRelExtratSaldos', { url: '/emissora/relExtratSaldos', templateUrl: 'app/emissora/relExtratSaldos.html', controller: 'EmissoraRelExtratSaldosController' })
    .state('empRelExtratForn', { url: '/emissora/relExtratForn', templateUrl: 'app/emissora/relExtratForn.html', controller: 'EmissoraRelExtratFornController' })
    .state('empRelExtratTrans', { url: '/emissora/relExtratTrans', templateUrl: 'app/emissora/relExtratTrans.html', controller: 'EmissoraRelExtratTransController' })

    .state('otherwise', { url: '*path', templateUrl: 'app/_shared/404.html', controller: 'Erro404Controller' });

    $locationProvider.html5Mode(true);

}]);
 
angular.module('app').config(function ($httpProvider) {

    $httpProvider.defaults.useXDomain = true;

	$httpProvider.interceptors.push('AuthInterceptorService');

	if (!$httpProvider.defaults.headers.get) {
		$httpProvider.defaults.headers.get = {};
	}

	$httpProvider.defaults.headers.get['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
	$httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache no-store';
	$httpProvider.defaults.headers.get['Pragma'] = 'no-cache no-store';
});
