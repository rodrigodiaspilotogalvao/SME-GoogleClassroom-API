﻿using MediatR;
using Sentry;
using SME.GoogleClassroom.Aplicacao.Interfaces;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class TratarImportacaoMuralAvisosCursoGsaUseCase : ITratarImportacaoMuralAvisosCursoGsaUseCase
    {
        private readonly IMediator mediator;

        public TratarImportacaoMuralAvisosCursoGsaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(MensagemRabbit mensagem)
        {
            if (mensagem?.Mensagem is null)
                throw new NegocioException("Não foi possível gerar a carga de dados para a atualização de mural de avisos GSA.");

            var filtro = mensagem.ObterObjetoMensagem<FiltroTratarMuralAvisosCursoDto>();

            foreach (var curso in filtro.Cursos)
                await EnviarParaTratamento(filtro, curso);

            return true;
        }

        private async Task EnviarParaTratamento(FiltroTratarMuralAvisosCursoDto filtro, CursoResponsavelDto curso)
        {
            var paginaMural = await mediator.Send(new ObterMuralAvisosDoCursoGoogleQuery(curso, filtro.TokenProximaPagina));

            if (paginaMural.Avisos.Any())
                await mediator.Send(new TratarImportacaoAvisosCommand(paginaMural.Avisos, curso.CursoId, filtro.UltimaExecucao));

            filtro.TokenProximaPagina = paginaMural.TokenProximaPagina;
            filtro.Cursos = new CursoResponsavelDto[] { curso };

            if (!string.IsNullOrEmpty(filtro.TokenProximaPagina))
                await PublicaProximaPaginaAsync(filtro);
        }

        private async Task PublicaProximaPaginaAsync(FiltroTratarMuralAvisosCursoDto filtro)
        {
            try
            {
                var syncCursoComparativo = await mediator
                    .Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaGsaMuralAvisosTratar, filtro));

                if (!syncCursoComparativo)
                    SentrySdk.CaptureMessage("Não foi possível sincronizar os cursos do usuário GSA.");
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }
    }
}
