using GerenciadorEventos.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GerenciadorEventos.Repositorio
{
    public class EmailRepositorio
    {
        private readonly Contexto contexto;

        public EmailRepositorio()
        {
            contexto = new Contexto();
        }

        public int RegistrarEmail(Email email)
        {
            try
            {
                //Insere E-mail
                const string commandText = "INSERT INTO email (cod_usuario, destinatario, titulo, mensagem, data_envio) VALUES (@cod_usuario, @destinatario, @titulo, @mensagem, @data_envio) ";

                var parameters = new Dictionary<string, object>
                {
                    {"cod_usuario", email.usuario.codigo},
                    {"destinatario", email.destinatario},
                    {"titulo", email.titulo},
                    {"mensagem", email.mensagem},
                    {"data_envio", email.dataEnvio}
                };

                return contexto.ExecutaComando(commandText, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}