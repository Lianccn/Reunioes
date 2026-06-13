using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Sala> Salas { get; set; }
    public DbSet<Reserva> Reservas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=reunioes.db");

    public void CriarSala(string nome, int andar, int quantidadeAssentos)
    {
        Salas.Add(new Sala(nome, andar, quantidadeAssentos));
        SaveChanges();
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
        if (inicio.Hour < 8 || fim.Hour > 19 || (fim.Hour == 19 && fim.Minute > 0))
            return "Erro: Por favor, Escolha reservas apenas entre 08:00 e 19:00.";

        if (Reservas.HorarioIndisponivel(salaId, inicio, fim))
            return "Erro: Horário indisponível para esta sala.";

        Reservas.Add(new Reserva(salaId, inicio, fim));
        SaveChanges();
        return "Reserva realizada!";
    }

    public int TotalReunioesUltimosDias(int dias)
    {
        return Reservas.TotalReunioesUltimosDias(dias);
    }

    public void AtualizarSala(int id, string novoNome, int novoAndar, int novosAssentos)
    {
        var sala = Salas.Find(id);
        if (sala == null) return;

        sala.Atualizar(novoNome, novoAndar, novosAssentos);
        SaveChanges();
    }

    public void ExcluirSala(int id)
    {
        var sala = Salas.Find(id);
        if (sala == null) return;

        Salas.Remove(sala);
        SaveChanges();
    }
}