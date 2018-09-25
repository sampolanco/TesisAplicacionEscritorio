using ProyectoTerminal2.model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ProyectoTerminal2.controller
{
    class ImagenController
    {
        RutaImagenController rutaImagenController;
        public List<ImagenDTO> listaDatosImagen = new List<ImagenDTO>();
        ProgressBar barraProgreso;
        ImagenDAO imagenDAO = new ImagenDAO();
        ResultadosDTO resultadosDTO = new ResultadosDTO();
        

        public ImagenController(RutaImagenController rutaImagenController, ProgressBar barra) {
            try
            {
                this.rutaImagenController = rutaImagenController;
                this.barraProgreso = barra;
            }
            catch { }
        }
        //Realizar analizis a imagenes de carpeta
        public int comenzarAnalisisClick()
        {
            //variables
            DateTime tiempo1,tiempo2;
            TimeSpan total;
            string carpetaOrigen="",nombreArchivo="", sdate, secondhalf, firsthalf;
            DateTime fechaCaptura;
            double latitud = 0, longitud = 0;
            bool ignorarImagenesinInfo = false;
            int cantidadImagenes = 0, numeroMangosEnfermos=0;
            //se reinician variables de la informacion procesada y la barra de progreso
            //List<ImagenDTO> listaDatosImagen = new List<ImagenDTO>();
            //int resultado;
            Image imagen;
            PropertyItem gpsLatitudRef, gpsLongituddRef, gpsLatitud, gpsLongitud, propiedadFecha;

            try
            {
                //Instancia de la libreria para realizar el procesamiento
                //Procesamiento.Class1 openCV = new Procesamiento.Class1();
                //OpenCV.Class1 openCV = new OpenCV.Class1();
                
                //Se obtiene la ruta de las imagenes
                carpetaOrigen = rutaImagenController.getOrigen();
                //se eliminan los antiguos resultados (imagenes) en la carpeta
                if (limpiarCarpetaResultados() == false)
                    return -1;
           
                //Abrir carpeta de imagenes
                DirectoryInfo di = new DirectoryInfo(carpetaOrigen);
                //Se obtiene cuantas imagenes se van a procesar
                //y se especifica la longitud de la barra de carga
                cantidadImagenes = di.GetFiles("*.jpg").Length;
                this.barraProgreso.Minimum = 1;
                this.barraProgreso.Maximum = cantidadImagenes;
                barraProgreso.Value = 1;
                barraProgreso.Refresh();
                //Para cada imagen en el directorio se realiza el procesamiento
                foreach (var archivo in di.GetFiles("*.jpg"))
                {
                    //nombre del archivo
                    nombreArchivo = archivo.Name;
                    //Abrir imagen para obtener propiedades
                    imagen = new Bitmap(carpetaOrigen + "\\" + nombreArchivo);
                    PropertyItem[] propItems = imagen.PropertyItems;
                    try {
                        //Se toman las coordenadas GPS
                        gpsLatitudRef = imagen.GetPropertyItem(1);
                        gpsLatitud = imagen.GetPropertyItem(2);
                        gpsLongituddRef = imagen.GetPropertyItem(3);
                        gpsLongitud = imagen.GetPropertyItem(4);
                        //Se ejecuta el algoritmo para generar la coordenada
                        latitud = obtenerCoordenadasGPS(gpsLatitudRef, gpsLatitud);
                        longitud = obtenerCoordenadasGPS(gpsLongituddRef, gpsLongitud);

                        //Se obtiene la fecha de captura
                        propiedadFecha = imagen.GetPropertyItem(36867);
                        sdate = Encoding.UTF8.GetString(propiedadFecha.Value).Trim();
                        secondhalf = sdate.Substring(sdate.IndexOf(" "), (sdate.Length - sdate.IndexOf(" ")));
                        firsthalf = sdate.Substring(0, 10);
                        firsthalf = firsthalf.Replace(":", "-");
                        sdate = firsthalf + secondhalf;
                        fechaCaptura = DateTime.Parse(sdate);
                    }
                    catch {
                        //Si aun no se ignoran las imagenes sin datos de la posicio o fecha
                        if (ignorarImagenesinInfo == false)
                        {
                            //Cuando la imagen no contiene informacion de la posicion o la fecha
                            DialogResult result1 = MessageBox.Show("La imagen " + nombreArchivo +
                                " no contiene informacion de la posicion geografica o la fecha de captura." +
                                "¿Desea Continuar?",
                            "Examinar imagen", MessageBoxButtons.YesNo);
                            if (result1 == DialogResult.Yes)
                            {
                                DialogResult result2 = MessageBox.Show("Realizar esta acción para todas las imagenes",
                                "Examinar imagen", MessageBoxButtons.YesNo);
                                //Realizar la operacion para todas las imagenes
                                if (result2 == DialogResult.Yes)
                                {
                                    //Ignorar imagenes que no contienen informacion necesaria
                                    ignorarImagenesinInfo = true;
                                }
                            }
                            //No se desea continuar el procesamiento
                            else
                            {
                                return -1;
                            }
                        }
                        fechaCaptura = new DateTime();
                    }
                   
                    //Ejecutar el algoritmo de procesamiento
                    Procesamiento.Class1 openCV = new Procesamiento.Class1();
                    tiempo1 = DateTime.Now;
                    numeroMangosEnfermos =openCV.procesamiento(carpetaOrigen, archivo.Name);
                    tiempo2 = DateTime.Now;
                    total = new TimeSpan(tiempo2.Ticks - tiempo1.Ticks);
                    //Se guarda el resultado del procesamiento
                    if (guardarResultado(nombreArchivo, numeroMangosEnfermos, latitud,longitud, fechaCaptura) == false)
                        return -1;
                    //Se incrementa la barra de  carga y se acutualiza
                    barraProgreso.Increment(1);
                    barraProgreso.Refresh();
                }
                //Si se aceptaron imagenes sin datos gps o fecha
                if (ignorarImagenesinInfo == true)
                    return 0;
                else//Apto para subir a internet
                    return 1;
            }
            catch
            {
                return -1;
            }
        }
        //guardar datos de imagen en listaDatosImagen 
        public bool guardarResultado(string nombre, int estado, double latitud, double longitud, DateTime fechaCaptura)
        {
            try
            {
                //Lista donde se guardan los resultados de cada imagen
                ImagenDTO imagenDTO = new ImagenDTO(nombre, estado, latitud, longitud, fechaCaptura);
                listaDatosImagen.Add(imagenDTO);
                return true;
            }
            catch
            {
                return false;
            }
        }
        //Obtener la coordenada GPS de la imagen
        private static double obtenerCoordenadasGPS(PropertyItem propItemRef, PropertyItem propItem)
        {
            //Se toman los grados
            uint gradosNumerador = BitConverter.ToUInt32(propItem.Value, 0);
            uint gradosDenominador = BitConverter.ToUInt32(propItem.Value, 4);
            double grados = gradosNumerador / (double)gradosDenominador;
            //Se toman los minutos
            uint minutosNumerator = BitConverter.ToUInt32(propItem.Value, 8);
            uint minutosDenominator = BitConverter.ToUInt32(propItem.Value, 12);
            double minutos = minutosNumerator / (double)minutosDenominator;
            //Se toman los segundos
            uint segundosNumerator = BitConverter.ToUInt32(propItem.Value, 16);
            uint segundosDenominator = BitConverter.ToUInt32(propItem.Value, 20);
            double segundos = segundosNumerator / (double)segundosDenominator;
            //Se obtiene la coordenada
            double coordenada = grados + (minutos / 60f) + (segundos / 3600f);
            //Se validan las coordenadas negativas
            string gpsRef = System.Text.Encoding.ASCII.GetString(new byte[1] { propItemRef.Value[0] }); //N, S, E, o W
            if (gpsRef == "S" || gpsRef == "W")
                coordenada = 0 - coordenada;
            return coordenada;
        }
        public bool limpiarCarpetaResultados()
        {
            try
            {
                //Se eliminan los datos de clase
                listaDatosImagen = new List<ImagenDTO>();
                // carpeta destino
                string carpetaDestino = rutaImagenController.getDestino();
                //Si existe la carpeta de destino borrar
                if (Directory.Exists(carpetaDestino))
                {
                    // Borrar carpeta
                    Directory.Delete(carpetaDestino,true);
                }
                //Crear carpeta
                Directory.CreateDirectory(carpetaDestino);
                return true;
            }
            catch { return false; }
        }
        public bool agregarComentario(string nombreImagen,string comentario)
        {
            return true;
        }
        //-----------------------------------------Cambiar el estado de subir datos de imagen a web service
        public bool quitarResultado(string nombreImagen,int tieneManchas)
        {
            int mangosEnfermos = 0;
            //Se busca la imagen en la lista
            foreach (var x in listaDatosImagen)
            {
                //Si se encontro la imagen
                if (x.getImagenenNombre() == nombreImagen)
                {
                    mangosEnfermos = x.getNumeroMangosEnfermos();
                    //Si aun existen mangos enfermos y la imagen tiene manchas 
                    //se disminuye la cantidad de mangos enfermos para la imagen
                    if (mangosEnfermos > 0 && tieneManchas==1)
                        x.setNumeroMangosEnfermos(mangosEnfermos-1);

                    return true;
                }
            }
            return false;
        }
        public int contarMangosEnfermos()
        {
            int mangosEnfermos = 0;
            //Se busca la imagen en la lista
            foreach (var x in listaDatosImagen)
            {
                //Si se encontro la imagen
                mangosEnfermos= mangosEnfermos+x.getNumeroMangosEnfermos();
            }
            return mangosEnfermos;
        }
        //-------------------------------------------------------Subir puntos al web service
        public bool subirResultados(string email, string password,string comentario)
        {
            try
            {
                //Se envia la informacion al web service
                imagenDAO.subirInformacionImagen(email, password, generarCadenaPuntos(), comentario);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string generarCadenaPuntos() {
            string retorno = "{\"punto\":[";//{ "punto":[{"latitud":"x","longitud":"x","numeroMangosEnfermos":"x","fechaCaptura":"x"}]}
            //Se obtiene el maximo numero de imagenes y un contador que ayuda a formar el JSON
            int numeroImagenes = listaDatosImagen.Count,contador=1;
            //Para cada una de las imagenes  de la lista de imagenes procesadas
            foreach (var imagen in listaDatosImagen)
            {
                //No es la ultima imagen
                if (contador < numeroImagenes)
                {
                    //retorno = retorno + '{'+'"'+"latitud"+'"'+':'+imagen.getLatitud().ToString() + "longitud" + "numeroMangosEnfermos";
                    retorno = retorno + "{\"latitud\":\"" + imagen.getLatitud().ToString().Replace(',', '.') + "\"" +
                                        ",\"longitud\":\"" + imagen.getLongitud().ToString().Replace(',', '.') + "\"" +
                                        ",\"fecha\":\"" + imagen.getFechaCaptura().ToString("yyyy") + "-" + imagen.getFechaCaptura().ToString("MM") + "-" + imagen.getFechaCaptura().ToString("dd") + "\"" +
                                        ",\"numeroMangosEnfermos\":\"" + imagen.getNumeroMangosEnfermos().ToString() + "\"},";
                }
                else
                {
                    retorno = retorno + "{\"latitud\":\"" + imagen.getLatitud().ToString().Replace(',', '.') + "\"" +
                                        ",\"longitud\":\"" + imagen.getLongitud().ToString().Replace(',', '.') + "\"" +
                                        ",\"fecha\":\"" + imagen.getFechaCaptura().ToString("yyyy")+"-"+ imagen.getFechaCaptura().ToString("MM") + "-"+ imagen.getFechaCaptura().ToString("dd") + "\"" +
                                        ",\"numeroMangosEnfermos\":\"" + imagen.getNumeroMangosEnfermos().ToString() + "\"}]}";
                }
                contador++;
            }
            return retorno;
        }

        /*//Ejecutar el algoritmo de procesamiento
        OpenCV.ProcesamientoImagen.Class1 opeasdnCV = new OpenCV.ProcesamientoImagen.Class1();
        openCV.cargarImagen(carpetaOrigen,archivo.Name);
        openCV.verImagen(carpetaOrigen, archivo.Name);
        openCV.cargarImagen2(carpetaOrigen,archivo.Name);*/

        /*//Hilo para correr el procesamiento de imagen
                        Alpha oAlpha = new Alpha(carpetaOrigen, nombreArchivo);
                        Thread oThread = new Thread(new ThreadStart(oAlpha.hiloProcesamiento));
                        oThread.Start();
                        //Esperar a que el hilo corra
                        while (!oThread.IsAlive) ;
                        //Dormir hilo principal
                        Thread.Sleep(1);
                        //Esperar a que el hilo acabe
                        oThread.Join();
                        //Eliminar Hilo
                        oThread.Abort();
                        //Console.WriteLine("Hilo finalizado");
                    //Si al guardar los datos de la imagen da error retornar
                    
         
                  public class Alpha
            {
                ProcesamientoImagen.Class1 openCV = new ProcesamientoImagen.Class1();
                String carpetaOrigen,nombre;
        
                public Alpha(string carpetaOrigen,string nombre) {
                    this.carpetaOrigen = carpetaOrigen;
                    this.nombre = nombre;
                }
                // This method that will be called when the thread is started
                public void hiloProcesamiento()
                {
                    Console.WriteLine("Ejecutar procesamiento para" + this.nombre);
                    try { 
                        openCV.cargarImagen2(this.carpetaOrigen, this.nombre);
                        return;
                    }
                    catch
                    {
                        return;
                    }
                }
            };
         */
    }
}
