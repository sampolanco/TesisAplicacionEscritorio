namespace ProyectoTerminal2.controller
{
    class UsuarioDAO
    {
        Conexion conexion = new Conexion();
        public bool validarUsuario(string email, string password)
        {
            //Conexion con WS para validar
            //"email=email4@hotmail.com&password=prueba"
            try
            {
                string body = "email=" + email + "&" + "password=" + password;
                //URL mediante la cual se accede al metodo POST
                string url = "/loginEscritorio/";
                return conexion.peticionPost(url, body);
            }
            catch
            {
                return false;
            }
        }
    }
}
