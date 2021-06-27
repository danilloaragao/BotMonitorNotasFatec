using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace MonitorNotas
{
    public class Email
    {
        private string _usuario { get; set; }
        private string _senha { get; set; }

        public Email(string usuario, string senha)
        {
            this._usuario = usuario;
            this._senha = senha;
        }
        public void EnviarEmail(List<Materia> materias, string enderecoDestino)
        {
            try
            {
                var fromAddress = new MailAddress(this._usuario, "Envio Automático");
                var toAddress = new MailAddress(enderecoDestino);
                string fromPassword = this._senha;
                string subject = "Atualização das notas no portal";
                string body = "";

                foreach (Materia materia in materias)
                {
                    body += $"{materia.Nome} -> {materia.Nota.LimparNota()}\n";
                }

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Não foi possível enviar o e-mail: {ex.Message}");
            }
        }
    }
}
