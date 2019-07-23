
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Assertions;

using Random = System.Random;


namespace FreakshowStudio.MarkovWordLists.Runtime
{
    public class MarkovWordlist
        : ScriptableObject,
            ISerializationCallbackReceiver
    {
        [Serializable]
        private struct ChainElement
        {
            [SerializeField]
            public string key;

            [SerializeField]
            public string value;

            [SerializeField]
            public double probability;
        };

        [SerializeField]
        private int _order;

        [SerializeField]
        private char _startCharacter;

        [SerializeField]
        private char _endCharacter;

        [SerializeField]
        private ChainElement[] _serializedChain;

        private Dictionary<string, Dictionary<string, double>> _chain =
            new Dictionary<string, Dictionary<string, double>>();

        private Random _random = new Random();

        public static MarkovWordlist FromData(
            int order,
            IEnumerable<string> words,
            char startCharacter,
            char endCharacter)
        {
            var asset = CreateInstance<MarkovWordlist>();

            asset._order = order;
            asset._startCharacter = startCharacter;
            asset._endCharacter = endCharacter;

            asset.GenerateChain(words);

            return asset;
        }

        private void GenerateChain(
            IEnumerable<string> words)
        {
            _chain.Clear();

            string endString = new string(_endCharacter, 1);

            var chain = new Dictionary<string, Dictionary<string, int>>();

            foreach (var word in words)
            {
                var lowercaseWord = word.ToLowerInvariant();

                for (int orderIdx = 0; orderIdx < _order; ++orderIdx)
                {
                    int orderLength = orderIdx + 1;
                    string startString = new string(
                        _startCharacter, orderLength);

                    var paddedWord =
                        $"{startString}{lowercaseWord}{endString}";

                    var paddedWordLength = paddedWord.Length;

                    for (int wordIdx = 0;
                         wordIdx < paddedWordLength - orderLength;
                         ++wordIdx)
                    {
                        var key = paddedWord.Substring(
                            wordIdx,
                            orderLength);

                        var value = paddedWord.Substring(
                            wordIdx + orderLength,
                            1);

                        if (chain.ContainsKey(key))
                        {
                            if (chain[key].ContainsKey(value))
                            {
                                chain[key][value]++;
                            }
                            else
                            {
                                chain[key].Add(value, 1);
                            }
                        }
                        else
                        {
                            chain.Add(key, new Dictionary<string, int>());
                            chain[key].Add(value, 1);
                        }
                    }
                }
            }

            _chain = NormalizeChain(chain);
        }

        private Dictionary<string, Dictionary<string, double>> NormalizeChain(
            Dictionary<string,Dictionary<string,int>> chain)
        {
            var normalizedChain = 
                new Dictionary<string, Dictionary<string, double>>();

            foreach (var kvp in chain)
            {
                var key = kvp.Key;
                var row = kvp.Value;
                var sum = row.Values.Sum();

                var newRow = row.ToDictionary(
                    pair => pair.Key,
                    pair => (double)pair.Value / sum);
                    
                normalizedChain[key] = newRow;
            }

            return normalizedChain;
        }

        public void SetRandomSeed(int seed)
        {
            _random = new Random(seed);
        }

        public string PickNext(string key)
        {
            bool hasKey = _chain.ContainsKey(key);
            while (!hasKey)
            {
                if (key.Length == 1)
                {
                    return new string(_endCharacter, 1);
                }

                key = key.Substring(1, key.Length - 1);
                hasKey = _chain.ContainsKey(key);
            }

            var row = _chain[key];
            var r = _random.NextDouble();
            var n = 0.0;

            foreach (var kvp in row)
            {
                n += kvp.Value;
                if (r < n)
                {
                    return kvp.Key;
                }
            }

            return row.Last().Key;
        }

        public string GenerateName(
            int minLength,
            int maxLength)
        {
            StringBuilder sb = new StringBuilder();

            var start = new string(_startCharacter, _order);
            sb.Append(start);

            while (true)
            {
                var next = PickNext(start);
                sb.Append(next);

                var currentName = sb.ToString();
                var currentLength = currentName.Length;

                var trimmedName = new []
                {
                    new string(_startCharacter, 1),
                    new string(_endCharacter, 1),
                }.Aggregate(
                    currentName, (s1, s2) => 
                        s1.Replace(s2, string.Empty));

                var trimmedLength = trimmedName.Length;

                var naturalLength = currentName.LastIndexOf(_endCharacter);

                if (trimmedLength >= maxLength)
                {
                    trimmedName = trimmedName.Substring(
                        0,
                        maxLength);

                    return trimmedName;
                }

                if (naturalLength >= 0 &&
                    trimmedLength >= minLength)
                {
                    return trimmedName;
                }

                if (naturalLength >= 0)
                {
                    currentName = currentName.Substring(
                        0,
                        naturalLength - 1);
                    currentLength = currentName.Length;

                    Assert.IsTrue(currentName.Length > 0);
                }

                var startIdx = currentLength - _order;
                var length = _order;
                if (startIdx < 0)
                {
                    length += startIdx;
                    if (length < 0)
                    {
                        length = 1;
                    }

                    startIdx = 0;
                }

                start = currentName.Substring(
                    startIdx, length);
            }
        }

        public void OnBeforeSerialize()
        {
            var flatChain =
                from keys in _chain
                from values in keys.Value
                select new ChainElement()
                {
                    key = keys.Key,
                    value = values.Key,
                    probability = values.Value,
                };

            _serializedChain = flatChain.ToArray();
        }

        public void OnAfterDeserialize()
        {
            _chain = _serializedChain
                .GroupBy(e => e.key)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(
                        d => d.value,
                        d => d.probability));
        }
    }
}
