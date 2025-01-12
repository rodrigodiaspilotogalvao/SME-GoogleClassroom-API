﻿using MediatR;
using Newtonsoft.Json;
using Sentry;
using SME.GoogleClassroom.Aplicacao.Interfaces;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class RealizarTratamentoAlunoErroUseCase : IRealizarTratamentoAlunoErroUseCase
    {
        private readonly IMediator mediator;

        public RealizarTratamentoAlunoErroUseCase(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<bool> Executar(MensagemRabbit mensagemRabbit)
        {
            var filtro = JsonConvert.DeserializeObject<FiltroAlunoErroDto>(mensagemRabbit.Mensagem.ToString());
            var usuarioErro = filtro.UsuarioErro;
            if (usuarioErro is null) return true;

            try
            {
                if (usuarioErro.UsuarioTipo != UsuarioTipo.Aluno)
                    return false;
                
                var parametrosCargaInicialDto = filtro.AnoLetivo.HasValue ? new ParametrosCargaInicialDto(filtro.TiposUes, filtro.Ues, filtro.Turmas, filtro.AnoLetivo) :
                    await mediator.Send(new ObterParametrosCargaIncialPorAnoQuery(DateTime.Today.Year));
                var alunoEol = await mediator.Send(new ObterAlunoParaTratamentoDeErroQuery(usuarioErro.UsuarioId.GetValueOrDefault(), parametrosCargaInicialDto));
                if (alunoEol is null)
                    return false;

                var filtroAluno = new FiltroAlunoDto(alunoEol, filtro.AnoLetivo, filtro.TiposUes, filtro.Ues, filtro.Turmas);
                var publicarFuncionario = await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaAlunoIncluir, RotasRabbit.FilaAlunoIncluir, filtroAluno));
                if (!publicarFuncionario)
                {
                    var mensagem = $"Não foi possível inserir o aluno RA{usuarioErro.UsuarioId} na fila de inclusão.";
                    await mediator.Send(new IncluirUsuarioErroCommand(usuarioErro.UsuarioId, usuarioErro.Email, mensagem, usuarioErro.UsuarioTipo, usuarioErro.ExecucaoTipo));
                }
            }
            catch (Exception ex)
            {
                await mediator.Send(new IncluirUsuarioErroCommand(usuarioErro.UsuarioId, usuarioErro.Email, ex.InnerException?.Message ?? ex.Message, usuarioErro.UsuarioTipo, usuarioErro.ExecucaoTipo));
            }

            return false;
        }
    }
}