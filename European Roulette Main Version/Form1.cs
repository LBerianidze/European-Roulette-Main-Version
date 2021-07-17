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
        List<int> spins = new List<int>();
        int lastSpin = -1;
        int lastRowIndex = -1;
        int lastCellIndex = -1;
        /// <summary>
        /// Spin Operation Type. 1 - Spin Added, 2 - Spin Cancelled
        /// </summary>
        byte lastOperation = 0;
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
                if(spin == 0)
                {
                    lastThirteenSpinsDataGridView.Rows[0].Cells[i].Style.ForeColor = Color.LightGreen;
                }
                else if(RedSpins.Contains(spin))
                {
                    lastThirteenSpinsDataGridView.Rows[0].Cells[i].Style.ForeColor = Color.Firebrick;
                }

            }
            // Обрабатываем таблицу всех спинов
            if(spins.Count > 13)
            {
                int rowsCount = (int)Math.Ceiling((spins.Count - 13) / 15.0); // Необходимое количество строк для отображения всех спинов - 13( Они в верхней таблице)
                if(lastOperation == 1)
                {
                    if (dataGridView1.Rows.Count < rowsCount)
                        dataGridView1.Rows.Add();

                    int tableSpinsCount = spins.Count - 13;
                    int lastSpinRowIndex = (int)Math.Floor(tableSpinsCount / 15.0);
                    int lastSpinCellIndex = tableSpinsCount - (lastSpinRowIndex * 15) - 1;
                    dataGridView1.Rows[lastSpinRowIndex].Cells[lastSpinCellIndex].Value = spins[spins.Count - 14];
                }
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;

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
