using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoTerminal2.model
{
    class UsuarioDTO
    {
        private string nombre;
        private string email;
        private string password;
        public UsuarioDTO(string password,string email)
        {
            this.nombre = "";
            this.password = password;
            this.email = email;
        }
        public bool setNombre(string origen)
        {
            this.nombre = origen;
            return true;
        }
        public bool setEmail(string email)
        {
            this.email = email;
            return true;
        }
        public bool setPassword(string destino)
        {
            this.password = destino;
            return true;
        }
        public string getNombre()
        {
            return this.nombre;
        }
        public string getEmail()
        {
            return this.email;
        }
        public string getPassword()
        {
            return this.password;
        }
    }
}
