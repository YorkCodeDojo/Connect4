using System;
using System.Drawing;
using System.Windows.Forms;

namespace EsthersConnectFour
{
    public partial class Form1 : Form
    {
        private int y = 0;
        private Bitmap _template;
        private Bitmap _layer1Template;
        private int _boardWidth;
        private int _boardHeight;
        private int _columnWidth;
        private int _rowHeight;
        private Game _game;
        private bool _weAreRed;
        private Brush _droppingColour;
        private readonly APIDetails _details;
        private readonly API _api;

        static class Constants
        {
            public const int NumberOfRows = 6;
            public const int NumberOfColumns = 7;
            public const int CounterSize = 50;
        }

        public Form1(APIDetails details, API api)
        {
            InitializeComponent();
            _details = details;
            _api = api;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.Black;
            _boardWidth = ClientSize.Width - 200;
            _boardHeight = ClientSize.Height - 200;
            _columnWidth = _boardWidth / Constants.NumberOfColumns;
            _rowHeight = _boardHeight / Constants.NumberOfRows;
            _template = CreateTemplate();
            CreateBackGroundTemplate();

            _game = await _api.GetGame(_details.PlayerID);
            _weAreRed = _game.RedPlayerID == _details.PlayerID;

            timer1.Interval = 16; //60FPS
            timer1.Enabled = true;
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

        private async void timer1_Tick(object sender, EventArgs e)
        {
            var currentContext = BufferedGraphicsManager.Current;
            using (var myBuffer = currentContext.Allocate(this.CreateGraphics(), this.DisplayRectangle))
            {
                var g = myBuffer.Graphics;

                g.DrawImage(_layer1Template, 0, 0);

                g.TranslateTransform(100, 100);

                if (_game.CounterDropping)
                {
                    var y = (int)_game.UpdateDroppingCounter(_boardHeight, _rowHeight);
                    if (!_game.CounterDropping)
                    {
                        _game.Play(_game.DropColumn, _weAreRed ? CellContent.Red : CellContent.Yellow);
                        await _api.MakeMove(_details.PlayerID, _game.DropColumn, _details.Password);
                        _game = await _api.GetGame(_details.PlayerID);  //TODO:  Show other player's counter dropping
                        _game.HighLightedColumn = -1;
                    }
                    else
                    {
                        var x = _game.DropColumn * _columnWidth;
                        g.FillEllipse(_droppingColour, x + 40, y, 50, 50);
                    }
                }

                DrawBoard(g);

                if (_game.HighLightedColumn > -1)
                {
                    var where = new Rectangle((_game.HighLightedColumn * _columnWidth) + 40,
                          -Constants.CounterSize / 2,
                          Constants.CounterSize,
                          Constants.CounterSize);

                    if (_weAreRed)
                        g.FillPie(Brushes.Red, where, 0, -180);
                    else
                        g.FillPie(Brushes.Yellow, where, 0, -180);
                }

                CheckForEndGame(g);

                myBuffer.Render();
            }
        }

        private void CheckForEndGame(Graphics g)
        {
            switch (_game.CurrentState)
            {
                case GameState.RedWon:
                    DisplayMessage(g, $"Red has won. {(_weAreRed ? "Well done" : "Bad Luck")}");
                    break;
                case GameState.YellowWon:
                    DisplayMessage(g, $"Yellow has won. {(!_weAreRed ? "Well done" : "Bad Luck")}");
                    break;
                case GameState.Draw:
                    DisplayMessage(g, "It was a draw");
                    break;
                default:
                    break;
            }
        }

        private void DisplayMessage(Graphics g, string message)
        {
            var font = new System.Drawing.Font("Segoe UI", 21.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            g.ResetTransform(); 
            g.TranslateTransform((this.ClientSize.Width - 500) / 2, 0);
            g.FillRectangle(Brushes.DeepPink, 0, 0, 500, 100);
            var width = g.MeasureString(message, font).Width;
            var height = g.MeasureString(message, font).Height;
            g.DrawString(message, font, Brushes.White, (500 - width) / 2, (100 - height) / 2);
        }

        private void DrawBoard(Graphics g)
        {
            for (int column = 0; column < Constants.NumberOfColumns; column++)
            {
                for (int row = 0; row < Constants.NumberOfRows; row++)
                {
                    var x = column * _columnWidth;
                    var y = row * _rowHeight;

                    g.DrawImage(_template, x, y, _columnWidth, _rowHeight);

                    switch (_game.Cells[column, Constants.NumberOfRows - row - 1])
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

        private void CreateBackGroundTemplate()
        {
            _layer1Template = new Bitmap(ClientSize.Width, ClientSize.Height);
            var layer1Graphics = Graphics.FromImage(_layer1Template);
            layer1Graphics.DrawImage(this.BackgroundImage, new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (_game.HighLightedColumn > -1)
            {
                if (!_game.CanPlay(_game.HighLightedColumn)) return;

                if (_weAreRed)
                {
                    _droppingColour = Brushes.Red;
                }
                else
                {
                    _droppingColour = Brushes.Yellow;
                }

                _game.StartDroppingCounter(_droppingColour, _game.HighLightedColumn);
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_game.CounterDropping) return;

            if (_columnWidth > 0 && e.Y <= 100 && e.X >= 100 && (e.X - 100) / _columnWidth < 7)
            {
                var columnNumber = (e.X - 100) / _columnWidth;
                _game.HighLightedColumn = columnNumber;
            }
            else
            {
                _game.HighLightedColumn = -1;
            }
        }
    }
}
