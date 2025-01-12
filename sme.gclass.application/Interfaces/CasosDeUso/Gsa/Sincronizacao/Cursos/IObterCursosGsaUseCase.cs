﻿using SME.GoogleClassroom.Infra;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public interface IObterCursosGsaUseCase
    {
        Task<PaginacaoResultadoDto<CursoGsaDto>> Executar(FiltroObterCursosGsaDto filtro);
    }
}
