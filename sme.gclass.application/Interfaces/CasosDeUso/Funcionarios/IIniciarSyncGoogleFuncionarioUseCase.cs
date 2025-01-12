﻿using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao.Interfaces
{
    public interface IIniciarSyncGoogleFuncionarioUseCase
    {
        Task<bool> Executar();
        Task<bool> Executar(long rf);
    }
}