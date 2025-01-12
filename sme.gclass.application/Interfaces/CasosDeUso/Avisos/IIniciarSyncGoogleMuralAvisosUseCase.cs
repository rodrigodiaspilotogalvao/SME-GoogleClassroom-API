﻿using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao.Interfaces
{
    public interface IIniciarSyncGoogleMuralAvisosUseCase
    {
        Task Executar(long? cursoId = null, int? pagina = null, int? totalPaginas = null);
    }
}
