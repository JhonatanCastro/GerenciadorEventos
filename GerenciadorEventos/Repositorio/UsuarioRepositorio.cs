using System.Collections.Generic;
using System.Linq;
using GerenciadorEventos.Entidades;
using System;

namespace GerenciadorEventos.Repositorio
{
    public class UsuarioRepositorio
    {
        private readonly Contexto contexto;

        public UsuarioRepositorio()
        {
            contexto = new Contexto();
        }

        public int Inserir(Usuario usuario)
        {
            try
            {
                //Insere Endereço
                const string commandText2 = "INSERT INTO endereco (logradouro, numero, bairro, cidade, cod_uf, cep) VALUES (@logradouro, @numero, @bairro, @cidade, @cod_uf, @cep) ";

                var parameters2 = new Dictionary<string, object>
                {
                    {"logradouro", usuario.endereco.logradouro},
                    {"numero", usuario.endereco.numero},
                    {"bairro", usuario.endereco.bairro},
                    {"cidade", usuario.endereco.cidade},
                    {"cod_uf", usuario.endereco.uf.codigo},
                    {"cep", usuario.endereco.cep}
                };
                
                var retornoEndereco = contexto.ExecutaComando(commandText2, parameters2);

                //Buscar Endereço Inserido
                const string strQuery = "SELECT codigo FROM endereco WHERE codigo = (SELECT MAX(codigo) FROM endereco) ORDER by codigo DESC";
                
                var rows = contexto.ExecutaComandoComRetorno(strQuery, null);

                int codigo = 0;
                foreach (var row in rows)
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0");
                }

                //Insere Usuario
                const string commandText = "INSERT INTO usuario (nome, rg, cpf, cod_endereco, telefone, email, senha, ativo, nivel_acesso, liberado) VALUES (@nome,@rg,@cpf,@cod_endereco,@telefone,@email,@senha,@ativo,@nivel_acesso,@liberado) ";

                var parameters = new Dictionary<string, object>
                {
                    {"nome", usuario.nome},
                    {"rg", usuario.rg},
                    {"cpf", usuario.cpf},
                    {"cod_endereco", codigo},
                    {"telefone", usuario.telefone},
                    {"email", usuario.email},
                    {"senha", usuario.senha},
                    {"ativo", usuario.ativo},
                    {"nivel_acesso", usuario.nivelAcesso.codigo},
                    {"liberado", usuario.liberado}
                };

                return contexto.ExecutaComando(commandText, parameters);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private int Alterar(Usuario usuario)
        {
            try
            {
                //Atualiza Usuario
                var commandText = " UPDATE usuario SET ";
                commandText += " nome = @nome, ";
                commandText += " rg = @rg, ";
                commandText += " cpf = @cpf, ";
                commandText += " cod_endereco = @cod_endereco, ";
                commandText += " telefone = @telefone, ";
                commandText += " email = @email, ";
                commandText += " senha = @senha, ";
                commandText += " ativo = @ativo, ";
                commandText += " nivel_acesso = @nivel_acesso ";
                commandText += " WHERE codigo = @codigo ";

                var parameters = new Dictionary<string, object>
                {
                    {"codigo", usuario.codigo},
                    {"nome", usuario.nome},
                    {"rg", usuario.rg},
                    {"cpf", usuario.cpf},
                    {"cod_endereco", usuario.endereco.codigo},
                    {"telefone", usuario.telefone},
                    {"email", usuario.email},
                    {"senha", usuario.senha},
                    {"ativo", usuario.ativo },
                    {"nivel_acesso", usuario.nivelAcesso.codigo}
                };

                //Atualiza Endereço do Usuario
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
                    {"codigo", usuario.endereco.codigo},
                    {"logradouro", usuario.endereco.logradouro},
                    {"numero", usuario.endereco.numero},
                    {"bairro", usuario.endereco.bairro},
                    {"cidade", usuario.endereco.cidade},
                    {"cod_uf", usuario.endereco.uf.codigo},
                    {"cep", usuario.endereco.cep}
                };

                var resultadoEndereco = contexto.ExecutaComando(commandText, parameters);

                return contexto.ExecutaComando(commandText2, parameters2);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private int TrocarSenha(Usuario usuario)
        {
            try
            {
                var commandText = " UPDATE usuario SET ";
                commandText += " senha = @senha ";
                commandText += " WHERE codigo = @codigo ";

                var parameters = new Dictionary<string, object>
                {
                    {"codigo", usuario.codigo},
                    {"senha", usuario.senha}
                };

                return contexto.ExecutaComando(commandText, parameters);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void Salvar(Usuario usuario)
        {
            try
            {
                if (usuario.codigo > 0)
                    Alterar(usuario);
                else
                    Inserir(usuario);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        public Usuario BuscarPorCodigo(int codigo)
        {
            var usuario = new List<Usuario>();
            const string strQuery = "SELECT usu.codigo, usu.nome, usu.rg, usu.cpf, usu.cod_endereco, usu.telefone, usu.email, usu.senha, usu.ativo, usu.nivel_acesso, ende.logradouro, ende.numero, ende.bairro, ende.cidade, ende.cod_uf, ende.cep, estado.uf, estado.estado, nivel.nivel_acesso AS nomeNivel " +
                                    "FROM usuario usu "+
                                    "INNER JOIN endereco ende ON ende.codigo = usu.cod_endereco "+
                                    "INNER JOIN uf estado ON estado.codigo = ende.cod_uf "+
                                    "INNER JOIN nivelacesso nivel ON nivel.codigo = usu.nivel_acesso " +
                                    "WHERE usu.codigo = @codigo";
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
                    ativo = char.Parse(row["ativo"].ToString())
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

                tempUsuario.eventosInscritos = new InscricaoRepositorio().BuscarInscricoesPorUsuario(tempUsuario.codigo);

                usuario.Add(tempUsuario);
            }

            return usuario.FirstOrDefault();
        }

        public List<Usuario> ListaUsuarios()
        {
            var user = new List<Usuario>();
            const string strQuery = "SELECT usu.codigo, usu.nome, usu.rg, usu.cpf, usu.cod_endereco, usu.telefone, usu.email, usu.senha, usu.ativo, usu.nivel_acesso, usu.liberado, ende.logradouro, ende.numero, ende.bairro, ende.cidade, ende.cod_uf, ende.cep, estado.uf, estado.estado, nivel.nivel_acesso AS nomeNivel " +
                                    "FROM usuario usu " +
                                    "INNER JOIN endereco ende ON ende.codigo = usu.cod_endereco " +
                                    "INNER JOIN uf estado ON estado.codigo = ende.cod_uf "+
                                    "INNER JOIN nivelacesso nivel ON nivel.codigo = usu.nivel_acesso";

            var rows = contexto.ExecutaComandoComRetorno(strQuery, null);
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
                    liberado = char.Parse(row["liberado"].ToString())
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

                tempUsuario.eventosInscritos = new InscricaoRepositorio().BuscarInscricoesPorUsuario(tempUsuario.codigo);

                user.Add(tempUsuario);
            }

            return user;
        }

        public Usuario Autenticacao(Usuario usuario)
        {
            var user = new List<Usuario>();
            const string strQuery = "SELECT usu.codigo, usu.nome, usu.rg, usu.cpf, usu.cod_endereco, usu.telefone, usu.email, usu.senha, usu.ativo, usu.nivel_acesso, ende.logradouro, ende.numero, ende.bairro, ende.cidade, ende.cod_uf, estado.uf, estado.estado, nivel.nivel_acesso AS nomeNivel " +
                                    "FROM usuario usu " +
                                    "INNER JOIN endereco ende ON ende.codigo = usu.cod_endereco " +
                                    "INNER JOIN uf estado ON estado.codigo = ende.cod_uf " +
                                    "INNER JOIN nivelacesso nivel ON nivel.codigo = usu.nivel_acesso " +
                                    "WHERE email = @email AND senha = @senha AND liberado = 'S'";
            var parametros = new Dictionary<string, object>
            {
                {"email", usuario.email},
                {"senha", usuario.senha}
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
                    ativo = char.Parse(row["ativo"].ToString())
                    
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
                    cidade = row["cidade"].ToString()
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

                tempUsuario.eventosInscritos = new InscricaoRepositorio().BuscarInscricoesPorUsuario(tempUsuario.codigo);

                user.Add(tempUsuario);
            }

            return user.FirstOrDefault();
        }

        public int LiberarAcesso(int CodigoUsuario)
        {
            try
            {
                //Atualiza Usuario
                var commandText = " UPDATE usuario SET ";
                commandText += " liberado = @liberado ";
                commandText += " WHERE codigo = @codigo ";

                var parameters = new Dictionary<string, object>
                {
                    {"liberado", 'S'},
                    {"codigo", CodigoUsuario}
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