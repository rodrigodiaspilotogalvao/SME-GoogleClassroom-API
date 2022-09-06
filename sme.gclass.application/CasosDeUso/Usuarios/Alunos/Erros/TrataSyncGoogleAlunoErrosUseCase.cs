﻿using MediatR;
using Sentry;
using SME.GoogleClassroom.Aplicacao.Interfaces;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class TrataSyncGoogleAlunoErrosUseCase : ITrataSyncGoogleAlunoErrosUseCase
    {
        private readonly IMediator mediator;
        private readonly bool _deveExecutarIntegracao;

        public TrataSyncGoogleAlunoErrosUseCase(IMediator mediator, VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.mediator = mediator;
            _deveExecutarIntegracao = variaveisGlobaisOptions.DeveExecutarIntegracao;
        }

        public async Task<bool> Executar(MensagemRabbit mensagemRabbit)
        {
            try
            {
                var filtro = ObterFiltro(mensagemRabbit);
                var usuarioErros = await mediator.Send(new ObtemUsuariosErrosPorTipoQuery(UsuarioTipo.Aluno));
                if (!usuarioErros?.Any() ?? true) return true;

                foreach (var usuarioErro in usuarioErros)
                {
                    var filtroAluno = new FiltroAlunoErroDto(usuarioErro, filtro.AnoLetivo, filtro.TiposUes, filtro.Ues, filtro.Turmas);
                    
                    await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaAlunoErroTratar, RotasRabbit.FilaAlunoErroTratar, filtroAluno));

                    await ExcluirUsuarioErroAsync(usuarioErro);
                }

                return true;
            }
            catch (Exception ex)
            {
                await mediator.Send(new SalvarLogViaRabbitCommand($"TrataSyncGoogleAlunoErrosUseCase", LogNivel.Critico, LogContexto.Gsa, ex.Message, ex.StackTrace));
            }

            return false;
        }

        private async Task ExcluirUsuarioErroAsync(UsuarioErro usuarioErro)
        {
            await mediator.Send(new ExcluirUsuarioErroCommand(usuarioErro.Id));
        }

        private FiltroCargaInicialDto ObterFiltro(MensagemRabbit mensagem)
        {
            try
            {
                return mensagem.ObterObjetoMensagem<FiltroCargaInicialDto>();
            }
            catch { return null; }
        }
    }
}