using MusicProcessor.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace MusicProcessor.Processor
{
    class DraftProblemParser : IDraftProblemParser
    {
        public IReadOnlyList<Tuple<string, int>> Parse(string problemPath, int defaultSampleLength)
        {
            List<Tuple<string, int>> list = new List<Tuple<string, int>>();
            using (FileStream fs = File.OpenRead(problemPath))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] pairs = Regex.Split(line, "[ ]{1,}");

                        foreach (string pair in pairs)
                        {
                            if (string.IsNullOrWhiteSpace(pair))
                            {
                                continue;
                            }

                            string[] sampleToLength = pair.Split(',');
                            if (sampleToLength.Length > 2)
                            {
                                MessageBox.Show("Формат сэмпл-длина заданный в файле не верен. Поддерживаемые форматы: имя_сэмпла или имя_сэмпла,длина_сэмпла", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return new List<Tuple<string, int>>().AsReadOnly();
                            }

                            int sampleLength = defaultSampleLength;
                            if (sampleToLength.Length == 2 && !int.TryParse(sampleToLength[1].Trim(), out sampleLength))
                            {
                                MessageBox.Show("Формат сэмпл-длина заданный в файле не верен. Поддерживаемые форматы: имя_сэмпла или имя_сэмпла(имя файлы без расширения),длина_сэмпла(число)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return new List<Tuple<string, int>>().AsReadOnly();
                            }

                            list.Add(Tuple.Create(sampleToLength[0].Trim(), sampleLength));
                        }
                    }
                }
            }

            return list.AsReadOnly();
        }
    }
}
