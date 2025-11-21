namespace minikyrsovaya
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            gamePanel = new BufferedPanel();
            lblScore = new Label();
            //btnNewGame = new Button();
            picNewGame = new PictureBox();
            //btnHighScores = new Button();
            picHighScores = new PictureBox();
            picTitle = new PictureBox();
            lblTime = new Label();
            lblTarget = new Label();
            progressBar = new ProgressBar();
            startPanel = new BufferedPanel();
            picSelectLevel = new PictureBox();
            picExit = new PictureBox();
            picEndless = new PictureBox();
            picHard = new PictureBox();
            picMedium = new PictureBox();
            picEasy = new PictureBox();
            picTrophy = new PictureBox();
            picSettings = new PictureBox();
            picGameTitle = new PictureBox();
            pictureBoxLogo = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)picTitle).BeginInit();
            startPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picSelectLevel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picExit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picEndless).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picHard).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picMedium).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picEasy).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picTrophy).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picSettings).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picGameTitle).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picNewGame).BeginInit();
            SuspendLayout();
            // 
            // gamePanel
            // 
            gamePanel.BackColor = Color.LightCoral;
            gamePanel.BackgroundImage = (Image)resources.GetObject("gamePanel.BackgroundImage");
            gamePanel.BackgroundImageLayout = ImageLayout.Stretch;
            gamePanel.BorderStyle = BorderStyle.FixedSingle;
            gamePanel.Location = new Point(160, 211);
            gamePanel.Margin = new Padding(4, 5, 4, 5);
            gamePanel.Name = "gamePanel";
            gamePanel.Size = new Size(483, 483);
            gamePanel.TabIndex = 0;
            gamePanel.Visible = false;
            // 
            // lblScore
            // 
            lblScore.AutoSize = true;
            lblScore.BackColor = Color.Transparent;
            lblScore.Font = new Font("Candara", 20F, FontStyle.Bold, GraphicsUnit.Point, 204);
            lblScore.Location = new Point(13, 50);
            lblScore.Margin = new Padding(4, 0, 4, 0);
            lblScore.Name = "lblScore";
            lblScore.Size = new Size(120, 41);
            lblScore.TabIndex = 1;
            lblScore.Text = "Счет: 0";
            lblScore.Visible = false;
            // 
            // picNewGame
            // 
            picNewGame = new PictureBox();
            picNewGame.BackColor = Color.Transparent;
            picNewGame.Cursor = Cursors.Hand;
            picNewGame.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\newgameingame.png");
            picNewGame.Location = new Point(630, 50);
            picNewGame.Size = new Size(180, 65);
            picNewGame.SizeMode = PictureBoxSizeMode.Zoom;
            picNewGame.TabIndex = 2;
            picNewGame.TabStop = false;
            picNewGame.Visible = false;
            picNewGame.Click += newGameButton_Click;
            picNewGame.MouseEnter += picNewGame_MouseEnter;
            picNewGame.MouseLeave += picNewGame_MouseLeave;
            // 
            // picHighScores
            // 
            picHighScores = new PictureBox();
            picHighScores.BackColor = Color.Transparent;
            picHighScores.Cursor = Cursors.Hand;
            picHighScores.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\recordsingame.png");
            picHighScores.Location = new Point(630, 120);
            picHighScores.Size = new Size(180, 65);
            picHighScores.SizeMode = PictureBoxSizeMode.Zoom;
            picHighScores.TabIndex = 3;
            picHighScores.TabStop = false;
            picHighScores.Visible = false;
            picHighScores.Click += highScoresButton_Click;
            picHighScores.MouseEnter += picHighScores_MouseEnter;
            picHighScores.MouseLeave += picHighScores_MouseLeave;

            // 
            // picTitle
            // 
            picTitle.BackColor = Color.Transparent;
            picTitle.Image = (Image)resources.GetObject("picTitle.Image");
            picTitle.Location = new Point(250, 2);
            picTitle.Name = "picTitle";
            picTitle.Size = new Size(300, 100);
            picTitle.SizeMode = PictureBoxSizeMode.Zoom;
            picTitle.TabIndex = 4;
            picTitle.TabStop = false;
            picTitle.Visible = false;
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.BackColor = Color.Transparent;
            lblTime.Font = new Font("Candara", 16F, FontStyle.Bold, GraphicsUnit.Point, 204);
            lblTime.ForeColor = Color.Orange;
            lblTime.Location = new Point(10, 100);
            lblTime.Margin = new Padding(4, 0, 4, 0);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(268, 33);
            lblTime.TabIndex = 5;
            lblTime.Text = "Режим: Бесконечный";
            lblTime.Visible = false;
            // 
            // lblTarget
            // 
            lblTarget.AutoSize = true;
            lblTarget.Font = new Font("Candara", 16F, FontStyle.Bold, GraphicsUnit.Point, 204);
            lblTarget.ForeColor = Color.DarkRed;
            lblTarget.Location = new Point(10, 131);
            lblTarget.Margin = new Padding(4, 0, 4, 0);
            lblTarget.Name = "lblTarget";
            lblTarget.Size = new Size(268, 33);
            lblTarget.TabIndex = 6;
            lblTarget.Text = "Режим: Бесконечный";
            lblTarget.Visible = false;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(150, 140);
            progressBar.Margin = new Padding(4, 5, 4, 5);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(460, 25);
            progressBar.TabIndex = 7;
            progressBar.Visible = false;
            // 
            // startPanel
            // 
            startPanel.BackgroundImage = (Image)resources.GetObject("startPanel.BackgroundImage");
            startPanel.BackgroundImageLayout = ImageLayout.Stretch;
            startPanel.Controls.Add(picSelectLevel);
            startPanel.Controls.Add(picExit);
            startPanel.Controls.Add(picEndless);
            startPanel.Controls.Add(picHard);
            startPanel.Controls.Add(picMedium);
            startPanel.Controls.Add(picEasy);
            startPanel.Controls.Add(picTrophy);
            startPanel.Controls.Add(picSettings);
            startPanel.Controls.Add(picGameTitle);
            startPanel.Controls.Add(pictureBoxLogo);
            startPanel.Location = new Point(-6, 0);
            startPanel.Name = "startPanel";
            startPanel.Size = new Size(822, 781);
            startPanel.TabIndex = 8;
            // 
            // picSelectLevel
            // 
            picSelectLevel.Anchor = AnchorStyles.None;
            picSelectLevel.BackColor = Color.Transparent;
            picSelectLevel.Image = (Image)resources.GetObject("picSelectLevel.Image");
            picSelectLevel.Location = new Point(185, 172);
            picSelectLevel.Name = "picSelectLevel";
            picSelectLevel.Size = new Size(432, 105);
            picSelectLevel.SizeMode = PictureBoxSizeMode.Zoom;
            picSelectLevel.TabIndex = 10;
            picSelectLevel.TabStop = false;
            // 
            // picExit
            // 
            picExit.Anchor = AnchorStyles.None;
            picExit.BackColor = Color.Transparent;
            picExit.Cursor = Cursors.Hand;
            picExit.Image = (Image)resources.GetObject("picExit.Image");
            picExit.Location = new Point(202, 683);
            picExit.Name = "picExit";
            picExit.Size = new Size(394, 98);
            picExit.SizeMode = PictureBoxSizeMode.Zoom;
            picExit.TabIndex = 9;
            picExit.TabStop = false;
            picExit.Click += picExit_Click;
            picExit.MouseEnter += picExit_MouseEnter;
            picExit.MouseLeave += picExit_MouseLeave;
            // 
            // picEndless
            // 
            picEndless.Anchor = AnchorStyles.None;
            picEndless.BackColor = Color.Transparent;
            picEndless.Cursor = Cursors.Hand;
            picEndless.Image = (Image)resources.GetObject("picEndless.Image");
            picEndless.Location = new Point(256, 575);
            picEndless.Name = "picEndless";
            picEndless.Size = new Size(283, 130);
            picEndless.SizeMode = PictureBoxSizeMode.Zoom;
            picEndless.TabIndex = 8;
            picEndless.TabStop = false;
            picEndless.Click += picEndless_Click;
            picEndless.MouseEnter += picEndless_MouseEnter;
            picEndless.MouseLeave += picEndless_MouseLeave;
            // 
            // picHard
            // 
            picHard.Anchor = AnchorStyles.None;
            picHard.BackColor = Color.Transparent;
            picHard.Cursor = Cursors.Hand;
            picHard.Image = (Image)resources.GetObject("picHard.Image");
            picHard.Location = new Point(256, 470);
            picHard.Name = "picHard";
            picHard.Size = new Size(283, 130);
            picHard.SizeMode = PictureBoxSizeMode.Zoom;
            picHard.TabIndex = 7;
            picHard.TabStop = false;
            picHard.Click += picHard_Click;
            picHard.MouseEnter += picHard_MouseEnter;
            picHard.MouseLeave += picHard_MouseLeave;
            // 
            // picMedium
            // 
            picMedium.Anchor = AnchorStyles.None;
            picMedium.BackColor = Color.Transparent;
            picMedium.Cursor = Cursors.Hand;
            picMedium.Image = (Image)resources.GetObject("picMedium.Image");
            picMedium.Location = new Point(256, 368);
            picMedium.Name = "picMedium";
            picMedium.Size = new Size(280, 127);
            picMedium.SizeMode = PictureBoxSizeMode.Zoom;
            picMedium.TabIndex = 6;
            picMedium.TabStop = false;
            picMedium.Click += picMedium_Click;
            picMedium.MouseEnter += picMedium_MouseEnter;
            picMedium.MouseLeave += picMedium_MouseLeave;
            // 
            // picEasy
            // 
            picEasy.Anchor = AnchorStyles.None;
            picEasy.BackColor = Color.Transparent;
            picEasy.Cursor = Cursors.Hand;
            picEasy.Image = (Image)resources.GetObject("picEasy.Image");
            picEasy.Location = new Point(260, 267);
            picEasy.Name = "picEasy";
            picEasy.Size = new Size(276, 129);
            picEasy.SizeMode = PictureBoxSizeMode.Zoom;
            picEasy.TabIndex = 5;
            picEasy.TabStop = false;
            picEasy.Click += picEasy_Click;
            picEasy.MouseEnter += picEasy_MouseEnter;
            picEasy.MouseLeave += picEasy_MouseLeave;
            // 
            // picTrophy
            // 
            picTrophy.Anchor = AnchorStyles.None;
            picTrophy.BackColor = Color.Transparent;
            picTrophy.Cursor = Cursors.Hand;
            picTrophy.Image = (Image)resources.GetObject("picTrophy.Image");
            picTrophy.Location = new Point(704, 676);
            picTrophy.Name = "picTrophy";
            picTrophy.Size = new Size(81, 70);
            picTrophy.SizeMode = PictureBoxSizeMode.StretchImage;
            picTrophy.TabIndex = 8;
            picTrophy.TabStop = false;
            picTrophy.Click += picTrophy_Click;
            picTrophy.MouseEnter += picTrophy_MouseEnter;
            picTrophy.MouseLeave += picTrophy_MouseLeave;
            // 
            // picSettings
            // 
            picSettings.Anchor = AnchorStyles.None;
            picSettings.BackColor = Color.Transparent;
            picSettings.Cursor = Cursors.Hand;
            picSettings.Location = new Point(704, 572);
            picSettings.Name = "picSettings";
            picSettings.Size = new Size(81, 72);
            picSettings.SizeMode = PictureBoxSizeMode.StretchImage;
            picSettings.TabIndex = 9;
            picSettings.TabStop = false;
            picSettings.Click += picSettings_Click;
            picSettings.MouseEnter += picSettings_MouseEnter;
            picSettings.MouseLeave += picSettings_MouseLeave;
            // 
            //picGameTitle


            picGameTitle.Anchor = AnchorStyles.Top;
            picGameTitle.BackColor = Color.Transparent;
            picGameTitle.Image = (Image)resources.GetObject("picGameTitle.Image");
            picGameTitle.Location = new Point(240, -7);
            picGameTitle.Name = "picGameTitle";
            picGameTitle.Size = new Size(316, 182);
            picGameTitle.SizeMode = PictureBoxSizeMode.Zoom;
            picGameTitle.TabIndex = 0;
            picGameTitle.TabStop = false;
            // 
            // pictureBoxLogo
            // 
            pictureBoxLogo.Anchor = AnchorStyles.None;
            pictureBoxLogo.BackColor = Color.Transparent;
            pictureBoxLogo.Location = new Point(96, 404);
            pictureBoxLogo.Name = "pictureBoxLogo";
            pictureBoxLogo.Size = new Size(100, 600);
            pictureBoxLogo.TabIndex = 6;
            pictureBoxLogo.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(811, 781);
            Controls.Add(startPanel);
            Controls.Add(progressBar);
            Controls.Add(lblTarget);
            Controls.Add(lblTime);
            Controls.Add(picTitle);
            Controls.Add(picHighScores);
            Controls.Add(picNewGame);
            Controls.Add(lblScore);
            Controls.Add(gamePanel);
            Margin = new Padding(4, 5, 4, 5);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Три в Ряд";
            ((System.ComponentModel.ISupportInitialize)picTitle).EndInit();
            startPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picSelectLevel).EndInit();
            ((System.ComponentModel.ISupportInitialize)picExit).EndInit();
            ((System.ComponentModel.ISupportInitialize)picEndless).EndInit();
            ((System.ComponentModel.ISupportInitialize)picHard).EndInit();
            ((System.ComponentModel.ISupportInitialize)picMedium).EndInit();
            ((System.ComponentModel.ISupportInitialize)picEasy).EndInit();
            ((System.ComponentModel.ISupportInitialize)picTrophy).EndInit();
            ((System.ComponentModel.ISupportInitialize)picSettings).EndInit();
            ((System.ComponentModel.ISupportInitialize)picGameTitle).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)picNewGame).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private BufferedPanel gamePanel;
        private System.Windows.Forms.Label lblScore;
        //private System.Windows.Forms.Button btnNewGame;
        private System.Windows.Forms.PictureBox picNewGame;
        //private System.Windows.Forms.Button btnHighScores;
        private System.Windows.Forms.PictureBox picHighScores;
        private System.Windows.Forms.PictureBox picTitle;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.ProgressBar progressBar;

        // Новые элементы для стартового окна
        private BufferedPanel startPanel;
        private System.Windows.Forms.PictureBox picGameTitle;
        private System.Windows.Forms.PictureBox picEasy;
        private System.Windows.Forms.PictureBox picMedium;
        private System.Windows.Forms.PictureBox picHard;
        private System.Windows.Forms.PictureBox picEndless;
        private System.Windows.Forms.PictureBox picExit;
        private System.Windows.Forms.PictureBox picTrophy;
        private System.Windows.Forms.PictureBox picSelectLevel;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.PictureBox picSettings;
    }
}