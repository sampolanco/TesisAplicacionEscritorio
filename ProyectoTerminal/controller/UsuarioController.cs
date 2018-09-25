using ProyectoTerminal2.model;

namespace ProyectoTerminal2.controller
{
    public class UsuarioController
    {
        UsuarioDTO usuarioDTO;
        UsuarioDAO usuarioDAO = new UsuarioDAO();
        public bool validarUsuario(string email, string password)
        {
            try {
                bool salida=usuarioDAO.validarUsuario(email, password);
                if (salida == true)//Usuario valido
                {
                    usuarioDTO = new UsuarioDTO(password, email);
                    return true;
                }
                else
                    return false;
            }
            catch {
                return false;
            }
        }

        public string getEmail()
        {
            try
            {
                return usuarioDTO.getEmail();
            }
            catch
            {
                return "";
            }
        }
        public string getNombre()
        {
            try
            {
                return usuarioDTO.getNombre();
            }
            catch
            {
                return "";
            }
        }
        public string getPassword()
        {
            try
            {
                return usuarioDTO.getPassword();
            }
            catch
            {
                return "";
            }
        }
    }
}
