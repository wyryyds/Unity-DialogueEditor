using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HAITool.DialogueEditor.RunTime
{
    public class DialogueManager
    {
        private static DialogueManager instance;



        public static DialogueManager Instance
        {
            get 
            {
                instance ??= new DialogueManager();
                return  instance;
            }
        }
    }
}
