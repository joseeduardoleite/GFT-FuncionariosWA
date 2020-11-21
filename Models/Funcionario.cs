using System;

namespace WaMVC.Models
{
    public class Funcionario
    {
        public Funcionario() { }

        public Funcionario(Funcionario funcionario)
        {
            Id = funcionario.Id;
            Nome = funcionario.Nome;
            Cargo = funcionario.Cargo;
            InicioWa = funcionario.InicioWa;
            TerminoWa = funcionario.TerminoWa;
            Matricula = funcionario.Matricula;
            LocalDeTrabalho = funcionario.LocalDeTrabalho;
            Alocacao = funcionario.Alocacao;
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cargo { get; set; }
        public DateTime InicioWa { get; set; }
        public DateTime TerminoWa { get; set; }
        public string Matricula { get; set; }
        public GFT LocalDeTrabalho { get; set; }
        public Vaga Alocacao { get; set; }
    }
}