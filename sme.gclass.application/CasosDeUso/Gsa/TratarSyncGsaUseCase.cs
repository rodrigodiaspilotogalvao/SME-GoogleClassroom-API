﻿using MediatR;
using SME.GoogleClassroom.Aplicacao.Interfaces;
using SME.GoogleClassroom.Infra;
using System;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class TratarSyncGsaUseCase : ITratarSyncGsaUseCase
    {
        private readonly IMediator mediator;

        public TratarSyncGsaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(MensagemRabbit mensagemRabbit)
        {
            var filtro = new FiltroCagaCursosGsaDto();
            await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaGsaCursoCarregar, RotasRabbit.FilaGsaCursoCarregar, filtro));
            return true;
        }
    }
}
