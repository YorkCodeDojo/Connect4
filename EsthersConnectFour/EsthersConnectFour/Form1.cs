using System;
using System.Drawing;
using System.Windows.Forms;

namespace EsthersConnectFour
{
    public partial class Form1 : Form
    {
        private int _droppingCounter;
        private int y = 0;
        private Bitmap _template;
        private Bitmap _layer1Template;
        private int _boardWidth;
        private int _boardHeight;
        private int _columnWidth;

        static class Constants
        {
            public const int NumberOfRows = 6;
            public const int NumberOfColumns = 7;
            public const int CounterSize = 50;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.Black;
            _boardWidth = ClientSize.Width - 200;
            _boardHeight = ClientSize.Height - 200;
            _columnWidth = _boardWidth / Constants.NumberOfColumns;
            _template = CreateTemplate();
            CreateBackGroundTemplate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            this.DoubleBuffered = true;

            using (var g = this.CreateGraphics())
            {
                g.TranslateTransform(100, 100);
                for (int column = 0; column < Constants.NumberOfColumns; column++)
                {
                    for (int row = 0; row < Constants.NumberOfRows; row++)
                    {
                        var x = column * _columnWidth;
                        var y = row * (_boardHeight / Constants.NumberOfRows);

                        g.DrawImage(_template, x, y, _columnWidth, _boardHeight / Constants.NumberOfRows);
                    }
                }
            }
        }

        private Bitmap CreateTemplate()
        {
            var template = new Bitmap(_columnWidth, _boardHeight / Constants.NumberOfRows);
            var g1 = Graphics.FromImage(template);
            var b = new SolidBrush(Color.LightGray);
            g1.FillRectangle(Brushes.DarkBlue, 0, 0, _columnWidth, _boardHeight / Constants.NumberOfRows);
            g1.FillEllipse(b, 40, 20 + y, 50, 50);
            template.MakeTransparent(Color.LightGray);
            return template;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            using (var g = this.CreateGraphics())
            {
                g.TranslateTransform(100, 100);

                var column = _droppingCounter;
                var x = column * _columnWidth;

                var copyFrom = new RectangleF(100 + x, 0, _columnWidth, y + 150);
                var backgroundBitMap = _layer1Template.Clone(copyFrom, _layer1Template.PixelFormat);
                g.DrawImage(backgroundBitMap, x, -100);

                g.FillEllipse(Brushes.Red, x + 40, y, 50, 50);

                for (int row = 0; row < Constants.NumberOfRows; row++)
                {
                    var y1 = row * (_boardHeight / Constants.NumberOfRows);
                    g.DrawImage(_template, x, y1, _columnWidth, _boardHeight / Constants.NumberOfRows);
                }
            }

            var increment = 30;
            var stopAt = _boardHeight - 70;
            timer1.Enabled = y < stopAt;
            if (stopAt - y < increment)
                y = stopAt;
            else
                y += increment;

        }

        private void CreateBackGroundTemplate()
        {
            _layer1Template = new Bitmap(ClientSize.Width, ClientSize.Height);
            var layer1Graphics = Graphics.FromImage(_layer1Template);
            layer1Graphics.DrawImage(this.BackgroundImage, new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (_highlightedColumn > -1)
            {
                _droppingCounter = _highlightedColumn;
                y = -50;
                timer1.Interval = 50;
                timer1.Enabled = true;
            }
        }

        private int _highlightedColumn = -1;

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_columnWidth > 0 && e.Y <= 100 && e.X >= 100 && (e.X - 100) / _columnWidth < 7)
            {
                var columnNumber = (e.X - 100) / _columnWidth;
                if (_highlightedColumn != columnNumber)
                {
                    using (var g = this.CreateGraphics())
                    {
                        g.TranslateTransform(100, 100);

                        if (_highlightedColumn > -1)
                        {
                            var copyFrom = new RectangleF(100 + (_highlightedColumn * _columnWidth), 0, _columnWidth, 100);
                            var backgroundBitMap = _layer1Template.Clone(copyFrom, _layer1Template.PixelFormat);
                            g.DrawImage(backgroundBitMap, _highlightedColumn * _columnWidth, -100);
                        }

                        var where = new Rectangle((columnNumber * _columnWidth) + 40,
                                                  -Constants.CounterSize / 2,
                                                  Constants.CounterSize,
                                                  Constants.CounterSize);

                        g.FillPie(Brushes.Red, where, 0, -180);

                        _highlightedColumn = columnNumber;
                    }
                }
            }
            else if (_highlightedColumn > -1)
            {
                using (var g = this.CreateGraphics())
                {
                    g.TranslateTransform(100, 100);
                    var copyFrom = new RectangleF(100 + (_highlightedColumn * _columnWidth), 0, _columnWidth, 100);
                    var backgroundBitMap = _layer1Template.Clone(copyFrom, _layer1Template.PixelFormat);
                    g.DrawImage(backgroundBitMap, _highlightedColumn * _columnWidth, -100);
                    _highlightedColumn = -1;
                }
            }
        }
    }
}
