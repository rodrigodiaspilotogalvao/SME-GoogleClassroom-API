﻿using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao.Interfaces
{
    public interface IIniciarSyncGoogleAlunoErrosUseCase
    {
        Task<bool> Executar();
    }
}