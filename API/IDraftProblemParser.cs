using System;
using System.Collections.Generic;

namespace MusicProcessor.API
{
    interface IDraftProblemParser
    {
        IReadOnlyList<Tuple<string, int>> Parse(string problemPath, int defaultSampleLength);
    }
}
