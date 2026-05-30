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
        Console.WriteLine("0. Sair");
        Console.Write("\nOpção: ");

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

            db.Salas.Add(new Sala 
            { 
                Nome = nome, 
                Andar = andar, 
                QuantidadeAssentos = assentos 
            });
            db.SaveChanges();
            Console.WriteLine("Sala criada com sucesso!");

            Console.ReadKey();
            Console.Clear();
        }

        else if (opcao == 2)
        {
            Console.Write("Filtrar por nome (ou vazio): ");
            string busca = Console.ReadLine();

            var query = db.Salas.AsQueryable();
            if (!string.IsNullOrEmpty(busca)) 
                query = query.Where(s => s.Nome.Contains(busca));

            var salas = query.OrderBy(s => s.Andar).Take(10).ToList();

            foreach (var s in salas)
            {
                var horasOcupadas = db.Reservas.Where(r => r.SalaId == s.Id && r.Inicio.Date == DateTime.Today).ToList().Sum(r => (r.Fim - r.Inicio).TotalHours);
                double horasLivres = 11 - horasOcupadas;

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

            if (inicio.Hour < 8 || fim.Hour > 19 || (fim.Hour == 19 && fim.Minute > 0))
            {
                Console.WriteLine("Erro: Por favor, Escolha reservas apenas entre 08:00 e 19:00.");
            }
            else if (db.Reservas.Any(r => r.SalaId == id && inicio < r.Fim && fim > r.Inicio))
            {
                Console.WriteLine("Erro: Horário indisponível para esta sala.");
            }
            else
            {
                db.Reservas.Add(new Reserva { SalaId = id, Inicio = inicio, Fim = fim });
                db.SaveChanges();
                Console.WriteLine("Reserva realizada!");
            }
        
            Console.ReadKey();
            Console.Clear();
        }

        else if (opcao == 4)
        {
            var seteDiasAtras = DateTime.Now.AddDays(-7);
            int total = db.Reservas.Count(r => r.Inicio >= seteDiasAtras);
            Console.WriteLine($"Total de reuniões nos últimos 7 dias: {total}");
        }

        else if (opcao == 5)
        {
            Console.WriteLine("---MODIFICAR OU ATULIZAR--- ");
            Console.Write("ID da Sala: ");
            int id = int.Parse(Console.ReadLine());

            var sala = db.Salas.Find(id);

            if (sala == null)
            {
                Console.WriteLine("Sala não encontrada!");
            }
            else
            {
                Console.WriteLine($"Editando: {sala.Nome} (Andar {sala.Andar})");
                
                Console.Write("Novo Nome (ou Enter para manter): ");
                string novoNome = Console.ReadLine();
                sala.Nome = novoNome;

                Console.Write("Novo Andar: ");
                sala.Andar = int.Parse(Console.ReadLine());

                Console.Write("Nova Quantidade de Assentos: ");
                sala.QuantidadeAssentos = int.Parse(Console.ReadLine());

                Console.WriteLine("\n\tSalvando Alteirações...");

                db.SaveChanges();
                Console.WriteLine("\nSala atualizada com sucesso!");
                Console.ReadKey();
            }
        }
        
        else if (opcao == 6)
        {   
            Console.WriteLine("\n---EXCLUIR---\n");
            Console.Write("ID da Sala: ");
            int id = int.Parse(Console.ReadLine());

            var sala = db.Salas.Find(id);

            if (sala == null)
            {
                Console.WriteLine("Sala não encontrada!");
            }
            else
            {
                Console.WriteLine($"Tem certeza que deseja excluir a '{sala.Nome}'? (S/N)");
                if (Console.ReadLine().ToUpper() == "S")
                {
                    db.Salas.Remove(sala);
                    db.SaveChanges();

                    Console.WriteLine($"A {sala.Nome} excluída com sucesso!");
                    Console.ReadKey();
                }
            }
        }

        else if (opcao == 0)
        {
            break;
        }
        
            Console.ReadKey();
            Console.Clear();
    }
}