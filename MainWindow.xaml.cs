using System;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using WinForms = System.Windows.Forms;

namespace BtcPriceApp
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient = new();
        private readonly System.Timers.Timer _timer;
        private WinForms.NotifyIcon? _notifyIcon = null;
        private bool _isLoaded = false;

        public MainWindow()
        {
            InitializeComponent();

            this.SourceInitialized += (s, e) => SetupTrayIcon();

            _timer = new System.Timers.Timer(30000);
            _timer.Elapsed += async (s, e) => await UpdatePrice();
            _timer.AutoReset = true;
            _timer.Enabled = true;

            this.Loaded += async (s, e) =>
            {
                await UpdatePrice();
                _isLoaded = true;
            };
        }

        private void SetupTrayIcon()
        {
            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "bitcoin.ico");
            if (!File.Exists(iconPath))
            {
                System.Windows.MessageBox.Show("Ikona nebyla nalezena: " + iconPath);
                return;
            }

            _notifyIcon = new WinForms.NotifyIcon
            {
                Icon = new System.Drawing.Icon(iconPath),
                Visible = true,
                Text = "Bitcoin Price"
            };

            var contextMenu = new WinForms.ContextMenuStrip();
            var exitItem = new WinForms.ToolStripMenuItem("Ukončit");
            exitItem.Click += (a, b) => CloseApp();
            contextMenu.Items.Add(exitItem);

            _notifyIcon.DoubleClick += (a, b) => ShowWindow();
            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        private async Task UpdatePrice()
        {
            try
            {
                // Načtení ceny BTC v USD (každých 30 vteřin)
                var json = await _httpClient.GetStringAsync("https://api.coinbase.com/v2/exchange-rates?currency=BTC");
                using var doc = JsonDocument.Parse(json);
                var usdValueStr = doc.RootElement.GetProperty("data").GetProperty("rates").GetProperty("USD").GetString();
                var czkValueStr = doc.RootElement.GetProperty("data").GetProperty("rates").GetProperty("CZK").GetString();
                if (string.IsNullOrEmpty(usdValueStr) || string.IsNullOrEmpty(czkValueStr))
                {
                    throw new Exception("Kurz nebyl nalezen.");
                }
                // Převeďte string na decimal
                decimal usdValue;
                decimal czkValue;
                bool isParsedUsd = decimal.TryParse(usdValueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out usdValue);
                bool isParsedCzk = decimal.TryParse(czkValueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out czkValue);
                if (!isParsedUsd || !isParsedCzk) {
                    throw new Exception("Nepodařilo se převést na číslo.");
                }

                Dispatcher.Invoke(() =>
                {
                    // PriceTextBlockUSD a PriceTextBlockCZK by měly existovat v XAML
                    PriceTextBlockUSD.Text = $"{usdValue.ToString("N0")} $";
                    PriceTextBlockCZK.Text = $"{czkValue.ToString("N0")} Kč";
                    LastUpdateTextBlock.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                });

                if (_notifyIcon != null)
                {
                    _notifyIcon.Text = $"BTC: {usdValue.ToString("N0")} $ / {czkValue.ToString("N0")} Kč";
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    PriceTextBlockUSD.Text = "Chyba";
                    PriceTextBlockCZK.Text = ex.Message;
                    LastUpdateTextBlock.Text = "";
                });
            }
        }

        private void ShowWindow()
        {
            if (!_isLoaded) return;
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        private void CloseApp()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                WinForms.Application.DoEvents();
            }
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseApp();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            HelpPopup.IsOpen = true;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true
            });
            e.Handled = true;
        }
    }
}
