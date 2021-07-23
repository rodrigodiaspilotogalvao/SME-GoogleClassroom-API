﻿using MediatR;
using Sentry;
using SME.GoogleClassroom.Infra;
using System;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class IniciarProcessoRemoverProfessorCursoGsaUseCase : IIniciarProcessoRemoverProfessorCursoGsaUseCase
    {
        private readonly IMediator mediator;

        public IniciarProcessoRemoverProfessorCursoGsaUseCase(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<bool> Executar(long? turmaId = null)
        {
            try
            {
                var dto = new CarregarProfessoresCursoRemoverDto(turmaId);
                return await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaGsaCursoProfessorRemovidoTurmasCarregar, dto));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return false;
            }
        }
    }
}