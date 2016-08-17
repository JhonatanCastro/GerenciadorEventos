using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using GerenciadorEventos.Entidades;
using GerenciadorEventos.Repositorio;
using System.Web.Security;

namespace GerenciadorEventos.Controllers
{
    public class AcessoController : Controller
    {
        private UsuarioRepositorio usuarioRes;
        private NivelAcessoRepositorio nivelRes;
        private UfRepositorio ufRes;

        public AcessoController()
        {
            usuarioRes = new UsuarioRepositorio();
            nivelRes = new NivelAcessoRepositorio();
            ufRes = new UfRepositorio();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Registrar()
        {
            ViewBag.NivelAcesso = nivelRes.ListaNivelAcesso();
            ViewBag.UF = ufRes.ListaUf();

            return View();
        }

        [HttpPost]
        public ActionResult SalvarCadastro(Usuario usuario)
        {
            if (usuario.codigo == 0)
            {
                usuario.ativo = 'A';
                if (usuario.nivelAcesso.codigo != 1)
                {
                    usuario.liberado = 'N';
                }
                else
                {
                    usuario.liberado = 'S';
                }
                usuarioRes.Salvar(usuario);
                FormsAuthentication.SetAuthCookie(usuario.nome, usuario.LembreMe);
                //GerenciaSession.Usuario = usuario.id.ToString();
                if (usuario.nivelAcesso.codigo != 1)
                {
                    return RedirectToAction("ConfirmaCadastro");
                }
                else
                {
                    return RedirectToAction("Index","Home");
                }
            }
            else
            {
                FormsAuthentication.SetAuthCookie(usuario.nome, usuario.LembreMe);
                usuarioRes.Salvar(usuario);
                return RedirectToAction("Index","Home");
            }
        }

        public ActionResult ConfirmaCadastro()
        {
            return View();
        }

        public ActionResult Entrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Entrar(Usuario usuario)
        {
            bool lembrar = usuario.LembreMe;
            usuario = usuarioRes.Autenticacao(usuario);
            if (usuario == null)
            {
                ModelState.AddModelError("", "O usuário e/ou a senha está incorreto.");
                return View(usuario);
            }
            usuario.LembreMe = lembrar;
            FormsAuthentication.SetAuthCookie(usuario.nome, usuario.LembreMe);
            Session["usuario"] = usuario;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult GerenciarUsuario(int codigoUsuario)
        {
            Usuario usuario = new Usuario();
            usuario = usuarioRes.BuscarPorCodigo(codigoUsuario);

            ViewBag.NivelAcesso = nivelRes.ListaNivelAcesso();
            ViewBag.UF = ufRes.ListaUf();

            return View("Registrar", usuario);
        }
        
        public ActionResult Sair()
        {
            Session["usuario"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}