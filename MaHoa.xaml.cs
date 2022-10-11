using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Drawing;
using Microsoft.Win32;
using System.Security.Cryptography;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MaHoa.xaml
    /// </summary>
    public partial class MaHoa : Window
    {
        private string inPath1="";       //duong dan toi file anh can giau

        private byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        private int BlockSize = 128;
        public MaHoa()
        {
            InitializeComponent();
            WindowStartupLocation= WindowStartupLocation.CenterScreen;
            ThongDiepSauGiaiMa.IsEnabled = false;
            ThongDiepMaHoa.IsEnabled = false;
        }

        private void ChonAnh(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog(); // tạo biến lấy ảnh

            openFileDialog1.ShowDialog();  // mở giao diện chọn ảnh
            inPath1 = openFileDialog1.FileName; // lấy ra đường dẫn của file vừa mở
            if (inPath1 != "")
            {
                AnhGoc.Source = new BitmapImage(new Uri(new Uri(Directory.GetCurrentDirectory(), UriKind.Absolute), new Uri(inPath1, UriKind.Relative)));  // hiển  thị ảnh
            }
        }

        private void btMaHoa(object sender, RoutedEventArgs e)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(ThongDiep.Text);

            //Encrypt
            SymmetricAlgorithm crypt = Aes.Create();
            HashAlgorithm hash = MD5.Create();
            crypt.BlockSize = BlockSize;
            crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes("PVMINH"));
            crypt.IV = IV;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, crypt.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                }

                ThongDiepMaHoa.Text = Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        private void GiauTin(object sender, RoutedEventArgs e)
        {
            Bitmap img = new Bitmap(inPath1);
            int i, j;
            for (i = 0; i < img.Width; i++)
            {
                for (j = 0; j < img.Height; j++)
                {
                    System.Drawing.Color pixel = img.GetPixel(i, j);
                    if (i < 1 && j < ThongDiepMaHoa.Text.Length)
                    {
                        Console.WriteLine("R[" + i + "][" + j + "] : " + pixel.R);
                        Console.WriteLine("R[" + i + "][" + j + "] : " + pixel.B);
                        Console.WriteLine("R[" + i + "][" + j + "] : " + pixel.G);
                        char letter = Convert.ToChar(ThongDiepMaHoa.Text.Substring(j, 1));
                        int value = Convert.ToInt32(letter);
                        Console.WriteLine("Letter: " + letter + "\n Value: " + value);
                        img.SetPixel(i, j, System.Drawing.Color.FromArgb(pixel.R, pixel.G, value));
                    }
                    if (i == img.Width - 1 && j == img.Height - 1)
                    {
                        img.SetPixel(i, j, System.Drawing.Color.FromArgb(pixel.R, pixel.G, ThongDiepMaHoa.Text.Length));
                    }
                }
            }
            SaveFileDialog savefile = new SaveFileDialog();

            savefile.Filter = "JPG|*.jpg|PNG|*.png";
            savefile.ShowDialog();
            string filename = savefile.FileName;
            img.Save(filename);
        }
        private void LayThongDiep(object sender, RoutedEventArgs e)
        {
            Bitmap img = new Bitmap(inPath1);
            string message = "";
            System.Drawing.Color lpixel = img.GetPixel(img.Width - 1, img.Height - 1);
            int messlen = lpixel.B;
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    System.Drawing.Color pixel = img.GetPixel(i, j);
                    if (i < 1 && j < messlen)
                    {
                        Console.WriteLine("-------------");
                        Console.WriteLine("R[" + i + "][" + j + "] : " + pixel.R);
                        Console.WriteLine("R[" + i + "][" + j + "] : " + pixel.B);
                        Console.WriteLine("R[" + i + "][" + j + "] : " + pixel.G);

                        int value = pixel.B;
                        Console.WriteLine("Value: " + value);
                        char c = Convert.ToChar(value);

                        string letter = c.ToString();
                        message = message + letter;
                    }
                }
            }
            ThongDiep.Text = message;
        }

        private void btGiaiMa(object sender, RoutedEventArgs e)
        {
            byte[] bytes = Convert.FromBase64String(ThongDiep.Text);
            SymmetricAlgorithm crypt = Aes.Create();
            HashAlgorithm hash = MD5.Create();
            crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes("PVMINH"));
            crypt.IV = IV;

            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] decryptedBytes = new byte[bytes.Length];
                    cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                    ThongDiepSauGiaiMa.Text = Encoding.Unicode.GetString(decryptedBytes);
                }
            }
        }

        private void LamMoi(object sender, RoutedEventArgs e)
        {
            ThongDiep.Text = "";
            ThongDiepMaHoa.Text = "";
            ThongDiepSauGiaiMa.Text = "";
        }

    }
}
