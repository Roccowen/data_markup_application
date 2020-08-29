using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using markup_data_app;

namespace WindowsFormsApplication347
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Point MousePos1;
        private Point MousePos2;

        string[] FramesPaths;
        string CurrentFrame;
        string CurrentMarks;
        string CurrentDir;
        int Position;
        string ReadableFormatsPattern = @"(.*\.jpg|.*\.png|.*\.jpeg|.*\.bmp)";

        Rectangle GetRect(Point p1, Point p2)
        {
            var x1 = Math.Min(p1.X, p2.X);
            var x2 = Math.Max(p1.X, p2.X);
            var y1 = Math.Min(p1.Y, p2.Y);
            var y2 = Math.Max(p1.Y, p2.Y);
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        private void selectImage_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Файлы изображений|*.jpg;*.png;*.jpeg;*.bmp";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {                        
                        PictureBox1.Image = Image.FromFile(ofd.FileName);
                        CurrentFrame = Path.GetFullPath(ofd.FileName);
                        CurrentDir = Path.GetFullPath(Path.GetDirectoryName(ofd.FileName));
                        FramesPaths = Directory.GetFiles(CurrentDir)
                            .Where(n => Regex.IsMatch(n, ReadableFormatsPattern))
                            .OrderBy(n => n)
                            .ToArray();
                        Position = Array.IndexOf(FramesPaths, CurrentFrame);
                    }
                }
            }
            catch 
            {
                FramesPaths = null;
                CurrentFrame = null;
                Position = 0;
            }
            if (PictureBox1.Image != null)
            {
                PictureBox1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
            }       
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MousePos2 = e.Location;
                PictureBox1.Invalidate();
            }
            else
            {
                MousePos1 = MousePos2 = e.Location;
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (MousePos1 != MousePos2)
            {
                var rect = GetRect(MousePos1, MousePos2);

                Pen pen = new Pen(Color.Red, 3);
                PictureBox1.CreateGraphics().DrawRectangle(pen, rect);
                using (var img = Graphics.FromImage(PictureBox1.Image))
                {
                    img.DrawRectangle(pen, rect);
                }

                CurrentMarks += "(" + rect.X + "," + rect.Y + ":" + (rect.X + rect.Width) + "," + (rect.Y - rect.Height) + ") ";    
            }
            PictureBox1.Invalidate();
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (MousePos1 != MousePos2)
                ControlPaint.DrawFocusRectangle(e.Graphics, GetRect(MousePos1, MousePos2));
        }

        private void button2_Click(object sender, EventArgs e)
        {            
            Position += 1;
            if (Position >= FramesPaths.Length)
                Position = 0;
            PictureBox1.Image.Dispose();            
            PictureBox1.Image = Image.FromFile(FramesPaths[Position]);
            WriteMarks();
            CurrentFrame = FramesPaths[Position];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Position -= 1;
            if (Position <= 0)
                Position = FramesPaths.Length - 1;
            PictureBox1.Image.Dispose();
            PictureBox1.Image = Image.FromFile(FramesPaths[Position]);
            WriteMarks();
            CurrentFrame = FramesPaths[Position];
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.PictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = BorderStyle.FixedSingle;
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            if (this.PictureBox1.Image != null)
            {
                Size picSize = this.PictureBox1.Image.Size;
                Size panSize = this.panel1.Size;
                if (picSize.Height < panSize.Height)
                    this.PictureBox1.Location = new Point(this.PictureBox1.Location.X, (panSize.Height - picSize.Height) / 2);
                else
                    this.PictureBox1.Location = new Point(this.PictureBox1.Location.X, 0);
                if (picSize.Width < panSize.Width)
                    this.PictureBox1.Location = new Point((panSize.Width - picSize.Width) / 2, this.PictureBox1.Location.Y);
                else
                    this.PictureBox1.Location = new Point(0, this.PictureBox1.Location.Y);
            }
        }
        private void WriteMarks()
        {
            if (CurrentMarks != null)
            {
                var dataBaseIO = new DataBaseIO(CurrentDir + @"\marks_database.db");
                dataBaseIO.WriteMarks(CurrentFrame, CurrentMarks);
                CurrentMarks = null;
            }
        }
    }
}
