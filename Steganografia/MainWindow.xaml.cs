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
                charsToEncryptInImageTextBlock.Text = "Max amount of chars possible to encrypt: " + ((bitmap.PixelHeight * bitmap.PixelWidth) / 2);
            }
       
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveOutputButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {

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
                if ((bitmap.PixelHeight * bitmap.PixelWidth) / 2 > InputTextBox.Text.Length)
                {
                    EncryptButton.IsEnabled = true;
                }
                else EncryptButton.IsEnabled = false;
            }
            else EncryptButton.IsEnabled = false;

            if (bitmap != null) DecryptButton.IsEnabled = true; else DecryptButton.IsEnabled = false;
        }
    }
}
