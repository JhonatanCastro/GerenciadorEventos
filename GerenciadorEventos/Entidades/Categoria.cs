using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GerenciadorEventos.Entidades
{
    public class Categoria
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Categoria não pode ser vazio.")]
        [Display(Name = "Categoria")]
        public int codigo { get; set; }
        public string categoria { get; set; }
    }
}