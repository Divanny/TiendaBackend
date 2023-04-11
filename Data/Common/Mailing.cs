using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Model.Productos;
using Model.Usuarios;
using Model.Common;
using iTextSharp.tool.xml.html;

namespace Data.Common
{
    public class Mailing
    {
        public string genFactura(PedidosModel pedido, DireccionesModel direccion, UsuariosModel usuariosModel)
        {
            CarritosRepo carritosRepo = new CarritosRepo();
            var carrito = carritosRepo.Get(x => x.idCarrito == pedido.idCarrito).FirstOrDefault();

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string templatePath = System.IO.Path.Combine(basePath, "..\\Data\\Templates\\FacturaLayout.html");
            string template = System.IO.File.ReadAllText(templatePath);

            string listadoProductos = "";

            foreach (var item in carrito.Productos)
            {
                listadoProductos +=
                    $"<tr style=\"border-collapse:collapse\">" +
                        $"<td style=\"padding:5px 10px 5px 0;Margin:0\" width=\"80%\" align=\"left\">" +
                            $"{item.Nombre}" +
                        $"</td>" +
                        $"<td style=\"padding:5px 0;Margin:0\" width=\"20%\" align=\"left\">" +
                            $"{item.Precio}" +
                        $"</td>" +
                    $"</tr>";
            }

            template = Regex.Replace(template, "{{Nombre}}", usuariosModel.NombreUsuario);
            template = Regex.Replace(template, "{{numOrden}}", (pedido.idPedido).ToString());
            template = Regex.Replace(template, "{{Productos}}", listadoProductos);
            template = Regex.Replace(template, "{{Total}}", (pedido.MontoPagado).ToString());
            template = Regex.Replace(template, "{{Direccion}}", direccion.Direccion);
            template = Regex.Replace(template, "{{Ciudad}}", direccion.Ciudad);
            template = Regex.Replace(template, "{{CodigoPostal}}", direccion.CodigoPostal);
            template = Regex.Replace(template, "{{fechaEntrega}}", DateTime.Now.ToLongDateString());
            return template;
        }

        public void SendFacturaMail(PedidosModel pedido, DireccionesModel direccion, UsuariosModel usuariosModel)
        {
            string body = genFactura(pedido, direccion, usuariosModel);
            SendEmail(usuariosModel.CorreoElectronico, "Su pedido ha sido procesado con éxito.", body);
        }

        public void SendEmail(string to, string subject, string body)
        {
            string fromMail = "tiendads3@gmail.com";
            string fromPassword = "zxtmhqsjwwwwhisz";

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage(fromMail, to, subject, body);
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);
        }
    }
}
