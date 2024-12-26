namespace CandyProje
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menu = new Panel();
            txtPlayerName = new TextBox();
            lblPlayerName = new Label();
            btn_exit = new PictureBox();
            btn_option = new PictureBox();
            btn_start = new PictureBox();
            menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)btn_exit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)btn_option).BeginInit();
            ((System.ComponentModel.ISupportInitialize)btn_start).BeginInit();
            SuspendLayout();
            // 
            // menu
            // 
            menu.BackgroundImage = Properties.Resources.menu;
            menu.BackgroundImageLayout = ImageLayout.Stretch;
            menu.Controls.Add(txtPlayerName);
            menu.Controls.Add(lblPlayerName);
            menu.Controls.Add(btn_exit);
            menu.Controls.Add(btn_option);
            menu.Controls.Add(btn_start);
            menu.Location = new Point(127, 38);
            menu.Name = "menu";
            menu.Size = new Size(580, 481);
            menu.TabIndex = 0;
            // 
            // txtPlayerName
            // 
            txtPlayerName.Location = new Point(384, 224);
            txtPlayerName.Name = "txtPlayerName";
            txtPlayerName.Size = new Size(153, 27);
            txtPlayerName.TabIndex = 5;
            // 
            // lblPlayerName
            // 
            lblPlayerName.AutoSize = true;
            lblPlayerName.BackColor = Color.Silver;
            lblPlayerName.Location = new Point(294, 224);
            lblPlayerName.Name = "lblPlayerName";
            lblPlayerName.Size = new Size(87, 25);
            lblPlayerName.TabIndex = 4;
            lblPlayerName.Text = "PlayerName";
            lblPlayerName.TextAlign = ContentAlignment.MiddleLeft;
            lblPlayerName.UseCompatibleTextRendering = true;
            // 
            // btn_exit
            // 
            btn_exit.Image = Properties.Resources.exit_normal;
            btn_exit.Location = new Point(65, 318);
            btn_exit.Name = "btn_exit";
            btn_exit.Size = new Size(100, 43);
            btn_exit.SizeMode = PictureBoxSizeMode.AutoSize;
            btn_exit.TabIndex = 2;
            btn_exit.TabStop = false;
            btn_exit.Click += btn_exit_Click;
            btn_exit.MouseLeave += btn_exit_MouseLeave;
            btn_exit.MouseHover += btn_exit_MouseHover;
            // 
            // btn_option
            // 
            btn_option.Image = Properties.Resources.option_normal;
            btn_option.Location = new Point(65, 206);
            btn_option.Name = "btn_option";
            btn_option.Size = new Size(100, 43);
            btn_option.SizeMode = PictureBoxSizeMode.AutoSize;
            btn_option.TabIndex = 1;
            btn_option.TabStop = false;
            btn_option.Click += btn_option_Click;
            btn_option.MouseLeave += btn_option_MouseLeave;
            btn_option.MouseHover += btn_option_MouseHover;
            // 
            // btn_start
            // 
            btn_start.Image = Properties.Resources.start_normal;
            btn_start.Location = new Point(65, 92);
            btn_start.Name = "btn_start";
            btn_start.Size = new Size(100, 43);
            btn_start.SizeMode = PictureBoxSizeMode.AutoSize;
            btn_start.TabIndex = 0;
            btn_start.TabStop = false;
            btn_start.Click += btn_start_Click;
            btn_start.MouseLeave += btn_start_MouseLeave;
            btn_start.MouseHover += btn_start_MouseHover;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 0, 0);
            ClientSize = new Size(968, 602);
            Controls.Add(menu);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            menu.ResumeLayout(false);
            menu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)btn_exit).EndInit();
            ((System.ComponentModel.ISupportInitialize)btn_option).EndInit();
            ((System.ComponentModel.ISupportInitialize)btn_start).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel menu;
        private PictureBox btn_exit;
        private PictureBox btn_option;
        private PictureBox btn_start;
        private Label lblPlayerName;
        private TextBox txtPlayerName;
    }
}
