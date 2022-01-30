using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Tcp_Listener
{
    class Program
    {

        public static Bitmap ScreenConsole()
        {
            Bitmap memoryImage;
            memoryImage = new Bitmap(1400,780);
            Size s = new Size(memoryImage.Width, memoryImage.Height);

            Graphics memoryGraphics = Graphics.FromImage(memoryImage);

            memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);


            return memoryImage;
        }
        public static byte[] ImageToByteArray(Bitmap imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }


        static void Main(string[] args)
        {
            var EndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.107"), 3456);
            var listener = new TcpListener(EndPoint);
            listener.Start(10);
            try
            {
                while (true)
                {
                    Task.Run(() =>
                    {
                        do
                        {
                            var client = listener.AcceptTcpClient();
                            Console.WriteLine("Connected");

                            using (var stream = client.GetStream())
                            {
                                var bw = new BinaryWriter(stream);
                                var br = new BinaryReader(stream);

                                var msg = Console.ReadLine();

                                if (msg == "send")
                                {
                                    var image = ScreenConsole();
                                    bw.Write(ImageToByteArray(image));
                                }
                            }
                        } while (true);

                    });

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}
