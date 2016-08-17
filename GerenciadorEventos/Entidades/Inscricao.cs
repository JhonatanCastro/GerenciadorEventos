using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GerenciadorEventos.Entidades
{
    public class Inscricao
    {
        public int codigo { get; set; }
        public Evento evento { get; set; }
        public Usuario usuario { get; set; }
        public string statusPresenca { get; set; }
        public DateTime dataCadastro { get; set; }

        public Inscricao()
        {
            this.codigo = 0;
            this.usuario = new Usuario();
            this.evento = new Evento();
            this.statusPresenca = "";
            this.dataCadastro = DateTime.Now;
        }
    }
}