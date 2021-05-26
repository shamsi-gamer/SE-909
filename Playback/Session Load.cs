﻿using System.Text;
using System.Collections.Generic;


namespace IngameScript
{
    partial class Program
    {
        public partial class Session
        {
            public static Session Load(out int curClipTrack, 
                                       out int curClipIndex,
                                       out int copyClipTrack,
                                       out int copyClipIndex)
            {
                var session = new Session();

                if (!session.LoadSession(
                    out curClipTrack, 
                    out curClipIndex, 
                    out copyClipTrack, 
                    out copyClipIndex)) 
                    return null;

                if (!session.LoadInstruments()) session.CreateDefaultInstruments();
                if (!session.LoadTracks     ()) session.CreateDefaultTracks();

                return session;
            }


            bool LoadSession(out int curClipTrack, 
                             out int curClipIndex,
                             out int copyClipTrack,
                             out int copyClipIndex)
            {
                curClipTrack  = -1;
                curClipIndex  = -1;
                copyClipTrack = -1;
                copyClipIndex = -1;

                var sb = new StringBuilder();
                pnlStorageSession.ReadText(sb);

                if (!sb.ToString().Contains(";"))
                    return F;

                var state = sb.ToString().Split(';');
                var s = 0;

                Name = state[s++];

                LoadToggles(state[s++]);

                if (!int.TryParse(state[s++], out TicksPerStep))  return F;
                if (!int.TryParse(state[s++], out EditClip))      return F;
                if (!int.TryParse(state[s++], out curClipTrack))  return F;
                if (!int.TryParse(state[s++], out curClipIndex))  return F;
                if (!int.TryParse(state[s++], out copyClipTrack)) return F;
                if (!int.TryParse(state[s++], out copyClipIndex)) return F;

                return T;
            }


            bool LoadToggles(string toggles)
            {
                uint f;
                if (!uint.TryParse(toggles, out f)) return F;

                var i = 0;

                ShowSession = ReadBit(f, i++);
                Move        = ReadBit(f, i++);

                return T;
            }


            bool LoadInstruments()
            {
                Instruments.Clear();


                var sb = new StringBuilder();
                pnlStorageInstruments.ReadText(sb);

                var lines = sb.ToString().Split('\n');
                var line  = 0;


                while (line < lines.Length)
                {
                    SkipWhiteSpace(lines, ref line);

                    if (line < lines.Length)
                        Instruments.Add(Instrument.Load(this, lines, ref line));
                }

            
                return Instruments.Count > 0;
            }


            public void ImportInstruments()
            {
                LoadInstruments();

                // set all instruments to first

                foreach (var track in g_session.Tracks)
                {
                    foreach (var clip in track.Clips)
                    { 
                        if (!OK(clip))
                            continue;

                        int first, last;
                        clip.GetCurPatterns(out first, out last);

                        for (int p = first; p <= last; p++)
                        {
                            for (int ch = 0; ch < g_nChans; ch++)
                                clip.Patterns[p].Channels[ch].Instrument = g_session.Instruments[0];
                        }
                    }
                }
            }


            bool LoadTracks()
            {
                Tracks.Clear();

                var sb = new StringBuilder();
                pnlStorageInstruments.ReadText(sb);

                var lines = sb.ToString().Split('\n');
                var line  = 0;

                int nTracks;
                if (!int.TryParse(lines[line++], out nTracks)) return F;

                for (int t = 0; t < nTracks; t++)
                {
                    SkipWhiteSpace(lines, ref line);
                    var track = Track.Load(this, lines, ref line);

                    if (OK(track)) Tracks.Add(track);
                    else           return F;
                }


                return Tracks.Count > 0;
            }
        }
    }
}
