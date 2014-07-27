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
            this.charMultiMoveTextBox = new System.Windows.Forms.TextBox();
            this.charMultiMoveBtn = new System.Windows.Forms.Button();
            this.updateCharBtn = new System.Windows.Forms.Button();
            this.characterTextBox = new System.Windows.Forms.TextBox();
            this.fiefContainer = new System.Windows.Forms.SplitContainer();
            this.setBailiffBtn = new System.Windows.Forms.Button();
            this.keepSpendLabel = new System.Windows.Forms.Label();
            this.infraSpendLabel = new System.Windows.Forms.Label();
            this.garrSpendLabel = new System.Windows.Forms.Label();
            this.offSpendLabel = new System.Windows.Forms.Label();
            this.taxRateLabel = new System.Windows.Forms.Label();
            this.viewBailiffBtn = new System.Windows.Forms.Button();
            this.updateFiefBtn = new System.Windows.Forms.Button();
            this.adjustKeepSpendTextBox = new System.Windows.Forms.TextBox();
            this.adjInfrSpendTextBox = new System.Windows.Forms.TextBox();
            this.adjGarrSpendTextBox = new System.Windows.Forms.TextBox();
            this.adjOffSpendTextBox = new System.Windows.Forms.TextBox();
            this.adjustTaxTextBox = new System.Windows.Forms.TextBox();
            this.adjustSpendBtn = new System.Windows.Forms.Button();
            this.fiefTextBox = new System.Windows.Forms.TextBox();
            this.travelContainer = new System.Windows.Forms.SplitContainer();
            this.visitTavernBtn = new System.Windows.Forms.Button();
            this.visitCourtBtn1 = new System.Windows.Forms.Button();
            this.enterKeepBtn = new System.Windows.Forms.Button();
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
            this.meetingPlaceContainer = new System.Windows.Forms.SplitContainer();
            this.hireNPC_TextBox = new System.Windows.Forms.TextBox();
            this.hireNPC_Btn = new System.Windows.Forms.Button();
            this.meetingPlaceCharsContainer = new System.Windows.Forms.SplitContainer();
            this.meetingPlaceCharDisplayTextBox = new System.Windows.Forms.TextBox();
            this.meetingPlaceCharsListContainer = new System.Windows.Forms.SplitContainer();
            this.meetingPlaceTextBox = new System.Windows.Forms.TextBox();
            this.meetingPlaceCharsListView = new System.Windows.Forms.ListView();
            this.removeBaliffBtn = new System.Windows.Forms.Button();
            this.selfBailiffBtn = new System.Windows.Forms.Button();
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
            this.travelContainer.Panel1.SuspendLayout();
            this.travelContainer.Panel2.SuspendLayout();
            this.travelContainer.SuspendLayout();
            this.travelNavigationPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fiefsOwnedContainer)).BeginInit();
            this.fiefsOwnedContainer.Panel2.SuspendLayout();
            this.fiefsOwnedContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.meetingPlaceContainer)).BeginInit();
            this.meetingPlaceContainer.Panel1.SuspendLayout();
            this.meetingPlaceContainer.Panel2.SuspendLayout();
            this.meetingPlaceContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.meetingPlaceCharsContainer)).BeginInit();
            this.meetingPlaceCharsContainer.Panel1.SuspendLayout();
            this.meetingPlaceCharsContainer.Panel2.SuspendLayout();
            this.meetingPlaceCharsContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.meetingPlaceCharsListContainer)).BeginInit();
            this.meetingPlaceCharsListContainer.Panel1.SuspendLayout();
            this.meetingPlaceCharsListContainer.Panel2.SuspendLayout();
            this.meetingPlaceCharsListContainer.SuspendLayout();
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
            this.menuStrip1.Size = new System.Drawing.Size(801, 24);
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
            this.fiefManagementToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.fiefManagementToolStripMenuItem.Text = "Current fief";
            this.fiefManagementToolStripMenuItem.Click += new System.EventHandler(this.fiefManagementToolStripMenuItem_Click);
            // 
            // myFiefsToolStripMenuItem
            // 
            this.myFiefsToolStripMenuItem.Name = "myFiefsToolStripMenuItem";
            this.myFiefsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.myFiefsToolStripMenuItem.Text = "Manage my fiefs";
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
            this.characterContainer.Panel1.Controls.Add(this.charMultiMoveTextBox);
            this.characterContainer.Panel1.Controls.Add(this.charMultiMoveBtn);
            this.characterContainer.Panel1.Controls.Add(this.updateCharBtn);
            // 
            // characterContainer.Panel2
            // 
            this.characterContainer.Panel2.Controls.Add(this.characterTextBox);
            this.characterContainer.Size = new System.Drawing.Size(801, 511);
            this.characterContainer.SplitterDistance = 264;
            this.characterContainer.TabIndex = 2;
            // 
            // charMultiMoveTextBox
            // 
            this.charMultiMoveTextBox.Location = new System.Drawing.Point(119, 33);
            this.charMultiMoveTextBox.Name = "charMultiMoveTextBox";
            this.charMultiMoveTextBox.Size = new System.Drawing.Size(93, 20);
            this.charMultiMoveTextBox.TabIndex = 2;
            // 
            // charMultiMoveBtn
            // 
            this.charMultiMoveBtn.Location = new System.Drawing.Point(15, 33);
            this.charMultiMoveBtn.Name = "charMultiMoveBtn";
            this.charMultiMoveBtn.Size = new System.Drawing.Size(98, 23);
            this.charMultiMoveBtn.TabIndex = 1;
            this.charMultiMoveBtn.Text = "Move to ...";
            this.charMultiMoveBtn.UseVisualStyleBackColor = true;
            this.charMultiMoveBtn.Click += new System.EventHandler(this.charMultiMoveBtn_Click);
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
            this.characterTextBox.Size = new System.Drawing.Size(533, 511);
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
            this.fiefContainer.Panel1.Controls.Add(this.selfBailiffBtn);
            this.fiefContainer.Panel1.Controls.Add(this.removeBaliffBtn);
            this.fiefContainer.Panel1.Controls.Add(this.setBailiffBtn);
            this.fiefContainer.Panel1.Controls.Add(this.keepSpendLabel);
            this.fiefContainer.Panel1.Controls.Add(this.infraSpendLabel);
            this.fiefContainer.Panel1.Controls.Add(this.garrSpendLabel);
            this.fiefContainer.Panel1.Controls.Add(this.offSpendLabel);
            this.fiefContainer.Panel1.Controls.Add(this.taxRateLabel);
            this.fiefContainer.Panel1.Controls.Add(this.viewBailiffBtn);
            this.fiefContainer.Panel1.Controls.Add(this.updateFiefBtn);
            this.fiefContainer.Panel1.Controls.Add(this.adjustKeepSpendTextBox);
            this.fiefContainer.Panel1.Controls.Add(this.adjInfrSpendTextBox);
            this.fiefContainer.Panel1.Controls.Add(this.adjGarrSpendTextBox);
            this.fiefContainer.Panel1.Controls.Add(this.adjOffSpendTextBox);
            this.fiefContainer.Panel1.Controls.Add(this.adjustTaxTextBox);
            this.fiefContainer.Panel1.Controls.Add(this.adjustSpendBtn);
            // 
            // fiefContainer.Panel2
            // 
            this.fiefContainer.Panel2.Controls.Add(this.fiefTextBox);
            this.fiefContainer.Size = new System.Drawing.Size(801, 511);
            this.fiefContainer.SplitterDistance = 317;
            this.fiefContainer.TabIndex = 3;
            // 
            // setBailiffBtn
            // 
            this.setBailiffBtn.Location = new System.Drawing.Point(70, 285);
            this.setBailiffBtn.Name = "setBailiffBtn";
            this.setBailiffBtn.Size = new System.Drawing.Size(122, 23);
            this.setBailiffBtn.TabIndex = 17;
            this.setBailiffBtn.Text = "Appoint a Bailiff";
            this.setBailiffBtn.UseVisualStyleBackColor = true;
            this.setBailiffBtn.Click += new System.EventHandler(this.setBailiffBtn_Click);
            // 
            // keepSpendLabel
            // 
            this.keepSpendLabel.AutoSize = true;
            this.keepSpendLabel.Location = new System.Drawing.Point(77, 139);
            this.keepSpendLabel.Name = "keepSpendLabel";
            this.keepSpendLabel.Size = new System.Drawing.Size(100, 13);
            this.keepSpendLabel.TabIndex = 16;
            this.keepSpendLabel.Text = "Keep Expenditure : ";
            // 
            // infraSpendLabel
            // 
            this.infraSpendLabel.AutoSize = true;
            this.infraSpendLabel.Location = new System.Drawing.Point(40, 107);
            this.infraSpendLabel.Name = "infraSpendLabel";
            this.infraSpendLabel.Size = new System.Drawing.Size(137, 13);
            this.infraSpendLabel.TabIndex = 15;
            this.infraSpendLabel.Text = "Infrastructure Expenditure : ";
            // 
            // garrSpendLabel
            // 
            this.garrSpendLabel.AutoSize = true;
            this.garrSpendLabel.Location = new System.Drawing.Point(63, 77);
            this.garrSpendLabel.Name = "garrSpendLabel";
            this.garrSpendLabel.Size = new System.Drawing.Size(114, 13);
            this.garrSpendLabel.TabIndex = 14;
            this.garrSpendLabel.Text = "Garrison Expenditure : ";
            // 
            // offSpendLabel
            // 
            this.offSpendLabel.AutoSize = true;
            this.offSpendLabel.Location = new System.Drawing.Point(65, 48);
            this.offSpendLabel.Name = "offSpendLabel";
            this.offSpendLabel.Size = new System.Drawing.Size(112, 13);
            this.offSpendLabel.TabIndex = 13;
            this.offSpendLabel.Text = "Officials Expenditure : ";
            // 
            // taxRateLabel
            // 
            this.taxRateLabel.AutoSize = true;
            this.taxRateLabel.Location = new System.Drawing.Point(117, 19);
            this.taxRateLabel.Name = "taxRateLabel";
            this.taxRateLabel.Size = new System.Drawing.Size(60, 13);
            this.taxRateLabel.TabIndex = 12;
            this.taxRateLabel.Text = "Tax Rate : ";
            // 
            // viewBailiffBtn
            // 
            this.viewBailiffBtn.Location = new System.Drawing.Point(70, 227);
            this.viewBailiffBtn.Name = "viewBailiffBtn";
            this.viewBailiffBtn.Size = new System.Drawing.Size(122, 23);
            this.viewBailiffBtn.TabIndex = 11;
            this.viewBailiffBtn.Text = "View Bailiff";
            this.viewBailiffBtn.UseVisualStyleBackColor = true;
            this.viewBailiffBtn.Click += new System.EventHandler(this.viewBailiffBtn_Click);
            // 
            // updateFiefBtn
            // 
            this.updateFiefBtn.Location = new System.Drawing.Point(70, 256);
            this.updateFiefBtn.Name = "updateFiefBtn";
            this.updateFiefBtn.Size = new System.Drawing.Size(122, 23);
            this.updateFiefBtn.TabIndex = 10;
            this.updateFiefBtn.Text = "Update Fief";
            this.updateFiefBtn.UseVisualStyleBackColor = true;
            this.updateFiefBtn.Click += new System.EventHandler(this.updateFiefBtn_Click);
            // 
            // adjustKeepSpendTextBox
            // 
            this.adjustKeepSpendTextBox.Location = new System.Drawing.Point(187, 136);
            this.adjustKeepSpendTextBox.Name = "adjustKeepSpendTextBox";
            this.adjustKeepSpendTextBox.Size = new System.Drawing.Size(56, 20);
            this.adjustKeepSpendTextBox.TabIndex = 9;
            // 
            // adjInfrSpendTextBox
            // 
            this.adjInfrSpendTextBox.Location = new System.Drawing.Point(186, 104);
            this.adjInfrSpendTextBox.Name = "adjInfrSpendTextBox";
            this.adjInfrSpendTextBox.Size = new System.Drawing.Size(57, 20);
            this.adjInfrSpendTextBox.TabIndex = 7;
            // 
            // adjGarrSpendTextBox
            // 
            this.adjGarrSpendTextBox.Location = new System.Drawing.Point(187, 74);
            this.adjGarrSpendTextBox.Name = "adjGarrSpendTextBox";
            this.adjGarrSpendTextBox.Size = new System.Drawing.Size(56, 20);
            this.adjGarrSpendTextBox.TabIndex = 5;
            // 
            // adjOffSpendTextBox
            // 
            this.adjOffSpendTextBox.Location = new System.Drawing.Point(187, 44);
            this.adjOffSpendTextBox.Name = "adjOffSpendTextBox";
            this.adjOffSpendTextBox.Size = new System.Drawing.Size(56, 20);
            this.adjOffSpendTextBox.TabIndex = 3;
            // 
            // adjustTaxTextBox
            // 
            this.adjustTaxTextBox.Location = new System.Drawing.Point(187, 14);
            this.adjustTaxTextBox.Name = "adjustTaxTextBox";
            this.adjustTaxTextBox.Size = new System.Drawing.Size(57, 20);
            this.adjustTaxTextBox.TabIndex = 2;
            // 
            // adjustSpendBtn
            // 
            this.adjustSpendBtn.Location = new System.Drawing.Point(70, 185);
            this.adjustSpendBtn.Name = "adjustSpendBtn";
            this.adjustSpendBtn.Size = new System.Drawing.Size(122, 36);
            this.adjustSpendBtn.TabIndex = 0;
            this.adjustSpendBtn.Text = "Commit amounts above";
            this.adjustSpendBtn.UseVisualStyleBackColor = true;
            this.adjustSpendBtn.Click += new System.EventHandler(this.adjustSpendBtn_Click);
            // 
            // fiefTextBox
            // 
            this.fiefTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fiefTextBox.Location = new System.Drawing.Point(0, 0);
            this.fiefTextBox.Multiline = true;
            this.fiefTextBox.Name = "fiefTextBox";
            this.fiefTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.fiefTextBox.Size = new System.Drawing.Size(480, 511);
            this.fiefTextBox.TabIndex = 0;
            // 
            // travelContainer
            // 
            this.travelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.travelContainer.Location = new System.Drawing.Point(0, 24);
            this.travelContainer.Name = "travelContainer";
            // 
            // travelContainer.Panel1
            // 
            this.travelContainer.Panel1.Controls.Add(this.visitTavernBtn);
            this.travelContainer.Panel1.Controls.Add(this.visitCourtBtn1);
            this.travelContainer.Panel1.Controls.Add(this.enterKeepBtn);
            // 
            // travelContainer.Panel2
            // 
            this.travelContainer.Panel2.Controls.Add(this.travelNavigationPanel);
            this.travelContainer.Size = new System.Drawing.Size(801, 511);
            this.travelContainer.SplitterDistance = 266;
            this.travelContainer.TabIndex = 4;
            // 
            // visitTavernBtn
            // 
            this.visitTavernBtn.Location = new System.Drawing.Point(15, 72);
            this.visitTavernBtn.Name = "visitTavernBtn";
            this.visitTavernBtn.Size = new System.Drawing.Size(122, 23);
            this.visitTavernBtn.TabIndex = 2;
            this.visitTavernBtn.Text = "Visit Tavern";
            this.visitTavernBtn.UseVisualStyleBackColor = true;
            this.visitTavernBtn.Click += new System.EventHandler(this.visitTavernBtn_Click_1);
            // 
            // visitCourtBtn1
            // 
            this.visitCourtBtn1.Location = new System.Drawing.Point(15, 43);
            this.visitCourtBtn1.Name = "visitCourtBtn1";
            this.visitCourtBtn1.Size = new System.Drawing.Size(122, 23);
            this.visitCourtBtn1.TabIndex = 1;
            this.visitCourtBtn1.Text = "Visit Court";
            this.visitCourtBtn1.UseVisualStyleBackColor = true;
            this.visitCourtBtn1.Click += new System.EventHandler(this.visitCourtBtn1_Click);
            // 
            // enterKeepBtn
            // 
            this.enterKeepBtn.Location = new System.Drawing.Point(15, 13);
            this.enterKeepBtn.Name = "enterKeepBtn";
            this.enterKeepBtn.Size = new System.Drawing.Size(122, 23);
            this.enterKeepBtn.TabIndex = 0;
            this.enterKeepBtn.Text = "Enter Keep";
            this.enterKeepBtn.UseVisualStyleBackColor = true;
            this.enterKeepBtn.Click += new System.EventHandler(this.enterKeepBtn_Click);
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
            this.travel_SE_btn.Tag = "SE";
            this.travel_SE_btn.Text = "Go SE";
            this.travel_SE_btn.UseVisualStyleBackColor = true;
            this.travel_SE_btn.Click += new System.EventHandler(this.travelBtnClick);
            // 
            // travel_SW_btn
            // 
            this.travel_SW_btn.Location = new System.Drawing.Point(88, 265);
            this.travel_SW_btn.Name = "travel_SW_btn";
            this.travel_SW_btn.Size = new System.Drawing.Size(137, 123);
            this.travel_SW_btn.TabIndex = 5;
            this.travel_SW_btn.Tag = "SW";
            this.travel_SW_btn.Text = "Go SW";
            this.travel_SW_btn.UseVisualStyleBackColor = true;
            this.travel_SW_btn.Click += new System.EventHandler(this.travelBtnClick);
            // 
            // travel_E_btn
            // 
            this.travel_E_btn.Location = new System.Drawing.Point(299, 136);
            this.travel_E_btn.Name = "travel_E_btn";
            this.travel_E_btn.Size = new System.Drawing.Size(137, 123);
            this.travel_E_btn.TabIndex = 4;
            this.travel_E_btn.Tag = "E";
            this.travel_E_btn.Text = "Go E";
            this.travel_E_btn.UseVisualStyleBackColor = true;
            this.travel_E_btn.Click += new System.EventHandler(this.travelBtnClick);
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
            this.travel_W_btn.Tag = "W";
            this.travel_W_btn.Text = "Go W";
            this.travel_W_btn.UseVisualStyleBackColor = true;
            this.travel_W_btn.Click += new System.EventHandler(this.travelBtnClick);
            // 
            // travel_NE_btn
            // 
            this.travel_NE_btn.Location = new System.Drawing.Point(231, 7);
            this.travel_NE_btn.Name = "travel_NE_btn";
            this.travel_NE_btn.Size = new System.Drawing.Size(137, 123);
            this.travel_NE_btn.TabIndex = 1;
            this.travel_NE_btn.Tag = "NE";
            this.travel_NE_btn.Text = "Go NE";
            this.travel_NE_btn.UseVisualStyleBackColor = true;
            this.travel_NE_btn.Click += new System.EventHandler(this.travelBtnClick);
            // 
            // travel_NW_btn
            // 
            this.travel_NW_btn.Location = new System.Drawing.Point(88, 7);
            this.travel_NW_btn.Name = "travel_NW_btn";
            this.travel_NW_btn.Size = new System.Drawing.Size(137, 123);
            this.travel_NW_btn.TabIndex = 0;
            this.travel_NW_btn.Tag = "NW";
            this.travel_NW_btn.Text = "Go NW";
            this.travel_NW_btn.UseVisualStyleBackColor = true;
            this.travel_NW_btn.Click += new System.EventHandler(this.travelBtnClick);
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
            this.fiefsOwnedContainer.Size = new System.Drawing.Size(801, 511);
            this.fiefsOwnedContainer.SplitterDistance = 266;
            this.fiefsOwnedContainer.TabIndex = 5;
            // 
            // fiefsListView
            // 
            this.fiefsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fiefsListView.FullRowSelect = true;
            this.fiefsListView.GridLines = true;
            this.fiefsListView.Location = new System.Drawing.Point(0, 0);
            this.fiefsListView.Name = "fiefsListView";
            this.fiefsListView.Size = new System.Drawing.Size(531, 511);
            this.fiefsListView.TabIndex = 0;
            this.fiefsListView.UseCompatibleStateImageBehavior = false;
            this.fiefsListView.View = System.Windows.Forms.View.Details;
            this.fiefsListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.fiefsListView_ItemSelectionChanged);
            // 
            // meetingPlaceContainer
            // 
            this.meetingPlaceContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meetingPlaceContainer.Location = new System.Drawing.Point(0, 24);
            this.meetingPlaceContainer.Name = "meetingPlaceContainer";
            // 
            // meetingPlaceContainer.Panel1
            // 
            this.meetingPlaceContainer.Panel1.Controls.Add(this.hireNPC_TextBox);
            this.meetingPlaceContainer.Panel1.Controls.Add(this.hireNPC_Btn);
            // 
            // meetingPlaceContainer.Panel2
            // 
            this.meetingPlaceContainer.Panel2.Controls.Add(this.meetingPlaceCharsContainer);
            this.meetingPlaceContainer.Size = new System.Drawing.Size(801, 511);
            this.meetingPlaceContainer.SplitterDistance = 214;
            this.meetingPlaceContainer.TabIndex = 6;
            // 
            // hireNPC_TextBox
            // 
            this.hireNPC_TextBox.Location = new System.Drawing.Point(132, 13);
            this.hireNPC_TextBox.Name = "hireNPC_TextBox";
            this.hireNPC_TextBox.Size = new System.Drawing.Size(60, 20);
            this.hireNPC_TextBox.TabIndex = 1;
            // 
            // hireNPC_Btn
            // 
            this.hireNPC_Btn.Location = new System.Drawing.Point(15, 13);
            this.hireNPC_Btn.Name = "hireNPC_Btn";
            this.hireNPC_Btn.Size = new System.Drawing.Size(111, 23);
            this.hireNPC_Btn.TabIndex = 0;
            this.hireNPC_Btn.Text = "Hire/Fire NPC";
            this.hireNPC_Btn.UseVisualStyleBackColor = true;
            this.hireNPC_Btn.Click += new System.EventHandler(this.hireNPC_Btn_Click);
            // 
            // meetingPlaceCharsContainer
            // 
            this.meetingPlaceCharsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meetingPlaceCharsContainer.Location = new System.Drawing.Point(0, 0);
            this.meetingPlaceCharsContainer.Name = "meetingPlaceCharsContainer";
            // 
            // meetingPlaceCharsContainer.Panel1
            // 
            this.meetingPlaceCharsContainer.Panel1.Controls.Add(this.meetingPlaceCharDisplayTextBox);
            // 
            // meetingPlaceCharsContainer.Panel2
            // 
            this.meetingPlaceCharsContainer.Panel2.Controls.Add(this.meetingPlaceCharsListContainer);
            this.meetingPlaceCharsContainer.Size = new System.Drawing.Size(583, 511);
            this.meetingPlaceCharsContainer.SplitterDistance = 234;
            this.meetingPlaceCharsContainer.TabIndex = 0;
            // 
            // meetingPlaceCharDisplayTextBox
            // 
            this.meetingPlaceCharDisplayTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meetingPlaceCharDisplayTextBox.Location = new System.Drawing.Point(0, 0);
            this.meetingPlaceCharDisplayTextBox.Multiline = true;
            this.meetingPlaceCharDisplayTextBox.Name = "meetingPlaceCharDisplayTextBox";
            this.meetingPlaceCharDisplayTextBox.Size = new System.Drawing.Size(234, 511);
            this.meetingPlaceCharDisplayTextBox.TabIndex = 0;
            // 
            // meetingPlaceCharsListContainer
            // 
            this.meetingPlaceCharsListContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meetingPlaceCharsListContainer.Location = new System.Drawing.Point(0, 0);
            this.meetingPlaceCharsListContainer.Name = "meetingPlaceCharsListContainer";
            this.meetingPlaceCharsListContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // meetingPlaceCharsListContainer.Panel1
            // 
            this.meetingPlaceCharsListContainer.Panel1.Controls.Add(this.meetingPlaceTextBox);
            // 
            // meetingPlaceCharsListContainer.Panel2
            // 
            this.meetingPlaceCharsListContainer.Panel2.Controls.Add(this.meetingPlaceCharsListView);
            this.meetingPlaceCharsListContainer.Size = new System.Drawing.Size(345, 511);
            this.meetingPlaceCharsListContainer.SplitterDistance = 224;
            this.meetingPlaceCharsListContainer.TabIndex = 0;
            // 
            // meetingPlaceTextBox
            // 
            this.meetingPlaceTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meetingPlaceTextBox.Location = new System.Drawing.Point(0, 0);
            this.meetingPlaceTextBox.Multiline = true;
            this.meetingPlaceTextBox.Name = "meetingPlaceTextBox";
            this.meetingPlaceTextBox.Size = new System.Drawing.Size(345, 224);
            this.meetingPlaceTextBox.TabIndex = 0;
            // 
            // meetingPlaceCharsListView
            // 
            this.meetingPlaceCharsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meetingPlaceCharsListView.FullRowSelect = true;
            this.meetingPlaceCharsListView.GridLines = true;
            this.meetingPlaceCharsListView.Location = new System.Drawing.Point(0, 0);
            this.meetingPlaceCharsListView.Name = "meetingPlaceCharsListView";
            this.meetingPlaceCharsListView.Size = new System.Drawing.Size(345, 283);
            this.meetingPlaceCharsListView.TabIndex = 0;
            this.meetingPlaceCharsListView.UseCompatibleStateImageBehavior = false;
            this.meetingPlaceCharsListView.View = System.Windows.Forms.View.Details;
            this.meetingPlaceCharsListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.meetingPlaceCharsListView_ItemSelectionChanged);
            // 
            // removeBaliffBtn
            // 
            this.removeBaliffBtn.Location = new System.Drawing.Point(70, 314);
            this.removeBaliffBtn.Name = "removeBaliffBtn";
            this.removeBaliffBtn.Size = new System.Drawing.Size(122, 23);
            this.removeBaliffBtn.TabIndex = 18;
            this.removeBaliffBtn.Text = "Remove the Bailiff";
            this.removeBaliffBtn.UseVisualStyleBackColor = true;
            this.removeBaliffBtn.Click += new System.EventHandler(this.removeBaliffBtn_Click);
            // 
            // selfBailiffBtn
            // 
            this.selfBailiffBtn.Location = new System.Drawing.Point(70, 343);
            this.selfBailiffBtn.Name = "selfBailiffBtn";
            this.selfBailiffBtn.Size = new System.Drawing.Size(122, 35);
            this.selfBailiffBtn.TabIndex = 19;
            this.selfBailiffBtn.Text = "Appoint Myself as Bailiff";
            this.selfBailiffBtn.UseVisualStyleBackColor = true;
            this.selfBailiffBtn.Click += new System.EventHandler(this.selfBailiffBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 535);
            this.Controls.Add(this.fiefContainer);
            this.Controls.Add(this.meetingPlaceContainer);
            this.Controls.Add(this.travelContainer);
            this.Controls.Add(this.fiefsOwnedContainer);
            this.Controls.Add(this.characterContainer);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.characterContainer.Panel1.ResumeLayout(false);
            this.characterContainer.Panel1.PerformLayout();
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
            this.travelContainer.Panel1.ResumeLayout(false);
            this.travelContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.travelContainer)).EndInit();
            this.travelContainer.ResumeLayout(false);
            this.travelNavigationPanel.ResumeLayout(false);
            this.fiefsOwnedContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fiefsOwnedContainer)).EndInit();
            this.fiefsOwnedContainer.ResumeLayout(false);
            this.meetingPlaceContainer.Panel1.ResumeLayout(false);
            this.meetingPlaceContainer.Panel1.PerformLayout();
            this.meetingPlaceContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.meetingPlaceContainer)).EndInit();
            this.meetingPlaceContainer.ResumeLayout(false);
            this.meetingPlaceCharsContainer.Panel1.ResumeLayout(false);
            this.meetingPlaceCharsContainer.Panel1.PerformLayout();
            this.meetingPlaceCharsContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.meetingPlaceCharsContainer)).EndInit();
            this.meetingPlaceCharsContainer.ResumeLayout(false);
            this.meetingPlaceCharsListContainer.Panel1.ResumeLayout(false);
            this.meetingPlaceCharsListContainer.Panel1.PerformLayout();
            this.meetingPlaceCharsListContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.meetingPlaceCharsListContainer)).EndInit();
            this.meetingPlaceCharsListContainer.ResumeLayout(false);
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
        private System.Windows.Forms.Button adjustSpendBtn;
        private System.Windows.Forms.TextBox adjustTaxTextBox;
        private System.Windows.Forms.TextBox adjOffSpendTextBox;
        private System.Windows.Forms.TextBox adjGarrSpendTextBox;
        private System.Windows.Forms.TextBox adjInfrSpendTextBox;
        private System.Windows.Forms.TextBox adjustKeepSpendTextBox;
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
        private System.Windows.Forms.Button viewBailiffBtn;
        private System.Windows.Forms.SplitContainer meetingPlaceContainer;
        private System.Windows.Forms.SplitContainer meetingPlaceCharsContainer;
        private System.Windows.Forms.SplitContainer meetingPlaceCharsListContainer;
        private System.Windows.Forms.ListView meetingPlaceCharsListView;
        private System.Windows.Forms.TextBox meetingPlaceTextBox;
        private System.Windows.Forms.Button visitCourtBtn1;
        private System.Windows.Forms.Button enterKeepBtn;
        private System.Windows.Forms.Button visitTavernBtn;
        private System.Windows.Forms.TextBox meetingPlaceCharDisplayTextBox;
        private System.Windows.Forms.Button hireNPC_Btn;
        private System.Windows.Forms.TextBox hireNPC_TextBox;
        private System.Windows.Forms.TextBox charMultiMoveTextBox;
        private System.Windows.Forms.Button charMultiMoveBtn;
        private System.Windows.Forms.Label keepSpendLabel;
        private System.Windows.Forms.Label infraSpendLabel;
        private System.Windows.Forms.Label garrSpendLabel;
        private System.Windows.Forms.Label offSpendLabel;
        private System.Windows.Forms.Label taxRateLabel;
        private System.Windows.Forms.Button setBailiffBtn;
        private System.Windows.Forms.Button removeBaliffBtn;
        private System.Windows.Forms.Button selfBailiffBtn;

    }
}

