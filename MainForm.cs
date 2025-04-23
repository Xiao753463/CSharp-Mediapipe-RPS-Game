using System;
using System.Drawing;
using System.Windows.Forms;

namespace C__mediapipe_winform
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            CustomizeButtons();
            ApplyButtonAnimations();
 
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Opacity = 0; // 初始透明度設置為 0（完全透明）
            ArrangeButtons(); // 自動排列按鈕位置
            StartFadeInAnimation(); // 開始淡入動畫
        }

        private void CustomizeButtons()
        {
            Button[] buttons = { btnSinglePlayer, btnMultiPlayer, btnLeaderboard };

            foreach (var button in buttons)
            {
                button.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 2;
                button.FlatAppearance.BorderColor = Color.Gray;
                button.Cursor = Cursors.Hand;
            }
        }

        private void StartFadeInAnimation()
        {
            Timer fadeInTimer = new Timer();
            fadeInTimer.Interval = 30;
            fadeInTimer.Tick += (s, ev) =>
            {
                if (this.Opacity < 1)
                {
                    this.Opacity += 0.05;
                }
                else
                {
                    fadeInTimer.Stop();
                }
            };
            fadeInTimer.Start();
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

        private void ApplyButtonAnimations()
        {
            AddButtonClickAnimation(btnSinglePlayer);
            AddButtonClickAnimation(btnMultiPlayer);
            AddButtonClickAnimation(btnLeaderboard);
        }

        private void ArrangeButtons()
        {
            int spacing = 20;
            int totalHeight = (btnSinglePlayer.Height + spacing) * 3;
            int startY = (this.ClientSize.Height - totalHeight) / 2;

            btnSinglePlayer.Location = new Point((this.ClientSize.Width - btnSinglePlayer.Width) / 2, startY);
            btnMultiPlayer.Location = new Point((this.ClientSize.Width - btnMultiPlayer.Width) / 2, startY + btnSinglePlayer.Height + spacing);
            btnLeaderboard.Location = new Point((this.ClientSize.Width - btnLeaderboard.Width) / 2, startY + (btnSinglePlayer.Height + spacing) * 2);
        }

        private void btnSinglePlayer_Click(object sender, EventArgs e)
        {
            var spForm = new SinglePlayerForm();
            spForm.Show();
        }

        private void btnMultiPlayer_Click(object sender, EventArgs e)
        {
            var mpForm = new MultiPlayerForm();
            mpForm.Show();
        }

        private void btnLeaderboard_Click(object sender, EventArgs e)
        {
            new RecordForm().ShowDialog();
        }
    }
}
