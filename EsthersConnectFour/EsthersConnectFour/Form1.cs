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
        private int _rowHeight;
        private bool _redPlayerNext = true;
        private Game _game = new Game();
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
            _rowHeight = _boardHeight / Constants.NumberOfRows;
            _template = CreateTemplate();
            CreateBackGroundTemplate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            using (var g = this.CreateGraphics())
            {
                g.TranslateTransform(100, 100);
                DrawBoard(g);
            }
        }

        private Bitmap CreateTemplate()
        {
            var template = new Bitmap(_columnWidth, _rowHeight);
            var g1 = Graphics.FromImage(template);
            var b = new SolidBrush(Color.LightGray);
            g1.FillRectangle(Brushes.DarkBlue, 0, 0, _columnWidth, _rowHeight);
            g1.FillEllipse(b, 40, 20 + y, 50, 50);
            template.MakeTransparent(Color.LightGray);
            return template;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var currentContext = BufferedGraphicsManager.Current;
            using (var myBuffer = currentContext.Allocate(this.CreateGraphics(), this.DisplayRectangle))
            {
                var g = myBuffer.Graphics;

                g.DrawImage(_layer1Template, 0, 0);

                g.TranslateTransform(100, 100);

                var column = _droppingCounter;
                var x = column * _columnWidth;

                g.FillEllipse(_droppingColour, x + 40, y, 50, 50);

                var counterNumber = _game.NumberOfCountersInColumn(_droppingCounter);
                var row = Constants.NumberOfRows - counterNumber;

                DrawBoard(g, column, row);

                myBuffer.Render();

                var increment = 30;
                var stopAt = _boardHeight - 70 - (_rowHeight * (counterNumber - 1));

                timer1.Enabled = y < stopAt;
                if (stopAt - y < increment)
                    y = stopAt;
                else
                    y += increment;
            }
        }

        private void DrawBoard(Graphics g, int skipColumn = -1, int skipRow = -1)
        {
            for (int column = 0; column < Constants.NumberOfColumns; column++)
            {
                for (int row = 0; row < Constants.NumberOfRows; row++)
                {
                    var x = column * _columnWidth;
                    var y = row * _rowHeight;

                    g.DrawImage(_template, x, y, _columnWidth, _rowHeight);

                    if (row != skipRow || column != skipColumn)
                    {
                        switch (_game.Cells[column, row])
                        {
                            case CellContent.Red:
                                g.FillEllipse(Brushes.Red, x + 40, y + 20, 50, 50);
                                break;

                            case CellContent.Yellow:
                                g.FillEllipse(Brushes.Yellow, x + 40, y + 20, 50, 50);
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
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
                if (_redPlayerNext)
                {
                    _droppingColour = Brushes.Red;
                    if (!_game.Play(_highlightedColumn, CellContent.Red)) return;
                }
                else
                {
                    _droppingColour = Brushes.Yellow;
                    if (!_game.Play(_highlightedColumn, CellContent.Yellow)) return;
                }

                _redPlayerNext = !_redPlayerNext;
                _droppingCounter = _highlightedColumn;
                _highlightedColumn = -1;
                y = -50;
                timer1.Interval = 50;
                timer1.Enabled = true;
            }
        }

        private int _highlightedColumn = -1;
        private Brush _droppingColour;

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (timer1.Enabled) return;

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

                        if (_redPlayerNext)
                            g.FillPie(Brushes.Red, where, 0, -180);
                        else
                            g.FillPie(Brushes.Yellow, where, 0, -180);

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
