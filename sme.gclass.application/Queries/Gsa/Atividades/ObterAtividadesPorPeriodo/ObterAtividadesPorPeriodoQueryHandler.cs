﻿using MediatR;
using SME.GoogleClassroom.Dados.Interfaces;
using SME.GoogleClassroom.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class ObterAtividadesPorPeriodoQueryHandler : IRequestHandler<ObterAtividadesPorPeriodoQuery, IEnumerable<DadosAvaliacaoDto>>
    {
        private readonly IRepositorioAtividade repositorioAtividade;

        public ObterAtividadesPorPeriodoQueryHandler(IRepositorioAtividade repositorioAtividade)
        {
            this.repositorioAtividade = repositorioAtividade ?? throw new System.ArgumentNullException(nameof(repositorioAtividade));
        }

        public async Task<IEnumerable<DadosAvaliacaoDto>> Handle(ObterAtividadesPorPeriodoQuery request, CancellationToken cancellationToken)
            => await repositorioAtividade.ObterAtividadesPorPeriodo(request.DataInicio, request.DataFim);
    }
}