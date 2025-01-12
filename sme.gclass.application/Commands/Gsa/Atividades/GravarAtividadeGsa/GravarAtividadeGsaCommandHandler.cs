﻿using MediatR;
using SME.GoogleClassroom.Dados.Interfaces;
using SME.GoogleClassroom.Dominio;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class GravarAtividadeGsaCommandHandler : AsyncRequestHandler<GravarAtividadeGsaCommand>
    {
        private readonly IRepositorioAtividade repositorioAtividade;

        public GravarAtividadeGsaCommandHandler(IRepositorioAtividade repositorioAtividade)
        {
            this.repositorioAtividade = repositorioAtividade ?? throw new ArgumentNullException(nameof(repositorioAtividade));
        }

        protected override async Task Handle(GravarAtividadeGsaCommand request, CancellationToken cancellationToken)
        {
            var avisoGsa = MapearEntidade(
                request.AtividadeDto.Id,
                request.AtividadeDto.CursoId,
                request.AtividadeDto.UsuarioClassroomId,
                request.AtividadeDto.Titulo,
                request.AtividadeDto.Descricao,
                request.AtividadeDto.CriadoEm,
                request.AtividadeDto.AlteradoEm,
                request.AtividadeDto.DataEntrega,
                request.AtividadeDto.NotaMaxima.Value);

            if (await RegistroExistente(request.AtividadeDto.Id))
                await repositorioAtividade.AlterarAtividade(avisoGsa);
            else
                await repositorioAtividade.InserirAtividade(avisoGsa);
        }

        private AtividadeGsa MapearEntidade(long id, long cursoId, string usuarioId, string titulo, string descricao, DateTime criadoEm, DateTime alteradoEm, DateTime? dataEntrega, double notaMaxima)
            => new Dominio.AtividadeGsa(id,
                                    titulo,
                                    descricao,
                                    usuarioId,
                                    cursoId,
                                    criadoEm,
                                    alteradoEm, 
                                    dataEntrega,
                                    notaMaxima);

        private async Task<bool> RegistroExistente(long id)
            => await repositorioAtividade.RegistroExiste(id);
    }
}
