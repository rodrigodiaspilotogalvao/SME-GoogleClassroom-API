﻿using Google.Apis.Classroom.v1;
using Google.Apis.Classroom.v1.Data;
using MediatR;
using Polly;
using Polly.Registry;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using SME.GoogleClassroom.Infra.Politicas;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class ObterCursoProfessoresGsaGoogleQueryHandler : BaseIntegracaoGoogleClassroomHandler<ObterCursoProfessoresGsaGoogleQuery, PaginaConsultaCursoUsuariosGsaDto>
    {
        private readonly IMediator mediator;
        private readonly GsaSyncOptions gsaSyncOptions;
        private readonly IAsyncPolicy policy;

        public ObterCursoProfessoresGsaGoogleQueryHandler(IMediator mediator, IReadOnlyPolicyRegistry<string> registry, GsaSyncOptions gsaSyncOptions)
            : base()
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.gsaSyncOptions = gsaSyncOptions;
            this.policy = registry.Get<IAsyncPolicy>(PoliticaPolly.PolicyCargaGsa);
        }

        public override async Task<PaginaConsultaCursoUsuariosGsaDto> Handle(ObterCursoProfessoresGsaGoogleQuery request, CancellationToken cancellationToken)
        {
            var servicoClassroom = await mediator.Send(new ObterClassroomServiceGoogleClassroomQuery());
            return await policy.ExecuteAsync(() => ObterCursoProfessoresGsaGoogleAsync(servicoClassroom, request));
        }

        private async Task<PaginaConsultaCursoUsuariosGsaDto> ObterCursoProfessoresGsaGoogleAsync(ClassroomService servicoClassroom, ObterCursoProfessoresGsaGoogleQuery request)
        {
            var contadorDePagina = 0;
            var resultado = new PaginaConsultaCursoUsuariosGsaDto(request.TokenPagina);

            await ObterCursoProfessoresGsaGoogleTotalDePaginasPorExecucaoAsync(servicoClassroom, resultado, request.CursoId, contadorDePagina);
            return resultado;
        }

        private async Task ObterCursoProfessoresGsaGoogleTotalDePaginasPorExecucaoAsync(ClassroomService servicoClassroom, PaginaConsultaCursoUsuariosGsaDto paginaConsulta, long cursoId, int contadorDePagina)
        {
            var resultadoPagina = await policy.ExecuteAsync(() => ObterCursoProfessoresGsaGoogleAsync(servicoClassroom, cursoId, paginaConsulta.TokenProximaPagina));
            if (!resultadoPagina.Teachers?.Any() ?? true) return;

            paginaConsulta.TokenProximaPagina = resultadoPagina.NextPageToken;
            paginaConsulta.CursosDoUsuario.AddRange(resultadoPagina.Teachers
                .Select(estudante => new UsuarioCursoGsaDto
                {
                    CursoId = estudante.CourseId,
                    UsuarioId = estudante.UserId,
                    UsuarioCursoTipo = (short)UsuarioCursoGsaTipo.Professor
                })
                .ToList());

            contadorDePagina++;

            if (!string.IsNullOrWhiteSpace(paginaConsulta.TokenProximaPagina) && contadorDePagina < gsaSyncOptions.QuantidadeDePaginasConsultadasPorExecucao)
                await ObterCursoProfessoresGsaGoogleTotalDePaginasPorExecucaoAsync(servicoClassroom, paginaConsulta, cursoId, contadorDePagina);
        }

        private async Task<ListTeachersResponse> ObterCursoProfessoresGsaGoogleAsync(ClassroomService servicoClassroom, long cursoId, string tokenPagina)
        {
            var requestList = servicoClassroom.Courses.Teachers.List(cursoId.ToString());
            requestList.PageToken = tokenPagina;

            RegistraRequisicaoGoogleClassroom();
            return await requestList.ExecuteAsync();
        }
    }
}
