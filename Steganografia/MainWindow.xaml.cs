﻿using Microsoft.Win32;
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
            teoriaTextBlock.Text = "Szyforwanie wiadomości odbywa się podstawie zmiany LSB każdej składowej piksela na jeden bit tekstu jawnego. Zmiana wartości składowych pikseli o 1 sprawia, że nie jesteśmy w stanie zauważyć zmian w obrazie.\n\nZasada działania:\nWczytujemy obraz (dostępne rozszerzenia to png, jpeg, bmp) następnie wpisujemy tekst jawny. Poniżej wczytanego obrazu mamy pokazaną maksymalną ilość znaków możliwą do zaszyfrowania w danym obrazie. Pod tekstem jawnym też mamy informację o ilości znaków. Następnie szyfrujemy za pomocą przycisku Encrypt. Automatycznie przechodzimy do okna zapisu obrazu z zaszyfrowaną wiadomością.\nAby odczytać zaszyfrowaną wiadomość wczytujemy zdjęcie i wciskamy Decrypt. Odszyfrowaną wiadomość możemy zapisać do pliku txt.";
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
            Console.WriteLine(InputTextBox.Text.Length + "<!>" + InputTextBox.Text);
            Console.WriteLine(inputByteArray.Length);
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmap);

            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            var stride = width * 4;

            var pixels = new byte[height * stride];

            writeableBitmap.CopyPixels(pixels, stride, 0);

            int imgByte = 0;
            for(int i = 0; i < inputByteArray.Length; i++)
            {
                for(int j = 7; j >= 0; j--)
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


            /* String output = "";
             for (int i = 0; i < pixels.Length; i++)
             {
                 if ((pixels[i] & 1) == 1)
                 {
                     output += "1";
                 }
                 else
                 {
                     output += "0";
                 }
             }
             byte[] temptab = BinaryStringToByteArray(output);

             String outputString = Encoding.UTF8.GetString(temptab);
             Console.WriteLine("wynik: " + outputString);*/

            /*
            Int32Rect rect = new Int32Rect(0, 0, width, height);
            writeableBitmap.WritePixels(rect, pixels, stride, 0);
            
            ImageInButton.Source = writeableBitmap;


            bitmap = ConvertWriteableBitmapToBitmapImage(writeableBitmap);*/


            writeableBitmap.WritePixels(new Int32Rect(0,0,writeableBitmap.PixelWidth,writeableBitmap.PixelHeight), 
                                        pixels, writeableBitmap.PixelWidth * writeableBitmap.Format.BitsPerPixel / 8, 0);

            ImageInButton.Source = writeableBitmap;


            bitmap = ConvertWriteableBitmapToBitmapImage(writeableBitmap);




            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG|*.PNG|BMP|*.BMP|All files (*.*)|*.*";

            if (sfd.ShowDialog() == true)
            {

                if (sfd.FileName.ToLower().Contains("png"))
                {
                    using (FileStream stream5 = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                        encoder5.Frames.Add(BitmapFrame.Create(writeableBitmap));
                        encoder5.Save(stream5);

                        
                    }


                } else if (sfd.FileName.ToLower().Contains("bmp"))
                    {

                        using (FileStream stream5 = new FileStream(sfd.FileName, FileMode.Create))
                        {
                            BmpBitmapEncoder encoder5 = new BmpBitmapEncoder();
                            encoder5.Frames.Add(BitmapFrame.Create(writeableBitmap));
                            encoder5.Save(stream5);
                        }
                } 
                
            }


        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            String output = "";
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmap);

            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            var stride = width * 4;

            var pixels = new byte[height * stride];
            
            writeableBitmap.CopyPixels(pixels, stride, 0);

            /*byte[] message = new byte[(pixels.Length / 8)];

            int mIndex = 0;
            int counter = 7;
            for(int i = 0; i < pixels.Length; i++)
            {
                if (counter < 0)
                {
                    counter = 7;
                    mIndex++;
                }
                if ((pixels[i] & 1) == 1)
                {
                    message[mIndex] += ((Byte)((int)(Math.Pow(2, counter))));
                }

                counter--;
            }*/

            for(int i = 0; i < 20000; i++)
            {
                if((pixels[i] & 1) == 1)
                {
                    output += "1";
                } else
                {
                    output += "0";
                }
            }

            
            byte[] temptab = BinaryStringToByteArray(output);
            
            String outputString = Encoding.UTF8.GetString(temptab);
            Console.WriteLine("wynik: " + outputString);
            
            int index = outputString.IndexOf("<!>");
            if(index < 0)
            {
                MessageBoxResult m = MessageBox.Show("This image has no hidden message!");

            }
            else
            {
                String sLenght = outputString.Substring(0, index);
                int iLenght = 0;
                int.TryParse(sLenght, out iLenght);

                String result = outputString.Substring(index + 3, iLenght);
                OutputTextBox.Text = result;
            }
            
            


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
            InputTextBox.Clear();
            OutputTextBox.Clear();
            

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

        public BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }

        
    }
}
