using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace European_Roulette_Main_Version.CustomControls
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 50; i++)
            {
                dataGridView1.Columns.Add("column_" + i, "");
            }
            dataGridView1.Columns.Cast<DataGridViewColumn>().ToList().ForEach(t => t.Width = 22);
            dataGridView1.Rows.Add();
            dataGridView1.ClearSelection();
        }
        int currentSelected = -1;
        bool canGo = false;
        public void MoveNext()
        {
            if (currentSelected >= 49 || !canGo)
                return;
            dataGridView1.Rows[0].Cells.Cast<DataGridViewCell>().ToList().ForEach(t => t.Style.BackColor = Color.White);
            dataGridView1.Rows[0].Cells[++currentSelected].Style.BackColor = Color.LightPink;
            dataGridView1.FirstDisplayedScrollingColumnIndex = currentSelected;
        }
        public void MoveBack()
        {
            if (currentSelected <= 0 || !canGo)
                return;
            dataGridView1.Rows[0].Cells.Cast<DataGridViewCell>().ToList().ForEach(t => t.Style.BackColor = Color.White);
            dataGridView1.Rows[0].Cells[--currentSelected].Style.BackColor = Color.LightPink;
            if (dataGridView1.FirstDisplayedScrollingColumnIndex > currentSelected)
                dataGridView1.FirstDisplayedScrollingColumnIndex = currentSelected;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            MoveBack();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MoveNext();
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            canGo = false;
            dataGridView1.Rows[0].Cells.Cast<DataGridViewCell>().ToList().ForEach(t => t.Style.BackColor = Color.White);
            dataGridView1.FirstDisplayedScrollingColumnIndex = 0;
        }

        private void resumeBtn_Click(object sender, EventArgs e)
        {
            canGo = true;
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
            canGo = false;
        }
    }
}
