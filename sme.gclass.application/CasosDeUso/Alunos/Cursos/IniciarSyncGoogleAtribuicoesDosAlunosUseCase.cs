﻿using MediatR;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class IniciarSyncGoogleAtribuicoesDosAlunosUseCase : IIniciarSyncGoogleAtribuicoesDosAlunosUseCase
    {
        private readonly IMediator mediator;

        public IniciarSyncGoogleAtribuicoesDosAlunosUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar()
        {
            var publicarSyncAtribuicao = await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaAlunoCursoSync, RotasRabbit.FilaAlunoCursoSync, true));
            if (!publicarSyncAtribuicao)
            {
                throw new NegocioException("Não foi possível iniciar a sincronização de atribuições de cursos dos alunos.");
            }

            return publicarSyncAtribuicao;
        }
    }
}
