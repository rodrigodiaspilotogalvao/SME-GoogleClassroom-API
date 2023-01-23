﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SME.GoogleClassroom.Infra.Dtos.Gsa;

namespace SME.GoogleClassroom.Aplicacao.Interfaces.Sga.FuncionariosProfessores
{
    public interface IFuncionariosProfessoresEolSgaUseCase
    {
        Task<ProfessoresFuncionariosSgaDto> Executar(int anoLetivo, string codigoEscola);
    }
}