
using UnityEngine;
using UnityEngine.Assertions;
using FreakshowStudio.MarkovWordLists.Runtime;


namespace FreakshowStudio.MarkovWordLists.Samples.RandomNameGeneration.Runtime
{
    public class NameGenerator : MonoBehaviour
    {
        #region Inspector Variables
        #pragma warning disable 0649

        [SerializeField]
        private MarkovWordlist _wordlist;

        [SerializeField]
        private int _minLength = 3;

        [SerializeField]
        private int _maxLength = 12;

        #endregion // Inspector Variables
        #pragma warning restore 0649

        private string _name = "";

        private void OnGUI()
        {
            if (GUI.Button(
                new Rect(10f, 10f, 300f, 50f),
                "Generate Name"))
            {
                Assert.IsTrue(_minLength > 0);
                Assert.IsTrue(_maxLength > 0);
                Assert.IsTrue(_maxLength > _minLength);

                _name = _wordlist.GenerateName(_minLength, _maxLength);
            }

            GUI.Label(
                new Rect(10f, 100f, 300f, 50f),
                _name);
        }
    }
}
