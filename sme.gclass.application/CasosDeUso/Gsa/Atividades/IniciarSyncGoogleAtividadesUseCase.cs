﻿using MediatR;
using SME.GoogleClassroom.Infra;
using System;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class IniciarSyncGoogleAtividadesUseCase : IIniciarSyncGoogleAtividadesUseCase
    {
        private readonly IMediator mediator;

        public IniciarSyncGoogleAtividadesUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(long? cursoId = null, int? pagina = null, int? totalPaginas = null)
        {
            var filtro = new FiltroCargaAtividadesCursoDto(cursoId, pagina, totalPaginas);
            await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaGsaAtividadesCarregar, filtro));
        }
    }
}
