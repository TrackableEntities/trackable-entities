namespace EntitiesSelectionWizard
{
    partial class EntitiesSelectionDialog
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Service", "server-icon.png");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Client", "client-icon.png");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Shared", "shared-icon.png");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EntitiesSelectionDialog));
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Portable", "portable-icon.png");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(".Net 4.5", "dotnet-icon.png");
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.buttonsPanel = new System.Windows.Forms.Panel();
            this.finishButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.previousButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.entitiesListView1 = new System.Windows.Forms.ListView();
            this.wizardImageList = new System.Windows.Forms.ImageList(this.components);
            this.entitiesListView2 = new System.Windows.Forms.ListView();
            this.buttonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.descriptionLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.descriptionLabel.Location = new System.Drawing.Point(18, 110);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(362, 63);
            this.descriptionLabel.TabIndex = 1;
            this.descriptionLabel.Text = "Description";
            // 
            // buttonsPanel
            // 
            this.buttonsPanel.Controls.Add(this.finishButton);
            this.buttonsPanel.Controls.Add(this.nextButton);
            this.buttonsPanel.Controls.Add(this.previousButton);
            this.buttonsPanel.Controls.Add(this.cancelButton);
            this.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonsPanel.Location = new System.Drawing.Point(0, 173);
            this.buttonsPanel.Name = "buttonsPanel";
            this.buttonsPanel.Size = new System.Drawing.Size(399, 47);
            this.buttonsPanel.TabIndex = 19;
            // 
            // finishButton
            // 
            this.finishButton.Enabled = false;
            this.finishButton.Location = new System.Drawing.Point(203, 10);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(87, 27);
            this.finishButton.TabIndex = 19;
            this.finishButton.TabStop = false;
            this.finishButton.Text = "&Finish";
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(110, 10);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(87, 27);
            this.nextButton.TabIndex = 18;
            this.nextButton.TabStop = false;
            this.nextButton.Text = "&Next >";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // previousButton
            // 
            this.previousButton.Enabled = false;
            this.previousButton.Location = new System.Drawing.Point(17, 10);
            this.previousButton.Name = "previousButton";
            this.previousButton.Size = new System.Drawing.Size(87, 27);
            this.previousButton.TabIndex = 16;
            this.previousButton.TabStop = false;
            this.previousButton.Text = "< &Previous";
            this.previousButton.UseVisualStyleBackColor = true;
            this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(296, 10);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(87, 27);
            this.cancelButton.TabIndex = 17;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // entitiesListView1
            // 
            this.entitiesListView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            listViewItem2.Checked = true;
            listViewItem2.StateImageIndex = 1;
            this.entitiesListView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3});
            this.entitiesListView1.LargeImageList = this.wizardImageList;
            this.entitiesListView1.Location = new System.Drawing.Point(18, 16);
            this.entitiesListView1.MultiSelect = false;
            this.entitiesListView1.Name = "entitiesListView1";
            this.entitiesListView1.Size = new System.Drawing.Size(362, 80);
            this.entitiesListView1.TabIndex = 20;
            this.entitiesListView1.UseCompatibleStateImageBehavior = false;
            this.entitiesListView1.SelectedIndexChanged += new System.EventHandler(this.entitiesListView1_SelectedIndexChanged);
            // 
            // wizardImageList
            // 
            this.wizardImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("wizardImageList.ImageStream")));
            this.wizardImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.wizardImageList.Images.SetKeyName(0, "client-icon.png");
            this.wizardImageList.Images.SetKeyName(1, "server-icon.png");
            this.wizardImageList.Images.SetKeyName(2, "shared-icon.png");
            this.wizardImageList.Images.SetKeyName(3, "portable-icon.png");
            this.wizardImageList.Images.SetKeyName(4, "dotnet-icon.png");
            // 
            // entitiesListView2
            // 
            this.entitiesListView2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.entitiesListView2.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem4,
            listViewItem5});
            this.entitiesListView2.LargeImageList = this.wizardImageList;
            this.entitiesListView2.Location = new System.Drawing.Point(18, 16);
            this.entitiesListView2.MultiSelect = false;
            this.entitiesListView2.Name = "entitiesListView2";
            this.entitiesListView2.Size = new System.Drawing.Size(362, 80);
            this.entitiesListView2.TabIndex = 21;
            this.entitiesListView2.UseCompatibleStateImageBehavior = false;
            this.entitiesListView2.Visible = false;
            this.entitiesListView2.SelectedIndexChanged += new System.EventHandler(this.entitiesListView2_SelectedIndexChanged);
            // 
            // EntitiesSelectionDialog
            // 
            this.AcceptButton = this.previousButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(399, 220);
            this.Controls.Add(this.entitiesListView2);
            this.Controls.Add(this.entitiesListView1);
            this.Controls.Add(this.buttonsPanel);
            this.Controls.Add(this.descriptionLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(415, 259);
            this.Name = "EntitiesSelectionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Entities Selection";
            this.buttonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Panel buttonsPanel;
        private System.Windows.Forms.Button previousButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.ListView entitiesListView1;
        private System.Windows.Forms.ImageList wizardImageList;
        private System.Windows.Forms.ListView entitiesListView2;
    }
}