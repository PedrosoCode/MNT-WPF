using Npgsql;
using System;
using System.Configuration;
using System.Windows;
using BCrypt.Net; // Biblioteca BCrypt

namespace MNT
{
    public partial class CadastroWindow : Window
    {
        public CadastroWindow()
        {
            InitializeComponent();
        }

        private void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuarioCadastro.Text;
            string senha = txtSenhaCadastro.Password;
            string confirmarSenha = txtConfirmarSenhaCadastro.Password;

            if (senha != confirmarSenha)
            {
                MessageBox.Show("As senhas não coincidem!");
                return;
            }

            // Criptografar a senha com BCrypt
            string senhaCriptografada = BCrypt.Net.BCrypt.HashPassword(senha);

            try
            {
                using (var conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["PostgreSQLConnection"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("CALL sp_inserir_usuario(@p_usuario, @p_senha)", conn))
                    {
                        cmd.Parameters.AddWithValue("p_usuario", usuario);
                        cmd.Parameters.AddWithValue("p_senha", senhaCriptografada); // Usando a senha criptografada
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Usuário cadastrado com sucesso!");
                LoginWindow login = new LoginWindow();
                login.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao cadastrar: {ex.Message}");
            }
        }
    }
}
