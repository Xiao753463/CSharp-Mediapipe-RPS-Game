namespace C__mediapipe_winform
{
    partial class SinglePlayerForm
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
            this.lblPlayerGesture = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.lblComputerGesture = new System.Windows.Forms.Label();
            this.lblScore = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(278, 154);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(128, 18);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "等待開始遊戲...";
            // 
            // lblPlayerGesture
            // 
            this.lblPlayerGesture.AutoSize = true;
            this.lblPlayerGesture.Location = new System.Drawing.Point(249, 284);
            this.lblPlayerGesture.Name = "lblPlayerGesture";
            this.lblPlayerGesture.Size = new System.Drawing.Size(85, 18);
            this.lblPlayerGesture.TabIndex = 1;
            this.lblPlayerGesture.Text = "玩家手勢:";
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(506, 274);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(22, 18);
            this.lblResult.TabIndex = 2;
            this.lblResult.Text = "\"\"";
            // 
            // lblComputerGesture
            // 
            this.lblComputerGesture.AutoSize = true;
            this.lblComputerGesture.Location = new System.Drawing.Point(249, 367);
            this.lblComputerGesture.Name = "lblComputerGesture";
            this.lblComputerGesture.Size = new System.Drawing.Size(85, 18);
            this.lblComputerGesture.TabIndex = 3;
            this.lblComputerGesture.Text = "電腦手勢:";
            // 
            // lblScore
            // 
            this.lblScore.AutoSize = true;
            this.lblScore.Location = new System.Drawing.Point(596, 367);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(173, 18);
            this.lblScore.TabIndex = 4;
            this.lblScore.Text = "比分 - 玩家: 0 電腦: 0";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(130, 440);
            this.btnStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(150, 59);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "開始遊戲";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(299, 440);
            this.btnPlay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(150, 59);
            this.btnPlay.TabIndex = 6;
            this.btnPlay.Text = "出拳";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // SinglePlayerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1158, 754);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lblScore);
            this.Controls.Add(this.lblComputerGesture);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.lblPlayerGesture);
            this.Controls.Add(this.lblStatus);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SinglePlayerForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblPlayerGesture;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Label lblComputerGesture;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnPlay;
    }
}

