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
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Client / Service", "client-icon.png");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Shared", "shared-icon.png");
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.buttonsPanel = new System.Windows.Forms.Panel();
            this.finishButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.previousButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.serviceClientSharedListView = new System.Windows.Forms.ListView();
            this.wizardImageList = new System.Windows.Forms.ImageList(this.components);
            this.portableDotNetListView = new System.Windows.Forms.ListView();
            this.clientServiceListView = new System.Windows.Forms.ListView();
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
            // serviceClientSharedListView
            // 
            this.serviceClientSharedListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            listViewItem2.Checked = true;
            listViewItem2.StateImageIndex = 1;
            this.serviceClientSharedListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3});
            this.serviceClientSharedListView.LargeImageList = this.wizardImageList;
            this.serviceClientSharedListView.Location = new System.Drawing.Point(18, 12);
            this.serviceClientSharedListView.MultiSelect = false;
            this.serviceClientSharedListView.Name = "serviceClientSharedListView";
            this.serviceClientSharedListView.Size = new System.Drawing.Size(362, 80);
            this.serviceClientSharedListView.TabIndex = 20;
            this.serviceClientSharedListView.UseCompatibleStateImageBehavior = false;
            this.serviceClientSharedListView.SelectedIndexChanged += new System.EventHandler(this.serviceClientSharedListView_SelectedIndexChanged);
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
            // portableDotNetListView
            // 
            this.portableDotNetListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.portableDotNetListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem4,
            listViewItem5});
            this.portableDotNetListView.LargeImageList = this.wizardImageList;
            this.portableDotNetListView.Location = new System.Drawing.Point(18, 12);
            this.portableDotNetListView.MultiSelect = false;
            this.portableDotNetListView.Name = "portableDotNetListView";
            this.portableDotNetListView.Size = new System.Drawing.Size(362, 80);
            this.portableDotNetListView.TabIndex = 21;
            this.portableDotNetListView.UseCompatibleStateImageBehavior = false;
            this.portableDotNetListView.Visible = false;
            this.portableDotNetListView.SelectedIndexChanged += new System.EventHandler(this.portableDotNetListView_SelectedIndexChanged);
            // 
            // clientServiceListView
            // 
            this.clientServiceListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            listViewItem6.Checked = true;
            listViewItem6.StateImageIndex = 1;
            this.clientServiceListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem6,
            listViewItem7});
            this.clientServiceListView.LargeImageList = this.wizardImageList;
            this.clientServiceListView.Location = new System.Drawing.Point(18, 12);
            this.clientServiceListView.MultiSelect = false;
            this.clientServiceListView.Name = "clientServiceListView";
            this.clientServiceListView.Size = new System.Drawing.Size(362, 80);
            this.clientServiceListView.TabIndex = 22;
            this.clientServiceListView.UseCompatibleStateImageBehavior = false;
            this.clientServiceListView.SelectedIndexChanged += new System.EventHandler(this.clientServiceListView_SelectedIndexChanged);
            // 
            // EntitiesSelectionDialog
            // 
            this.AcceptButton = this.previousButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(399, 220);
            this.Controls.Add(this.clientServiceListView);
            this.Controls.Add(this.serviceClientSharedListView);
            this.Controls.Add(this.portableDotNetListView);
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
        private System.Windows.Forms.ListView serviceClientSharedListView;
        private System.Windows.Forms.ImageList wizardImageList;
        private System.Windows.Forms.ListView portableDotNetListView;
        private System.Windows.Forms.ListView clientServiceListView;
    }
}