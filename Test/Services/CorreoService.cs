using System.Net.Mail;
using Test.Models;

namespace Test.Services {
    public class CorreoService {

        public void enviarCorreo(string mensaje) {
            string EmailOrigen = "pruebas.jc96@gmail.com";
            string EmailDestino = "pruebas.jc96@gmail.com";
            string Contraseña = "Papeleria3";

            MailMessage mailMessage = new MailMessage(EmailOrigen, EmailDestino, "Transacción", "<p>" + mensaje + "</p>");
            mailMessage.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = true;

            smtpClient.Port = 587;
            smtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Contraseña);

            smtpClient.Send(mailMessage);

            smtpClient.Dispose();
        }

    }
}
