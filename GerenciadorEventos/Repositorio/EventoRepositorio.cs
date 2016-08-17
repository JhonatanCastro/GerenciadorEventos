using GerenciadorEventos.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace GerenciadorEventos.Repositorio
{
    public class EventoRepositorio
    {
        private readonly Contexto contexto;

        public EventoRepositorio()
        {
            contexto = new Contexto();
        }

        private int Inserir(Evento evento)
        {
            try
            {
                //Insere Endereço
                const string commandText2 = "INSERT INTO endereco (logradouro, numero, bairro, cidade, cod_uf, cep) VALUES (@logradouro, @numero, @bairro, @cidade, @cod_uf, @cep) ";

                var parameters2 = new Dictionary<string, object>
                {
                    {"logradouro", evento.endereco.logradouro},
                    {"numero", evento.endereco.numero},
                    {"bairro", evento.endereco.bairro},
                    {"cidade", evento.endereco.cidade},
                    {"cod_uf", evento.endereco.uf.codigo},
                    {"cep", evento.endereco.cep}
                };

                var retornoEndereco = contexto.ExecutaComando(commandText2, parameters2);

                //Buscar Endereço Inserido
                const string strQuery2 = "SELECT codigo FROM endereco WHERE codigo = (SELECT MAX(codigo) FROM endereco) ORDER by codigo DESC";

                var rows2 = contexto.ExecutaComandoComRetorno(strQuery2, null);

                int codigoEndereco = 0;
                foreach (var row in rows2)
                {
                    codigoEndereco = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0");
                }

                //Inserir Evento
                const string commandText = "INSERT INTO evento (nome, descricao, dataHoraInicioInscricao, dataHoraFimInscricao, dataHoraInicial, dataHoraFinal, cod_endereco, vagas, pago, valor, cod_categoria, nome_imagem, imagem, ativo, cod_organizador) VALUES (@nome, @descricao, @dataHoraInicioInscricao, @dataHoraFimInscricao, @dataHoraInicial, @dataHoraFinal, @cod_endereco, @vagas, @pago, @valor, @cod_categoria, @nome_imagem, @imagem, @ativo, @cod_organizador) ";

                var parameters = new Dictionary<string, object>
                {
                    {"nome", evento.nome},
                    {"descricao", evento.descricao},
                    {"dataHoraInicioInscricao", evento.dataHoraInicioInscricao},
                    {"dataHoraFimInscricao", evento.dataHoraFimInscricao},
                    {"dataHoraInicial", evento.dataHoraInicio},
                    {"dataHoraFinal", evento.dataHoraFim},
                    {"cod_endereco", codigoEndereco},
                    {"vagas", evento.vagas},
                    {"pago", (evento.pago)? "S":"N"},
                    {"valor", evento.valor},
                    {"cod_categoria", evento.categoria.codigo},
                    {"nome_imagem", evento.nomeImagem},
                    {"imagem", evento.imagem},
                    {"ativo", evento.ativo},
                    {"cod_organizador", evento.organizador.codigo}
                };

                var retornoEvento = contexto.ExecutaComando(commandText, parameters);

                //Buscar Evento Inserido
                const string strQuery = "SELECT codigo FROM evento WHERE codigo = (SELECT MAX(codigo) FROM evento) ORDER by codigo DESC";

                var rows = contexto.ExecutaComandoComRetorno(strQuery, null);

                int codigoEvento = 0;
                foreach (var row in rows)
                {
                    codigoEvento = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0");
                }
                evento.codigo = codigoEvento;

                //Inserir Palestrante
                foreach (Usuario palestrante in evento.palestrante)
                {
                    var retornoPalestrante = this.InserirPalestrante(palestrante, evento.codigo);
                }

                //Inserir Url Video
                foreach (Video video in evento.videos)
                {
                    var retornoVideo = this.InserirUrlVideo(video, evento.codigo);
                }

                //Inserir Conteudo
                foreach (Conteudo conteudo in evento.conteudo)
                {
                    var retornoPalestrante = this.InserirConteudo(conteudo, evento.codigo);
                }


                return retornoEvento;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int Alterar(Evento evento)
        {
            try
            {
                //Atualiza Evento
                var commandText = " UPDATE evento SET ";
                commandText += " nome = @nome, ";
                commandText += " descricao = @descricao, ";
                commandText += " dataHoraInicioInscricao = @dataHoraInicioInscricao, ";
                commandText += " dataHoraFimInscricao = @dataHoraFimInscricao, ";
                commandText += " dataHoraInicial = @dataHoraInicial, ";
                commandText += " dataHoraFinal = @dataHoraFinal, ";
                commandText += " cod_endereco = @cod_endereco, ";
                commandText += " vagas = @vagas, ";
                commandText += " pago = @pago, ";
                commandText += " valor = @valor, ";
                commandText += " cod_categoria = @cod_categoria, ";
                commandText += " nome_imagem = @nome_imagem, ";
                commandText += " imagem = @imagem, ";
                commandText += " ativo = @ativo ";
                commandText += " WHERE codigo = @codigo ";

                var parameters = new Dictionary<string, object>
                {
                    {"codigo", evento.codigo},
                    {"nome", evento.nome},
                    {"descricao", evento.descricao},
                    {"dataHoraInicioInscricao", evento.dataHoraInicioInscricao},
                    {"dataHoraFimInscricao", evento.dataHoraFimInscricao},
                    {"dataHoraInicial", evento.dataHoraInicio},
                    {"dataHoraFinal", evento.dataHoraFim},
                    {"cod_endereco", evento.endereco.codigo},
                    {"vagas", evento.vagas},
                    {"pago", (evento.pago)? "S":"N"},
                    {"valor", evento.valor},
                    {"cod_categoria", evento.categoria.codigo},
                    {"nome_imagem", evento.nomeImagem },
                    {"imagem", evento.imagem},
                    {"ativo", evento.ativo}
                };

                //Atualiza Endereço do Evento
                var commandText2 = " UPDATE endereco SET ";
                commandText2 += " logradouro = @logradouro, ";
                commandText2 += " numero = @numero, ";
                commandText2 += " bairro = @bairro, ";
                commandText2 += " cidade = @cidade, ";
                commandText2 += " cod_uf = @cod_uf, ";
                commandText2 += " cep = @cep ";
                commandText2 += " WHERE codigo = @codigo";

                var parameters2 = new Dictionary<string, object>
                {
                    {"codigo", evento.endereco.codigo},
                    {"logradouro", evento.endereco.logradouro},
                    {"numero", evento.endereco.numero},
                    {"bairro", evento.endereco.bairro},
                    {"cidade", evento.endereco.cidade},
                    {"cod_uf", evento.endereco.uf.codigo},
                    {"cep", evento.endereco.cep }
                };

                var resultadoEndereco = contexto.ExecutaComando(commandText2, parameters2);

                //Apagar Palestrante do evento
                var commandText3 = "DELETE FROM palestrante WHERE cod_evento = @cod_evento";

                var parameters3 = new Dictionary<string, object>
                {
                    {"cod_evento", evento.codigo}
                };

                var resultadoPalestrante = contexto.ExecutaComando(commandText3, parameters3);

                //Inserir Palestrante
                foreach (Usuario palestrante in evento.palestrante)
                {
                    var retornoPalestrante = this.InserirPalestrante(palestrante, evento.codigo);
                }

                //Apagar Url Video do evento
                var commandText4 = "DELETE FROM video WHERE cod_evento = @cod_evento";

                var parameters4 = new Dictionary<string, object>
                {
                    {"cod_evento", evento.codigo}
                };

                var resultadoUrlVideo = contexto.ExecutaComando(commandText4, parameters4);

                //Inserir Url Video
                foreach (Video video in evento.videos)
                {
                    var retornoVideo = this.InserirUrlVideo(video, evento.codigo);
                }

                //Apagar conteúdo do evento
                var commandText5 = "DELETE FROM conteudo WHERE cod_evento = @cod_evento";

                var parameters5 = new Dictionary<string, object>
                {
                    {"cod_evento", evento.codigo}
                };

                var resultadoConteudo = contexto.ExecutaComando(commandText5, parameters5);

                //Inserir Conteudo
                foreach (Conteudo conteudo in evento.conteudo)
                {
                    var retornoPalestrante = this.InserirConteudo(conteudo, evento.codigo);
                }

                return contexto.ExecutaComando(commandText, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int InserirPalestrante(Usuario palestrante, int CodigoEvento)
        {
            try
            {
                const string commandText = "INSERT INTO palestrante (cod_usuario, cod_evento) VALUES (@cod_usuario, @cod_evento) ";

                var parameters = new Dictionary<string, object>
                    {
                        {"cod_usuario", palestrante.codigo},
                        {"cod_evento", CodigoEvento}
                    };

                return contexto.ExecutaComando(commandText, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int InserirUrlVideo(Video video, int CodigoEvento)
        {
            try
            {
                const string commandText = "INSERT INTO video (cod_evento, url) VALUES (@cod_evento, @url) ";

                var parameters = new Dictionary<string, object>
                    {
                        {"cod_evento", CodigoEvento},
                        {"url", video.url}
                    };

                return contexto.ExecutaComando(commandText, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InserirConteudo(Conteudo conteudo, int CodigoEvento)
        {
            try
            {
                const string commandText = "INSERT INTO conteudo (cod_evento, tipo_conteudo, descricao, nome_anexo, anexo, cod_usuario) VALUES (@cod_evento, @tipo_conteudo, @descricao, @nome_anexo, @anexo, @cod_usuario) ";

                var parameters = new Dictionary<string, object>
                    {
                        {"cod_evento", CodigoEvento},
                        {"tipo_conteudo", conteudo.tipoConteudo},
                        {"descricao", conteudo.descricao},
                        {"nome_anexo", conteudo.nomeAnexo},
                        {"anexo", conteudo.anexo},
                        {"cod_usuario", conteudo.usuario.codigo }
                    };

                return contexto.ExecutaComando(commandText, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Salvar(Evento evento)
        {
            try
            {
                if (evento.codigo > 0)
                    Alterar(evento);
                else
                    Inserir(evento);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Evento BuscarPorCodigo(int CodigoEvento)
        {
            var evento = new Evento();
            const string strQuery = "SELECT evt.codigo, evt.nome, evt.descricao, evt.dataHoraInicioInscricao, evt.dataHoraFimInscricao, evt.dataHoraInicial, evt.dataHoraFinal, evt.cod_endereco, evt.vagas, evt.pago, evt.valor, evt.cod_categoria, evt.nome_imagem, evt.imagem, evt.ativo, evt.cod_organizador, " +
                                        "cat.categoria, " +
                                        "ende.logradouro, ende.numero, ende.bairro, ende.cidade, ende.cod_uf, ende.cep, " +
                                        "uf.uf, uf.estado, " +
                                        "(SELECT COUNT(codigo) FROM inscricao WHERE cod_evento = evt.codigo AND status_presenca <> 'Cancelada') AS inscritos " +
                                    "FROM evento evt " +
                                        "INNER JOIN endereco ende ON ende.codigo = evt.cod_endereco " +
                                        "INNER JOIN uf uf ON uf.codigo = ende.cod_uf " +
                                        "INNER JOIN categoria cat ON cat.codigo = evt.cod_categoria "+
                                        "WHERE evt.codigo = @codigo";

            var parametros = new Dictionary<string, object>
            {
                {"codigo", CodigoEvento}
            };
            var rows = contexto.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempEvento = new Evento
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0"),
                    nome = row["nome"].ToString(),
                    descricao = row["descricao"].ToString(),
                    dataHoraInicioInscricao = (!string.IsNullOrEmpty(row["dataHoraInicioInscricao"].ToString()) ? DateTime.Parse(row["dataHoraInicioInscricao"].ToString()) : DateTime.Parse("")),
                    dataHoraFimInscricao = (!string.IsNullOrEmpty(row["dataHoraFimInscricao"].ToString()) ? DateTime.Parse(row["dataHoraFimInscricao"].ToString()) : DateTime.Parse("")),
                    dataHoraInicio = DateTime.Parse(row["dataHoraInicial"].ToString()),
                    dataHoraFim = DateTime.Parse(row["dataHoraFinal"].ToString()),
                    vagas = int.Parse(row["vagas"].ToString()),
                    pago = (row["pago"].ToString() == "S") ? true : false,
                    valor = double.Parse(!string.IsNullOrEmpty(row["valor"].ToString()) ? row["valor"].ToString() : "0"),
                    nomeImagem = row["nome_imagem"].ToString(),
                    imagem = (byte[])row["imagem"],
                    ativo = char.Parse(row["ativo"].ToString()),
                    inscritos = int.Parse(!string.IsNullOrEmpty(row["inscritos"].ToString()) ? row["inscritos"].ToString() : "0"),
                    organizador = new Usuario()
                    {
                        codigo = int.Parse(!string.IsNullOrEmpty(row["cod_organizador"].ToString()) ? row["cod_organizador"].ToString() : "0")
                    }
                };
                if (tempEvento.valor == 0) tempEvento.valor = null;
                var tempCategoria = new Categoria
                {
                    codigo = int.Parse(row["cod_categoria"].ToString()),
                    categoria = row["categoria"].ToString()
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
                tempEvento.endereco = tempEndereco;
                tempEvento.categoria = tempCategoria;
                tempEvento.palestrante = this.BuscaPalestrantesEvento(tempEvento.codigo);
                tempEvento.videos = this.BuscaVideosEvento(tempEvento.codigo);
                tempEvento.conteudo = this.BuscaConteudoEvento(tempEvento.codigo);

                tempEvento.organizador = new UsuarioRepositorio().BuscarPorCodigo(tempEvento.organizador.codigo);
                tempEvento.comentarios = new ComentarioRepositorio().BuscarComentariosPorEvento(tempEvento.codigo);

                evento = tempEvento;
            }

            return evento;
        }

        public List<Evento> ListaEventos(object Filtros)
        {
            Type objTipo = Filtros.GetType();
            PropertyInfo[] objPropriedade = objTipo.GetProperties();

            var lstEvento = new List<Evento>();
            string strQuery = "SELECT evt.codigo, evt.nome, evt.descricao, evt.dataHoraInicioInscricao, evt.dataHoraFimInscricao, evt.dataHoraInicial, evt.dataHoraFinal, evt.cod_endereco, evt.vagas, evt.pago, evt.valor, evt.cod_categoria, evt.nome_imagem, evt.imagem, evt.ativo, evt.cod_organizador, " +
                                        "cat.categoria, "+
                                        "ende.logradouro, ende.numero, ende.bairro, ende.cidade, ende.cod_uf, ende.cep, "+
                                        "uf.uf, uf.estado, "+
                                        "(SELECT COUNT(codigo) FROM inscricao WHERE cod_evento = evt.codigo AND status_presenca <> 'Cancelada') AS inscritos " +
                                    "FROM evento evt " +
                                        "INNER JOIN endereco ende ON ende.codigo = evt.cod_endereco "+
                                        "INNER JOIN uf uf ON uf.codigo = ende.cod_uf "+
                                        "INNER JOIN categoria cat ON cat.codigo = evt.cod_categoria "+
                                    "WHERE 1=1 ";

            if (!objPropriedade[0].GetValue(Filtros, null).Equals(""))
            {
                strQuery += "AND uf.codigo = @uf ";
            }

            if (!objPropriedade[1].GetValue(Filtros, null).Equals(""))
            {
                strQuery += "AND cat.codigo = @categoria ";
            }

            if (!objPropriedade[2].GetValue(Filtros, null).Equals(""))
            {
                strQuery += "AND UPPER(evt.nome) LIKE UPPER('%"+ objPropriedade[2].GetValue(Filtros, null) + "%') ";
            }

            var parametros = new Dictionary<string, object>();

            //Parametros
            if (!objPropriedade[0].GetValue(Filtros, null).Equals(""))
            {
                parametros.Add("uf", objPropriedade[0].GetValue(Filtros, null));
            }

            if (!objPropriedade[1].GetValue(Filtros, null).Equals(""))
            {
                parametros.Add("categoria", objPropriedade[1].GetValue(Filtros, null));
            }

            //if (!objPropriedade[2].GetValue(Filtros, null).Equals(""))
            //{
            //    parametros.Add("nome", objPropriedade[2].GetValue(Filtros, null));
            //}


            var rows = contexto.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempEvento = new Evento
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0"),
                    nome = row["nome"].ToString(),
                    descricao = row["descricao"].ToString(),
                    dataHoraInicioInscricao = DateTime.Parse(row["dataHoraInicioInscricao"].ToString()),
                    dataHoraFimInscricao = DateTime.Parse(row["dataHoraFimInscricao"].ToString()),
                    dataHoraInicio = DateTime.Parse(row["dataHoraInicial"].ToString()),
                    dataHoraFim = DateTime.Parse(row["dataHoraFinal"].ToString()),
                    vagas = int.Parse(row["vagas"].ToString()),
                    pago = (row["pago"].ToString() == "S")? true : false,
                    valor = double.Parse(row["valor"].ToString()),
                    nomeImagem = row["nome_imagem"].ToString(),
                    imagem = (byte[])row["imagem"],
                    ativo = char.Parse(row["ativo"].ToString()),
                    inscritos = int.Parse(!string.IsNullOrEmpty(row["inscritos"].ToString()) ? row["inscritos"].ToString() : "0"),
                    organizador = new Usuario()
                    {
                        codigo = int.Parse(!string.IsNullOrEmpty(row["cod_organizador"].ToString()) ? row["cod_organizador"].ToString() : "0")
                    }
                };
                if(tempEvento.valor == 0) tempEvento.valor = null;
                tempEvento.strImagem = (tempEvento.imagem != null)?"data:image/png;base64," + Convert.ToBase64String(tempEvento.imagem) : "";
                var tempCategoria = new Categoria
                {
                    codigo = int.Parse(row["cod_categoria"].ToString()),
                    categoria = row["categoria"].ToString()
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
                tempEvento.endereco = tempEndereco;
                tempEvento.categoria = tempCategoria;
                tempEvento.palestrante = this.BuscaPalestrantesEvento(tempEvento.codigo);
                tempEvento.videos = this.BuscaVideosEvento(tempEvento.codigo);
                tempEvento.conteudo = this.BuscaConteudoEvento(tempEvento.codigo);

                tempEvento.organizador = new UsuarioRepositorio().BuscarPorCodigo(tempEvento.organizador.codigo);
                tempEvento.comentarios = new ComentarioRepositorio().BuscarComentariosPorEvento(tempEvento.codigo);

                lstEvento.Add(tempEvento);
            }

            return lstEvento;
        }

        public List<Usuario> BuscaPalestrantesEvento(int CodigoEvento)
        {
            var pal = new List<Usuario>();
            const string strQuery = "SELECT pal.codigo, pal.cod_usuario, pal.cod_evento, "+
                                        "usu.nome, usu.rg, usu.cpf, usu.cod_endereco, usu.telefone, usu.email, usu.ativo, "+
                                        "ende.logradouro, ende.numero, ende.bairro, ende.cidade, ende.cod_uf, "+
                                        "uf.uf, uf.estado "+
                                     "FROM palestrante pal "+
                                        "INNER JOIN usuario usu ON usu.codigo = pal.cod_usuario "+
                                        "INNER JOIN endereco ende ON ende.codigo = usu.cod_endereco "+
                                        "INNER JOIN uf uf ON uf.codigo = ende.cod_uf "+
                                        "WHERE cod_evento = @cod_evento";
            var parametros = new Dictionary<string, object>
            {
                {"cod_evento", CodigoEvento}
            };
            var rows = contexto.ExecutaComandoComRetorno(strQuery, parametros);

            foreach (var row in rows)
            {
                var tempPalestrante = new Usuario
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["cod_usuario"].ToString()) ? row["cod_usuario"].ToString() : "0"),
                    nome = row["nome"].ToString(),
                    rg = row["rg"].ToString(),
                    cpf = row["cpf"].ToString(),
                    telefone = row["telefone"].ToString(),
                    email = row["email"].ToString(),
                    ativo = char.Parse(row["ativo"].ToString())

                };
                var tempEndereco = new Endereco
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["cod_endereco"].ToString()) ? row["cod_endereco"].ToString() : "0"),
                    logradouro = row["logradouro"].ToString(),
                    numero = row["numero"].ToString(),
                    bairro = row["bairro"].ToString(),
                    cidade = row["cidade"].ToString()
                };
                var tempUf = new Uf
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["cod_uf"].ToString()) ? row["cod_uf"].ToString() : "0"),
                    uf = row["uf"].ToString(),
                    estado = row["estado"].ToString()
                };
                tempEndereco.uf = tempUf;
                tempPalestrante.endereco = tempEndereco;

                pal.Add(tempPalestrante);
            }

            return pal;
        }

        public List<Video> BuscaVideosEvento(int CodigoEvento)
        {
            var videos = new List<Video>();
            const string strQuery = "SELECT codigo, url "+
                                    "FROM video "+
                                    "WHERE cod_evento = @cod_evento";
            var parametros = new Dictionary<string, object>
            {
                {"cod_evento", CodigoEvento}
            };
            var rows = contexto.ExecutaComandoComRetorno(strQuery, parametros);

            foreach (var row in rows)
            {
                var tempVideo = new Video
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0"),
                    url = row["url"].ToString()

                };

                videos.Add(tempVideo);
            }

            return videos;
        }

        public List<Conteudo> BuscaConteudoEvento(int CodigoEvento)
        {
            var conteudo = new List<Conteudo>();
            const string strQuery = "SELECT codigo, tipo_conteudo, descricao, nome_anexo, anexo, cod_usuario "+
                                    "FROM conteudo "+
                                    "WHERE cod_evento = @cod_evento";
            var parametros = new Dictionary<string, object>
            {
                {"cod_evento", CodigoEvento}
            };
            var rows = contexto.ExecutaComandoComRetorno(strQuery, parametros);

            foreach (var row in rows)
            {
                var tempConteudo = new Conteudo
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0"),
                    tipoConteudo = row["tipo_conteudo"].ToString(),
                    descricao = row["descricao"].ToString(),
                    nomeAnexo = row["nome_anexo"].ToString(),
                    anexo = (byte[])row["anexo"]

                };

                var tempUsuario = new Usuario
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["cod_usuario"].ToString()) ? row["cod_usuario"].ToString() : "0")
                };

                tempConteudo.usuario = tempUsuario;

                conteudo.Add(tempConteudo);
            }

            return conteudo;
        }

        public int AtivarInativarEvento(int CodigoEvento, char elemento)
        {
            try
            {
                const string commandText = "UPDATE evento SET ativo = @ativo WHERE codigo = @codigo ";

                var parameters = new Dictionary<string, object>
                    {
                        {"ativo", elemento},
                        {"codigo", CodigoEvento}
                    };

                return contexto.ExecutaComando(commandText, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Evento> ListaEventosPorPalestrante(int CodigoPalestrante)
        {
            var lstEvento = new List<Evento>();
            string strQuery = "SELECT "+
                                    "evt.codigo, evt.nome, evt.descricao, evt.dataHoraInicioInscricao, evt.dataHoraFimInscricao, evt.dataHoraInicial, evt.dataHoraFinal, evt.cod_endereco, evt.vagas, evt.pago, evt.valor, evt.cod_categoria, evt.nome_imagem, evt.ativo, (SELECT COUNT(codigo) FROM inscricao WHERE cod_evento = evt.codigo AND status_presenca <> 'Cancelada') AS inscritos, evt.cod_organizador " +
                                "FROM palestrante pal "+
                                "INNER JOIN evento evt ON evt.codigo = pal.cod_evento "+
                                "WHERE pal.cod_usuario = @cod_usuario ";

            var parametros = new Dictionary<string, object>
            {
                {"cod_usuario", CodigoPalestrante}
            };

            var rows = contexto.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempEvento = new Evento
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0"),
                    nome = row["nome"].ToString(),
                    descricao = row["descricao"].ToString(),
                    dataHoraInicioInscricao = DateTime.Parse(row["dataHoraInicioInscricao"].ToString()),
                    dataHoraFimInscricao = DateTime.Parse(row["dataHoraFimInscricao"].ToString()),
                    dataHoraInicio = DateTime.Parse(row["dataHoraInicial"].ToString()),
                    dataHoraFim = DateTime.Parse(row["dataHoraFinal"].ToString()),
                    vagas = int.Parse(row["vagas"].ToString()),
                    pago = (row["pago"].ToString() == "S") ? true : false,
                    valor = double.Parse(row["valor"].ToString()),
                    nomeImagem = row["nome_imagem"].ToString(),
                    ativo = char.Parse(row["ativo"].ToString()),
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
                var tempEndereco = new Endereco
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["cod_endereco"].ToString()) ? row["cod_endereco"].ToString() : "0")
                };

                tempEvento.endereco = tempEndereco;
                tempEvento.categoria = tempCategoria;
                tempEvento.conteudo = this.BuscaConteudoEvento(tempEvento.codigo);

                tempEvento.organizador = new UsuarioRepositorio().BuscarPorCodigo(tempEvento.organizador.codigo);

                lstEvento.Add(tempEvento);
            }

            return lstEvento;
        }

        public List<Conteudo> BuscarConteudoPorPalestrante(int CodigoEvento, int CodigoPalestrante)
        {
            var conteudo = new List<Conteudo>();
            const string strQuery = "SELECT codigo, tipo_conteudo, descricao, nome_anexo, anexo, cod_usuario " +
                                    "FROM conteudo " +
                                    "WHERE cod_evento = @cod_evento AND cod_usuario = @cod_usuario";
            var parametros = new Dictionary<string, object>
            {
                {"cod_evento", CodigoEvento},
                {"cod_usuario", CodigoPalestrante}
            };
            var rows = contexto.ExecutaComandoComRetorno(strQuery, parametros);

            foreach (var row in rows)
            {
                var tempConteudo = new Conteudo
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0"),
                    tipoConteudo = row["tipo_conteudo"].ToString(),
                    descricao = row["descricao"].ToString(),
                    nomeAnexo = row["nome_anexo"].ToString(),
                    anexo = (byte[])row["anexo"]

                };

                var tempUsuario = new Usuario
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["cod_usuario"].ToString()) ? row["cod_usuario"].ToString() : "0")
                };

                tempConteudo.usuario = tempUsuario;

                conteudo.Add(tempConteudo);
            }

            return conteudo;
        }

        public int ApagarConteudo(int CodigoConteudo)
        {
            //Apagar conteúdo
            var commandText5 = "DELETE FROM conteudo WHERE codigo = @codigo";

            var parameters5 = new Dictionary<string, object>
                {
                    {"codigo", CodigoConteudo}
                };

            return contexto.ExecutaComando(commandText5, parameters5);
        }

        public Usuario BuscarOrganizadorEvento(int codigo)
        {
            var usuario = new List<Usuario>();
            const string strQuery = "SELECT usu.codigo, usu.nome, usu.rg, usu.cpf, usu.cod_endereco, usu.telefone, usu.email, usu.senha, usu.ativo, usu.nivel_acesso, ende.logradouro, ende.numero, ende.bairro, ende.cidade, ende.cod_uf, ende.cep, estado.uf, estado.estado, nivel.nivel_acesso AS nomeNivel "+
                                    "FROM evento evt "+
                                    "INNER JOIN usuario usu ON usu.codigo = evt.cod_organizador "+
                                    "INNER JOIN endereco ende ON ende.codigo = usu.cod_endereco "+
                                    "INNER JOIN uf estado ON estado.codigo = ende.cod_uf "+
                                    "INNER JOIN nivelacesso nivel ON nivel.codigo = usu.nivel_acesso "+
                                    "WHERE evt.codigo = @codigo";
            var parametros = new Dictionary<string, object>
            {
                {"codigo", codigo}
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
                    senha = row["senha"].ToString(),
                    ativo = char.Parse(row["ativo"].ToString()),
                    statusInscricao = "Organizador"
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

                usuario.Add(tempUsuario);
            }

            return usuario.FirstOrDefault();
        }
    }
}