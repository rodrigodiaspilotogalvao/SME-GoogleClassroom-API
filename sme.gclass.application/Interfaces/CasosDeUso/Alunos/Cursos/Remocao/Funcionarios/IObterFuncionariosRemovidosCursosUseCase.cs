﻿using SME.GoogleClassroom.Infra;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao.Interfaces
{
    public interface IObterFuncionariosRemovidosCursosUseCase
    {
        Task<PaginacaoResultadoDto<CursoUsuarioRemovidoConsultaDto>> Executar(FiltroObterUsuariosRemovidosCursosDto filtro);
    }
}
