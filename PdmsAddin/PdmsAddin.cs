using Aveva.ApplicationFramework;
using Aveva.ApplicationFramework.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdmsAddin
{
    //IAddin接口
    public class PdmsAddin : IAddin
    {
        //插件的名字
        public string Name { get { return "PdmsAddin"; } }
        //插件的描述
        public string Description { get { return "PdmsAddin"; } }
        //插件的构筑
        public void Start(ServiceManager serviceManager)
        {
            //必须的两个manager
            WindowManager myWindowManager = (WindowManager)serviceManager.GetService(typeof(WindowManager));
            CommandManager myCommandManager = (CommandManager)serviceManager.GetService(typeof(CommandManager));
            //新建一个基于Command接口构筑的DockedWindow
            PdmsAddinCommand myForm = new PdmsAddinCommand(myWindowManager);
            //把这个DockedWindow传进去
            myCommandManager.Commands.Add(myForm);
        }

        public void Stop()
        {
            
        }
    }
}
