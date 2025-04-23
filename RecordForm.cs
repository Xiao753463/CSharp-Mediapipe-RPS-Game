using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace C__mediapipe_winform
{
    public partial class RecordForm : Form
    {
        public RecordForm()
        {
            InitializeComponent();
            LoadRecords();
        }

        private void LoadRecords()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\PSRRecordDB.mdf;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM 積分紀錄 WHERE 玩家一 <> 玩家二";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Convert DataTable to array
                var recordsArray = dataTable.AsEnumerable().Select(row => row.ItemArray).ToArray();

                // Calculate win rate
                var winRates = CalculateWinRates(recordsArray);

                // Create a new DataTable to display win rates
                DataTable winRateTable = new DataTable();
                winRateTable.Columns.Add("玩家", typeof(string));
                winRateTable.Columns.Add("勝率", typeof(double));

                foreach (var winRate in winRates.OrderByDescending(wr => wr.Value))
                {
                    winRateTable.Rows.Add(winRate.Key, winRate.Value);
                }

                dataGridViewRecords.DataSource = winRateTable;
            }
        }

        private Dictionary<string, double> CalculateWinRates(object[][] recordsArray)
        {
            var winCounts = new Dictionary<string, int>();
            var totalCounts = new Dictionary<string, int>();

            foreach (var record in recordsArray)
            {
                string player1 = record[1].ToString();
                string player2 = record[2].ToString();
                int result = Convert.ToInt32(record[3]);

                if (!totalCounts.ContainsKey(player1))
                {
                    totalCounts[player1] = 0;
                    winCounts[player1] = 0;
                }
                if (!totalCounts.ContainsKey(player2))
                {
                    totalCounts[player2] = 0;
                    winCounts[player2] = 0;
                }

                totalCounts[player1]++;
                totalCounts[player2]++;

                if (result == 1)
                {
                    winCounts[player1]++;
                }
                else if (result == 2)
                {
                    winCounts[player2]++;
                }
            }

            var winRates = new Dictionary<string, double>();
            foreach (var player in totalCounts.Keys)
            {
                winRates[player] = Math.Round((double)winCounts[player] / totalCounts[player], 2);
            }

            return winRates;
        }

        private void InitializeComponent()
        {
            this.dataGridViewRecords = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRecords)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewRecords
            // 
            this.dataGridViewRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRecords.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewRecords.Name = "dataGridViewRecords";
            this.dataGridViewRecords.ReadOnly = true;
            this.dataGridViewRecords.RowHeadersWidth = 51;
            this.dataGridViewRecords.RowTemplate.Height = 24;
            this.dataGridViewRecords.Size = new System.Drawing.Size(760, 437);
            this.dataGridViewRecords.TabIndex = 0;

            // 美化 DataGridView 樣式
            this.dataGridViewRecords.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.dataGridViewRecords.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.dataGridViewRecords.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            this.dataGridViewRecords.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            this.dataGridViewRecords.EnableHeadersVisualStyles = false;

            this.dataGridViewRecords.DefaultCellStyle.BackColor = Color.White;
            this.dataGridViewRecords.DefaultCellStyle.ForeColor = Color.Black;
            this.dataGridViewRecords.DefaultCellStyle.SelectionBackColor = Color.SteelBlue;
            this.dataGridViewRecords.DefaultCellStyle.SelectionForeColor = Color.White;

            this.dataGridViewRecords.AlternatingRowsDefaultCellStyle.BackColor = Color.LightBlue;
            this.dataGridViewRecords.GridColor = Color.Gray;
            this.dataGridViewRecords.BorderStyle = BorderStyle.Fixed3D;

            // 
            // RecordForm
            // 
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.dataGridViewRecords);
            this.Name = "RecordForm";
            this.Text = "積分紀錄";
            this.Load += new System.EventHandler(this.RecordForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRecords)).EndInit();
            this.ResumeLayout(false);
        }


        private System.Windows.Forms.DataGridView dataGridViewRecords;

        private void RecordForm_Load(object sender, EventArgs e)
        {

        }
    }
}
