﻿using MediatR;
using Newtonsoft.Json;
using Sentry;
using SME.GoogleClassroom.Aplicacao.Interfaces;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class TrataSyncGoogleCursosDoAlunoUseCase : ITrataSyncGoogleCursosDoAlunoUseCase
    {
        private readonly IMediator mediator;

        public TrataSyncGoogleCursosDoAlunoUseCase(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<bool> Executar(MensagemRabbit mensagemRabbit)
        {
            var filtro = mensagemRabbit.ObterObjetoMensagem<FiltroAlunoCursoDto>();
            var alunoParaIncluirCursos = filtro.AlunoGoogle;
            if (alunoParaIncluirCursos is null)
            {
                await IncluirCursoDoAlunoComErroAsync(alunoParaIncluirCursos, "Não foi possível iniciar a inclusão de cursos professores no Google Classroom. O professor não foi informado corretamente.");
                return true;
            }
            var parametrosCargaInicialDto = filtro.AnoLetivo.HasValue ? new ParametrosCargaInicialDto(filtro.TiposUes, filtro.Ues, filtro.Turmas, filtro.AnoLetivo.Value) : 
                await mediator.Send(new ObterParametrosCargaIncialPorAnoQuery(DateTime.Today.Year));
            var anoLetivo = filtro.AnoLetivo ?? DateTime.Now.Year;
            var cursosDoAlunoParaIncluir = await mediator.Send(new ObterCursosDoAlunoParaIncluirGoogleQuery(alunoParaIncluirCursos.Codigo, anoLetivo, parametrosCargaInicialDto));

            if (!cursosDoAlunoParaIncluir?.Any() ?? true) return true;

            foreach (var cursoDoAlunoParaIncluir in cursosDoAlunoParaIncluir)
            {
                try
                {
                    var publicarProfessor = await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaAlunoCursoIncluir, RotasRabbit.FilaAlunoCursoIncluir, cursoDoAlunoParaIncluir));
                    if (!publicarProfessor)
                    {
                        await IncluirCursoDoAlunoComErroAsync(cursoDoAlunoParaIncluir, ObterMensagemDeErro(cursoDoAlunoParaIncluir));
                    }
                }
                catch (Exception ex)
                {
                    await IncluirCursoDoAlunoComErroAsync(cursoDoAlunoParaIncluir, ObterMensagemDeErro(cursoDoAlunoParaIncluir, ex));
                }
            }

            return true;
        }

        private async Task IncluirCursoDoAlunoComErroAsync(AlunoGoogle alunoGoogle, string mensagem)
        {
            var command = new IncluirCursoUsuarioErroCommand(
                alunoGoogle.Codigo,
                ExecucaoTipo.ProfessorCursoAdicionar,
                ErroTipo.Negocio,
                mensagem);

            await mediator.Send(command);
        }

        private async Task IncluirCursoDoAlunoComErroAsync(AlunoCursoEol cursoDoAlunoParaIncluirGoogle, string mensagem)
        {
            var command = new IncluirCursoUsuarioErroCommand(
                cursoDoAlunoParaIncluirGoogle.CodigoAluno,
                cursoDoAlunoParaIncluirGoogle.TurmaId,
                cursoDoAlunoParaIncluirGoogle.ComponenteCurricularId,
                ExecucaoTipo.ProfessorCursoAdicionar,
                ErroTipo.Negocio,
                mensagem);

            await mediator.Send(command);
        }

        private static string ObterMensagemDeErro(AlunoCursoEol cursoDoAlunoParaIncluirGoogle, Exception ex = null)
        {
            var mensagem = $"Não foi possível inserir o curso [TurmaId:{cursoDoAlunoParaIncluirGoogle.TurmaId}, ComponenteCurricularId:{cursoDoAlunoParaIncluirGoogle.ComponenteCurricularId}] aluno RA{cursoDoAlunoParaIncluirGoogle.CodigoAluno} na fila para inclusão no Google Classroom.";
            if (ex is null) return mensagem;
            return $"{mensagem}. {ex.InnerException?.Message ?? ex.Message}. {ex.StackTrace}";
        }
    }
}