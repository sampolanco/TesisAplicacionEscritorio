using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoTerminal2.model
{
    class ResultadosDTO
    {
        string comentario;

        public string getComentario()
        {
            try
            {
                return this.comentario;
            }
            catch { return ""; }
        }
        public bool setComentario(string comentario)
        {
            try
            {
                this.comentario = comentario;
                return true;
            }
            catch { return false; }
        }

    }
}
