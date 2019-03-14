using System;
using System.Collections.Generic;

namespace PaaminnerPaal.Dialogs
{
    [Serializable]
    public sealed class ContextStates
    {
        private static readonly Lazy<ContextStates> Lazy =
            new Lazy<ContextStates>(() => new ContextStates());

        private ContextStates()
        {
            RegisteredAllergies = new List<string>();
            DisplayedEntities = new List<int>();
            Entities = new List<string>();
            ContextIntent = "";
        }

        public void ResetStates()
        {
            //RegisteredAllergies.Clear();
            DisplayedEntities.Clear();
            Entities.Clear();
            ContextIntent = "";
        }

        public static ContextStates Instance => Lazy.Value;

        public string Json { get; set; }
        public string ContextIntent { get; set; }
        public bool OnlyCancel { get; set; }

        public List<string> RegisteredAllergies { get; }
        public List<int> DisplayedEntities { get; }
        public List<string> Entities { get; }
    }
}