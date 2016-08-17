using GerenciadorEventos.Entidades;
using GerenciadorEventos.Repositorio;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace GerenciadorEventos.Controllers
{
    public class GerenciarController : Controller
    {
        private InscricaoRepositorio inscricaoRes;
        private EventoRepositorio eventoRes;
        private UsuarioRepositorio usuarioRes;
        private EmailRepositorio emailRes;

        public GerenciarController()
        {
            inscricaoRes = new InscricaoRepositorio();
            eventoRes = new EventoRepositorio();
            usuarioRes = new UsuarioRepositorio();
            emailRes = new EmailRepositorio();
        }

        // GET: Gerenciar
        public ActionResult Index()
        {
            List<Evento> lstEventos = new List<Evento>();
            lstEventos = inscricaoRes.BuscarEventosPorUsuario(((Usuario)Session["usuario"]).codigo);
            return View(lstEventos);
        }

        public JsonResult CancelarInscricao(int CodigoEvento, int CodigoUsuario)
        {
            try
            {
                Usuario usuario = (Usuario)Session["usuario"];

                Inscricao inscricao = new Inscricao();
                if (CodigoUsuario == usuario.codigo)
                {
                    inscricao = usuario.eventosInscritos.Where(m => m.evento.codigo == CodigoEvento).FirstOrDefault();

                    inscricaoRes.CancelarInscricao(inscricao);

                    usuario.eventosInscritos = inscricaoRes.BuscarInscricoesPorUsuario(usuario.codigo);
                    Session["usuario"] = usuario;
                }
                else
                {
                    inscricao.usuario.codigo = CodigoUsuario;
                    inscricao.evento.codigo = CodigoEvento;
                    inscricao.dataCadastro = DateTime.Now;

                    inscricaoRes.CancelarInscricaoOutroUsuario(inscricao);
                }
                

                return Json(new { act = 1, mensagem = "Inscrição cancelada com sucesso." }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { act = 2, mensagem = "Erro ao cancelar a inscrição. Motivo: "+ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult VisualizarInscritos(int CodigoEvento)
        {
            List<Usuario> lstUsuario = new List<Usuario>();
            lstUsuario = inscricaoRes.ListaUsuariosInscritosPorEvento(CodigoEvento);

            ViewBag.Evento = eventoRes.BuscarPorCodigo(CodigoEvento);

            return View(lstUsuario);
        }

        public ActionResult ListaPresenca(int CodigoEvento)
        {
            try
            {
                List<Usuario> lstUsuario = new List<Usuario>();
                lstUsuario = inscricaoRes.ListaUsuariosInscritosPorEvento(CodigoEvento).Where(m => m.statusInscricao != "Cancelada").ToList();

                ViewToPdf viewpdf = new ViewToPdf();
                viewpdf.lstUsuario = lstUsuario;
                viewpdf.evento = eventoRes.BuscarPorCodigo(CodigoEvento);

                //string nomearquivo = "ListaPresenca.pdf";

                //var pdf = new ViewAsPdf
                //{
                //    ViewName = "ListaPresenca",
                //    Model = lstUsuario,
                //    FileName = nomearquivo,
                //    PageSize = Size.A4,
                //    //IsBackgroundDisabled = false,
                //    //WkhtmltopdfPath = @"C:\inetpub\wwwroot\neo_siteexecutor\Rotativa",
                //};

                //Busca o model
                //var model = new { Nome = "Penihel" };

                string htmlText = RenderViewToString("ListaPresenca", viewpdf);

                byte[] buffer = RenderPDF(htmlText, true);

                return File(buffer, "application/PDF");

                //return pdf;
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult AtribuirPresencaView(int CodigoEvento)
        {
            List<Usuario> lstUsuario = new List<Usuario>();
            lstUsuario = inscricaoRes.ListaUsuariosInscritosPorEvento(CodigoEvento).Where(m => m.statusInscricao != "Cancelada").ToList();

            ViewBag.Evento = eventoRes.BuscarPorCodigo(CodigoEvento);

            return View("VisualizarInscritos", lstUsuario);
        }

        public JsonResult AtribuirPresenca(int CodigoEvento, int CodigoUsuario, string Atributo)
        {
            try
            {
                Inscricao inscricao = new Inscricao();
                inscricao.usuario.codigo = CodigoUsuario;
                inscricao.evento.codigo = CodigoEvento;
                inscricao.statusPresenca = Atributo;
                inscricao.dataCadastro = DateTime.Now;

                inscricaoRes.AlterarStatusInscricao(inscricao);


                return Json(new { act = 1, mensagem = "Alteração do status realizado com sucesso." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { act = 2, mensagem = "Erro ao alterar status da inscrição. Motivo: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GerarCracha(int CodigoEvento)
        {
            try
            {
                List<Usuario> lstUsuario = new List<Usuario>();
                lstUsuario = eventoRes.BuscaPalestrantesEvento(CodigoEvento);
                lstUsuario.Add(eventoRes.BuscarOrganizadorEvento(CodigoEvento));

                ViewToPdf viewpdf = new ViewToPdf();
                viewpdf.lstUsuario = lstUsuario;
                viewpdf.evento = eventoRes.BuscarPorCodigo(CodigoEvento);

                //ViewBag.Evento = eventoRes.BuscarPorCodigo(CodigoEvento);

                //string nomearquivo = "Cracha.pdf";

                //var pdf = new ViewAsPdf
                //{
                //    ViewName = "GerarCracha",
                //    Model = lstUsuario,
                //    FileName = nomearquivo,
                //    PageSize = Size.A4,
                //    //IsBackgroundDisabled = false,
                //    //WkhtmltopdfPath = @"C:\inetpub\wwwroot\neo_siteexecutor\Rotativa",
                //};

                //return pdf;

                string htmlText = RenderViewToString("GerarCracha", viewpdf);

                byte[] buffer = RenderPDF(htmlText,true);

                return File(buffer, "application/PDF");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult GerarCertificado(int CodigoEvento)
        {
            try
            {
                List<Usuario> lstUsuario = new List<Usuario>();
                lstUsuario = inscricaoRes.ListaUsuariosInscritosPorEvento(CodigoEvento).Where(m => m.statusInscricao == "Presente").ToList();
                lstUsuario.AddRange(eventoRes.BuscaPalestrantesEvento(CodigoEvento));

                ViewToPdf viewpdf = new ViewToPdf();
                viewpdf.lstUsuario = lstUsuario;
                viewpdf.evento = eventoRes.BuscarPorCodigo(CodigoEvento);

                //ViewBag.Evento = eventoRes.BuscarPorCodigo(CodigoEvento);

                //string nomearquivo = "Certificados.pdf";

                //var pdf = new ViewAsPdf
                //{
                //    ViewName = "GerarCertificado",
                //    Model = lstUsuario,
                //    FileName = nomearquivo,
                //    PageOrientation = Orientation.Landscape,
                //    PageMargins = new Margins(0, 0, 0, 0),
                //    PageSize = Size.A4,
                //    IsGrayScale = true,
                //    //IsBackgroundDisabled = false
                //    //WkhtmltopdfPath = @"C:\inetpub\wwwroot\neo_siteexecutor\Rotativa",
                //};

                //return pdf;

                string htmlText = RenderViewToString("GerarCertificado", viewpdf);

                byte[] buffer = RenderPDF(htmlText, false);

                return File(buffer, "application/PDF");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult GerarCertificadoUsuario(int CodigoEvento, int CodigoUsuario)
        {
            try
            {
                List<Usuario> lstUsuario = new List<Usuario>();
                lstUsuario = inscricaoRes.ListaUsuariosInscritosPorEvento(CodigoEvento).Where(m => m.statusInscricao == "Presente" && m.codigo == CodigoUsuario).ToList();

                ViewToPdf viewpdf = new ViewToPdf();
                viewpdf.lstUsuario = lstUsuario;
                viewpdf.evento = eventoRes.BuscarPorCodigo(CodigoEvento);

                //ViewBag.Evento = eventoRes.BuscarPorCodigo(CodigoEvento);

                //string nomearquivo = "Certificado.pdf";

                //var pdf = new ViewAsPdf
                //{
                //    ViewName = "GerarCertificado",
                //    Model = lstUsuario,
                //    FileName = nomearquivo,
                //    PageOrientation = Orientation.Landscape,
                //    PageMargins = new Margins(0, 0, 0, 0),
                //    PageSize = Size.A4,
                //    IsGrayScale = true,
                //    //IsBackgroundDisabled = false,
                //    //WkhtmltopdfPath = @"C:\inetpub\wwwroot\neo_siteexecutor\Rotativa",
                //};

                //return pdf;

                string htmlText = RenderViewToString("GerarCertificado", viewpdf);

                byte[] buffer = RenderPDF(htmlText, false);

                return File(buffer, "application/PDF");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult EscreverEmail(int CodigoEvento)
        {
            Evento evento = new Evento();
            evento = eventoRes.BuscarPorCodigo(CodigoEvento);

            Email email = new Email();
            email.evento = evento;
            email.usuario = (Usuario)Session["usuario"];

            return View(email);
        }

        public JsonResult EnviarEmail(Email email)
        {
            try
            {
                List<Usuario> lstUsuario = new List<Usuario>();
                lstUsuario = inscricaoRes.ListaUsuariosInscritosPorEvento(email.evento.codigo).Where(m => m.statusInscricao != "Cancelada").ToList();
                lstUsuario.AddRange(eventoRes.BuscaPalestrantesEvento(email.evento.codigo));

                string destinatarios = "";
                string mensagemEmail = "";

                //enviar
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("gerenciadoreventospuc@gmail.com");
                    foreach (Usuario usuario in lstUsuario)
                    {
                        destinatarios += usuario.email + ";";

                        mail.To.Add(usuario.email); // para
                    }

                    mail.Subject = email.titulo; // assunto


                    mensagemEmail = "<!DOCTYPE html> " +
                                            "<html> " +
                                            "<head> " +
                                                "<meta charset=''utf-8''> " +
                                                "<title></title> " +
                                                "<style type=''text/css''> " +
                                                    "a:link, a:active{ " +
                                                    "text-decoration:none; " +
                                                    "color:#004B97; " +
                                                    "} " +
                                                    ".dvConteudo{ " +
                                                    "width:500px; " +
                                                "min-height:130px; " +
                                                "font-family: Calibri; " +
                                                    "font-size:12px; " +
                                                    "} " +
                                                    ".dvImg{ " +
                                                    "width:15%; " +
                                                    "float:left; " +
                                                    "padding-top:4%; " +
                                                    "padding-left:3%; " +
                                                    "} " +
                                                    ".espacotabela{ " +
                                                    "padding-top:20px; " +
                                                    "} " +
                                                    ".dvTexto{ " +
                                                    "border-left: solid 2px #004B97; " +
                                                    "width:76%; " +
                                                    "float:Left; " +
                                                    "padding-left:3%; " +
                                                    "} " +
                                                    ".dvLinha{ " +
                                                    "float:left; " +
                                                    "width:100%; " +
                                                    "margin-top:10px; " +
                                                    "border-top:solid 8px #004B97; " +
                                                    "} " +
                                                    ".lblNome{ " +
                                                    "font-weight:bold; " +
                                                    "color:#004B97; " +
                                                    "} " +
                                                    ".lblTxt{ " +
                                                    "color:#004B97; " +
                                                    "} " +
                                                    ".lblSite{ " +
                                                    "font-weight:bold; " +
                                                    "color:#004B97; " +
                                                    "} " +
                                                    "p{ " +
                                                        "margin:0; " +
                                                    "} " +
                                                "</style> " +
                                            "</head> " +
                                            "<body> " +

                                            "<center> "+
                                            " <table style='width:100%;'> " +
                                                " <tr style='color:#0094ff;'> "+
                                                " <td style='width:60%;'> "+
                                                    " <h1>Gerenciador de Eventos</h1> "+
                                                " </td> "+
                                                " <td> "+
                                                    " <h2>"+email.titulo+"</h2> "+
                                                " </td> "+
                                                " </tr> "+
                                                " <tr> "+
                                                " <td colspan=2 style='background-color:#0094ff;'> "+
                                                " </td> "+
                                                " </tr> "+
                                                " <tr>"+
                                                " <td colspan=2>"+email.mensagem+"</td> " +
                                                "</tr> " +
                                                "</table> "+
                                                "</center> "+

                                            "</body> " +
                                    "</html>";


                    mail.Body = mensagemEmail; // mensagem
                    mail.IsBodyHtml = true;

                    // em caso de anexos
                    //mail.Attachments.Add(new Attachment(@"C:\teste.txt"));

                    using (var smtp = new SmtpClient("smtp.gmail.com"))
                    {
                        smtp.EnableSsl = true; // GMail requer SSL
                        smtp.Port = 587;       // porta para SSL
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network; // modo de envio
                        smtp.UseDefaultCredentials = false; // vamos utilizar credencias especificas

                        // seu usuário e senha para autenticação
                        smtp.Credentials = new NetworkCredential("gerenciadoreventospuc@gmail.com", "375i4u9E");

                        // envia o e-mail
                        smtp.Send(mail);
                    }
                }

                email.destinatario = destinatarios;
                email.dataEnvio = DateTime.Now;

                //inserir
                emailRes.RegistrarEmail(email);

                return Json(new { act = 1, mensagem = "E-mail(s) enviado(s) com sucesso." }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { act = 2, mensagem = "Ocorreu um erro ao enviar o(s) e-mail(s). Motivo: "+ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Relatorios(int CodigoEvento)
        {
            List<Usuario> lstUsuario = new List<Usuario>();
            lstUsuario = inscricaoRes.ListaUsuariosInscritosPorEvento(CodigoEvento);

            Evento evento = new Evento();
            evento = eventoRes.BuscarPorCodigo(CodigoEvento);

            ViewBag.Evento = evento;

            return View(lstUsuario);
        }

        public ActionResult LiberarAcesso()
        {
            List<Usuario> lstUsuario = new List<Usuario>();
            lstUsuario = usuarioRes.ListaUsuarios().Where(m => m.liberado == 'N').ToList();

            return View(lstUsuario);
        }

        public JsonResult AcaoLiberarAcesso(int CodigoUsuario)
        {
            try
            {
                usuarioRes.LiberarAcesso(CodigoUsuario);

                return Json(new { act = 1, mensagem = "Usuário liberado com sucesso." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { act = 2, mensagem = "Erro ao alterar status do usuário. Motivo: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private string RenderViewToString(string viewName, object viewData)
        {
            var renderedView = new StringBuilder();

            var controller = this;


            using (var responseWriter = new StringWriter(renderedView))
            {
                var fakeResponse = new HttpResponse(responseWriter);

                var fakeContext = new HttpContext(System.Web.HttpContext.Current.Request, fakeResponse);

                var fakeControllerContext = new ControllerContext(new HttpContextWrapper(fakeContext), controller.ControllerContext.RouteData,
                    controller.ControllerContext.Controller);

                var oldContext = System.Web.HttpContext.Current;
                System.Web.HttpContext.Current = fakeContext;

                using (var viewPage = new ViewPage())
                {
                    var viewContext = new ViewContext(fakeControllerContext, new FakeView(), new ViewDataDictionary(), new TempDataDictionary(), responseWriter);

                    var html = new HtmlHelper(viewContext, viewPage);
                    html.RenderPartial(viewName, viewData);
                    System.Web.HttpContext.Current = oldContext;
                }
            }

            return renderedView.ToString();
        }

        private byte[] RenderPDF(string htmlText, bool margem)
        {
            byte[] renderedBuffer;

            int HorizontalMargin = 0;
            int VerticalMargin = 0;

            if (margem)
            {
                HorizontalMargin = 40;
                VerticalMargin = 40;
            }
            

            using (var outputMemoryStream = new MemoryStream())
            {
                using (var pdfDocument = new Document(PageSize.A4, HorizontalMargin, HorizontalMargin, VerticalMargin, VerticalMargin))
                {
                    iTextSharp.text.pdf.PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDocument, outputMemoryStream);
                    pdfWriter.CloseStream = false;

                    pdfDocument.Open();
                    using (var htmlViewReader = new StringReader(htmlText))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(pdfWriter, pdfDocument, htmlViewReader);
                    }

                }

                renderedBuffer = new byte[outputMemoryStream.Position];
                outputMemoryStream.Position = 0;
                outputMemoryStream.Read(renderedBuffer, 0, renderedBuffer.Length);
            }

            return renderedBuffer;
        }

    }

    public class FakeView : IView
    {
        #region IView Members

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}