using Microsoft.EntityFrameworkCore;

using (var db = new AppDbContext())
{
    db.Database.EnsureCreated();

    while (true)
    {
        Console.WriteLine("\n1.\tCriar Nova Sala");
        Console.WriteLine("2.\tMostrar Salas");
        Console.WriteLine("3.\tReservar Horário");
        Console.WriteLine("4.\tVer Reuniões dos ultimos 7 dias");
        Console.WriteLine("5.\tAtualizar/Modificar Sala");
        Console.WriteLine("6.\tExcluir Sala");
        Console.WriteLine("0.\tSair");
        Console.WriteLine("\nOpção: ");

        string entrada = Console.ReadLine();
        if (!int.TryParse(entrada, out int opcao)) continue;

        if (opcao == 1)
        {
            Console.Write("Nome da Sala: ");
            string nome = Console.ReadLine();

            Console.Write("Andar: ");
            int andar = int.Parse(Console.ReadLine());

            Console.Write("Assentos: ");
            int assentos = int.Parse(Console.ReadLine());

            string erro = db.CriarSala(nome, andar, assentos);
            if (erro != null)
                Console.WriteLine($"Erro:\n{erro}");
            else
            {
                Console.WriteLine("Sala criada com sucesso!");
                Console.WriteLine("Para consultar o ID das salas, escolha a opção 2 no menu.");
            }

            Console.ReadKey();
            Console.Clear();
        }

        else if (opcao == 2)
        {
            Console.Write("Filtrar por nome (ou vazio): ");
            string busca = Console.ReadLine();

            var salas = db.BuscarSalas(busca);

            foreach (var s in salas)
            {
                double horasLivres = db.CalcularHorasLivres(s.Id);
                Console.WriteLine("\n==============================================\n");
                Console.WriteLine($"ID: {s.Id}\n{s.Nome}\n{s.Andar}º Andar\nNumero de assentos: {s.QuantidadeAssentos}\nHoras Livres Hoje: {horasLivres}h");
            }

            Console.ReadKey();
            Console.Clear();
        }

        else if (opcao == 3)
        {
            Console.Write("ID da Sala: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Início (dd/mm/aaaa hh:mm): ");
            DateTime inicio = DateTime.Parse(Console.ReadLine());

            Console.Write("Fim (dd/mm/aaaa hh:mm): ");
            DateTime fim = DateTime.Parse(Console.ReadLine());

            string resultado = db.RealizarReserva(id, inicio, fim);
            Console.WriteLine(resultado);

            Console.ReadKey();
            Console.Clear();
        }

        else if (opcao == 4)
        {
            int total = db.TotalReunioesUltimosDias(7);
            Console.WriteLine($"Total de reuniões nos últimos 7 dias: {total}");

            Console.ReadKey();
            Console.Clear();
        }

        else if (opcao == 5)
        {
            Console.WriteLine("---MODIFICAR OU ATULIZAR--- ");
            Console.Write("ID da Sala: ");
            int id = int.Parse(Console.ReadLine());

            var sala = db.BuscarSalaPorId(id);

            if (sala == null)
            {
                Console.WriteLine("Sala não encontrada!");
            }
            else
            {
                Console.WriteLine($"Editando: {sala.Nome} (Andar {sala.Andar})");

                Console.Write("Novo Nome: ");
                string novoNome = Console.ReadLine();

                Console.Write("Novo Andar: ");
                int novoAndar = int.Parse(Console.ReadLine());

                Console.Write("Nova Quantidade de Assentos: ");
                int novosAssentos = int.Parse(Console.ReadLine());

                Console.WriteLine("\n\tSalvando Alteirações...");

                string erro = db.AtualizarSala(id, novoNome, novoAndar, novosAssentos);
                if (erro != null)
                    Console.WriteLine($"Erro:\n{erro}");
                else
                    Console.WriteLine("\nSala atualizada com sucesso!");
            }

            Console.ReadKey();
            Console.Clear();
        }

        else if (opcao == 6)
        {
            Console.WriteLine("\n---EXCLUIR---\n");
            Console.Write("ID da Sala: ");
            int id = int.Parse(Console.ReadLine());

            var sala = db.BuscarSalaPorId(id);

            if (sala == null)
            {
                Console.WriteLine("Sala não encontrada!");
            }
            else
            {
                Console.WriteLine($"Tem certeza que deseja excluir a '{sala.Nome}'? (S/N)");
                if (Console.ReadLine().ToUpper() == "S")
                {
                    db.ExcluirSala(id);
                    Console.WriteLine($"A {sala.Nome} excluída com sucesso!");
                    Console.ReadKey();
                }
            }

            Console.ReadKey();
            Console.Clear();
        }

        else if (opcao == 0)
        {
            break;
        }

        Console.ReadKey();
        Console.Clear();
    }
}