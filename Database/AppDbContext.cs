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

    private string ValidarHorarioReserva(DateTime inicio, DateTime fim)
    {
        var horarioInicial = new TimeSpan(8, 0, 0);
        var horarioFinal = new TimeSpan(19, 0, 0);

        if (fim <= inicio)
            return "Erro: O fim da reserva precisa ser maior que o início.";

        if (inicio.Date != fim.Date)
            return "Erro: A reserva precisa começar e terminar no mesmo dia.";

        if (inicio.TimeOfDay < horarioInicial || fim.TimeOfDay > horarioFinal)
            return "Erro: Por favor, Escolha reservas apenas entre 08:00 e 19:00.";

        return null;
    }

    public string CriarSala(string nome, int andar, int quantidadeAssentos)
    {
        var sala = new Sala(nome, andar, quantidadeAssentos);
        string erro = ValidarEntidade(sala);

        if (erro != null)
            return erro;

        Salas.Add(sala);
        SaveChanges();
        return "Sala criada com sucesso!";
    }

    public List<Sala> BuscarSalas(string busca, int pagina, int tamanhoPagina)
    {
        var query = Salas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
            query = query.Where(s => s.Nome.Contains(busca));

        return query
            .OrderBy(s => s.Andar)
            .ThenBy(s => s.Nome)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToList();
    }

    public int ContarSalas(string busca)
    {
        var query = Salas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
            query = query.Where(s => s.Nome.Contains(busca));

        return query.Count();
    }

    public Sala BuscarSalaPorId(int id)
    {
        return Salas.Find(id);
    }

    public double CalcularHorasLivres(int salaId, DateTime inicioPeriodo, DateTime fimPeriodo)
    {
        return Reservas.CalcularHorasLivres(salaId, inicioPeriodo, fimPeriodo);
    }

    public string RealizarReserva(int salaId, DateTime inicio, DateTime fim)
    {
        var sala = Salas.Find(salaId);
        if (sala == null)
            return "Sala não encontrada!";

        var reserva = new Reserva(salaId, inicio, fim);

        string erro = ValidarEntidade(reserva);
        if (erro != null)
            return erro;

        erro = ValidarHorarioReserva(inicio, fim);
        if (erro != null)
            return erro;

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

        if (sala == null)
            return "Sala não encontrada!";

        if (string.IsNullOrWhiteSpace(novoNome))
            novoNome = sala.Nome;

        var salaTemp = new Sala(novoNome, novoAndar, novosAssentos);
        string erro = ValidarEntidade(salaTemp);

        if (erro != null)
            return erro;

        sala.Atualizar(novoNome, novoAndar, novosAssentos);
        SaveChanges();
        return "Sala atualizada com sucesso!";
    }

    public void ExcluirSala(int id)
    {
        var sala = Salas.Find(id);

        if (sala == null)
            return;

        var reservasDaSala = Reservas.Where(r => r.SalaId == id).ToList();

        Reservas.RemoveRange(reservasDaSala);
        Salas.Remove(sala);
        SaveChanges();
    }

    public List<Reserva> BuscarReservas(int? salaId, bool somenteFuturas, int pagina, int tamanhoPagina)
    {
        var query = Reservas.Include(r => r.Sala).AsQueryable();

        if (salaId.HasValue)
            query = query.Where(r => r.SalaId == salaId.Value);

        if (somenteFuturas)
            query = query.Where(r => r.Inicio > DateTime.Now);

        return query
            .OrderBy(r => r.Inicio)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToList();
    }

    public int ContarReservas(int? salaId, bool somenteFuturas)
    {
        var query = Reservas.AsQueryable();

        if (salaId.HasValue)
            query = query.Where(r => r.SalaId == salaId.Value);

        if (somenteFuturas)
            query = query.Where(r => r.Inicio > DateTime.Now);

        return query.Count();
    }

    public Reserva BuscarReservaPorId(int id)
    {
        return Reservas.Include(r => r.Sala).FirstOrDefault(r => r.Id == id);
    }

    public string ReagendarReserva(int id, DateTime novoInicio, DateTime novoFim)
    {
        var reserva = Reservas.Find(id);

        if (reserva == null)
            return "Reserva não encontrada!";

        if (reserva.Inicio <= DateTime.Now)
            return "Erro: Só é permitido reagendar reservas futuras.";

        var reservaTemp = new Reserva(reserva.SalaId, novoInicio, novoFim);

        string erro = ValidarEntidade(reservaTemp);
        if (erro != null)
            return erro;

        erro = ValidarHorarioReserva(novoInicio, novoFim);
        if (erro != null)
            return erro;

        if (Reservas.HorarioIndisponivel(reserva.SalaId, novoInicio, novoFim, reserva.Id))
            return "Erro: Horário indisponível para esta sala.";

        reserva.Reagendar(novoInicio, novoFim);
        SaveChanges();
        return "Reserva reagendada com sucesso!";
    }

    public string CancelarReserva(int id)
    {
        var reserva = Reservas.Find(id);

        if (reserva == null)
            return "Reserva não encontrada!";

        Reservas.Remove(reserva);
        SaveChanges();
        return "Reserva cancelada com sucesso!";
    }
}