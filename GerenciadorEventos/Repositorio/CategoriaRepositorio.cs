using GerenciadorEventos.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GerenciadorEventos.Repositorio
{
    public class CategoriaRepositorio
    {
        private readonly Contexto contexto;

        public CategoriaRepositorio()
        {
            contexto = new Contexto();
        }

        public List<Categoria> ListaCategoria()
        {
            var listaCategoria = new List<Categoria>();
            const string strQuery = "SELECT codigo, categoria FROM categoria";

            var rows = contexto.ExecutaComandoComRetorno(strQuery, null);
            foreach (var row in rows)
            {
                var tempCategoria = new Categoria
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0"),
                    categoria = row["categoria"].ToString()
                };

                listaCategoria.Add(tempCategoria);
            }

            return listaCategoria;
        }
    }
}