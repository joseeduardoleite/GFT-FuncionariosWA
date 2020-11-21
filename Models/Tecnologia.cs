namespace WaMVC.Models
{
    public class Tecnologia
    {
        public Tecnologia() { }

        public Tecnologia(Tecnologia tec)
        {
            Id = tec.Id;
            Nome = tec.Nome;
        }

        public int Id { get; set; }
        public string Nome { get; set; }
    }
}