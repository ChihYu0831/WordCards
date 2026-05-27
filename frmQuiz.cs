using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WordCards
{
    public class frmQuiz : Form
    {
        private List<WordItem> _words;       // 所有的單字
        private WordItem _currentQuestion;   // 目前的題目
        private Random _rand = new Random();
        private int _score = 0;              // 答對題數
        private int _totalQuestions = 0;     // 總作答題數

        // 畫面上的控制項
        private Label lblQuestion;
        private Button[] btnOptions;
        private Label lblScore;

        // 建構子，接收主畫面傳來的單字清單
        public frmQuiz(IEnumerable<WordItem> words)
        {
            _words = words.ToList();
            InitializeUI();
            NextQuestion();
        }

        // 以程式碼自動產生測驗畫面，不需要拉控制項
        private void InitializeUI()
        {
            this.Text = "單字測驗模式";
            this.Size = new Size(400, 460); // 稍微加高一點點，為了放提示文字
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // 題目的 Label (音標)
            lblQuestion = new Label()
            {
                AutoSize = false, 
                Font = new Font("微軟正黑體", 20, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 80
            };
            this.Controls.Add(lblQuestion);

            // 提示文字的 Label ("請選出對應的英文字")
            Label lblInstruction = new Label()
            {
                AutoSize = false,
                Text = "請選出對應的英文字",
                Font = new Font("微軟正黑體", 12, FontStyle.Regular),
                ForeColor = Color.Black, // 用黑色讓視覺重點保持在音標上
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 30
            };
            this.Controls.Add(lblInstruction);

            // 分數的 Label
            lblScore = new Label()
            {
                Font = new Font("微軟正黑體", 12),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,
                Height = 40,
                Text = "分數: 0 / 0"
            };
            this.Controls.Add(lblScore);

            // 產生 4 個選項按鈕
            btnOptions = new Button[4];
            int buttonY = 130; // 將按鈕的起始 Y 座標往下移，避開上方的 Label
            for (int i = 0; i < 4; i++)
            {
                btnOptions[i] = new Button()
                {
                    Font = new Font("微軟正黑體", 14),
                    Bounds = new Rectangle(50, buttonY + (i * 60), 280, 45),
                    Cursor = Cursors.Hand
                };
                btnOptions[i].Click += Option_Click; // 綁定點擊事件
                this.Controls.Add(btnOptions[i]);
            }
        }

        // 產生下一題
        private void NextQuestion()
        {
            if (_words.Count < 4)
            {
                MessageBox.Show("單字庫數量不足 4 個，無法進行選擇題測驗！", "提示");
                this.Close();
                return;
            }

            // 隨機抽選 4 個不同的單字
            var shuffledWords = _words.OrderBy(x => _rand.Next()).Take(4).ToList();

            // 隨機決定這 4 個單字中的哪一個是正確答案
            _currentQuestion = shuffledWords[_rand.Next(4)];
            
            // --- 修改這裡：加上 .Trim() 把前後多餘的空白濾掉 ---
            string phonogramText = _currentQuestion.Phonogram?.Trim();

            // 預防某些單字剛好沒有輸入音標
            if (string.IsNullOrWhiteSpace(phonogramText))
            {
                phonogramText = "(此單字無音標)";
            }

            lblQuestion.Text = phonogramText;
            // ------------------------

            // 將選項綁定到按鈕上 (顯示英文)
            for (int i = 0; i < 4; i++)
            {
                btnOptions[i].Text = shuffledWords[i].Word;
            }
        }

        // 按下選項按鈕時的判斷邏輯
        private void Option_Click(object sender, EventArgs e)
        {
            Button clickedBtn = sender as Button;
            _totalQuestions++;

            // 判斷按鈕上的英文是否等於正確答案的英文
            if (clickedBtn.Text == _currentQuestion.Word)
            {
                _score++;
                MessageBox.Show("答對了！", "測驗結果", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"答錯了！\n正確答案是: {_currentQuestion.Word}", "測驗結果", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            lblScore.Text = $"分數: {_score} / {_totalQuestions}";
            NextQuestion(); // 進入下一題
        }
    }
}
