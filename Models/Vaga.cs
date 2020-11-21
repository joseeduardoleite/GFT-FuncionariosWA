using System;

namespace WaMVC.Models
{
    public class Vaga
    {
        public Vaga() { }

        public Vaga(Vaga vaga)
        {
            Id = vaga.Id;
            AberturaVaga = vaga.AberturaVaga;
            CodigoVaga = vaga.CodigoVaga;
            DescricaoVaga = vaga.DescricaoVaga;
            Projeto = vaga.Projeto;
            QuantidadeVaga = vaga.QuantidadeVaga;
        }

        public int Id { get; set; }
        public DateTime AberturaVaga { get; set; }
        public string CodigoVaga { get; set; }
        public string DescricaoVaga { get; set; }
        public string Projeto { get; set; }
        public int QuantidadeVaga { get; set; }
    }
}