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
            this.fiefManagementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.myFiefsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.travelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.navigateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.characterContainer = new System.Windows.Forms.SplitContainer();
            this.updateCharBtn = new System.Windows.Forms.Button();
            this.characterTextBox = new System.Windows.Forms.TextBox();
            this.fiefContainer = new System.Windows.Forms.SplitContainer();
            this.updateFiefBtn = new System.Windows.Forms.Button();
            this.adjustKeepSpendTextBox = new System.Windows.Forms.TextBox();
            this.adjustKeepSpendBtn = new System.Windows.Forms.Button();
            this.adjInfrSpendTextBox = new System.Windows.Forms.TextBox();
            this.adjInfrSpendBtn = new System.Windows.Forms.Button();
            this.adjGarrSpendTextBox = new System.Windows.Forms.TextBox();
            this.adjGarrSpendBtn = new System.Windows.Forms.Button();
            this.adjOffSpendTextBox = new System.Windows.Forms.TextBox();
            this.adjustTaxTextBox = new System.Windows.Forms.TextBox();
            this.adjustTaxButton = new System.Windows.Forms.Button();
            this.adjOffSpendBtn = new System.Windows.Forms.Button();
            this.fiefTextBox = new System.Windows.Forms.TextBox();
            this.travelContainer = new System.Windows.Forms.SplitContainer();
            this.travelNavigationPanel = new System.Windows.Forms.Panel();
            this.travel_SE_btn = new System.Windows.Forms.Button();
            this.travel_SW_btn = new System.Windows.Forms.Button();
            this.travel_E_btn = new System.Windows.Forms.Button();
            this.travel_Home_btn = new System.Windows.Forms.Button();
            this.travel_W_btn = new System.Windows.Forms.Button();
            this.travel_NE_btn = new System.Windows.Forms.Button();
            this.travel_NW_btn = new System.Windows.Forms.Button();
            this.fiefsOwnedContainer = new System.Windows.Forms.SplitContainer();
            this.fiefsListView = new System.Windows.Forms.ListView();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.characterContainer)).BeginInit();
            this.characterContainer.Panel1.SuspendLayout();
            this.characterContainer.Panel2.SuspendLayout();
            this.characterContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fiefContainer)).BeginInit();
            this.fiefContainer.Panel1.SuspendLayout();
            this.fiefContainer.Panel2.SuspendLayout();
            this.fiefContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.travelContainer)).BeginInit();
            this.travelContainer.Panel2.SuspendLayout();
            this.travelContainer.SuspendLayout();
            this.travelNavigationPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fiefsOwnedContainer)).BeginInit();
            this.fiefsOwnedContainer.Panel2.SuspendLayout();
            this.fiefsOwnedContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.characterToolStripMenuItem,
            this.fiefToolStripMenuItem,
            this.travelToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(727, 24);
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
            this.fiefManagementToolStripMenuItem,
            this.myFiefsToolStripMenuItem});
            this.fiefToolStripMenuItem.Name = "fiefToolStripMenuItem";
            this.fiefToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.fiefToolStripMenuItem.Text = "Fief";
            // 
            // fiefManagementToolStripMenuItem
            // 
            this.fiefManagementToolStripMenuItem.Name = "fiefManagementToolStripMenuItem";
            this.fiefManagementToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.fiefManagementToolStripMenuItem.Text = "Current fief";
            this.fiefManagementToolStripMenuItem.Click += new System.EventHandler(this.fiefManagementToolStripMenuItem_Click);
            // 
            // myFiefsToolStripMenuItem
            // 
            this.myFiefsToolStripMenuItem.Name = "myFiefsToolStripMenuItem";
            this.myFiefsToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.myFiefsToolStripMenuItem.Text = "My fiefs";
            this.myFiefsToolStripMenuItem.Click += new System.EventHandler(this.myFiefsToolStripMenuItem_Click);
            // 
            // travelToolStripMenuItem
            // 
            this.travelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navigateToolStripMenuItem});
            this.travelToolStripMenuItem.Name = "travelToolStripMenuItem";
            this.travelToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.travelToolStripMenuItem.Text = "Travel";
            // 
            // navigateToolStripMenuItem
            // 
            this.navigateToolStripMenuItem.Name = "navigateToolStripMenuItem";
            this.navigateToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.navigateToolStripMenuItem.Text = "Navigate";
            this.navigateToolStripMenuItem.Click += new System.EventHandler(this.navigateToolStripMenuItem_Click);
            // 
            // characterContainer
            // 
            this.characterContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.characterContainer.Location = new System.Drawing.Point(0, 24);
            this.characterContainer.Name = "characterContainer";
            // 
            // characterContainer.Panel1
            // 
            this.characterContainer.Panel1.Controls.Add(this.updateCharBtn);
            // 
            // characterContainer.Panel2
            // 
            this.characterContainer.Panel2.Controls.Add(this.characterTextBox);
            this.characterContainer.Size = new System.Drawing.Size(727, 434);
            this.characterContainer.SplitterDistance = 240;
            this.characterContainer.TabIndex = 2;
            // 
            // updateCharBtn
            // 
            this.updateCharBtn.Location = new System.Drawing.Point(13, 4);
            this.updateCharBtn.Name = "updateCharBtn";
            this.updateCharBtn.Size = new System.Drawing.Size(101, 23);
            this.updateCharBtn.TabIndex = 0;
            this.updateCharBtn.Text = "Update Character";
            this.updateCharBtn.UseVisualStyleBackColor = true;
            this.updateCharBtn.Click += new System.EventHandler(this.updateCharacter_Click);
            // 
            // characterTextBox
            // 
            this.characterTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.characterTextBox.Location = new System.Drawing.Point(0, 0);
            this.characterTextBox.Multiline = true;
            this.characterTextBox.Name = "characterTextBox";
            this.characterTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.characterTextBox.Size = new System.Drawing.Size(483, 434);
            this.characterTextBox.TabIndex = 0;
            // 
            // fiefContainer
            // 
            this.fiefContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fiefContainer.Location = new System.Drawing.Point(0, 24);
            this.fiefContainer.Name = "fiefContainer";
            // 
            // fiefContainer.Panel1
            // 
            this.fiefContainer.Panel1.Controls.Add(this.updateFiefBtn);
            this.fiefContainer.Panel1.Controls.Add(this.adjustKeepSpendTextBox);
            this.fiefContainer.Panel1.Controls.Add(this.adjustKeepSpendBtn);
            this.fiefContainer.Panel1.Controls.Add(this.adjInfrSpendTextBox);
            this.fiefContainer.Panel1.Controls.Add(this.adjInfrSpendBtn);
            this.fiefContainer.Panel1.Controls.Add(this.adjGarrSpendTextBox);
            this.fiefContainer.Panel1.Controls.Add(this.adjGarrSpendBtn);
            this.fiefContainer.Panel1.Controls.Add(this.adjOffSpendTextBox);
            this.fiefContainer.Panel1.Controls.Add(this.adjustTaxTextBox);
            this.fiefContainer.Panel1.Controls.Add(this.adjustTaxButton);
            this.fiefContainer.Panel1.Controls.Add(this.adjOffSpendBtn);
            // 
            // fiefContainer.Panel2
            // 
            this.fiefContainer.Panel2.Controls.Add(this.fiefTextBox);
            this.fiefContainer.Size = new System.Drawing.Size(727, 434);
            this.fiefContainer.SplitterDistance = 288;
            this.fiefContainer.TabIndex = 3;
            // 
            // updateFiefBtn
            // 
            this.updateFiefBtn.Location = new System.Drawing.Point(13, 168);
            this.updateFiefBtn.Name = "updateFiefBtn";
            this.updateFiefBtn.Size = new System.Drawing.Size(122, 23);
            this.updateFiefBtn.TabIndex = 10;
            this.updateFiefBtn.Text = "Update Fief";
            this.updateFiefBtn.UseVisualStyleBackColor = true;
            this.updateFiefBtn.Click += new System.EventHandler(this.updateFiefBtn_Click);
            // 
            // adjustKeepSpendTextBox
            // 
            this.adjustKeepSpendTextBox.Location = new System.Drawing.Point(144, 138);
            this.adjustKeepSpendTextBox.Name = "adjustKeepSpendTextBox";
            this.adjustKeepSpendTextBox.Size = new System.Drawing.Size(55, 20);
            this.adjustKeepSpendTextBox.TabIndex = 9;
            // 
            // adjustKeepSpendBtn
            // 
            this.adjustKeepSpendBtn.Location = new System.Drawing.Point(12, 138);
            this.adjustKeepSpendBtn.Name = "adjustKeepSpendBtn";
            this.adjustKeepSpendBtn.Size = new System.Drawing.Size(123, 23);
            this.adjustKeepSpendBtn.TabIndex = 8;
            this.adjustKeepSpendBtn.Text = "Adjust Keep Spend";
            this.adjustKeepSpendBtn.UseVisualStyleBackColor = true;
            this.adjustKeepSpendBtn.Click += new System.EventHandler(this.adjustKeepSpendBtn_Click);
            // 
            // adjInfrSpendTextBox
            // 
            this.adjInfrSpendTextBox.Location = new System.Drawing.Point(143, 93);
            this.adjInfrSpendTextBox.Name = "adjInfrSpendTextBox";
            this.adjInfrSpendTextBox.Size = new System.Drawing.Size(57, 20);
            this.adjInfrSpendTextBox.TabIndex = 7;
            // 
            // adjInfrSpendBtn
            // 
            this.adjInfrSpendBtn.Location = new System.Drawing.Point(13, 93);
            this.adjInfrSpendBtn.Name = "adjInfrSpendBtn";
            this.adjInfrSpendBtn.Size = new System.Drawing.Size(124, 39);
            this.adjInfrSpendBtn.TabIndex = 6;
            this.adjInfrSpendBtn.Text = "Adjust Infrastructure Spend";
            this.adjInfrSpendBtn.UseVisualStyleBackColor = true;
            this.adjInfrSpendBtn.Click += new System.EventHandler(this.adjInfrSpendBtn_Click);
            // 
            // adjGarrSpendTextBox
            // 
            this.adjGarrSpendTextBox.Location = new System.Drawing.Point(144, 63);
            this.adjGarrSpendTextBox.Name = "adjGarrSpendTextBox";
            this.adjGarrSpendTextBox.Size = new System.Drawing.Size(56, 20);
            this.adjGarrSpendTextBox.TabIndex = 5;
            // 
            // adjGarrSpendBtn
            // 
            this.adjGarrSpendBtn.Location = new System.Drawing.Point(13, 63);
            this.adjGarrSpendBtn.Name = "adjGarrSpendBtn";
            this.adjGarrSpendBtn.Size = new System.Drawing.Size(124, 23);
            this.adjGarrSpendBtn.TabIndex = 4;
            this.adjGarrSpendBtn.Text = "Adjust Garrison Spend";
            this.adjGarrSpendBtn.UseVisualStyleBackColor = true;
            this.adjGarrSpendBtn.Click += new System.EventHandler(this.adjGarrSpendBtn_Click);
            // 
            // adjOffSpendTextBox
            // 
            this.adjOffSpendTextBox.Location = new System.Drawing.Point(144, 33);
            this.adjOffSpendTextBox.Name = "adjOffSpendTextBox";
            this.adjOffSpendTextBox.Size = new System.Drawing.Size(56, 20);
            this.adjOffSpendTextBox.TabIndex = 3;
            // 
            // adjustTaxTextBox
            // 
            this.adjustTaxTextBox.Location = new System.Drawing.Point(143, 6);
            this.adjustTaxTextBox.Name = "adjustTaxTextBox";
            this.adjustTaxTextBox.Size = new System.Drawing.Size(57, 20);
            this.adjustTaxTextBox.TabIndex = 2;
            // 
            // adjustTaxButton
            // 
            this.adjustTaxButton.Location = new System.Drawing.Point(12, 4);
            this.adjustTaxButton.Name = "adjustTaxButton";
            this.adjustTaxButton.Size = new System.Drawing.Size(125, 23);
            this.adjustTaxButton.TabIndex = 1;
            this.adjustTaxButton.Text = "Adjust Tax Rate";
            this.adjustTaxButton.UseVisualStyleBackColor = true;
            this.adjustTaxButton.Click += new System.EventHandler(this.adjustTaxButton_Click);
            // 
            // adjOffSpendBtn
            // 
            this.adjOffSpendBtn.Location = new System.Drawing.Point(13, 33);
            this.adjOffSpendBtn.Name = "adjOffSpendBtn";
            this.adjOffSpendBtn.Size = new System.Drawing.Size(124, 23);
            this.adjOffSpendBtn.TabIndex = 0;
            this.adjOffSpendBtn.Text = "Adjust Officials Spend";
            this.adjOffSpendBtn.UseVisualStyleBackColor = true;
            this.adjOffSpendBtn.Click += new System.EventHandler(this.adjOffSpend_Click);
            // 
            // fiefTextBox
            // 
            this.fiefTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fiefTextBox.Location = new System.Drawing.Point(0, 0);
            this.fiefTextBox.Multiline = true;
            this.fiefTextBox.Name = "fiefTextBox";
            this.fiefTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.fiefTextBox.Size = new System.Drawing.Size(435, 434);
            this.fiefTextBox.TabIndex = 0;
            // 
            // travelContainer
            // 
            this.travelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.travelContainer.Location = new System.Drawing.Point(0, 24);
            this.travelContainer.Name = "travelContainer";
            // 
            // travelContainer.Panel2
            // 
            this.travelContainer.Panel2.Controls.Add(this.travelNavigationPanel);
            this.travelContainer.Size = new System.Drawing.Size(727, 434);
            this.travelContainer.SplitterDistance = 242;
            this.travelContainer.TabIndex = 4;
            // 
            // travelNavigationPanel
            // 
            this.travelNavigationPanel.Controls.Add(this.travel_SE_btn);
            this.travelNavigationPanel.Controls.Add(this.travel_SW_btn);
            this.travelNavigationPanel.Controls.Add(this.travel_E_btn);
            this.travelNavigationPanel.Controls.Add(this.travel_Home_btn);
            this.travelNavigationPanel.Controls.Add(this.travel_W_btn);
            this.travelNavigationPanel.Controls.Add(this.travel_NE_btn);
            this.travelNavigationPanel.Controls.Add(this.travel_NW_btn);
            this.travelNavigationPanel.Location = new System.Drawing.Point(3, 6);
            this.travelNavigationPanel.Name = "travelNavigationPanel";
            this.travelNavigationPanel.Size = new System.Drawing.Size(475, 416);
            this.travelNavigationPanel.TabIndex = 0;
            // 
            // travel_SE_btn
            // 
            this.travel_SE_btn.Location = new System.Drawing.Point(231, 265);
            this.travel_SE_btn.Name = "travel_SE_btn";
            this.travel_SE_btn.Size = new System.Drawing.Size(137, 123);
            this.travel_SE_btn.TabIndex = 6;
            this.travel_SE_btn.Text = "Go SE";
            this.travel_SE_btn.UseVisualStyleBackColor = true;
            // 
            // travel_SW_btn
            // 
            this.travel_SW_btn.Location = new System.Drawing.Point(88, 265);
            this.travel_SW_btn.Name = "travel_SW_btn";
            this.travel_SW_btn.Size = new System.Drawing.Size(137, 123);
            this.travel_SW_btn.TabIndex = 5;
            this.travel_SW_btn.Text = "Go SW";
            this.travel_SW_btn.UseVisualStyleBackColor = true;
            // 
            // travel_E_btn
            // 
            this.travel_E_btn.Location = new System.Drawing.Point(299, 136);
            this.travel_E_btn.Name = "travel_E_btn";
            this.travel_E_btn.Size = new System.Drawing.Size(137, 123);
            this.travel_E_btn.TabIndex = 4;
            this.travel_E_btn.Text = "Go E";
            this.travel_E_btn.UseVisualStyleBackColor = true;
            this.travel_E_btn.Click += new System.EventHandler(this.travel_E_btn_Click);
            // 
            // travel_Home_btn
            // 
            this.travel_Home_btn.Location = new System.Drawing.Point(156, 136);
            this.travel_Home_btn.Name = "travel_Home_btn";
            this.travel_Home_btn.Size = new System.Drawing.Size(137, 123);
            this.travel_Home_btn.TabIndex = 3;
            this.travel_Home_btn.Text = "You are here";
            this.travel_Home_btn.UseVisualStyleBackColor = true;
            // 
            // travel_W_btn
            // 
            this.travel_W_btn.Location = new System.Drawing.Point(13, 136);
            this.travel_W_btn.Name = "travel_W_btn";
            this.travel_W_btn.Size = new System.Drawing.Size(137, 123);
            this.travel_W_btn.TabIndex = 2;
            this.travel_W_btn.Text = "Go W";
            this.travel_W_btn.UseVisualStyleBackColor = true;
            this.travel_W_btn.Click += new System.EventHandler(this.travel_W_btn_Click);
            // 
            // travel_NE_btn
            // 
            this.travel_NE_btn.Location = new System.Drawing.Point(231, 7);
            this.travel_NE_btn.Name = "travel_NE_btn";
            this.travel_NE_btn.Size = new System.Drawing.Size(137, 123);
            this.travel_NE_btn.TabIndex = 1;
            this.travel_NE_btn.Text = "Go NE";
            this.travel_NE_btn.UseVisualStyleBackColor = true;
            // 
            // travel_NW_btn
            // 
            this.travel_NW_btn.Location = new System.Drawing.Point(88, 7);
            this.travel_NW_btn.Name = "travel_NW_btn";
            this.travel_NW_btn.Size = new System.Drawing.Size(137, 123);
            this.travel_NW_btn.TabIndex = 0;
            this.travel_NW_btn.Text = "Go NW";
            this.travel_NW_btn.UseVisualStyleBackColor = true;
            // 
            // fiefsOwnedContainer
            // 
            this.fiefsOwnedContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fiefsOwnedContainer.Location = new System.Drawing.Point(0, 24);
            this.fiefsOwnedContainer.Name = "fiefsOwnedContainer";
            // 
            // fiefsOwnedContainer.Panel2
            // 
            this.fiefsOwnedContainer.Panel2.Controls.Add(this.fiefsListView);
            this.fiefsOwnedContainer.Size = new System.Drawing.Size(727, 434);
            this.fiefsOwnedContainer.SplitterDistance = 242;
            this.fiefsOwnedContainer.TabIndex = 5;
            // 
            // fiefsListView
            // 
            this.fiefsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fiefsListView.FullRowSelect = true;
            this.fiefsListView.GridLines = true;
            this.fiefsListView.Location = new System.Drawing.Point(0, 0);
            this.fiefsListView.Name = "fiefsListView";
            this.fiefsListView.Size = new System.Drawing.Size(481, 434);
            this.fiefsListView.TabIndex = 0;
            this.fiefsListView.UseCompatibleStateImageBehavior = false;
            this.fiefsListView.View = System.Windows.Forms.View.Details;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(727, 458);
            this.Controls.Add(this.travelContainer);
            this.Controls.Add(this.fiefsOwnedContainer);
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
            this.fiefContainer.Panel1.ResumeLayout(false);
            this.fiefContainer.Panel1.PerformLayout();
            this.fiefContainer.Panel2.ResumeLayout(false);
            this.fiefContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fiefContainer)).EndInit();
            this.fiefContainer.ResumeLayout(false);
            this.travelContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.travelContainer)).EndInit();
            this.travelContainer.ResumeLayout(false);
            this.travelNavigationPanel.ResumeLayout(false);
            this.fiefsOwnedContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fiefsOwnedContainer)).EndInit();
            this.fiefsOwnedContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem characterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fiefToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem personalCharacteristicsAndAffairsToolStripMenuItem;
        private System.Windows.Forms.SplitContainer characterContainer;
        private System.Windows.Forms.Button updateCharBtn;
        private System.Windows.Forms.TextBox characterTextBox;
        private System.Windows.Forms.SplitContainer fiefContainer;
        private System.Windows.Forms.ToolStripMenuItem fiefManagementToolStripMenuItem;
        private System.Windows.Forms.TextBox fiefTextBox;
        private System.Windows.Forms.Button adjOffSpendBtn;
        private System.Windows.Forms.Button adjustTaxButton;
        private System.Windows.Forms.TextBox adjustTaxTextBox;
        private System.Windows.Forms.TextBox adjOffSpendTextBox;
        private System.Windows.Forms.TextBox adjGarrSpendTextBox;
        private System.Windows.Forms.Button adjGarrSpendBtn;
        private System.Windows.Forms.TextBox adjInfrSpendTextBox;
        private System.Windows.Forms.Button adjInfrSpendBtn;
        private System.Windows.Forms.TextBox adjustKeepSpendTextBox;
        private System.Windows.Forms.Button adjustKeepSpendBtn;
        private System.Windows.Forms.Button updateFiefBtn;
        private System.Windows.Forms.ToolStripMenuItem travelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem navigateToolStripMenuItem;
        private System.Windows.Forms.SplitContainer travelContainer;
        private System.Windows.Forms.Panel travelNavigationPanel;
        private System.Windows.Forms.Button travel_SE_btn;
        private System.Windows.Forms.Button travel_SW_btn;
        private System.Windows.Forms.Button travel_E_btn;
        private System.Windows.Forms.Button travel_Home_btn;
        private System.Windows.Forms.Button travel_W_btn;
        private System.Windows.Forms.Button travel_NE_btn;
        private System.Windows.Forms.Button travel_NW_btn;
        private System.Windows.Forms.ToolStripMenuItem myFiefsToolStripMenuItem;
        private System.Windows.Forms.SplitContainer fiefsOwnedContainer;
        private System.Windows.Forms.ListView fiefsListView;

    }
}

