using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GerenciadorEventos.Entidades;

namespace GerenciadorEventos.Repositorio
{
    public class UfRepositorio
    {
        private readonly Contexto contexto;

        public UfRepositorio()
        {
            contexto = new Contexto();
        }

        public List<Uf> ListaUf()
        {
            var listaUf = new List<Uf>();
            const string strQuery = "SELECT codigo, uf, estado FROM uf";

            var rows = contexto.ExecutaComandoComRetorno(strQuery, null);
            foreach (var row in rows)
            {
                var tempUf = new Uf
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0"),
                    uf = row["uf"].ToString(),
                    estado = row["estado"].ToString()
                };

                listaUf.Add(tempUf);
            }

            return listaUf;
        }
    }
}