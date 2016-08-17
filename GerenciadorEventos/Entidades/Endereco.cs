using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GerenciadorEventos.Entidades
{
    public class Endereco
    {
        public int codigo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Logradouro não pode ser vazio.")]
        [Display(Name = "Logradouro")]
        public string logradouro { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Número não pode ser vazio.")]
        [Display(Name = "Número")]
        public string numero { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Bairro não pode ser vazio.")]
        [Display(Name = "Bairro")]
        public string bairro { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Cidade não pode ser vazio.")]
        [Display(Name = "Cidade")]
        public string cidade { get; set; }
        public Uf uf { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo CEP não pode ser vazio.")]
        [Display(Name = "CEP")]
        public string cep { get; set; }
    }
}