using ProyectoTerminal2.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoTerminal2.controller
{
    class RutaImagenController
    {
        //Se instancia la clase imagen
        RutaImagenDTO rutaImagenDTO = new RutaImagenDTO();
        public bool cambiarRutaOrigen(string origen) {
            try
            {
                return rutaImagenDTO.setOrigen(origen);
            }
            catch {
                return false;
            }
        }
        public string getOrigen()
        {
            try
            {
                return rutaImagenDTO.getOrigen();
            }
            catch
            {
                return "";
            }
        }
        public string getDestino()
        {
            try
            {
                return rutaImagenDTO.getDestino();
            }
            catch
            {
                return "";
            }
        }
    }
}
