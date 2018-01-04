// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BundleConfig.cs" company="">
//   Copyright � 2014 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DevKit.Web
{
	using System.Web.Optimization;
	 
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new StyleBundle("~/bundles/css").Include(
				"~/css/bootstrap.css",
				"~/css/bootstrap-theme.css",
				"~/css/bootstrap-additions.min.css",
				"~/css/perfect-scrollbar.css",
				"~/css/application.css",
				"~/css/loading-bar.min.css",
				"~/css/toastr.css",
				"~/css/dialogs.css",
				"~/css/jquery.fileupload.css",
				"~/css/jquery.fileupload-ui.css",
				"~/css/blueimp-gallery.min.css",
				"~/css/angular-ui-tree.min.css",
				"~/css/angular-motion.min.css",
				"~/css/select.css",
				"~/css/select2.css"
				));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                 "~/scripts/jquery-2.1.0.min.js",
    "~/scripts/jquery.mousewheel.js",
    "~/scripts/jquery-ui-1.10.4.min.js",
    "~/scripts/angular.js",
    //"~/scripts/angular.min.js",
    "~/scripts/angular-resource.min.js",
    "~/scripts/i18n/angular-locale_pt-br.js",
    "~/scripts/angular-mocks.js",
    "~/scripts/xtform.js",
    "~/scripts/purl.js",
    "~/scripts/restful-ng-mock.js",
    "~/scripts/bootstrap.min.js",
    "~/scripts/ui-bootstrap-tpls-0.10.0.min.js",
    "~/scripts/angular-animate.min.js",
    "~/scripts/angular-ui-router.min.js",
    "~/scripts/angular-sanitize.min.js",
    "~/scripts/loading-bar.min.js",
    "~/scripts/spin.min.js",
    "~/scripts/angular-spinner.min.js",
    "~/scripts/toastr.min.js",
    "~/scripts/sortable.js",
    "~/scripts/keypress.js",
    "~/scripts/angular-strap.min.js",
    "~/scripts/angular-strap.tpl.min.js",
    "~/scripts/angular-draganddrop.js",
    "~/scripts/perfect-scrollbar.js",
    "~/scripts/angular-perfect-scrollbar.js",
    "~/scripts/select.min.js",
    "~/scripts/select2.min.js",
    "~/scripts/select2_locale_pt-BR.js",
    "~/scripts/angular_select2.js",
    "~/scripts/d3.min.js",
    "~/scripts/moment.js",
    "~/scripts/ng-csv.min.js",
    "~/scripts/angularjs-nvd3-directives.min.js",
    "~/scripts/angular-ui-mask.min.js",
    "~/scripts/bindonce.min.js",
    "~/scripts/load-image.min.js",
    "~/scripts/canvas-to-blob.min.js",
    "~/scripts/jquery.blueimp-gallery.min.js",
    "~/scripts/jquery.iframe-transport.js",
    "~/scripts/jquery.fileupload.js",
    "~/scripts/jquery.fileupload-process.js",
    "~/scripts/jquery.fileupload-image.js",
    "~/scripts/jquery.fileupload-validate.js",
    "~/scripts/jquery.fileupload-angular.js",
    "~/scripts/angular-ui-tree.min.js",
    "~/scripts/ng-currency.js"
 //   "~/scripts/maskMoney.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
				"~/app/_shared/filters.js",
				"~/app/services.js",
				"~/app/_shared/directives.js",
				"~/app/_shared/directives.validator.js",
				"~/app/_shared/filtro/diretivaFiltro.js",
				"~/app/_shared/filtro/diretivaNgFiltro.js",
				"~/app/_shared/filtro/diretivaFiltroParametrizacao.js",
				"~/app/_shared/filtro/diretivaNgFiltroParametrizacao.js",
				"~/app/_shared/paginacao/diretivaNgPaginacao.js",
				"~/app/_shared/paginacao/diretivaPaginacao.js",
				"~/app/_shared/controllers.js",
				"~/app/_shared/HighlightService.js",
				"~/app/login/AuthInterceptorService.js",
				"~/app/login/AuthService.js",
				"~/app/login/LoginController.js",
				"~/app/_shared/MenuController.js",
				"~/app/_shared/NgSelectsService.js",
				"~/app/app.js",
				"~/app/home/HomeController.js",
                "~/app/system/associado/ListingAssociadosController.js",
                "~/app/system/venda/VendaController.js",
                "~/app/home/CacheController.js",
                "~/app/system/loja/ListingLojasController.js",
                "~/app/system/loja/LojaController.js",
                "~/app/system/loja/LojaMensagensController.js",
                "~/app/associado/LimitesController.js",
                "~/app/associado/ExtratosController.js",
                "~/app/gestaoLogista/GLTransController.js",
                "~/app/gestaoLogista/GLDemonstrativo.js",
                "~/app/emissora/EmissoraNovoCartao.js",
                "~/app/emissora/EmissoraAltSenha.js",
                "~/app/emissora/EmissoraAltSenhaUsuario.js",
                "~/app/emissora/EmissoraAltSegVia.js",
                "~/app/emissora/EmissoraAltLimite.js",
                "~/app/emissora/EmissoraAltCota.js",
                "~/app/emissora/EmissoraAltBloq.js",
                "~/app/emissora/EmissoraAltDesbloq.js",
                "~/app/emissora/EmissoraRelatoriosController.js",
                "~/app/emissora/EmissoraListagemCartaoController.js",
                "~/app/emissora/EmissoraListagemUsuarioController.js",
                "~/app/emissora/EmissoraListagemFechamentoController.js",
                "~/app/emissora/EmissoraExpFechamentoController.js",
                "~/app/emissora/EmissoraRelExtratSaldosController.js",
                "~/app/emissora/EmissoraRelRepFinancController.js",
                "~/app/associado/LojistasAssociadoController.js",
                "~/app/system/relatorios/ListingRelatoriosController.js",
                "~/app/system/relatorios/ListingRelAssociadosController.js",
                "~/app/system/relatorios/ListingLojistaTransController.js",
                "~/app/system/cancelamento/CancelamentoController.js"

                ));

			BundleTable.EnableOptimizations = false;
		}
	}
}
