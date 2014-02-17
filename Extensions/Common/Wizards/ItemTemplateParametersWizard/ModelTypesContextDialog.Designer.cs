using System.Windows.Forms;

namespace ItemTemplateParametersWizard
{
    public partial class ModelTypesContextDialog : Form
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
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.entitySetTextBox = new System.Windows.Forms.TextBox();
            this.entityNameComboBox = new System.Windows.Forms.ComboBox();
            this.dbContextComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.descriptionLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.descriptionLabel.Location = new System.Drawing.Point(62, 22);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(329, 44);
            this.descriptionLabel.TabIndex = 1;
            this.descriptionLabel.Text = "Description";
            this.descriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 121);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Entity Set Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 151);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "DbContext Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Entity Name:";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(231, 188);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 17;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(139, 188);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 16;
            this.okButton.TabStop = false;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // entitySetTextBox
            // 
            this.entitySetTextBox.Location = new System.Drawing.Point(136, 118);
            this.entitySetTextBox.Name = "entitySetTextBox";
            this.entitySetTextBox.Size = new System.Drawing.Size(282, 20);
            this.entitySetTextBox.TabIndex = 1;
            // 
            // entityNameComboBox
            // 
            this.entityNameComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.entityNameComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.entityNameComboBox.DisplayMember = "DisplayName";
            this.entityNameComboBox.FormattingEnabled = true;
            this.entityNameComboBox.Location = new System.Drawing.Point(136, 88);
            this.entityNameComboBox.Name = "entityNameComboBox";
            this.entityNameComboBox.Size = new System.Drawing.Size(282, 21);
            this.entityNameComboBox.TabIndex = 0;
            this.entityNameComboBox.ValueMember = "Type";
            // 
            // dbContextComboBox
            // 
            this.dbContextComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.dbContextComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.dbContextComboBox.DisplayMember = "DisplayName";
            this.dbContextComboBox.FormattingEnabled = true;
            this.dbContextComboBox.Location = new System.Drawing.Point(136, 148);
            this.dbContextComboBox.Name = "dbContextComboBox";
            this.dbContextComboBox.Size = new System.Drawing.Size(282, 21);
            this.dbContextComboBox.TabIndex = 2;
            this.dbContextComboBox.ValueMember = "Type";
            // 
            // ModelTypesDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(457, 228);
            this.Controls.Add(this.dbContextComboBox);
            this.Controls.Add(this.entityNameComboBox);
            this.Controls.Add(this.entitySetTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.descriptionLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModelTypesDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Model Types";
            this.Load += new System.EventHandler(this.ModelTypesDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox entitySetTextBox;
        private System.Windows.Forms.ComboBox entityNameComboBox;
        private System.Windows.Forms.ComboBox dbContextComboBox;
    }
}