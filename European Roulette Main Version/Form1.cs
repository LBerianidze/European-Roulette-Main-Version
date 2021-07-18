using European_Roulette_Main_Version.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace European_Roulette_Main_Version
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.InitializeComponent();
        }
        private static readonly int[] RedSpins = { 1, 3, 5, 9, 12, 7, 14, 18, 16, 21, 19, 23, 27, 25, 30, 32, 36, 34 };
        private static readonly int[] FalloutIntervals = { 11, 14, 19, 28, 37, 55, 74, 111, 185, 295, 0 };
        private List<int> spins = new List<int>();
        private int lastSpin = -1;

        /// <summary>
        /// Spin Operation Type. 1 - Spin Added, 2 - Spin Cancelled
        /// </summary>
        private byte lastOperation = 0;
        private static readonly int[] Sequence = { 22, 18, 29, 7, 28, 12, 35, 3, 26, 0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1, 20, 14, 31, 9 };
        private void Button35_Click(object sender, EventArgs e)
        {
            this.lastSpin = Convert.ToInt32((sender as Control).Text);
            this.spins.Add(this.lastSpin);
            this.lastOperation = 1;
            this.Calculate();
        }
        private void ResetDataGridViewCellStyle()
        {
            this.lastThirteenSpinsDataGridView.Rows[0].Cells.Cast<DataGridViewCell>().ToList().ForEach(t => { t.Style.ForeColor = Color.Black; t.Value = ""; });
        }
        private void Calculate()
        {
            this.ResetDataGridViewCellStyle(); // Сбрасываем цвет ячеек в таблице последних 13 спинов
            for (var i = 0; i < 13 && i < this.spins.Count; i++)
            {
                //Спин на текущей итерации
                var spin = this.spins[this.spins.Count - (i + 1)];
                this.lastThirteenSpinsDataGridView.Rows[0].Cells[i].Value = spin;
                //Присваиваем цвет связанный с текущим цветом.
                if (spin == 0)
                {
                    this.lastThirteenSpinsDataGridView.Rows[0].Cells[i].Style.ForeColor = Color.LightGreen;
                }
                else if (RedSpins.Contains(spin))
                {
                    this.lastThirteenSpinsDataGridView.Rows[0].Cells[i].Style.ForeColor = Color.Firebrick;
                }

            }
            // Обрабатываем таблицу всех спинов
            if (this.spins.Count > 13)
            {
                var rowsCount = (int)Math.Ceiling((this.spins.Count - 13) / 15.0); // Необходимое количество строк для отображения всех спинов - 13( Они в верхней таблице)
                if (this.lastOperation == 1) // Если спин был добавлен
                {
                    if (this.dataGridView1.Rows.Count < rowsCount)
                    {
                        this.dataGridView1.Rows.Add();
                    }

                    var spin = this.spins[this.spins.Count - 14];
                    var tableSpinsCount = this.spins.Count - 13;
                    var lastSpinRowIndex = (int)Math.Ceiling(tableSpinsCount / 15.0) - 1;
                    var lastSpinCellIndex = tableSpinsCount - (lastSpinRowIndex * 15) - 1;
                    this.dataGridView1.Rows[lastSpinRowIndex].Cells[lastSpinCellIndex].Value = spin;
                    if (spin == 0)
                    {
                        this.dataGridView1.Rows[lastSpinRowIndex].Cells[lastSpinCellIndex].Style.ForeColor = Color.LightGreen;
                    }
                    else if (RedSpins.Contains(spin))
                    {
                        this.dataGridView1.Rows[lastSpinRowIndex].Cells[lastSpinCellIndex].Style.ForeColor = Color.Firebrick;
                    }
                }
                else if (this.lastOperation == 2) // Если спин был отменен
                {
                    if (this.dataGridView1.Rows.Count > rowsCount) // Если спин был последним в текущей строке - удаляем последнюю строку
                    {
                        this.dataGridView1.Rows.RemoveAt(this.dataGridView1.Rows.Count - 1);
                    }
                    else // иначе, очищаем последнюю заполненную данными ячейке в таблице
                    {
                        var tableSpinsCount = this.spins.Count - 13;
                        var lastSpinRowIndex = (int)Math.Ceiling(tableSpinsCount / 15.0) - 1;
                        var lastSpinCellIndex = tableSpinsCount - (lastSpinRowIndex * 15);
                        this.dataGridView1.Rows[lastSpinRowIndex].Cells[lastSpinCellIndex].Value = "";
                    }
                }
                this.dataGridView1.FirstDisplayedScrollingRowIndex = this.dataGridView1.Rows.Count - 1;


            }
            else
            {
                this.dataGridView1.Rows.Clear();
            }

            var sectorsNotFallout = new Dictionary<int, List<double>>();
            for (var i = 0; i < Sequence.Length; i++)
            {
                var currentSector = Sequence[i];
                var lastIndex = this.spins.Count;
                sectorsNotFallout.Add(currentSector, new List<double>());
                for (var z = this.spins.Count - 1; z >= 0; z--)
                {
                    if (this.spins[z] == currentSector)
                    {
                        sectorsNotFallout[currentSector].Add(lastIndex - z - 1);
                        lastIndex = z;
                    }
                }
                //sectorsNotFallout[currentSector].Reverse();
            }
            for (var i = 0; i < sectorsNotFallout.Count; i++)
            {
                var number = sectorsNotFallout.ElementAt(i).Key;
                var nonFallouts = sectorsNotFallout[number];
                if (nonFallouts.Count != 0)
                {
                    this.notFalloutsDgView.Rows[2].Cells[i].Value = nonFallouts.First();
                }
                else
                {
                    this.notFalloutsDgView.Rows[2].Cells[i].Value = this.spins.Count;
                }
                for (var z = 1; z < nonFallouts.Count; z++)
                {
                    nonFallouts[z] += nonFallouts[z - 1];
                }
                for (var z = 1; z < nonFallouts.Count; z++)
                {
                    nonFallouts[z] /= z + 1.0;
                }
                var total = nonFallouts.Sum() / nonFallouts.Count;
                if (Double.IsNaN(total))
                {
                    total = 0;
                }

                this.notFalloutsDgView.Rows[3].Cells[i].Value = (int)Math.Round(total);
            }
            Dictionary<int, int> lastFalloutsSector = null;
            for (var z = 0; z < FalloutIntervals.Length; z++)
            {
                var falloutsSector = new Dictionary<int, int>();
                for (var i = 0; i < Sequence.Length; i++)
                {
                    var currentSector = Sequence[i];
                    falloutsSector.Add(currentSector, 0);
                }
                var count = FalloutIntervals[z] == 0 ? this.spins.Count : FalloutIntervals[z];
                if (z == 0)
                {
                    this.CalculateFallouts(falloutsSector, count);
                }
                else
                {
                    for (var e = 0; e < lastFalloutsSector.Count; e++)
                    {
                        falloutsSector[e] = lastFalloutsSector[e];
                    }
                    this.CalculateFallouts(falloutsSector, count, FalloutIntervals[z - 1]);
                }
                lastFalloutsSector = falloutsSector;
                for (var i = 0; i < Sequence.Length; i++)
                {
                    this.falloutDgView.Rows[z].Cells[i].Value = falloutsSector.ElementAt(i).Value;
                }
            }

        }
        private void CalculateFallouts(Dictionary<int, int> numbers, int count, int skip = 0)
        {
            for (var i = skip; i < count && i < this.spins.Count; i++)
            {
                numbers[this.spins[this.spins.Count - (i + 1)]]++;
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
            for (var i = 0; i < 13; i++)
            {
                this.lastThirteenSpinsDataGridView.Columns.Add("column_" + i, "");
            }
            this.lastThirteenSpinsDataGridView.Rows.Add();
            //Добавляем в основную таблицу 15 колонок
            for (var i = 0; i < 15; i++)
            {
                this.dataGridView1.Columns.Add("column_" + i, "");
            }
            //Добавляем в таблицу невыпадений колонки(37) для каждого спина
            //Добавляем в основную таблицу 15 колонок
            for (var i = 0; i < 37; i++)
            {
                this.notFalloutsDgView.Columns.Add("column_" + i, "");
                this.falloutDgView.Columns.Add("column_" + i, "");
                this.identifierDgView.Columns.Add("column_" + i, "");
            }
            this.falloutDgView.Columns.Add("column_" + 37, "");
            for (var i = 0; i < 4; i++)
            {
                this.notFalloutsDgView.Rows.Add();
                this.notFalloutsDgView.Rows[i].Height = 14;
            }
            this.notFalloutsDgView.Rows[0].Height = 18;
            this.notFalloutsDgView.Rows[1].Height = 10;

            for (var i = 0; i < 3; i++)
            {
                this.identifierDgView.Rows.Add();
                this.identifierDgView.Rows[i].Height = 10;
            }
            this.identifierDgView.Rows[1].Height = 22;
            for (var i = 0; i < 12; i++)
            {
                this.falloutDgView.Rows.Add();
                this.falloutDgView.Rows[i].Height = 13;
            }

            for (var i = 0; i < 2; i++)
            {
                for (var z = 0; z < 37; z++)
                {
                    this.notFalloutsDgView.Rows[i + 2].Cells[z].Value = "00";
                }
            }
            for (var i = 0; i < Sequence.Length; i++)
            {
                this.identifierDgView.Rows[1].Cells[i].Value = Sequence[i];
                if (Sequence[i] == 0)
                {
                    this.identifierDgView.Rows[1].Cells[i].Style.ForeColor = Color.LightGreen;
                }
                else if (RedSpins.Contains(Sequence[i]))
                {
                    this.identifierDgView.Rows[1].Cells[i].Style.ForeColor = Color.Firebrick;
                }
            }
            for (var i = 0; i < 11; i++)
            {
                for (var z = 0; z < 37; z++)
                {
                    this.falloutDgView.Rows[i].Cells[z].Value = "00";
                }
                this.falloutDgView.Rows[i].Cells[37].Value = FalloutIntervals[i];
            }
            this.falloutDgView.Rows[11].Cells[37].Style.BackColor = Color.Gray;
        }

        private void CancelSpinBtn_Click(object sender, EventArgs e)
        {
            if (this.spins.Count == 0)
            {
                return;
            }

            this.spins.RemoveLast();
            this.lastOperation = 2;
            this.Calculate();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
        }

        private void SaveSpinsBtn_Click(object sender, EventArgs e)
        {
            var r = new Random();
            for (var i = 0; i < 10000; i++)
            {
                var rand = r.Next(0, 36);
                var btn = this.Controls.Cast<Control>().First(t => t.Name == "button" + (rand + 1)) as Button;
                btn.PerformClick();
            }
        }
    }
}
