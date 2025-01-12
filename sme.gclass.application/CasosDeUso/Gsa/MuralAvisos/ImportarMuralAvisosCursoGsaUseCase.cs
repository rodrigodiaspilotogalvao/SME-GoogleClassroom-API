﻿using MediatR;
using Sentry;
using SME.GoogleClassroom.Aplicacao.Interfaces;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class ImportarMuralAvisosCursoGsaUseCase : IImportarMuralAvisosCursoGsaUseCase
    {
        private readonly IMediator mediator;

        public ImportarMuralAvisosCursoGsaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(MensagemRabbit mensagem)
        {
            if (mensagem?.Mensagem is null)
                throw new NegocioException("Não foi possível realizar a importação do aviso do mural. Mensagem não recebida");

            var avisosGsa = mensagem.ObterObjetoMensagem<AvisoMuralGsaDto[]>();

            foreach (var avisoGsa in avisosGsa)
            {
                try
                {
                    await GravarAvisoGsa(avisoGsa);
                    if (!avisoGsa.CursoCriadoManualmente)
                    {
                        var usuario = await ObterUsuario(avisoGsa.UsuarioClassroomId);
                        if (!await EnviarParaSgp(avisoGsa, usuario))
                            throw new NegocioException("Erro ao publicar aviso do mural para sincronização no SGP");
                    }
                }
                catch (Exception ex)
                {
                    await mediator.Send(new SalvarLogViaRabbitCommand($"ImportarMuralAvisosCursoGsaUseCase - Não foi possível importar o aviso do mural GSA do curso {avisoGsa.CursoId} e e usuario {avisoGsa.UsuarioClassroomId}", LogNivel.Critico, LogContexto.Gsa, ex.Message, ex.StackTrace));
                    await EnviarErro(avisoGsa);
                    throw;
                }
            }

            return true;
        }

        private async Task EnviarErro(AvisoMuralGsaDto avisoGsa)
        {
            await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaGsaMuralAvisosIncluirErro, avisoGsa));
        }

        private async Task<UsuarioGoogleDto> ObterUsuario(string usuarioClassroomId)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorClassroomIdQuery(usuarioClassroomId));

            if (usuario == null)
                throw new NegocioException("Usuário não localizado na base GCA para gravação do aviso do mural");

            return usuario;
        }

        private async Task<bool> EnviarParaSgp(AvisoMuralGsaDto avisoGsa, UsuarioGoogleDto usuario)
        {
            var curso = await mediator.Send(new ObterCursoGooglePorIdQuery(avisoGsa.CursoId));

            var avisoDto = new AvisoMuralIntegracaoSgpDto()
            {
                AvisoClassroomId = avisoGsa.Id,
                TurmaId = curso.TurmaId.ToString(),
                ComponenteCurricularId = curso.ComponenteCurricularId,
                UsuarioRf = usuario.Id.ToString(),
                Mensagem = avisoGsa.Mensagem,
                DataCriacao = avisoGsa.CriadoEm,
                DataAlteracao = avisoGsa.AlteradoEm,
                Email = usuario.Email
            };

            return await mediator.Send(new PublicaFilaRabbitSgpCommand(RotasRabbitSgp.RotaMuralAvisosSync, avisoDto, usuario.Id.ToString(), usuario.Nome));
        }

        private async Task GravarAvisoGsa(AvisoMuralGsaDto avisoGsa)
        {
            await mediator.Send(new GravarAvisoGsaCommand(avisoGsa));
        }

    }
}
