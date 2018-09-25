using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ProyectoTerminal2.controller
{
    class EscritorioController
    {

        RutaImagenController rutaImagenController;
        UsuarioController usuarioController;
        public ImagenController imagenController;

        //Constructor
        public EscritorioController(ProgressBar barra,UsuarioController usuarioController)
        {
            try
            {
                this.usuarioController = usuarioController;
                this.rutaImagenController = new RutaImagenController();
                //Se crea la instancia de ImagenController y se le pasa la clase de ruta y la barra
                this.imagenController = new ImagenController(rutaImagenController, barra);
            }
            catch { }
        }
        //Comenzar el analisis del procesamiento
        public int comenzarAnalisisClick()
        {
            try
            {
                //Algoritmo que procesa imagenes
                int resultado=imagenController.comenzarAnalisisClick();
                MessageBox.Show("Analisis finalizado");
                return resultado;
            }
            catch
            {
                MessageBox.Show("Algo ha salido mal");
                return -1;
            }
        }
        //Cambiar el origen de datos
        public bool origenClick(string ruta) {
            try
            {
               return rutaImagenController.cambiarRutaOrigen(ruta);
            }
            catch
            {
                return false;
            }
        }
        //Obtener Direccion donde se guardan los resultados
        public string obtenerDireccionResultados()
        {
            try
            {
                return rutaImagenController.getDestino();
            }
            catch
            {
                return "";
            }
        }
        //Obtener Direccion de origen de datos
        public string obtenerDireccionOrigen()
        {
            try
            {
                return rutaImagenController.getOrigen();
            }
            catch
            {
                return "";
            }
        }
        //Cambiar el estado de subir datos de imagen a web service
        public bool quitarResultado(string nombre, int tieneManchas)
        {
            try
            {
                imagenController.quitarResultado(nombre, tieneManchas);
                return true;
            }
            catch { return false; }
        }
        public int contarMangosEnfermos()
        {
            try
            {
                return imagenController.contarMangosEnfermos(); ;
            }
            catch { return -1; }
        }
        //Subir resultados imagenes a web service
        public bool subirResultados(string comentario) {
            imagenController.subirResultados(usuarioController.getEmail(),usuarioController.getPassword(),comentario);
            MessageBox.Show("Analisis finalizado");
            return true;
        }
        public string getEmail()
        {
            try
            {
                return usuarioController.getEmail();
            }
            catch { return ""; }
        }
    }
}
