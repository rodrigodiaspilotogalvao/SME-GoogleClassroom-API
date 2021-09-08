﻿using Dapper;
using SME.GoogleClassroom.Dados.Interfaces;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Dados
{
    public class RepositorioNota : RepositorioGoogle, IRepositorioNota
    {
        public RepositorioNota(ConnectionStrings connectionStrings) : base(connectionStrings)
        {
        }

        public async Task<long> AlterarNota(NotaGsa notaGsa)
        {
            const string updateQuery = @"update public.notas
                                            set atividade_id = @atividadeId
                                              , usuario_id = @usuarioId
                                              , nota = @nota
                                              , status = @status
                                              , data_importacao = @dataImportacao
                                              , data_inclusao = @dataInclusao
                                              , data_alteracao = @dataAlteracao
                                        where id = @id";

            var parametros = new
            {
                id = notaGsa.Id,
                atividadeId = notaGsa.AtividadeId,
                usuarioId = notaGsa.UsuarioId,
                nota = notaGsa.Nota,
                status = (int)notaGsa.Status,
                dataImportacao = notaGsa.DataImportacao,
                dataInclusao = notaGsa.DataInclusao,
                dataAlteracao = notaGsa.DataAlteracao,
            };

            using var conn = ObterConexao();
            return await conn.ExecuteAsync(updateQuery, parametros);
        }

        public async Task<long> InserirNota(NotaGsa notaGsa)
        {
            const string insertQuery = @"insert into public.notas
                                        (id, atividade_id, usuario_id, nota, status, data_importacao, data_inclusao, data_alteracao)
                                        values
                                        (@id, @atividadeId, @usuarioId, @nota, @status, @dataImportacao, @dataInclusao, @dataAlteracao)";

            var parametros = new
            {
                id = notaGsa.Id,
                atividadeId = notaGsa.AtividadeId,
                usuarioId = notaGsa.UsuarioId,
                nota = notaGsa.Nota,
                status = (int)notaGsa.Status,
                dataImportacao = notaGsa.DataImportacao,
                dataInclusao = notaGsa.DataInclusao,
                dataAlteracao = notaGsa.DataAlteracao,
            };

            using var conn = ObterConexao();
            return await conn.ExecuteAsync(insertQuery, parametros);
        }

        public async Task<bool> RegistroExiste(string id)
        {
            using var conn = ObterConexao();
            return await conn.QueryFirstOrDefaultAsync<bool>("select 1 from notas where id = @id", new { id });
        }
    }
}
