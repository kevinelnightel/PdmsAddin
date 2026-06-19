namespace PdmsAddin
{
    partial class PdmsAddinUserControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnGetPipes = new System.Windows.Forms.Button();
            this.dataGridViewPipeList = new System.Windows.Forms.DataGridView();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.btnLoadFilePath = new System.Windows.Forms.Button();
            this.openFileDialogFilePath = new System.Windows.Forms.OpenFileDialog();
            this.textBoxSavePath = new System.Windows.Forms.TextBox();
            this.btnSavePath = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.openFileDialogSavePath = new System.Windows.Forms.OpenFileDialog();
            this.textBoxDWGPath = new System.Windows.Forms.TextBox();
            this.btnDWGPath = new System.Windows.Forms.Button();
            this.openFileDialogDWGPath = new System.Windows.Forms.OpenFileDialog();
            this.btnTestCode = new System.Windows.Forms.Button();
            this.ColumnPipe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClearCe = new System.Windows.Forms.Button();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.folderBrowserDialogSavePath = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPipeList)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGetPipes
            // 
            this.btnGetPipes.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnGetPipes.Location = new System.Drawing.Point(27, 20);
            this.btnGetPipes.Name = "btnGetPipes";
            this.btnGetPipes.Size = new System.Drawing.Size(99, 61);
            this.btnGetPipes.TabIndex = 0;
            this.btnGetPipes.Text = "Add\r\nPipes";
            this.btnGetPipes.UseVisualStyleBackColor = true;
            this.btnGetPipes.Click += new System.EventHandler(this.btnShowHelloWorld_Click);
            // 
            // dataGridViewPipeList
            // 
            this.dataGridViewPipeList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPipeList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnPipe});
            this.dataGridViewPipeList.Location = new System.Drawing.Point(27, 96);
            this.dataGridViewPipeList.Name = "dataGridViewPipeList";
            this.dataGridViewPipeList.RowHeadersWidth = 62;
            this.dataGridViewPipeList.RowTemplate.Height = 30;
            this.dataGridViewPipeList.Size = new System.Drawing.Size(442, 564);
            this.dataGridViewPipeList.TabIndex = 1;
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxFilePath.Location = new System.Drawing.Point(27, 679);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new System.Drawing.Size(351, 31);
            this.textBoxFilePath.TabIndex = 2;
            // 
            // btnLoadFilePath
            // 
            this.btnLoadFilePath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLoadFilePath.Location = new System.Drawing.Point(384, 679);
            this.btnLoadFilePath.Name = "btnLoadFilePath";
            this.btnLoadFilePath.Size = new System.Drawing.Size(85, 31);
            this.btnLoadFilePath.TabIndex = 3;
            this.btnLoadFilePath.Text = "OPT";
            this.btnLoadFilePath.UseVisualStyleBackColor = true;
            this.btnLoadFilePath.Click += new System.EventHandler(this.btnLoadFilePath_Click);
            // 
            // openFileDialogFilePath
            // 
            this.openFileDialogFilePath.FileName = "openFileDialogFilePath";
            // 
            // textBoxSavePath
            // 
            this.textBoxSavePath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxSavePath.Location = new System.Drawing.Point(27, 721);
            this.textBoxSavePath.Name = "textBoxSavePath";
            this.textBoxSavePath.Size = new System.Drawing.Size(351, 31);
            this.textBoxSavePath.TabIndex = 4;
            this.textBoxSavePath.Text = "D:\\PDMSISO\\DXF";
            // 
            // btnSavePath
            // 
            this.btnSavePath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSavePath.Location = new System.Drawing.Point(384, 721);
            this.btnSavePath.Name = "btnSavePath";
            this.btnSavePath.Size = new System.Drawing.Size(85, 31);
            this.btnSavePath.TabIndex = 5;
            this.btnSavePath.Text = "Save";
            this.btnSavePath.UseVisualStyleBackColor = true;
            this.btnSavePath.Click += new System.EventHandler(this.btnSavePath_Click);
            // 
            // btnApply
            // 
            this.btnApply.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnApply.Location = new System.Drawing.Point(27, 811);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(99, 47);
            this.btnApply.TabIndex = 6;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // openFileDialogSavePath
            // 
            this.openFileDialogSavePath.FileName = "openFileDialogSavePath";
            // 
            // textBoxDWGPath
            // 
            this.textBoxDWGPath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxDWGPath.Location = new System.Drawing.Point(27, 763);
            this.textBoxDWGPath.Name = "textBoxDWGPath";
            this.textBoxDWGPath.Size = new System.Drawing.Size(351, 31);
            this.textBoxDWGPath.TabIndex = 7;
            this.textBoxDWGPath.Text = "D:\\PDMSISO\\UNDER\\UNDER.DWG";
            // 
            // btnDWGPath
            // 
            this.btnDWGPath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDWGPath.Location = new System.Drawing.Point(384, 763);
            this.btnDWGPath.Name = "btnDWGPath";
            this.btnDWGPath.Size = new System.Drawing.Size(85, 31);
            this.btnDWGPath.TabIndex = 8;
            this.btnDWGPath.Text = "Under";
            this.btnDWGPath.UseVisualStyleBackColor = true;
            this.btnDWGPath.Click += new System.EventHandler(this.btnDWGPath_Click);
            // 
            // openFileDialogDWGPath
            // 
            this.openFileDialogDWGPath.FileName = "openFileDialogDWGPath";
            // 
            // btnTestCode
            // 
            this.btnTestCode.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnTestCode.Location = new System.Drawing.Point(146, 811);
            this.btnTestCode.Name = "btnTestCode";
            this.btnTestCode.Size = new System.Drawing.Size(99, 47);
            this.btnTestCode.TabIndex = 9;
            this.btnTestCode.Text = "Merge";
            this.btnTestCode.UseVisualStyleBackColor = true;
            this.btnTestCode.Click += new System.EventHandler(this.btnTestCode_Click);
            // 
            // ColumnPipe
            // 
            this.ColumnPipe.HeaderText = "Pipeline";
            this.ColumnPipe.MinimumWidth = 8;
            this.ColumnPipe.Name = "ColumnPipe";
            this.ColumnPipe.Width = 500;
            // 
            // btnClearCe
            // 
            this.btnClearCe.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnClearCe.Location = new System.Drawing.Point(146, 20);
            this.btnClearCe.Name = "btnClearCe";
            this.btnClearCe.Size = new System.Drawing.Size(99, 61);
            this.btnClearCe.TabIndex = 10;
            this.btnClearCe.Text = "Clear\r\nCE";
            this.btnClearCe.UseVisualStyleBackColor = true;
            this.btnClearCe.Click += new System.EventHandler(this.btnClearCe_Click);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnClearAll.Location = new System.Drawing.Point(261, 20);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(99, 61);
            this.btnClearAll.TabIndex = 10;
            this.btnClearAll.Text = "Clear\r\nAll";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // PdmsAddinUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnClearAll);
            this.Controls.Add(this.btnClearCe);
            this.Controls.Add(this.btnTestCode);
            this.Controls.Add(this.btnDWGPath);
            this.Controls.Add(this.textBoxDWGPath);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnSavePath);
            this.Controls.Add(this.textBoxSavePath);
            this.Controls.Add(this.btnLoadFilePath);
            this.Controls.Add(this.textBoxFilePath);
            this.Controls.Add(this.dataGridViewPipeList);
            this.Controls.Add(this.btnGetPipes);
            this.Name = "PdmsAddinUserControl";
            this.Size = new System.Drawing.Size(492, 885);
            this.Load += new System.EventHandler(this.PdmsAddinUserControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPipeList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetPipes;
        private System.Windows.Forms.DataGridView dataGridViewPipeList;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TextBox textBoxFilePath;
        private System.Windows.Forms.Button btnLoadFilePath;
        private System.Windows.Forms.OpenFileDialog openFileDialogFilePath;
        private System.Windows.Forms.TextBox textBoxSavePath;
        private System.Windows.Forms.Button btnSavePath;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.OpenFileDialog openFileDialogSavePath;
        private System.Windows.Forms.TextBox textBoxDWGPath;
        private System.Windows.Forms.Button btnDWGPath;
        private System.Windows.Forms.OpenFileDialog openFileDialogDWGPath;
        private System.Windows.Forms.Button btnTestCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPipe;
        private System.Windows.Forms.Button btnClearCe;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogSavePath;
    }
}
