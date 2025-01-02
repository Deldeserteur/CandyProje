//Adý : ASSELEM DAMIEN
//Soyadý : ADJALTE
//Oðrenci Numara : B221200580
//Proje adý : Match3 Game

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
using System.Text.Json;
using Microsoft.VisualBasic;


namespace CandyProje
{
    public class PlayerScore
    {
        private string playerName;

        public string PlayerName
        {
            get { return playerName ?? string.Empty; }
            set { playerName = value ?? string.Empty; }
        }
        public int Score { get; set; }
        public DateTime PlayDate { get; set; }

        public PlayerScore(string playerName, int score)
        {
            if (string.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("Player name cannot be null or empty", nameof(playerName));
            }

            this.PlayerName = playerName;
            this.Score = score;
            this.PlayDate = DateTime.Now;
        }
    }

    public partial class Form1 : Form
    {
        private GameBoard gameBoard;
        public string playerName;
        private Form scoreForm;
        private readonly string scorePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
       "CandyProje",
       "highscores.txt"
        );

        public Form1()
        {
            InitializeComponent();
            InitializeGameBoard();

            // Enable keyboard input
            this.KeyPreview = true;
          

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

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P && gameBoard != null && gameBoard.Visible)
            {
                gameBoard.TogglePause();
                e.Handled = true; // Prevent the key from being handled elsewhere
            }
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
            if (string.IsNullOrWhiteSpace(playerName))
            {
                Console.WriteLine("Warning: InitializeGameBoard called with empty player name"); // Debug line
                return;
            }

            // Initialize the game board with desired number of rows and columns
            gameBoard = new GameBoard(8, 8, playerName); // Example for an 8x8 grid
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
            playerName = txtPlayerName.Text.Trim(); // save the player name
            Console.WriteLine($"Starting game with player name: {playerName}"); // Debug line


            // Initialiser le jeu
            menu.Hide();

            if (gameBoard == null) // S'assurer que gameBoard est initialisé une seule fois 
            {
                InitializeGameBoard();
            }

            gameBoard.Show(); // Show the game board when the start button is clicked
            gameBoard.InitializeScoreAndTimer(playerName); // Initialiser le timer et les labels

            // Ajouter les Labels au formulaire principal
            this.Controls.Add(gameBoard.TimerLabel);
            this.Controls.Add(gameBoard.ScoreLabel);
            this.Controls.Add(gameBoard.PlayerNameLabel);
            CenterGameBoard();

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.P && gameBoard != null && gameBoard.Visible)
            {
                gameBoard.TogglePause();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
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
            ShowHighScores();

        }

        private void ShowHighScores()
        {
            playerName = txtPlayerName.Text; // save the player name

            var scoreForm = new Form
            {
                Text = "High Scores",
                Size = new Size(400, 500),
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            var listView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Dock = DockStyle.Fill
            };

            // Add columns
            listView.Columns.Add("Rank", 50);
            listView.Columns.Add("Player", 120);
            listView.Columns.Add("Score", 80);
            listView.Columns.Add("Date", 130);

            // Load and display scores
            var scores = LoadHighScores();

            if (scores.Count == 0)
            {
                listView.Items.Add(new ListViewItem(new[] { "-", "No scores yet", "-", "-" }));
            }
            else
            {
                int rank = 1;
                foreach (var score in scores)
                {
                        var item = new ListViewItem(new[]
                        {
                    rank.ToString(),
                    score.PlayerName,
                    score.Score.ToString("N0"),
                    score.PlayDate.ToString("g")
                 });
                     listView.Items.Add(item);
                    rank++;
                }
            }

            scoreForm.Controls.Add(listView);

            Button closeButton = new Button
            {
                Text = "Close",
                Dock = DockStyle.Bottom,
                Height = 30
            };
            closeButton.Click += (s, e) => scoreForm.Close();

            scoreForm.Controls.Add(closeButton);
            scoreForm.ShowDialog();
        }
   
        private List<PlayerScore> LoadHighScores()
        {
            List<PlayerScore> scores = new List<PlayerScore>();

            try
            {
                // Make sure we're using the correct path
                string scorePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "CandyProje",
                    "highscores.txt"
                );

                if (File.Exists(scorePath))
                {
                    string[] lines = File.ReadAllLines(scorePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 3 && !string.IsNullOrWhiteSpace(parts[0]))
                        {
                            try
                            {
                                var score = new PlayerScore(
                                    parts[0].Trim(), // PlayerName - add Trim() to remove any whitespace
                                    int.Parse(parts[1].Trim()) // Score
                                )
                                {
                                    PlayDate = DateTime.Parse(parts[2].Trim()) // Date
                                };
                                scores.Add(score);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error parsing score line: {line}. Error: {ex.Message}");
                                continue;
                            }
                        }
                    }

                    // Sort scores by score value in descending order
                    scores = scores.OrderByDescending(s => s.Score).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading scores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<PlayerScore>();
            }

            return scores;
        }
       
        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }

    public class GameBoard : Panel
    {
        private string playerName;
        private Button[,] buttons;
        private Random Random = new Random();
        private int rows;
        private int cols;
        private Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Orange };
        private Button selectedButton = null;
        private bool isPaused = false; // Flag to track if the game is paused
        private Label timerLabel;
        private Label scoreLabel;
        private Label playerNameLabel;
        private List<PlayerScore> highScores;
        private Point? selectedPosition = null; // fields for keyboard selection
        private Point? targetPosition = null;

        private readonly string scorePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "CandyProje",
        "highscores.txt"
        );

        private System.Windows.Forms.Timer timer;
        private int timeLeft;
        private int score;
        private Form1 Form;
        private bool gameOver = false; // Flag pour indiquer si le jeu est terminé

        private Panel pauseOverlay;
        public Label TimerLabel { get; private set; }
        public Label ScoreLabel { get; private set; }
        public Label PlayerNameLabel { get; private set; }

        // Property to check pause state
        public bool IsPaused => isPaused;

        //contructor
        public GameBoard(int rows, int cols, string playerName )
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                throw new ArgumentException("Player name cannot be empty", nameof(playerName));
            }

            this.rows = rows;
            this.cols = cols;
            this.playerName = playerName; // store the player name

            Console.WriteLine($"GameBoard initialized with player name: {this.playerName}"); // Debug line

            InitializeBoard();
            InitializeScoreAndTimer(playerName);
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

            if (Parent is Form form)
            {
                form.KeyPreview = true;
                form.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.P)
                    {
                        TogglePause();
                        e.Handled = true;
                    }
                };
            }

            ////handling to enable keyboard input
            this.Focus();
            this.Select();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (Parent is Form form)
            {
                form.KeyPreview = true;
                form.KeyDown += Parent_KeyDown;
                form.KeyDown += HandleKeyDown;

            }
        }

        private void Parent_KeyDown(object sender, KeyEventArgs e)
        {
            // Pause handling
            if (e.KeyCode == Keys.P)
            {
                TogglePause();
                e.Handled = true;
                return;
            }

            if (isPaused || gameOver) return;

            // Movement handling
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    HandleArrowKey(e.KeyCode);
                    e.Handled = true;
                    break;
                case Keys.Enter:
                case Keys.Space:
                    HandleSelectionKey();
                    e.Handled = true;
                    break;
            }
        }

        public void PauseGame()
        {
            if (!isPaused)
            {
                isPaused = true;
                timer.Stop(); // Stop the timer

                // Create and show pause overlay
                if (pauseOverlay == null)
                {
                    pauseOverlay = new Panel
                    {
                        Size = this.Size,
                        Location = new Point(0, 0),
                        BackColor = Color.FromArgb(128, 0, 0, 0), // Semi-transparent black
                        Visible = false
                    };

                    // Add "PAUSED" text
                    Label pausedLabel = new Label
                    {
                        Text = "PAUSED",
                        Font = new Font("Arial", 24, FontStyle.Bold),
                        ForeColor = Color.White,
                        AutoSize = true
                    };
                    pauseOverlay.Controls.Add(pausedLabel);
                    pausedLabel.Location = new Point(
                        (pauseOverlay.Width - pausedLabel.Width) / 2,
                        (pauseOverlay.Height - pausedLabel.Height) / 2
                    );

                    this.Controls.Add(pauseOverlay);
                } 
            

             pauseOverlay.Visible = true;
                pauseOverlay.BringToFront();

                // Disable all candy buttons
                foreach (Button button in buttons)
                {
                    button.Enabled = false;
                }
            }
        }

        public void ResumeGame()
        {
            if (isPaused)
            {
                isPaused = false;
                timer.Start(); // Resume the timer

                // Hide pause overlay
                if (pauseOverlay != null)
                {
                    pauseOverlay.Visible = false;
                }

                // Re-enable all candy buttons
                foreach (Button button in buttons)
                {
                    button.Enabled = true;
                }
            }
        }

        public void TogglePause()
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        private void SwapButtons(Button button1, Button button2)
        {
            Color tempColor = button1.BackColor;
            button1.BackColor = button2.BackColor;
            button2.BackColor = tempColor;
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            if (isPaused) return; // Don't process clicks while paused

            Button button = sender as Button;

            if (button == null) return;

            Point pos = (Point)button.Tag;

            if (IsJoker(button))
            {
                ActivateJoker(button, pos);
                MakePiecesFall();
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

        private int GetMatchLength(int row, int col, bool horizontal)
        {
            Color color = buttons[row, col].BackColor;
            if (color == Color.Transparent || !string.IsNullOrEmpty(buttons[row, col].Text)) return 0;

            int matchLength = 1;

            if (horizontal)
            {
                // Check right
                for (int i = col + 1; i < cols; i++)
                {
                    if (buttons[row, i].BackColor == color && string.IsNullOrEmpty(buttons[row, i].Text))
                        matchLength++;
                    else
                        break;
                }
                // Check left
                for (int i = col - 1; i >= 0; i--)
                {
                    if (buttons[row, i].BackColor == color && string.IsNullOrEmpty(buttons[row, i].Text))
                        matchLength++;
                    else
                        break;
                }
            }
            else
            {
                // Check down
                for (int i = row + 1; i < rows; i++)
                {
                    if (buttons[i, col].BackColor == color && string.IsNullOrEmpty(buttons[i, col].Text))
                        matchLength++;
                    else
                        break;
                }
                // Check up
                for (int i = row - 1; i >= 0; i--)
                {
                    if (buttons[i, col].BackColor == color && string.IsNullOrEmpty(buttons[i, col].Text))
                        matchLength++;
                    else
                        break;
                }
            }

            return matchLength;
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

        private async void RemoveMatches()
        {
            bool[,] toRemove = new bool[rows, cols];
            int matchCount = 0;
            Dictionary<Point, (int horizontal, int vertical)> matchLengths = new Dictionary<Point, (int, int)>();

            // Detect initial matches
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    // Skip empty or joker cells
                    if (buttons[row, col].BackColor == Color.Transparent ||
                        !string.IsNullOrEmpty(buttons[row, col].Text))
                        continue;

                    int horizontalMatch = GetMatchLength(row, col, true);
                    int verticalMatch = GetMatchLength(row, col, false);

                    if (horizontalMatch >= 3 || verticalMatch >= 3)
                    {
                        matchLengths[new Point(row, col)] = (horizontalMatch, verticalMatch);
                    }
                }
            }

            // Process matches and create jokers
            foreach (var match in matchLengths)
            {
                Point pos = match.Key;
                var (horizontalMatch, verticalMatch) = match.Value;
                int row = pos.X, col = pos.Y;

                // Only process the starting position of a match
                if ((horizontalMatch >= 3 && (col == 0 || buttons[row, col - 1].BackColor != buttons[row, col].BackColor)) ||
                    (verticalMatch >= 3 && (row == 0 || buttons[row - 1, col].BackColor != buttons[row, col].BackColor)))
                {
                    Color currentColor = buttons[row, col].BackColor;
                    bool jokerCreated = false;

                    // Create joker based on match pattern
                    if (horizontalMatch >= 4)
                    {
                        buttons[row, col].Text = "R"; // Row joker
                        buttons[row, col].ForeColor = Color.White;
                        jokerCreated = true;
                    }
                    else if (verticalMatch >= 4)
                    {
                        buttons[row, col].Text = "C"; // Column joker
                        buttons[row, col].ForeColor = Color.White;
                        jokerCreated = true;
                    }
                    else if (horizontalMatch >= 5 || verticalMatch >= 5)
                    {
                        buttons[row, col].Text = "CB"; // Color bomb
                        buttons[row, col].ForeColor = Color.White;
                        jokerCreated = true;
                    }
                    else if (IsTShape(row, col) || IsLShape(row, col))
                    {
                        buttons[row, col].Text = "B"; // Bomb
                        buttons[row, col].ForeColor = Color.White;
                        jokerCreated = true;
                    }

                    // Mark pieces for removal, excluding joker position
                    if (horizontalMatch >= 3)
                    {
                        for (int i = 0; i < horizontalMatch && (col + i) < cols; i++)
                        {
                            if (jokerCreated && i == 0) continue; // Skip joker position
                            toRemove[row, col + i] = true;
                        }
                    }
                    if (verticalMatch >= 3)
                    {
                        for (int i = 0; i < verticalMatch && (row + i) < rows; i++)
                        {
                            if (jokerCreated && i == 0) continue; // Skip joker position
                            toRemove[row + i, col] = true;
                        }
                    }

                    // Keep joker's color
                    if (jokerCreated)
                    {
                        buttons[row, col].BackColor = currentColor;
                    }
                }
            }

            // Remove marked pieces
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (toRemove[row, col] && string.IsNullOrEmpty(buttons[row, col].Text))
                    {
                        buttons[row, col].BackColor = Color.Transparent; // Make the cell transparent
                        buttons[row, col].Text = ""; // Clear the text
                        matchCount++;
                    }
                }
            }

            // Handle the falling of pieces and filling empty spaces
            if (matchCount > 0)
            {
                score += matchCount * 10;
                ScoreLabel.Text = "Score: " + score;
                await Task.Delay(500);
                FillEmptySpaces();
                DetectAndRemoveInitialMatches();
            }

        }

        private void FillEmptySpaces()
        {
            for (int col = 0; col < cols; col++)
            {
                int emptyRow = rows - 1; // Start from the bottom of the column

                // Move non-transparent boxes down
                for (int row = rows - 1; row >= 0; row--)
                {
                    if (buttons[row, col].BackColor != Color.Transparent)
                    {
                        // Move the current box down to the first empty row
                        buttons[emptyRow, col].BackColor = buttons[row, col].BackColor;
                        buttons[emptyRow, col].Text = buttons[row, col].Text;
                        emptyRow--; // Move to the next empty row
                    }
                }

                // Clear the remaining boxes above the last filled position
                for (int row = emptyRow; row >= 0; row--)
                {
                    buttons[row, col].BackColor = Color.Transparent;
                    buttons[row, col].Text = ""; // Optionally generate a new box here
                }

                // Generate new boxes at the top of the column
                for (int row = 0; row <= emptyRow; row++)
                {
                    buttons[row, col].BackColor = colors[Random.Next(colors.Length)];
                    buttons[row, col].Text = ""; // Ensure no joker text is assigned
                }
            }
        }

        public void AddPlayerNameLabel(string playerName)
        {
            Label playerNameLabel = new Label
            {
                Text = $"Player: {playerName}",
                Location = new Point(200, 10), // Ajuster pour une position visible
                Font = new Font("Arial", 14, FontStyle.Bold),
                AutoSize = true,
            };

            Console.WriteLine($"Player Label Position: {playerNameLabel.Location}, Size: {playerNameLabel.Size}");


            // Vérifiez que le Label est visible et correctement ajouté
            playerNameLabel.BringToFront();
            this.Controls.Add(playerNameLabel);

        }

        private void CreateJokerAtPosition(int row, int col, string type, Color color)
        {
            buttons[row, col].Text = type;
            buttons[row, col].ForeColor = Color.White;
            buttons[row, col].BackColor = color;
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
            if (!IsValidJoker(joker)) return;

            string jokerType = joker.Text;
            var affectedCells = new List<Point>();

            switch (jokerType)
            {
                case "B": // Bomb
                    affectedCells = GetBombAffectedCells(position);
                    break;
                case "R": // Row
                    affectedCells = GetRowAffectedCells(position);
                    break;
                case "C": // Column
                    affectedCells = GetColumnAffectedCells(position);
                    break;
                case "CB": // Color Bomb
                    affectedCells = GetColorBombAffectedCells(joker.BackColor);
                    break;
            }

            ClearAffectedCells(affectedCells);
            MakePiecesFall();
        }

        private bool IsValidJoker(Button joker)
        {
            return joker != null && !string.IsNullOrEmpty(joker.Text) &&
                   new[] { "B", "R", "C", "CB" }.Contains(joker.Text);
        }

        private bool IsJoker(Button button)
        {
            return !string.IsNullOrEmpty(button.Text) &&
                   new[] { "B", "R", "C", "CB" }.Contains(button.Text);
        }

        private void CreateSpecialPiece(int row, int col, string type)
        {
            buttons[row, col].Text = type;
            buttons[row, col].ForeColor = Color.White;
            buttons[row, col].Font = new Font("Arial", 12, FontStyle.Bold);

            // Add visual feedback
            using (var g = buttons[row, col].CreateGraphics())
            {
                g.DrawRectangle(new Pen(Color.Gold, 2),
                    new Rectangle(2, 2, buttons[row, col].Width - 4,
                                buttons[row, col].Height - 4));
            }
        }

        private List<Point> GetBombAffectedCells(Point position)
        {
            var cells = new List<Point>();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = position.X + i;
                    int newCol = position.Y + j;
                    if (IsValidPosition(new Point(newRow, newCol)))
                    {
                        cells.Add(new Point(newRow, newCol));
                    }
                }
            }
            return cells;
        }

        private List<Point> GetRowAffectedCells(Point position)
        {
            return Enumerable.Range(0, cols)
                            .Select(col => new Point(position.X, col))
                            .ToList();
        }

        private List<Point> GetColumnAffectedCells(Point position)
        {
            return Enumerable.Range(0, rows)
                            .Select(row => new Point(row, position.Y))
                            .ToList();
        }

        private List<Point> GetColorBombAffectedCells(Color targetColor)
        {
            var cells = new List<Point>();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (buttons[row, col].BackColor == targetColor)
                    {
                        cells.Add(new Point(row, col));
                    }
                }
            }
            return cells;
        }

        private bool IsValidPosition(Point position)
        {
            return position.X >= 0 &&
                   position.X < rows &&
                   position.Y >= 0 &&
                   position.Y < cols;
        }

        private void ClearAffectedCells(List<Point> cells)
        {
            foreach (var cell in cells)
            {
                buttons[cell.X, cell.Y].BackColor = Color.Transparent;
                buttons[cell.X, cell.Y].Text = "";
            }
        }

        public void InitializeScoreAndTimer(string playerName)
        {
            this.playerName = playerName; // Ensure playerName is stored here too

            //Label for time
            TimerLabel = new Label
            {
                Text = "Time : 60",
                Location = new Point(10, 10),
                Font = new Font("Arial", 14, FontStyle.Bold),
                AutoSize = true
            };
            this.Controls.Add(TimerLabel);

            // Label for player name
            PlayerNameLabel = new Label
            {
                Text = $"Player: {playerName}",
                Location = new Point(250, 10), // Adjust the position as needed
                Font = new Font("Arial", 14, FontStyle.Bold),
                AutoSize = true
            };
            this.Controls.Add(PlayerNameLabel);

            //label for score
            ScoreLabel = new Label
            {
                Text = "Score: 0",
                Location = new Point(500, 10),
                Font = new Font("Arial", 14, FontStyle.Bold),
                AutoSize = true
            };
            this.Controls.Add(ScoreLabel);

            timer = new System.Windows.Forms.Timer
            {
                Interval = 2000 // 2 seconds
            };
            timer.Tick += Timer_Tick;
            timeLeft = 50; // Initial time
            timer.Start();

            Button restartButton = new Button
            {
                Text = "Restart",
                Location = new Point(700, 10),
                Size = new Size(80, 30),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.LightGray
            };

            restartButton.Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to restart the game?\nYour current progress will be lost.",
                    "Restart Game",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    // Save the current score before restarting
                    if (score > 0)
                    {
                        var finalScore = new PlayerScore(playerName, score);
                        SaveScore(finalScore);
                    }

                    RestartGame();
                }
            };

            this.Controls.Add(restartButton);

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isPaused) return; // Skip the timer logic if the game is paused

            if (TimerLabel != null) // Check if TimerLabel has been initialized
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
                    timer.Enabled = false; // Disable the timer permanently

                    Console.WriteLine($"Game over for player: {this.playerName}"); // Debug line


                    // Use the stored player name, with a fallback just in case
                    string finalPlayerName = !string.IsNullOrWhiteSpace(this.playerName)
                        ? this.playerName
                        : "Unknown Player";

                    // Create PlayerScore with the validated player name
                    var finalScore = new PlayerScore(finalPlayerName, score);
                    SaveScore(finalScore);

                    // Show game over dialog with option to play again
                    DialogResult result = MessageBox.Show(
                        $"Time's up! {finalPlayerName}'s score: {score}\n\nWould you like to play again?",
                        "Game Over",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        RestartGame();
                    }
                    else
                    {
                        this.Hide();
                        Form1 newForm = new Form1();
                        newForm.Show();
                    }
                }
            }
        }

        public bool CanRestart()
        {
            return gameOver || timeLeft <= 0;
        }

        public void RestartGame()
        {
            // Reset game state
            gameOver = false;
            score = 0;
            timeLeft = 50;

            // Update labels
            ScoreLabel.Text = "Score: 0";
            TimerLabel.Text = "Time: " + timeLeft;

            // Clear and reinitialize the game board
            foreach (Button button in buttons)
            {
                button.BackColor = colors[Random.Next(colors.Length)];
                button.Text = "";
            }

            // Check for and remove any initial matches
            DetectAndRemoveInitialMatches();

            // Restart the timer
            timer.Start();

            // Make sure the board is visible
            this.Show();

            // Reset the pause state if necessary
            if (isPaused)
            {
                ResumeGame();
            }

            // Clear any selected button
            if (selectedButton != null)
            {
                selectedButton.FlatAppearance.BorderSize = 0;
                selectedButton = null;
            }

            // Hide pause overlay if it exists and is visible
            if (pauseOverlay != null)
            {
                pauseOverlay.Visible = false;
            }
        }

        private void SaveScore(PlayerScore newScore)
        {
            try
            {
                // Validate the score object
                if (newScore == null)
                {
                    MessageBox.Show("Invalid score data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Ensure directory exists
                string directory = Path.GetDirectoryName(scorePath);
                Directory.CreateDirectory(directory);

                // Load existing scores
                List<PlayerScore> scores = LoadHighScores();

                // Add new score
                scores.Add(newScore);

                // Sort scores and take top 10
                scores = scores.OrderByDescending(s => s.Score).Take(10).ToList();

                // Save scores to file
                using (StreamWriter writer = new StreamWriter(scorePath, false))
                {
                    foreach (var score in scores)
                    {
                        if (score != null && !string.IsNullOrEmpty(score.PlayerName))
                        {
                            // Use pipe separator and ensure proper formatting
                            string line = $"{score.PlayerName.Trim()}|{score.Score}|{score.PlayDate:yyyy-MM-dd HH:mm:ss}";
                            writer.WriteLine(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving score: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<PlayerScore> LoadHighScores()
        {
            List<PlayerScore> scores = new List<PlayerScore>();

            if (File.Exists(scorePath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(scorePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 3)
                        {
                            PlayerScore score = new PlayerScore(
                              parts[0], // PlayerName
                                int.Parse(parts[1]) // Score
                            )
                            {
                                PlayDate = DateTime.Parse(parts[2]) // Date
                            };
                            scores.Add(score);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading scores: {ex.Message}");
                    return new List<PlayerScore>();
                }
            }

            return scores;
        
        }

        private void GameBoard_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.P)
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }

            if (isPaused || gameOver) return;

            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    HandleArrowKey(e.KeyCode);
                    e.IsInputKey = true;
                    break;
                case Keys.Enter:
                case Keys.Space:
                    HandleSelectionKey();
                    break;
            }
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (isPaused || gameOver) return;

            switch (e.KeyCode)
            {
                case Keys.A:
                case Keys.D:
                case Keys.W:
                case Keys.S:
                    HandleWASDKey(e.KeyCode);
                    e.Handled = true;
                    break;
                case Keys.Enter:
                case Keys.Space:
                    HandleSelectionKey();
                    e.Handled = true;
                    break;
            }
        }

        private void HandleWASDKey(Keys key)
        {
            if (!selectedPosition.HasValue)
            {
                selectedPosition = new Point(rows / 2, cols / 2);
                HighlightSelectedButton();
                return;
            }

            Point newPos = selectedPosition.Value;
            switch (key)
            {
                case Keys.A:  // Left
                    if (newPos.Y > 0) newPos.Y--;
                    break;
                case Keys.D:  // Right
                    if (newPos.Y < cols - 1) newPos.Y++;
                    break;
                case Keys.W:  // Up
                    if (newPos.X > 0) newPos.X--;
                    break;
                case Keys.S:  // Down
                    if (newPos.X < rows - 1) newPos.X++;
                    break;
            }

            if (!targetPosition.HasValue)
            {
                ClearHighlight();
                selectedPosition = newPos;
                HighlightSelectedButton();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (Parent is Form form)
            {
                form.KeyPreview = true;
                form.KeyDown += HandleKeyDown;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (Parent is Form form)
            {
                form.KeyDown -= HandleKeyDown;
            }
            base.OnHandleDestroyed(e);
        }

        private void HandleArrowKey(Keys key)
        {
            if (!selectedPosition.HasValue)
            {
                selectedPosition = new Point(rows / 2, cols / 2);
                HighlightSelectedButton();
                return;
            }

            Point newPos = selectedPosition.Value;
            switch (key)
            {
                case Keys.Left:
                    if (newPos.Y > 0) newPos.Y--;
                    break;
                case Keys.Right:
                    if (newPos.Y < cols - 1) newPos.Y++;
                    break;
                case Keys.Up:
                    if (newPos.X > 0) newPos.X--;
                    break;
                case Keys.Down:
                    if (newPos.X < rows - 1) newPos.X++;
                    break;
            }

            if (!targetPosition.HasValue)
            {
                ClearHighlight();
                selectedPosition = newPos;
                HighlightSelectedButton();
            }
        }

        private void HandleSelectionKey()
        {
            if (!selectedPosition.HasValue) return;

            if (!targetPosition.HasValue)
            {
                targetPosition = selectedPosition;
                buttons[selectedPosition.Value.X, selectedPosition.Value.Y].FlatAppearance.BorderColor = Color.Yellow;
            }
            else
            {
                Point pos1 = targetPosition.Value;
                Point pos2 = selectedPosition.Value;

                if (AreAdjacent(pos1, pos2))
                {
                    Button button1 = buttons[pos1.X, pos1.Y];
                    Button button2 = buttons[pos2.X, pos2.Y];

                    SwapButtons(button1, button2);

                    if (GetMatchLength(pos1.X, pos1.Y, true) >= 3 ||
                        GetMatchLength(pos1.X, pos1.Y, false) >= 3 ||
                        GetMatchLength(pos2.X, pos2.Y, true) >= 3 ||
                        GetMatchLength(pos2.X, pos2.Y, false) >= 3)
                    {
                        RemoveMatches();
                    }
                    else
                    {
                        SwapButtons(button1, button2);
                    }
                }

                ClearHighlight();
                selectedPosition = null;
                targetPosition = null;
            }
        }

        private bool AreAdjacent(Point p1, Point p2)
        {
            return (Math.Abs(p1.X - p2.X) == 1 && p1.Y == p2.Y) ||
                   (Math.Abs(p1.Y - p2.Y) == 1 && p1.X == p2.X);
        }

        private void HighlightSelectedButton()
        {
            if (selectedPosition.HasValue)
            {
                buttons[selectedPosition.Value.X, selectedPosition.Value.Y].FlatAppearance.BorderColor = Color.White;
                buttons[selectedPosition.Value.X, selectedPosition.Value.Y].FlatAppearance.BorderSize = 2;
            }
        }

        private void ClearHighlight()
        {
            if (selectedPosition.HasValue)
            {
                buttons[selectedPosition.Value.X, selectedPosition.Value.Y].FlatAppearance.BorderSize = 0;
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
