using MusicProcessor.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicProcessor.Processor
{
    class SamplesLibrary : ISamplesLibrary
    {
        private readonly IDictionary<string, string> _samplesToPath = new Dictionary<string, string>();

        public IEnumerable<Tuple<string, string>> Entries => _samplesToPath.Select(e => Tuple.Create(e.Key, e.Value)).OrderBy(e => e.Item1);

        public IEnumerable<string> Validate(IEnumerable<Tuple<string, int>> samples)
        {
            return samples.Select(s => s.Item1).Where(s => !_samplesToPath.ContainsKey(s)).Distinct().OrderBy(s => s);
        }

        public string FindSamplePath(string sampleName)
        {
            _samplesToPath.TryGetValue(sampleName, out string result);
            return result;
        }
        public void Load(string path)
        {
            _samplesToPath.Clear();
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            foreach (var (file, name) in from string file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                         .Where(s => s.EndsWith(".mp3") || s.EndsWith(".wav"))
            let name = Path.GetFileNameWithoutExtension(file)
                                         where !_samplesToPath.ContainsKey(name)
                                         select (file, name))
            {
                _samplesToPath.Add(name, file);
            }
        }
    }
}
