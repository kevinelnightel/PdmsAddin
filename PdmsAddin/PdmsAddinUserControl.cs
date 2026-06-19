using Aveva.Pdms.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace PdmsAddin
{
    public partial class PdmsAddinUserControl : UserControl
    {
        public PdmsAddinUserControl()
        {
            InitializeComponent();
        }

        private void PdmsAddinUserControl_Load(object sender, EventArgs e)
        {
        }

        private void btnShowHelloWorld_Click(object sender, EventArgs e)
        {
            //实例化这个类，并且获取当前对象下的所有管线名
            PipesHandle pipesHandle = new PipesHandle();
            List<String>  pipeList = pipesHandle.AddPipes();

            //新建一个空的列表
            List<String> pipes = new List<string>();
            //从表格里把管线数据获取出来
            foreach (DataGridViewRow pipe in dataGridViewPipeList.Rows)
                if (!pipe.IsNewRow)
                {
                    string pipeLine = pipe.Cells["ColumnPipe"].Value.ToString();

                    pipes.Add(pipeLine);
                }


            foreach (string pipe in pipeList)
            {
                if (pipe != null)   //判断是否为空
                {
                    if(!pipes.Contains(pipe))    //判断表格里是否已经有了，没有的再加进去
                    {
                        dataGridViewPipeList.Rows.Add(pipe);
                    }
                   
                }

            }
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            dataGridViewPipeList.Rows.Clear();
        }

        private void btnClearCe_Click(object sender, EventArgs e)
        {
            if (dataGridViewPipeList.CurrentRow != null && !dataGridViewPipeList.CurrentRow.IsNewRow)   //别把占位行删除了
            {
                dataGridViewPipeList.Rows.Remove(dataGridViewPipeList.CurrentRow);
            }
        }

        private void btnLoadFilePath_Click(object sender, EventArgs e)
        {

            string inputPath = textBoxFilePath.Text.Trim();
            // 如果输入可能是文件路径，则取其目录；否则直接当作目录尝试
            string dir = inputPath;
            if (System.IO.File.Exists(inputPath))
                dir = System.IO.Path.GetDirectoryName(inputPath);

            if (!string.IsNullOrEmpty(dir) && System.IO.Directory.Exists(dir))
                openFileDialogFilePath.InitialDirectory = dir;

            openFileDialogFilePath.Title = "Please choose option file";
            //如果文件选择没有问题，就把文件名设置复制进来
            if(openFileDialogFilePath.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text = openFileDialogFilePath.FileName;
            }

        }

        private void btnSavePath_Click(object sender, EventArgs e)
        {
            //设置文件的保存路径
            folderBrowserDialogSavePath.Description = "Please set save path";

            if (!string.IsNullOrEmpty(textBoxSavePath.Text) && Directory.Exists(textBoxSavePath.Text))
            {
                folderBrowserDialogSavePath.SelectedPath = textBoxSavePath.Text;
            }

            if (folderBrowserDialogSavePath.ShowDialog() == DialogResult.OK)
            {
                // 将选中的文件夹路径存入 TextBox
                textBoxSavePath.Text = folderBrowserDialogSavePath.SelectedPath;
            }
        }

        private void btnDWGPath_Click(object sender, EventArgs e)
        {

            //设置底图的路径
            openFileDialogDWGPath.Title = "Please choose DWG file";

            string inputPath = textBoxDWGPath.Text.Trim();

            // 如果输入可能是文件路径，则取其目录；否则直接当作目录尝试
            string dir = inputPath;

            if (System.IO.File.Exists(inputPath))
                dir = System.IO.Path.GetDirectoryName(inputPath);

            if (!string.IsNullOrEmpty(dir) && System.IO.Directory.Exists(dir))
                openFileDialogDWGPath.InitialDirectory = dir;


            if (openFileDialogDWGPath.ShowDialog() == DialogResult.OK)
            {
                textBoxDWGPath.Text = openFileDialogDWGPath.FileName;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            PipesHandle pipesHandle = new PipesHandle();    //处理工具的类先实例化
            pipesHandle.ClearFilePathFiles(textBoxSavePath.Text);   //先把文件夹里清空

            foreach (DataGridViewRow pipe in dataGridViewPipeList.Rows) //再依次去执行输出轴测图
            {
                if (!pipe.IsNewRow)
                {
                    string pipeLine = pipe.Cells["ColumnPipe"].Value.ToString();
                    
                    pipesHandle.GenerateIso(textBoxFilePath.Text, textBoxSavePath.Text, pipeLine);
                }
                    
            }

        }

        private void btnTestCode_Click(object sender, EventArgs e)
        {
            PipesHandle pipesHandle = new PipesHandle();
            foreach (string file in Directory.GetFiles(textBoxSavePath.Text))
            {
                if(file.Contains(".dxf"))
                {
                   // pipesHandle.TestCode(file);
                    PipesHandle.MergeDxfIntoDwg(file, textBoxDWGPath.Text);
                }
                    
            }

            MessageBox.Show("完成");

        }

        
    }
}
