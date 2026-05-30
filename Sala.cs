public class Sala
{
    public int Id { get; set; }
    public string Nome { get; set; } // O requisito pede Nome
    public int Andar { get; set; }
    public int QuantidadeAssentos { get; set; }

    // Relacionamento para o Banco de Dados
    public List<Reserva> ListaAgendamentos { get; set; } = new List<Reserva>();

    // Construtor vazio obrigatório para o Entity Framework
    public Sala() { }

    public Sala(string nome, int andar, int quantidadeAssentos)
    {
        Nome = nome;
        Andar = andar;
        QuantidadeAssentos = quantidadeAssentos;
    }
}