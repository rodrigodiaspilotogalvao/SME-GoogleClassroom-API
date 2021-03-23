﻿using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Dados
{
    public interface IRepositorioUsuario
    {
        Task<PaginacaoResultadoDto<AlunoGoogle>> ObterAlunosAsync(Paginacao paginacao, long? codigoEol, string email);

        Task<bool> ExisteAlunoPorRf(long rf);

        Task<string> ObterEmailUsuarioPorTipo(string email, int usuarioTipo);

        Task<PaginacaoResultadoDto<FuncionarioGoogle>> ObterFuncionariosAsync(Paginacao paginacao, long? rf, string email);

        Task<PaginacaoResultadoDto<ProfessorGoogle>> ObterProfessoresAsync(Paginacao paginacao, long? rf, string email);

        Task<IEnumerable<FuncionarioGoogle>> ObterFuncionariosPorRfs(long[] rfs);

        Task<bool> ExisteFuncionarioPorRf(long rf);

        Task<IEnumerable<ProfessorGoogle>> ObterProfessoresPorRfs(long[] rfs);

        Task<PaginacaoResultadoDto<ProfessorGoogle>> ObterProfessoresFuncionariosPaginadoPorRfs(Paginacao paginacao, long[] rfs);

        Task<bool> ExisteProfessorPorRf(long rf);

        Task<long> SalvarAsync(long? id, string nome, string email, UsuarioTipo tipo, string organizationPath, DateTime dataInclusao, DateTime? dataAtualizacao);
        
        Task<IEnumerable<AlunoGoogle>> ObterAlunosPorCodigos(long[] codigosEol);
        Task<PaginacaoResultadoDto<AlunoGoogle>> ObterAlunosPaginadoPorCodigos(Paginacao paginacao, long[] codigosAluno);
        Task<FuncionarioGoogle> ObterFuncionarioPorEmail(string email);
    }
}