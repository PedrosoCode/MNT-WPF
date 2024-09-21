using Npgsql;
using System;
using System.Configuration;
using System.Windows;
using BCrypt.Net; // Biblioteca BCrypt

namespace MNT
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text;
            string senha = txtSenha.Password;
            string senhaCriptografada = "";

            try
            {
                using (var conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["PostgreSQLConnection"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT senha FROM usuarios WHERE usuario = @p_usuario", conn))
                    {
                        cmd.Parameters.AddWithValue("p_usuario", usuario);

                        var result = cmd.ExecuteReader();
                        if (result.Read())
                        {
                            senhaCriptografada = result["senha"].ToString(); // Pegamos a senha criptografada do banco
                        }
                        else
                        {
                            MessageBox.Show("Usuário não encontrado.");
                            return;
                        }
                    }

                    // Verificar a senha com BCrypt
                    if (BCrypt.Net.BCrypt.Verify(senha, senhaCriptografada))
                    {
                        // Fechar janela de login e abrir a Home
                        HomeWindow home = new HomeWindow();
                        home.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Senha incorreta.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao fazer login: {ex.Message}");
            }
        }

        private void Cadastro_Click(object sender, RoutedEventArgs e)
        {
            CadastroWindow cadastro = new CadastroWindow();
            cadastro.Show();
            this.Close();
        }
    }
}
