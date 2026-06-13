public class Sala
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public int Andar { get; set; }
    public int QuantidadeAssentos { get; set; }
    public List<Reserva> ListaAgendamentos { get; set; } = new List<Reserva>();

    public Sala() { }

    public Sala(string nome, int andar, int quantidadeAssentos)
    {
        Nome = nome;
        Andar = andar;
        QuantidadeAssentos = quantidadeAssentos;
    }

    public void Atualizar(string novoNome, int novoAndar, int novosAssentos)
    {
        Nome = novoNome;
        Andar = novoAndar;
        QuantidadeAssentos = novosAssentos;
    }
}