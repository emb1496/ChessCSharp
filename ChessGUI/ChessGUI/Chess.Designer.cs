namespace ChessGUI
{
    partial class Chess
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBoxChat = new System.Windows.Forms.TextBox();
            this.textBoxNotation = new System.Windows.Forms.TextBox();
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.ButtonSendMessage = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.LowerTimeLabel = new System.Windows.Forms.Label();
            this.ThreeHour = new System.Windows.Forms.RadioButton();
            this.OneHour = new System.Windows.Forms.RadioButton();
            this.ThirtyMin = new System.Windows.Forms.RadioButton();
            this.FifteenMin = new System.Windows.Forms.RadioButton();
            this.TenMin = new System.Windows.Forms.RadioButton();
            this.FiveMin = new System.Windows.Forms.RadioButton();
            this.OneMin = new System.Windows.Forms.RadioButton();
            this.UpperTimeLabel = new System.Windows.Forms.Label();
            this.ButtonStartOfGame = new System.Windows.Forms.Button();
            this.ButtonBackOne = new System.Windows.Forms.Button();
            this.ButtonForwardOne = new System.Windows.Forms.Button();
            this.ButtonCurrentMove = new System.Windows.Forms.Button();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelBlackPawnsTaken = new System.Windows.Forms.Label();
            this.labelBlackKnightsTaken = new System.Windows.Forms.Label();
            this.labelBlackBishopsTaken = new System.Windows.Forms.Label();
            this.labelBlackRooksTaken = new System.Windows.Forms.Label();
            this.labelWhiteQueensTaken = new System.Windows.Forms.Label();
            this.labelWhiteRooksTaken = new System.Windows.Forms.Label();
            this.labelWhiteBishopsTaken = new System.Windows.Forms.Label();
            this.labelWhiteKnightsTaken = new System.Windows.Forms.Label();
            this.labelWhitePawnsTaken = new System.Windows.Forms.Label();
            this.labelBlackQueensTaken = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxChat
            // 
            this.textBoxChat.Location = new System.Drawing.Point(616, 13);
            this.textBoxChat.Multiline = true;
            this.textBoxChat.Name = "textBoxChat";
            this.textBoxChat.ReadOnly = true;
            this.textBoxChat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxChat.Size = new System.Drawing.Size(348, 474);
            this.textBoxChat.TabIndex = 0;
            // 
            // textBoxNotation
            // 
            this.textBoxNotation.Location = new System.Drawing.Point(970, 13);
            this.textBoxNotation.Multiline = true;
            this.textBoxNotation.Name = "textBoxNotation";
            this.textBoxNotation.ReadOnly = true;
            this.textBoxNotation.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxNotation.Size = new System.Drawing.Size(335, 258);
            this.textBoxNotation.TabIndex = 1;
            // 
            // textBoxInput
            // 
            this.textBoxInput.Location = new System.Drawing.Point(616, 493);
            this.textBoxInput.Multiline = true;
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxInput.Size = new System.Drawing.Size(348, 179);
            this.textBoxInput.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(150, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(317, 39);
            this.button1.TabIndex = 4;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // ButtonSendMessage
            // 
            this.ButtonSendMessage.Location = new System.Drawing.Point(970, 614);
            this.ButtonSendMessage.Name = "ButtonSendMessage";
            this.ButtonSendMessage.Size = new System.Drawing.Size(341, 58);
            this.ButtonSendMessage.TabIndex = 5;
            this.ButtonSendMessage.Text = "Send Message";
            this.ButtonSendMessage.UseVisualStyleBackColor = true;
            this.ButtonSendMessage.Click += new System.EventHandler(this.ButtonSendMessage_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(970, 496);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 17);
            this.label1.TabIndex = 6;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(970, 493);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(0, 17);
            this.Label2.TabIndex = 7;
            // 
            // Timer1
            // 
            this.Timer1.Interval = 1000;
            this.Timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // LowerTimeLabel
            // 
            this.LowerTimeLabel.AutoSize = true;
            this.LowerTimeLabel.Location = new System.Drawing.Point(450, 629);
            this.LowerTimeLabel.Name = "LowerTimeLabel";
            this.LowerTimeLabel.Size = new System.Drawing.Size(0, 17);
            this.LowerTimeLabel.TabIndex = 8;
            // 
            // ThreeHour
            // 
            this.ThreeHour.AutoSize = true;
            this.ThreeHour.Checked = true;
            this.ThreeHour.Location = new System.Drawing.Point(150, 59);
            this.ThreeHour.Name = "ThreeHour";
            this.ThreeHour.Size = new System.Drawing.Size(77, 21);
            this.ThreeHour.TabIndex = 9;
            this.ThreeHour.TabStop = true;
            this.ThreeHour.Text = "3 hours";
            this.ThreeHour.UseVisualStyleBackColor = true;
            this.ThreeHour.CheckedChanged += new System.EventHandler(this.ThreeHour_CheckedChanged);
            // 
            // OneHour
            // 
            this.OneHour.AutoSize = true;
            this.OneHour.Location = new System.Drawing.Point(150, 86);
            this.OneHour.Name = "OneHour";
            this.OneHour.Size = new System.Drawing.Size(70, 21);
            this.OneHour.TabIndex = 10;
            this.OneHour.Text = "1 hour";
            this.OneHour.UseVisualStyleBackColor = true;
            this.OneHour.CheckedChanged += new System.EventHandler(this.OneHour_CheckedChanged);
            // 
            // ThirtyMin
            // 
            this.ThirtyMin.AutoSize = true;
            this.ThirtyMin.Location = new System.Drawing.Point(150, 113);
            this.ThirtyMin.Name = "ThirtyMin";
            this.ThirtyMin.Size = new System.Drawing.Size(71, 21);
            this.ThirtyMin.TabIndex = 11;
            this.ThirtyMin.Text = "30 min";
            this.ThirtyMin.UseVisualStyleBackColor = true;
            this.ThirtyMin.CheckedChanged += new System.EventHandler(this.ThirtyMin_CheckedChanged);
            // 
            // FifteenMin
            // 
            this.FifteenMin.AutoSize = true;
            this.FifteenMin.Location = new System.Drawing.Point(150, 140);
            this.FifteenMin.Name = "FifteenMin";
            this.FifteenMin.Size = new System.Drawing.Size(71, 21);
            this.FifteenMin.TabIndex = 12;
            this.FifteenMin.Text = "15 min";
            this.FifteenMin.UseVisualStyleBackColor = true;
            this.FifteenMin.CheckedChanged += new System.EventHandler(this.FifteenMin_CheckedChanged);
            // 
            // TenMin
            // 
            this.TenMin.AutoSize = true;
            this.TenMin.Location = new System.Drawing.Point(150, 167);
            this.TenMin.Name = "TenMin";
            this.TenMin.Size = new System.Drawing.Size(71, 21);
            this.TenMin.TabIndex = 13;
            this.TenMin.Text = "10 min";
            this.TenMin.UseVisualStyleBackColor = true;
            this.TenMin.CheckedChanged += new System.EventHandler(this.TenMin_CheckedChanged);
            // 
            // FiveMin
            // 
            this.FiveMin.AutoSize = true;
            this.FiveMin.Location = new System.Drawing.Point(150, 194);
            this.FiveMin.Name = "FiveMin";
            this.FiveMin.Size = new System.Drawing.Size(63, 21);
            this.FiveMin.TabIndex = 14;
            this.FiveMin.Text = "5 min";
            this.FiveMin.UseVisualStyleBackColor = true;
            this.FiveMin.CheckedChanged += new System.EventHandler(this.FiveMin_CheckedChanged);
            // 
            // OneMin
            // 
            this.OneMin.AutoSize = true;
            this.OneMin.Location = new System.Drawing.Point(150, 221);
            this.OneMin.Name = "OneMin";
            this.OneMin.Size = new System.Drawing.Size(63, 21);
            this.OneMin.TabIndex = 15;
            this.OneMin.Text = "1 min";
            this.OneMin.UseVisualStyleBackColor = true;
            this.OneMin.CheckedChanged += new System.EventHandler(this.OneMin_CheckedChanged);
            // 
            // UpperTimeLabel
            // 
            this.UpperTimeLabel.AutoSize = true;
            this.UpperTimeLabel.Location = new System.Drawing.Point(450, 13);
            this.UpperTimeLabel.Name = "UpperTimeLabel";
            this.UpperTimeLabel.Size = new System.Drawing.Size(0, 17);
            this.UpperTimeLabel.TabIndex = 16;
            // 
            // ButtonStartOfGame
            // 
            this.ButtonStartOfGame.Enabled = false;
            this.ButtonStartOfGame.Location = new System.Drawing.Point(970, 516);
            this.ButtonStartOfGame.Name = "ButtonStartOfGame";
            this.ButtonStartOfGame.Size = new System.Drawing.Size(75, 48);
            this.ButtonStartOfGame.TabIndex = 17;
            this.ButtonStartOfGame.Text = "|<";
            this.ButtonStartOfGame.UseVisualStyleBackColor = true;
            this.ButtonStartOfGame.Click += new System.EventHandler(this.ButtonStartOfGame_Click);
            // 
            // ButtonBackOne
            // 
            this.ButtonBackOne.Enabled = false;
            this.ButtonBackOne.Location = new System.Drawing.Point(1051, 516);
            this.ButtonBackOne.Name = "ButtonBackOne";
            this.ButtonBackOne.Size = new System.Drawing.Size(75, 48);
            this.ButtonBackOne.TabIndex = 18;
            this.ButtonBackOne.Text = "<";
            this.ButtonBackOne.UseVisualStyleBackColor = true;
            this.ButtonBackOne.Click += new System.EventHandler(this.ButtonBackOne_Click);
            // 
            // ButtonForwardOne
            // 
            this.ButtonForwardOne.Enabled = false;
            this.ButtonForwardOne.Location = new System.Drawing.Point(1132, 516);
            this.ButtonForwardOne.Name = "ButtonForwardOne";
            this.ButtonForwardOne.Size = new System.Drawing.Size(75, 48);
            this.ButtonForwardOne.TabIndex = 19;
            this.ButtonForwardOne.Text = ">";
            this.ButtonForwardOne.UseVisualStyleBackColor = true;
            this.ButtonForwardOne.Click += new System.EventHandler(this.ButtonForwardOne_Click);
            // 
            // ButtonCurrentMove
            // 
            this.ButtonCurrentMove.Enabled = false;
            this.ButtonCurrentMove.Location = new System.Drawing.Point(1213, 516);
            this.ButtonCurrentMove.Name = "ButtonCurrentMove";
            this.ButtonCurrentMove.Size = new System.Drawing.Size(75, 48);
            this.ButtonCurrentMove.TabIndex = 20;
            this.ButtonCurrentMove.Text = ">|";
            this.ButtonCurrentMove.UseVisualStyleBackColor = true;
            this.ButtonCurrentMove.Click += new System.EventHandler(this.ButtonCurrentMove_Click);
            // 
            // pictureBox10
            // 
            this.pictureBox10.Image = global::ChessGUI.Properties.Resources.pW;
            this.pictureBox10.Location = new System.Drawing.Point(970, 373);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Size = new System.Drawing.Size(50, 50);
            this.pictureBox10.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox10.TabIndex = 30;
            this.pictureBox10.TabStop = false;
            // 
            // pictureBox9
            // 
            this.pictureBox9.Image = global::ChessGUI.Properties.Resources.NW;
            this.pictureBox9.Location = new System.Drawing.Point(1026, 373);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(50, 50);
            this.pictureBox9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox9.TabIndex = 29;
            this.pictureBox9.TabStop = false;
            // 
            // pictureBox8
            // 
            this.pictureBox8.Image = global::ChessGUI.Properties.Resources.BW;
            this.pictureBox8.Location = new System.Drawing.Point(1082, 373);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(50, 50);
            this.pictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox8.TabIndex = 28;
            this.pictureBox8.TabStop = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.Image = global::ChessGUI.Properties.Resources.RW;
            this.pictureBox7.Location = new System.Drawing.Point(1138, 373);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(50, 50);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox7.TabIndex = 27;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Image = global::ChessGUI.Properties.Resources.NB;
            this.pictureBox6.Location = new System.Drawing.Point(1026, 277);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(50, 50);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox6.TabIndex = 26;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::ChessGUI.Properties.Resources.BB;
            this.pictureBox5.Location = new System.Drawing.Point(1082, 277);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(50, 50);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox5.TabIndex = 25;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::ChessGUI.Properties.Resources.RB;
            this.pictureBox4.Location = new System.Drawing.Point(1138, 277);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(50, 50);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 24;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::ChessGUI.Properties.Resources.QB;
            this.pictureBox3.Location = new System.Drawing.Point(1194, 277);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(50, 50);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 23;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::ChessGUI.Properties.Resources.QW;
            this.pictureBox2.Location = new System.Drawing.Point(1194, 373);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(50, 50);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 22;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ChessGUI.Properties.Resources.pB;
            this.pictureBox1.Location = new System.Drawing.Point(970, 277);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(50, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // labelBlackPawnsTaken
            // 
            this.labelBlackPawnsTaken.AutoSize = true;
            this.labelBlackPawnsTaken.Location = new System.Drawing.Point(985, 335);
            this.labelBlackPawnsTaken.Name = "labelBlackPawnsTaken";
            this.labelBlackPawnsTaken.Size = new System.Drawing.Size(22, 17);
            this.labelBlackPawnsTaken.TabIndex = 31;
            this.labelBlackPawnsTaken.Text = "x0";
            // 
            // labelBlackKnightsTaken
            // 
            this.labelBlackKnightsTaken.AutoSize = true;
            this.labelBlackKnightsTaken.Location = new System.Drawing.Point(1041, 335);
            this.labelBlackKnightsTaken.Name = "labelBlackKnightsTaken";
            this.labelBlackKnightsTaken.Size = new System.Drawing.Size(22, 17);
            this.labelBlackKnightsTaken.TabIndex = 32;
            this.labelBlackKnightsTaken.Text = "x0";
            // 
            // labelBlackBishopsTaken
            // 
            this.labelBlackBishopsTaken.AutoSize = true;
            this.labelBlackBishopsTaken.Location = new System.Drawing.Point(1097, 335);
            this.labelBlackBishopsTaken.Name = "labelBlackBishopsTaken";
            this.labelBlackBishopsTaken.Size = new System.Drawing.Size(22, 17);
            this.labelBlackBishopsTaken.TabIndex = 33;
            this.labelBlackBishopsTaken.Text = "x0";
            // 
            // labelBlackRooksTaken
            // 
            this.labelBlackRooksTaken.AutoSize = true;
            this.labelBlackRooksTaken.Location = new System.Drawing.Point(1153, 335);
            this.labelBlackRooksTaken.Name = "labelBlackRooksTaken";
            this.labelBlackRooksTaken.Size = new System.Drawing.Size(22, 17);
            this.labelBlackRooksTaken.TabIndex = 34;
            this.labelBlackRooksTaken.Text = "x0";
            // 
            // labelWhiteQueensTaken
            // 
            this.labelWhiteQueensTaken.AutoSize = true;
            this.labelWhiteQueensTaken.Location = new System.Drawing.Point(1209, 431);
            this.labelWhiteQueensTaken.Name = "labelWhiteQueensTaken";
            this.labelWhiteQueensTaken.Size = new System.Drawing.Size(22, 17);
            this.labelWhiteQueensTaken.TabIndex = 35;
            this.labelWhiteQueensTaken.Text = "x0";
            // 
            // labelWhiteRooksTaken
            // 
            this.labelWhiteRooksTaken.AutoSize = true;
            this.labelWhiteRooksTaken.Location = new System.Drawing.Point(1153, 431);
            this.labelWhiteRooksTaken.Name = "labelWhiteRooksTaken";
            this.labelWhiteRooksTaken.Size = new System.Drawing.Size(22, 17);
            this.labelWhiteRooksTaken.TabIndex = 36;
            this.labelWhiteRooksTaken.Text = "x0";
            // 
            // labelWhiteBishopsTaken
            // 
            this.labelWhiteBishopsTaken.AutoSize = true;
            this.labelWhiteBishopsTaken.Location = new System.Drawing.Point(1097, 431);
            this.labelWhiteBishopsTaken.Name = "labelWhiteBishopsTaken";
            this.labelWhiteBishopsTaken.Size = new System.Drawing.Size(22, 17);
            this.labelWhiteBishopsTaken.TabIndex = 37;
            this.labelWhiteBishopsTaken.Text = "x0";
            // 
            // labelWhiteKnightsTaken
            // 
            this.labelWhiteKnightsTaken.AutoSize = true;
            this.labelWhiteKnightsTaken.Location = new System.Drawing.Point(1041, 431);
            this.labelWhiteKnightsTaken.Name = "labelWhiteKnightsTaken";
            this.labelWhiteKnightsTaken.Size = new System.Drawing.Size(22, 17);
            this.labelWhiteKnightsTaken.TabIndex = 38;
            this.labelWhiteKnightsTaken.Text = "x0";
            // 
            // labelWhitePawnsTaken
            // 
            this.labelWhitePawnsTaken.AutoSize = true;
            this.labelWhitePawnsTaken.Location = new System.Drawing.Point(985, 431);
            this.labelWhitePawnsTaken.Name = "labelWhitePawnsTaken";
            this.labelWhitePawnsTaken.Size = new System.Drawing.Size(22, 17);
            this.labelWhitePawnsTaken.TabIndex = 39;
            this.labelWhitePawnsTaken.Text = "x0";
            // 
            // labelBlackQueensTaken
            // 
            this.labelBlackQueensTaken.AutoSize = true;
            this.labelBlackQueensTaken.Location = new System.Drawing.Point(1209, 335);
            this.labelBlackQueensTaken.Name = "labelBlackQueensTaken";
            this.labelBlackQueensTaken.Size = new System.Drawing.Size(22, 17);
            this.labelBlackQueensTaken.TabIndex = 40;
            this.labelBlackQueensTaken.Text = "x0";
            // 
            // Chess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1317, 684);
            this.Controls.Add(this.labelBlackQueensTaken);
            this.Controls.Add(this.labelWhitePawnsTaken);
            this.Controls.Add(this.labelWhiteKnightsTaken);
            this.Controls.Add(this.labelWhiteBishopsTaken);
            this.Controls.Add(this.labelWhiteRooksTaken);
            this.Controls.Add(this.labelWhiteQueensTaken);
            this.Controls.Add(this.labelBlackRooksTaken);
            this.Controls.Add(this.labelBlackBishopsTaken);
            this.Controls.Add(this.labelBlackKnightsTaken);
            this.Controls.Add(this.labelBlackPawnsTaken);
            this.Controls.Add(this.pictureBox10);
            this.Controls.Add(this.pictureBox9);
            this.Controls.Add(this.pictureBox8);
            this.Controls.Add(this.pictureBox7);
            this.Controls.Add(this.pictureBox6);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ButtonCurrentMove);
            this.Controls.Add(this.ButtonForwardOne);
            this.Controls.Add(this.ButtonBackOne);
            this.Controls.Add(this.ButtonStartOfGame);
            this.Controls.Add(this.UpperTimeLabel);
            this.Controls.Add(this.OneMin);
            this.Controls.Add(this.FiveMin);
            this.Controls.Add(this.TenMin);
            this.Controls.Add(this.FifteenMin);
            this.Controls.Add(this.ThirtyMin);
            this.Controls.Add(this.OneHour);
            this.Controls.Add(this.ThreeHour);
            this.Controls.Add(this.LowerTimeLabel);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ButtonSendMessage);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxInput);
            this.Controls.Add(this.textBoxNotation);
            this.Controls.Add(this.textBoxChat);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Chess";
            this.Text = "Chess";
            this.Load += new System.EventHandler(this.Chess_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxChat;
        private System.Windows.Forms.TextBox textBoxNotation;
        private System.Windows.Forms.TextBox textBoxInput;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button ButtonSendMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Timer Timer1;
        private System.Windows.Forms.Label LowerTimeLabel;
        private System.Windows.Forms.RadioButton ThreeHour;
        private System.Windows.Forms.RadioButton OneHour;
        private System.Windows.Forms.RadioButton ThirtyMin;
        private System.Windows.Forms.RadioButton FifteenMin;
        private System.Windows.Forms.RadioButton TenMin;
        private System.Windows.Forms.RadioButton FiveMin;
        private System.Windows.Forms.RadioButton OneMin;
        private System.Windows.Forms.Label UpperTimeLabel;
        private System.Windows.Forms.Button ButtonStartOfGame;
        private System.Windows.Forms.Button ButtonBackOne;
        private System.Windows.Forms.Button ButtonForwardOne;
        private System.Windows.Forms.Button ButtonCurrentMove;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.PictureBox pictureBox9;
        private System.Windows.Forms.PictureBox pictureBox10;
        private System.Windows.Forms.Label labelBlackPawnsTaken;
        private System.Windows.Forms.Label labelBlackKnightsTaken;
        private System.Windows.Forms.Label labelBlackBishopsTaken;
        private System.Windows.Forms.Label labelBlackRooksTaken;
        private System.Windows.Forms.Label labelWhiteQueensTaken;
        private System.Windows.Forms.Label labelWhiteRooksTaken;
        private System.Windows.Forms.Label labelWhiteBishopsTaken;
        private System.Windows.Forms.Label labelWhiteKnightsTaken;
        private System.Windows.Forms.Label labelWhitePawnsTaken;
        private System.Windows.Forms.Label labelBlackQueensTaken;
    }
}

