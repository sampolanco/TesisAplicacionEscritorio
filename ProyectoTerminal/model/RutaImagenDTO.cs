using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoTerminal2.model
{
    class RutaImagenDTO
    {
        private string origen;
        private string salida;
        public RutaImagenDTO()
        {
            //Origen y destino por default
            this.origen = "C:";
            this.salida = "C:";
        }
        //Se guarda la direccion de origen y destino de datos
        public bool setOrigen(string origen) {
            this.origen = origen;
            this.salida = origen + "\\salida";
            return true;
        }
        public string getOrigen()
        {
            return this.origen;
        }
        public string getDestino()
        {
            return this.salida;
        }

    }
}
