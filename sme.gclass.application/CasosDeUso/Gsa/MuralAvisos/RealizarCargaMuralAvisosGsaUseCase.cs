﻿using MediatR;
using Sentry;
using SME.GoogleClassroom.Aplicacao.Interfaces;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class RealizarCargaMuralAvisosGsaUseCase : IRealizarCargaMuralAvisosGsaUseCase
    {
        private readonly IMediator mediator;

        public RealizarCargaMuralAvisosGsaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(MensagemRabbit mensagem)
        {
            var ultimaExecucao = await mediator
                .Send(new ObterDataUltimaExecucaoPorTipoQuery(ExecucaoTipo.MuralAvisosCarregar));

            var anoAtual = DateTime.Now.Year;

            var filtro = mensagem
                .ObterObjetoMensagem<FiltroCargaMuralAvisosCursoDto>();

            if (!filtro.CursoId.HasValue && ultimaExecucao.Date.Equals(DateTime.Today.Date))
                return true;

            filtro.Pagina = filtro.Pagina ?? 1;

            var retorno = await mediator
                .Send(new ObterCursosComResponsaveisPorAnoQuery(anoAtual, filtro.CursoId, filtro.CursoId.HasValue ? null : filtro.Pagina, 100));

            var totalPaginas = retorno.totalPaginas ?? filtro.TotalPaginas;

            Console.WriteLine($">>> Carga Mural Avisos - Página: {filtro.Pagina}/{totalPaginas}");

            try
            {
                var cursosAgrupados = retorno.cursos
                    .GroupBy(c => c.CursoId);

                var cursosResponsaveis = from cr in cursosAgrupados
                                         select new CursoResponsavelDto(cr.Key, cr.Select(c => c.UsuarioId));

                await PublicarMensagemTratar(ultimaExecucao, cursosResponsaveis);

                if (filtro.Pagina > totalPaginas)
                    await mediator.Send(new AtualizaExecucaoControleCommand(ExecucaoTipo.MuralAvisosCarregar));
                else
                    await PulicarMensagemProximaPagina(filtro.Pagina.Value + 1, totalPaginas.Value);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }

            return true;
        }

        private async Task PublicarMensagemTratar(DateTime ultimaExecucao, IEnumerable<CursoResponsavelDto> cursosResponsaveis)
        {
            await mediator
                .Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaGsaMuralAvisosTratar, new FiltroTratarMuralAvisosCursoDto(cursosResponsaveis, ultimaExecucao)));
        }

        private async Task PulicarMensagemProximaPagina(int proximaPagina, int totalPaginas)
        {
            var filtro = new FiltroCargaMuralAvisosCursoDto(pagina: proximaPagina, totalPaginas: totalPaginas);

            await mediator
                .Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaGsaMuralAvisosCarregar, filtro));
        }
    }
}
