using MimeKit;
using MailKit.Net.Smtp;
using System.Text;
using System.Data;

namespace MedicineSheet
{
    public static class EmailSender
    {
        public static bool SendExpiredMedicineEmail(IEnumerable<ExpiredMedicine> expiredMedicines) 
        {
            bool wasEmailSuccess = true;
            using var smtp = new SmtpClient();

            try {
                using var email = new MimeMessage();

                email.From.Add(new MailboxAddress("Medicine App", "familia.petitbarragan@gmail.com"));
                email.To.Add(new MailboxAddress("Familia", "familia.petitbarragan@gmail.com"));

                email.Subject = BuildSubject(expiredMedicines);

                var builder = new BodyBuilder() {
                    HtmlBody = BuildEmailBody(expiredMedicines)
                };

                email.Body = builder.ToMessageBody();
        
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                // Note: only needed if the SMTP server requires authentication
                smtp.Authenticate("picho.ep@gmail.com", "gzvk waxl kbse nopl");

                smtp.Send(email);
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
                wasEmailSuccess = false;
            }
            finally {
                smtp.Disconnect(true);
            }
            
            return wasEmailSuccess;
        }

        private static string BuildSubject(IEnumerable<ExpiredMedicine> expiredMedicines){
                

                StringBuilder subject = new StringBuilder();

                string subjectFirstPart = DateTime.Now.ToString("MMM-yyyy");
                string subjectSecondPart = expiredMedicines.Count() > 1 
                    ? "Some Medicines have expired" 
                    : "A Medicine has expired"; 

                subject.Append(subjectFirstPart);
                subject.Append(" - ");
                subject.Append(subjectSecondPart);

                return subject.ToString();
            
        }

        private static string BuildEmailBody(IEnumerable<ExpiredMedicine> expiredMedicines){

            StringBuilder htmlTemplate = new StringBuilder();

            string myLink = string.Format(@"<a href='{0}'>{1}</a>", @"https://docs.google.com/spreadsheets/d/1LyX9XXCI_raCMoFtp-T0-6-VmpMdTmo5Sj8PY5hmcHw/edit#gid=0", @"Google Spreadsheet");

            htmlTemplate.Append(@"<!DOCTYPE html>
                            <html lang=""en"">
                            <head>
                                <style>
                                    .card {
                                        border-radius: 25px;
                                        background-color: lightblue;
                                        padding: 20px;
                                        margin-bottom: 15px;
                                    }
                                </style>
                            </head>
                            <body style=""margin:0;padding:0;"">
                                <h3>Medicine Notification!</h3>
                                <p>These Medicines have expired, go and check the ");
            htmlTemplate.Append(myLink);                   
            htmlTemplate.Append(".<p>");
            htmlTemplate.Append(CreateHtmlList(expiredMedicines));
            htmlTemplate.Append(@" <p>Good luck!</p>
                            <p>i love you :) !</p>
                            </body>
                            </html>");

            return htmlTemplate.ToString();

        }

        private static string CreateHtmlList(IEnumerable<ExpiredMedicine> expiredMedicines) {

            StringBuilder sb = new StringBuilder();

            foreach(ExpiredMedicine expiredMedicine in expiredMedicines){
                sb.AppendLine($"<div class=\"card\">");
                sb.AppendLine($"<div>The medicine {expiredMedicine.Name} has expired.</div>");
                sb.AppendLine($"<div>On the date {expiredMedicine.ExpirationDate.ToString("MM/yyyy")}</div>");
                sb.AppendLine($"</div>");
            }
                

            return sb.ToString();
        }
    }
}