// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BundleConfig.cs" company="">
//   Copyright © 2014 
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
				"~/css/application.min.css",
				"~/css/loading-bar.min.css",
				"~/css/toastr.min.css",
				"~/css/dialogs.css",
				"~/css/jquery.fileupload.css",
				"~/css/jquery.fileupload-ui.css",
				"~/css/angular-ui-tree.min.css",
				"~/css/angular-motion.min.css",
				"~/css/select.css",
				"~/css/select2.css"
				));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                 "~/scripts/jquery-2.1.0.min.js",
    "~/scripts/jquery.mousewheel.js",
    "~/scripts/jquery-ui-1.10.4.min.js",
    "~/scripts/angular.min.js",
    "~/scripts/angular-resource.min.js",
    "~/scripts/i18n/angular-locale_pt-br.js",
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
    "~/scripts/angularjs-nvd3-directives.min.js",
    "~/scripts/angular-ui-mask.min.js",
    "~/scripts/bindonce.min.js",
    "~/scripts/load-image.min.js",
    "~/scripts/canvas-to-blob.min.js",
    "~/scripts/jquery.iframe-transport.js",
    "~/scripts/jquery.fileupload.js",
    "~/scripts/jquery.fileupload-process.js",
    "~/scripts/jquery.fileupload-image.js",
    "~/scripts/jquery.fileupload-validate.js",
    "~/scripts/jquery.fileupload-angular.js",
    "~/scripts/angular-ui-tree.min.js"
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
				"~/app/system/user/UserController.js",
				"~/app/system/user/UserPasswordController.js",
				"~/app/system/user/ListingUsersController.js",
				"~/app/system/profile/ProfileController.js",
				"~/app/system/profile/ListingProfilesController.js",
                "~/app/dba/listingLotes.js",
                "~/app/dba/listingEmpresas.js",
                "~/app/dba/listingExpAutorizacao.js",
                "~/app/dba/listingTUSS.js",
                "~/app/dba/listingEspecialidades.js",
                "~/app/dba/listingCredenciados.js",
                "~/app/dba/novoLote.js",
                "~/app/dba/credenciado.js",
                "~/app/dba/empresa.js",
                "~/app/emissora/listagemAssociados.js",
                "~/app/emissora/associado.js",
                "~/app/emissora/altSenha.js",
                "~/app/emissora/segVia.js",
                "~/app/emissora/bloqueio.js",
                "~/app/emissora/desbloqueio.js",
                "~/app/emissora/precoDiaria.js",
                "~/app/emissora/precoMaterial.js",
                "~/app/emissora/precoMedicamento.js",
                "~/app/emissora/precoNaoMedico.js",
                "~/app/emissora/listagemEmissorFechamento.js",
                "~/app/emissora/listagemEmissorProcedimentos.js",
                "~/app/emissora/listagemCredenciados.js",
                "~/app/emissora/credenciado.js",
                "~/app/emissora/AutorizacaoProcController.js",
                "~/app/credenciado/CredenciadoPasswordController.js",
                "~/app/credenciado/ListagemCredenciadoProcedimentosController.js",
                "~/app/credenciado/AutorizacaoProcController.js",
                "~/app/credenciado/ListagemCredFechamento.js",
                "~/app/configuration/project/ProjectController.js",
				"~/app/configuration/project/ListingProjectsController.js",
				"~/app/configuration/sprint/SprintController.js",
				"~/app/configuration/sprint/ListingSprintsController.js",
				"~/app/configuration/taskType/TaskTypeController.js",
				"~/app/configuration/taskType/ListingTaskTypesController.js",                
                "~/app/task/task/TaskController.js",
				"~/app/task/task/ListingTasksController.js",
				"~/app/task/kanban/ListingUserKanbanController.js",
				"~/app/task/management/ManagementController.js",
				"~/app/task/timesheet/TimesheetController.js",
				"~/app/configuration/news/NewsController.js",
				"~/app/configuration/news/ListingNewsController.js",
				"~/app/configuration/surveys/SurveyController.js",
				"~/app/configuration/surveys/ListingSurveysController.js",
				"~/app/home/HomeController.js"

                ));

			BundleTable.EnableOptimizations = false;
		}
	}
}
