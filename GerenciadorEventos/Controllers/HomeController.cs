using GerenciadorEventos.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GerenciadorEventos.Entidades;

namespace GerenciadorEventos.Controllers
{
    public class HomeController : Controller
    {
        private UfRepositorio ufRes;
        private CategoriaRepositorio categoriaRes;
        private UsuarioRepositorio usuarioRes;
        private EventoRepositorio eventoRes;

        public HomeController()
        {
            ufRes = new UfRepositorio();
            categoriaRes = new CategoriaRepositorio();
            usuarioRes = new UsuarioRepositorio();
            eventoRes = new EventoRepositorio();
        }

        public ActionResult Index()
        {
            ViewBag.Estados = ufRes.ListaUf();
            ViewBag.Categorias = categoriaRes.ListaCategoria();

            object pFiltro = new
            {
                uf = "",
                categoria = "",
                nome = ""
            };

            List<Evento> lstEventos = new List<Evento>();
            lstEventos = eventoRes.ListaEventos(pFiltro).Where(m => m.ativo == 'A' && m.dataHoraFim > DateTime.Now).OrderBy(m => m.dataHoraFimInscricao).ToList();

            return View(lstEventos);
        }

        public PartialViewResult FiltrarEventos(FormCollection colecao)
        {
            object pFiltro = new
            {
                uf = colecao["estados"].ToString(),
                categoria = colecao["categoria"].ToString(),
                nome = colecao["nomeEvento"].ToString()
            };

            List<Evento> lstEventos = new List<Evento>();
            lstEventos = eventoRes.ListaEventos(pFiltro).Where(m => m.ativo == 'A' && m.dataHoraFim > DateTime.Now).OrderBy(m => m.dataHoraFimInscricao).ToList();

            return PartialView(lstEventos);
        }

        public ActionResult ExibirImagem(int CodigoEvento)
        {
            Evento evento = eventoRes.BuscarPorCodigo(CodigoEvento); //Pull image from the database.
            if (evento != null)
            {
                if (evento.imagem != null)
                    return File(evento.imagem, "image/png");
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        public PartialViewResult Loading()
        {
            return PartialView("Loading");
        }

        public PartialViewResult Mensagem(int tipo, string titulo, string mensagem)
        {
            ViewBag.Tipo = tipo;
            ViewBag.Titulo = titulo;
            ViewBag.Mensagem = mensagem;

            return PartialView("Mensagem");
        }

        public PartialViewResult MensagemDecisao(int tipo, string titulo, string mensagem)
        {
            ViewBag.Tipo = tipo;
            ViewBag.Titulo = titulo;
            ViewBag.Mensagem = mensagem;

            return PartialView("MensagemDecisao");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}