using MovieStore.Domain.Abstract;
using MovieStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MovieStore.Domain.Concrete {
    public class EmailSettings {
        public string MailToAddress = "sandervanbelleghem@gmail.com";
        public string MailFromAddress = "sandervanbelleghem@gmail.com";
        public bool UseSsl = true;
        public string Username = "sandervanbelleghem@gmail.com";
        public string Password = "";
        public string ServerName = "Smtp.Gmail.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"c:\movie_store_emails";
    }

    public class EmailOrderProcessor : IOrderProcessor {
        private EmailSettings emailSettings;
        private IOrderRepository orderRepo;
        
        public EmailOrderProcessor(EmailSettings settings) {
            emailSettings = settings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingInfo) {

            // Save to database!
            foreach (var line in cart.Lines) {

                Order order = new Order();
                order.MovieID = line.Movie.MovieID;
                order.Name = line.Movie.Name;
                order.Price = line.Movie.Price;
                order.Category = line.Movie.Category;
                order.Quantity = line.Quantity;

                orderRepo.SaveOrder(order);
            }

            using (var smtpClient = new SmtpClient()) {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if (emailSettings.WriteAsFile) {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder body = new StringBuilder()
                .AppendLine("A new order has been submitted")
                .AppendLine("---")
                .AppendLine("Items:");

                foreach (var line in cart.Lines) {
                    var subtotal = line.Movie.Price * line.Quantity;
                    body.AppendFormat("{0} x {1} (subtotal: {2:c}", line.Quantity, line.Movie.Name, subtotal);
                }

                body.AppendFormat("Total order value: {0:c}",
                cart.ComputeTotalValue())
                .AppendLine("---")
                .AppendLine("Ship to:")
                .AppendLine(shippingInfo.Name)
                .AppendLine(shippingInfo.Line1)
                .AppendLine(shippingInfo.Line2 ?? "")
                .AppendLine(shippingInfo.Line3 ?? "")
                .AppendLine(shippingInfo.City)
                .AppendLine(shippingInfo.State ?? "")
                .AppendLine(shippingInfo.Country)
                .AppendLine(shippingInfo.Zip)
                .AppendLine("---");

                MailMessage mailMessage = new MailMessage (  emailSettings.MailFromAddress, emailSettings.MailToAddress, "New order submitted!", body.ToString());

                if (emailSettings.WriteAsFile) {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                //smtpClient.Send(mailMessage);
            }
        }
    }
}
