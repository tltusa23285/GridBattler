using GBGame.Data;
using System.Collections.Generic;
using UnityEngine;

namespace GBGame
{
    public class ActiveActionList
    {
        private readonly ActionList DefaultList;
        private List<string> Actions;
        private List<string> InHand;
        private List<string> Used;
        public ActiveActionList(ActionList list) 
        { 
            DefaultList = list;
            Actions = new List<string>(list.Actions);
            InHand = new List<string>();
            Used = new List<string>();
            Shuffle(3);
        }

        public void Shuffle(int iterations = 1)
        {
            int r;
            string temp;
            for (int n = 0; n < iterations; n++)
            {
                for (int i = 0; i < Actions.Count; i++)
                {
                    temp = Actions[i];
                    r = Random.Range(i, Actions.Count);
                    Actions[i] = Actions[r];
                    Actions[r] = temp;
                }
            }
        }
    }
}
