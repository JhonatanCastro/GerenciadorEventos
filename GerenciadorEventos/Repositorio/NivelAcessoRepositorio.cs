using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GerenciadorEventos.Entidades;

namespace GerenciadorEventos.Repositorio
{
    public class NivelAcessoRepositorio
    {
        private readonly Contexto contexto;

        public NivelAcessoRepositorio()
        {
            contexto = new Contexto();
        }

        public List<NivelAcesso> ListaNivelAcesso()
        {
            var listaNivel = new List<NivelAcesso>();
            const string strQuery = "SELECT codigo, nivel_acesso FROM nivelacesso WHERE codigo <> 4";

            var rows = contexto.ExecutaComandoComRetorno(strQuery, null);
            foreach (var row in rows)
            {
                var tempNivelAcesso = new NivelAcesso
                {
                    codigo = int.Parse(!string.IsNullOrEmpty(row["codigo"].ToString()) ? row["codigo"].ToString() : "0"),
                    nivelAcesso = row["nivel_acesso"].ToString()
                };

                listaNivel.Add(tempNivelAcesso);
            }

            return listaNivel;
        }
    }
}