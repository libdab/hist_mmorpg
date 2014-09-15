namespace hist_mmorpg
{
    partial class SelectionForm
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
            this.npcContainer = new System.Windows.Forms.SplitContainer();
            this.npcDetailsTextBox = new System.Windows.Forms.TextBox();
            this.npcListContainer = new System.Windows.Forms.SplitContainer();
            this.chooseNpcBtn = new System.Windows.Forms.Button();
            this.npcListView = new System.Windows.Forms.ListView();
            this.lockOutContainer = new System.Windows.Forms.SplitContainer();
            this.closeBtn = new System.Windows.Forms.Button();
            this.barEnglishCheckBox = new System.Windows.Forms.CheckBox();
            this.barFrenchCheckBox = new System.Windows.Forms.CheckBox();
            this.unbarCharBtn = new System.Windows.Forms.Button();
            this.barThisCharTextBox = new System.Windows.Forms.TextBox();
            this.barThisCharBtn = new System.Windows.Forms.Button();
            this.barredListContainer = new System.Windows.Forms.SplitContainer();
            this.barredListLabel = new System.Windows.Forms.Label();
            this.barredListView = new System.Windows.Forms.ListView();
            this.transferContainer = new System.Windows.Forms.SplitContainer();
            this.transferCancelBtn = new System.Windows.Forms.Button();
            this.transferPickupBtn = new System.Windows.Forms.Button();
            this.transferListView = new System.Windows.Forms.ListView();
            this.armiesContainer = new System.Windows.Forms.SplitContainer();
            this.armiesListContainer = new System.Windows.Forms.SplitContainer();
            this.armiesTextBox = new System.Windows.Forms.TextBox();
            this.armiesListView = new System.Windows.Forms.ListView();
            this.armiesCloseBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.npcContainer)).BeginInit();
            this.npcContainer.Panel1.SuspendLayout();
            this.npcContainer.Panel2.SuspendLayout();
            this.npcContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.npcListContainer)).BeginInit();
            this.npcListContainer.Panel1.SuspendLayout();
            this.npcListContainer.Panel2.SuspendLayout();
            this.npcListContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lockOutContainer)).BeginInit();
            this.lockOutContainer.Panel1.SuspendLayout();
            this.lockOutContainer.Panel2.SuspendLayout();
            this.lockOutContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.barredListContainer)).BeginInit();
            this.barredListContainer.Panel1.SuspendLayout();
            this.barredListContainer.Panel2.SuspendLayout();
            this.barredListContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transferContainer)).BeginInit();
            this.transferContainer.Panel1.SuspendLayout();
            this.transferContainer.Panel2.SuspendLayout();
            this.transferContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.armiesContainer)).BeginInit();
            this.armiesContainer.Panel1.SuspendLayout();
            this.armiesContainer.Panel2.SuspendLayout();
            this.armiesContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.armiesListContainer)).BeginInit();
            this.armiesListContainer.Panel1.SuspendLayout();
            this.armiesListContainer.Panel2.SuspendLayout();
            this.armiesListContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // npcContainer
            // 
            this.npcContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.npcContainer.Location = new System.Drawing.Point(0, 0);
            this.npcContainer.Name = "npcContainer";
            // 
            // npcContainer.Panel1
            // 
            this.npcContainer.Panel1.Controls.Add(this.npcDetailsTextBox);
            // 
            // npcContainer.Panel2
            // 
            this.npcContainer.Panel2.Controls.Add(this.npcListContainer);
            this.npcContainer.Size = new System.Drawing.Size(620, 547);
            this.npcContainer.SplitterDistance = 206;
            this.npcContainer.TabIndex = 0;
            // 
            // npcDetailsTextBox
            // 
            this.npcDetailsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.npcDetailsTextBox.Location = new System.Drawing.Point(0, 0);
            this.npcDetailsTextBox.Multiline = true;
            this.npcDetailsTextBox.Name = "npcDetailsTextBox";
            this.npcDetailsTextBox.Size = new System.Drawing.Size(206, 547);
            this.npcDetailsTextBox.TabIndex = 0;
            // 
            // npcListContainer
            // 
            this.npcListContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.npcListContainer.Location = new System.Drawing.Point(0, 0);
            this.npcListContainer.Name = "npcListContainer";
            this.npcListContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // npcListContainer.Panel1
            // 
            this.npcListContainer.Panel1.Controls.Add(this.chooseNpcBtn);
            // 
            // npcListContainer.Panel2
            // 
            this.npcListContainer.Panel2.Controls.Add(this.npcListView);
            this.npcListContainer.Size = new System.Drawing.Size(410, 547);
            this.npcListContainer.SplitterDistance = 136;
            this.npcListContainer.TabIndex = 0;
            // 
            // chooseNpcBtn
            // 
            this.chooseNpcBtn.Location = new System.Drawing.Point(106, 47);
            this.chooseNpcBtn.Name = "chooseNpcBtn";
            this.chooseNpcBtn.Size = new System.Drawing.Size(189, 35);
            this.chooseNpcBtn.TabIndex = 0;
            this.chooseNpcBtn.Text = "Appoint this NPC";
            this.chooseNpcBtn.UseVisualStyleBackColor = true;
            this.chooseNpcBtn.Click += new System.EventHandler(this.chooseNpcBtn_Click);
            // 
            // npcListView
            // 
            this.npcListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.npcListView.FullRowSelect = true;
            this.npcListView.GridLines = true;
            this.npcListView.Location = new System.Drawing.Point(0, 0);
            this.npcListView.Name = "npcListView";
            this.npcListView.Size = new System.Drawing.Size(410, 407);
            this.npcListView.TabIndex = 0;
            this.npcListView.UseCompatibleStateImageBehavior = false;
            this.npcListView.View = System.Windows.Forms.View.Details;
            this.npcListView.SelectedIndexChanged += new System.EventHandler(this.npcListView_SelectedIndexChanged);
            // 
            // lockOutContainer
            // 
            this.lockOutContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lockOutContainer.Location = new System.Drawing.Point(0, 0);
            this.lockOutContainer.Name = "lockOutContainer";
            // 
            // lockOutContainer.Panel1
            // 
            this.lockOutContainer.Panel1.Controls.Add(this.closeBtn);
            this.lockOutContainer.Panel1.Controls.Add(this.barEnglishCheckBox);
            this.lockOutContainer.Panel1.Controls.Add(this.barFrenchCheckBox);
            this.lockOutContainer.Panel1.Controls.Add(this.unbarCharBtn);
            this.lockOutContainer.Panel1.Controls.Add(this.barThisCharTextBox);
            this.lockOutContainer.Panel1.Controls.Add(this.barThisCharBtn);
            // 
            // lockOutContainer.Panel2
            // 
            this.lockOutContainer.Panel2.Controls.Add(this.barredListContainer);
            this.lockOutContainer.Size = new System.Drawing.Size(620, 547);
            this.lockOutContainer.SplitterDistance = 257;
            this.lockOutContainer.TabIndex = 1;
            // 
            // closeBtn
            // 
            this.closeBtn.Location = new System.Drawing.Point(16, 224);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(108, 35);
            this.closeBtn.TabIndex = 6;
            this.closeBtn.Text = "Close Lock Out Options";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // barEnglishCheckBox
            // 
            this.barEnglishCheckBox.AutoSize = true;
            this.barEnglishCheckBox.Location = new System.Drawing.Point(151, 170);
            this.barEnglishCheckBox.Name = "barEnglishCheckBox";
            this.barEnglishCheckBox.Size = new System.Drawing.Size(101, 17);
            this.barEnglishCheckBox.TabIndex = 5;
            this.barEnglishCheckBox.Text = "Bar The English";
            this.barEnglishCheckBox.UseVisualStyleBackColor = true;
            this.barEnglishCheckBox.CheckedChanged += new System.EventHandler(this.barEnglishCheckBox_CheckedChanged);
            // 
            // barFrenchCheckBox
            // 
            this.barFrenchCheckBox.AutoSize = true;
            this.barFrenchCheckBox.Location = new System.Drawing.Point(151, 143);
            this.barFrenchCheckBox.Name = "barFrenchCheckBox";
            this.barFrenchCheckBox.Size = new System.Drawing.Size(100, 17);
            this.barFrenchCheckBox.TabIndex = 3;
            this.barFrenchCheckBox.Text = "Bar The French";
            this.barFrenchCheckBox.UseVisualStyleBackColor = true;
            this.barFrenchCheckBox.CheckedChanged += new System.EventHandler(this.barFrenchCheckBox_CheckedChanged);
            // 
            // unbarCharBtn
            // 
            this.unbarCharBtn.Location = new System.Drawing.Point(16, 88);
            this.unbarCharBtn.Name = "unbarCharBtn";
            this.unbarCharBtn.Size = new System.Drawing.Size(108, 35);
            this.unbarCharBtn.TabIndex = 2;
            this.unbarCharBtn.Text = "UnBar Selected Character";
            this.unbarCharBtn.UseVisualStyleBackColor = true;
            this.unbarCharBtn.Click += new System.EventHandler(this.unbarCharBtn_Click);
            // 
            // barThisCharTextBox
            // 
            this.barThisCharTextBox.Location = new System.Drawing.Point(151, 61);
            this.barThisCharTextBox.Name = "barThisCharTextBox";
            this.barThisCharTextBox.Size = new System.Drawing.Size(59, 20);
            this.barThisCharTextBox.TabIndex = 1;
            // 
            // barThisCharBtn
            // 
            this.barThisCharBtn.Location = new System.Drawing.Point(16, 61);
            this.barThisCharBtn.Name = "barThisCharBtn";
            this.barThisCharBtn.Size = new System.Drawing.Size(108, 21);
            this.barThisCharBtn.TabIndex = 0;
            this.barThisCharBtn.Text = "Bar This Character";
            this.barThisCharBtn.UseVisualStyleBackColor = true;
            this.barThisCharBtn.Click += new System.EventHandler(this.barThisCharBtn_Click);
            // 
            // barredListContainer
            // 
            this.barredListContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.barredListContainer.Location = new System.Drawing.Point(0, 0);
            this.barredListContainer.Name = "barredListContainer";
            this.barredListContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // barredListContainer.Panel1
            // 
            this.barredListContainer.Panel1.Controls.Add(this.barredListLabel);
            // 
            // barredListContainer.Panel2
            // 
            this.barredListContainer.Panel2.Controls.Add(this.barredListView);
            this.barredListContainer.Size = new System.Drawing.Size(359, 547);
            this.barredListContainer.SplitterDistance = 45;
            this.barredListContainer.TabIndex = 0;
            // 
            // barredListLabel
            // 
            this.barredListLabel.AutoSize = true;
            this.barredListLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.barredListLabel.Location = new System.Drawing.Point(18, 9);
            this.barredListLabel.Name = "barredListLabel";
            this.barredListLabel.Size = new System.Drawing.Size(320, 24);
            this.barredListLabel.TabIndex = 0;
            this.barredListLabel.Text = "Characters Barred From This Fief";
            // 
            // barredListView
            // 
            this.barredListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.barredListView.FullRowSelect = true;
            this.barredListView.Location = new System.Drawing.Point(0, 0);
            this.barredListView.Name = "barredListView";
            this.barredListView.Size = new System.Drawing.Size(359, 498);
            this.barredListView.TabIndex = 0;
            this.barredListView.UseCompatibleStateImageBehavior = false;
            this.barredListView.View = System.Windows.Forms.View.Details;
            this.barredListView.SelectedIndexChanged += new System.EventHandler(this.barredListView_SelectedIndexChanged);
            // 
            // transferContainer
            // 
            this.transferContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.transferContainer.Location = new System.Drawing.Point(0, 0);
            this.transferContainer.Name = "transferContainer";
            // 
            // transferContainer.Panel1
            // 
            this.transferContainer.Panel1.Controls.Add(this.transferCancelBtn);
            this.transferContainer.Panel1.Controls.Add(this.transferPickupBtn);
            // 
            // transferContainer.Panel2
            // 
            this.transferContainer.Panel2.Controls.Add(this.transferListView);
            this.transferContainer.Size = new System.Drawing.Size(620, 547);
            this.transferContainer.SplitterDistance = 206;
            this.transferContainer.TabIndex = 2;
            // 
            // transferCancelBtn
            // 
            this.transferCancelBtn.Location = new System.Drawing.Point(17, 53);
            this.transferCancelBtn.Name = "transferCancelBtn";
            this.transferCancelBtn.Size = new System.Drawing.Size(79, 24);
            this.transferCancelBtn.TabIndex = 1;
            this.transferCancelBtn.Text = "Cancel";
            this.transferCancelBtn.UseVisualStyleBackColor = true;
            this.transferCancelBtn.Click += new System.EventHandler(this.transferCancelBtn_Click);
            // 
            // transferPickupBtn
            // 
            this.transferPickupBtn.Location = new System.Drawing.Point(17, 23);
            this.transferPickupBtn.Name = "transferPickupBtn";
            this.transferPickupBtn.Size = new System.Drawing.Size(168, 24);
            this.transferPickupBtn.TabIndex = 0;
            this.transferPickupBtn.Text = "Pick Up Selected Troops";
            this.transferPickupBtn.UseVisualStyleBackColor = true;
            this.transferPickupBtn.Click += new System.EventHandler(this.transferPickupBtn_Click);
            // 
            // transferListView
            // 
            this.transferListView.CheckBoxes = true;
            this.transferListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.transferListView.FullRowSelect = true;
            this.transferListView.Location = new System.Drawing.Point(0, 0);
            this.transferListView.Name = "transferListView";
            this.transferListView.Size = new System.Drawing.Size(410, 547);
            this.transferListView.TabIndex = 0;
            this.transferListView.UseCompatibleStateImageBehavior = false;
            this.transferListView.View = System.Windows.Forms.View.Details;
            // 
            // armiesContainer
            // 
            this.armiesContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.armiesContainer.Location = new System.Drawing.Point(0, 0);
            this.armiesContainer.Name = "armiesContainer";
            this.armiesContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // armiesContainer.Panel1
            // 
            this.armiesContainer.Panel1.Controls.Add(this.armiesCloseBtn);
            // 
            // armiesContainer.Panel2
            // 
            this.armiesContainer.Panel2.Controls.Add(this.armiesListContainer);
            this.armiesContainer.Size = new System.Drawing.Size(620, 547);
            this.armiesContainer.SplitterDistance = 85;
            this.armiesContainer.TabIndex = 3;
            // 
            // armiesListContainer
            // 
            this.armiesListContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.armiesListContainer.Location = new System.Drawing.Point(0, 0);
            this.armiesListContainer.Name = "armiesListContainer";
            // 
            // armiesListContainer.Panel1
            // 
            this.armiesListContainer.Panel1.Controls.Add(this.armiesTextBox);
            // 
            // armiesListContainer.Panel2
            // 
            this.armiesListContainer.Panel2.Controls.Add(this.armiesListView);
            this.armiesListContainer.Size = new System.Drawing.Size(620, 458);
            this.armiesListContainer.SplitterDistance = 308;
            this.armiesListContainer.TabIndex = 0;
            // 
            // armiesTextBox
            // 
            this.armiesTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.armiesTextBox.Location = new System.Drawing.Point(0, 0);
            this.armiesTextBox.Multiline = true;
            this.armiesTextBox.Name = "armiesTextBox";
            this.armiesTextBox.Size = new System.Drawing.Size(308, 458);
            this.armiesTextBox.TabIndex = 0;
            // 
            // armiesListView
            // 
            this.armiesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.armiesListView.FullRowSelect = true;
            this.armiesListView.Location = new System.Drawing.Point(0, 0);
            this.armiesListView.Name = "armiesListView";
            this.armiesListView.Size = new System.Drawing.Size(308, 458);
            this.armiesListView.TabIndex = 0;
            this.armiesListView.UseCompatibleStateImageBehavior = false;
            this.armiesListView.View = System.Windows.Forms.View.Details;
            this.armiesListView.SelectedIndexChanged += new System.EventHandler(this.armiesListView_SelectedIndexChanged);
            // 
            // armiesCloseBtn
            // 
            this.armiesCloseBtn.Location = new System.Drawing.Point(261, 18);
            this.armiesCloseBtn.Name = "armiesCloseBtn";
            this.armiesCloseBtn.Size = new System.Drawing.Size(108, 35);
            this.armiesCloseBtn.TabIndex = 7;
            this.armiesCloseBtn.Text = "Close";
            this.armiesCloseBtn.UseVisualStyleBackColor = true;
            this.armiesCloseBtn.Click += new System.EventHandler(this.armiesCloseBtn_Click);
            // 
            // SelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 547);
            this.Controls.Add(this.armiesContainer);
            this.Controls.Add(this.lockOutContainer);
            this.Controls.Add(this.npcContainer);
            this.Controls.Add(this.transferContainer);
            this.Name = "SelectionForm";
            this.Text = "SelectionForm";
            this.npcContainer.Panel1.ResumeLayout(false);
            this.npcContainer.Panel1.PerformLayout();
            this.npcContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.npcContainer)).EndInit();
            this.npcContainer.ResumeLayout(false);
            this.npcListContainer.Panel1.ResumeLayout(false);
            this.npcListContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.npcListContainer)).EndInit();
            this.npcListContainer.ResumeLayout(false);
            this.lockOutContainer.Panel1.ResumeLayout(false);
            this.lockOutContainer.Panel1.PerformLayout();
            this.lockOutContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lockOutContainer)).EndInit();
            this.lockOutContainer.ResumeLayout(false);
            this.barredListContainer.Panel1.ResumeLayout(false);
            this.barredListContainer.Panel1.PerformLayout();
            this.barredListContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.barredListContainer)).EndInit();
            this.barredListContainer.ResumeLayout(false);
            this.transferContainer.Panel1.ResumeLayout(false);
            this.transferContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.transferContainer)).EndInit();
            this.transferContainer.ResumeLayout(false);
            this.armiesContainer.Panel1.ResumeLayout(false);
            this.armiesContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.armiesContainer)).EndInit();
            this.armiesContainer.ResumeLayout(false);
            this.armiesListContainer.Panel1.ResumeLayout(false);
            this.armiesListContainer.Panel1.PerformLayout();
            this.armiesListContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.armiesListContainer)).EndInit();
            this.armiesListContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer npcContainer;
        private System.Windows.Forms.SplitContainer npcListContainer;
        private System.Windows.Forms.Button chooseNpcBtn;
        private System.Windows.Forms.ListView npcListView;
        private System.Windows.Forms.TextBox npcDetailsTextBox;
        private System.Windows.Forms.SplitContainer lockOutContainer;
        private System.Windows.Forms.SplitContainer barredListContainer;
        private System.Windows.Forms.Label barredListLabel;
        private System.Windows.Forms.ListView barredListView;
        private System.Windows.Forms.TextBox barThisCharTextBox;
        private System.Windows.Forms.Button barThisCharBtn;
        private System.Windows.Forms.Button unbarCharBtn;
        private System.Windows.Forms.CheckBox barEnglishCheckBox;
        private System.Windows.Forms.CheckBox barFrenchCheckBox;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.SplitContainer transferContainer;
        private System.Windows.Forms.ListView transferListView;
        private System.Windows.Forms.Button transferPickupBtn;
        private System.Windows.Forms.Button transferCancelBtn;
        private System.Windows.Forms.SplitContainer armiesContainer;
        private System.Windows.Forms.SplitContainer armiesListContainer;
        private System.Windows.Forms.TextBox armiesTextBox;
        private System.Windows.Forms.ListView armiesListView;
        private System.Windows.Forms.Button armiesCloseBtn;

    }
}