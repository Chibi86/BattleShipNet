using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace BattleShipNet.Models
{
    public static class MailModel
    {
        public static void SendInviteEmail(string email, string fromName, string url)
        {
            try
            {
                MailAddress toEmail = new MailAddress(email);

                try
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(toEmail);
                    mail.From = new MailAddress("no-reply@battleship.chibidesign.se", "BattleShip.ChibiDesign.se", System.Text.Encoding.UTF8);
                    mail.Subject = "Your friend " + HttpUtility.HtmlEncode(fromName) + " want to play BattleShip with you!";
                    mail.SubjectEncoding = System.Text.Encoding.UTF8;
                    mail.Body = "Your friend " + HttpUtility.HtmlEncode(fromName) + " want to play BattleShip with you!<br /><br />" +
                                "Joina Game: <a href=\"" + url + "\">" + url + "</a>";
                    mail.BodyEncoding = System.Text.Encoding.UTF8;
                    mail.IsBodyHtml = true;
                    mail.Priority = MailPriority.High;
                    SmtpClient client = new SmtpClient();
                    client.Credentials = new NetworkCredential("info@battleship.chibidesign.se", "****");
                    client.Port = 587;
                    client.Host = "smtp01.binero.se";
                    client.EnableSsl = true;
                    client.Send(mail);
                }
                catch
                {
                    throw new Exception("Email-server issues contact site administrator!");
                }
            }
            catch
            {
                throw new Exception("Invalid email input");
            }
        }
    }
}