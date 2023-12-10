using LeggyDialogueLib;
using LeggyTreeLib;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeggyDialogLib
{
    public class DialogueTree
    {
        public DialogueTreeNode ConversationStart;
        public DialogueTree(string opener, string response)
        {
            ConversationStart = new DialogueTreeNode(new DialogueOption(opener, response));
        }

        public DialogueTree(DialogueTreeNode rootNode)
        {
            ConversationStart = rootNode;
        }

        public DialogueTreeNode[] Options
        {
            get
            {
                return ConversationStart.GetOptions();

            }
        }

        public ITreeNode<DialogueOption>[] ToNodeArray()
        {
            return ConversationStart.ToNodeArray();
        }

        private DialogueNodeRecord[] GenerateRecords()
        {
            List<DialogueNodeRecord> records = new List<DialogueNodeRecord>();
            var dialogueArray = ConversationStart.ToNodeArray();
            for (int i = 0; i < dialogueArray.Length; i++)
            {
                var thisDialogue = dialogueArray[i];
                records.Add(new DialogueNodeRecord(i.ToString(), thisDialogue.Value));
                // for all previous records
                for (int scanBack = i - 1; scanBack >= 0; scanBack--)
                {
                    // swap information with the previous records
                    var previousDialogue = dialogueArray[scanBack];
                    if (previousDialogue.Parent == thisDialogue) // this node is the parent of the previous
                    {
                        records[scanBack].ParentId = i.ToString();
                    }
                    if (previousDialogue.Children.Contains(thisDialogue)) // this node is a child of the previous
                    {
                        records[scanBack].childIds.Add(i.ToString());
                    }
                    if (thisDialogue.Parent == previousDialogue) // the previous node is the parent of this one
                    {
                        records[i].ParentId = scanBack.ToString();
                    }
                    if (thisDialogue.Children.Contains(previousDialogue)) // the previous node is a child of this one
                    {
                        records[i].childIds.Add(scanBack.ToString());
                    }
                }
            }
            return records.ToArray();
        }

        private static List<DialogueNodeRecord> ReadRecords(string path)
        {
            string[] lines = File.ReadAllLines(path);
            List<DialogueNodeRecord> records = new List<DialogueNodeRecord>();
            foreach (string line in lines)
            {
                records.Add(DialogueNodeRecord.FromString(line));
            }
            return records;
        }

        public static void BuildFromRecords(List<DialogueNodeRecord> records, ref ITreeNode<DialogueOption> thisNode, DialogueNodeRecord thisRecord)
        {
            for(int i=0; i<thisRecord.childIds.Count(); i++)
            {
                var childRecord = records.Find(x => x.Id == thisRecord.childIds[i]);
                if (childRecord == null) throw new BadTreeException("Referenced node (id: " + thisRecord.childIds[i] + ") could not be found");
                thisNode.AddChild(new DialogueOption(childRecord.PlayerDialog, childRecord.Response));
                var childNode = thisNode.Children[i];
                BuildFromRecords(records, ref childNode, childRecord );
            }
            
        }

        public static DialogueTree Open(string path)
        {
            var records = ReadRecords(path);
            var rootRecord = records.Find(x=>x.ParentId == string.Empty);
            if(rootRecord==null) throw new BadTreeException("No valid root node found");
            ITreeNode<DialogueOption> rootNode = new DialogueTreeNode(new DialogueOption(rootRecord.PlayerDialog, rootRecord.Response));
            BuildFromRecords(records, ref rootNode, rootRecord);
            return new DialogueTree(new DialogueTreeNode(rootNode));
        }

        public void Save(string path)
        {
            var records = GenerateRecords();

            // Generate the csv text
            string outputText = "";
            for(int i  = 0; i < records.Length; i++) // can't handle \n in any fields!
            {
                outputText += records[i].ToString();
                if(i < records.Length - 1) outputText += "\n";
            }

            // Write csv text to file
            using (FileStream fs = File.Create(path))
            {
                // writing data in string
                byte[] info = new UTF8Encoding(true).GetBytes(outputText);
                fs.Write(info, 0, info.Length);

                // writing data in bytes already
                byte[] data = new byte[] { 0x0 };
                fs.Write(data, 0, data.Length);
            }
        }

    }
}
