using LeggyDialogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeggyDialogueLib
{
    public class DialogueNodeRecord : DialogueOption
    {
        public string Id, ParentId;
        public List<string> childIds = new List<string>();
        public DialogueNodeRecord(string id, DialogueOption dialogue) : base(dialogue.PlayerDialog, dialogue.Response)
        {
            Id = id;
            ParentId = string.Empty;
        }

        public override string ToString() // can't handle commas in any fields - big problem
        {
            string output = Id + "," + ParentId+","+PlayerDialog+","+Response+",";
            for(int i = 0; i < childIds.Count; i++)
            {
                if(i> 0) output += " ";
                output += childIds[i];
            }
            return output;
        }

        public static DialogueNodeRecord FromString(string record)
        {
            var splitRecord = record.Split(",");
            DialogueNodeRecord output = new DialogueNodeRecord(splitRecord[0], new DialogueOption(splitRecord[2], splitRecord[3]));
            output.ParentId = splitRecord[1];
            output.childIds = splitRecord[4].Split(" ").ToList();
            int tempOut = 0;
            output.childIds.RemoveAll(x => !int.TryParse(x, out tempOut)); // maybe just store the ids as integers instead but this should work for now
            return output;
        }
    }
}
