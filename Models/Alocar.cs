using System;

namespace WaMVC.Models
{
    public class Alocar
    {
        public Alocar() { }

        public Alocar(FuncionarioTecnologia func, VagaTecnologia vag, DateTime inicioAlocacao)
        {
            this.func = func;
            this.vag = vag;
            this.InicioAlocacao = inicioAlocacao;
        }

        public int Id { get; set; }
        public DateTime InicioAlocacao { get; set; }
        public FuncionarioTecnologia func { get; set; }
        public VagaTecnologia vag { get; set; }
    }
}