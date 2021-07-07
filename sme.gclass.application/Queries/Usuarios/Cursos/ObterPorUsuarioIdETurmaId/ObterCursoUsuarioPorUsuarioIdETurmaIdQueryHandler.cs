﻿using MediatR;
using SME.GoogleClassroom.Dados;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao.Queries.Usuarios.Cursos.ObterPorUsuarioIdETurmaId
{
    public class ObterCursoUsuarioPorUsuarioIdETurmaIdQueryHandler : IRequestHandler<ObterCursoUsuarioPorUsuarioIdETurmaIdQuery, IEnumerable<CursoUsuarioRemoverDto>>
    {
        private readonly IRepositorioCursoUsuario repositorioCursoUsuario;

        public ObterCursoUsuarioPorUsuarioIdETurmaIdQueryHandler(IRepositorioCursoUsuario repositorioCursoUsuario)
        {
            this.repositorioCursoUsuario = repositorioCursoUsuario ?? throw new ArgumentNullException(nameof(repositorioCursoUsuario));
        }

        public async Task<IEnumerable<CursoUsuarioRemoverDto>> Handle(ObterCursoUsuarioPorUsuarioIdETurmaIdQuery request, CancellationToken cancellationToken)
        {
            return await repositorioCursoUsuario.ObterPorUsuarioIdETurmaId(request.UsuarioId, request.TurmaId);
        }
    }
}
