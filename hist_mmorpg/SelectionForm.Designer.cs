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
            this.npcListContainer = new System.Windows.Forms.SplitContainer();
            this.npcListView = new System.Windows.Forms.ListView();
            this.chooseNpcBtn = new System.Windows.Forms.Button();
            this.npcDetailsTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.npcContainer)).BeginInit();
            this.npcContainer.Panel1.SuspendLayout();
            this.npcContainer.Panel2.SuspendLayout();
            this.npcContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.npcListContainer)).BeginInit();
            this.npcListContainer.Panel1.SuspendLayout();
            this.npcListContainer.Panel2.SuspendLayout();
            this.npcListContainer.SuspendLayout();
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
            // npcDetailsTextBox
            // 
            this.npcDetailsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.npcDetailsTextBox.Location = new System.Drawing.Point(0, 0);
            this.npcDetailsTextBox.Multiline = true;
            this.npcDetailsTextBox.Name = "npcDetailsTextBox";
            this.npcDetailsTextBox.Size = new System.Drawing.Size(206, 547);
            this.npcDetailsTextBox.TabIndex = 0;
            // 
            // SelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 547);
            this.Controls.Add(this.npcContainer);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer npcContainer;
        private System.Windows.Forms.SplitContainer npcListContainer;
        private System.Windows.Forms.Button chooseNpcBtn;
        private System.Windows.Forms.ListView npcListView;
        private System.Windows.Forms.TextBox npcDetailsTextBox;

    }
}