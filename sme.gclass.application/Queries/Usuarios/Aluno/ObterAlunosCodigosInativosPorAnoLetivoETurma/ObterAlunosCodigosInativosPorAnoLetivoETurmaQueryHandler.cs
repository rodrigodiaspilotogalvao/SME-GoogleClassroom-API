﻿using MediatR;
using SME.GoogleClassroom.Dados;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class ObterAlunosCodigosInativosPorAnoLetivoETurmaCelpQueryHandler : IRequestHandler<ObterAlunosCodigosInativosPorAnoLetivoETurmaCelpQuery, IEnumerable<long>>
    {
        private readonly IRepositorioAlunoEol repositorioAlunoEol;

        public ObterAlunosCodigosInativosPorAnoLetivoETurmaCelpQueryHandler(IRepositorioAlunoEol repositorioAlunoEol)
        {
            this.repositorioAlunoEol = repositorioAlunoEol ?? throw new ArgumentNullException(nameof(repositorioAlunoEol));
        }

        public async Task<IEnumerable<long>> Handle(ObterAlunosCodigosInativosPorAnoLetivoETurmaCelpQuery request, CancellationToken cancellationToken)
        {
            return await repositorioAlunoEol.ObterAlunosCodigosInativosPorAnoLetivoETurmaCelp(request.AnoLetivo, request.TurmaId);
        }
    }
}
