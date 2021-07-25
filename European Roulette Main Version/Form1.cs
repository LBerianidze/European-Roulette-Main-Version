using European_Roulette_Main_Version.CustomControls;
using European_Roulette_Main_Version.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace European_Roulette_Main_Version
{
    public partial class Form1 : Form
    {
        private readonly Timer timer;
        public Form1()
        {
            InitializeComponent();
            timer = new Timer()
            {
                Interval = 1000
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private int ticksCount = 0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(++ticksCount);
            string sttimer = timeSpan.ToString(@"hh\:mm\:ss");

            textBox2.Text = sttimer;
        }
        Dictionary<int, Dictionary<int, int>> lengths = new Dictionary<int, Dictionary<int, int>>();
        CircularLinkedList<int> linkedList;
        private static readonly int[] RedSpins = { 1, 3, 5, 9, 12, 7, 14, 18, 16, 21, 19, 23, 27, 25, 30, 32, 36, 34 };
        private static readonly int[] FalloutIntervals = { 11, 14, 19, 28, 37, 55, 111, 295, 0 };
        private readonly List<int> spins = new List<int>();
        private int lastSpin = -1;
        List<ColorSchema> schemas = new List<ColorSchema>();
        /// <summary>
        /// Spin Operation Type. 1 - Spin Added, 2 - Spin Cancelled
        /// </summary>
        private byte lastOperation = 0;
        private static readonly int[] Sequence = { 22, 18, 29, 7, 28, 12, 35, 3, 26, 0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1, 20, 14, 31, 9 };
        bool blocked = false;

        #region Methods
        private void Button35_Click(object sender, EventArgs e)
        {
            lastSpin = Convert.ToInt32((sender as Control).Text);
            spins.Add(lastSpin);
            lastOperation = 1;
            Calculate();
            Controls.OfType<UserControl1>().ToList().ForEach(t => t.MoveNext());

        }
        private void ResetDataGridViewCellStyle()
        {
            if (blocked)
                return;
            lastThirteenSpinsDataGridView.Rows[0].Cells.Cast<DataGridViewCell>().ToList().ForEach(t => { t.Style.ForeColor = Color.Black; t.Value = ""; });
        }
        private void Calculate()
        {
            ResetDataGridViewCellStyle(); // Сбрасываем цвет ячеек в таблице последних 13 спинов
            for (int i = 0; i < 13 && i < spins.Count; i++)
            {
                //Спин на текущей итерации
                int spin = spins[spins.Count - (i + 1)];
                lastThirteenSpinsDataGridView.Rows[0].Cells[i].Value = spin;
                //Присваиваем цвет связанный с текущим цветом.
                if (spin == 0)
                {
                    lastThirteenSpinsDataGridView.Rows[0].Cells[i].Style.ForeColor = Color.LightGreen;
                }
                else if (RedSpins.Contains(spin))
                {
                    lastThirteenSpinsDataGridView.Rows[0].Cells[i].Style.ForeColor = Color.Firebrick;
                }
            }
            // Обрабатываем таблицу всех спинов
            if (spins.Count > 13)
            {
                int rowsCount = (int)Math.Ceiling((spins.Count - 13) / 15.0); // Необходимое количество строк для отображения всех спинов - 13( Они в верхней таблице)
                if (lastOperation == 1) // Если спин был добавлен
                {
                    if (dataGridView1.Rows.Count < rowsCount)
                    {
                        dataGridView1.Rows.Add();
                    }

                    int spin = spins[spins.Count - 14];
                    int dgRows = dataGridView1.Rows.Count - 1;
                    for (int i = dgRows; i >= 0; i--)
                    {
                        for (int z = dataGridView1.Columns.Count - 1; z >= 0; z--)
                        {
                            if (dataGridView1.Rows[i].Cells[z].Value == null)
                                continue;

                            if (i == dgRows && z == dataGridView1.Columns.Count - 1)
                            {
                                dataGridView1.Rows.Add();
                                dataGridView1.Rows[i + 1].Cells[0].Value = dataGridView1.Rows[i].Cells[z].Value;
                            }
                            else if (z == dataGridView1.Columns.Count - 1)
                            {
                                dataGridView1.Rows[i + 1].Cells[0].Value = dataGridView1.Rows[i].Cells[z].Value;
                            }
                            else
                            {
                                dataGridView1.Rows[i].Cells[z + 1].Value = dataGridView1.Rows[i].Cells[z].Value;

                            }
                        }
                    }
                    dataGridView1.Rows[0].Cells[0].Value = spin;

                }
                else if (lastOperation == 2) // Если спин был отменен
                {
                    int dgRows = dataGridView1.Rows.Count - 1;
                    int tableSpinsCount = spins.Count - 13;
                    int lastSpinRowIndex = (int)Math.Ceiling(tableSpinsCount / 15.0) - 1;
                    int lastSpinCellIndex = tableSpinsCount - (lastSpinRowIndex * 15);
                    for (int i = 0; i <= dgRows; i++)
                    {
                        for (int z = 0; z < dataGridView1.ColumnCount; z++)
                        {
                            if (i == 0 && z == 0) continue;
                            if (z == 0)
                                dataGridView1.Rows[i - 1].Cells[dataGridView1.ColumnCount - 1].Value = dataGridView1.Rows[i].Cells[0].Value;
                            else
                                dataGridView1.Rows[i].Cells[z - 1].Value = dataGridView1.Rows[i].Cells[z].Value;
                        }
                    }
                    if (dataGridView1.Rows.Count > rowsCount) // Если спин был последним в текущей строке - удаляем последнюю строку
                    {
                        dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1);
                    }
                    else
                    {

                        dataGridView1.Rows[lastSpinRowIndex].Cells[lastSpinCellIndex].Value = null;
                    }
                }

            }
            else
            {
                dataGridView1.Rows.Clear();
            }


            Dictionary<int, List<double>> sectorsNotFallout = new Dictionary<int, List<double>>();
            for (int i = 0; i < Sequence.Length; i++)
            {
                int currentSector = Sequence[i];
                int lastIndex = spins.Count;
                sectorsNotFallout.Add(currentSector, new List<double>());
                for (int z = spins.Count - 1; z >= 0; z--)
                {
                    if (spins[z] == currentSector)
                    {
                        sectorsNotFallout[currentSector].Add(lastIndex - z - 1);
                        lastIndex = z;
                    }
                }
                //sectorsNotFallout[currentSector].Reverse();
            }
            int sum = 0;
            for (int i = 0; i < sectorsNotFallout.Count; i++)
            {
                int number = sectorsNotFallout.ElementAt(i).Key;
                List<double> nonFallouts = sectorsNotFallout[number];
                for (int z = 1; z < nonFallouts.Count; z++)
                {
                    nonFallouts[z] += nonFallouts[z - 1];
                }
                for (int z = 1; z < nonFallouts.Count; z++)
                {
                    nonFallouts[z] /= z + 1.0;
                }
                double total = nonFallouts.Sum() / nonFallouts.Count;
                if (double.IsNaN(total))
                {
                    total = 0;
                }
                int rounded = (int)Math.Round(total);
                sum += rounded;
                if (!blocked)
                {
                    if (nonFallouts.Count != 0)
                    {
                        notFalloutsDgView.Rows[2].Cells[i].Value = nonFallouts.First();
                    }
                    else
                    {
                        notFalloutsDgView.Rows[2].Cells[i].Value = spins.Count;
                    }
                    notFalloutsDgView.Rows[3].Cells[i].Value = rounded;
                }
            }
            Dictionary<int, int> lastFalloutsSector = null;
            double norma40 = 1.4 * (spins.Count / (double)37); ;
            double norma20 = 1.2 * (spins.Count / (double)37);
            for (int z = 0; z < FalloutIntervals.Length; z++)
            {
                Dictionary<int, int> falloutsSector = new Dictionary<int, int>();
                for (int i = 0; i < Sequence.Length; i++)
                {
                    int currentSector = Sequence[i];
                    falloutsSector.Add(currentSector, 0);
                }
                int count = FalloutIntervals[z] == 0 ? spins.Count : FalloutIntervals[z];
                if (z == 0)
                {
                    CalculateFallouts(falloutsSector, count);
                }
                else
                {
                    for (int e = 0; e < lastFalloutsSector.Count; e++)
                    {
                        falloutsSector[e] = lastFalloutsSector[e];
                    }
                    CalculateFallouts(falloutsSector, count, FalloutIntervals[z - 1]);
                }
                lastFalloutsSector = falloutsSector;
                if (!blocked)
                {
                    var schema = schemas.FirstOrDefault(t => t.Count == FalloutIntervals[z]);
                    for (int i = 0; i < Sequence.Length; i++)
                    {
                        falloutDgView.Rows[z].Cells[i].Value = falloutsSector.ElementAt(i).Value;
                        if (schema != null)
                        {
                            if (schema.Count == 0)
                            {
                                if (falloutsSector.ElementAt(i).Value >= norma40)
                                {
                                    falloutDgView.Rows[z].Cells[i].Style.ForeColor = Color.Green;
                                }
                                else if (falloutsSector.ElementAt(i).Value >= norma40)
                                    falloutDgView.Rows[z].Cells[i].Style.ForeColor = Color.Black;
                                else
                                    falloutDgView.Rows[z].Cells[i].Style.ForeColor = Color.Red;

                            }
                            else
                            {
                                if (falloutsSector.ElementAt(i).Value >= schema.Green)
                                    falloutDgView.Rows[z].Cells[i].Style.ForeColor = Color.Green;
                                else if (falloutsSector.ElementAt(i).Value >= schema.Yellow)
                                {
                                    falloutDgView.Rows[z].Cells[i].Style.ForeColor = Color.Black;
                                }
                                else
                                    falloutDgView.Rows[z].Cells[i].Style.ForeColor = Color.Red;
                            }
                        }
                    }
                }
            }

            var nb = GetNeighbours(spins.Last());
            int pos = 0, pos1 = 0, pos2 = 0;
            for (int i = spins.Count - 1; i > 0; i--)
            {
                pos++;
                if (nb.Contains(spins[i]))
                {
                    break;
                }
            }
            for (int i = spins.Count - 1; i > 1; i--)
            {
                pos1++;
                if (nb.Contains(spins[i]) && nb.Contains(spins[i - 1]))
                {
                    break;
                }
            }
            for (int i = spins.Count - 1; i > 1; i--)
            {
                pos2++;
                if (spins[i] == spins[i - 1])
                {
                    break;
                }
            }
            double degree1 = 0, degree2 = 0;
            if (spins.Count > 1)
            {
                int prevSpin = spins[spins.Count - 2];
                int last = spins.Last();
                degree1 = lengths[last][prevSpin];
                degree2 = lengths[prevSpin][last];

            }
            if (lastOperation == 1)
            {
                dataGridView2.Rows.Add
                    (
                    spins.Count,
                    spins.Last(),
                    sum,
                    Calculate37(),
                    pos == spins.Count ? "∞" : pos.ToString(),
                    pos1 == spins.Count ? "∞" : pos1.ToString(),
                    pos2 == spins.Count ? "∞" : pos2.ToString(),
                    degree1 + " " + degree2
                    );
            }
            else
            {
                dataGridView2.Rows.Remove(dataGridView1.Rows[dataGridView1.Rows.Count - 1]);
            }
            dataGridView2.FirstDisplayedScrollingRowIndex = dataGridView2.Rows.Count - 1;
            textBox1.Text = spins.Count.ToString();

        }
        private List<int> GetNeighbours(int spin)
        {
            var node = linkedList.head;
            while (true)
            {
                if (node.Value == spin)
                    break;
                node = node.Next;
            }
            return new List<int>()
            {
                node.Previous.Previous.Value,
                node.Previous.Value,
                node.Next.Value,
                node.Next.Next.Value,
            };
        }
        private int Calculate37()
        {
            List<int> tmp = new List<int>();
            for (int i = 0; i < 37 && i < spins.Count; i++)
            {
                tmp.Add(spins[spins.Count - i - 1]);
            }
            return tmp.GroupBy(t => t).Count();
        }
        private void CalculateFallouts(Dictionary<int, int> numbers, int count, int skip = 0)
        {
            for (int i = skip; i < count && i < spins.Count; i++)
            {
                numbers[spins[spins.Count - (i + 1)]]++;
            }
        }
        /// <summary>
        /// Инициализация контролов при старте программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //Добавление в таблицу последних 13 спинов соответствующих колонок и строки
            for (int i = 0; i < 13; i++)
            {
                lastThirteenSpinsDataGridView.Columns.Add("column_" + i, "");
            }
            lastThirteenSpinsDataGridView.Rows.Add();
            //Добавляем в основную таблицу 15 колонок
            for (int i = 0; i < 15; i++)
            {
                dataGridView1.Columns.Add("column_" + i, "");
            }
            //Добавляем в таблицу невыпадений колонки(37) для каждого спина
            //Добавляем в основную таблицу 15 колонок
            for (int i = 0; i < 37; i++)
            {
                notFalloutsDgView.Columns.Add("column_" + i, "");
                falloutDgView.Columns.Add("column_" + i, "");
                identifierDgView.Columns.Add("column_" + i, "");
            }
            falloutDgView.Columns.Add("column_" + 37, "");
            for (int i = 0; i < 4; i++)
            {
                notFalloutsDgView.Rows.Add();
                notFalloutsDgView.Rows[i].Height = 14;
            }
            notFalloutsDgView.Rows[0].Height = 18;
            notFalloutsDgView.Rows[1].Height = 10;

            for (int i = 0; i < 3; i++)
            {
                identifierDgView.Rows.Add();
                identifierDgView.Rows[i].Height = 10;
            }
            identifierDgView.Rows[1].Height = 22;
            for (int i = 0; i < 12; i++)
            {
                falloutDgView.Rows.Add();
                falloutDgView.Rows[i].Height = 13;
            }

            for (int i = 0; i < 2; i++)
            {
                for (int z = 0; z < 37; z++)
                {
                    notFalloutsDgView.Rows[i + 2].Cells[z].Value = "00";
                }
            }
            for (int i = 0; i < Sequence.Length; i++)
            {
                identifierDgView.Rows[1].Cells[i].Value = Sequence[i];
                if (Sequence[i] == 0)
                {
                    identifierDgView.Rows[1].Cells[i].Style.ForeColor = Color.LightGreen;
                }
                else if (RedSpins.Contains(Sequence[i]))
                {
                    identifierDgView.Rows[1].Cells[i].Style.ForeColor = Color.Firebrick;
                }
            }
            for (int i = 0; i < FalloutIntervals.Length; i++)
            {
                for (int z = 0; z < 37; z++)
                {
                    falloutDgView.Rows[i].Cells[z].Value = "00";
                }
                falloutDgView.Rows[i].Cells[37].Value = FalloutIntervals[i];
            }
            linkedList = new CircularLinkedList<int>();
            for (int i = 0; i < Sequence.Length; i++)
            {
                linkedList.AddLast(Sequence[i]);
            }
            falloutDgView.Rows[FalloutIntervals.Length - 1].Cells[37].Style.BackColor = Color.Gray;


            schemas.Add(new ColorSchema(37, 2, 1, 0));
            schemas.Add(new ColorSchema(55, 3, 1, 0));
            schemas.Add(new ColorSchema(111, 5, 2, 0));
            schemas.Add(new ColorSchema(296, 12, 6, 0));
            schemas.Add(new ColorSchema(0, -1, -1, -1));


            for (int i = 0; i < Sequence.Length; i++)
            {
                int spin = Sequence[i];
                lengths.Add(spin, new Dictionary<int, int>());
                for (int z = 0; z < Sequence.Length; z++)
                {
                    int spin2 = Sequence[z];
                    if (i > z)
                        lengths[spin].Add(spin2, Convert.ToInt32(Math.Round((i - z) * 9.73 / 10) * 10));
                    else
                        lengths[spin].Add(spin2, Convert.ToInt32(Math.Round((i + (37 - z)) * 9.73 / 10) * 10));
                }
            }
        }
        private void CancelSpinBtn_Click(object sender, EventArgs e)
        {
            if (spins.Count == 0)
            {
                return;
            }

            spins.RemoveLast();
            lastOperation = 2;
            Calculate();
            Controls.OfType<UserControl1>().ToList().ForEach(t => t.MoveBack());
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
        }
        private void SaveSpinsBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                FileName = "Text File",
                Filter = "Text File|*.txt"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, string.Join(", ", spins));
            }
        }
        private void loadSpinsBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Text File|*.txt",
                FileName = "Text file"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.SuspendLayout();
                Stopwatch st = new Stopwatch();
                st.Start();
                blocked = true;
                dataGridView1.ScrollBars = ScrollBars.None;
                dataGridView1.Rows.Clear();
                dataGridView2.Rows.Clear();
                List<int> sps = File.ReadAllText(ofd.FileName).Replace(" ", "").Split(',').Select(t => Convert.ToInt32(t)).ToList();
                this.Controls.Remove(dataGridView1);

                for (int i = 0; i < sps.Count; i++)
                {
                    if (i == sps.Count - 1)
                        blocked = false;
                    Button btn = Controls.Cast<Control>().First(t => t.Name == "button" + (sps[i] + 1)) as Button;
                    btn.PerformClick();
                    this.Text = "Европейская Рулетка - Основная версия v.1.0 - " + (i / (float)sps.Count) * 100;
                }
                this.Controls.Add(dataGridView1);
                st.Stop();
                this.ResumeLayout();
                MessageBox.Show(st.Elapsed.TotalSeconds.ToString());
                dataGridView1.ScrollBars = ScrollBars.Vertical;

            }
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
        }
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (blocked)
                return;
            int spin = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
            if (spin == 0)
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.LightGreen;
            }
            else if (RedSpins.Contains(spin))
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Firebrick;
            }
            else
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Black;

            }
        }
        #endregion
    }
}
