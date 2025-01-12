﻿using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Dados
{
    public interface IRepositorioUsuarioCursoGsa
    {
        Task<bool> ExistePorUsuarioIdCursoIdAsync(string usuarioId, string cursoId);

        Task<bool> SalvarAsync(UsuarioCursoGsa usuarioCursoGsa);

        Task<bool> RemoverAsync(UsuarioCursoGsa usuarioCursoGsa);

        Task<ConsultaCursosDoUsuarioGsa> ObterCursosDoUsuarioGsaAsync(string usuarioId);

        Task LimparAsync();

        Task<IEnumerable<CursoUsuarioRemoverDto>> ObterAlunosCodigosComRegistroEmCurso(IEnumerable<long> alunosCodigos, long turmaId);
    }
}