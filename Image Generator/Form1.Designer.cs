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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.CaptioningChecboxImages = new System.Windows.Forms.ImageList(this.components);
            this.DatasetCheckBoxImages = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.DeleteImages = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.ImageCaptCheckbox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.DataSetCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.resolutionBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.ParentFormPanel = new System.Windows.Forms.Panel();
            this.SentenceGeneratePanel = new System.Windows.Forms.TableLayoutPanel();
            this.ProcessStatus = new System.Windows.Forms.Label();
            this.generatedImage = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.sentenceBox = new System.Windows.Forms.TextBox();
            this.GenerateButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.DatasetPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.ProcessedImages = new System.Windows.Forms.Label();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.GenerateDatasetButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.LoadDataset = new System.Windows.Forms.Button();
            this.ChosenFile = new System.Windows.Forms.Label();
            this.ProcessedBar = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.ParentFormPanel.SuspendLayout();
            this.SentenceGeneratePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.generatedImage)).BeginInit();
            this.tableLayoutPanel8.SuspendLayout();
            this.DatasetPanel.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            this.SuspendLayout();
            // 
            // CaptioningChecboxImages
            // 
            this.CaptioningChecboxImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("CaptioningChecboxImages.ImageStream")));
            this.CaptioningChecboxImages.TransparentColor = System.Drawing.Color.Transparent;
            this.CaptioningChecboxImages.Images.SetKeyName(0, "tgl-off.png");
            this.CaptioningChecboxImages.Images.SetKeyName(1, "tgl-on.png");
            // 
            // DatasetCheckBoxImages
            // 
            this.DatasetCheckBoxImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("DatasetCheckBoxImages.ImageStream")));
            this.DatasetCheckBoxImages.TransparentColor = System.Drawing.Color.Transparent;
            this.DatasetCheckBoxImages.Images.SetKeyName(0, "tgl-off.png");
            this.DatasetCheckBoxImages.Images.SetKeyName(1, "tgl-on.png");
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ParentFormPanel, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1224, 678);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(67)))), ((int)(((byte)(120)))));
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.65031F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 61.34969F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(350, 678);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(67)))), ((int)(((byte)(120)))));
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel1.Location = new System.Drawing.Point(75, 31);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 200);
            this.panel1.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.DeleteImages, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel6, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.label7, 0, 5);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 265);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(344, 410);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // DeleteImages
            // 
            this.DeleteImages.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.DeleteImages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(45)))), ((int)(((byte)(82)))));
            this.DeleteImages.FlatAppearance.BorderSize = 0;
            this.DeleteImages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DeleteImages.Font = new System.Drawing.Font("Trebuchet MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeleteImages.ForeColor = System.Drawing.Color.White;
            this.DeleteImages.Location = new System.Drawing.Point(56, 315);
            this.DeleteImages.Margin = new System.Windows.Forms.Padding(3, 0, 3, 25);
            this.DeleteImages.Name = "DeleteImages";
            this.DeleteImages.Size = new System.Drawing.Size(231, 50);
            this.DeleteImages.TabIndex = 8;
            this.DeleteImages.Text = "Delete saved images";
            this.DeleteImages.UseVisualStyleBackColor = false;
            this.DeleteImages.Click += new System.EventHandler(this.DeleteImages_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Trebuchet MS", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "Options";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.28571F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.71429F));
            this.tableLayoutPanel4.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.ImageCaptCheckbox, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 63);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(338, 44);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Trebuchet MS", 10F);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(173, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "Use image captioning";
            // 
            // ImageCaptCheckbox
            // 
            this.ImageCaptCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.ImageCaptCheckbox.BackColor = System.Drawing.Color.Transparent;
            this.ImageCaptCheckbox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ImageCaptCheckbox.Dock = System.Windows.Forms.DockStyle.Right;
            this.ImageCaptCheckbox.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(67)))), ((int)(((byte)(120)))));
            this.ImageCaptCheckbox.FlatAppearance.BorderSize = 0;
            this.ImageCaptCheckbox.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(67)))), ((int)(((byte)(120)))));
            this.ImageCaptCheckbox.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(67)))), ((int)(((byte)(120)))));
            this.ImageCaptCheckbox.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(67)))), ((int)(((byte)(120)))));
            this.ImageCaptCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ImageCaptCheckbox.ImageIndex = 0;
            this.ImageCaptCheckbox.ImageList = this.CaptioningChecboxImages;
            this.ImageCaptCheckbox.Location = new System.Drawing.Point(231, 3);
            this.ImageCaptCheckbox.Name = "ImageCaptCheckbox";
            this.ImageCaptCheckbox.Size = new System.Drawing.Size(104, 38);
            this.ImageCaptCheckbox.TabIndex = 3;
            this.ImageCaptCheckbox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ImageCaptCheckbox.UseVisualStyleBackColor = true;
            this.ImageCaptCheckbox.CheckedChanged += new System.EventHandler(this.ImageCaptCheckbox_CheckedChanged);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.22841F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.77159F));
            this.tableLayoutPanel5.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.DataSetCheckBox, 1, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 113);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(338, 44);
            this.tableLayoutPanel5.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Trebuchet MS", 10F);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(3, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 23);
            this.label3.TabIndex = 3;
            this.label3.Text = "Use dataset";
            // 
            // DataSetCheckBox
            // 
            this.DataSetCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.DataSetCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.DataSetCheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.DataSetCheckBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.DataSetCheckBox.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(67)))), ((int)(((byte)(120)))));
            this.DataSetCheckBox.FlatAppearance.BorderSize = 0;
            this.DataSetCheckBox.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(67)))), ((int)(((byte)(120)))));
            this.DataSetCheckBox.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(67)))), ((int)(((byte)(120)))));
            this.DataSetCheckBox.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(67)))), ((int)(((byte)(120)))));
            this.DataSetCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DataSetCheckBox.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.DataSetCheckBox.ImageIndex = 0;
            this.DataSetCheckBox.ImageList = this.DatasetCheckBoxImages;
            this.DataSetCheckBox.Location = new System.Drawing.Point(231, 3);
            this.DataSetCheckBox.Name = "DataSetCheckBox";
            this.DataSetCheckBox.Size = new System.Drawing.Size(104, 38);
            this.DataSetCheckBox.TabIndex = 4;
            this.DataSetCheckBox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.DataSetCheckBox.UseVisualStyleBackColor = true;
            this.DataSetCheckBox.CheckedChanged += new System.EventHandler(this.DataSetCheckBox_CheckedChanged);
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.93475F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.06525F));
            this.tableLayoutPanel6.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.resolutionBox, 1, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 163);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(338, 44);
            this.tableLayoutPanel6.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Trebuchet MS", 10F);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(3, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 23);
            this.label4.TabIndex = 4;
            this.label4.Text = "Resolution";
            // 
            // resolutionBox
            // 
            this.resolutionBox.AllowDrop = true;
            this.resolutionBox.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.resolutionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.resolutionBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resolutionBox.Location = new System.Drawing.Point(214, 9);
            this.resolutionBox.Name = "resolutionBox";
            this.resolutionBox.Size = new System.Drawing.Size(121, 24);
            this.resolutionBox.TabIndex = 5;
            this.resolutionBox.SelectedIndexChanged += new System.EventHandler(this.ResolutionBox_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Trebuchet MS", 8F);
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(88, 392);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(167, 18);
            this.label7.TabIndex = 5;
            this.label7.Text = " © 2020 Lukáš Chaloupský";
            // 
            // ParentFormPanel
            // 
            this.ParentFormPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(45)))), ((int)(((byte)(82)))));
            this.ParentFormPanel.Controls.Add(this.SentenceGeneratePanel);
            this.ParentFormPanel.Controls.Add(this.DatasetPanel);
            this.ParentFormPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ParentFormPanel.Location = new System.Drawing.Point(350, 0);
            this.ParentFormPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ParentFormPanel.Name = "ParentFormPanel";
            this.ParentFormPanel.Size = new System.Drawing.Size(874, 678);
            this.ParentFormPanel.TabIndex = 1;
            // 
            // SentenceGeneratePanel
            // 
            this.SentenceGeneratePanel.ColumnCount = 1;
            this.SentenceGeneratePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SentenceGeneratePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.SentenceGeneratePanel.Controls.Add(this.ProcessStatus, 0, 1);
            this.SentenceGeneratePanel.Controls.Add(this.generatedImage, 0, 0);
            this.SentenceGeneratePanel.Controls.Add(this.tableLayoutPanel8, 0, 2);
            this.SentenceGeneratePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SentenceGeneratePanel.Location = new System.Drawing.Point(0, 0);
            this.SentenceGeneratePanel.Name = "SentenceGeneratePanel";
            this.SentenceGeneratePanel.RowCount = 3;
            this.SentenceGeneratePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.SentenceGeneratePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.SentenceGeneratePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.SentenceGeneratePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.SentenceGeneratePanel.Size = new System.Drawing.Size(874, 678);
            this.SentenceGeneratePanel.TabIndex = 0;
            // 
            // ProcessStatus
            // 
            this.ProcessStatus.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ProcessStatus.AutoSize = true;
            this.ProcessStatus.Font = new System.Drawing.Font("Trebuchet MS", 8F);
            this.ProcessStatus.ForeColor = System.Drawing.Color.White;
            this.ProcessStatus.Location = new System.Drawing.Point(388, 508);
            this.ProcessStatus.Name = "ProcessStatus";
            this.ProcessStatus.Size = new System.Drawing.Size(98, 18);
            this.ProcessStatus.TabIndex = 4;
            this.ProcessStatus.Text = "Process status";
            this.ProcessStatus.Visible = false;
            // 
            // generatedImage
            // 
            this.generatedImage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.generatedImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(67)))), ((int)(((byte)(120)))));
            this.generatedImage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("generatedImage.BackgroundImage")));
            this.generatedImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.generatedImage.Location = new System.Drawing.Point(25, 26);
            this.generatedImage.Margin = new System.Windows.Forms.Padding(25, 3, 25, 3);
            this.generatedImage.Name = "generatedImage";
            this.generatedImage.Size = new System.Drawing.Size(824, 455);
            this.generatedImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.generatedImage.TabIndex = 5;
            this.generatedImage.TabStop = false;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 3;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel8.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.sentenceBox, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.GenerateButton, 1, 1);
            this.tableLayoutPanel8.Controls.Add(this.SaveButton, 2, 1);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(3, 544);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 2;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(868, 131);
            this.tableLayoutPanel8.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Trebuchet MS", 10F);
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(25, 9);
            this.label6.Margin = new System.Windows.Forms.Padding(25, 0, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(146, 23);
            this.label6.TabIndex = 5;
            this.label6.Text = "Image description";
            // 
            // sentenceBox
            // 
            this.sentenceBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sentenceBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.sentenceBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.sentenceBox.Location = new System.Drawing.Point(25, 43);
            this.sentenceBox.Margin = new System.Windows.Forms.Padding(25, 11, 3, 3);
            this.sentenceBox.Name = "sentenceBox";
            this.sentenceBox.Size = new System.Drawing.Size(540, 27);
            this.sentenceBox.TabIndex = 6;
            this.sentenceBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SentenceBox_KeyUp);
            // 
            // GenerateButton
            // 
            this.GenerateButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.GenerateButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(67)))), ((int)(((byte)(120)))));
            this.GenerateButton.FlatAppearance.BorderSize = 0;
            this.GenerateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GenerateButton.Font = new System.Drawing.Font("Trebuchet MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GenerateButton.ForeColor = System.Drawing.Color.White;
            this.GenerateButton.Location = new System.Drawing.Point(583, 32);
            this.GenerateButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.GenerateButton.Name = "GenerateButton";
            this.GenerateButton.Size = new System.Drawing.Size(120, 50);
            this.GenerateButton.TabIndex = 7;
            this.GenerateButton.Text = "Generate";
            this.GenerateButton.UseVisualStyleBackColor = false;
            this.GenerateButton.Click += new System.EventHandler(this.GenerateButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.SaveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(67)))), ((int)(((byte)(120)))));
            this.SaveButton.FlatAppearance.BorderSize = 0;
            this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveButton.Font = new System.Drawing.Font("Trebuchet MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveButton.ForeColor = System.Drawing.Color.White;
            this.SaveButton.Location = new System.Drawing.Point(722, 32);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(3, 0, 25, 3);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(120, 50);
            this.SaveButton.TabIndex = 8;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = false;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // DatasetPanel
            // 
            this.DatasetPanel.ColumnCount = 1;
            this.DatasetPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DatasetPanel.Controls.Add(this.label5, 0, 0);
            this.DatasetPanel.Controls.Add(this.ProcessedImages, 0, 3);
            this.DatasetPanel.Controls.Add(this.tableLayoutPanel10, 0, 1);
            this.DatasetPanel.Controls.Add(this.ProcessedBar, 0, 2);
            this.DatasetPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DatasetPanel.Location = new System.Drawing.Point(0, 0);
            this.DatasetPanel.Name = "DatasetPanel";
            this.DatasetPanel.RowCount = 6;
            this.DatasetPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.DatasetPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.DatasetPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.DatasetPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.DatasetPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DatasetPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.DatasetPanel.Size = new System.Drawing.Size(874, 678);
            this.DatasetPanel.TabIndex = 9;
            this.DatasetPanel.Visible = false;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Trebuchet MS", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(247, 41);
            this.label5.Margin = new System.Windows.Forms.Padding(25, 0, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(401, 38);
            this.label5.TabIndex = 4;
            this.label5.Text = "Generate images for dataset";
            // 
            // ProcessedImages
            // 
            this.ProcessedImages.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ProcessedImages.AutoSize = true;
            this.ProcessedImages.Font = new System.Drawing.Font("Trebuchet MS", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ProcessedImages.ForeColor = System.Drawing.Color.White;
            this.ProcessedImages.Location = new System.Drawing.Point(401, 240);
            this.ProcessedImages.Name = "ProcessedImages";
            this.ProcessedImages.Size = new System.Drawing.Size(71, 18);
            this.ProcessedImages.TabIndex = 3;
            this.ProcessedImages.Text = "Processed";
            this.ProcessedImages.Visible = false;
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 2;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Controls.Add(this.GenerateDatasetButton, 0, 0);
            this.tableLayoutPanel10.Controls.Add(this.tableLayoutPanel11, 1, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(3, 123);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 1;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(868, 74);
            this.tableLayoutPanel10.TabIndex = 0;
            // 
            // GenerateDatasetButton
            // 
            this.GenerateDatasetButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.GenerateDatasetButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(67)))), ((int)(((byte)(120)))));
            this.GenerateDatasetButton.FlatAppearance.BorderSize = 0;
            this.GenerateDatasetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GenerateDatasetButton.Font = new System.Drawing.Font("Trebuchet MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GenerateDatasetButton.ForeColor = System.Drawing.Color.White;
            this.GenerateDatasetButton.Location = new System.Drawing.Point(289, 10);
            this.GenerateDatasetButton.Margin = new System.Windows.Forms.Padding(25, 0, 25, 3);
            this.GenerateDatasetButton.Name = "GenerateDatasetButton";
            this.GenerateDatasetButton.Size = new System.Drawing.Size(120, 50);
            this.GenerateDatasetButton.TabIndex = 8;
            this.GenerateDatasetButton.Text = "Generate";
            this.GenerateDatasetButton.UseVisualStyleBackColor = false;
            this.GenerateDatasetButton.Click += new System.EventHandler(this.GenerateDatasetButton_Click);
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.ColumnCount = 2;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.Controls.Add(this.LoadDataset, 0, 0);
            this.tableLayoutPanel11.Controls.Add(this.ChosenFile, 1, 0);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(437, 3);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 1;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.Size = new System.Drawing.Size(428, 68);
            this.tableLayoutPanel11.TabIndex = 9;
            // 
            // LoadDataset
            // 
            this.LoadDataset.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.LoadDataset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(67)))), ((int)(((byte)(120)))));
            this.LoadDataset.FlatAppearance.BorderSize = 0;
            this.LoadDataset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LoadDataset.Font = new System.Drawing.Font("Trebuchet MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadDataset.ForeColor = System.Drawing.Color.White;
            this.LoadDataset.Location = new System.Drawing.Point(25, 7);
            this.LoadDataset.Margin = new System.Windows.Forms.Padding(25, 0, 0, 3);
            this.LoadDataset.Name = "LoadDataset";
            this.LoadDataset.Size = new System.Drawing.Size(120, 50);
            this.LoadDataset.TabIndex = 9;
            this.LoadDataset.Text = "Load data";
            this.LoadDataset.UseVisualStyleBackColor = false;
            this.LoadDataset.Click += new System.EventHandler(this.LoadDataset_Click);
            // 
            // ChosenFile
            // 
            this.ChosenFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ChosenFile.AutoSize = true;
            this.ChosenFile.Font = new System.Drawing.Font("Trebuchet MS", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ChosenFile.ForeColor = System.Drawing.Color.White;
            this.ChosenFile.Location = new System.Drawing.Point(148, 40);
            this.ChosenFile.Name = "ChosenFile";
            this.ChosenFile.Padding = new System.Windows.Forms.Padding(10, 0, 0, 10);
            this.ChosenFile.Size = new System.Drawing.Size(10, 28);
            this.ChosenFile.TabIndex = 10;
            // 
            // ProcessedBar
            // 
            this.ProcessedBar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ProcessedBar.BackColor = System.Drawing.Color.Black;
            this.ProcessedBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(43)))), ((int)(((byte)(120)))));
            this.ProcessedBar.Location = new System.Drawing.Point(137, 217);
            this.ProcessedBar.Margin = new System.Windows.Forms.Padding(50, 3, 50, 3);
            this.ProcessedBar.Name = "ProcessedBar";
            this.ProcessedBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ProcessedBar.Size = new System.Drawing.Size(600, 5);
            this.ProcessedBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProcessedBar.TabIndex = 1;
            this.ProcessedBar.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1224, 678);
            this.Controls.Add(this.tableLayoutPanel1);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1242, 725);
            this.Name = "Form1";
            this.Text = "Image Generator";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.ParentFormPanel.ResumeLayout(false);
            this.SentenceGeneratePanel.ResumeLayout(false);
            this.SentenceGeneratePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.generatedImage)).EndInit();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.DatasetPanel.ResumeLayout(false);
            this.DatasetPanel.PerformLayout();
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel11.ResumeLayout(false);
            this.tableLayoutPanel11.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList CaptioningChecboxImages;
        private System.Windows.Forms.ImageList DatasetCheckBoxImages;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox ImageCaptCheckbox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox DataSetCheckBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox resolutionBox;
        private System.Windows.Forms.Panel ParentFormPanel;
        private System.Windows.Forms.TableLayoutPanel SentenceGeneratePanel;
        private System.Windows.Forms.Label ProcessStatus;
        private System.Windows.Forms.PictureBox generatedImage;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox sentenceBox;
        private System.Windows.Forms.Button GenerateButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.TableLayoutPanel DatasetPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.Button GenerateDatasetButton;
        private System.Windows.Forms.Label ProcessedImages;
        private System.Windows.Forms.ProgressBar ProcessedBar;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel11;
        private System.Windows.Forms.Button LoadDataset;
        private System.Windows.Forms.Label ChosenFile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button DeleteImages;
    }
}

