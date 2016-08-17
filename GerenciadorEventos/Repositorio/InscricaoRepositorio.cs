using GerenciadorEventos.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GerenciadorEventos.Repositorio
{
    public class InscricaoRepositorio
    {
        private readonly Contexto contexto;

        public InscricaoRepositorio()
        {
            contexto = new Contexto();
        }

        /* Tipos de status da presença */
        /* Inscrito, Ausente, Presente, Cancelada */

        public int Inscrever(Inscricao inscricao)
        {
            try
            {
                //Insere Endereço
                const string commandText = "INSERT INTO inscricao (cod_evento, cod_usuario, status_presenca, data_cadastro) VALUES (@cod_evento, @cod_usuario, 'Inscrito', @data_cadastro) ";

                var parameters = new Dictionary<string, object>
                {
                    {"cod_evento", inscricao.evento.codigo},
                    {"cod_usuario", inscricao.usuario.codigo},
                    {"data_cadastro", inscricao.dataCadastro }
                };

                var retorno = contexto.ExecutaComando(commandText, parameters);

                //Buscar Inscrição Inserida
                const string strQuery2 = "SELECT codigo FROM inscricao WHERE codigo = (SELECT MAX(codigo) FROM inscricao) ORDER by codigo DESC";

                var rows2 = contexto.ExecutaComandoComRetorno(strQuery2, null);

                int codigoInscricao = 0;
                foreach (var row in rows2)
                {
                    codigoInscricao = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0");
                }

                return codigoInscricao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int CancelarInscricao(Inscricao inscricao)
        {
            try
            {
                //Cancela Inscrição
                var commandText = " UPDATE inscricao SET ";
                commandText += " status_presenca = 'Cancelada', ";
                commandText += " data_cadastro = @data_cadastro ";
                commandText += " WHERE codigo = @codigo ";

                var parameters = new Dictionary<string, object>
                {
                    {"codigo", inscricao.codigo},
                    {"data_cadastro", inscricao.dataCadastro }
                };
                
                return contexto.ExecutaComando(commandText, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int CancelarInscricaoOutroUsuario(Inscricao inscricao)
        {
            try
            {
                //Cancela Inscrição
                var commandText = " UPDATE inscricao SET ";
                commandText += " status_presenca = 'Cancelada', ";
                commandText += " data_cadastro = @data_cadastro ";
                commandText += " WHERE cod_usuario = @cod_usuario AND cod_evento = @cod_evento ";

                var parameters = new Dictionary<string, object>
                {
                    {"cod_usuario", inscricao.usuario.codigo},
                    {"cod_evento", inscricao.evento.codigo},
                    {"data_cadastro", inscricao.dataCadastro }
                };

                return contexto.ExecutaComando(commandText, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AlterarStatusInscricao(Inscricao inscricao)
        {
            try
            {
                //Cancela Inscrição
                var commandText = " UPDATE inscricao SET ";
                commandText += " status_presenca = @status, ";
                commandText += " data_cadastro = @data_cadastro ";
                commandText += " WHERE cod_usuario = @cod_usuario AND cod_evento = @cod_evento AND status_presenca = 'Inscrito'";

                var parameters = new Dictionary<string, object>
                {
                    {"cod_usuario", inscricao.usuario.codigo},
                    {"cod_evento", inscricao.evento.codigo},
                    {"status", inscricao.statusPresenca},
                    {"data_cadastro", inscricao.dataCadastro }
                };

                return contexto.ExecutaComando(commandText, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Inscricao> BuscarInscricoesPorUsuario(int CodigoUsuario)
        {
            var lstInscricao = new List<Inscricao>();
            const string strQuery = "SELECT ins.codigo, ins.cod_evento, ins.cod_usuario, ins.status_presenca, "+
                                        "usu.nome, usu.rg, usu.cpf, usu.telefone, usu.email, usu.ativo, usu.nivel_acesso, "+
                                        "evt.nome AS nome_evento, evt.descricao, evt.dataHoraInicioInscricao, evt.dataHoraFimInscricao, evt.dataHoraInicial, evt.dataHoraFinal, evt.vagas, evt.pago, evt.valor, evt.cod_categoria, evt.ativo AS ativo_evento, (SELECT COUNT(codigo) FROM inscricao WHERE cod_evento = evt.codigo AND status_presenca <> 'Cancelada') AS inscritos " +
                                    "FROM inscricao ins " +
                                        "INNER JOIN usuario usu ON usu.codigo = ins.cod_usuario "+
                                        "INNER JOIN evento evt ON evt.codigo = ins.cod_evento "+
                                    "WHERE ins.cod_usuario = @codigo AND ins.status_presenca <> 'Cancelada'";

            var parametros = new Dictionary<string, object>
            {
                {"codigo", CodigoUsuario}
            };
            var rows = contexto.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempInscricao = new Inscricao
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0"),
                    statusPresenca = row["status_presenca"].ToString()
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
                    inscritos = int.Parse(!string.IsNullOrEmpty(row["inscritos"].ToString()) ? row["inscritos"].ToString() : "0")
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
                    codigo = int.Parse(row["nivel_acesso"].ToString())
                };

                tempUsuario.nivelAcesso = tempNivel;
                tempEvento.categoria = tempCategoria;

                tempInscricao.usuario = tempUsuario;
                tempInscricao.evento = tempEvento;

                lstInscricao.Add(tempInscricao);
            }

            return lstInscricao;
        }

        public List<Evento> BuscarEventosPorUsuario(int CodigoUsuario)
        {
            var lstEventos = new List<Evento>();
            const string strQuery = "SELECT "+
                                        "DISTINCT "+
                                        "evt.codigo, evt.nome AS nome_evento, evt.descricao, evt.dataHoraInicioInscricao, evt.dataHoraFimInscricao, evt.dataHoraInicial, evt.dataHoraFinal, evt.vagas, evt.pago, evt.valor, evt.cod_categoria, evt.ativo AS ativo_evento, (SELECT COUNT(codigo) FROM inscricao WHERE cod_evento = evt.codigo AND status_presenca <> 'Cancelada') AS inscritos, evt.cod_organizador, " +
                                        "CASE WHEN ins.status_presenca IS NOT NULL AND ins.cod_usuario = @codigo AND evt.cod_organizador = @codigo THEN "+
                                            "'Organizador - Participante' "+
                                        "WHEN ins.status_presenca IS NULL AND evt.cod_organizador = @codigo THEN "+
                                            "'Organizador' "+
                                            "ELSE " +
                                                "'Participante' "+
                                        "END AS relacionamento "+
                                    "FROM evento evt "+
                                        "LEFT JOIN inscricao ins ON ins.cod_evento = evt.codigo AND ins.status_presenca <> 'Cancelada' AND ins.cod_usuario = @codigo " +
                                    "WHERE evt.cod_organizador = @codigo OR ins.cod_usuario = @codigo";

            var parametros = new Dictionary<string, object>
            {
                {"codigo", CodigoUsuario}
            };
            var rows = contexto.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempEvento = new Evento
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0"),
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
                    organizador = new Usuario
                    {
                        codigo = int.Parse(!string.IsNullOrEmpty(row["cod_organizador"].ToString()) ? row["cod_organizador"].ToString() : "0")
                    },
                    relacionamento = row["relacionamento"].ToString()
                };
                if (tempEvento.valor == 0) tempEvento.valor = null;
                var tempCategoria = new Categoria
                {
                    codigo = int.Parse(row["cod_categoria"].ToString())
                };
                tempEvento.categoria = tempCategoria;

                lstEventos.Add(tempEvento);
            }

            return lstEventos;
        }

        public List<Usuario> ListaUsuariosInscritosPorEvento(int CodigoEvento)
        {
            var user = new List<Usuario>();
            const string strQuery = "SELECT "+
                                        "usu.codigo, usu.nome, usu.rg, usu.cpf, usu.cod_endereco, usu.telefone, usu.email, usu.ativo, usu.nivel_acesso, "+
                                        "ende.logradouro, ende.numero, ende.bairro, ende.cidade, ende.cod_uf, ende.cep, estado.uf, estado.estado, nivel.nivel_acesso AS nomeNivel, "+
                                        "ins.status_presenca, ins.data_cadastro, "+
                                        "CASE WHEN ins.status_presenca = 'Presente' THEN "+
                                            "1 "+
                                            "WHEN ins.status_presenca = 'Inscrito' THEN "+
                                            "2 "+
                                            "WHEN ins.status_presenca = 'Ausente' THEN "+
                                            "3 "+
                                            "ELSE "+
                                            "4 "+
                                        "END AS ordem "+
                                    "FROM usuario usu "+
                                    "INNER JOIN endereco ende ON ende.codigo = usu.cod_endereco "+
                                    "INNER JOIN uf estado ON estado.codigo = ende.cod_uf "+
                                    "INNER JOIN nivelacesso nivel ON nivel.codigo = usu.nivel_acesso "+
                                    "INNER JOIN inscricao ins ON ins.cod_usuario = usu.codigo "+
                                    "WHERE ins.cod_evento = @codigo "+
                                    "ORDER BY ordem";

            var parametros = new Dictionary<string, object>
            {
                {"codigo", CodigoEvento}
            };

            var rows = contexto.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempUsuario = new Usuario
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0"),
                    nome = row["nome"].ToString(),
                    rg = row["rg"].ToString(),
                    cpf = row["cpf"].ToString(),
                    telefone = row["telefone"].ToString(),
                    email = row["email"].ToString(),
                    ativo = char.Parse(row["ativo"].ToString()),
                    statusInscricao = row["status_presenca"].ToString(),
                    dataCadastro = DateTime.Parse(row["data_cadastro"].ToString())
                };
                var tempNivel = new NivelAcesso
                {
                    codigo = int.Parse(row["nivel_acesso"].ToString()),
                    nivelAcesso = row["nomeNivel"].ToString()
                };
                var tempEndereco = new Endereco
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["cod_endereco"].ToString()) ? row["cod_endereco"].ToString() : "0"),
                    logradouro = row["logradouro"].ToString(),
                    numero = row["numero"].ToString(),
                    bairro = row["bairro"].ToString(),
                    cidade = row["cidade"].ToString(),
                    cep = row["cep"].ToString()
                };
                var tempUf = new Uf
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["cod_uf"].ToString()) ? row["cod_uf"].ToString() : "0"),
                    uf = row["uf"].ToString(),
                    estado = row["estado"].ToString()
                };
                tempEndereco.uf = tempUf;
                tempUsuario.endereco = tempEndereco;
                tempUsuario.nivelAcesso = tempNivel;

                user.Add(tempUsuario);
            }

            return user;
        }
    }
}