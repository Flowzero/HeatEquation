using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Timers;

namespace PnMM__Lab1_Main_
{
    public partial class Form1 : Form
    {
        private bool debug = false;
        private System.Timers.Timer timer;
        private List<PointF> temperatureData, temperatureData_;

        private DirectSheme solver;
        private ReversedSheme solver_;

        private int POINT_COUNT;
        private float MAX_COORDINATE;
        private const int MARGIN = 50;
        private const float MAX_TEMPERATURE = 100f;
        private const int GRAPH_SPACING = 100;

        private NumericUpDown numAlpha, numDx, numDtPercent, numTimerInterval, numFinalPos, numHeatSourcePos, numHeatSourceTemp;
        private Button btnRestart, btnApplyTimer, btnSetHeatSource;
        private Label lblAlpha, lblDx, lblDtPercent, lblTimerInterval, lblFinalPos, lblHeatSourcePos, lblHeatSourceTemp;
        private Panel controlPanel;

        private float heatSourcePosition = 0.4f;
        private float heatSourceTemperature = 100f;

        public Form1()
        {
            InitializeComponent();
            InitializeControls();
            InitializeSolvers();
            InitializeTimer();

            this.Size = new Size(1300, 950);
            this.Text = "Сравнение явной и неявной схем с расширенным управлением";
            this.DoubleBuffered = true;

            this.Resize += Form1_Resize;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (controlPanel != null)
            {
                controlPanel.Width = this.ClientSize.Width - 20;
            }
            this.Invalidate();
        }

        private void InitializeControls()
        {
            controlPanel = new Panel()
            {
                Location = new Point(10, 10),
                Size = new Size(this.ClientSize.Width - 20, 100),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray
            };

            int yPos = 15;
            int xPos = 10;

            lblAlpha = new Label()
            {
                Text = "α:",
                Location = new Point(xPos, yPos),
                Size = new Size(20, 20),
                Font = new Font("Arial", 9)
            };
            xPos += 25;

            numAlpha = new NumericUpDown()
            {
                Location = new Point(xPos, yPos - 2),
                Size = new Size(70, 20),
                DecimalPlaces = 6,
                Minimum = 0.000001m,
                Maximum = 100000m,
                Value = 0.0002m,
                Font = new Font("Arial", 9)
            };
            xPos += 80;

            lblDx = new Label()
            {
                Text = "dx:",
                Location = new Point(xPos, yPos),
                Size = new Size(25, 20),
                Font = new Font("Arial", 9)
            };
            xPos += 30;

            numDx = new NumericUpDown()
            {
                Location = new Point(xPos, yPos - 2),
                Size = new Size(60, 20),
                DecimalPlaces = 4,
                Minimum = 0.001m,
                Maximum = 1m,
                Value = 0.02m,
                Increment = 0.001m,
                Font = new Font("Arial", 9)
            };
            xPos += 70;

            lblDtPercent = new Label()
            {
                Text = "dt %:",
                Location = new Point(xPos, yPos),
                Size = new Size(35, 20),
                Font = new Font("Arial", 9)
            };
            xPos += 40;

            numDtPercent = new NumericUpDown()
            {
                Location = new Point(xPos, yPos - 2),
                Size = new Size(50, 20),
                DecimalPlaces = 2,
                Minimum = 0.01m,
                Maximum = 100m,
                Value = 25m,
                Font = new Font("Arial", 9)
            };
            xPos += 60;

            lblFinalPos = new Label()
            {
                Text = "Длина L:",
                Location = new Point(xPos, yPos),
                Size = new Size(50, 20),
                Font = new Font("Arial", 9)
            };
            xPos += 55;

            numFinalPos = new NumericUpDown()
            {
                Location = new Point(xPos, yPos - 2),
                Size = new Size(60, 20),
                DecimalPlaces = 2,
                Minimum = 0.1m,
                Maximum = 10m,
                Value = 2m,
                Font = new Font("Arial", 9)
            };
            xPos += 70;

            yPos += 30;
            xPos = 10;

            lblHeatSourcePos = new Label()
            {
                Text = "Источник X:",
                Location = new Point(xPos, yPos),
                Size = new Size(65, 20),
                Font = new Font("Arial", 9)
            };
            xPos += 70;

            numHeatSourcePos = new NumericUpDown()
            {
                Location = new Point(xPos, yPos - 2),
                Size = new Size(60, 20),
                DecimalPlaces = 2,
                Minimum = 0m,
                Maximum = 10m,
                Value = 0.4m,
                Font = new Font("Arial", 9)
            };
            xPos += 70;

            lblHeatSourceTemp = new Label()
            {
                Text = "Температура:",
                Location = new Point(xPos, yPos),
                Size = new Size(75, 20),
                Font = new Font("Arial", 9)
            };
            xPos += 80;

            numHeatSourceTemp = new NumericUpDown()
            {
                Location = new Point(xPos, yPos - 2),
                Size = new Size(60, 20),
                DecimalPlaces = 1,
                Minimum = 0m,
                Maximum = 1000m,
                Value = 100m,
                Font = new Font("Arial", 9)
            };
            xPos += 70;

            btnSetHeatSource = new Button()
            {
                Text = "Установить источник",
                Location = new Point(xPos, yPos - 2),
                Size = new Size(120, 23),
                Font = new Font("Arial", 9),
                BackColor = Color.LightGreen
            };
            xPos += 130;

            yPos += 30;
            xPos = 10;

            lblTimerInterval = new Label()
            {
                Text = "Скорость анимации (мс):",
                Location = new Point(xPos, yPos),
                Size = new Size(150, 20),
                Font = new Font("Arial", 9)
            };
            xPos += 155;

            numTimerInterval = new NumericUpDown()
            {
                Location = new Point(xPos, yPos - 2),
                Size = new Size(60, 20),
                Minimum = 10m,
                Maximum = 5000m,
                Value = 1000m,
                Increment = 100m,
                Font = new Font("Arial", 9)
            };
            xPos += 70;

            btnApplyTimer = new Button()
            {
                Text = "Применить скорость",
                Location = new Point(xPos, yPos - 2),
                Size = new Size(120, 23),
                Font = new Font("Arial", 9),
                BackColor = Color.LightYellow
            };
            xPos += 130;

            btnRestart = new Button()
            {
                Text = "🔄 Перезапуск",
                Location = new Point(xPos, yPos - 2),
                Size = new Size(100, 23),
                Font = new Font("Arial", 9),
                BackColor = Color.LightBlue
            };

            btnRestart.Click += BtnRestart_Click;
            btnApplyTimer.Click += BtnApplyTimer_Click;
            btnSetHeatSource.Click += BtnSetHeatSource_Click;

            controlPanel.Controls.AddRange(new Control[] {
                lblAlpha, numAlpha, lblDx, numDx, lblDtPercent, numDtPercent,
                lblFinalPos, numFinalPos, lblHeatSourcePos, numHeatSourcePos,
                lblHeatSourceTemp, numHeatSourceTemp, btnSetHeatSource,
                lblTimerInterval, numTimerInterval, btnApplyTimer, btnRestart
            });

            this.Controls.Add(controlPanel);
        }

        private void InitializeSolvers()
        {
            float alpha = (float)numAlpha.Value;
            float dx = (float)numDx.Value;
            float finalPos = (float)numFinalPos.Value;
            float percent = (float)numDtPercent.Value / 100f;

            heatSourcePosition = (float)numHeatSourcePos.Value;
            heatSourceTemperature = (float)numHeatSourceTemp.Value;

            try
            {
                solver = new DirectSheme(alpha, finalPos, 1f, percent, dx);
                solver_ = new ReversedSheme(alpha, finalPos, 1f, percent, dx);

                SetHeatSource(solver, heatSourcePosition, heatSourceTemperature);
                SetHeatSource(solver_, heatSourcePosition, heatSourceTemperature);

                POINT_COUNT = solver.NumberOfPoints;
                MAX_COORDINATE = solver.FinalPos;

                InitializeData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetHeatSource(BaseSolver solver, float position, float temperature)
        {
            var temperatureField = solver.GetType().GetField("temperature",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (temperatureField != null)
            {
                float[] tempArray = (float[])temperatureField.GetValue(solver);
                if (tempArray != null)
                {
                    int index = (int)Math.Round(position / solver.Dx);
                    if (index >= 0 && index < tempArray.Length)
                    {
                        tempArray[index] = temperature;
                    }
                }
            }
        }

        private void BtnRestart_Click(object sender, EventArgs e)
        {
            RestartSimulation();
        }

        private void BtnApplyTimer_Click(object sender, EventArgs e)
        {
            if (timer != null)
            {
                timer.Interval = (double)numTimerInterval.Value;
            }
        }

        private void BtnSetHeatSource_Click(object sender, EventArgs e)
        {
            heatSourcePosition = (float)numHeatSourcePos.Value;
            heatSourceTemperature = (float)numHeatSourceTemp.Value;

            if (solver != null && solver_ != null)
            {
                SetHeatSource(solver, heatSourcePosition, heatSourceTemperature);
                SetHeatSource(solver_, heatSourcePosition, heatSourceTemperature);

                GenerateNewData();
                this.Invalidate();
            }
        }

        private void RestartSimulation()
        {
            if (timer != null)
            {
                timer.Stop();
            }

            InitializeSolvers();

            if (timer != null)
            {
                timer.Start();
            }

            this.Invalidate();
        }

        private void InitializeData()
        {
            temperatureData = new List<PointF>();
            temperatureData_ = new List<PointF>();
            GenerateNewData();
        }

        private void InitializeTimer()
        {
            timer = new System.Timers.Timer((double)numTimerInterval.Value);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(GenerateNewData));
                this.Invoke(new Action(() => this.Invalidate()));
            }
            else
            {
                GenerateNewData();
                this.Invalidate();
            }
        }

        private void GenerateNewData()
        {
            if (solver == null || solver_ == null) return;

            temperatureData.Clear();
            temperatureData_.Clear();

            try
            {
                float[] temperatures = solver.SolveStep();
                float[] temperatures_ = solver_.SolveStep();

                for (int i = 0; i < temperatures.Length; i++)
                {
                    float x = i * solver.Dx;
                    temperatureData.Add(new PointF(x, temperatures[i]));
                    temperatureData_.Add(new PointF(x, temperatures_[i]));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка расчета: {ex.Message}");
                timer.Stop();

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                        MessageBox.Show($"Ошибка расчета: {ex.Message}", "Ошибка",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawGraph(e.Graphics);
        }

        private void DrawGraph(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            int controlPanelHeight = controlPanel.Height + 20;
            int graphWidth = (this.ClientSize.Width - 2 * MARGIN - GRAPH_SPACING) / 2;
            int graphHeight = this.ClientSize.Height - 2 * MARGIN - controlPanelHeight;

            if (graphWidth <= 0 || graphHeight <= 0) return;

            DrawSingleGraph(g, temperatureData, "Явная схема",
                           MARGIN, MARGIN + controlPanelHeight, graphWidth, graphHeight, Color.Red);

            DrawSingleGraph(g, temperatureData_, "Неявная схема",
                           MARGIN + graphWidth + GRAPH_SPACING, MARGIN + controlPanelHeight,
                           graphWidth, graphHeight, Color.Blue);

            DrawLegend(g, graphWidth * 2 + GRAPH_SPACING, controlPanelHeight);
        }

        private void DrawSingleGraph(Graphics g, List<PointF> data, string title,
                                   int xOffset, int yOffset, int width, int height, Color color)
        {
            if (data.Count < 2) return;

            g.DrawRectangle(new Pen(Color.Gray, 1), xOffset, yOffset, width, height);

            Pen axisPen = new Pen(Color.Black, 2);
            g.DrawLine(axisPen, xOffset, yOffset + height, xOffset, yOffset);
            g.DrawLine(axisPen, xOffset, yOffset + height, xOffset + width, yOffset + height);

            Pen gridPen = new Pen(Color.LightGray, 1);
            for (int i = 0; i <= 10; i++)
            {
                float x = xOffset + i * width / 10f;
                g.DrawLine(gridPen, x, yOffset, x, yOffset + height);
                float y = yOffset + i * height / 10f;
                g.DrawLine(gridPen, xOffset, y, xOffset + width, y);
            }

            PointF[] points = new PointF[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                PointF dataPoint = data[i];
                float x = xOffset + dataPoint.X * width / MAX_COORDINATE;
                float y = yOffset + height - (dataPoint.Y * height / MAX_TEMPERATURE);

                if (!float.IsNaN(x) && !float.IsInfinity(x) && !float.IsNaN(y) && !float.IsInfinity(y))
                    points[i] = new PointF(x, y);
            }

            if (points.Length >= 2)
            {
                Pen curvePen = new Pen(color, 2);
                g.DrawCurve(curvePen, points);
            }

            Font labelFont = new Font("Arial", 8);
            Font titleFont = new Font("Arial", 10, FontStyle.Bold);
            Brush labelBrush = new SolidBrush(Color.Black);

            for (int i = 0; i <= 10; i++)
            {
                float x = xOffset + i * width / 10f;
                string label = (i * MAX_COORDINATE / 10f).ToString("F1");
                g.DrawString(label, labelFont, labelBrush, x - 10, yOffset + height + 5);

                float y = yOffset + i * height / 10f;
                string tempLabel = (MAX_TEMPERATURE - i * MAX_TEMPERATURE / 10f).ToString("F0");
                g.DrawString(tempLabel, labelFont, labelBrush, xOffset - 25, y - 6);
            }

            SizeF titleSize = g.MeasureString(title, titleFont);
            g.DrawString(title, titleFont, labelBrush,
                        xOffset + width / 2 - titleSize.Width / 2, yOffset - 25);
        }

        private void DrawLegend(Graphics g, int totalWidth, int controlPanelHeight)
        {
            int legendY = this.ClientSize.Height - 20;
            Font legendFont = new Font("Arial", 9);

            if (solver != null)
            {
                string info = $"Время: {solver.CurrentTime:F3} с | α: {solver.Alpha:E4} | dx: {solver.Dx:F4} | dt: {solver.Dt:E4} | Источник: x={heatSourcePosition:F2}, T={heatSourceTemperature}°C";
                SizeF infoSize = g.MeasureString(info, legendFont);
                g.DrawString(info, legendFont, Brushes.Black,
                            MARGIN + totalWidth / 2 - infoSize.Width / 2, legendY);
            }

            DrawLegendItem(g, "Явная схема", Color.Red, MARGIN, legendY - 25);
            DrawLegendItem(g, "Неявная схема", Color.Blue, MARGIN + 120, legendY - 25);
        }

        private void DrawLegendItem(Graphics g, string text, Color color, int x, int y)
        {
            Font font = new Font("Arial", 9);
            g.FillRectangle(new SolidBrush(color), x, y, 15, 10);
            g.DrawRectangle(Pens.Black, x, y, 15, 10);
            g.DrawString(text, font, Brushes.Black, x + 20, y - 2);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
        }
    }
}