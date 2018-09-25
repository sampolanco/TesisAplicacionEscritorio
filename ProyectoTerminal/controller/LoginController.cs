using ProyectoTerminal2.views;
using System.Windows.Forms;

namespace ProyectoTerminal2.controller
{
    class LoginController
    {
        UsuarioController usuarioController = new UsuarioController();
        public bool validarClick(string email, string password)
        {
            bool retorno = usuarioController.validarUsuario(email, password);
            if (retorno == true)
            {
                //Se abre la ventana de escritorio
                using (EscritorioView ventana = new EscritorioView(usuarioController))
                {
                    if (ventana.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //Finalizo exitosamente
                        return true;
                    }
                }
            }
            else
            {
                MessageBox.Show("Usuario no valido");
                return false; 
            }
            return false;
        }
    }
}
