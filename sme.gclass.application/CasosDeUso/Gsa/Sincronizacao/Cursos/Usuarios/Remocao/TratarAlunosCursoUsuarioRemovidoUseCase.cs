﻿using MediatR;
using Sentry;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class TratarAlunosCursoUsuarioRemovidoUseCase : ITratarAlunosCursoUsuarioRemovidoUseCase
    {
        private readonly IMediator mediator;

        public TratarAlunosCursoUsuarioRemovidoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(MensagemRabbit mensagemRabbit)
        {
            var dto = mensagemRabbit.ObterObjetoMensagem<FiltroTurmaRemoverCursoUsuarioDto>();

            var parametrosCargaInicialDto = await mediator.Send(new ObterParametrosCargaIncialPorAnoQuery(DateTime.Today.Year));
            var alunosCodigosInativosEOL = await mediator.Send(new ObterAlunosCodigosInativosPorAnoLetivoETurmaQuery(DateTime.Today.Year, dto.TurmaId, dto.DatasAluno.DataInicio, dto.DatasAluno.DataFim, parametrosCargaInicialDto));

            if (alunosCodigosInativosEOL != null && alunosCodigosInativosEOL.Any())
            {
                var cursosUsuarios = await mediator.Send(new ObterAlunosCodigosComRegistroEmCursosQuery(alunosCodigosInativosEOL, dto.TurmaId));

                if (cursosUsuarios != null && cursosUsuarios.Any())
                {
                    foreach (var cursoUsuario in cursosUsuarios)
                    {
                        var cursoUsuarioRemover = new CursoUsuarioRemoverDto()
                        {
                            CursoUsuarioId = cursoUsuario.CursoUsuarioId,
                            CursoId = cursoUsuario.CursoId,
                            UsuarioId = cursoUsuario.UsuarioId,
                            UsuarioGsaId = cursoUsuario.UsuarioGsaId,
                            TipoUsuario = (int)UsuarioTipo.Aluno,
                            TipoGsa = (int)UsuarioCursoGsaTipo.Estudante,
                        };

                        await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaGsaCursoUsuarioRemovidoSync, cursoUsuarioRemover));
                    }
                }
            }

            return true;
        }
    }
}
