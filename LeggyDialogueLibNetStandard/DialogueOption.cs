using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeggyDialogLib
{
    public class DialogueOption
    {
        public string PlayerDialog;
        public string Response;
        public bool Said;
        public DialogueOption(string playerDialog, string response)
        {
            PlayerDialog = playerDialog;
            Response = response;
            Said = false;
        }
    }
}
