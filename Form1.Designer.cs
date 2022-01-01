
namespace CollidingBalls
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
            this.label_score = new System.Windows.Forms.Label();
            this.playBtn = new System.Windows.Forms.Button();
            this.pauseBtn = new System.Windows.Forms.Button();
            this.backupBtn = new System.Windows.Forms.Button();
            this.restoreBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label_score
            // 
            this.label_score.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_score.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_score.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label_score.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label_score.Location = new System.Drawing.Point(1047, 9);
            this.label_score.Margin = new System.Windows.Forms.Padding(0);
            this.label_score.Name = "label_score";
            this.label_score.Size = new System.Drawing.Size(140, 30);
            this.label_score.TabIndex = 0;
            this.label_score.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // playBtn
            // 
            this.playBtn.Location = new System.Drawing.Point(1028, 680);
            this.playBtn.Name = "playBtn";
            this.playBtn.Size = new System.Drawing.Size(75, 23);
            this.playBtn.TabIndex = 1;
            this.playBtn.Text = "Play";
            this.playBtn.UseVisualStyleBackColor = true;
            this.playBtn.Click += new System.EventHandler(this.playBtn_Click);
            // 
            // pauseBtn
            // 
            this.pauseBtn.Location = new System.Drawing.Point(1109, 680);
            this.pauseBtn.Name = "pauseBtn";
            this.pauseBtn.Size = new System.Drawing.Size(75, 23);
            this.pauseBtn.TabIndex = 2;
            this.pauseBtn.Text = "Pause";
            this.pauseBtn.UseVisualStyleBackColor = true;
            this.pauseBtn.Click += new System.EventHandler(this.pauseBtn_Click);
            // 
            // backupBtn
            // 
            this.backupBtn.Location = new System.Drawing.Point(13, 679);
            this.backupBtn.Name = "backupBtn";
            this.backupBtn.Size = new System.Drawing.Size(75, 23);
            this.backupBtn.TabIndex = 3;
            this.backupBtn.Text = "Backup(S)";
            this.backupBtn.UseVisualStyleBackColor = true;
            this.backupBtn.Click += new System.EventHandler(this.backupBtn_Click);
            // 
            // restoreBtn
            // 
            this.restoreBtn.Location = new System.Drawing.Point(94, 679);
            this.restoreBtn.Name = "restoreBtn";
            this.restoreBtn.Size = new System.Drawing.Size(75, 23);
            this.restoreBtn.TabIndex = 4;
            this.restoreBtn.Text = "Restore(R)";
            this.restoreBtn.UseVisualStyleBackColor = true;
            this.restoreBtn.Click += new System.EventHandler(this.restoreBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1196, 715);
            this.Controls.Add(this.restoreBtn);
            this.Controls.Add(this.backupBtn);
            this.Controls.Add(this.pauseBtn);
            this.Controls.Add(this.playBtn);
            this.Controls.Add(this.label_score);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawObjects);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Controller);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label_score;
        private System.Windows.Forms.Button playBtn;
        private System.Windows.Forms.Button pauseBtn;
        private System.Windows.Forms.Button backupBtn;
        private System.Windows.Forms.Button restoreBtn;
    }
}

