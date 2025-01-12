﻿using System;

namespace SME.GoogleClassroom.Dominio
{
    public class AtividadeGsa
    {
        public long Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string UsuarioGsaId { get; set; }
        public long CursoGsaId { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime DataAlteracao { get; set; }

        private DateTime? _dataEntrega;
        public DateTime? DataEntrega 
        { 
            get => _dataEntrega;
            set => _dataEntrega = value.Equals(DateTime.MinValue) ? null : value;
        }
        public double NotaMaxima { get; set; }

        public AtividadeGsa()
        {
        }

        public AtividadeGsa(long id, string titulo, string descricao, string usuarioId, long cursoId,
            DateTime dataInclusao, DateTime dataAlteracao, DateTime? dataEntrega, double? notaMaxima)
        {
            Id = id;
            Titulo = titulo;
            Descricao = descricao;
            UsuarioGsaId = usuarioId;
            CursoGsaId = cursoId;
            DataInclusao = dataInclusao;
            DataAlteracao = dataAlteracao;
            DataEntrega = dataEntrega;
            NotaMaxima = notaMaxima ?? 0;
        }
    }
}