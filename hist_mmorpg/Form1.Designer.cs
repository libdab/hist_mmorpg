using System.Collections.Generic;

namespace hist_mmorpg
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.characterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.personalCharacteristicsAndAffairsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fiefToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.characterContainer = new System.Windows.Forms.SplitContainer();
            this.checkDeath = new System.Windows.Forms.Button();
            this.characterTextBox = new System.Windows.Forms.TextBox();
            this.fiefContainer = new System.Windows.Forms.SplitContainer();
            this.fiefTextBox = new System.Windows.Forms.TextBox();
            this.fiefManagementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.characterContainer)).BeginInit();
            this.characterContainer.Panel1.SuspendLayout();
            this.characterContainer.Panel2.SuspendLayout();
            this.characterContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fiefContainer)).BeginInit();
            this.fiefContainer.Panel2.SuspendLayout();
            this.fiefContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.characterToolStripMenuItem,
            this.fiefToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(428, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // characterToolStripMenuItem
            // 
            this.characterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.personalCharacteristicsAndAffairsToolStripMenuItem});
            this.characterToolStripMenuItem.Name = "characterToolStripMenuItem";
            this.characterToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.characterToolStripMenuItem.Text = "Character";
            // 
            // personalCharacteristicsAndAffairsToolStripMenuItem
            // 
            this.personalCharacteristicsAndAffairsToolStripMenuItem.Name = "personalCharacteristicsAndAffairsToolStripMenuItem";
            this.personalCharacteristicsAndAffairsToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.personalCharacteristicsAndAffairsToolStripMenuItem.Text = "Personal characteristics and affairs";
            this.personalCharacteristicsAndAffairsToolStripMenuItem.Click += new System.EventHandler(this.personalCharacteristicsAndAffairsToolStripMenuItem_Click);
            // 
            // fiefToolStripMenuItem
            // 
            this.fiefToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fiefManagementToolStripMenuItem});
            this.fiefToolStripMenuItem.Name = "fiefToolStripMenuItem";
            this.fiefToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.fiefToolStripMenuItem.Text = "Fief";
            // 
            // characterContainer
            // 
            this.characterContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.characterContainer.Location = new System.Drawing.Point(0, 24);
            this.characterContainer.Name = "characterContainer";
            // 
            // characterContainer.Panel1
            // 
            this.characterContainer.Panel1.Controls.Add(this.checkDeath);
            // 
            // characterContainer.Panel2
            // 
            this.characterContainer.Panel2.Controls.Add(this.characterTextBox);
            this.characterContainer.Size = new System.Drawing.Size(428, 358);
            this.characterContainer.SplitterDistance = 142;
            this.characterContainer.TabIndex = 2;
            // 
            // checkDeath
            // 
            this.checkDeath.Location = new System.Drawing.Point(13, 4);
            this.checkDeath.Name = "checkDeath";
            this.checkDeath.Size = new System.Drawing.Size(101, 23);
            this.checkDeath.TabIndex = 0;
            this.checkDeath.Text = "Check for death!";
            this.checkDeath.UseVisualStyleBackColor = true;
            // 
            // characterTextBox
            // 
            this.characterTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.characterTextBox.Location = new System.Drawing.Point(0, 0);
            this.characterTextBox.Multiline = true;
            this.characterTextBox.Name = "characterTextBox";
            this.characterTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.characterTextBox.Size = new System.Drawing.Size(282, 358);
            this.characterTextBox.TabIndex = 0;
            // 
            // fiefContainer
            // 
            this.fiefContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fiefContainer.Location = new System.Drawing.Point(0, 24);
            this.fiefContainer.Name = "fiefContainer";
            // 
            // fiefContainer.Panel2
            // 
            this.fiefContainer.Panel2.Controls.Add(this.fiefTextBox);
            this.fiefContainer.Size = new System.Drawing.Size(428, 358);
            this.fiefContainer.SplitterDistance = 142;
            this.fiefContainer.TabIndex = 3;
            // 
            // fiefTextBox
            // 
            this.fiefTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fiefTextBox.Location = new System.Drawing.Point(0, 0);
            this.fiefTextBox.Multiline = true;
            this.fiefTextBox.Name = "fiefTextBox";
            this.fiefTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.fiefTextBox.Size = new System.Drawing.Size(282, 358);
            this.fiefTextBox.TabIndex = 0;
            // 
            // fiefManagementToolStripMenuItem
            // 
            this.fiefManagementToolStripMenuItem.Name = "fiefManagementToolStripMenuItem";
            this.fiefManagementToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.fiefManagementToolStripMenuItem.Text = "Fief management";
            this.fiefManagementToolStripMenuItem.Click += new System.EventHandler(this.fiefManagementToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 382);
            this.Controls.Add(this.fiefContainer);
            this.Controls.Add(this.characterContainer);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.characterContainer.Panel1.ResumeLayout(false);
            this.characterContainer.Panel2.ResumeLayout(false);
            this.characterContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.characterContainer)).EndInit();
            this.characterContainer.ResumeLayout(false);
            this.fiefContainer.Panel2.ResumeLayout(false);
            this.fiefContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fiefContainer)).EndInit();
            this.fiefContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem characterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fiefToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem personalCharacteristicsAndAffairsToolStripMenuItem;
        private System.Windows.Forms.SplitContainer characterContainer;
        private System.Windows.Forms.Button checkDeath;
        private System.Windows.Forms.TextBox characterTextBox;
        private System.Windows.Forms.SplitContainer fiefContainer;
        private System.Windows.Forms.ToolStripMenuItem fiefManagementToolStripMenuItem;
        private System.Windows.Forms.TextBox fiefTextBox;

    }
}

