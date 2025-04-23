using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;


namespace C__mediapipe_winform
{
    public partial class SinglePlayerForm : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private string playerGesture = "";
        private string computerGesture = "";
        private int playerScore = 0;
        private int computerScore = 0;
        private bool isGameRunning = false;

        // ============== 新增：倒數計時相關 ==============
        private Timer countdownTimer;
        private int countdown = 3; // 倒數秒數，可自行調整


        // ============== UI 美化 ==============
        // ============== UI 美化 ==============
        private void CustomizeUI()
        {
            this.Text = "單人模式 - 猜拳遊戲";
            this.BackColor = Color.WhiteSmoke;

            // 美化標籤字樣與大小
            lblStatus.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblPlayerGesture.Font = new Font("Segoe UI", 12F);
            lblComputerGesture.Font = new Font("Segoe UI", 12F);
            lblScore.Font = new Font("Segoe UI", 12F);
            lblResult.Font = new Font("Segoe UI", 14F, FontStyle.Bold);

            // 美化按鈕樣式
            Button[] buttons = { btnStart, btnPlay };
            foreach (var button in buttons)
            {
                button.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.BorderColor = Color.Gray;
                button.FlatAppearance.MouseOverBackColor = Color.LightGray;
                button.Cursor = Cursors.Hand;
            }
        }
        public SinglePlayerForm()
        {
            InitializeComponent();
            InitializeGame();
            CustomizeUI(); // 美化介面
            Task.Run(() => RunPythonScript("127.0.0.1", 5000));
        }
        private string ShowInputDialog(string title, string prompt)
        {
            Form promptForm = new Form()
            {
                Width = 400,
                Height = 200,
                Text = title,
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label textLabel = new Label() { Left = 20, Top = 20, Text = prompt, Width = 340, Font = new Font("Segoe UI", 12F) };
            TextBox inputBox = new TextBox() { Left = 20, Top = 50, Width = 340, Font = new Font("Segoe UI", 12F) };
            Button confirmation = new Button() { Text = "確定", Left = 270, Width = 90, Top = 100, DialogResult = DialogResult.OK, Font = new Font("Segoe UI", 12F, FontStyle.Bold) };

            confirmation.Click += (sender, e) => { promptForm.Close(); };

            promptForm.Controls.Add(textLabel);
            promptForm.Controls.Add(inputBox);
            promptForm.Controls.Add(confirmation);
            promptForm.AcceptButton = confirmation;

            // 加入淡入動畫效果
            promptForm.Opacity = 0;
            Timer fadeInTimer = new Timer { Interval = 30 };
            fadeInTimer.Tick += (s, e) =>
            {
                if (promptForm.Opacity < 1)
                {
                    promptForm.Opacity += 0.05;
                }
                else
                {
                    fadeInTimer.Stop();
                }
            };
            fadeInTimer.Start();

            return promptForm.ShowDialog() == DialogResult.OK ? inputBox.Text : string.Empty;
        }

        private string playerName = ""; // 儲存玩家名稱

        private void InitializeGame()
        {
            // 設定按鈕初始狀態
            btnStart.Enabled = true;
            btnPlay.Enabled = false;
            //btnReset.Enabled = false;

            // 初始化倒數 Timer
            countdownTimer = new Timer();
            countdownTimer.Interval = 1000; // 1秒跳一次
            countdownTimer.Tick += CountdownTimer_Tick;

            // 要求玩家輸入名稱
            GetPlayerName();
        }
        private void GetPlayerName()
        {
            while (string.IsNullOrEmpty(playerName))
            {
                playerName = ShowInputDialog("玩家名稱輸入", "請輸入您的名稱以開始遊戲：");

                if (string.IsNullOrEmpty(playerName))
                {
                    MessageBox.Show("玩家名稱不能為空，請再次輸入！");
                }
            }

            lblStatus.Text = $"歡迎，{playerName}！請按「開始遊戲」連接伺服器。";
        }

        private void RunPythonScript(string ipAddress, int port)
        {
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"single_hand_gesture.py {ipAddress} {port}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine(result);
                }
                using (StreamReader reader = process.StandardError)
                {
                    string error = reader.ReadToEnd();
                    Console.WriteLine(error);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // ============== 連線到 Python ==============
        private async void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync("localhost", 5000);
                stream = client.GetStream();

                btnStart.Enabled = false;
                btnPlay.Enabled = true;
                //btnReset.Enabled = true;
                isGameRunning = true;

                // 開始接收 Python 發送的手勢數據
                _ = ReceiveGestureData();

                lblStatus.Text = "遊戲已開始！等待玩家手勢...";
            }
            catch (Exception ex)
            {
                MessageBox.Show("無法連接到 Python 服務器: " + ex.Message);
            }
        }

        private async Task ReceiveGestureData()
        {
            byte[] buffer = new byte[1024];
            while (isGameRunning)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // 連線斷開

                    string gesture = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    this.Invoke((MethodInvoker)delegate
                    {
                        playerGesture = gesture;
                        lblPlayerGesture.Text = $"玩家手勢: {gesture}";
                    });
                }
                catch
                {
                    if (isGameRunning)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            lblStatus.Text = "連接已斷開";
                            StopGame();
                        });
                    }
                    break;
                }
            }
        }

        // ============== 點擊「Play」 => 開始倒數計時 ==============
        private void btnPlay_Click(object sender, EventArgs e)
        {
            // 檢查目前玩家手勢是否有效
            if (playerGesture == "Unknown" || string.IsNullOrEmpty(playerGesture))
            {
                MessageBox.Show("請先做出有效的手勢，或等待 Python 偵測到手勢！");
                return;
            }

            // 開始倒數
            StartCountdown();
        }

        // ============== 啟動 3 秒（或 N 秒）倒數 ==============
        private void StartCountdown()
        {
            countdown = 3; // 你要幾秒就設幾秒
            lblStatus.Text = $"請在 {countdown} 秒內維持手勢！";
            countdownTimer.Start();
        }

        // ============== 每秒遞減倒數計時 ==============
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            countdown--;
            lblStatus.Text = $"請在 {countdown} 秒內維持手勢！";

            if (countdown <= 0)
            {
                // 倒數結束，停止Timer
                countdownTimer.Stop();

                // 電腦隨機出拳
                string[] gestures = new[] { "Rock", "Paper", "Scissors" };
                Random rand = new Random();
                computerGesture = gestures[rand.Next(gestures.Length)];
                lblComputerGesture.Text = $"電腦手勢: {computerGesture}";

                // 和玩家手勢比大小
                DetermineWinner();
            }
        }

        // ============== 比較結果 (玩家 vs. 電腦) ==============
        private void DetermineWinner()
        {
            if (playerGesture == computerGesture)
            {
                lblResult.Text = "平手！";
                //SaveNameToDatabase(playerName, "電腦", 0);
                return;
            }

            bool playerWins =
                   (playerGesture == "Rock" && computerGesture == "Scissors") ||
                   (playerGesture == "Scissors" && computerGesture == "Paper") ||
                   (playerGesture == "Paper" && computerGesture == "Rock");

            if (playerWins)
            {
                playerScore++;
                lblResult.Text = "玩家贏！";
                SaveNameToDatabase(playerName, "電腦", 1);
            }
            else
            {
                computerScore++;
                lblResult.Text = "電腦贏！";
                SaveNameToDatabase(playerName, "電腦", 2);
            }

            lblScore.Text = $"比分 - 玩家: {playerScore} 電腦: {computerScore}";
            // 傳送比分到 Python，並加上換行符
            try
            {
                string scoreMessage = $"Score: Player {playerScore} - Computer {computerScore}\n";
                byte[] scoreData = Encoding.UTF8.GetBytes(scoreMessage);
                stream.Write(scoreData, 0, scoreData.Length);
                stream.Flush(); // 確保資料立即發送
            }
            catch (Exception ex)
            {
                MessageBox.Show($"發送比分時出錯: {ex.Message}");
            }
        }

        private void SaveNameToDatabase(string playerName, string playerName2, int wins)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\PSRRecordDB.mdf;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO 積分紀錄 VALUES (@玩家一, @玩家二, @勝負結果)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@玩家一", playerName); 
                    command.Parameters.AddWithValue("@玩家二", playerName2);
                    command.Parameters.AddWithValue("@勝負結果", wins);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        // ============== 重置分數、手勢、顯示UI ==============
        private void btnReset_Click(object sender, EventArgs e)
        {
            playerScore = 0;
            computerScore = 0;
            lblScore.Text = "比分 - 玩家: 0 電腦: 0";
            lblResult.Text = "";
            lblPlayerGesture.Text = "玩家手勢: ";
            lblComputerGesture.Text = "電腦手勢: ";
            lblStatus.Text = $"歡迎，{playerName}！請按「開始遊戲」連接伺服器。";
        }
        // ============== 停止遊戲、關閉 Socket 連線 ==============
        private void StopGame()
        {
            isGameRunning = false;
            if (client != null)
            {
                client.Close();
            }
            btnStart.Enabled = true;
            btnPlay.Enabled = false;
            //btnReset.Enabled = false;
        }

        // ============== Form 關閉前，停止遊戲、釋放資源 ==============
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopGame();
        }
    }
}
