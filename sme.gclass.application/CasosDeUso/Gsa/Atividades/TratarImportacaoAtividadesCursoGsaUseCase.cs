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
    public class TratarImportacaoAtividadesCursoGsaUseCase : ITratarImportacaoAtividadesCursoGsaUseCase
    {
        private readonly IMediator mediator;

        public TratarImportacaoAtividadesCursoGsaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(MensagemRabbit mensagem)
        {
            if (mensagem?.Mensagem is null)
                throw new NegocioException("Não foi possível gerar a carga de dados para a atualização de mural de avisos GSA.");

            var filtro = mensagem.ObterObjetoMensagem<FiltroTratarAtividadesCursoDto>();

            await EnviarParaTratamento(filtro, filtro.Curso);

            return true;
        }

        private async Task EnviarParaTratamento(FiltroTratarAtividadesCursoDto filtro, CursoGsaId curso)
        {
            var paginaAtividades = await mediator.Send(new ObterAtividadesDoCursoGoogleQuery(curso, filtro.TokenProximaPagina));

            if (paginaAtividades.Atividades.Any())
                await mediator.Send(new TratarImportacaoAtividadesCommand(paginaAtividades.Atividades, Convert.ToInt64(curso.CursoId), filtro.UltimaExecucao));

            filtro.TokenProximaPagina = paginaAtividades.TokenProximaPagina;

            if (!string.IsNullOrEmpty(filtro.TokenProximaPagina))
                await PublicaProximaPaginaAsync(filtro);
        }

        private async Task PublicaProximaPaginaAsync(FiltroTratarAtividadesCursoDto filtro)
        {
            try
            {
                var syncAtividades = await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaGsaAtividadesTratar, filtro));
                if (!syncAtividades)
                    throw new Exception("Não foi possível sincronizar os atividades avaliativas GSA.");
            }
            catch (Exception ex)
            {
                await LogarErro(ex);
            }
        }

        private Task LogarErro(Exception ex)
        {
            return mediator.Send(new SalvarLogViaRabbitCommand($"Erro ao publicar consulta de proxima pagina to tratamento de Atividades", LogNivel.Critico, LogContexto.Atividades, ex.Message, rastreamento: ex.StackTrace));
        }
    }
}