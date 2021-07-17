using European_Roulette_Main_Version.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace European_Roulette_Main_Version
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static readonly int[] RedSpins = { 1, 3, 5, 9, 12, 7, 14, 18, 16, 21, 19, 23, 27, 25, 30, 32, 36, 34 };
        private static readonly int[] FalloutIntervals = { 11, 14, 19, 28, 37, 55, 74, 111, 185, 295, 0 };
        List<int> spins = new List<int>();
        int lastSpin = -1;
        /// <summary>
        /// Spin Operation Type. 1 - Spin Added, 2 - Spin Cancelled
        /// </summary>
        byte lastOperation = 0;
        private static readonly int[] Sequence = { 22, 18, 29, 7, 28, 12, 35, 3, 26, 0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1, 20, 14, 31, 9 };
        private void Button35_Click(object sender, EventArgs e)
        {
            lastSpin = Convert.ToInt32((sender as Control).Text);
            spins.Add(lastSpin);
            lastOperation = 1;
            Calculate();
        }
        private void ResetDataGridViewCellStyle()
        {
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
                        dataGridView1.Rows.Add();
                    int spin = spins[spins.Count - 14];
                    int tableSpinsCount = spins.Count - 13;
                    int lastSpinRowIndex = (int)Math.Ceiling(tableSpinsCount / 15.0) - 1;
                    int lastSpinCellIndex = tableSpinsCount - (lastSpinRowIndex * 15) - 1;
                    dataGridView1.Rows[lastSpinRowIndex].Cells[lastSpinCellIndex].Value = spin;
                    if (spin == 0)
                        dataGridView1.Rows[lastSpinRowIndex].Cells[lastSpinCellIndex].Style.ForeColor = Color.LightGreen;
                    else if (RedSpins.Contains(spin))
                        dataGridView1.Rows[lastSpinRowIndex].Cells[lastSpinCellIndex].Style.ForeColor = Color.Firebrick;
                }
                else if (lastOperation == 2) // Если спин был отменен
                {
                    if (dataGridView1.Rows.Count > rowsCount) // Если спин был последним в текущей строке - удаляем последнюю строку
                        dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1);
                    else // иначе, очищаем последнюю заполненную данными ячейке в таблице
                    {
                        int tableSpinsCount = spins.Count - 13;
                        int lastSpinRowIndex = (int)Math.Ceiling(tableSpinsCount / 15.0) - 1;
                        int lastSpinCellIndex = tableSpinsCount - (lastSpinRowIndex * 15);
                        dataGridView1.Rows[lastSpinRowIndex].Cells[lastSpinCellIndex].Value = "";
                    }
                }
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;


            }
            else
            {
                dataGridView1.Rows.Clear();
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

            for(int i = 0;i<2;i++)
            {
                for (int z = 0; z < 37; z++)
                {
                    notFalloutsDgView.Rows[i+2].Cells[z].Value = "00";
                }
            }
            for(int i = 0;i<Sequence.Length;i++)
            {
                identifierDgView.Rows[1].Cells[i].Value = Sequence[i];
                if(Sequence[i] == 0)
                    identifierDgView.Rows[1].Cells[i].Style.ForeColor = Color.LightGreen;
                else if(RedSpins.Contains(Sequence[i]))
                    identifierDgView.Rows[1].Cells[i].Style.ForeColor = Color.Firebrick;
            }
            for (int i = 0; i < 11; i++)
            {
                for (int z = 0; z < 37; z++)
                {
                    falloutDgView.Rows[i].Cells[z].Value = "00";
                }
                falloutDgView.Rows[i].Cells[37].Value = FalloutIntervals[i];
            }
            falloutDgView.Rows[11].Cells[37].Style.BackColor = Color.Gray;
        }

        private void CancelSpinBtn_Click(object sender, EventArgs e)
        {
            if (spins.Count == 0)
                return;
            spins.RemoveLast();
            lastOperation = 2;
            Calculate();
        }
    }
}
