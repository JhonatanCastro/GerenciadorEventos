using GerenciadorEventos.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GerenciadorEventos.Repositorio
{
    public class ComentarioRepositorio
    {
        private readonly Contexto contexto;

        public ComentarioRepositorio()
        {
            contexto = new Contexto();
        }

        public int PublicarComentario(Comentario comentario)
        {
            try
            {
                //Insere Comentario
                const string commandText = "INSERT INTO comentario (cod_evento, cod_usuario, comentario, data_cadastro) VALUES (@cod_evento, @cod_usuario, @comentario, @data_cadastro) ";

                var parameters = new Dictionary<string, object>
                {
                    {"cod_evento", comentario.evento.codigo},
                    {"cod_usuario", comentario.usuario.codigo},
                    {"comentario", comentario.comentario},
                    {"data_cadastro", comentario.data}
                };

                return contexto.ExecutaComando(commandText, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ApagarComentario(Comentario comentario)
        {
            try
            {
                //Apagar Comentario
                var commandText = " DELETE FROM comentario WHERE codigo = @codigo ";

                var parameters = new Dictionary<string, object>
                {
                    {"codigo", comentario.codigo}
                };

                return contexto.ExecutaComando(commandText, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Comentario> BuscarComentariosPorEvento(int CodigoEvento)
        {
            var lstComentario = new List<Comentario>();
            const string strQuery = "SELECT com.codigo, com.cod_evento, com.cod_usuario, com.comentario, com.data_cadastro, "+
                                        "usu.nome, usu.rg, usu.cpf, usu.telefone, usu.email, usu.ativo, usu.nivel_acesso, nivel.nivel_acesso AS nomeNivel, " +
                                        "evt.nome AS nome_evento, evt.descricao, evt.dataHoraInicioInscricao, evt.dataHoraFimInscricao, evt.dataHoraInicial, evt.dataHoraFinal, evt.vagas, evt.pago, evt.valor, evt.cod_categoria, evt.ativo AS ativo_evento, (SELECT COUNT(codigo) FROM inscricao WHERE codigo = evt.codigo AND status_presenca <> 'Cancelada') AS inscritos, evt.	cod_organizador " +
                                    "FROM comentario com "+
                                        "INNER JOIN usuario usu ON usu.codigo = com.cod_usuario "+
                                        "INNER JOIN nivelacesso nivel ON nivel.codigo = usu.nivel_acesso " +
                                        "INNER JOIN evento evt ON evt.codigo = com.cod_evento " +
                                    "WHERE com.cod_evento = @codigo";

            var parametros = new Dictionary<string, object>
            {
                {"codigo", CodigoEvento}
            };
            var rows = contexto.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempComentario = new Comentario
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0"),
                    comentario = row["comentario"].ToString(),
                    data = DateTime.Parse(row["data_cadastro"].ToString())
                };

                var tempEvento = new Evento
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["cod_evento"].ToString()) ? row["cod_evento"].ToString() : "0"),
                    nome = row["nome_evento"].ToString(),
                    descricao = row["descricao"].ToString(),
                    dataHoraInicioInscricao = (!string.IsNullOrEmpty(row["dataHoraInicioInscricao"].ToString()) ? DateTime.Parse(row["dataHoraInicioInscricao"].ToString()) : DateTime.Parse("")),
                    dataHoraFimInscricao = (!string.IsNullOrEmpty(row["dataHoraFimInscricao"].ToString()) ? DateTime.Parse(row["dataHoraFimInscricao"].ToString()) : DateTime.Parse("")),
                    dataHoraInicio = DateTime.Parse(row["dataHoraInicial"].ToString()),
                    dataHoraFim = DateTime.Parse(row["dataHoraFinal"].ToString()),
                    vagas = int.Parse(row["vagas"].ToString()),
                    pago = (row["pago"].ToString() == "S") ? true : false,
                    valor = double.Parse(!string.IsNullOrEmpty(row["valor"].ToString()) ? row["valor"].ToString() : "0"),
                    ativo = char.Parse(row["ativo_evento"].ToString()),
                    inscritos = int.Parse(!string.IsNullOrEmpty(row["inscritos"].ToString()) ? row["inscritos"].ToString() : "0"),
                    organizador = new Usuario()
                    {
                        codigo = int.Parse(!string.IsNullOrEmpty(row["cod_organizador"].ToString()) ? row["cod_organizador"].ToString() : "0")
                    }
                };
                if (tempEvento.valor == 0) tempEvento.valor = null;
                var tempCategoria = new Categoria
                {
                    codigo = int.Parse(row["cod_categoria"].ToString())
                };

                var tempUsuario = new Usuario
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["cod_usuario"].ToString()) ? row["cod_usuario"].ToString() : "0"),
                    nome = row["nome"].ToString(),
                    rg = row["rg"].ToString(),
                    cpf = row["cpf"].ToString(),
                    telefone = row["telefone"].ToString(),
                    email = row["email"].ToString(),
                    ativo = char.Parse(row["ativo"].ToString())

                };
                var tempNivel = new NivelAcesso
                {
                    codigo = int.Parse(row["nivel_acesso"].ToString()),
                    nivelAcesso = row["nomeNivel"].ToString()
                };

                tempUsuario.nivelAcesso = tempNivel;
                tempEvento.categoria = tempCategoria;

                tempComentario.usuario = tempUsuario;
                tempComentario.evento = tempEvento;

                lstComentario.Add(tempComentario);
            }

            return lstComentario;
        }
    }
}