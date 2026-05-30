public class Sala
{
    public int Id {get; set;}
    public int Numero {get; set;}

    public int QuantidadeAssentos {get; set;}

    private List<Reserva> ListaAgendamentos;

    public Sala(int numero, int quantidadeAssentos)
    {
        Numero = numero;
        QuantidadeAssentos = quantidadeAssentos;

        ListaAgendamentos = new List<Reserva>();
    }

    public bool VerificarDisponibilidade(DateTime inicio, DateTime fim)
    {
        foreach (var reserva in ListaAgendamentos)
        {
            bool conflito = inicio < reserva.Fim && fim > reserva.Inicio;

            if (conflito)
            {
                return false;
            }
        }

        return true;
    }

    public bool CriarAgendamento(DateTime inicio, DateTime fim)
    {

        if (inicio.Hour < 8 || fim.Hour > 19)
        {
            Console.WriteLine("Horário inválido.");
            return false;
        }

        if (!VerificarDisponibilidade(inicio, fim))
        {
            Console.WriteLine("Sala indisponível.");
            return false;
        }

        Reserva novaReserva = new Reserva
        {
            Inicio = inicio,
            Fim = fim
        };

        ListaAgendamentos.Add(novaReserva);

        Console.WriteLine("Reserva criada.");

        return true;
    }

    public bool ExcluirAgendamento(DateTime inicio)
    {
        var reserva = ListaAgendamentos.FirstOrDefault(r => r.Inicio == inicio);

        if (reserva == null)
        {
            Console.WriteLine("Reserva não encontrada.");
            return false;
        }

        ListaAgendamentos.Remove(reserva);

        Console.WriteLine("Reserva removida.");

        return true;
    }

    public void MostrarAgendamentos()
    {
        foreach (var reserva in ListaAgendamentos)
        {
            Console.WriteLine($"Início: {reserva.Inicio} | Fim: {reserva.Fim}");
        }
    }
}