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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PdmsAddinUserControl));
            this.dataGridViewPipeList = new System.Windows.Forms.DataGridView();
            this.ColumnPipe = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.btnMerge = new System.Windows.Forms.Button();
            this.folderBrowserDialogSavePath = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTipbtnGetPipes = new System.Windows.Forms.ToolTip(this.components);
            this.btnGetPipes = new System.Windows.Forms.Button();
            this.toolTipbtnClearCe = new System.Windows.Forms.ToolTip(this.components);
            this.btnClearCe = new System.Windows.Forms.Button();
            this.toolTipbtnClearAll = new System.Windows.Forms.ToolTip(this.components);
            this.btnClearAll = new System.Windows.Forms.Button();
            this.toolTipbtnApply = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipbtnMerge = new System.Windows.Forms.ToolTip(this.components);
            this.tabControlMyTools = new System.Windows.Forms.TabControl();
            this.tabPageIsoTool = new System.Windows.Forms.TabPage();
            this.tabPageMaterialTool = new System.Windows.Forms.TabPage();
            this.progressBarMyTool = new System.Windows.Forms.ProgressBar();
            this.labelMessage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPipeList)).BeginInit();
            this.tabControlMyTools.SuspendLayout();
            this.tabPageIsoTool.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewPipeList
            // 
            this.dataGridViewPipeList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPipeList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnPipe});
            this.dataGridViewPipeList.Location = new System.Drawing.Point(27, 81);
            this.dataGridViewPipeList.Name = "dataGridViewPipeList";
            this.dataGridViewPipeList.RowHeadersWidth = 62;
            this.dataGridViewPipeList.RowTemplate.Height = 20;
            this.dataGridViewPipeList.Size = new System.Drawing.Size(442, 413);
            this.dataGridViewPipeList.TabIndex = 1;
            // 
            // ColumnPipe
            // 
            this.ColumnPipe.HeaderText = "Pipeline";
            this.ColumnPipe.MinimumWidth = 8;
            this.ColumnPipe.Name = "ColumnPipe";
            this.ColumnPipe.Width = 500;
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxFilePath.Location = new System.Drawing.Point(6, 6);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new System.Drawing.Size(331, 31);
            this.textBoxFilePath.TabIndex = 2;
            this.textBoxFilePath.Text = "D:\\PDMSISO";
            // 
            // btnLoadFilePath
            // 
            this.btnLoadFilePath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLoadFilePath.Location = new System.Drawing.Point(343, 6);
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
            this.textBoxSavePath.Location = new System.Drawing.Point(6, 43);
            this.textBoxSavePath.Name = "textBoxSavePath";
            this.textBoxSavePath.Size = new System.Drawing.Size(331, 31);
            this.textBoxSavePath.TabIndex = 4;
            this.textBoxSavePath.Text = "D:\\PDMSISO\\DXF";
            // 
            // btnSavePath
            // 
            this.btnSavePath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSavePath.Location = new System.Drawing.Point(343, 43);
            this.btnSavePath.Name = "btnSavePath";
            this.btnSavePath.Size = new System.Drawing.Size(85, 31);
            this.btnSavePath.TabIndex = 5;
            this.btnSavePath.Text = "Save";
            this.btnSavePath.UseVisualStyleBackColor = true;
            this.btnSavePath.Click += new System.EventHandler(this.btnSavePath_Click);
            // 
            // btnApply
            // 
            this.btnApply.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnApply.BackgroundImage")));
            this.btnApply.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnApply.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnApply.Location = new System.Drawing.Point(8, 127);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(50, 50);
            this.btnApply.TabIndex = 6;
            this.toolTipbtnApply.SetToolTip(this.btnApply, "生成轴测图");
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
            this.textBoxDWGPath.Location = new System.Drawing.Point(6, 80);
            this.textBoxDWGPath.Name = "textBoxDWGPath";
            this.textBoxDWGPath.Size = new System.Drawing.Size(331, 31);
            this.textBoxDWGPath.TabIndex = 7;
            this.textBoxDWGPath.Text = "D:\\PDMSISO\\UNDER\\UNDER.DWG";
            // 
            // btnDWGPath
            // 
            this.btnDWGPath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDWGPath.Location = new System.Drawing.Point(343, 80);
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
            // btnMerge
            // 
            this.btnMerge.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnMerge.BackgroundImage")));
            this.btnMerge.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnMerge.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnMerge.Location = new System.Drawing.Point(64, 127);
            this.btnMerge.Name = "btnMerge";
            this.btnMerge.Size = new System.Drawing.Size(50, 50);
            this.btnMerge.TabIndex = 9;
            this.toolTipbtnMerge.SetToolTip(this.btnMerge, "合并底图");
            this.btnMerge.UseVisualStyleBackColor = true;
            this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
            // 
            // btnGetPipes
            // 
            this.btnGetPipes.BackgroundImage = global::PdmsAddin.Properties.Resources.Add;
            this.btnGetPipes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnGetPipes.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnGetPipes.Location = new System.Drawing.Point(27, 20);
            this.btnGetPipes.Name = "btnGetPipes";
            this.btnGetPipes.Size = new System.Drawing.Size(40, 40);
            this.btnGetPipes.TabIndex = 0;
            this.toolTipbtnGetPipes.SetToolTip(this.btnGetPipes, "添加管道");
            this.btnGetPipes.UseVisualStyleBackColor = true;
            this.btnGetPipes.Click += new System.EventHandler(this.btnShowHelloWorld_Click);
            // 
            // btnClearCe
            // 
            this.btnClearCe.BackgroundImage = global::PdmsAddin.Properties.Resources.RemoveCE;
            this.btnClearCe.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnClearCe.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnClearCe.Location = new System.Drawing.Point(73, 20);
            this.btnClearCe.Name = "btnClearCe";
            this.btnClearCe.Size = new System.Drawing.Size(40, 40);
            this.btnClearCe.TabIndex = 10;
            this.toolTipbtnClearCe.SetToolTip(this.btnClearCe, "移除当前管道");
            this.btnClearCe.UseVisualStyleBackColor = true;
            this.btnClearCe.Click += new System.EventHandler(this.btnClearCe_Click);
            // 
            // btnClearAll
            // 
            this.btnClearAll.BackgroundImage = global::PdmsAddin.Properties.Resources.RemoveAll;
            this.btnClearAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnClearAll.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnClearAll.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnClearAll.Location = new System.Drawing.Point(119, 20);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(40, 40);
            this.btnClearAll.TabIndex = 10;
            this.toolTipbtnClearAll.SetToolTip(this.btnClearAll, "清空管道");
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // tabControlMyTools
            // 
            this.tabControlMyTools.Controls.Add(this.tabPageIsoTool);
            this.tabControlMyTools.Controls.Add(this.tabPageMaterialTool);
            this.tabControlMyTools.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControlMyTools.Location = new System.Drawing.Point(27, 500);
            this.tabControlMyTools.Name = "tabControlMyTools";
            this.tabControlMyTools.SelectedIndex = 0;
            this.tabControlMyTools.Size = new System.Drawing.Size(442, 336);
            this.tabControlMyTools.TabIndex = 11;
            // 
            // tabPageIsoTool
            // 
            this.tabPageIsoTool.Controls.Add(this.textBoxFilePath);
            this.tabPageIsoTool.Controls.Add(this.btnLoadFilePath);
            this.tabPageIsoTool.Controls.Add(this.btnSavePath);
            this.tabPageIsoTool.Controls.Add(this.btnMerge);
            this.tabPageIsoTool.Controls.Add(this.btnDWGPath);
            this.tabPageIsoTool.Controls.Add(this.btnApply);
            this.tabPageIsoTool.Controls.Add(this.textBoxDWGPath);
            this.tabPageIsoTool.Controls.Add(this.textBoxSavePath);
            this.tabPageIsoTool.Location = new System.Drawing.Point(4, 33);
            this.tabPageIsoTool.Name = "tabPageIsoTool";
            this.tabPageIsoTool.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageIsoTool.Size = new System.Drawing.Size(434, 299);
            this.tabPageIsoTool.TabIndex = 0;
            this.tabPageIsoTool.Text = "ISO Tools";
            this.tabPageIsoTool.UseVisualStyleBackColor = true;
            // 
            // tabPageMaterialTool
            // 
            this.tabPageMaterialTool.Location = new System.Drawing.Point(4, 33);
            this.tabPageMaterialTool.Name = "tabPageMaterialTool";
            this.tabPageMaterialTool.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMaterialTool.Size = new System.Drawing.Size(434, 321);
            this.tabPageMaterialTool.TabIndex = 1;
            this.tabPageMaterialTool.Text = "Material Tool";
            this.tabPageMaterialTool.UseVisualStyleBackColor = true;
            // 
            // progressBarMyTool
            // 
            this.progressBarMyTool.Location = new System.Drawing.Point(215, 842);
            this.progressBarMyTool.Name = "progressBarMyTool";
            this.progressBarMyTool.Size = new System.Drawing.Size(250, 21);
            this.progressBarMyTool.TabIndex = 10;
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(34, 845);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(0, 18);
            this.labelMessage.TabIndex = 12;
            // 
            // PdmsAddinUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.progressBarMyTool);
            this.Controls.Add(this.tabControlMyTools);
            this.Controls.Add(this.btnClearAll);
            this.Controls.Add(this.btnClearCe);
            this.Controls.Add(this.dataGridViewPipeList);
            this.Controls.Add(this.btnGetPipes);
            this.Name = "PdmsAddinUserControl";
            this.Size = new System.Drawing.Size(492, 885);
            this.toolTipbtnClearCe.SetToolTip(this, "移除当前管道");
            this.Load += new System.EventHandler(this.PdmsAddinUserControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPipeList)).EndInit();
            this.tabControlMyTools.ResumeLayout(false);
            this.tabPageIsoTool.ResumeLayout(false);
            this.tabPageIsoTool.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetPipes;
        private System.Windows.Forms.DataGridView dataGridViewPipeList;
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
        private System.Windows.Forms.Button btnMerge;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPipe;
        private System.Windows.Forms.Button btnClearCe;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogSavePath;
        private System.Windows.Forms.ToolTip toolTipbtnGetPipes;
        private System.Windows.Forms.ToolTip toolTipbtnClearCe;
        private System.Windows.Forms.ToolTip toolTipbtnClearAll;
        private System.Windows.Forms.ToolTip toolTipbtnApply;
        private System.Windows.Forms.ToolTip toolTipbtnMerge;
        private System.Windows.Forms.TabControl tabControlMyTools;
        private System.Windows.Forms.TabPage tabPageIsoTool;
        private System.Windows.Forms.TabPage tabPageMaterialTool;
        private System.Windows.Forms.ProgressBar progressBarMyTool;
        private System.Windows.Forms.Label labelMessage;
    }
}
