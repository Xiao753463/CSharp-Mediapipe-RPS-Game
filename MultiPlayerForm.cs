using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace C__mediapipe_winform
{
    public partial class MultiPlayerForm : Form
    {
        private UdpClient udpClient;
        private const int UDPPort = 9000; // 定義廣播埠號
        private Thread listenThread;
        // 新增兩個變數存放玩家名稱
        private string playerAName = "Player A";
        private string playerBName = "Player B";
        private TcpListener server;
        private bool isServerRunning = false;

        // 兩位玩家的連線與手勢
        private TcpClient clientA;
        private TcpClient clientB;
        private NetworkStream streamA;
        private NetworkStream streamB;
        private string gestureA = ""; // A 的最新手勢
        private string gestureB = ""; // B 的最新手勢

        // 分數或其他資料
        private int scoreA = 0;
        private int scoreB = 0;

        // 用以標記玩家是否「用石頭準備好」
        private bool readyA = false;
        private bool readyB = false;

        // 用以避免重複進入倒數
        private bool isCountingDown = false;


        private string playerName = ""; // 儲存玩家名稱

        public MultiPlayerForm()
        {
            InitializeComponent();
            CustomizeUI();
            ApplyButtonAnimations();
            GetPlayerName();
        }

        private void CustomizeUI()
        {
            this.Text = "雙人連線對戰";
            this.BackColor = Color.WhiteSmoke;

            lblStatus.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblPlayerAGesture.Font = new Font("Segoe UI", 12F);
            lblPlayerBGesture.Font = new Font("Segoe UI", 12F);
            lblScore.Font = new Font("Segoe UI", 12F);
            lblResult.Font = new Font("Segoe UI", 14F, FontStyle.Bold);

            Button[] buttons = { btnStartServer, btnStopServer };
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

        private void ApplyButtonAnimations()
        {
            AddButtonClickAnimation(btnStartServer);
   
            AddButtonClickAnimation(btnStopServer);
        }
        private void AddButtonClickAnimation(Button button)
        {
            button.MouseDown += (s, e) =>
            {
                button.Size = new Size(button.Width - 5, button.Height - 5);
            };

            button.MouseUp += (s, e) =>
            {
                button.Size = new Size(button.Width + 5, button.Height + 5);
            };
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

            lblStatus.Text = $"歡迎，{playerName}！請按「啟動伺服器」建立對戰，或按「連線伺服器」加入對戰";
        }
        private string ShowInputDialog(string title, string prompt)
        {
            Form promptForm = new Form()
            {
                Width = 400,
                Height = 200,
                Text = title,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 20, Top = 20, Text = prompt, Width = 340 };
            TextBox inputBox = new TextBox() { Left = 20, Top = 50, Width = 340 };
            Button confirmation = new Button() { Text = "確定", Left = 270, Width = 90, Top = 100, DialogResult = DialogResult.OK };

            confirmation.Click += (sender, e) => { promptForm.Close(); };

            promptForm.Controls.Add(textLabel);
            promptForm.Controls.Add(inputBox);
            promptForm.Controls.Add(confirmation);
            promptForm.AcceptButton = confirmation;

            return promptForm.ShowDialog() == DialogResult.OK ? inputBox.Text : string.Empty;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Demo：允許跨執行緒更新 UI，正式專案建議使用 Invoke()
            CheckForIllegalCrossThreadCalls = false;
        }

        // ============== (1) 按鈕：啟動伺服器 + 自動啟動 Python 客戶端 ==============
        private void btnStartServer_Click(object sender, EventArgs e)
        {
            try
            {
                udpClient = new UdpClient();
                udpClient.EnableBroadcast = true; // 啟用廣播模式
                // 啟動 C# 伺服器
                IPAddress ip = IPAddress.Any; // 或指定 127.0.0.1
                int port = 5000;
                server = new TcpListener(ip, port);
                server.Start();
                isServerRunning = true;

                lblStatus.Text = "伺服器啟動，等待玩家連線中...";
                Task.Run(() => AcceptClients());
                Thread.Sleep(1000);
                // 伺服器啟動後，執行本機 Python 客戶端 (連線 127.0.0.1:5000)
                Task.Run(() => RunPythonScript("127.0.0.1", port));

            }
            catch (Exception ex)
            {
                lblStatus.Text = "啟動伺服器失敗: " + ex.Message;
            }
        }

        // ============== (2) 按鈕：連線到其他伺服器 (詢問 IP) + 自動啟動 Python 客戶端 ==============
        private void btnConnectServer_Click(object sender, EventArgs e)
        {
            // 跳出簡易輸入框 取得伺服器 IP
            string serverIP = Prompt.ShowDialog("請輸入要連線的伺服器 IP：", "連線伺服器");
            if (string.IsNullOrEmpty(serverIP))
            {
                lblStatus.Text = "取消連線。";
                return;
            }

            udpClient = new UdpClient(UDPPort);
            listenThread = new Thread(ListenForBroadcast);
            listenThread.IsBackground = true;
            listenThread.Start();
            int port = 5000;  // 可以固定，也可再問一次 Port

            // 不啟動 C# 端伺服器，只啟動 Python 客戶端連線到該 IP
            Task.Run(() => RunPythonScript(serverIP, port));

            lblStatus.Text = $"已嘗試連線至伺服器 {serverIP}:{port}，請檢查 Python 視窗。";
        }
        private void ListenForBroadcast()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, UDPPort);

            try
            {
                while (true)
                {
                    byte[] receivedData = udpClient.Receive(ref endPoint);
                    string jsonData = Encoding.UTF8.GetString(receivedData);

                    // 解析JSON數據
                    var componentData = JsonConvert.DeserializeObject<ComponentData>(jsonData);

                    // 在客戶端表單中顯示元件資訊
                    this.Invoke((MethodInvoker)delegate
                    {
                        Control control = this.Controls.Find(componentData.ComponentName, true).FirstOrDefault();
                        if (control == null)
                        {
                            // 動態新增標籤元件
                            Label newLabel = new Label
                            {
                                Name = componentData.ComponentName,
                                Text = componentData.TextContent,
                                AutoSize = true,
                                Location = new Point(10, 10 * (this.Controls.Count + 1))
                            };
                            this.Controls.Add(newLabel);
                        }
                        else
                        {
                            control.Text = componentData.TextContent;
                        }
                    });
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"接收失敗: {ex.Message}");
            }
        }

        private class ComponentData
        {
            public string ComponentName { get; set; }
            public string TextContent { get; set; }
        }

        // ============== 啟動 Python 客戶端的函式 ==============
        private void RunPythonScript(string ipAddress, int port)
        {
            // 假設 hand_client.py 與此可執行檔在同個資料夾
            // 如果不在同一個資料夾，請指定完整路徑
            string pythonScriptPath = "multi_hand_gesture.py";

            ProcessStartInfo start = new ProcessStartInfo
            {
                // FileName 為 python，可換成 python.exe 的絕對路徑
                FileName = "python",
                // 將 server IP 與 port 當作參數傳給 python
                Arguments = $"{pythonScriptPath} {ipAddress} {port} {playerName}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
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
            catch (Exception ex)
            {
                Console.WriteLine("無法啟動 Python 客戶端: " + ex.Message);
            }
        }
        private void sendMessage(Label label)
        {
            try
            {
                var componentData = new
                {
                    ComponentName = label.Name, // 元件名稱
                    TextContent = label.Text      // 元件文字內容
                };
                // 將資料序列化為JSON格式
                string jsonData = JsonConvert.SerializeObject(componentData);
                byte[] data = Encoding.UTF8.GetBytes(jsonData);

                // 發送廣播訊息
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, UDPPort);
                udpClient.Send(data, data.Length, endPoint);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"發送失敗: {ex.Message}");
            }
        }
        private void AcceptClients()
        {
            try
            {
                // 同時只等待兩位玩家
                server.Server.Listen(2);

                // 接受 Player A
                clientA = server.AcceptTcpClient();
                streamA = clientA.GetStream();
                // 先讀一次看是否是名稱
                playerAName = ReceiveName(streamA);
                if (string.IsNullOrEmpty(playerAName)) playerAName = "Player A";
                lblStatus.Text = $"{playerAName} 已連線";
                sendMessage(lblStatus);
                // 接受 Player B
                clientB = server.AcceptTcpClient();
                streamB = clientB.GetStream();
                playerBName = ReceiveName(streamB);
                if (string.IsNullOrEmpty(playerBName)) playerBName = "Player B";
                lblStatus.Text = $"{playerBName} 已連線";
                sendMessage(lblStatus);

                // 啟動兩個 Thread 持續接收手勢
                Thread threadA = new Thread(() => ReceiveGestureA());
                threadA.IsBackground = true;
                threadA.Start();

                Thread threadB = new Thread(() => ReceiveGestureB());
                threadB.IsBackground = true;
                threadB.Start();

                lblStatus.Text = $"{playerAName} 與 {playerBName} 皆已連線，請雙方先出石頭表示準備...";
                sendMessage(lblStatus);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AcceptClients Error: " + ex.Message);
                lblStatus.Text = "AcceptClients Error: " + ex.Message;
            }
        }
        private string ReceiveName(NetworkStream stream)
        {
            // 假設我們預期玩家名稱封包不會超過 512 bytes
            byte[] buffer = new byte[512];
            // Read() 會阻塞直到客戶端送來資料
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                // 看是否包含 "NAME:"
                if (msg.StartsWith("NAME:"))
                {
                    return msg.Substring("NAME:".Length).Trim();
                }
            }
            return null;
        }
        // ============== A 持續接收手勢 ==============
        private void ReceiveGestureA()
        {
            Console.WriteLine("ReceiveGestureA() started!");
            byte[] buffer = new byte[1024];
            while (isServerRunning)
            {
                try
                {
                    int bytesRead = streamA.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // 連線中斷

                    string gesture = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    gestureA = gesture;
                    if (gestureA.Contains("Rock")) gestureA = "Rock";
                    else if (gestureA.Contains("Paper")) gestureA = "Paper";
                    else if(gestureA.Contains("Scissors")) gestureA = "Scissors";
                    lblPlayerAGesture.Text = $"{playerAName}: " + gestureA;
                    sendMessage(lblPlayerAGesture);

                    // 如果 A 出石頭 => readyA = true；否則 false
                    readyA = (gesture == "Rock");

                    // 每次更新都檢查一下是否可以進入倒數
                    CheckReadyState();
                }
                catch
                {
                    lblStatus.Text = $"{playerAName} 連線中斷";
                    sendMessage(lblStatus);
                    break;
                }
            }
            lblStatus.Text = $"{playerAName} 連線中斷";
            sendMessage(lblStatus);
            lblPlayerAGesture.Text = $"{playerAName}: ";
            sendMessage(lblPlayerAGesture);
            Console.WriteLine("ReceiveGestureA() ended!");
        }

        // ============== B 持續接收手勢 ==============
        private void ReceiveGestureB()
        {
            byte[] buffer = new byte[1024];
            while (isServerRunning)
            {
                try
                {
                    int bytesRead = streamB.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // 連線中斷

                    string gesture = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    gestureB = gesture;
                    if (gestureB.Contains("Rock")) gestureB = "Rock";
                    else if (gestureB.Contains("Paper")) gestureB = "Paper";
                    else if (gestureB.Contains("Scissors")) gestureB = "Scissors";
                    lblPlayerBGesture.Text = $"{playerBName}: " + gestureB;
                    sendMessage(lblPlayerBGesture);

                    // 如果 B 出石頭 => readyB = true；否則 false
                    readyB = (gesture == "Rock");

                    // 每次更新都檢查一下是否可以進入倒數
                    CheckReadyState();
                }
                catch
                {
                    lblStatus.Text = $"{playerBName} 連線中斷";
                    sendMessage(lblStatus);
                    lblPlayerBGesture.Text = $"{playerBName}: ";
                    sendMessage(lblPlayerBGesture);
                    break;
                }
            }
        }

        // ============== 檢查雙方是否都準備好 ==============
        private void CheckReadyState()
        {
            // 雙方都出石頭且尚未進入倒數 => 開始倒數
            if (readyA && readyB && !isCountingDown)
            {
                isCountingDown = true;
                lblStatus.Text = "雙方都出石頭，3 秒後判定最終手勢...";
                sendMessage(lblStatus);
                StartCountdown();
            }
        }

        // ============== 執行 3 秒倒數的函式 ==============
        private void StartCountdown()
        {
            // 建立一個背景執行緒來處理倒數
            Thread countdownThread = new Thread(() =>
            {
                int countdown = 3;
                while (countdown > 0)
                {
                    lblCountdown.Text = $"倒數：{countdown} 秒";
                    sendMessage(lblCountdown);
                    Thread.Sleep(1000);
                    countdown--;
                }

                // 倒數結束，清空倒數顯示
                lblCountdown.Text = "";
                sendMessage(lblCountdown);

                // 取得倒數結束那一刻的手勢
                string finalA = gestureA;
                string finalB = gestureB;

                // 進行判斷
                DetermineWinner(finalA, finalB);

                // 重置
                readyA = false;
                readyB = false;
                isCountingDown = false;

                // 再次提示玩家：若要下一局，再次雙方出石頭
                lblStatus.Text = "若要進行下一局，請雙方再次出石頭表示準備...";
                sendMessage(lblStatus);
            });

            countdownThread.IsBackground = true;
            countdownThread.Start();
        }

        // ============== 比較結果 (玩家 A vs. 玩家 B) ==============
        private void DetermineWinner(string ga, string gb)
        {
            if (ga == gb)
            {
                lblResult.Text = $"結果：平手 ({ga} vs. {gb})";
                sendMessage(lblResult);
                return;
            }

            bool aWin = (ga.Contains("Rock") && gb.Contains("Scissors")) ||
                        (ga.Contains("Scissors") && gb.Contains("Paper")) ||
                        (ga.Contains("Paper") && gb.Contains("Rock"));
            bool bWin = (gb.Contains("Rock") && ga.Contains("Scissors")) ||
                        (gb.Contains("Scissors") && ga.Contains("Paper")) ||
                        (gb.Contains("Paper") && ga.Contains("Rock"));
            if (aWin)
            {
                scoreA++;
                lblResult.Text = $"結果：{playerAName} 勝利！({ga} vs. {gb})";

                SaveNameToDatabase(playerAName, playerBName, 1);
                sendMessage(lblResult);
            }
            else if (bWin)
            {
                scoreB++;
                lblResult.Text = $"結果：{playerBName} 勝利！({ga} vs. {gb})";
                SaveNameToDatabase(playerAName, playerBName, 2);
                sendMessage(lblResult);
            }
            else
            {
                lblResult.Text = $"結果：辨識錯誤";
                sendMessage(lblResult);
            }

            lblScore.Text = $"{playerAName}: {scoreA}  {playerBName}: {scoreB}";
            sendMessage(lblScore);
        }

        // ============== 停止伺服器 ==============
        private void btnStopServer_Click(object sender, EventArgs e)
        {
            StopServer();
        }

        private void StopServer()
        {
            isServerRunning = false;
            if (clientA != null) clientA.Close();
            if (clientB != null) clientB.Close();
            if (server != null) server.Stop();
            lblStatus.Text = "伺服器已停止";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            udpClient?.Close();
            listenThread?.Abort();
            StopServer();
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
    }
    
    //================================================
    // (3) 輔助函式: 顯示一個 Prompt 讓使用者輸入 IP
    //================================================
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 350,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, Width = 280 };
            TextBox inputBox = new TextBox() { Left = 20, Top = 45, Width = 280 };
            Button confirmation = new Button() { Text = "OK", Left = 220, Width = 80, Top = 75, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : "";
        }
    }
}
