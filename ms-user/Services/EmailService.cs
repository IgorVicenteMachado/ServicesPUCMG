//using SendGrid;
//using SendGrid.Helpers.Mail;

//namespace ms_user.Services
//{
//    public class EmailService
//    {
//        public async Task Send(string toName, string toEmail, string subject, 
//            string body, 
//            string fromName = "Equipe DevGames", 
//            string fromEmail = "iguinho.jogos10@hotmail.com")
//        {
//            var apiKey = Configuration.SENDGRID_API_KEY;
//            var client = new SendGridClient(apiKey);
//            var msg = new SendGridMessage()
//            {
//                From = new EmailAddress(fromEmail, fromName),
//                Subject = subject,
//                HtmlContent = $"<strong>{body}</strong>"
//            };
//            msg.AddTo(new EmailAddress(toEmail, toName));
//            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
//        }
//    }
//}
