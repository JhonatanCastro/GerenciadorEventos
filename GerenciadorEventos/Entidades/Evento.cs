using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GerenciadorEventos.Entidades
{
    public class Evento
    {
        public int codigo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Nome não pode ser vazio.")]
        [Display(Name = "Nome")]
        public string nome { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Descrição não pode ser vazio.")]
        [Display(Name = "Descrição")]
        public string descricao { get; set; }
        
        [Display(Name = "Data Inicio Inscrição")]
        public DateTime? dataHoraInicioInscricao { get; set; }

        [Display(Name = "Data Fim Inscrição")]
        public DateTime? dataHoraFimInscricao { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Data/Hora do inicio do evento não pode ser vazio.")]
        [Display(Name = "Data Inicio Evento")]
        public DateTime? dataHoraInicio { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Data/Hora do fim do evento não pode ser vazio.")]
        [Display(Name = "Data Fim Evento")]
        public DateTime? dataHoraFim { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O campo Vagas não pode ser vazio.")]
        [Display(Name = "Vagas")]
        public int? vagas { get; set; }

        [Display(Name = "Evento pago?")]
        public bool pago { get; set; }

        [Display(Name = "Valor")]
        public double? valor { get; set; }

        public string nomeImagem { get; set; }
        public byte[] imagem { get; set; }
        public string strImagem { get; set; }
        public Categoria categoria { get; set; }
        public Endereco endereco { get; set; }
        public List<Usuario> palestrante { get; set; }
        public List<Video> videos { get; set; }
        public List<Conteudo> conteudo { get; set; }
        public List<Comentario> comentarios { get; set; }
        public Usuario organizador { get; set; }
        public char ativo { get; set; }
        public int inscritos { get; set; }
        public string relacionamento { get; set; }

        public Evento()
        {
            this.codigo = 0;
            this.nome = "";
            this.descricao = "";
            //this.dataHora = DateTime.Now;
            //this.vagas = 0;
            this.nomeImagem = "";
            this.categoria = new Categoria();
            //this.endereco = new Endereco();
            this.palestrante = new List<Usuario>();
            this.videos = new List<Video>();
            this.conteudo = new List<Conteudo>();
            this.comentarios = new List<Comentario>();
            this.organizador = new Usuario();
            this.ativo = '\0';
            this.inscritos = 0;
        }
    }
}