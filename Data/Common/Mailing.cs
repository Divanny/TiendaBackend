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
using System.Web;
using System.Web.Hosting;
using System.IO;

namespace Data.Common
{
    public class Mailing
    {
        public string genFactura(Pedidos pedido)
        {
            UsuariosRepo usuariosRepo = new UsuariosRepo();
            DireccionesRepo direccionesRepo = new DireccionesRepo();

            UsuariosModel usuario = usuariosRepo.Get(x => x.idUsuario == pedido.idUsuario).FirstOrDefault();
            DireccionesModel direccion = direccionesRepo.Get(x => x.idDireccion == pedido.idDireccion).FirstOrDefault();

            CarritosRepo carritosRepo = new CarritosRepo();
            CarritosModel carrito = carritosRepo.Get(x => x.idCarrito == pedido.idCarrito).FirstOrDefault();


            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\", "site\\wwwroot\\Templates\\FacturaLayout.html");

            //string templatePath = HostingEnvironment.MapPath("~/Data/Templates/FacturaLayout.html");
            //string templatePath = HttpContext.Current.Server.MapPath("~/Data/Templates/FacturaLayout.html");
            string template = System.IO.File.ReadAllText(templatePath);

            string listadoProductos = "";

            foreach (var item in carrito.Productos)
            {
                listadoProductos +=
                    $"<tr style=\"border-collapse:collapse\">" +
                        $"<td style=\"padding:5px 10px 5px 0;Margin:0\" width=\"80%\" align=\"left\">" +
                            $"{item.Nombre}({item.CantidadEnCarrito})" +
                        $"</td>" +
                        $"<td style=\"padding:5px 0;Margin:0\" width=\"20%\" align=\"left\">" +
                            $"${item.Precio.ToString("0.##")}" +
                        $"</td>" +
                    $"</tr>";
            }

            template = Regex.Replace(template, "{{Nombre}}", usuario.NombreUsuario);
            template = Regex.Replace(template, "{{numOrden}}", (pedido.idPedido).ToString());
            template = Regex.Replace(template, "{{Productos}}", listadoProductos);
            template = Regex.Replace(template, "{{Total}}", (pedido.MontoPagado).ToString());
            template = Regex.Replace(template, "{{Direccion}}", direccion.Direccion);
            template = Regex.Replace(template, "{{Ciudad}}", direccion.Ciudad);
            template = Regex.Replace(template, "{{CodigoPostal}}", direccion.CodigoPostal);
            template = Regex.Replace(template, "{{fechaEntrega}}", DateTime.Now.ToLongDateString());
            return template;
        }

        public string genFactura(PedidosModel pedido)
        {
            UsuariosRepo usuariosRepo = new UsuariosRepo();
            DireccionesRepo direccionesRepo = new DireccionesRepo();

            UsuariosModel usuario = usuariosRepo.Get(x => x.idUsuario == pedido.idUsuario).FirstOrDefault();
            DireccionesModel direccion = direccionesRepo.Get(x => x.idDireccion == pedido.idDireccion).FirstOrDefault();

            CarritosRepo carritosRepo = new CarritosRepo();
            CarritosModel carrito = carritosRepo.Get(x => x.idCarrito == pedido.idCarrito).FirstOrDefault();

            //string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\", "site\\wwwroot\\Templates\\FacturaPDFLayout.html");

            string templatePath = HttpContext.Current.Server.MapPath("~/Templates/FacturaPDFLayout.html");
            string template = System.IO.File.ReadAllText(templatePath);

            string listadoProductos = "";

            foreach (var item in carrito.Productos)
            {
                listadoProductos +=
                    $"<tr style=\"border-collapse:collapse\">" +
                        $"<td style=\"padding:5px 10px 5px 0;Margin:0\" width=\"80%\" align=\"left\">" +
                            $"{item.Nombre}({item.CantidadEnCarrito})" +
                        $"</td>" +
                        $"<td style=\"padding:5px 0;Margin:0\" width=\"20%\" align=\"left\">" +
                            $"${item.Precio.ToString("0.##")}" +
                        $"</td>" +
                    $"</tr>";
            }

            template = Regex.Replace(template, "{{Nombre}}", usuario.NombreUsuario);
            template = Regex.Replace(template, "{{numOrden}}", (pedido.idPedido).ToString());
            template = Regex.Replace(template, "{{Productos}}", listadoProductos);
            template = Regex.Replace(template, "{{Total}}", (pedido.MontoPagado).ToString("0.##"));
            template = Regex.Replace(template, "{{Direccion}}", direccion.Direccion);
            template = Regex.Replace(template, "{{Ciudad}}", direccion.Ciudad);
            template = Regex.Replace(template, "{{CodigoPostal}}", direccion.CodigoPostal);
            template = Regex.Replace(template, "{{fechaEntrega}}", DateTime.Now.ToLongDateString());
            return template;
        }

        public void SendFacturaMail(Pedidos pedido)
        {
            string body = genFactura(pedido);

            UsuariosRepo usuariosRepo = new UsuariosRepo();
            UsuariosModel usuario = usuariosRepo.Get(x => x.idUsuario == pedido.idUsuario).FirstOrDefault();

            SendEmail(usuario.CorreoElectronico, "Su pedido ha sido procesado con éxito.", body);
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
