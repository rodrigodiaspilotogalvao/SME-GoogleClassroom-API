﻿using MediatR;
using SME.GoogleClassroom.Dados;
using SME.GoogleClassroom.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class ObterAlunosCodigosComRegistroEmCursosQueryHandler : IRequestHandler<ObterAlunosCodigosComRegistroEmCursosQuery, IEnumerable<CursoUsuarioRemoverDto>>
    {
        private readonly IRepositorioUsuarioCursoGsa repositorioUsuarioCursoGsa;

        public ObterAlunosCodigosComRegistroEmCursosQueryHandler(IRepositorioUsuarioCursoGsa repositorioUsuarioCursoGsa)
        {
            this.repositorioUsuarioCursoGsa = repositorioUsuarioCursoGsa ?? throw new ArgumentNullException(nameof(repositorioUsuarioCursoGsa));
        }

        public async Task<IEnumerable<CursoUsuarioRemoverDto>> Handle(ObterAlunosCodigosComRegistroEmCursosQuery request, CancellationToken cancellationToken)
        {
            return await repositorioUsuarioCursoGsa.ObterAlunosCodigosComRegistroEmCurso(request.AlunosCodigos, request.TurmaId);
        }
    }
}
