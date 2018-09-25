using ProyectoTerminal2.controller;
using ProyectoTerminal2.views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoTerminal2
{
    public partial class Login : Form
    {
        LoginController loginController = new LoginController();
        public Login()
        {
            InitializeComponent();
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            try
            {
                loginController.validarClick(textBoxEmail.Text, textBoxContraseña.Text);
            }
            catch { return; }
        }

        private void linkCrearCuenta_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://servidortesisv3-samyckl.rhcloud.com//registro");
        }
    }
}
