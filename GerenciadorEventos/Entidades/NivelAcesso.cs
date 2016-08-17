using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GerenciadorEventos.Entidades
{
    public class NivelAcesso
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Nível de Acesso não pode ser vazio.")]
        [Display(Name = "Nível de Acesso")]
        public int codigo { get; set; }
        public string nivelAcesso { get; set; }
    }
}