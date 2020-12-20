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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Screenboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public bool saved = false;
        public string filename = CreateMD5(DateTime.Now.ToString());
        public MainWindow()
        {
            InitializeComponent();
        }

        private void drawingBoard_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            ContextMenu cm = this.FindResource("cmCanvas") as ContextMenu;
            cm.PlacementTarget = sender as InkCanvas;
            cm.IsOpen = true;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if(drawingBoard.Strokes.Count() > 0 && saved == false)
            {
                var box = MessageBox.Show("Are you sure you want to exit without saving?", "Save", MessageBoxButton.YesNo);
                if(box == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }

            }
            else
            {
                Application.Current.Shutdown();
            }
            
        }

        // https://social.msdn.microsoft.com/Forums/vstudio/en-US/cd92fad7-8a1d-4b8b-a728-8122af105504/is-it-possible-to-save-inkcanvas-as-png-with-transparent-background?forum=wpf
        private void SaveFunction()
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(drawingBoard);
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(drawingBoard);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }
            rtb.Render(dv);

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                pngEncoder.Save(ms);
                System.IO.File.WriteAllBytes(filename+".png", ms.ToArray());
            }

            saved = true;
        }
        //https://stackoverflow.com/questions/11454004/calculate-a-md5-hash-from-a-string
        // God, I love stackoverflow
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFunction();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            drawingBoard.Strokes.Clear();
        }

        private void drawingBoard_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                saved = false;
            }
        }
    }
}
