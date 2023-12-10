using LeggyTreeLib;
using System.Linq;
namespace LeggyDialogLib
{
    public class DialogueTreeNode :  TreeNode<DialogueOption>
    {
        public DialogueTreeNode(DialogueOption dialog) : base(dialog) 
        {

        }
        public DialogueTreeNode(DialogueOption dialog, DialogueTreeNode parent) : base(dialog, parent)
        {

        }

        public DialogueTreeNode(ITreeNode<DialogueOption> node) : base(node.Value, node.Parent)
        {
            _children = node.Children.ToList();
        }

        public DialogueTreeNode[] GetOptions()
        {
            if(Value.Said)
            {
                DialogueTreeNode[] output = { };
                foreach(ITreeNode<DialogueOption> child in _children) 
                {
                    output = output.Concat(new DialogueTreeNode(child).GetOptions()).ToArray();
                }
                return output;
            }
            return new DialogueTreeNode[1]{ this};
        }
    }
}
