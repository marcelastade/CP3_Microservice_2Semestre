namespace CP3.Domain
{
    public class Emprestimo
    {
        public int IdEmprestimo { get; set; }
        public int ISBN { get; set; }
        public int IdUsuario { get; set; }
        public DateTime DataEmprestimo { get; set; }
        public DateTime DataPrevistaDev { get; set; }
        public DateTime ?DataRealDev { get; set; }
        public string Status { get; set; }
    }
}
