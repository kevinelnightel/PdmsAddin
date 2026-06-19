using Aveva.Pdms.Database;
using Aveva.Pdms.Shared;
using Aveva.Pdms.Utilities.CommandLine;
using Aveva.PDMS.Database.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PdmsAddin
{
    public class PipesHandle
    {
        public List<String> AddPipes()
        {
            //获取当前
            DbElement currentElement = CurrentElement.Element;
            //加一个类型过滤器
            TypeFilter pipeTypeFilter = new TypeFilter(DbElementTypeInstance.PIPE);
            //从当前的范围，获取过滤器设置的类型的对象
            DBElementCollection pipeCollection = new DBElementCollection(currentElement, pipeTypeFilter);
            //设置一个要获取的属性，这里是全名
            DbAttribute attName = DbAttributeInstance.FLNN;
            //实例化一个列表用来存放管线名
            List<String> pipeList = new List<String>();

            foreach (DbElement element in pipeCollection)
            {
                string name = element.GetString(attName);
                pipeList.Add(name);
            }
            //返回这个管线名
            return pipeList;
        }

        public void GenerateIso(string optSavePath,string fileSavePath ,string pipeLine)    //根据输入的管线名和OPT文件，输出dxf文件
        {
            this.RunCommand("ISODRAFTMODE");
            this.RunCommand("$m " + optSavePath);
            
            string new1PipeLine = pipeLine.Replace("\"", "'");   //如果管线名里有"符号，替换成'
            string new2PipeLine = new1PipeLine.Replace("/", ".");   //如果管线名里有/符号，替换成.


            this.RunCommand("File dxf  \"" + fileSavePath + "\\" + new2PipeLine + "-\"  SINGLE");
            this.RunCommand("detail   /" + pipeLine);
            this.RunCommand("exit");
        }

        public void ClearFilePathFiles(string fileSavePath)    //用来请问对应文件夹内的所有文件
        {
            foreach (string file in Directory.GetFiles(fileSavePath))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"删除文件 {file} 失败：{ex.Message}");
                }
            }
        }
 
        public void RunCommand(string commandLine)       //用来执行PML代码
        {
            Command myCommand;
            myCommand = Command.CreateCommand(commandLine);
            myCommand.Run();
        }

        public static bool MergeDxfIntoDwg(string dxfPath, string dwgPath)
        {
            // 外部 exe 的绝对路径（可以根据实际情况配置）
            string exePath = @"C:\AVEVA\PdmsAddin\MergeDxfAndDwg.exe";

            // 检查 exe 是否存在
            if (!System.IO.File.Exists(exePath))
            {
                MessageBox.Show("合并工具未找到，请检查部署。");
                return false;
            }

            // 参数：用双引号括起路径，以处理空格
            string args = $"\"{dxfPath}\" \"{dwgPath}\"";

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = exePath;
                startInfo.Arguments = args;
                startInfo.UseShellExecute = false;          // 不使用系统 Shell
                startInfo.RedirectStandardOutput = true;    // 捕获输出
                startInfo.RedirectStandardError = true;     // 捕获错误
                startInfo.CreateNoWindow = true;            // 不显示控制台窗口

                using (Process process = Process.Start(startInfo))
                {
                    // 等待进程结束
                    process.WaitForExit();

                    // 读取输出信息
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    PipesHandle mypPpeHandle = new PipesHandle();

                    if (process.ExitCode == 0)
                    {
                        string filename = Path.GetFileNameWithoutExtension(dxfPath);
                        mypPpeHandle.RunCommand("$P " + filename + " is merged");
                        return true;
                    }
                    else
                    {
                        MessageBox.Show($"合并失败 (ExitCode: {process.ExitCode})\n错误信息: {error}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动外部进程时出错: {ex.Message}", "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        //用来测试代码
        public void TestCode(string commandLine)
        {
            Command myCommand;
            myCommand = Command.CreateCommand("$p " + commandLine);
            myCommand.Run();
        }
    }
}
