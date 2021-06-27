using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MonitorNotas
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Clear();

                long usuario = 0;
                string senha = "";

                //Credenciais do e-mail que fará o envio das notas
                string usuarioEmail = "";
                string senhaEmail = "";

                string enderecoDestino = "";

                List<Materia> notas = new List<Materia>();
                Email email = new Email(usuarioEmail, senhaEmail);

                while (true)
                {
                    Console.WriteLine("Matrícula Fatec: ");
                    string matriculaStr = Console.ReadLine();
                    if (long.TryParse(matriculaStr, out usuario) && usuario > 10000000)
                        break;
                    Console.WriteLine("Matrícula inválida\n");
                }

                while (true)
                {
                    Console.WriteLine("\nSenha Fatec SAN: ");
                    senha = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(senha))
                        break;
                    Console.WriteLine("Senha inválida\n");
                }

                Console.WriteLine("\n(Opcional) - E-mail para envio das notas:");
                enderecoDestino = Console.ReadLine();

                if (!enderecoDestino.Contains("@") || !enderecoDestino.Contains("."))
                {
                    enderecoDestino = "";
                    Console.WriteLine("\nE-mail inválido. As notas serão exibidas apenas aqui.");
                }
                Console.Clear();

                Crawler crawler = new Crawler(usuario.ToString(), senha);
                notas = crawler.PegarNotas();
                MostrarNotas(notas);

                while (!VerificaNotasLancadas(notas))
                {
                    try
                    {
                        List<Materia> notasAtuais = new List<Materia>();
                        notasAtuais = crawler.PegarNotas();
                        bool atualizado = false;

                        foreach (Materia materia in notasAtuais)
                        {
                            if (!materia.Nota.Equals(notas.FirstOrDefault(m => m.Nome.Equals(materia.Nome)).Nota))
                            {
                                atualizado = true;
                                break;
                            }
                        }
                        Console.WriteLine($"{DateTime.Now:dd/MM/yyyy - HH:mm} - Processado com sucesso - {(atualizado ? "Notas atualizadas" : "Sem atualizações")}.");

                        if (atualizado)
                        {
                            notas = notasAtuais;
                            if (!string.IsNullOrWhiteSpace(enderecoDestino))
                                email.EnviarEmail(notas, enderecoDestino);

                            MostrarNotas(notas);
                        }
                    }
                    catch (LoginException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{DateTime.Now:dd/MM/yyyy - HH:mm} - Falha no processamento: {ex.Message}");
                    }
                    Thread.Sleep(1000 * 60 * 30);
                }
                Console.WriteLine("Todas as notas foram lançadas. Pressione Qualquer tecla para encerrar.");
                Console.ReadKey();
            }
            catch (LoginException)
            {
                Console.WriteLine("RA ou Senha incorreto.");
                Console.ReadKey();
            }
        }

        private static bool VerificaNotasLancadas(List<Materia> materias)
        {
            foreach (Materia materia in materias)
            {
                if (materia.Nota.Contains("lan"))
                    return false;
            }
            return true;
        }

        private static void MostrarNotas(List<Materia> notas)
        {
            Console.WriteLine("=====Notas=====");
            foreach (Materia materia in notas)
            {
                Console.WriteLine($"{materia.Nome}: {materia.Nota}");
            }
            Console.WriteLine("===============\n\n");
        }
    }
}
