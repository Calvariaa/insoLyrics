﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace insoLyrics.Beatmap.Formats
{
    using Predicate = Predicate<string>;
    internal abstract class BeatmapDecoder
    {
        private static readonly List<Tuple<Predicate, Type>> decoders = new List<Tuple<Predicate, Type>>();

        static BeatmapDecoder()
        {
            OsuLegacyDecoder.Register();
        }

        public static BeatmapDecoder GetDecoder(StreamReader stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            string line;
            do
            {
                line = stream.ReadLine()?.Trim();
            }
            while (line != null && line.Length == 0);

            Type decoder = null;
            if (line != null)
            {
                decoder = decoders.FirstOrDefault(t => t.Item1.Invoke(line))?.Item2;
            }

            if (line == null || decoder == null)
            {
                throw new IOException(@"Unknown file format");
            }
            return (BeatmapDecoder)Activator.CreateInstance(decoder, line);
        }

        protected static void AddDecoder<T>(Predicate predict) where T : BeatmapDecoder
        {
            decoders.Add(new Tuple<Predicate, Type>(predict, typeof(T)));
        }

        public virtual BeatmapMetadata Decode(StreamReader stream)
        {
            return ParseFile(stream);
        }

        public virtual void Decode(StreamReader stream, BeatmapMetadata beatmap)
        {
            ParseFile(stream, beatmap);
        }

        protected virtual BeatmapMetadata ParseFile(StreamReader stream)
        {
            var beatmap = new BeatmapMetadata();

            ParseFile(stream, beatmap);
            return beatmap;
        }

        protected abstract void ParseFile(StreamReader stream, BeatmapMetadata beatmap);
    }
}
