﻿'use strict';

angular.module('app.services', ['ngResource'])

.service('$confirmacao', ['$modal', '$rootScope', '$q', function ($modal, $rootScope, $q) {

	var deferred;
	var scope = $rootScope.$new();

	scope.resposta = function (res) {
		deferred.resolve(res);
		confirmacao.hide();
	}

	var confirmacao = $modal({ template: 'app/_shared/templateConfirmacao.html', scope: scope, show: false });
	var parentShow = confirmacao.show;

	confirmacao.exibir = function (titulo, mensagem) {
		scope.titulo = titulo;
		scope.mensagem = mensagem;
		deferred = $q.defer();
		parentShow();
		return deferred.promise;
	}

	return confirmacao;
}])

.factory('Api', ['$resource', function ($resource)
{
	var opcoes = {
		'add': { method: 'POST' },
		'list': { method: 'GET', isArray: true },
		'listPage': { method: 'GET', isArray: false },
		'get': { method: 'GET', isArray: false },
		'update': { method: 'PUT' },
		'remove': { method: 'DELETE' }
	};

    return {

        EmissorCancelamento: $resource('api/emissorcancelamento/:id', {}, opcoes),
        Setup: $resource('api/setup/:id', {}, opcoes),
        User: $resource('api/user/:id', {}, opcoes),
        Empresa: $resource('api/empresa/:id', {}, opcoes),
        Credenciado: $resource('api/credenciado/:id', {}, opcoes),
        Especialidade: $resource('api/especialidade/:id', {}, opcoes),
        TUSS: $resource('api/tuss/:id', {}, opcoes),
        ConfigSenha: $resource('api/configsenha/:id', {}, opcoes),
        EmissorFechamentoEdicaoGuia: $resource('api/emissorfechamentoedicaoguia/:id', {}, opcoes),
        EmissorFechamentoOper: $resource('api/emissorfechamentooper/:id', {}, opcoes),
		Profile: $resource('api/profile/:id', {}, opcoes),
        Permission: $resource('api/permission/:id', {}, opcoes),
		Project: $resource('api/project/:id', {}, opcoes),
		Phase: $resource('api/phase/:id', {}, opcoes),
		Version: $resource('api/version/:id', {}, opcoes),
		Sprint: $resource('api/sprint/:id', {}, opcoes),
		TaskType: $resource('api/taskType/:id', {}, opcoes),
		TaskCategory: $resource('api/taskCategory/:id', {}, opcoes),
		TaskFlow: $resource('api/taskFlow/:id', {}, opcoes),
		TaskCount: $resource('api/taskCount/:id', {}, opcoes),
		TaskTypeAccumulator: $resource('api/taskAccumulator/:id', {}, opcoes),
		TaskCheckPoint: $resource('api/taskCheckPoint/:id', {}, opcoes),
		Task: $resource('api/task/:id', {}, opcoes),
		Priority: $resource('api/priority/:id', {}, opcoes),
		UserKanban: $resource('api/userKanbanView/:id', {}, opcoes),
		Management: $resource('api/managementView/:id', {}, opcoes),
		AccType: $resource('api/accumulatorType/:id', {}, opcoes),        
		VersionState: $resource('api/versionState/:id', {}, opcoes),
		ProjectTemplate: $resource('api/projectTemplate/:id', {}, opcoes),
		Timesheet: $resource('api/timesheetView/:id', {}, opcoes),
		News: $resource('api/news/:id', {}, opcoes),
        Survey: $resource('api/survey/:id', {}, opcoes),
        Associado: $resource('api/associado/:id', {}, opcoes),
        LotesGrafica: $resource('api/lotesgrafica/:id', {}, opcoes),
        Cartao: $resource('api/cartao/:id', {}, opcoes),
        Cache: $resource('api/cache/:id', {}, opcoes),
        HomeView: $resource('api/homeView/:id', {}, opcoes),
        MonthCombo: $resource('api/month/:id', {}, opcoes),
        DayMonthCombo: $resource('api/daymonth/:id', {}, opcoes),		
        EstadoCombo: $resource('api/estadocombo/:id', {}, opcoes),
        TipoSituacaoAutorizacaoCombo: $resource('api/tiposituacaoautorizacaocombo/:id', {}, opcoes),
        CidadeCombo: $resource('api/cidadecombo/:id', {}, opcoes),
        ClientCombo: $resource('api/clientcombo/:id', {}, opcoes),
        ClientGroupCombo: $resource('api/clientgroupcombo/:id', {}, opcoes),
        ProfileCombo: $resource('api/profilecombo/:id', {}, opcoes),
        ProjectCombo: $resource('api/projectcombo/:id', {}, opcoes),
        TaskTypeCombo: $resource('api/tasktypecombo/:id', {}, opcoes),
        UserCombo: $resource('api/usercombo/:id', {}, opcoes),
        PhaseCombo: $resource('api/phasecombo/:id', {}, opcoes),
        TaskCategoryCombo: $resource('api/taskcategorycombo/:id', {}, opcoes),
        TaskFlowCombo: $resource('api/taskflowcombo/:id', {}, opcoes),
        SprintCombo: $resource('api/sprintcombo/:id', {}, opcoes),
        VersionCombo: $resource('api/versioncombo/:id', {}, opcoes),
        EmpresaCombo: $resource('api/empresacombo/:id', {}, opcoes),
        AutorizaProc: $resource('api/autorizaproc/:id', {}, opcoes),
        EmpresaSecaoCombo: $resource('api/empresasecaocombo/:id', {}, opcoes),
        CredenciadoListagemAutorizacao: $resource('api/credenciadolistagemautorizacao/:id', {}, opcoes),
        EmissorLancaDespesa: $resource('api/emissorlancadespesa/:id', {}, opcoes),
        EmissorListagemAutorizacao: $resource('api/emissorlistagemautorizacao/:id', {}, opcoes),
        EmissorListagemCredenciado: $resource('api/emissorlistagemcredenciado/:id', {}, opcoes),
        EmissorListagemFechamento: $resource('api/emissorlistagemfechamento/:id', {}, opcoes),
        LotesExpAutorizacao: $resource('api/lotesexpautorizacao/:id', {}, opcoes),
        TipoSituacaoCombo: $resource('api/tiposituacao/:id', {}, opcoes),
        TipoExpedicaoCombo: $resource('api/tipoexpedicao/:id', {}, opcoes),
        TipoAutorizacaoCombo: $resource('api/tipoautorizacaocombo/:id', {}, opcoes),
        TipoCoberturaDependenteCombo: $resource('api/tipocoberturadependentecombo/:id', {}, opcoes),
        PrecoDiaria: $resource('api/precodiaria/:id', {}, opcoes),
        PrecoMaterial: $resource('api/precomaterial/:id', {}, opcoes),
        PrecoMedicamento: $resource('api/precomedicamento/:id', {}, opcoes),
        PrecoNaoMedico: $resource('api/preconaomedico/:id', {}, opcoes),
        PrecoOPME: $resource('api/precoopme/:id', {}, opcoes),
        PrecoPacote: $resource('api/precopacote/:id', {}, opcoes),
        PrecoProcedimento: $resource('api/precoprocedimento/:id', {}, opcoes),
	};
}]);
