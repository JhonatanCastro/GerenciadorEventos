using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GerenciadorEventos.Entidades
{
    public class ViewToPdf
    {
        public List<Usuario> lstUsuario { get; set; }
        public Evento evento { get; set; }
    }
}