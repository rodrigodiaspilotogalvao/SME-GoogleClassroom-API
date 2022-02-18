﻿using MediatR;
using Newtonsoft.Json;
using SME.GoogleClassroom.Infra;
using SME.GoogleClassroom.Infra.Enumeradores;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class SincronizacaoGsaFormacaoCidadeTurmaComponenteUseCase : ISincronizacaoGsaFormacaoCidadeTurmaComponenteUseCase
    {
        private readonly IMediator mediator;

        public SincronizacaoGsaFormacaoCidadeTurmaComponenteUseCase(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<bool> Executar(MensagemRabbit mensagemRabbit)
        {
            var filtro = JsonConvert.DeserializeObject<FiltroFormacaoCidadeTurmaComponenteDto>(mensagemRabbit.Mensagem.ToString());

            try
            {
                foreach (var salaVirtual in filtro.SalasVirtuais)
                {
                    await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaGsaFormacaoCidadeTurmasTratarCurso,
                    new FiltroFormacaoCidadeTurmaCursoDto($"{filtro.SalaVirtual} - {salaVirtual.SalaVirtual}",
                                                          filtro.CodigoDre,
                                                          filtro.AnoLetivo,
                                                          salaVirtual.ComponentesCurricularIds,
                                                          salaVirtual.ModalidadesIds,
                                                          salaVirtual.TipoEscola,
                                                          salaVirtual.TipoConsulta,
                                                          salaVirtual.AnoTurma,
                                                          salaVirtual.IncluirAlunoCurso)));
                }
                   
            }
            catch (Exception)
            {
                await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaGsaFormacaoCidadeTurmasTratarComponenteErro, filtro));
            }

            return true;
        }
    }
}
