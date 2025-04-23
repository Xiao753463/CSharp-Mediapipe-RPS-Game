namespace C__mediapipe_winform
{
    partial class MultiPlayerForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblPlayerAGesture = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.lblPlayerBGesture = new System.Windows.Forms.Label();
            this.lblScore = new System.Windows.Forms.Label();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.btnStopServer = new System.Windows.Forms.Button();
            this.lblCountdown = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(194, 147);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(109, 15);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "等待開始遊戲...";
            // 
            // lblPlayerAGesture
            // 
            this.lblPlayerAGesture.AutoSize = true;
            this.lblPlayerAGesture.Location = new System.Drawing.Point(168, 257);
            this.lblPlayerAGesture.Name = "lblPlayerAGesture";
            this.lblPlayerAGesture.Size = new System.Drawing.Size(124, 15);
            this.lblPlayerAGesture.TabIndex = 1;
            this.lblPlayerAGesture.Text = "等待玩家連線中...";
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(476, 257);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(52, 15);
            this.lblResult.TabIndex = 2;
            this.lblResult.Text = "結果：";
            // 
            // lblPlayerBGesture
            // 
            this.lblPlayerBGesture.AutoSize = true;
            this.lblPlayerBGesture.Location = new System.Drawing.Point(168, 324);
            this.lblPlayerBGesture.Name = "lblPlayerBGesture";
            this.lblPlayerBGesture.Size = new System.Drawing.Size(124, 15);
            this.lblPlayerBGesture.TabIndex = 3;
            this.lblPlayerBGesture.Text = "等待玩家連線中...";
            // 
            // lblScore
            // 
            this.lblScore.AutoSize = true;
            this.lblScore.Location = new System.Drawing.Point(478, 324);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(50, 15);
            this.lblScore.TabIndex = 4;
            this.lblScore.Text = "比分 - ";
            // 
            // btnStartServer
            // 
            this.btnStartServer.Location = new System.Drawing.Point(63, 472);
            this.btnStartServer.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(397, 49);
            this.btnStartServer.TabIndex = 8;
            this.btnStartServer.Text = "啟動伺服器\n";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // btnStopServer
            // 
            this.btnStopServer.Location = new System.Drawing.Point(466, 472);
            this.btnStopServer.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnStopServer.Name = "btnStopServer";
            this.btnStopServer.Size = new System.Drawing.Size(347, 49);
            this.btnStopServer.TabIndex = 9;
            this.btnStopServer.Text = "連接伺服器";
            this.btnStopServer.UseVisualStyleBackColor = true;
            this.btnStopServer.Click += new System.EventHandler(this.btnConnectServer_Click);
            // 
            // lblCountdown
            // 
            this.lblCountdown.AutoSize = true;
            this.lblCountdown.Location = new System.Drawing.Point(176, 189);
            this.lblCountdown.Name = "lblCountdown";
            this.lblCountdown.Size = new System.Drawing.Size(0, 15);
            this.lblCountdown.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(55, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(197, 38);
            this.label1.TabIndex = 11;
            this.label1.Text = "雙人連線對戰";
            // 
            // MultiPlayerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1271, 629);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblCountdown);
            this.Controls.Add(this.btnStopServer);
            this.Controls.Add(this.btnStartServer);
            this.Controls.Add(this.lblScore);
            this.Controls.Add(this.lblPlayerBGesture);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.lblPlayerAGesture);
            this.Controls.Add(this.lblStatus);
            this.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.Name = "MultiPlayerForm";
            this.Text = "MultiPlayerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblPlayerAGesture;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Label lblPlayerBGesture;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.Button btnStopServer;
        private System.Windows.Forms.Label lblCountdown;
        private System.Windows.Forms.Label label1;
    }
}

