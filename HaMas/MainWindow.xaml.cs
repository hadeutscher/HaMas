/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HaMas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Data data;
        private string curr_file = null;
        private System.Drawing.Point offset = new System.Drawing.Point();

        private static readonly Font boldFont = new Font("Carmela", 30, System.Drawing.FontStyle.Bold);
        private static readonly Font regularFont = new Font("Carmela", 20, System.Drawing.FontStyle.Regular);

        public MainWindow()
        {
            InitializeComponent();
            DataContext = data = new Data();
            UpdateImage();
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                curr_file = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                offset = new System.Drawing.Point();
                UpdateImage();
            }
        }

        private void Image_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private RectangleF scaleToBox(SizeF img, SizeF box)
        {
            var ratioX = box.Width / img.Width;
            var ratioY = box.Height / img.Height;
            var ratio = Math.Max(ratioX, ratioY);
            var new_size = new SizeF(img.Width * ratio, img.Height * ratio);
            return new RectangleF(offset.X + (new_size.Width - box.Width) / -2, offset.Y + (new_size.Height - box.Height) / -2, new_size.Width, new_size.Height);
        }

        private Bitmap MakeImage(bool highQuality=false)
        {
            var tp_image = System.Drawing.Image.FromFile("template.png");
            var fb_image = curr_file == null ? new Bitmap(tp_image.Width, tp_image.Height) : System.Drawing.Image.FromFile(curr_file);
            var result = new Bitmap(tp_image.Width, tp_image.Height);
            using (Graphics graphics = Graphics.FromImage(result))
            {
                if (highQuality)
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                }
                graphics.DrawImage(fb_image,
                    scaleToBox(new SizeF(fb_image.Width, fb_image.Height), new SizeF(tp_image.Width, tp_image.Height)),
                    new RectangleF(0, 0, fb_image.Width, fb_image.Height),
                    GraphicsUnit.Pixel);
                graphics.DrawImage(tp_image, new RectangleF(0, 0, tp_image.Width, tp_image.Height), new RectangleF(0, 0, tp_image.Width, tp_image.Height), GraphicsUnit.Pixel);

                string name_text = nameBox.Text;
                string other_text = "היא זהות גנובה";

                SizeF name_size = graphics.MeasureString(name_text, boldFont);
                SizeF other_size = graphics.MeasureString(other_text, regularFont);
                PointF edge = new PointF(568, 575);
                graphics.DrawString(name_text, boldFont, System.Drawing.Brushes.White, new PointF(edge.X - name_size.Width, edge.Y));
                graphics.DrawString(other_text, regularFont, System.Drawing.Brushes.White, new PointF(edge.X - name_size.Width - other_size.Width, edge.Y + 7));
                
            }
            return result;
        }

        private void UpdateImage()
        {
            using (MemoryStream memory = new MemoryStream())
            {
                MakeImage(false).Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                data.Image = bitmapImage;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MakeImage(true).Save("result.png");
        }

        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateImage();
        }

        private void nameBox_KeyDown(object sender, KeyEventArgs e)
        {
            int d = 10;
            if (e.Key == Key.Left)
            {
                offset = new System.Drawing.Point(offset.X - d, offset.Y);
            }
            else if (e.Key == Key.Right)
            {
                offset = new System.Drawing.Point(offset.X + d, offset.Y);
            }
            else if (e.Key == Key.Up)
            {
                offset = new System.Drawing.Point(offset.X, offset.Y - d);
            }
            else if (e.Key == Key.Down)
            {
                offset = new System.Drawing.Point(offset.X, offset.Y + d);
            }
            else
            {
                return;
            }
            UpdateImage();
        }
    }
}
