﻿using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Dados
{
    public interface IRepositorioAlunoEol
    {
        Task<PaginacaoResultadoDto<AlunoEol>> ObterAlunosParaInclusaoAsync(Paginacao paginacao, DateTime dataReferencia, long? codigoEol);
        Task<AlunoEol> ObterAlunoParaTratamentoDeErroAsync(long codigoEol, int anoLetivo);
        Task<IEnumerable<AlunoCursoEol>> ObterCursosDoAlunoParaIncluirAsync(long codigoAluno, int anoLetivo);
    }
}
