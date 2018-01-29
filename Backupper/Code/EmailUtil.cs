using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net;
using log4net;

namespace Base.Configuration
{
    /// <summary>
    /// Utilità per invio mail
    /// </summary>
    public class EmailUtil
    {


        /// <summary>
        /// Invia una mail
        /// </summary>
        /// <param name="senderAddress">Indirizzo del mittente</param>
        /// <param name="name">Il nome da mostrare come sender</param>
        /// <param name="recipientAddress">Destinatario</param>
        /// <param name="subject">Oggetto del messaggio</param>
        /// <param name="body">Body del messaggio</param>
        /// <param name="sendAsHtml">True se usa il formato html per il body</param>
        /// <param name="replyTo">L'indirizzo mail per la risposta</param>
        /// <param name="attachments">La lista di allegati come percorsi assoluti</param>
        /// <param name="ccAddresses">La lista degli indirizzi cc.</param>
        /// <param name="ccnAddresses">La lista degli indirizzi ccn.</param>
        /// <returns>True se l'invio ha avuto esito positivo</returns>
        public static EmailSendResult Send(ILog logger, string smtpServer, string smtpUsername, string smtpPassword, string senderAddress, string name, string recipientAddress, string subject, string body, bool sendAsHtml, string replyTo, IList<string> attachments, string ccAddresses, string ccnAddresses)
        {
            recipientAddress = recipientAddress.Trim();
            if (!String.IsNullOrEmpty(ccAddresses))
            {
                ccAddresses = ccAddresses.Trim();
            }

            if (!String.IsNullOrEmpty(ccnAddresses))
            {
                ccnAddresses = ccnAddresses.Trim();
            }

            String smtpSenderEmail = senderAddress;

            if (String.IsNullOrEmpty(smtpServer))
                return EmailSendResult.CreateOk(); // email disabilitata

            MailMessage mail = new MailMessage();
            SmtpClient smtp = new SmtpClient();

            logger.Info("Sending email to " + recipientAddress);

            if (!String.IsNullOrEmpty(ccAddresses))
            {
                logger.Info("Sending email (cc) to " + ccAddresses);
            }

            if (!String.IsNullOrEmpty(ccnAddresses))
            {
                logger.Info("Sending email (ccn) to " + ccnAddresses);
            }

            logger.Debug("Email subject is: " + subject);

            try
            {

                smtp.Host = smtpServer;
                if (ApplicationConfigSettings.SmtpPort > 0)
                    smtp.Port = ApplicationConfigSettings.SmtpPort;
                smtp.EnableSsl = ApplicationConfigSettings.SmtpEnableSsl;
                logger.Info("smtp.Host = " + smtpServer);
                logger.Info("smtp.EnableSsl = " + ApplicationConfigSettings.SmtpEnableSsl);
                if (!String.IsNullOrEmpty(smtpUsername))
                {
                    smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    logger.Info("NetworkCredential: Username = " + smtpUsername + ";pwd = " + smtpPassword);
                }
                MailAddress address = new MailAddress(smtpSenderEmail, name);

                mail.From = address;
                mail.To.Clear();
                mail.To.Add(recipientAddress);

                if (!String.IsNullOrEmpty(ccAddresses))
                {
                    mail.CC.Clear();
                    mail.CC.Add(ccAddresses);
                }

                if (!String.IsNullOrEmpty(ccnAddresses))
                {
                    mail.Bcc.Clear();
                    mail.Bcc.Add(ccnAddresses);
                }

                mail.IsBodyHtml = sendAsHtml;

                if (!String.IsNullOrEmpty(replyTo))
                {
                    mail.ReplyTo = new MailAddress(replyTo);
                }

                mail.Subject = subject;
                mail.Body = body;
                mail.BodyEncoding = System.Text.Encoding.GetEncoding("iso-8859-1");

                if (attachments != null)
                {
                    foreach (string attachment in attachments)
                    {
                        mail.Attachments.Add(new Attachment(attachment));
                    }
                }

                smtp.Send(mail);
                // per cancellare allegati
                mail.Dispose();
                logger.Info("Email sent to " + recipientAddress);

                return EmailSendResult.CreateOk();
            }
            catch (Exception ex)
            {
                logger.Fatal(String.Format("An error occurred while sending an email to <{0}>", recipientAddress), ex);
                return EmailSendResult.CreateError(ex);
            }

        }

    }
}
