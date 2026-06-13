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

        Console.WriteLine("7.\tMostrar Reservas");
        Console.WriteLine("8.\tReagendar Reserva");
        Console.WriteLine("9.\tCancelar Reserva");
        
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

            string resultado = db.CriarSala(nome, andar, assentos);
            Console.WriteLine(resultado);
            if (resultado == "Sala criada com sucesso!")
                Console.WriteLine("Para consultar o ID das salas, escolha a opção 2 no menu.");

            Console.ReadKey();
            Console.Clear();
        }

        else if (opcao == 2)
        {
            Console.Write("Filtrar por nome (ou vazio): ");
            string busca = Console.ReadLine();

            Console.Write("Início do período (dd/mm/aaaa hh:mm): ");
            DateTime inicioPeriodo = DateTime.Parse(Console.ReadLine());

            Console.Write("Fim do período (dd/mm/aaaa hh:mm): ");
            DateTime fimPeriodo = DateTime.Parse(Console.ReadLine());

            if (fimPeriodo <= inicioPeriodo)
            {
                Console.WriteLine("Erro: O fim do período precisa ser maior que o início.");
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                int pagina = 1;
                int tamanhoPagina = 10;

                while (true)
                {
                    Console.Clear();

                    var salas = db.BuscarSalas(busca, pagina, tamanhoPagina);
                    int totalSalas = db.ContarSalas(busca);
                    int totalPaginas = totalSalas == 0 ? 1 : (int)Math.Ceiling(totalSalas / (double)tamanhoPagina);

                    if (!salas.Any())
                    {
                        Console.WriteLine("Nenhuma sala encontrada!");
                        Console.ReadKey();
                        break;
                    }

                    Console.WriteLine($"\nPágina {pagina} de {totalPaginas}");

                    foreach (var s in salas)
                    {
                        double horasLivres = db.CalcularHorasLivres(s.Id, inicioPeriodo, fimPeriodo);

                        Console.WriteLine("\n==============================================\n");
                        Console.WriteLine($"ID: {s.Id}\n{s.Nome}\n{s.Andar}º Andar\nNumero de assentos: {s.QuantidadeAssentos}\nHoras Livres no Período: {horasLivres}h");
                    }

                    Console.WriteLine("\nN.\tPróxima Página");
                    Console.WriteLine("V.\tPágina Anterior");
                    Console.WriteLine("S.\tSair");
                    Console.WriteLine("\nOpção: ");

                    string navegacao = Console.ReadLine().ToUpper();

                    if (navegacao == "N")
                    {
                        if (pagina < totalPaginas) pagina++;
                    }
                    else if (navegacao == "V")
                    {
                        if (pagina > 1) pagina--;
                    }
                    else if (navegacao == "S")
                    {
                        break;
                    }
                }

                Console.Clear();
            }
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

                Console.Write("Novo Nome (ou Enter para manter): ");
                string novoNome = Console.ReadLine();

                Console.Write("Novo Andar: ");
                int novoAndar = int.Parse(Console.ReadLine());

                Console.Write("Nova Quantidade de Assentos: ");
                int novosAssentos = int.Parse(Console.ReadLine());

                Console.WriteLine("\n\tSalvando Alteirações...");

                string resultado = db.AtualizarSala(id, novoNome, novoAndar, novosAssentos);
                Console.WriteLine($"\n{resultado}");
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
                }
            }

            Console.ReadKey();
            Console.Clear();
        }

        else if (opcao == 7)
        {
            Console.Write("ID da Sala (ou vazio): ");
            string entradaSala = Console.ReadLine();

            int? salaId = null;
            if (!string.IsNullOrWhiteSpace(entradaSala))
                salaId = int.Parse(entradaSala);

            Console.Write("Mostrar apenas reservas futuras? (S/N): ");
            bool somenteFuturas = Console.ReadLine().ToUpper() == "S";

            int pagina = 1;
            int tamanhoPagina = 10;

            while (true)
            {
                Console.Clear();

                var reservas = db.BuscarReservas(salaId, somenteFuturas, pagina, tamanhoPagina);
                int totalReservas = db.ContarReservas(salaId, somenteFuturas);
                int totalPaginas = totalReservas == 0 ? 1 : (int)Math.Ceiling(totalReservas / (double)tamanhoPagina);

                if (!reservas.Any())
                {
                    Console.WriteLine("Nenhuma reserva encontrada!");
                    Console.ReadKey();
                    break;
                }

                Console.WriteLine($"\nPágina {pagina} de {totalPaginas}");

                foreach (var r in reservas)
                {
                    Console.WriteLine("\n==============================================\n");
                    Console.WriteLine($"Reserva ID: {r.Id}\nSala: {r.Sala.Nome}\nAndar: {r.Sala.Andar}\nInício: {r.Inicio:dd/MM/yyyy HH:mm}\nFim: {r.Fim:dd/MM/yyyy HH:mm}");
                }

                Console.WriteLine("\nN.\tPróxima Página");
                Console.WriteLine("V.\tPágina Anterior");
                Console.WriteLine("S.\tSair");
                Console.WriteLine("\nOpção: ");

                string navegacao = Console.ReadLine().ToUpper();

                if (navegacao == "N")
                {
                    if (pagina < totalPaginas) pagina++;
                }
                else if (navegacao == "V")
                {
                    if (pagina > 1) pagina--;
                }
                else if (navegacao == "S")
                {
                    break;
                }
            }

            Console.Clear();
        }

        else if (opcao == 8)
        {
            Console.WriteLine("\n---REAGENDAR RESERVA---\n");
            Console.Write("ID da Reserva: ");
            int id = int.Parse(Console.ReadLine());

            var reserva = db.BuscarReservaPorId(id);

            if (reserva == null)
            {
                Console.WriteLine("Reserva não encontrada!");
            }
            else
            {
                Console.WriteLine($"Sala: {reserva.Sala.Nome}");
                Console.WriteLine($"Início Atual: {reserva.Inicio:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Fim Atual: {reserva.Fim:dd/MM/yyyy HH:mm}");

                Console.Write("Novo Início (dd/mm/aaaa hh:mm): ");
                DateTime novoInicio = DateTime.Parse(Console.ReadLine());

                Console.Write("Novo Fim (dd/mm/aaaa hh:mm): ");
                DateTime novoFim = DateTime.Parse(Console.ReadLine());

                string resultado = db.ReagendarReserva(id, novoInicio, novoFim);
                Console.WriteLine(resultado);
            }

            Console.ReadKey();
            Console.Clear();
        }

        else if (opcao == 9)
        {
            Console.WriteLine("\n---CANCELAR RESERVA---\n");
            Console.Write("ID da Reserva: ");
            int id = int.Parse(Console.ReadLine());

            var reserva = db.BuscarReservaPorId(id);

            if (reserva == null)
            {
                Console.WriteLine("Reserva não encontrada!");
            }
            else
            {
                Console.WriteLine($"Sala: {reserva.Sala.Nome}");
                Console.WriteLine($"Início: {reserva.Inicio:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Fim: {reserva.Fim:dd/MM/yyyy HH:mm}");
                Console.WriteLine("Tem certeza que deseja cancelar esta reserva? (S/N)");

                if (Console.ReadLine().ToUpper() == "S")
                {
                    string resultado = db.CancelarReserva(id);
                    Console.WriteLine(resultado);
                }
            }

            Console.ReadKey();
            Console.Clear();
        }

        else if (opcao == 0)
        {
            break;
        }
    }
}