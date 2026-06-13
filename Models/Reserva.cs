public class Reserva
{
    public int Id { get; set; }
    public int SalaId { get; set; }
    public Sala Sala { get; set; }
    public DateTime Inicio { get; set; }
    public DateTime Fim { get; set; }

    public Reserva() { }

    public Reserva(int salaId, DateTime inicio, DateTime fim)
    {
        SalaId = salaId;
        Inicio = inicio;
        Fim = fim;
    }
}

public static class ReservaExtensions
{
    public static double CalcularHorasLivres(this IQueryable<Reserva> reservas, int salaId, DateTime data)
    {
        var horasOcupadas = reservas
            .Where(r => r.SalaId == salaId && r.Inicio.Date == data)
            .ToList()
            .Sum(r => (r.Fim - r.Inicio).TotalHours);

        return 11 - horasOcupadas;
    }

    public static bool HorarioIndisponivel(this IQueryable<Reserva> reservas, int salaId, DateTime inicio, DateTime fim)
    {
        return reservas.Any(r => r.SalaId == salaId && inicio < r.Fim && fim > r.Inicio);
    }

    public static int TotalReunioesUltimosDias(this IQueryable<Reserva> reservas, int dias)
    {
        var dataLimite = DateTime.Now.AddDays(-dias);
        return reservas.Count(r => r.Inicio >= dataLimite);
    }
}