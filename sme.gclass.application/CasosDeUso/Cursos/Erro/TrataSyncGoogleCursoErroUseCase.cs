﻿using MediatR;
using Sentry;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class TrataSyncGoogleCursoErroUseCase : ITrataSyncGoogleCursoErroUseCase
    {
        private readonly IMediator mediator;
        private readonly bool _deveExecutarIntegracao;

        public TrataSyncGoogleCursoErroUseCase(IMediator mediator, VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _deveExecutarIntegracao = variaveisGlobaisOptions.DeveExecutarIntegracao;
        }

        public async Task<bool> Executar(MensagemRabbit mensagemRabbit)
        {
            try
            {
                var cursosErroParaTratar = await mediator.Send(new ObterCursosComErroQuery());
                if (cursosErroParaTratar != null && cursosErroParaTratar.Any())
                {
                    foreach (var cursoErroParaTratar in cursosErroParaTratar)
                    {
                        try
                        {  
                            await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaCursoErroTratar, RotasRabbit.FilaCursoErroTratar, cursoErroParaTratar));

                            await ExcluirCursoErroAsync(cursoErroParaTratar);                            
                        }
                        catch (Exception ex)
                        {
                            await mediator.Send(new InserirCursoErroCommand(cursoErroParaTratar.TurmaId, cursoErroParaTratar.ComponenteCurricularId, $"ex.: {ex.Message}", null, ExecucaoTipo.CursoAdicionar, ErroTipo.Interno));
                        }                        
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);                
            }
            return false;
        }

        private async Task ExcluirCursoErroAsync(CursoErro cursoErro)
        {
            if (!_deveExecutarIntegracao) return;
            if(!await mediator.Send(new ExcluirCursoErroCommand(cursoErro.Id)))
            {
                SentrySdk.CaptureMessage($"Não foi possível excluir o erro Id {cursoErro.Id} da turma Id {cursoErro.TurmaId} e componente curricular Id {cursoErro.ComponenteCurricularId}");
            }
        }
    }
}
