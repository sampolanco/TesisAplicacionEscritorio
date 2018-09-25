using ProyectoTerminal2.controller;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ProyectoTerminal2.views
{
    public partial class EscritorioView : Form
    {
        
        EscritorioController escritorioController;
        int contador = 0;
        int fila = 0;
        int columna = 0;
        public EscritorioView(UsuarioController usuarioController)
        {
            try
            {
                InitializeComponent();
                //Se crea la instancia del controller y se le pasa la barra de progreso y el controller del usuario
                this.escritorioController = new EscritorioController(this.barraProgreso, usuarioController);
                inicializarValores();
            }
            catch { }
        }
        private bool inicializarValores()
        {
            try
            {
                lblOrigen.Text = escritorioController.obtenerDireccionOrigen();
                lblDestino.Text = escritorioController.obtenerDireccionResultados();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void btnComenzarAnalisis_Click(object sender, EventArgs e)
        {
            try
            {
                //Se deshabilita el boton de subir informacion
                btnSubirInformacion.Enabled = false;
                //Borra cualquier contenido actual
                tablePanel.Controls.Clear();
                
                //Estado del analisis
                txtProgreso.Text = "Progreso del analisis";
                //Se procesan las imagenes
                int result=escritorioController.comenzarAnalisisClick();
                if (result >= 0)
                {
                    //Se pintan los resultados
                    pintarResultados();
                    //Se termino de pintar
                    txtProgreso.Text = "Analisis terminado";
                    //Numero de mangos enfermos
                    txtMangosEnfermos.Text = escritorioController.contarMangosEnfermos().ToString();
                    //Apto para subir resultados
                    if (result == 1)
                        btnSubirInformacion.Enabled = true;
                }
            }
            catch
            {
                return;
            }
        }
        //Se hizo click en cambiar ruta de origen de datos
        private void btnOrigen_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    escritorioController.origenClick(folderBrowserDialog.SelectedPath);
                    inicializarValores();
                }
            }
            catch { }
        }
        
        public int subir() {
            return 1;
        }

        private void btnCambiarRutas_Click(object sender, EventArgs e)
        {
            try
            {
                //Se busca la carpeta
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    escritorioController.origenClick(folderBrowserDialog.SelectedPath);
                    inicializarValores();
                }
            }
            catch { }
        }

        //
        public bool pintarResultados()
        {
            try
            {
                int tieneManchas = 0;
                //Se obtiene la ruta donde se guardaron los resultados
                string carpetaResultados = escritorioController.obtenerDireccionResultados();
                //Abrir carpeta de imagenes resultado
                DirectoryInfo di = new DirectoryInfo(carpetaResultados);
                string nombreImagen = "";
                //Borra cualquier contenido actual
                tablePanel.Controls.Clear();
                fila=0;
                columna = 0;
                //Para cada imagen en la carpeta de resultados
                foreach (var imagen in di.GetFiles("*.jpg"))
                {
                    //Se guarda el nombre de la imaen
                    nombreImagen = imagen.Name;
                    //si esta seleccionado mostrar solo imagenes con manchas
                    if (checkBoxMostrar.Checked == true)
                    {
                        //Se obtiene el indice que indica si tiene plaga
                        tieneManchas = int.Parse((nombreImagen).Split(new Char[] { '_' })[0]);
                        //si esta sano pasar al siguiente elemento
                        if(tieneManchas==2)
                            continue;
                    }
                    //Se crea un nuevo panel para pintarlo en el el tablePanel
                    Panel miPanel = new System.Windows.Forms.Panel();
                    //se asigna una imagen background al panel
                    miPanel.BackgroundImage = Image.FromFile(carpetaResultados + "\\" + nombreImagen);
                    miPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                    miPanel.Location = new System.Drawing.Point(95, 3);
                   // miPanel.Dock = DockStyle.Fill;
                    //Se agrega como nombre del panel el nombre de la imagen
                    miPanel.Name = nombreImagen;//contador.ToString();
                    miPanel.Size = new System.Drawing.Size(150, 225);
                    miPanel.TabIndex = 1;
                    //Se asigna la funcion que se ejecutara al hacer click en el panel
                    miPanel.Click += MiPanel_Click;
                    //Existen mas de 8 imagenes en la columna actual
                    if (columna == 9)
                    {
                        //Reiniciar el contador de columna y bajar de fila
                        columna = 0;
                        fila = fila + 1;
                        tablePanel.RowCount = tablePanel.RowCount + 1;
                        //se agrega una nueva fila
                        tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
                    }
                    //Se pinta la imagen
                    tablePanel.Controls.Add(miPanel, columna, fila);
                    //miPanel.Show();
                    //Se pasa a la siguiente columna para pintar
                    columna = columna + 1;
                    contador = contador + 1;
                }
                tablePanel.Refresh();
                return true;
            }
            catch { return false; }
        }
        //Funcion del evento se hizo click en la imagen
        private void MiPanel_Click(object sender, EventArgs e)
        {
            try
            {
                //Se obtiene el nombre de la imagen y su estado
                string nombre = (((Panel)sender).Name).Split(new Char[] {'_'})[2];
                int tieneManchas = int.Parse((((Panel)sender).Name).Split(new Char[] { '_' })[0]);
                //Se obtiene el nombre del panel(nombre deimagen) y se pone en false subirImagen
                escritorioController.quitarResultado(nombre, tieneManchas);
                txtMangosEnfermos.Text = escritorioController.contarMangosEnfermos().ToString();
                //Hacer invisible la imagen
                ((Panel)sender).Visible = false;
            }
            catch { return; }
        }

        private void btnSubirInformacion_Click(object sender, EventArgs e)
        {
            try
            {
                escritorioController.subirResultados(txtComentario.Text);
            }
            catch { return; }
        }

        private void carpetasToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void checkBoxMostrar_CheckedChanged(object sender, EventArgs e)
        {
            pintarResultados();
        }

        private void linkPagina_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://servidortesisv3-samyckl.rhcloud.com");
        }

        private void rutasToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                //Se busca la carpeta
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    escritorioController.origenClick(folderBrowserDialog.SelectedPath);
                    inicializarValores();
                }
            }
            catch { }
        }

        private void analisisToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                //Se deshabilita el boton de subir informacion
                btnSubirInformacion.Enabled = false;
                //Estado del analisis
                txtProgreso.Text = "Progreso del analisis";
                //Se procesan las imagenes
                int result = escritorioController.comenzarAnalisisClick();
                if (result >= 0)
                {
                    //Se pintan los resultados
                    pintarResultados();
                    //Se termino de pintar
                    txtProgreso.Text = "Analisis terminado";
                    //Apto para subir resultados
                    if (result == 1)
                        btnSubirInformacion.Enabled = true;
                }
            }
            catch
            {
                return;
            }
        }

        private void manualDeUsuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://servidortesisv3-samyckl.rhcloud.com//manual//escritorio");

        }

        private void acercaDePestMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://servidortesisv3-samyckl.rhcloud.com//manual");
        }

        private void tuCuentaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://servidortesisv3-samyckl.rhcloud.com");
        }
    }
}
