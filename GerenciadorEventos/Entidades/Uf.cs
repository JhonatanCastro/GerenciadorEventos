using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GerenciadorEventos.Entidades
{
    public class Uf
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo UF não pode ser vazio.")]
        [Display(Name = "UF")]
        public int codigo { get; set; }
        public string uf { get; set; }
        public string estado { get; set; }
    }
}