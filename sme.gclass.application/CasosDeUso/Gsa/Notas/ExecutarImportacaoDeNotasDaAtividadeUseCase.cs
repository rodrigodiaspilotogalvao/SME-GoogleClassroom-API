﻿using MediatR;
using Sentry;
using SME.GoogleClassroom.Aplicacao.Interfaces;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class ExecutarImportacaoDeNotasDaAtividadeUseCase : AbstractUseCase, IExecutarImportacaoDeNotasDaAtividadeUseCase
    {
        public ExecutarImportacaoDeNotasDaAtividadeUseCase(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit mensagem)
        {
            var importacaoDto = mensagem.ObterObjetoMensagem<SincronizarImportacaoNotasDto>();
            try
            {
                var usuario = await mediator.Send(new ObterUsuarioPorClassroomIdQuery(importacaoDto.Nota.UsuarioId));
                var status = Enum.Parse<StatusGSA>(importacaoDto.Nota.StatusNota);
                await mediator.Send(new GravarNotaGsaCommand(importacaoDto.Nota.Id,
                                                             importacaoDto.DadosAvaliacao.Id,
                                                             importacaoDto.Nota.UsuarioId,
                                                             importacaoDto.Nota.Nota,
                                                             status,
                                                             importacaoDto.Nota.DataInclusao,
                                                             importacaoDto.Nota.DataAlteracao));

                if (usuario == null)
                    return true;

                var notaFinal = TratarNotaAluno(importacaoDto.DadosAvaliacao, importacaoDto.Nota);
                var notaSgpDto = new NotaSgpDto(importacaoDto.DadosAvaliacao.TurmaId,
                                                importacaoDto.DadosAvaliacao.ComponenteCurricularId,
                                                importacaoDto.DadosAvaliacao.Id,
                                                status,
                                                notaFinal,
                                                importacaoDto.DadosAvaliacao.DataInclusao,
                                                importacaoDto.DadosAvaliacao.DataEntrega,
                                                usuario.Id,
                                                importacaoDto.DadosAvaliacao.Titulo);

                return await EnviaParaSGP(notaSgpDto);
            }
            catch(Exception)
            {
                await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaGsaNotasAtividadesSyncErro, importacaoDto));
                return false;
            }
        }

        private async Task<bool> EnviaParaSGP(NotaSgpDto notaSgpDto)
            => await mediator.Send(new PublicaFilaRabbitSgpCommand(RotasRabbitSgp.RotaAtividadesNotasSync, notaSgpDto));

        private double? TratarNotaAluno(DadosAvaliacaoDto dadosAvaliacao, NotaGsaDto nota)
        {
            var notaMaxima = dadosAvaliacao.NotaMaxima ?? 100;

            return nota.Nota.HasValue ?
                (double?)Math.Round(nota.Nota.Value / notaMaxima * 10) :
                null;
        }
    }
}
