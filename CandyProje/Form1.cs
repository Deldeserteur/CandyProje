using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;


namespace CandyProje
{
    public partial class Form1 : Form
    {
        private GameBoard gameBoard;

        public Form1()
        {
            InitializeComponent();
            InitializeGameBoard();

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            float fontSize = Math.Min(this.ClientSize.Width / 20, this.ClientSize.Height / 20); // Adjust divisor for scaling
            menu.Font = new Font(menu.Font.FontFamily, fontSize, menu.Font.Style);
        }

        private void btn_start_MouseHover(object sender, EventArgs e)
        {
            btn_start.Image = Properties.Resources.start_hover;
        }

        private void btn_option_MouseHover(object sender, EventArgs e)
        {
            btn_option.Image = Properties.Resources.option_hover;

        }

        private void btn_exit_MouseHover(object sender, EventArgs e)
        {
            btn_exit.Image = Properties.Resources.exit_hover;

        }

        private void btn_start_MouseLeave(object sender, EventArgs e)
        {
            btn_start.Image = Properties.Resources.start_normal;

        }

        private void btn_option_MouseLeave(object sender, EventArgs e)
        {
            btn_option.Image = Properties.Resources.option_normal;

        }

        private void btn_exit_MouseLeave(object sender, EventArgs e)
        {
            btn_exit.Image = Properties.Resources.exit_normal;

        }

        private void InitializeGameBoard()
        {
            // Initialize the game board with desired number of rows and columns
            gameBoard = new GameBoard(8, 8); // Example for an 8x8 grid
            gameBoard.Parent = this;

            // Set the properties to center the game board
            gameBoard.Anchor = AnchorStyles.None;
            this.Controls.Add(gameBoard);
            gameBoard.Hide(); // Start with the game board hidden

            //Add resize event to keep the game board centered on windows resize
            this.Resize += new EventHandler(Form1_Resize);
        }

        private void txtPlayerName_TextChanged(object sender, EventArgs e)
        {
            btn_start.Enabled = !string.IsNullOrWhiteSpace(txtPlayerName.Text);
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPlayerName.Text))
            {
                // Show a warning message
                MessageBox.Show("Please enter your name before starting the game!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

            }

            // Initialiser le jeu
            menu.Hide();

            if (gameBoard == null) // S'assurer que gameBoard est initialisé une seule fois 
            {

                InitializeGameBoard();
            }
            gameBoard.Show(); // Show the game board when the start button is clicked
            gameBoard.InitializeScoreAndTimer(); // Initialiser le timer et les labels


            // Ajouter les Labels au formulaire principal
            this.Controls.Add(gameBoard.TimerLabel);
            this.Controls.Add(gameBoard.ScoreLabel);

            CenterGameBoard();

        }

        private void CenterGameBoard()
        {
            if (gameBoard != null)
            {
                gameBoard.Left = (this.ClientSize.Width - gameBoard.Width) / 2;
                gameBoard.Top = (this.ClientSize.Height - gameBoard.Height) / 2;

            }
        }

        private void btn_option_Click(object sender, EventArgs e)
        {

        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }

    public class GameBoard : Panel
    {
        private Button[,] buttons;
        private int rows;
        private int cols;
        private Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Orange };
        private Random Random = new Random();
        private Button selectedButton = null;
        private Label timerLabel;
        private Label scoreLabel;
        private System.Windows.Forms.Timer timer;
        private int timeLeft;
        private int score;
        private const int MIN_GRID_SIZE = 4;
        private const int NAAX_GRID_SIZE = 12;
        private bool gameOver = false; // Flag pour indiquer si le jeu est terminé

        public Label TimerLabel { get; private set; }
        public Label ScoreLabel { get; private set; }

        public GameBoard(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            InitializeBoard();
            InitializeScoreAndTimer();
            DetectAndRemoveInitialMatches();
        }

        private void InitializeBoard()
        {
            this.buttons = new Button[rows, cols];
            this.Size = new Size(cols * 50, rows * 50); // set the size of the game board based on the number of rows and colums
            this.Dock = DockStyle.None;

            Random random = new Random();

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Button button = new Button();
                    button.Size = new Size(50, 50);
                    button.Location = new Point(col * 50, row * 50);
                    button.Click += OnButtonClick;
                    button.Tag = new Point(row, col); // Store the position in the Tag property

                    //Assign a Randomn color to the button
                    button.BackColor = colors[random.Next(colors.Length)];

                    this.buttons[row, col] = button;
                    this.Controls.Add(button);
                }
            }

            //delete auto match
            //DetectAndRemoveInitialMatches();
        }


        private void SwapButtons(Button button1, Button button2)
        {
            Color tempColor = button1.BackColor;
            button1.BackColor = button2.BackColor;
            button2.BackColor = tempColor;
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button.Text == "B" || button.Text == "R" || button.Text == "C" || button.Text == "CB")
            {
                Point pos = (Point)button.Tag;
                ActivateJoker(button, pos);
                DetectAndRemoveInitialMatches();
                return;
            }

            // Rest of the button-click logic for swapping and matching
            if (selectedButton == null)
            {
                // Sélectionner le bouton
                selectedButton = button;
                button.FlatAppearance.BorderColor = Color.Black;
                button.FlatAppearance.BorderSize = 2;
            }
            else
            {
                Point pos1 = (Point)selectedButton.Tag;
                Point pos2 = (Point)button.Tag;

                // Vérifier si les boutons sont adjacents
                if ((Math.Abs(pos1.X - pos2.X) == 1 && pos1.Y == pos2.Y) ||
                    (Math.Abs(pos1.Y - pos2.Y) == 1 && pos1.X == pos2.X))
                {
                    SwapButtons(selectedButton, button);

                    // Vérifier les correspondances après l'échange
                    if (GetMatchLength(pos1.X, pos1.Y, true) >= 3 || GetMatchLength(pos1.X, pos1.Y, false) >= 3 ||
                        GetMatchLength(pos2.X, pos2.Y, true) >= 3 || GetMatchLength(pos2.X, pos2.Y, false) >= 3)
                    {
                        RemoveMatches();
                    }
                    else
                    {
                        // Inverser l'échange s'il n'y a pas de correspondance
                        SwapButtons(selectedButton, button);
                    }
                }

                // Réinitialiser la sélection
                selectedButton.FlatAppearance.BorderSize = 0;
                selectedButton = null;
            }

        }

        private void DetectAndRemoveInitialMatches()
        {
            bool hasMatches = false;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (GetMatchLength(row, col, true) >= 3 || GetMatchLength(row, col, false) >= 3)
                    {
                        hasMatches = true;
                        break;
                    }
                }
                if (hasMatches)
                {
                    break;
                }
            }
            if (hasMatches)
            {
                RemoveMatches();
            }
        }

        private int GetMatchLength(int row, int col, bool horizontal)
        {
            Color color = buttons[row, col].BackColor;
            if (color == Color.Transparent) return 0;

            int matchLength = 1;

            if (horizontal)
            {
                for (int i = col + 1; i < cols && buttons[row, i].BackColor == color; i++)
                {
                    matchLength++;
                }
                for (int i = col - 1; i >= 0 && buttons[row, i].BackColor == color; i--)
                {
                    matchLength++;
                }
            }
            else
            {
                for (int i = row + 1; i < rows && buttons[i, col].BackColor == color; i++)
                {
                    matchLength++;
                }
                for (int i = row - 1; i >= 0 && buttons[i, col].BackColor == color; i--)
                {
                    matchLength++;
                }
            }

            return matchLength;
        }

        private async void RemoveMatches()
        {
            bool[,] toRemove = new bool[rows, cols];
            int matchCount = 0;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    // Skip already transparent tiles or jokers
                    if (buttons[row, col].BackColor == Color.Transparent ||
                        buttons[row, col].Text == "B" || buttons[row, col].Text == "R" || buttons[row, col].Text == "C")
                        continue;

                    int horizontalMatch = GetMatchLength(row, col, true);
                    int verticalMatch = GetMatchLength(row, col, false);

                    // Assign jokers based on match patterns
                    if (horizontalMatch >= 4 && toRemove[row, col] == false)
                    {
                        buttons[row, col].Text = "R"; // Row Joker
                        buttons[row, col].ForeColor = Color.White;
                        toRemove[row, col] = false; // Exclude joker from removal
                    }
                    else if (verticalMatch >= 4 && toRemove[row, col] == false)
                    {
                        buttons[row, col].Text = "C"; // Column Joker
                        buttons[row, col].ForeColor = Color.White;
                        toRemove[row, col] = false;
                    }
                    else if (horizontalMatch >= 5 || verticalMatch >= 5)
                    {
                        buttons[row, col].Text = "CB"; // Color Bomb
                        buttons[row, col].ForeColor = Color.White;
                        toRemove[row, col] = false;
                    }
                    else if (IsTShape(row, col) || IsLShape(row, col))
                    {
                        buttons[row, col].Text = "B"; // Bomb Joker
                        buttons[row, col].ForeColor = Color.White;
                        toRemove[row, col] = false;
                    }

                    // Mark matches for removal
                    if (horizontalMatch >= 3)
                    {
                        for (int i = 0; i < horizontalMatch && (col + i) < cols; i++)
                        {
                            toRemove[row, col + i] = true;
                        }
                    }
                    if (verticalMatch >= 3)
                    {
                        for (int i = 0; i < verticalMatch && (row + i) < rows; i++)
                        {
                            toRemove[row + i, col] = true;
                        }
                    }
                }
            }

            // Remove matches and update score
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (toRemove[row, col])
                    {
                        buttons[row, col].BackColor = Color.Transparent;
                        buttons[row, col].Text = "";
                        matchCount++;
                    }
                }
            }

            if (matchCount > 0)
            {
                score += matchCount * 10;
                ScoreLabel.Text = "Score: " + score;

                await Task.Delay(500); // Visual delay

                MakePiecesFall();
                DetectAndRemoveInitialMatches(); // Detect further matches after falling pieces
            }
        }

        private void CreateJoker(Button button, string jokerType)
        {
            switch (jokerType)
            {
                case "Bomb":
                    button.Text = "B";
                    button.ForeColor = Color.White;
                    button.Image = Image.FromFile(@"C: \Users\damie\source\repos\CandyProje\CandyProje\Resources\Bomb.png");
                    break;
                case "Row":
                    button.Text = "R";
                    button.ForeColor = Color.White;
                    button.Image = Image.FromFile(@"C:\Users\damie\source\repos\CandyProje\CandyProje\Resources\RowJoker.png");
                    break;
                case "Column":
                    button.Text = "C";
                    button.ForeColor = Color.White;
                    button.Image = Image.FromFile(@"C:\Users\damie\source\repos\CandyProje\CandyProje\Resources\ColumnJoker.png");
                    break;
                case "Column":
                    button.Text = "C";
                    button.ForeColor = Color.White;
                    button.Image = Image.FromFile(@"C:\Users\damie\source\repos\CandyProje\CandyProje\Resources\ColumnJoker.png");
                    break;
                case "ColorBomb":
                    button.Text = "CB";
                    button.ForeColor = Color.White;
                    button.Image = Image.FromFile(@"C:\Users\damie\source\repos\CandyProje\CandyProje\Resources\ColorBomb.png");
                    break;
            }

            button.BackgroundImageLayout = ImageLayout.Stretch; // Scale the image to fit the button
        }

        private bool IsTShape(int row, int col)
        {
            Color color = buttons[row, col].BackColor;

            return (row > 0 && row < rows - 1 && col > 0 && col < cols - 1 &&
                    buttons[row - 1, col].BackColor == color &&
                    buttons[row + 1, col].BackColor == color &&
                    ((buttons[row, col - 1].BackColor == color && buttons[row, col + 1].BackColor == color) ||
                    (buttons[row - 1, col].BackColor == color && buttons[row + 1, col].BackColor == color)));
        }

        private bool IsLShape(int row, int col)
        {
            Color color = buttons[row, col].BackColor;

            return (row > 0 && col > 0 &&
                    buttons[row - 1, col].BackColor == color &&
                    buttons[row, col - 1].BackColor == color &&
                    ((row < rows - 1 && buttons[row + 1, col].BackColor == color) ||
                    (col < cols - 1 && buttons[row, col + 1].BackColor == color)));
        }

        private void ActivateJoker(Button joker, Point position)
        {
            string jokerType = joker.Text;

            if (jokerType == "B")
            {
                // Bomb Joker: Clear a 3x3 area around it
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        int newRow = position.X + i;
                        int newCol = position.Y + j;

                        if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                        {
                            buttons[newRow, newCol].BackColor = Color.Transparent;
                            buttons[newRow, newCol].Text = "";
                        }
                    }
                }
            }
            else if (jokerType == "R")
            {
                // Row Joker: Clear the entire row
                for (int col = 0; col < cols; col++)
                {
                    buttons[position.X, col].BackColor = Color.Transparent;
                    buttons[position.X, col].Text = "";
                }
            }
            else if (jokerType == "C")
            {
                // Column Joker: Clear the entire column
                for (int row = 0; row < rows; row++)
                {
                    buttons[row, position.Y].BackColor = Color.Transparent;
                    buttons[row, position.Y].Text = "";
                }
            }
            else if (jokerType == "CB")
            {
                // Color Bomb: Clear all candies of the same color
                Color targetColor = joker.BackColor;
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        if (buttons[row, col].BackColor == targetColor)
                        {
                            buttons[row, col].BackColor = Color.Transparent;
                            buttons[row, col].Text = "";
                        }
                    }
                }
            }

            // Reset the joker itself
            joker.BackColor = Color.Transparent;
            joker.Text = "";
        }

        public void InitializeScoreAndTimer()
        {
            //Label for time
            TimerLabel = new Label
            {
                Text = "Time Remaining: 60",
                Location = new Point(10, 10),
                Font = new Font("Arial", 14, FontStyle.Bold),
                AutoSize = true
            };
            this.Controls.Add(TimerLabel);

            //label for score
            ScoreLabel = new Label
            {
                Text = "Score: 0",
                Location = new Point(200, 10),
                Font = new Font("Arial", 14, FontStyle.Bold),
                AutoSize = true
            };
            this.Controls.Add(ScoreLabel);

            timer = new System.Windows.Forms.Timer
            {
                Interval = 2000 // 2 seconds
            };
            timer.Tick += Timer_Tick;
            timeLeft = 60; // Initial time
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (TimerLabel != null) // Vérifie si timerLabel a bien été initialisé
            {
                if (timeLeft > 0)
                {
                    timeLeft--;
                    TimerLabel.Text = "Time: " + timeLeft;
                }
                else if (!gameOver)
                {
                    gameOver = true;
                    timer.Stop();
                    timer.Enabled = false; // Désactiver définitivement le timer
                    MessageBox.Show("Time End your score  : " + score);
                }
            }
        }

        private void MakePiecesFall()
        {
            for (int col = 0; col < cols; col++)
            {
                int emptyRow = rows - 1; // Position where the next piece must "fall"

                for (int row = rows - 1; row >= 0; row--)
                {
                    if (buttons[row, col].BackColor != Color.Transparent &&
                        buttons[row, col].Text != "B" && buttons[row, col].Text != "R" && buttons[row, col].Text != "C")
                    {
                        // Move non-joker pieces to the first empty row
                        buttons[emptyRow, col].BackColor = buttons[row, col].BackColor;
                        buttons[emptyRow, col].Text = buttons[row, col].Text;

                        if (emptyRow != row)
                        {
                            buttons[row, col].BackColor = Color.Transparent;
                            buttons[row, col].Text = "";
                        }

                        emptyRow--;
                    }
                }

                // Fill empty spaces with new colors
                for (int row = emptyRow; row >= 0; row--)
                {
                    buttons[row, col].BackColor = colors[Random.Next(colors.Length)];
                    buttons[row, col].Text = ""; // Ensure no joker text is assigned
                }
            }
        }

        private void FillGrid()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (buttons[row, col].BackColor == Color.Transparent)
                    {
                        buttons[col, row].BackColor = colors[Random.Next(colors.Length)];
                    }
                }
            }
        }
    }
}
