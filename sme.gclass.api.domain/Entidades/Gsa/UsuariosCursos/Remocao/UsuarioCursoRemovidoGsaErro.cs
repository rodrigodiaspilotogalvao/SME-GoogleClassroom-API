﻿using System;

namespace SME.GoogleClassroom.Dominio
{
    public class UsuarioCursoRemovidoGsaErro : BaseErro
    {
        public string UsuarioId { get; set; }
        public string CursoId { get; set; }

        public UsuarioCursoRemovidoGsaErro(string usuarioId, string cursoId, string mensagem)
        {
            UsuarioId = usuarioId;
            CursoId = cursoId;
            Mensagem = mensagem;
            ExecucaoTipo = ExecucaoTipo.AlunoCursoRemover;
            Tipo = ErroTipo.Negocio;
            DataInclusao = DateTime.Now;
        }
    }
}
