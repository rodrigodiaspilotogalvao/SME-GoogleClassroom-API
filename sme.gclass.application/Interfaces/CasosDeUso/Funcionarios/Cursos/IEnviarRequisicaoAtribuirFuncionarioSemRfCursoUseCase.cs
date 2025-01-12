﻿using SME.GoogleClassroom.Infra;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public interface IEnviarRequisicaoAtribuirFuncionarioSemRfCursoUseCase
    {
        Task<bool> Executar(AtribuirFuncionarioSemRfCursoDto atribuirFuncionarioSemRfCurso);
    }
}