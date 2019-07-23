
using System.IO;
using FreakshowStudio.MarkovWordLists.Runtime;
using UnityEditor.Experimental.AssetImporters;

using UnityEngine;


namespace FreakshowStudio.MarkovWordLists.Editor
{
    [ScriptedImporter(1, "markov")]
    public class MarkovWordlistImporter : ScriptedImporter
    {
        #region Inspector Variables
        #pragma warning disable 0649

        [SerializeField]
        private int _order = 1;

        [SerializeField]
        private char _startCharacter = '^';

        [SerializeField]
        private char _endCharacter = '.';

        #endregion // Inspector Variables
        #pragma warning restore 0649

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var words = File.ReadLines(ctx.assetPath);

            var asset = MarkovWordlist.FromData(
                _order,
                words,
                _startCharacter,
                _endCharacter);

            ctx.AddObjectToAsset("MarkovWordlist", asset);
            ctx.SetMainObject(asset);
        }
    }
}
