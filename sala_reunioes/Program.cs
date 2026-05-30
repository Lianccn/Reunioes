// using (var db = new AppDbContext())
using System.Data.Common;

{
    // db.Database.EnsureCreated();

    Console.WriteLine("\nAgendar Reuniões\n");
    Console.WriteLine("1\tCadastrar Salas\n");
    Console.WriteLine("2\tListar Salas\n");
    Console.WriteLine("3\tEscolher Sala\n");
    Console.WriteLine("0\tSair\n");

    var opcao = int.Parse(Console.ReadLine());

    while (true)
    {
        if (opcao == 1)
        {
            Console.WriteLine("Numero da Sala: ");
            int numero = int.Parse(Console.ReadLine());
 
            Console.WriteLine("Numero do Andar: ");
            int andar = int.Parse(Console.ReadLine());
 
            Console.WriteLine("Quantidade de Assentos: ");
            int assentos = int.Parse(Console.ReadLine());
 
            db.Salas.Add(new Sala
            {
                Numero = numero,
                Andar = andar,
                QuantidadeAssentos = assentos
            });
            db.SaveChanges();

            Console.WriteLine("Sala Cadastrada")
        }
    }
}


