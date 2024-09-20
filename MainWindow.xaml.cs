using System;
using System.Windows;

namespace MNT
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void Cadastro_Click(object sender, RoutedEventArgs e)
        {
            CadastroWindow cadastro = new CadastroWindow();
            cadastro.Show();
            this.Close();
        }
    }
}
