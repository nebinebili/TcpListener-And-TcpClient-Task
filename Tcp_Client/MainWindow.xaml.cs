using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tcp_Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public byte[] ReadAllBytesAsync(BinaryReader reader)
        {
            const int bufferSize = 1024;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }

        }

        public async void ProcessClient()
        {
            var EndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.107"), 3456);
            var client = new TcpClient();
            client.Connect(EndPoint);
            
            try
            {
                using(var stream = client.GetStream())
                {
                    var br = new BinaryReader(stream);
                    while (true)
                    {

                        var imagebuffer = await Task.Run(() => ReadAllBytesAsync(br));

                        using (var ms1=new MemoryStream(imagebuffer))
                        {
                            var image = System.Drawing.Image.FromStream(ms1);

                            using (var ms = new MemoryStream())
                            {
                                image.Save(ms, ImageFormat.Bmp);
                                ms.Seek(0, SeekOrigin.Begin);

                                var bitmapImage = new BitmapImage();
                                bitmapImage.BeginInit();
                                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                bitmapImage.StreamSource = ms;
                                bitmapImage.EndInit();

                                img_box.Source = bitmapImage;
                            }
                        }  
                    }
                }  
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            ProcessClient();
        }
    }
}
