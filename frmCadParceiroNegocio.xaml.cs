using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace MNT
{
    public partial class frmCadParceiroNegocio : Window
    {
        private int? codigoParceiro; // Código do parceiro, opcional

        // Construtor padrão sem parâmetros
        public frmCadParceiroNegocio()
        {
            InitializeComponent();
        }

        // Construtor que aceita o código do parceiro
        public frmCadParceiroNegocio(int codigoParceiro)
        {
            InitializeComponent();
            this.codigoParceiro = codigoParceiro;

            // Carregar os dados do parceiro com base no código
            CarregarDadosParceiro(codigoParceiro);
        }

        // Método para carregar os dados do parceiro (você pode adaptar isso conforme sua necessidade)
        private void CarregarDadosParceiro(int codigoParceiro)
        {
            try
            {
                using (var conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["PostgreSQLConnection"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM tb_cad_parceiro_negocio WHERE codigo = @codigo", conn))
                    {
                        cmd.Parameters.AddWithValue("codigo", codigoParceiro);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtNomeFantasia.Text = reader["nome_fantasia"].ToString();
                                txtRazaoSocial.Text = reader["razao_social"].ToString();
                                txtCnpjCpf.Text = reader["cnpj_cpf"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados do parceiro: {ex.Message}");
            }
        }

        private void Filtrar_Click(object sender, RoutedEventArgs e)
        {
            string nomeFantasia = txtNomeFantasia.Text;
            string razaoSocial = txtRazaoSocial.Text;
            string cnpjCpf = txtCnpjCpf.Text;

            try
            {
                // Limpar a grid antes de fazer uma nova busca
                dataGridParceiros.ItemsSource = new List<ParceiroNegocio>();

                using (var conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["PostgreSQLConnection"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM fn_select_parceiro_negocio_grid(@p_nome_fantasia, @p_razao_social, @p_cnpj_cpf)", conn))
                    {
                        // Adicionando parâmetros
                        cmd.Parameters.AddWithValue("p_nome_fantasia", string.IsNullOrEmpty(nomeFantasia) ? (object)DBNull.Value : nomeFantasia);
                        cmd.Parameters.AddWithValue("p_razao_social", string.IsNullOrEmpty(razaoSocial) ? (object)DBNull.Value : razaoSocial);
                        cmd.Parameters.AddWithValue("p_cnpj_cpf", string.IsNullOrEmpty(cnpjCpf) ? (object)DBNull.Value : cnpjCpf);

                        NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Preencher a grid com os dados retornados
                        List<ParceiroNegocio> parceiros = new List<ParceiroNegocio>();
                        foreach (DataRow row in dt.Rows)
                        {
                            parceiros.Add(new ParceiroNegocio
                            {
                                Codigo = Convert.ToInt32(row["codigo"]),
                                NomeFantasia = row["nome_fantasia"].ToString(),
                                RazaoSocial = row["razao_social"].ToString(),
                                CnpjCpf = row["cnpj_cpf"].ToString()
                            });
                        }
                        dataGridParceiros.ItemsSource = parceiros;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao filtrar parceiros: {ex.Message}");
            }
        }

        // Evento para editar o parceiro
        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int codigoParceiro = (int)button.Tag;

            // Abrir uma nova aba de edição passando o código do parceiro
            frmCadParceiroNegocio editarParceiroWindow = new frmCadParceiroNegocio(codigoParceiro);
            editarParceiroWindow.Show();
        }

        // Evento para excluir o parceiro
        private void Excluir_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int codigoParceiro = (int)button.Tag;

            MessageBoxResult result = MessageBox.Show("Tem certeza que deseja excluir este parceiro?", "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["PostgreSQLConnection"].ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = new NpgsqlCommand("CALL sp_excluir_parceiro(@p_codigo)", conn))
                        {
                            cmd.Parameters.AddWithValue("p_codigo", codigoParceiro);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Recarregar a grid após a exclusão
                    Filtrar_Click(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir parceiro: {ex.Message}");
                }
            }
        }
    }

    // Modelo de dados do parceiro
    public class ParceiroNegocio
    {
        public int Codigo { get; set; }
        public string NomeFantasia { get; set; }
        public string RazaoSocial { get; set; }
        public string CnpjCpf { get; set; }
    }
}
