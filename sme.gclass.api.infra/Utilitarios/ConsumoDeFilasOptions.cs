﻿namespace SME.GoogleClassroom.Infra
{
    public class ConsumoDeFilasOptions
    {
        public const string Secao = "ConsumoDeFilas";
        public bool ConsumirFilasSync { get; set; }
        public bool ConsumirFilasDeInclusao { get; set; }
    }
}