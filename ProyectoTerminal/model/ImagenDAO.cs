using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoTerminal2.controller
{
    class ImagenDAO
    {
        public bool subirInformacionImagen(string email,string password,string puntos,string comentario)
        {
            Conexion conexion = new Conexion();
            try
            {
                string body = "email=" + email + "&" + "password=" + password + "&" + "puntos=" + puntos+ "&" + "comentario=" + comentario;
                string url = "/punto/";
                return conexion.peticionPost(url, body);
            }
            catch
            {
                return false;
            }
        }
    }
}
