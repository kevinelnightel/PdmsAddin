using Aveva.ApplicationFramework.Presentation;
using Aveva.ApplicationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdmsAddin
{
   //Command接口
     class PdmsAddinCommand : Command
    {
        //声明一个DockedWindow
        DockedWindow myForm;
        //构造函数，在实例化的时候自动运行
        //要把这个WindowManager传进来
        public PdmsAddinCommand(WindowManager windowManager)
        {
            //这个Key就是PDMS里面的插件存放位置
            this.Key = "PdmsAddin.MyTools";
            //实例化一个用户控件
            PdmsAddinUserControl myUserControl = new PdmsAddinUserControl();
            //把之前声明的DockedWindow实例化，把用户控件传进去
            myForm = windowManager.CreateDockedWindow("PdmsAddinForm", "MyTools", myUserControl, DockedPosition.Right);
           
            //是否记住关闭之前的窗口样式
            myForm.SaveLayout = false;

            //订阅窗口关闭的情况，如果窗口关闭，那么工具栏的按钮就弹起来
            myForm.Closed += MyForm_Closed;
            //订阅加载窗口完成的情况，如果加载完成，那么工具栏按钮的开启状态和窗口的显示情况一致
            windowManager.WindowLayoutLoaded += WindowManager_WindowLayoutLoaded;
            this.ExecuteOnCheckedChange = false;
        }

        private void WindowManager_WindowLayoutLoaded(object sender, EventArgs e)
        {
            //如果加载完成，那么工具栏按钮的开启状态和窗口的显示情况一致
            this.Checked = myForm.Visible;
        }

        private void MyForm_Closed(object sender, EventArgs e)
        {
            //如果窗口关闭，那么工具栏的按钮就弹起来
            this.Checked = false;
        }
        //定义按钮点击后发生什么
        public override void Execute()
        {
           
            if (this.Checked)
            {
                //如果按钮按下前是true，窗口打开，那么按下后自动false，按钮弹起，窗口关闭
                myForm.Hide();
            }
            else
            {
                //如果按钮按下前是false，窗口关闭，那么按下后自动true，按钮弹起，窗口开启
                myForm.Show();
                //每次打开都定义下窗口尺寸
                myForm.Width = 350;
                myForm.Height = 800;
            }
        }
    }
}
