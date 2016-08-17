using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace GerenciadorEventos.Repositorio
{
    public class Contexto : IDisposable
    {
        private MySqlConnection conexao;

        public Contexto()
        {
            var conexaoString = ConfigurationManager.ConnectionStrings["gerenciadordeeventos"].ConnectionString;
            conexao = new MySqlConnection(conexaoString);
        }

        public int ExecutaComando(string comandoSQL, Dictionary<string, object> parametros)
        {
            var resultado = 0;
            if (string.IsNullOrEmpty(comandoSQL))
            {
                throw new ArgumentException("O comandoSQL não pode ser nulo ou vazio");
            }
            try
            {
                AbrirConexao();
                var cmdComando = CriarComando(comandoSQL, parametros);
                resultado = cmdComando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FecharConexao();
            }

            return resultado;
        }

        public List<Dictionary<string, object>> ExecutaComandoComRetorno(string comandoSQL, Dictionary<string, object> parametros = null)
        {
            List<Dictionary<string, object>> linhas = null;

            if (string.IsNullOrEmpty(comandoSQL))
            {
                throw new ArgumentException("O comandoSQL não pode ser nulo ou vazio");
            }
            try
            {
                AbrirConexao();
                var cmdComando = CriarComando(comandoSQL, parametros);
                using (var reader = cmdComando.ExecuteReader())
                {
                    linhas = new List<Dictionary<string, object>>();
                    while (reader.Read())
                    {
                        var linha = new Dictionary<string, object>();

                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var nomeDaColuna = reader.GetName(i);
                            var valorDaColuna = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            linha.Add(nomeDaColuna, valorDaColuna);
                        }
                        linhas.Add(linha);
                    }
                }
            }
            finally
            {
                FecharConexao();
            }

            return linhas;
        }

        private MySqlCommand CriarComando(string comandoSQL, Dictionary<string, object> parametros)
        {
            var cmdComando = conexao.CreateCommand();
            cmdComando.CommandText = comandoSQL;
            AdicionarParamatros(cmdComando, parametros);
            return cmdComando;
        }

        private static void AdicionarParamatros(MySqlCommand cmdComando, Dictionary<string, object> parametros)
        {
            if (parametros == null)
                return;

            foreach (var item in parametros)
            {
                var parametro = cmdComando.CreateParameter();
                parametro.ParameterName = item.Key;
                parametro.Value = item.Value ?? DBNull.Value;
                cmdComando.Parameters.Add(parametro);
            }
        }

        private void AbrirConexao()
        {
            if (conexao.State == ConnectionState.Open) return;

            conexao.Open();
        }

        private void FecharConexao()
        {
            if (conexao.State == ConnectionState.Open)
                conexao.Close();
        }

        public void Dispose()
        {
            if (conexao == null) return;

            conexao.Dispose();
            conexao = null;
        }
    }
}