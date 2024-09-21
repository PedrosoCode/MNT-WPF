using System.Windows;
using System.Windows.Controls;

namespace MNT
{
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();
        }

        private void MenuDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Verificar se algum item foi selecionado
            if (MenuDropdown.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedForm = selectedItem.Content.ToString();

                // Criar uma nova aba com base na seleção
                TabItem newTab = new TabItem();
                newTab.Header = selectedForm;

                // Substituir o conteúdo conforme o formulário selecionado
                if (selectedForm == "Formulário 1")
                {
                    newTab.Content = new TextBlock { Text = "Este é o Formulário 1.", FontSize = 18, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                }
                else if (selectedForm == "Formulário 2")
                {
                    newTab.Content = new TextBlock { Text = "Este é o Formulário 2.", FontSize = 18, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                }
                else if (selectedForm == "Parceiro de Negócio")
                {
                    // Adiciona a tela de Parceiro de Negócio como um UserControl
                    var parceiroControl = new frmCadParceiroNegocio();
                    newTab.Content = parceiroControl.Content;
                }

                // Adicionar a nova aba ao TabControl
                MainTabControl.Items.Add(newTab);

                // Definir a nova aba como a aba ativa
                MainTabControl.SelectedItem = newTab;
            }
        }
    }
}
