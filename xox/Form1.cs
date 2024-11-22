using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace xox
{
    public partial class Form1 : Form
    {
        private string currentPlayer = "X";
        private bool gameEnded = false;
        private Label labelStatus;
        private Button[,] buttons;
        private Button resetButton;
        private Button playWithFriendButton;
        private Button playWithComputerButton;
        private Button mainMenuButton;
        private ComboBox gridSizeSelector;
        private ComboBox difficultySelector;
        private bool playWithComputer = false;
        private string computerDifficulty = "Orta";
        private bool dontAskAgain = false;
        private string playerXName = "X";
        private string playerOName = "O";

        public Form1()
        {
            InitializeComponent();
            InitializeMainMenu();
        }

        private void InitializeMainMenu()
        {
            this.Size = new Size(600, 500);
            this.Text = "XOX Oyunu";
            this.BackColor = Color.FromArgb(240, 240, 240);

            Label titleLabel = new Label
            {
                Text = "XOX Oyunu",
                Location = new Point(225, 20),
                Size = new Size(150, 30),
                Font = new Font(FontFamily.GenericSansSerif, 18, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            this.Controls.Add(titleLabel);

            playWithFriendButton = new Button
            {
                Text = "Arkadaşla Oyna",
                Location = new Point(100, 80),
                Size = new Size(120, 50),
                BackColor = Color.LightSkyBlue,
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold)
            };
            playWithFriendButton.Click += PlayWithFriendButton_Click;
            this.Controls.Add(playWithFriendButton);

            playWithComputerButton = new Button
            {
                Text = "Bilgisayarla Oyna",
                Location = new Point(300, 80),
                Size = new Size(120, 50),
                BackColor = Color.LightCoral,
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold)
            };
            playWithComputerButton.Click += PlayWithComputerButton_Click;
            this.Controls.Add(playWithComputerButton);

            Label gridSizeLabel = new Label
            {
                Text = "Izgara Boyutu:",
                Location = new Point(100, 160),
                Size = new Size(100, 30),
                Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            this.Controls.Add(gridSizeLabel);

            gridSizeSelector = new ComboBox
            {
                Location = new Point(220, 160),
                Size = new Size(150, 30),
                Font = new Font(FontFamily.GenericSansSerif, 12)
            };
            gridSizeSelector.Items.AddRange(new object[] { "3x3", "4x4", "5x5", "6x6", "7x7", "8x8" });
            gridSizeSelector.SelectedIndex = 0;
            this.Controls.Add(gridSizeSelector);

            Label difficultyLabel = new Label
            {
                Text = "Zorluk Seviyesi:",
                Location = new Point(100, 220),
                Size = new Size(150, 30),
                Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            this.Controls.Add(difficultyLabel);

            difficultySelector = new ComboBox
            {
                Location = new Point(250, 220),
                Size = new Size(150, 30),
                Font = new Font(FontFamily.GenericSansSerif, 12)
            };
            difficultySelector.Items.AddRange(new object[] { "Kolay", "Orta", "Zor" });
            difficultySelector.SelectedIndex = 1;
            this.Controls.Add(difficultySelector);
        }

        private void InitializeGameBoard(int gridSize)
        {
            this.Controls.Clear();
            buttons = new Button[gridSize, gridSize];
            this.Size = new Size(100 + gridSize * 80, 300 + gridSize * 80);

            labelStatus = new Label
            {
                Text = playWithComputer ? "Senin Sıran" : "Sıradaki Oyuncu: " + playerXName,
                Location = new Point(10, 20 + gridSize * 80),
                Size = new Size(400, 30),
                Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            this.Controls.Add(labelStatus);

            int buttonSize = 80;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    buttons[i, j] = new Button
                    {
                        Location = new Point(10 + j * buttonSize, 10 + i * buttonSize),
                        Size = new Size(buttonSize, buttonSize),
                        Font = new Font(FontFamily.GenericSansSerif, 24, FontStyle.Bold),
                        BackColor = Color.WhiteSmoke,
                        FlatStyle = FlatStyle.Flat
                    };
                    buttons[i, j].Click += button_Click;
                    this.Controls.Add(buttons[i, j]);
                }
            }

            resetButton = new Button
            {
                Text = "Oyunu Sıfırla",
                Location = new Point(10, 60 + gridSize * 80),
                Size = new Size(120, 40),
                BackColor = Color.LightGoldenrodYellow,
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold)
            };
            resetButton.Click += resetButton_Click;
            this.Controls.Add(resetButton);

            mainMenuButton = new Button
            {
                Text = "Ana Menüye Dön",
                Location = new Point(150, 60 + gridSize * 80),
                Size = new Size(150, 40),
                BackColor = Color.LightPink,
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold)
            };
            mainMenuButton.Click += MainMenuButton_Click;
            this.Controls.Add(mainMenuButton);
        }

        private void PlayWithFriendButton_Click(object sender, EventArgs e)
        {
            GetPlayerNames();
            StartGame(false);
        }

        private void GetPlayerNames()
        {
            playerXName = Prompt.ShowDialog("X Oyuncusunun Adını Girin:", "Oyuncu X");
            playerOName = Prompt.ShowDialog("O Oyuncusunun Adını Girin:", "Oyuncu O");
        }

        private void PlayWithComputerButton_Click(object sender, EventArgs e)
        {
            StartGame(true);
        }

        private void StartGame(bool playWithComputer)
        {
            this.playWithComputer = playWithComputer;
            computerDifficulty = difficultySelector.SelectedItem.ToString();
            gameEnded = false;
            currentPlayer = "X";
            int gridSize = gridSizeSelector.SelectedIndex + 3;
            InitializeGameBoard(gridSize);
            labelStatus.Text = playWithComputer ? "Senin Sıran" : "Sıradaki Oyuncu: " + playerXName;
        }

        private void button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (button.Text != "" || gameEnded)
            {
                return;
            }

            button.Text = currentPlayer;
            CheckForWinner();

            if (!gameEnded)
            {
                currentPlayer = (currentPlayer == "X") ? "O" : "X";
                labelStatus.Text = playWithComputer && currentPlayer == "X" ? "Senin Sıran" : playWithComputer ? "Bilgisayarın Sırası" : "Sıradaki Oyuncu: " + (currentPlayer == "X" ? playerXName : playerOName);

                if (playWithComputer && currentPlayer == "O")
                {
                    Task.Delay(500).Wait();
                    ComputerMove();
                    CheckForWinner();
                    if (!gameEnded)
                    {
                        currentPlayer = "X";
                        labelStatus.Text = "Senin Sıran";
                    }
                }
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            if (playWithComputer && !dontAskAgain)
            {
                DialogResult result = AskDifficultyChange();
                if (result == DialogResult.Yes)
                {
                    ChangeDifficulty();
                }
                else if (result == DialogResult.No)
                {
                    dontAskAgain = true;
                }
            }
            StartGame(playWithComputer);
        }

        private DialogResult AskDifficultyChange()
        {
            return MessageBox.Show("Bilgisayar zorluğunu değiştirmek ister misiniz?", "Zorluk Değiştir", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        }

        private void ChangeDifficulty()
        {
            computerDifficulty = Prompt.ShowDialog("Yeni zorluk seviyesini girin (Kolay, Orta, Zor):", "Zorluk Değiştir");
            if (!new[] { "Kolay", "Orta", "Zor" }.Contains(computerDifficulty))
            {
                computerDifficulty = "Orta";
            }
        }

        private void MainMenuButton_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            InitializeMainMenu();
        }

        private void ComputerMove()
        {
            switch (computerDifficulty)
            {
                case "Kolay":
                    EasyComputerMove();
                    break;
                case "Orta":
                    MediumComputerMove();
                    break;
                case "Zor":
                    HardComputerMove();
                    break;
            }
        }

        private void EasyComputerMove()
        {
            foreach (var button in buttons.Cast<Button>().Where(b => b.Text == ""))
            {
                button.Text = "O";
                return;
            }
        }

        private void MediumComputerMove()
        {
            int gridSize = buttons.GetLength(0);
            int bestScore = int.MinValue;
            Button bestMove = null;

            foreach (var button in buttons.Cast<Button>().Where(b => b.Text == ""))
            {
                button.Text = "O";
                int score = Minimax(buttons, 0, false);
                button.Text = "";

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = button;
                }
            }

            if (bestMove != null)
            {
                bestMove.Text = "O";
            }
        }

        private void HardComputerMove()
        {
            int gridSize = buttons.GetLength(0);
            int bestScore = int.MinValue;
            Button bestMove = null;

            foreach (var button in buttons.Cast<Button>().Where(b => b.Text == ""))
            {
                button.Text = "O";
                int score = Minimax(buttons, 0, false);
                button.Text = "";

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = button;
                }
            }

            if (bestMove != null)
            {
                bestMove.Text = "O";
            }
        }

        private int Minimax(Button[,] board, int depth, bool isMaximizing)
        {
            string result = EvaluateBoard();
            if (result == "O") return 10 - depth;
            if (result == "X") return depth - 10;
            if (result == "Draw") return 0;

            int gridSize = board.GetLength(0);
            int bestScore = isMaximizing ? int.MinValue : int.MaxValue;

            foreach (var button in board.Cast<Button>().Where(b => b.Text == ""))
            {
                button.Text = isMaximizing ? "O" : "X";
                int score = Minimax(board, depth + 1, !isMaximizing);
                button.Text = "";

                bestScore = isMaximizing ? Math.Max(score, bestScore) : Math.Min(score, bestScore);
            }

            return bestScore;
        }

        private string EvaluateBoard()
        {
            int gridSize = buttons.GetLength(0);

            for (int i = 0; i < gridSize; i++)
            {
                if (buttons[i, 0].Text != "" && Enumerable.Range(0, gridSize).All(j => buttons[i, j].Text == buttons[i, 0].Text))
                    return buttons[i, 0].Text;

                if (buttons[0, i].Text != "" && Enumerable.Range(0, gridSize).All(j => buttons[j, i].Text == buttons[0, i].Text))
                    return buttons[0, i].Text;
            }

            if (buttons[0, 0].Text != "" && Enumerable.Range(0, gridSize).All(i => buttons[i, i].Text == buttons[0, 0].Text))
                return buttons[0, 0].Text;

            if (buttons[0, gridSize - 1].Text != "" && Enumerable.Range(0, gridSize).All(i => buttons[i, gridSize - i - 1].Text == buttons[0, gridSize - 1].Text))
                return buttons[0, gridSize - 1].Text;

            if (buttons.Cast<Button>().All(b => b.Text != ""))
                return "Draw";

            return null;
        }

        private void CheckForWinner()
        {
            int gridSize = buttons.GetLength(0);

            for (int i = 0; i < gridSize; i++)
            {
                if (buttons[i, 0].Text != "" && Enumerable.Range(0, gridSize).All(j => buttons[i, j].Text == buttons[i, 0].Text))
                {
                    HighlightWinner(i, true);
                    return;
                }

                if (buttons[0, i].Text != "" && Enumerable.Range(0, gridSize).All(j => buttons[j, i].Text == buttons[0, i].Text))
                {
                    HighlightWinner(i, false);
                    return;
                }
            }

            if (buttons[0, 0].Text != "" && Enumerable.Range(0, gridSize).All(i => buttons[i, i].Text == buttons[0, 0].Text))
            {
                HighlightDiagonalWinner(true);
                return;
            }

            if (buttons[0, gridSize - 1].Text != "" && Enumerable.Range(0, gridSize).All(i => buttons[i, gridSize - i - 1].Text == buttons[0, gridSize - 1].Text))
            {
                HighlightDiagonalWinner(false);
                return;
            }

            if (buttons.Cast<Button>().All(b => b.Text != ""))
            {
                gameEnded = true;
                labelStatus.Text = "Berabere!";
            }
        }

        private void HighlightWinner(int index, bool isRow)
        {
            gameEnded = true;
            for (int i = 0; i < buttons.GetLength(0); i++)
            {
                if (isRow)
                {
                    buttons[index, i].BackColor = Color.LightGreen;
                }
                else
                {
                    buttons[i, index].BackColor = Color.LightGreen;
                }
            }
            labelStatus.Text = "Kazanan: " + (isRow ? buttons[index, 0].Text : buttons[0, index].Text);
        }

        private void HighlightDiagonalWinner(bool isLeftToRight)
        {
            gameEnded = true;
            for (int i = 0; i < buttons.GetLength(0); i++)
            {
                if (isLeftToRight)
                {
                    buttons[i, i].BackColor = Color.LightGreen;
                }
                else
                {
                    buttons[i, buttons.GetLength(0) - i - 1].BackColor = Color.LightGreen;
                }
            }
            labelStatus.Text = "Kazanan: " + (isLeftToRight ? buttons[0, 0].Text : buttons[0, buttons.GetLength(0) - 1].Text);
        }
    }

    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text, Width = 300 };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 300 };
            Button confirmation = new Button() { Text = "OK", Left = 150, Width = 100, Top = 100, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
