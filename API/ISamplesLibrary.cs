using System;
using System.Collections.Generic;

namespace MusicProcessor.API
{
    public interface ISamplesLibrary
    {
        IEnumerable<string> Validate(IEnumerable<Tuple<string, int>> samples);

        string FindSamplePath(string sampleName);

        IEnumerable<Tuple<string, string>> Entries { get; }

        void Load(String path);
    }
}
