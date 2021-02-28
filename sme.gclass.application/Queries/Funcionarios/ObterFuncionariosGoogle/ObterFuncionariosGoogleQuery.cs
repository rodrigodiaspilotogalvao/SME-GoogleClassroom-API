﻿using MediatR;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;

namespace SME.GoogleClassroom.Aplicacao
{
    public class ObterFuncionariosGoogleQuery : IRequest<PaginacaoResultadoDto<UsuarioDto>>
    {
        public ObterFuncionariosGoogleQuery(Paginacao paginacao)
        {
            Paginacao = paginacao;
        }

        public Paginacao Paginacao { get; set; }
    }
}
