using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class ExamplePlayer : ISaveable<ExamplePlayer>
    {
        [SerializeField] private PlayerLevel level;
        [SerializeField] private string exampleSerializedField = "This field is serialized.";
        [SerializeField] private string testString = "This object was created from scratch and has not been saved.";

        public int pubVarExample = 5;
        public string pubVarExampleStr = "";

        private double fullPrivateField = 13.465747;


        public ExamplePlayer SaveToFile()
        {
            // do saving stuff
            testString = $"This object was last saved at {System.DateTime.Now}";
            fullPrivateField = Mathf.PI;
            return this;
        }

        public void LoadFromFile(ExamplePlayer toLoad)
        {
            this.level = toLoad.level;
            this.exampleSerializedField = toLoad.exampleSerializedField;
            this.testString = toLoad.testString;
            this.pubVarExample = toLoad.pubVarExample;
            this.pubVarExampleStr = toLoad.pubVarExampleStr;
            // do loading stuff
        }
    }

    public class UnrelatedClass : ISaveable<UnrelatedClass>
    {
        public void LoadFromFile(UnrelatedClass toLoad)
        {
            // this class also loads things
        }

        public UnrelatedClass SaveToFile()
        {
            // this class also saves things
            return this;
        }
    }
}
