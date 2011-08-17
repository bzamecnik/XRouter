using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XRouter.Common;

namespace XRouter.Gui
{
    /// <summary>
    /// Interaction logic for TokensViewControl.xaml
    /// </summary>
    public partial class TokensViewControl : UserControl
    {
        private static readonly int PageSize = 30;

        private ConfigurationManager ConfigManager { get; set; }

        private int currentPage = 1;

        public TokensViewControl()
        {
            InitializeComponent();
        }

        internal void Initialize(ConfigurationManager configManager)
        {
            ConfigManager = configManager;

            LoadTokens();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (uiTokens.Items.Count == 0) {
                return;
            }
            currentPage++;
            LoadTokens();
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage == 1) {
                return;
            }
            currentPage--;
            LoadTokens();
        }

        private void LoadTokens()
        {
            Token[] tokens = ConfigManager.ConsoleServer.GetTokens(PageSize, currentPage);
            TokenRow[] rows = tokens.Select(t => new TokenRow(t)).ToArray();
            uiTokens.ItemsSource = rows;
            uiPageNumber.Text = currentPage.ToString();
        }

        private class TokenRow
        {
            public string Guid { get; private set; }

            public string State { get; private set; }

            public string Content { get; private set; }

            public TokenRow(Token token)
            {
                Guid = token.Guid.ToString();
                State = token.State.ToString();
                Content = token.Content.XDocument.ToString().Replace(Environment.NewLine, " ");
            }
        }
    }
}
