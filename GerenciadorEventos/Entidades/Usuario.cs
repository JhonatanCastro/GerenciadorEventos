using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GerenciadorEventos.Entidades
{
    public class Usuario
    {
        public int codigo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Nome não pode ser vazio.")]
        [Display(Name = "Nome")]
        public string nome { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo RG não pode ser vazio.")]
        [Display(Name = "RG")]
        public string rg { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo CPF não pode ser vazio.")]
        [Display(Name = "CPF")]
        public string cpf { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Telefone não pode ser vazio.")]
        [Display(Name = "Telefone")]
        public string telefone { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo E-mail não pode ser vazio.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})$", ErrorMessage = "E-mail inválido.")]
        [Display(Name = "E-mail")]
        [StringLength(60, ErrorMessage = "Tamanho do campo não pode ser maior que 60 caracteres.")]
        public string email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Senha não pode ser vazio.")]
        [Display(Name = "Senha")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 20 caracteres.")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z]).*$", ErrorMessage = "Senha fora do padrão. A senha deve conter no mínimo 6(seis) caracteres com letras e números.")]
        public string senha { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Confirma Senha não pode ser vazio.")]
        [Display(Name = "Confirma Senha")]
        [DataType(DataType.Password)]
        [Compare("senha", ErrorMessage = "A senha e o confirma senha são diferentes.")]
        public string confirmasenha { get; set; }
        public Endereco endereco { get; set; }

        public char ativo { get; set; }

        public NivelAcesso nivelAcesso { get; set; }
        public char liberado { get; set; }
        public string statusInscricao { get; set; }
        public DateTime dataCadastro { get; set; }

        [Display(Name = "Lembre Me?")]
        public bool LembreMe { get; set; }

        public List<Inscricao> eventosInscritos { get; set; }

        public Usuario()
        {
            this.nivelAcesso = new NivelAcesso();
            this.eventosInscritos = new List<Inscricao>();
        }
    }
}