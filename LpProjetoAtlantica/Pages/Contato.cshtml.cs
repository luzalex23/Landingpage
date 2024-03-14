using System;
using System.Net;
using System.Net.Mail;
using MailKit.Net.Imap;
using MimeKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MailKit;

namespace LpProjetoAtlantica.Pages
{
    public class ContatoModel : PageModel
    {
        private readonly ILogger<ContatoModel> _logger;

        public ContatoModel(ILogger<ContatoModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public string Email { get; set; }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Envie um e-mail de confirmação para o usuário inscrito
                    EnviarEmailDeConfirmacao(Email);

                    // Envie uma notificação por e-mail para o administrador
                    EnviarNotificacaoEmail(Email);

                    _logger.LogInformation($"Endereço de e-mail do contato: {Email} inscrito com sucesso.");
                    return RedirectToPage("/Index"); // ou qualquer outra página
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao enviar e-mails: {ex.Message}");
                    // Trate o erro aqui, por exemplo, mostrando uma mensagem de erro na página
                    return Page();
                }
            }
            else
            {
                // ModelState.IsValid é falso, então há erros de validação no modelo
                // Trate os erros de validação aqui, por exemplo, mostrando uma mensagem de erro na página
                return Page();
            }
        }

        private void EnviarEmailDeConfirmacao(string email)
        {
            // Configurações do servidor IMAP
            string imapServer = "imap.titan.email";
            int imapPort = 993;
            string imapUsername = "contato@naturalmentesucesso.com";
            string imapPassword = "naturaladm@232114";

            // Configuração da mensagem de confirmação
            using (var client = new ImapClient())
            {
                client.Connect(imapServer, imapPort, true);
                client.Authenticate(imapUsername, imapPassword);

                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Seu Nome", imapUsername));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Confirmação de Inscrição";
                message.Body = new TextPart("plain")
                {
                    Text = "Você foi inscrito com sucesso!"
                };

                inbox.Append(message);
                client.Disconnect(true);
            }
        }

        private void EnviarNotificacaoEmail(string email)
        {
            // Configurações do servidor SMTP
            string smtpServer = "smtp.titan.com";
            int smtpPort = 465;
            string smtpUsername = "contato@naturalmentesucesso.com";
            string smtpPassword = "naturaladm@232114";

            // Configuração da mensagem de notificação
            using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUsername),
                    Subject = "Nova inscrição recebida",
                    Body = $"Nova inscrição recebida com o e-mail: {email}"
                };

                // Adicione o seu endereço de e-mail como destino da notificação
                mailMessage.To.Add("contato@naturalmentesucesso.com");

                smtpClient.Send(mailMessage);
            }
        }
    }
}
