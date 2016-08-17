using GerenciadorEventos.Entidades;
using GerenciadorEventos.Repositorio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GerenciadorEventos.Controllers
{
    public class EventosController : Controller
    {
        private UfRepositorio ufRes;
        private CategoriaRepositorio categoriaRes;
        private UsuarioRepositorio usuarioRes;
        private EventoRepositorio eventoRes;
        private InscricaoRepositorio inscricaoRes;
        private ComentarioRepositorio comentarioRes;

        public EventosController()
        {
            ufRes = new UfRepositorio();
            categoriaRes = new CategoriaRepositorio();
            usuarioRes = new UsuarioRepositorio();
            eventoRes = new EventoRepositorio();
            inscricaoRes = new InscricaoRepositorio();
            comentarioRes = new ComentarioRepositorio();
        }

        // GET: Eventos
        public ActionResult Index()
        {
            object pFiltro = new
            {
                uf = "",
                categoria = "",
                nome = ""
            };

            Usuario usuario = (Usuario)Session["usuario"];

            List<Evento> lstEventos = new List<Evento>();
            if (usuario.nivelAcesso.codigo == 4)
            {
                
                lstEventos = eventoRes.ListaEventos(pFiltro).OrderBy(m => m.dataHoraFimInscricao).ToList();
            }
            else
            {
                lstEventos = eventoRes.ListaEventos(pFiltro).Where(m => m.organizador.codigo == usuario.codigo).OrderBy(m => m.dataHoraFimInscricao).ToList();
            }

            return View(lstEventos);
        }
        
        public ActionResult CadastroEvento()
        {
            ViewBag.Estados = ufRes.ListaUf();
            ViewBag.Categorias = categoriaRes.ListaCategoria();
            ViewBag.Palestrantes = usuarioRes.ListaUsuarios().Where(m => m.nivelAcesso.codigo == 2 && m.liberado == 'S').ToList();

            Session["palestrantes"] = null;
            Session["videos"] = null;
            Session["conteudo"] = null;

            Evento evento = new Evento();

            evento.organizador.codigo = ((Usuario)Session["usuario"]).codigo;

            return View(evento);
        }

        public JsonResult SalvarEvento(Evento evento)
        {
            try
            {
                if (evento.dataHoraInicioInscricao >= evento.dataHoraFimInscricao)
                {
                    return Json(new { act = 0, mensagem = "A data do inicio da inscrição é maior que a data do fim da inscrição." }, JsonRequestBehavior.AllowGet);
                }
                if (evento.dataHoraFimInscricao >= evento.dataHoraInicio)
                {
                    return Json(new { act = 0, mensagem = "A data do fim da inscrição é maior que a data do inicio do evento." }, JsonRequestBehavior.AllowGet);
                }
                if (evento.dataHoraInicio >= evento.dataHoraFim)
                {
                    return Json(new { act = 0, mensagem = "A data do inicio do evento é maior que a data do fim do evento." }, JsonRequestBehavior.AllowGet);
                }

                List<Usuario> lstPalestrantes = (List<Usuario>)Session["palestrantes"];
                if (lstPalestrantes == null)
                {
                    lstPalestrantes = new List<Usuario>();
                }
                List<Video> lstVideos = (List<Video>)Session["videos"];
                if (lstVideos == null)
                {
                    lstVideos = new List<Video>();
                }
                List<Conteudo> lstConteudos = (List<Conteudo>)Session["conteudo"];
                if (lstConteudos == null)
                {
                    lstConteudos = new List<Conteudo>();
                }

                string strDir = Path.Combine(Server.MapPath("~/Content/anexos/" + ((Usuario)Session["usuario"]).codigo.ToString()));
                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(strDir);

                //Recupera a imagem de capa
                if (dirInfo.Exists)
                {
                    foreach (System.IO.FileInfo item in dirInfo.GetFiles())
                    {
                        byte[] arquivo = new byte[item.Length];
                        System.IO.FileStream arq = item.OpenRead();
                        arq.Read(arquivo, 0, int.Parse(arq.Length.ToString()));
                        arq.Close();

                        if (item.Name == evento.nomeImagem)
                        {
                            evento.imagem = arquivo;
                        }
                    }
                }
                
                //Recupera o conteudo
                foreach (Conteudo conteudo in lstConteudos)
                {
                    if (dirInfo.Exists)
                    {
                        foreach (System.IO.FileInfo item in dirInfo.GetFiles())
                        {
                            byte[] arquivo = new byte[item.Length];
                            System.IO.FileStream arq = item.OpenRead();
                            arq.Read(arquivo, 0, int.Parse(arq.Length.ToString()));
                            arq.Close();

                            if (item.Name == conteudo.nomeAnexo)
                            {
                                conteudo.anexo = arquivo;
                            }
                        }
                    }
                }

                evento.palestrante = lstPalestrantes;
                evento.videos = lstVideos;
                evento.conteudo = lstConteudos;
                evento.ativo = 'A';

                if (!evento.pago)
                {
                    evento.valor = 0;
                }

                eventoRes.Salvar(evento);

                //Apagar Imagens do diretorio
                if (System.IO.Directory.Exists(dirInfo.FullName))
                {
                    foreach (System.IO.FileInfo item in dirInfo.GetFiles())
                    {
                        item.Delete();
                    }
                }

                return Json(new { act = 1, mensagem = "Evento salvo com sucesso." }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { act = 0, mensagem = "Não foi possível salvar o evento. Motivo: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UploadImagem()
        {
            try
            {
                string strDir = Path.Combine(Server.MapPath("~/Content/anexos/" + ((Usuario)Session["usuario"]).codigo.ToString()));
                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(strDir);

                if (!System.IO.Directory.Exists(dirInfo.FullName))
                {
                    Directory.CreateDirectory(strDir);
                }

                HttpPostedFileBase file = Request.Files["files"];

                string extension = Path.GetExtension(file.FileName);

                if (extension.ToUpper() == ".PNG")
                {
                    string caminho = Path.Combine(strDir, Path.GetFileName(file.FileName));

                    //foreach (System.IO.FileInfo item in dirInfo.GetFiles())
                    //{
                    //    if (item.Name == file.FileName)
                    //    {
                    //        throw new Exception("O anexo já existe no diretorio!");
                    //    }
                    //}

                    file.SaveAs(caminho);

                    ViewBag.NomeImagem = file.FileName;

                    return PartialView();
                }
                else
                {
                    throw new Exception("A imagem não está no formato png!");
                }
            }
            catch (Exception ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Content(ex.Message);
            }

        }

        public FileResult DownloadImagem(string nomeImagem)
        {
            string strDir = Path.Combine(Server.MapPath("~/Content/anexos/" + ((Usuario)Session["usuario"]).codigo.ToString()));
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(strDir);

            string nomeImg = "";
            byte[] anexoImg = null;

            if (dirInfo.Exists)
            {
                foreach (System.IO.FileInfo item in dirInfo.GetFiles())
                {
                    byte[] arquivo = new byte[item.Length];
                    System.IO.FileStream arq = item.OpenRead();
                    arq.Read(arquivo, 0, int.Parse(arq.Length.ToString()));
                    arq.Close();

                    if (item.Name == nomeImagem)
                    {
                        anexoImg = arquivo;
                        nomeImg = item.Name;
                    }
                }
            }

            return File(anexoImg, "application/octet-stream", nomeImg);
        }

        public JsonResult RemoverImagem(string nomeImagem = "")
        {
            try
            {
                if (nomeImagem != "")
                {
                    string strDir = Path.Combine(Server.MapPath("~/Content/anexos/" + ((Usuario)Session["usuario"]).codigo.ToString()));
                    System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(strDir);

                    if (System.IO.Directory.Exists(dirInfo.FullName))
                    {
                        FileInfo arquivo = new FileInfo(strDir + "\\" + nomeImagem);

                        arquivo.Delete();

                        return Json(new { act = 1, mensagem = "A imagem " + nomeImagem + " foi removida " }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { act = 0, mensagem = "Imagem não encontrada." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { act = 0, mensagem = "Imagem não encontrada." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { act = 0, mensagem = "Falha ao remover a imagem " + nomeImagem + ". Motivo: " + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult ExibirImagem(string NomeImagem)
        {
            if(NomeImagem != "" && NomeImagem != null)
            {
                ViewBag.NomeImagem = NomeImagem;

                return View("UploadImagem");
            }
            else
            {
                return null;
            }
        }

        public PartialViewResult AdicionarPalestrante(int codigoPalestrante)
        {
            Usuario oPalestrante = new Usuario();
            oPalestrante = usuarioRes.BuscarPorCodigo(codigoPalestrante);

            List<Usuario> lstPalestrantes = (List<Usuario>)Session["palestrantes"];
            if (lstPalestrantes == null)
            {
                lstPalestrantes = new List<Usuario>();
            }

            lstPalestrantes.Add(oPalestrante);
            Session["palestrantes"] = lstPalestrantes;

            return PartialView(lstPalestrantes);
        }

        public PartialViewResult RemoverPalestrante(int posicao)
        {
            List<Usuario> lstPalestrantes = (List<Usuario>)Session["palestrantes"];
            lstPalestrantes.RemoveAt(posicao);
            Session["palestrantes"] = lstPalestrantes;

            return PartialView("AdicionarPalestrante", lstPalestrantes);
        }

        public PartialViewResult ExibirPalestrante()
        {
            List<Usuario> lstPalestrantes = (List<Usuario>)Session["palestrantes"];
            if (lstPalestrantes == null)
            {
                lstPalestrantes = new List<Usuario>();
            }

            return PartialView("AdicionarPalestrante", lstPalestrantes);
        }

        public PartialViewResult AdicionarUrlVideo(string Url)
        {
            Video oVideo = new Video();
            if (Url.Contains("youtube"))
            {
                string[] array = Url.Split(new[] { "watch?v=" }, StringSplitOptions.None);
                
                oVideo.url = "https://www.youtube.com/embed/" + array[1];
            }
            else if(Url.Contains("vimeo"))
            {
                string[] array = Url.Split(new[] { "https://vimeo.com/" }, StringSplitOptions.None);

                oVideo.url = "https://player.vimeo.com/video/" + array[1];
            }
            else
            {

            }

            List<Video> lstVideos = (List<Video>)Session["videos"];
            if (lstVideos == null)
            {
                lstVideos = new List<Video>();
            }

            lstVideos.Add(oVideo);
            Session["videos"] = lstVideos;

            return PartialView(lstVideos);
        }

        public PartialViewResult RemoverUrlVideo(int posicao)
        {
            List<Video> lstVideos = (List<Video>)Session["videos"];
            lstVideos.RemoveAt(posicao);
            Session["videos"] = lstVideos;

            return PartialView("AdicionarUrlVideo", lstVideos);
        }

        public PartialViewResult ExibirUrlVideo()
        {
            List<Video> lstVideos = (List<Video>)Session["videos"];
            if (lstVideos == null)
            {
                lstVideos = new List<Video>();
            }

            return PartialView("AdicionarUrlVideo", lstVideos);
        }

        public PartialViewResult ModalConteudo()
        {
            Conteudo conteudo = new Conteudo();
            conteudo.usuario = new Usuario();
            conteudo.usuario.codigo = ((Usuario)Session["usuario"]).codigo;
            return PartialView(conteudo);
        }

        public PartialViewResult AdicionarConteudo(Conteudo conteudo)
        {
            List<Conteudo> lstConteudos = (List<Conteudo>)Session["conteudo"];
            if (lstConteudos == null)
            {
                lstConteudos = new List<Conteudo>();
            }

            lstConteudos.Add(conteudo);
            Session["conteudo"] = lstConteudos;

            return PartialView(lstConteudos);
        }

        public PartialViewResult RemoverConteudo(int posicao)
        {
            List<Conteudo> lstConteudos = (List<Conteudo>)Session["conteudo"];
            lstConteudos.RemoveAt(posicao);
            Session["conteudo"] = lstConteudos;

            return PartialView("AdicionarConteudo", lstConteudos);
        }

        public PartialViewResult ExibirConteudo()
        {
            List<Conteudo> lstConteudos = (List<Conteudo>)Session["conteudo"];
            if (lstConteudos == null)
            {
                lstConteudos = new List<Conteudo>();
            }

            string strDir = Path.Combine(Server.MapPath("~/Content/anexos/" + ((Usuario)Session["usuario"]).codigo.ToString()));
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(strDir);

            if (!System.IO.Directory.Exists(dirInfo.FullName))
            {
                Directory.CreateDirectory(strDir);
            }

            foreach (Conteudo conteudo in lstConteudos)
            {
                if (conteudo.nomeAnexo != "" && conteudo.nomeAnexo != null)
                {
                    var CaminhoAnexo = new FileStream(strDir.Replace("\\\\", "/") + "/" + conteudo.nomeAnexo, FileMode.Create);

                    BinaryWriter bw = new BinaryWriter(CaminhoAnexo);
                    bw.Write(conteudo.anexo);
                    bw.Flush();
                    bw.Close();
                }
            }

            return PartialView("AdicionarConteudo", lstConteudos);
        }

        public ActionResult UploadAnexo()
        {
            try
            {
                string strDir = Path.Combine(Server.MapPath("~/Content/anexos/"+((Usuario)Session["usuario"]).codigo.ToString()));
                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(strDir);

                if (!System.IO.Directory.Exists(dirInfo.FullName))
                {
                    Directory.CreateDirectory(strDir);
                }

                HttpPostedFileBase file = Request.Files["files"];

                string extension = Path.GetExtension(file.FileName);

                if (extension.ToUpper() != ".EXE" || extension.ToUpper() != ".BAT")
                {
                    string caminho = Path.Combine(strDir, Path.GetFileName(file.FileName));

                    //foreach (System.IO.FileInfo item in dirInfo.GetFiles())
                    //{
                    //    if (item.Name == file.FileName)
                    //    {
                    //        throw new Exception("O anexo já existe no diretorio!");
                    //    }
                    //}

                    file.SaveAs(caminho);
                    
                    ViewBag.NomeAnexo = file.FileName;

                    return PartialView();
                }
                else
                {
                    throw new Exception("O anexo está em um formato proibido!");
                }
            }
            catch (Exception ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Content(ex.Message);
            }

        }

        public FileResult DownloadAnexo(string nomeAnexo)
        {
            string strDir = Path.Combine(Server.MapPath("~/Content/anexos/" + ((Usuario)Session["usuario"]).codigo.ToString()));
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(strDir);

            string nomeAnx = "";
            byte[] anexo = null;

            if (dirInfo.Exists)
            {
                foreach (System.IO.FileInfo item in dirInfo.GetFiles())
                {
                    byte[] arquivo = new byte[item.Length];
                    System.IO.FileStream arq = item.OpenRead();
                    arq.Read(arquivo, 0, int.Parse(arq.Length.ToString()));
                    arq.Close();

                    if (item.Name == nomeAnexo)
                    {
                        anexo = arquivo;
                        nomeAnx = item.Name;
                    }
                }
            }

            return File(anexo, "application/octet-stream", nomeAnx);
        }

        public JsonResult RemoverAnexo(string nomeAnexo = "")
        {
            try
            {
                if (nomeAnexo != "")
                {
                    string strDir = Path.Combine(Server.MapPath("~/Content/anexos/" + ((Usuario)Session["usuario"]).codigo.ToString()));
                    System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(strDir);

                    if (System.IO.Directory.Exists(dirInfo.FullName))
                    {
                        FileInfo arquivo = new FileInfo(strDir + "\\" + nomeAnexo);

                        arquivo.Delete();

                        return Json(new { act = 1, mensagem = "O anexo " + nomeAnexo + " foi removido " }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { act = 0, mensagem = "Anexo não encontrado." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { act = 0, mensagem = "Anexo não encontrado." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { act = 0, mensagem = "Falha ao remover o anexo " + nomeAnexo + ". Motivo: " + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult EditarEvento(int CodigoEvento)
        {
            ViewBag.Estados = ufRes.ListaUf();
            ViewBag.Categorias = categoriaRes.ListaCategoria();
            ViewBag.Palestrantes = usuarioRes.ListaUsuarios().Where(m => m.nivelAcesso.codigo == 2 && m.liberado == 'S').ToList();

            Evento evento = new Evento();
            evento = eventoRes.BuscarPorCodigo(CodigoEvento);

            string strDir = Path.Combine(Server.MapPath("~/Content/anexos/" + ((Usuario)Session["usuario"]).codigo.ToString()));
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(strDir);

            if (!System.IO.Directory.Exists(dirInfo.FullName))
            {
                Directory.CreateDirectory(strDir);
            }

            if (evento.nomeImagem != "" && evento.nomeImagem != null)
            {
                var imageCapa = new FileStream(strDir.Replace("\\\\", "/") + "/" + evento.nomeImagem, FileMode.Create);

                BinaryWriter bw = new BinaryWriter(imageCapa);
                bw.Write(evento.imagem);
                bw.Flush();
                bw.Close();
            }

            Session["palestrantes"] = evento.palestrante;
            Session["videos"] = evento.videos;
            Session["conteudo"] = evento.conteudo;

            return View("CadastroEvento", evento);
        }

        public JsonResult AtivarInativarEvento(int CodigoEvento, char elemento)
        {
            try
            {
                eventoRes.AtivarInativarEvento(CodigoEvento, elemento);

                if (elemento == 'A')
                {
                    return Json(new { act = 1, mensagem = "Evento ativado com sucesso." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { act = 1, mensagem = "Evento inativado com sucesso." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception ex)
            {
                return Json(new { act = 0, mensagem = "Não foi possível salvar o evento. Motivo: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult VisualizarEvento(int CodigoEvento)
        {
            Evento evento = new Evento();
            evento = eventoRes.BuscarPorCodigo(CodigoEvento);
            if(evento.vagas == null)
            {
                evento.vagas = 0;
            }

            return View(evento);
        }

        public FileResult DownloadAnexoVisualizar(int CodigoEvento, int CodigoAnexo)
        {
            Conteudo conteudo = new Conteudo();
            conteudo = eventoRes.BuscaConteudoEvento(CodigoEvento).Where(m => m.codigo == CodigoAnexo).FirstOrDefault();

            return File(conteudo.anexo, "application/octet-stream", conteudo.nomeAnexo);
        }

        public PartialViewResult DivInscricao(int CodigoEvento)
        {
            Evento evento = new Evento();
            evento = eventoRes.BuscarPorCodigo(CodigoEvento);
            if (evento.vagas == null)
            {
                evento.vagas = 0;
            }

            return PartialView(evento);
        }

        public PartialViewResult Inscricao(int CodigoEvento, int CodigoUsuario)
        {
            Usuario usuario = (Usuario)Session["usuario"];

            Inscricao inscricao = new Inscricao();
            inscricao.evento.codigo = CodigoEvento;
            inscricao.usuario.codigo = CodigoUsuario;
            inscricao.codigo = inscricaoRes.Inscrever(inscricao);

            usuario.eventosInscritos.Add(inscricao);
            Session["usuario"] = usuario;

            inscricao.evento = eventoRes.BuscarPorCodigo(CodigoEvento);

            return PartialView("DivInscricao", inscricao.evento);
        }

        public PartialViewResult CancelarInscricao(int CodigoEvento, int CodigoUsuario)
        {
            Usuario usuario = (Usuario)Session["usuario"];

            Inscricao inscricao = new Inscricao();
            inscricao = usuario.eventosInscritos.Where(m => m.evento.codigo == CodigoEvento).FirstOrDefault();

            inscricaoRes.CancelarInscricao(inscricao);

            usuario.eventosInscritos = inscricaoRes.BuscarInscricoesPorUsuario(usuario.codigo);
            Session["usuario"] = usuario;

            Evento evento = new Evento();
            evento = eventoRes.BuscarPorCodigo(CodigoEvento);

            return PartialView("DivInscricao", evento);
        }

        public PartialViewResult PublicarComentario(int CodigoEvento, int CodigoUsuario, string Comentario)
        {
            Comentario comentario = new Comentario();
            comentario.evento.codigo = CodigoEvento;
            comentario.usuario.codigo = CodigoUsuario;
            comentario.comentario = Comentario;

            comentarioRes.PublicarComentario(comentario);

            Evento evento = new Evento();

            evento = eventoRes.BuscarPorCodigo(CodigoEvento);

            ViewBag.NumeroComentario = evento.comentarios.Count;
            ViewBag.Palestrantes = evento.palestrante;
            ViewBag.Organizador = evento.organizador;

            return PartialView(evento.comentarios);
        }

        public PartialViewResult RemoverComentario(int CodigoComentario, int CodigoEvento)
        {
            Comentario comentario = new Comentario();
            comentario.codigo = CodigoComentario;

            comentarioRes.ApagarComentario(comentario);

            Evento evento = new Evento();

            evento = eventoRes.BuscarPorCodigo(CodigoEvento);

            ViewBag.NumeroComentario = evento.comentarios.Count;
            ViewBag.Palestrantes = evento.palestrante;
            ViewBag.Organizador = evento.organizador;

            return PartialView("PublicarComentario", evento.comentarios);
        }

        //Area do Palestrante

        public ActionResult GerenciarConteudo()
        {
            Usuario usuario = new Usuario();
            usuario = (Usuario)Session["usuario"];
            List<Evento> lstEventos = new List<Evento>();

            if (usuario.nivelAcesso.codigo == 4)
            {
                object pFiltro = new
                {
                    uf = "",
                    categoria = "",
                    nome = ""
                };

                lstEventos = eventoRes.ListaEventos(pFiltro).OrderBy(m => m.dataHoraFimInscricao).ToList();
            }
            else
            {
                lstEventos = eventoRes.ListaEventosPorPalestrante(usuario.codigo).OrderBy(m => m.dataHoraFimInscricao).ToList();
            }

            return View(lstEventos);
        }

        public ActionResult AdicionarConteudoEvento(int CodigoEvento)
        {
            Evento evento = new Evento();
            evento = eventoRes.BuscarPorCodigo(CodigoEvento);

            return View(evento);
        }

        public PartialViewResult ExibirConteudoEvento(int CodigoEvento)
        {
            Evento evento = new Evento();
            evento = eventoRes.BuscarPorCodigo(CodigoEvento);

            string strDir = Path.Combine(Server.MapPath("~/Content/anexos/" + ((Usuario)Session["usuario"]).codigo.ToString()));
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(strDir);

            if (!System.IO.Directory.Exists(dirInfo.FullName))
            {
                Directory.CreateDirectory(strDir);
            }

            foreach (Conteudo conteudo in evento.conteudo)
            {
                if (conteudo.nomeAnexo != "" && conteudo.nomeAnexo != null)
                {
                    var CaminhoAnexo = new FileStream(strDir.Replace("\\\\", "/") + "/" + conteudo.nomeAnexo, FileMode.Create);

                    BinaryWriter bw = new BinaryWriter(CaminhoAnexo);
                    bw.Write(conteudo.anexo);
                    bw.Flush();
                    bw.Close();
                }
            }

            return PartialView(evento);
        }

        public PartialViewResult ModalConteudoPalestrante(int CodigoEvento)
        {
            Conteudo conteudo = new Conteudo();
            conteudo.usuario = new Usuario();
            conteudo.usuario.codigo = ((Usuario)Session["usuario"]).codigo;
            ViewBag.CodigoEvento = CodigoEvento;

            return PartialView(conteudo);
        }

        public PartialViewResult AdicionarConteudoPalestrante(int CodigoUsuario, string TipoConteudo, string Descricao, string NomeAnexo, int CodigoEvento)
        {
            Conteudo conteudo = new Conteudo();
            conteudo.usuario.codigo = CodigoUsuario;
            conteudo.tipoConteudo = TipoConteudo;
            conteudo.descricao = Descricao;
            conteudo.nomeAnexo = NomeAnexo;

            string strDir = Path.Combine(Server.MapPath("~/Content/anexos/" + ((Usuario)Session["usuario"]).codigo.ToString()));
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(strDir);

            //Recupera o conteudo
            if (dirInfo.Exists)
            {
                foreach (System.IO.FileInfo item in dirInfo.GetFiles())
                {
                    byte[] arquivo = new byte[item.Length];
                    System.IO.FileStream arq = item.OpenRead();
                    arq.Read(arquivo, 0, int.Parse(arq.Length.ToString()));
                    arq.Close();

                    if (item.Name == conteudo.nomeAnexo)
                    {
                        conteudo.anexo = arquivo;
                    }
                }
            }

            eventoRes.InserirConteudo(conteudo, CodigoEvento);

            Evento evento = new Evento();
            evento = eventoRes.BuscarPorCodigo(CodigoEvento);

            return PartialView("ExibirConteudoEvento", evento);
        }

        public PartialViewResult ApagarConteudo(int CodigoConteudo, int CodigoEvento)
        {
            eventoRes.ApagarConteudo(CodigoConteudo);

            Evento evento = new Evento();
            evento = eventoRes.BuscarPorCodigo(CodigoEvento);

            return PartialView("ExibirConteudoEvento", evento);
        }
    }
}