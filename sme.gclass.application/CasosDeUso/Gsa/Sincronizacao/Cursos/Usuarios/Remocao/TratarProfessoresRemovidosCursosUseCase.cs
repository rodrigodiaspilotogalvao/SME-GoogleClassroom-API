﻿using MediatR;
using SME.GoogleClassroom.Aplicacao.Interfaces;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class TratarProfessoresRemovidosCursosUseCase : ITratarProfessoresRemovidosCursosUseCase
    {
        private readonly IMediator mediator;

        public TratarProfessoresRemovidosCursosUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(MensagemRabbit mensagemRabbit)
        {
            var dto = mensagemRabbit.ObterObjetoMensagem<FiltroTurmaRemoverCursoUsuarioDto>();

            var professoresASeremRemovidos = await mediator.Send(new ObterProfessoresParaRemoverCursoQuery(dto.TurmaId.ToString(), dto.DatasProfessor.DataInicio, dto.DatasProfessor.DataFim)); 

            foreach (var professorASerRemovido in professoresASeremRemovidos)
            {
               var professorCurso = await ObterInformacoesDoProfessorECurso(professorASerRemovido);

                if (professorCurso.professor == null)
                    continue;

               var cursoUsuarioRemover = new CursoUsuarioRemoverDto
                {
                    CursoUsuarioId = professorCurso.cursoUsuarioId,
                    CursoId = professorCurso.curso.CursoId,
                    TipoUsuario = (int)UsuarioTipo.Professor,
                    TipoGsa = (int)UsuarioCursoGsaTipo.Professor,
                    UsuarioId = professorCurso.professor.Indice, 
                    UsuarioGsaId = professorCurso.professor.GoogleClassRoomId,
                };

                if (ProfessorASerRemovidoEhResponsavelPeloCurso(professorCurso.professor, professorCurso.curso))
                {
                    var funcionariosDoCurso = await mediator.Send(new ObterFuncionariosPorCursoQuery(professorCurso.curso.CursoId));

                    var novoResponsavel = DefinaNovoResponsavelPeloCurso(funcionariosDoCurso, professorCurso.professor);

                    var donoDoCursoAlterado = await mediator.Send(new AtribuirDonoCursoCommand(professorCurso.curso.TurmaId, professorCurso.curso.ComponenteCurricularId, novoResponsavel.GoogleClassroomId, novoResponsavel.Email));
                }

                await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaGsaCursoUsuarioRemovidoSync, cursoUsuarioRemover));
            }

            return true;
        }

        private UsuarioGoogleDto DefinaNovoResponsavelPeloCurso(IEnumerable<UsuarioGoogleDto> funcionariosDoCurso, ProfessorCursosCadastradosDto professor)
        {
            UsuarioGoogleDto funcionarioResponsavel;

            var tiposFuncionarios = new[] { "/Professores", "/Admin/CP", "/Admin/AD", "/Admin/DIRETOR" };
            var funcionarios = funcionariosDoCurso.Where(o => !o.Email.Equals(professor.Email)).ToList();

            foreach (var tipoFuncionario in tiposFuncionarios)
            {
                funcionarioResponsavel = funcionarios.FirstOrDefault(o => o.OrganizationPath.Equals(tipoFuncionario));

                if (funcionarioResponsavel != null)
                    return funcionarioResponsavel;
            }

            throw new NegocioException("Não foi possível localizar novo responsável pelo curso. O professor não poderá ser removido.");
        }

        private async Task<(ProfessorCursosCadastradosDto professor, CursoDto curso, long cursoUsuarioId)> ObterInformacoesDoProfessorECurso(RemoverAtribuicaoProfessorCursoEolDto professorASerRemovido)
        {
            var paginacao = new Paginacao(1, 50);
            var professorCurso = await mediator.Send(new ObterProfessoresCursosGoogleQuery(paginacao, long.Parse(professorASerRemovido.UsuarioRf), professorASerRemovido.TurmaCodigo, professorASerRemovido.ComponenteCurricularCodigo));

            if (professorCurso.Items == null || !professorCurso.Items.Any())
                throw new NegocioException("Não foi possível localizar o professor do curso.");

            var professor = professorCurso.Items.FirstOrDefault();
            var curso = professor.Cursos.FirstOrDefault();

            if (curso is null)
                throw new NegocioException("Não foi possível localizar curso para a remoção de professores.");

            if (professor is null)
                throw new NegocioException("Não foi possível localizar professor para ser removido.");

            return (professor, curso, professorCurso.Items.FirstOrDefault().CursoUsuarioId);
        }

        private bool ProfessorASerRemovidoEhResponsavelPeloCurso(ProfessorCursosCadastradosDto professor, CursoDto curso)
        {
            return curso.Email.Equals(professor.Email);
        }
    }
}