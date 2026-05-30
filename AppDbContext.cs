using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Sala> Salas {get; set;}
    public DbSet<Reserva> Reservas {get; set;}

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=reunioes.db");

}