using MusicProcessor.API;
using NAudio.Lame;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace MusicProcessor.Processor
{
    class AudioPipeline
    {
        private readonly ISamplesLibrary _samplesLibrary;
        private readonly IReadOnlyList<Tuple<string, int>> _sampleToLength;
        private readonly IProgressHandler _progressHandler;
        private readonly bool _useCustomSampleLength;

        public AudioPipeline(ISamplesLibrary samplesLibrary, IReadOnlyList<Tuple<string, int>> sampleToLength, IProgressHandler progressHandler, bool useCustomSampleLength)
        {
            _samplesLibrary = samplesLibrary;
            _sampleToLength = sampleToLength;
            _progressHandler = progressHandler;
            _useCustomSampleLength = useCustomSampleLength;
        }

        public void Process(string outputFile)
        {
            _progressHandler.Reset();
            _progressHandler.SetMax(_sampleToLength.Count);

            float[] buf = new float[1024];
            using (FileStream fileWriter = new FileStream(outputFile, FileMode.Create))
            {
                using (WaveFileWriter audioWriter = new WaveFileWriter(fileWriter, new WaveFormat()))
                {
                    int progress = 0;
                    foreach (var pair in _sampleToLength)
                    {
                        string path = _samplesLibrary.FindSamplePath(pair.Item1);
                        if (string.IsNullOrEmpty(path))
                        {
                            MessageBox.Show(string.Format("Файл сэмпла с именем '{0}' не найден в библиотеке сэмплов.", pair.Item1), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            continue;
                        }

                        using (MediaFoundationReader pcmStream = new MediaFoundationReader(path))
                        {
                            double sampleLengthInMs = _useCustomSampleLength ? pair.Item2 : pcmStream.TotalTime.Milliseconds; // miliseconds
                            while (sampleLengthInMs > 0)
                            {
                                pcmStream.CurrentTime = TimeSpan.FromMilliseconds(sampleLengthInMs);
                                TimeSpan lengthInStream = pcmStream.CurrentTime;
                                pcmStream.CurrentTime = TimeSpan.FromMilliseconds(0);

                                var sampleProvider = (OffsetSampleProvider) pcmStream
                                    .ToSampleProvider()
                                    .ToStereo()
                                    .Take(lengthInStream);

                                int samplesLeft = sampleProvider.TakeSamples;
                                while (samplesLeft > 0)
                                {
                                    int read = sampleProvider.Read(buf, 0, buf.Length);
                                    if (read == 0)
                                    {
                                        break;
                                    }

                                    samplesLeft -= read;
                                    audioWriter.WriteSamples(buf, 0, read);
                                }

                                sampleLengthInMs -= lengthInStream.TotalMilliseconds;
                            }
                        }

                        _progressHandler.Report(++progress);
                    }
                    
                    _progressHandler.Report(_sampleToLength.Count);
                }
            }

            using (FileStream wavFileStream = new FileStream(outputFile, FileMode.Open))
            using (FileStream mp3FileStream = new FileStream(outputFile + ".mp3", FileMode.Create))
            {
                ConvertWavToMp3(wavFileStream, mp3FileStream);
            }
        }

        public void ConvertWavToMp3(FileStream wavFileStream, FileStream mp3FileStream)
        {
            using (var rdr = new WaveFileReader(wavFileStream))
            using (var wtr = new LameMP3FileWriter(mp3FileStream, rdr.WaveFormat, 128))
            {
                rdr.CopyTo(wtr);
            }
        }
    }
}
