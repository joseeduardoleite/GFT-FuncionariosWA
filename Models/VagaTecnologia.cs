namespace WaMVC.Models
{
    public class VagaTecnologia
    {
        public int Id { get; set; }
        public Vaga Vaga { get; set; }
        public Tecnologia Tecnologia { get; set; }
        public int Status {get; set;}
    }
}