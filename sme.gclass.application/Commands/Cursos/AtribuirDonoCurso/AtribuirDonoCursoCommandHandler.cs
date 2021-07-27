﻿using Google;
using MediatR;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class AtribuirDonoCursoCommandHandler : IRequestHandler<AtribuirDonoCursoCommand, bool>
    {
        private readonly IMediator mediator;

        public AtribuirDonoCursoCommandHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(AtribuirDonoCursoCommand request, CancellationToken cancellationToken)
        {
            var curso = await mediator.Send(new ObterCursoPorTurmaComponenteCurricularQuery(request.TurmaId, request.ComponenteCurricularId));

            if (curso == null)
                throw new NegocioException("Não foi possível alterar o dono do curso, pois o curso não existe na base do GSA");
            
            try
            {
                var retornoGoogle = await mediator.Send(new AtribuirDonoCursoGoogleCommand(curso.Id, request.GoogleClassroomId));

                if (retornoGoogle)
                {
                    curso.Email = request.UsuarioEmail;
                    await mediator.Send(new AlterarCursoCommand(curso));
                }

                return retornoGoogle;
            }
            catch (GoogleApiException gEx)
            {
                if (gEx.RegistroNaoEncontrado()) throw new NegocioException("Usuário não existe no Google Classroom");
                else if (gEx.AcessoNaoAutorizado()) throw new NegocioException("Usuário sem acesso ao Google Classroom");
                else if (gEx.EmailContaServicoInvalido()) throw new NegocioException("Email informado é inválido");
                else
                    throw new NegocioException(gEx.Message);
            }
        }
    }
}
