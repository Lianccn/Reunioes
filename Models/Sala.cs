using System.ComponentModel.DataAnnotations;

public class Sala
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da sala é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
    public string Nome { get; set; }

    [Range(1, 100, ErrorMessage = "O andar deve ser entre 1 e 100")]
    public int Andar { get; set; }

    [Range(1, 500, ErrorMessage = "A quantidade de assentos deve ser entre 1 e 500")]
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
        if (!string.IsNullOrWhiteSpace(novoNome))
            Nome = novoNome;

        Andar = novoAndar;
        QuantidadeAssentos = novosAssentos;
    }
}