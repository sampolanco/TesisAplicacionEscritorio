using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoTerminal2.model
{
    class ImagenDTO
    {
        string nombre;
        int  numeroMangosEnfermos;
        double latitud;
        double longitud;
        int subir;
        DateTime fechaCaptura;

        public ImagenDTO(string nombre,int numeroMangosEnfermos, double latitud, double longitud, DateTime fechaCaptura) {
            this.nombre = nombre;
            this.numeroMangosEnfermos = numeroMangosEnfermos;
            this.latitud = latitud;
            this.longitud = longitud;
            this.subir = 1;
            this.fechaCaptura = fechaCaptura;
        }

        public string getImagenenNombre()
        {
            try
            {
                return this.nombre;
            }
            catch { return ""; }
        }
        public bool setImagenenNombre(string nombre)
        {
            try
            {
                this.nombre = nombre;
                return true;
            }
            catch { return false; }
        }
        public DateTime getFechaCaptura()
        {
            try
            {
                return this.fechaCaptura;
            }
            catch { return new DateTime(); }
        }
        public bool setFechaCaptura(DateTime fechaCaptura)
        {
            try
            {
                this.fechaCaptura = fechaCaptura;
                return true;
            }
            catch { return false; }
        }
        

        public string getNombre()
        {
            try
            {
                return this.nombre;
            }
            catch { return ""; }
        }

        public int getNumeroMangosEnfermos()
        {
            try
            {
                return this.numeroMangosEnfermos;
            }
            catch { return -1; }
        }
        public bool setNumeroMangosEnfermos(int estado)
        {
            try
            {
                this.numeroMangosEnfermos = estado;
                return true;
            }
            catch { return false; }
        }

        public double getLatitud()
        {
            try
            {
                return this.latitud;
            }
            catch { return 0; }
        }
        public bool setLatitud(double latitud)
        {
            try
            {
                this.latitud = latitud;
                return true;
            }
            catch { return false; }
        }

        public double getLongitud()
        {
            try
            {
                return this.longitud;
            }
            catch { return 0; }
        }
        public bool setLongitud(double longitud)
        {
            try
            {
                this.longitud = longitud;
                return true;
            }
            catch { return false; }
        }

        public int getSubir()
        {
            try
            {
                return this.subir;
            }
            catch { return -1; }
        }
        public bool setSubir(int subir)
        {
            try
            {
                this.subir = subir;
                return true;
            }
            catch { return false; }
        }
    }
}
