﻿using MediatR;
using SME.GoogleClassroom.Aplicacao.Interfaces;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class SincronizarArquivamentoCursosExtintosUseCase : AbstractUseCase, ISincronizarArquivamentoCursosExtintosUseCase
    {
        public SincronizarArquivamentoCursosExtintosUseCase(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit mensagem)
        {
            if (mensagem?.Mensagem is null)
                throw new NegocioException("Não foi possível sincronizar a exinção de turmas. Mensagem não recebida");

            var cursoDto = mensagem.ObterObjetoMensagem<ArquivarCursoExtintoDto>();

            try
            {
                await IncluirCursoArquivado(cursoDto.CursoId, DateTime.Now, cursoDto.DataExtincao, cursoDto.Excluir);

                await SincronizarArquivamentoCurso(cursoDto.CursoId, cursoDto.Excluir);
            }
            catch (System.Exception ex)
            {
                await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaCursoExtintoArquivarSyncErro, cursoDto));
            }

            return true;
        }

        private async Task SincronizarArquivamentoCurso(long cursoId, bool excluir)
        {
            if (excluir)
                await mediator.Send(new ExcluirCursoGoogleCommand(cursoId));
            else
                await mediator.Send(new ArquivarCursoGoogleCommand(cursoId));
        }

        private async Task IncluirCursoArquivado(long cursoId, DateTime dataArquivamento, DateTime dataExtincao, bool excluir)
        {
            await mediator.Send(new InserirCursoArquivadoCommand(cursoId, dataArquivamento, dataExtincao, excluir));
        }
    }
}