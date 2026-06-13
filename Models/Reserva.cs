using System.ComponentModel.DataAnnotations;

public class Reserva : IValidatableObject
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A sala de reunião é obrigatória")]
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

    public void Reagendar(DateTime novoInicio, DateTime novoFim)
    {
        Inicio = novoInicio;
        Fim = novoFim;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Inicio == default)
            yield return new ValidationResult("O início da reserva é obrigatório", new[] { nameof(Inicio) });

        if (Fim == default)
            yield return new ValidationResult("O fim da reserva é obrigatório", new[] { nameof(Fim) });

        if (Inicio != default && Fim != default && Fim <= Inicio)
            yield return new ValidationResult("O fim da reserva precisa ser maior que o início", new[] { nameof(Fim) });

        if (Inicio != default && Fim != default && Inicio.Date != Fim.Date)
            yield return new ValidationResult("A reserva precisa começar e terminar no mesmo dia", new[] { nameof(Inicio), nameof(Fim) });
    }
}

public static class ReservaExtensions
{
    public static double CalcularHorasLivres(this IQueryable<Reserva> reservas, int salaId, DateTime inicioPeriodo, DateTime fimPeriodo)
    {
        var totalHoras = (fimPeriodo - inicioPeriodo).TotalHours;

        var horasOcupadas = reservas
            .Where(r => r.SalaId == salaId && r.Inicio < fimPeriodo && r.Fim > inicioPeriodo)
            .ToList()
            .Sum(r =>
            {
                var inicio = r.Inicio > inicioPeriodo ? r.Inicio : inicioPeriodo;
                var fim = r.Fim < fimPeriodo ? r.Fim : fimPeriodo;
                return (fim - inicio).TotalHours;
            });

        var horasLivres = totalHoras - horasOcupadas;

        if (horasLivres < 0)
            return 0;

        return Math.Round(horasLivres, 2);
    }

    public static bool HorarioIndisponivel(this IQueryable<Reserva> reservas, int salaId, DateTime inicio, DateTime fim)
    {
        return reservas.Any(r => r.SalaId == salaId && inicio < r.Fim && fim > r.Inicio);
    }

    public static bool HorarioIndisponivel(this IQueryable<Reserva> reservas, int salaId, DateTime inicio, DateTime fim, int reservaIgnoradaId)
    {
        return reservas.Any(r => r.Id != reservaIgnoradaId && r.SalaId == salaId && inicio < r.Fim && fim > r.Inicio);
    }

    public static int TotalReunioesUltimosDias(this IQueryable<Reserva> reservas, int dias)
    {
        var dataLimite = DateTime.Now.AddDays(-dias);
        return reservas.Count(r => r.Inicio >= dataLimite);
    }
}