﻿using System;
using System.Collections.Generic;


namespace IngameScript
{
    partial class Program
    {
        public partial class Clip
        {
            public static Clip Load(string[] lines, ref int line)//, out string curPath)
            { 
                //curPath     = "";

                if (lines.Length < 3)
                    return null;

                var clip = new Clip(null);

                var cfg = lines[line++].Split(';');
                if (!clip.LoadConfig  (cfg))             return null;//, out curPath)) return null;
                if (!clip.LoadChords  (lines[line++]))   return null;
                if (!clip.LoadMems    (lines[line++]))   return null;
                if (!clip.LoadPatterns(lines, ref line)) return null;
                if (!clip.LoadBlocks  (lines[line++]))   return null;

                clip.UpdateAutoKeys();

                return clip;
            }


            bool LoadToggles(string toggles)
            {
                uint f;
                if (!uint.TryParse(toggles, out f)) return F;

                var i = 0;

                Loop       = ReadBit(f, i++);
                Block      = ReadBit(f, i++);
                AllPats    = ReadBit(f, i++);
                Follow     = ReadBit(f, i++);
                AutoCue    = ReadBit(f, i++);

                MovePat    = ReadBit(f, i++);

                In         = ReadBit(f, i++);
                Out        = ReadBit(f, i++);
            
                AllChan    = ReadBit(f, i++);
                RndInst    = ReadBit(f, i++);
            
                Piano      = ReadBit(f, i++);
            
                Transpose  = ReadBit(f, i++);
                Spread     = ReadBit(f, i++);

                Shift      = ReadBit(f, i++);
                MixerShift = ReadBit(f, i++);
            
                Hold       = ReadBit(f, i++);
                Pick       = ReadBit(f, i++);

                ChordMode  = ReadBit(f, i++);
                ChordEdit  = ReadBit(f, i++);
                ChordAll   = ReadBit(f, i++);

                HalfSharp  = ReadBit(f, i++);

                ParamKeys  = ReadBit(f, i++);
                ParamAuto  = ReadBit(f, i++);
                       
                MemSet     = ReadBit(f, i++);

                return T;
            }


            bool LoadConfig(string[] cfg)//, out string curPath)
            {
                //curPath = "";

                var c = 0;

                Name = cfg[c++].Replace("\u0085", "\n");

                LoadToggles(cfg[c++]);

                if (!int  .TryParse(cfg[c++], out CurPat         )) return F;
                if (!int  .TryParse(cfg[c++], out CurChan        )) return F;
                                                                 
                if (!int  .TryParse(cfg[c++], out SelChan        )) return F;
                if (!int  .TryParse(cfg[c++], out CurSrc         )) return F;
                                                             
                //curPath = cfg[c++];                              
                                                             
                if (!int  .TryParse(cfg[c++], out EditStepIndex  )) return F;
                if (!int  .TryParse(cfg[c++], out EditLengthIndex)) return F;
                                                           
                if (!int  .TryParse(cfg[c++], out CurNote        )) return F;
                                                                 
                if (!int  .TryParse(cfg[c++], out Chord          )) return F;
                if (!int  .TryParse(cfg[c++], out ChordSpread    )) return F;
                                                                 
                if (!int  .TryParse(cfg[c++], out SongOff        )) return F;
                if (!int  .TryParse(cfg[c++], out InstOff        )) return F;
                if (!int  .TryParse(cfg[c++], out SrcOff         )) return F;
                                                                 
                if (!int  .TryParse(cfg[c++], out Solo           )) return F;
                                                                 
                if (!float.TryParse(cfg[c++], out Volume         )) return F;
                                                                 
                if (!int  .TryParse(cfg[c++], out ColorIndex     )) return F;

                return T;
            }


            bool LoadPatterns(string[] lines, ref int line)
            {
                int nPats = int.Parse(lines[line++]);

                for (int p = 0; p < nPats; p++)
                {
                    int i = 0;
                    var pat = Pattern.Load(lines[line++].Split(';'), ref i);
                    if (!OK(pat)) return F;

                    pat.Clip = this;

                    Patterns.Add(pat);
                }

                return T;
            }


            bool LoadBlocks(string line)
            {
                var data = line.Split(';');
                var i    = 0;

                Blocks.Clear();

                int nBlocks = int.Parse(data[i++]);

                for (int b = 0; b < nBlocks; b++)
                {
                    int first = int.Parse(data[i++]);
                    int last  = int.Parse(data[i++]);

                    Blocks.Add(new Block(first, last));
                }

                return T;
            }


            bool LoadMems(string line)
            {
                var mems = line.Split(';');

                for (int m = 0; m < nMems; m++)
                    if (!int_TryParse(mems[m], out Mems[m])) return F;

                return T;
            }


            bool LoadChords(string strChords)
            {
                Chords = new List<int>[4];

                for (int _c = 0; _c < Chords.Length; _c++)
                    Chords[_c] = new List<int>();


                var chords = strChords.Split(';');

                for (int _c = 0; _c < chords.Length; _c++)
                { 
                    var _keys = chords[_c].Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);

                    Chords[_c] = new List<int>();

                    int key;
                    foreach (var k in _keys)
                    {
                        if (!int_TryParse(k, out key)) return F;
                        Chords[_c].Add(key);
                    }
                }


                return T;
            }
        }
    }
}
