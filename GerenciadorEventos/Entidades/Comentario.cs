using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GerenciadorEventos.Entidades
{
    public class Comentario
    {
        public int codigo { get; set; }
        public string comentario { get; set; }
        public Usuario usuario { get; set; }
        public Evento evento { get; set; }
        public DateTime data { get; set; }

        public Comentario()
        {
            this.codigo = 0;
            this.comentario = "";
            this.usuario = new Usuario();
            this.evento = new Evento();
            this.data = DateTime.Now;
        }
    }
}