using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GerenciadorEventos.Entidades
{
    public class Conteudo
    {
        public int codigo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Logradouro não pode ser vazio.")]
        [Display(Name = "Tipo de Conteúdo")]
        public string tipoConteudo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Logradouro não pode ser vazio.")]
        [Display(Name = "Descrição")]
        public string descricao { get; set; }
        public string nomeAnexo { get; set; }
        public byte[] anexo { get; set; }
        public Usuario usuario { get; set; }

        public Conteudo()
        {
            this.usuario = new Usuario();
        }
    }
}