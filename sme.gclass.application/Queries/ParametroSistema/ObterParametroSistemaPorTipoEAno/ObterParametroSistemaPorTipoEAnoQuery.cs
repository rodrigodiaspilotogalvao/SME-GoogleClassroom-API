﻿using FluentValidation;
using MediatR;
using SME.GoogleClassroom.Dominio;

namespace SME.GoogleClassroom.Aplicacao
{
    public class ObterParametroSistemaPorTipoEAnoQuery: IRequest<ParametrosSistema>
    {
        public TipoParametroSistema Tipo { get; set; }
        public int Ano { get; set; }

        public ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema tipo, int ano)
        {
            Tipo = tipo;
            Ano = ano;
        }
    }

    public class ObterParametroSistemaPorTipoEAnoQueryValidator : AbstractValidator<ObterParametroSistemaPorTipoEAnoQuery>
    {
        public ObterParametroSistemaPorTipoEAnoQueryValidator()
        {
            RuleFor(a => a.Tipo)
                .NotEmpty()
                .WithMessage("O tipo deve ser informado");

            RuleFor(a => a.Ano)
                .NotEmpty()
                .WithMessage("O ano deve ser informado");
        }
    }
}
