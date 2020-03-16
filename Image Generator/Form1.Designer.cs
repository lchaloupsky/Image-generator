namespace Image_Generator
{
    partial class Form1
    {
        /// <summary>
        /// Vyžaduje se proměnná návrháře.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Uvolněte všechny používané prostředky.
        /// </summary>
        /// <param name="disposing">hodnota true, když by se měl spravovaný prostředek odstranit; jinak false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kód generovaný Návrhářem Windows Form

        /// <summary>
        /// Metoda vyžadovaná pro podporu Návrháře - neupravovat
        /// obsah této metody v editoru kódu.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.generatedImage = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.resolutionBox = new System.Windows.Forms.ComboBox();
            this.sentenceBox = new System.Windows.Forms.TextBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.generateButton = new System.Windows.Forms.Button();
            this.DataSetCheckBox = new System.Windows.Forms.CheckBox();
            this.LoadDataset = new System.Windows.Forms.Button();
            this.ProcessedImages = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.generatedImage)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.generatedImage, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(834, 533);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // generatedImage
            // 
            this.generatedImage.BackColor = System.Drawing.Color.White;
            this.generatedImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generatedImage.Location = new System.Drawing.Point(3, 3);
            this.generatedImage.Name = "generatedImage";
            this.generatedImage.Size = new System.Drawing.Size(828, 420);
            this.generatedImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.generatedImage.TabIndex = 0;
            this.generatedImage.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.Controls.Add(this.resolutionBox, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.sentenceBox, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.saveButton, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.generateButton, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.DataSetCheckBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.LoadDataset, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.ProcessedImages, 3, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 429);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(828, 101);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // resolutionBox
            // 
            this.resolutionBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.resolutionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.resolutionBox.FormattingEnabled = true;
            this.resolutionBox.Location = new System.Drawing.Point(706, 63);
            this.resolutionBox.Name = "resolutionBox";
            this.resolutionBox.Size = new System.Drawing.Size(119, 24);
            this.resolutionBox.TabIndex = 3;
            this.resolutionBox.SelectedIndexChanged += new System.EventHandler(this.ResolutionBox_SelectedIndexChanged);
            // 
            // sentenceBox
            // 
            this.sentenceBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.sentenceBox.Location = new System.Drawing.Point(3, 64);
            this.sentenceBox.Name = "sentenceBox";
            this.sentenceBox.Size = new System.Drawing.Size(449, 22);
            this.sentenceBox.TabIndex = 2;
            this.sentenceBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SentenceBox_KeyUp);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.saveButton.Location = new System.Drawing.Point(591, 60);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(100, 30);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // generateButton
            // 
            this.generateButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.generateButton.Location = new System.Drawing.Point(467, 60);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(100, 30);
            this.generateButton.TabIndex = 0;
            this.generateButton.Text = "Generate";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.GenerateButton_Click);
            // 
            // DataSetCheckBox
            // 
            this.DataSetCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.DataSetCheckBox.AutoSize = true;
            this.DataSetCheckBox.Location = new System.Drawing.Point(465, 14);
            this.DataSetCheckBox.Name = "DataSetCheckBox";
            this.DataSetCheckBox.Size = new System.Drawing.Size(104, 21);
            this.DataSetCheckBox.TabIndex = 4;
            this.DataSetCheckBox.Text = "use dataset";
            this.DataSetCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.DataSetCheckBox.UseVisualStyleBackColor = true;
            this.DataSetCheckBox.CheckedChanged += new System.EventHandler(this.DataSetCheckBox_CheckedChanged);
            // 
            // LoadDataset
            // 
            this.LoadDataset.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LoadDataset.Location = new System.Drawing.Point(591, 10);
            this.LoadDataset.Name = "LoadDataset";
            this.LoadDataset.Size = new System.Drawing.Size(100, 30);
            this.LoadDataset.TabIndex = 5;
            this.LoadDataset.Text = "Load";
            this.LoadDataset.UseVisualStyleBackColor = true;
            this.LoadDataset.Click += new System.EventHandler(this.LoadDataset_Click);
            // 
            // ProcessedImages
            // 
            this.ProcessedImages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ProcessedImages.AutoSize = true;
            this.ProcessedImages.Location = new System.Drawing.Point(706, 16);
            this.ProcessedImages.Name = "ProcessedImages";
            this.ProcessedImages.Size = new System.Drawing.Size(119, 17);
            this.ProcessedImages.TabIndex = 6;
            this.ProcessedImages.Text = "Processed";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(834, 533);
            this.Controls.Add(this.tableLayoutPanel1);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.MinimumSize = new System.Drawing.Size(852, 580);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.generatedImage)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox generatedImage;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.TextBox sentenceBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ComboBox resolutionBox;
        private System.Windows.Forms.CheckBox DataSetCheckBox;
        private System.Windows.Forms.Button LoadDataset;
        private System.Windows.Forms.Label ProcessedImages;
    }
}

