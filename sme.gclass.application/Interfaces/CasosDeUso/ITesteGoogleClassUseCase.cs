﻿using SME.GoogleClassroom.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public interface ITesteGoogleClassUseCase
    {
        Task<bool> Executar(MensagemRabbit mensagemRabbit);
    }
}
