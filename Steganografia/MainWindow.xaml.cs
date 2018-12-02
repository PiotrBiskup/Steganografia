using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Steganografia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapImage bitmap;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Title = "Open Image";
            ofd.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.bmp, *.png) | *.jpg; *.jpeg; *.jpe; *.bmp; *.png"; ;

            if (ofd.ShowDialog() == true)
            {
                bitmap = new BitmapImage(new Uri(ofd.FileName));
                ImageInButton.Source = bitmap;
                checkIfEncryptButtonEnable();
                charsToEncryptInImageTextBlock.Text = "Max amount of chars possible to encrypt: " + ((bitmap.PixelHeight * bitmap.PixelWidth) / 4);
            }
       
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] inputByteArray = ConvertToByteArray(InputTextBox.Text.Length + "<!>" + InputTextBox.Text, Encoding.UTF8);
           
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmap);

            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            var stride = width * ((writeableBitmap.Format.BitsPerPixel) / 8);

            var pixels = new byte[height * stride];

            writeableBitmap.CopyPixels(pixels, stride, 0);

            int imgByte = 0;
            for(int i = 0; i < inputByteArray.Length; i++)
            {
                for(int j = 7; j <= 0; j--)
                {   
                    Boolean bit = (inputByteArray[i] & (1 << j)) != 0;

                    if(!bit)
                    {
                        if (pixels[imgByte] % 2 == 1)
                            pixels[imgByte]--;
                    } else
                    {
                        if (pixels[imgByte] % 2 == 0)
                            pixels[imgByte]++;
                    }

                    imgByte++;

                }
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            writeableBitmap.WritePixels(rect, pixels, stride, 0);
            Console.WriteLine(Encoding.UTF8.GetString(inputByteArray));
            ImageInButton.Source = writeableBitmap;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG|*.PNG|GIF|*.GIF|BMP|*.BMP|All files (*.*)|*.*";

            if (sfd.ShowDialog() == true)
            {
                File.WriteAllBytes(sfd.FileName, pixels);
            }


        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveOutputButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".txt";
            sfd.Filter = "Text documents (.txt)|*.txt";

            if (sfd.ShowDialog() == true)
            {
                File.WriteAllText(sfd.FileName, OutputTextBox.Text);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] tab = ConvertToByteArray("ąa", Encoding.UTF8);
            Console.WriteLine(tab.Length);
            Console.WriteLine(ToBinary(tab));
            Console.WriteLine(Encoding.UTF8.GetString(tab));
                

        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkIfEncryptButtonEnable();
            charsToEncryptInMessageTextBlock.Text = "Amount of chars: " + InputTextBox.Text.Length;
        }

        private void loadTextButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".txt";
            ofd.Filter = "Text documents (.txt)|*.txt";

            if (ofd.ShowDialog() == true)
            {
                InputTextBox.Text = File.ReadAllText(ofd.FileName);
            }
        }

        private void checkIfEncryptButtonEnable()
        {
            if (bitmap != null && InputTextBox.Text.Length > 0)
            {
                if ((bitmap.PixelHeight * bitmap.PixelWidth) / 4 > InputTextBox.Text.Length)
                {
                    EncryptButton.IsEnabled = true;
                }
                else EncryptButton.IsEnabled = false;
            }
            else EncryptButton.IsEnabled = false;

            if (bitmap != null) DecryptButton.IsEnabled = true; else DecryptButton.IsEnabled = false;
        }

        BitmapImage encryptMessageInImage()
        {

            return null;
        }

        private static byte[] ConvertToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public static String ToBinary(Byte[] data)
        {
            return string.Join("", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
        }

        public static byte[] BinaryStringToByteArray(String input)
        {
            int numOfBytes = input.Length / 8;
            byte[] bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = Convert.ToByte(input.Substring(8 * i, 8), 2);
            }

            return bytes;
        }
    }
}
