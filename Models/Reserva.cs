public class Reserva
{
    public int Id { get; set; }
    public int SalaId { get; set; }
    public Sala Sala { get; set; }
    public DateTime Inicio { get; set; }
    public DateTime Fim { get; set; }
}