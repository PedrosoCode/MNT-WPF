using Npgsql;
using System;
using System.Configuration;
using System.Windows;

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

            try
            {
                using (var conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["PostgreSQLConnection"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("CALL sp_verificar_login(@p_usuario, @p_senha, @login_valido)", conn))
                    {
                        cmd.Parameters.AddWithValue("p_usuario", usuario);
                        cmd.Parameters.AddWithValue("p_senha", senha);

                        // Parâmetro de saída
                        var loginValido = cmd.Parameters.Add("login_valido", NpgsqlTypes.NpgsqlDbType.Boolean);
                        loginValido.Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        // Verificando o valor de saída
                        if ((bool)loginValido.Value)
                        {
                            MessageBox.Show("Login bem-sucedido!");
                        }
                        else
                        {
                            MessageBox.Show("Usuário ou senha incorretos.");
                        }
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
