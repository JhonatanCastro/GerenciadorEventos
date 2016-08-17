using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GerenciadorEventos.Entidades
{
    public class Email
    {
        public int codigo { get; set; }
        public string destinatario { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Informe o Assunto.")]
        [Display(Name = "Assunto")]
        public string titulo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Informe a Mensagem.")]
        [Display(Name = "Mensagem")]
        public string mensagem { get; set; }
        public Evento evento { get; set; }
        public Usuario usuario { get; set; }
        public DateTime dataEnvio { get; set; }
    }
}