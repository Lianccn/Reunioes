using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class AppDbContext : DbContext
{
    public DbSet<Sala> Salas { get; set; }
    public DbSet<Reserva> Reservas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=reunioes.db");

    private string ValidarEntidade(object entidade)
    {
        var contexto = new ValidationContext(entidade);
        var resultados = new List<ValidationResult>();
        bool valido = Validator.TryValidateObject(entidade, contexto, resultados, true);

        if (!valido)
            return string.Join("\n", resultados.Select(r => r.ErrorMessage));

        return null;
    }

    public string CriarSala(string nome, int andar, int quantidadeAssentos)
    {
        var sala = new Sala(nome, andar, quantidadeAssentos);
        string erro = ValidarEntidade(sala);
        if (erro != null) return erro;

        Salas.Add(sala);
        SaveChanges();
        return null;
    }

    public List<Sala> BuscarSalas(string busca)
    {
        var query = Salas.AsQueryable();
        if (!string.IsNullOrEmpty(busca))
            query = query.Where(s => s.Nome.Contains(busca));

        return query.OrderBy(s => s.Andar).Take(10).ToList();
    }

    public Sala BuscarSalaPorId(int id)
    {
        return Salas.Find(id);
    }

    public double CalcularHorasLivres(int salaId)
    {
        return Reservas.CalcularHorasLivres(salaId, DateTime.Today);
    }

    public string RealizarReserva(int salaId, DateTime inicio, DateTime fim)
    {
        var reserva = new Reserva(salaId, inicio, fim);
        string erro = ValidarEntidade(reserva);
        if (erro != null) return erro;

        if (inicio.Hour < 8 || fim.Hour > 19 || (fim.Hour == 19 && fim.Minute > 0))
            return "Erro: Por favor, Escolha reservas apenas entre 08:00 e 19:00.";

        if (Reservas.HorarioIndisponivel(salaId, inicio, fim))
            return "Erro: Horário indisponível para esta sala.";

        Reservas.Add(reserva);
        SaveChanges();
        return "Reserva realizada!";
    }

    public int TotalReunioesUltimosDias(int dias)
    {
        return Reservas.TotalReunioesUltimosDias(dias);
    }

    public string AtualizarSala(int id, string novoNome, int novoAndar, int novosAssentos)
    {
        var sala = Salas.Find(id);
        if (sala == null) return "Sala não encontrada!";

        var salaTemp = new Sala(novoNome, novoAndar, novosAssentos);
        string erro = ValidarEntidade(salaTemp);
        if (erro != null) return erro;

        sala.Atualizar(novoNome, novoAndar, novosAssentos);
        SaveChanges();
        return null;
    }

    public void ExcluirSala(int id)
    {
        var sala = Salas.Find(id);
        if (sala == null) return;

        Salas.Remove(sala);
        SaveChanges();
    }
}