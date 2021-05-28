﻿using System;
using System.Collections.Generic;


namespace IngameScript
{
    partial class Program
    {
        public partial class Clip
        {
            public string        Name;
            public Track         Track;

            public List<Pattern> Patterns;
            public List<Block>   Blocks;

            public List<Key>[]   ChannelAutoKeys = new List<Key>[g_nChans];


            public int           Length;

            public float         EditPos,
                                 LastEditPos;
            
            public List<Note>    EditNotes;
            public Note          Inter;


            public bool[]        ChanOn = new bool[g_nChans];


            public bool          Recording;

            public bool          Loop,
                                 Block,
                                 AllPats,
                                 Follow,
                                 AutoCue;

            public bool          AllChan,
                                 RndInst,
                                 
                                 Piano,
                                 
                                 Transpose  = False,
                                 Spread     = False,
                                        
                                 Shift      = False,
                                 
                                 Hold,
                                 Pick,
                                 
                                 ChordMode,
                                 ChordEdit,
                                 ChordAll,
                                 HalfSharp,
                                 
                                 ParamKeys,
                                 ParamAuto,
                                 
                                 MemSet = False;
                                 
                                 
            public int           ChordSpread;
                                 
            public bool          In, 
                                 Out,
                                 
                                 MovePat;
                                 
            public float         Volume;
                           

            public int           CurPat,
                                 CurChan,
                                 SelChan = -1,
                                 CurSrc  = -1,
                                 CurSet  = -1;
                                 
            public int           EditStepIndex   = 2;
            public int           EditLengthIndex = 2;
                                 
                                 
            public List<int>[]   Chords; // = new List<int>[4];
                                 
            public int           Chord,
                                 CurNote,
                                 
                                 SongOff,
                                 InstOff,
                                 SrcOff,
                                 
                                 Solo = -1;
                                 
                                 
            public int           ColorIndex;
                                 
                                 
            public int[]         Mems = new int[nMems];
                                 

            public Pattern       CurPattern     => Patterns[CurPat];
            public Channel       CurChannel     => CurPattern.Channels[CurChan];
            public Instrument    CurInstrument  => CurChannel.Instrument;
            public Channel       SelChannel     => SelChan > -1 ? CurPattern.Channels[SelChan] : Channel_null;
            public Instrument    SelInstrument  => SelChannel?.Instrument ?? Instrument_null;
            public Source        SelSource      => CurSrc > -1 ? SelInstrument.Sources[CurSrc] : Source_null;


            public float         EditStep       => g_steps[EditStepIndex  ];
            public float         EditStepLength => g_steps[EditLengthIndex];
            public int           EditLength     => (int)(EditStepLength * TicksPerStep);


            public Clip(Track track, string name = "Clip")
            {
                Track       = track;
                Name        = name;
                            
                Length      = -1;
                            
                Patterns    = new List<Pattern>();

                Blocks      = new List<Block>();

                for (int i = 0; i < ChannelAutoKeys.Length; i++)
                    ChannelAutoKeys[i] = new List<Key>();

                EditNotes       = new List<Note>();
                                
                Recording       = 
                Loop            =
                Block           =
                AllPats         =
                Follow          =
                AutoCue         = False;
                                
                MovePat         = 
                                
                In              = 
                Out             = 
                                
                AllChan         = 
                RndInst         = 
                                
                Piano           = 
                                
                Shift           = 
                MixerShift      = 
                                
                Hold            = 
                Pick            = 
                                
                ChordMode       =
                ChordEdit       =
                ChordAll        = 
                                
                HalfSharp       =
                                
                ParamKeys       = 
                ParamAuto       =
                                
                MemSet          = False;
                                
                CurPat          =  
                CurChan         = 0;
                                
                SelChan         = 
                CurSrc          = 
                CurSet          = -1;
                           
                EditStepIndex   =  
                EditLengthIndex = 2;
                         
                CurNote         = 69 * NoteScale;
                                
                Chord           = -1;
                ChordSpread     =  
                                
                SongOff         =  
                InstOff         =  
                SrcOff          = 0;
                                
                Solo            = -1;
                                
                Volume          = 1;
                                
                ColorIndex      = 4;


                for (int m = 0; m < nMems; m++)
                    Mems[m] = -1;


                Chords = new List<int>[4];

                for (int i = 0; i < Chords.Length; i++)
                    Chords[i] = new List<int>();


                ResetState();
            }


            public Clip(Clip clip, Track track)
            {
                Name     = clip.Name;
                Track    = track;

                Length   = clip.Length;

                Patterns = new List<Pattern>();
                foreach (var pat in clip.Patterns)
                { 
                    Patterns.Add(new Pattern(pat));
                    Patterns.Last().Clip = this;
                }

                Blocks = new List<Block>();
                foreach (var b in clip.Blocks)
                    Blocks.Add(new Block(b));

                for (int i = 0; i < ChannelAutoKeys.Length; i++)
                    ChannelAutoKeys[i] = new List<Key>(clip.ChannelAutoKeys[i]);

                EditNotes       = new List<Note>();
                                
                Loop            = clip.Loop;
                Block           = clip.Block;
                AllPats         = clip.AllPats;
                Follow          = clip.Follow;
                AutoCue         = clip.AutoCue;
                                
                MovePat         = clip.MovePat;
                                
                In              = clip.In;
                Out             = clip.Out;
                                
                AllChan         = clip.AllChan;
                RndInst         = clip.RndInst;
                                
                Piano           = clip.Piano;
                                
                Shift           = clip.Shift;
                                
                Hold            = clip.Hold;
                Pick            = clip.Pick;
                                
                ChordMode       = clip.ChordMode;
                ChordEdit       = clip.ChordEdit;
                ChordAll        = clip.ChordAll;
                                
                HalfSharp       = clip.HalfSharp;
                                
                ParamKeys       = clip.ParamKeys;
                ParamAuto       = clip.ParamAuto;
                                
                MemSet          = clip.MemSet;
                                
                CurPat          = clip.CurPat;
                CurChan         = clip.CurChan;
                SelChan         = clip.SelChan;
                CurSrc          = clip.CurSrc;
                CurSet          = clip.CurSet;

                EditStepIndex   = clip.EditStepIndex;
                EditLengthIndex = clip.EditLengthIndex;

                CurNote         = clip.CurNote;
                                
                Chord           = clip.Chord;
                ChordSpread     = clip.ChordSpread;
                                
                SongOff         = clip.SongOff;
                InstOff         = clip.InstOff;
                SrcOff          = clip.SrcOff;
                                
                Solo            = clip.Solo;
                                
                Volume          = clip.Volume;
                                
                ColorIndex      = clip.ColorIndex;


                for (int m = 0; m < nMems; m++)
                    Mems[m] = -1;


                Chords = new List<int>[4];

                for (int i = 0; i < Chords.Length; i++)
                    Chords[i] = new List<int>();


                ResetState();
            }


            public void ClearAudoKeys()
            {
                foreach (var keys in ChannelAutoKeys)
                    keys.Clear();
            }


            public void UpdateAutoKeys()
            {
                for (int ch = 0; ch < g_nChans; ch++)
                { 
                    var chanKeys = ChannelAutoKeys[ch];

                    chanKeys.Clear();

                    for (int p = 0; p < Patterns.Count; p++)
                    {
                        var keys = Patterns[p].Channels[ch].AutoKeys;

                        for (int k = 0; k < keys.Count; k++)
                        { 
                            chanKeys.Add(new Key(
                                keys[k].SourceIndex,
                                keys[k].Parameter,
                                keys[k].Value, 
                                keys[k].StepTime + p*g_patSteps,
                                keys[k].Channel));
                        }
                    }

                    chanKeys.Sort((a, b) => a.StepTime.CompareTo(b.StepTime));
                }
            }


            public void Clear()
            {
                Name = "";

                Patterns.Clear();
                Blocks.Clear();

                foreach (var keys in ChannelAutoKeys)
                    keys.Clear();

                ResetState();
            }


            void ResetState()
            {
                EditPos     = float_NaN;
                LastEditPos = float_NaN;

                Inter       = Note_null;
                EditNotes.Clear();
            }


            public int   GetKeyPat(Key key) { return Patterns.FindIndex(p => OK(Array.Find(p.Channels, c => c.AutoKeys.Contains(key)))); }
            public float GetStep  (Key key) { return GetKeyPat(key) * g_patSteps + key.StepTime; }


            public Block GetBlock(int pat)
            {
                return Blocks.Find(b =>
                       pat >= b.First
                    && pat <= b.Last);
            }


            public void SetCue()
            {
                Track.NextPat = Track.NextPat == CurPat ? -1 : CurPat;
            }


            public void GetPosLimits(int pat, out int start, out int end)
            {
                int first, last;
                GetPlayPatterns(pat, out first, out last);

                start =  first     * g_patSteps;
                end   = (last + 1) * g_patSteps;
            }


            public void GetPlayPatterns(int p, out int f, out int l)
            {
                if (Loop)
                {
                    f = p;
                    l = p;

                    var b = GetBlock(p);

                    if (Block && OK(b))
                    {
                        f = b.First;
                        l = b.Last;
                    }
                }
                else
                {
                    f = 0;
                    l = Patterns.Count-1;
                }
            }


            public void TrimCurrentNotes(int ch = -1)
            {
                var timeStep = IsPlaying ? PlayStep : TimeStep;

                foreach (var note in g_notes)
                {
                    if (   ch < 0
                        || note.iChan == ch)
                    { 
                        var noteStep = IsPlaying ? note.SongStep : note.Step;
                        note.UpdateStepLength(timeStep - noteStep);
                    }
                }
            }


            public void WrapCurrentNotes(int nWrapSteps)
            {
                var timeStep = IsPlaying ? PlayStep : TimeStep;

                foreach (var note in g_notes)
                {
                    var noteStep = IsPlaying ? note.SongStep : note.Step;

                    if (   timeStep >= noteStep
                        && timeStep <  noteStep + note.StepLength)
                        note.UpdateStepTime(-nWrapSteps);
                }
            }


            public void StartBlock()
            {
                var b = GetBlock(CurPat);

                if (!OK(b))
                {
                    Blocks.Add(new Block(CurPat));

                    In     = True;
                    Follow = False;
                }
                else
                {
                    In = !In;

                    if (In)
                    {
                        Out    = False;
                        Follow = False;
                    }
                }

                MovePatternOff();
            }


            public void EndBlock()
            {
                var b = GetBlock(CurPat);

                if (!OK(b))
                {
                    Blocks.Add(new Block(CurPat));

                    Out    = True;
                    Follow = False;
                }
                else
                {
                    Out = !Out;

                    if (Out)
                    {
                        In     = False;
                        Follow = False;
                    }

                    //g_blocks[b].Next = currentPattern + 1;

                    //if (g_blocks[b].Next == g_blocks[b].Start)
                    // g_blocks[b].Next = g_blocks[b].Start + 1;
                    //else if (g_blocks[b].Next < g_blocks[b].Start)
                    // Swap(ref g_blocks[b].Next, ref g_blocks[b].Start);
                }
            }
        

            public void ClearBlock()
            {
                Blocks.Remove(GetBlock(CurPat));
                DisableBlock();
                MovePatternOff(); 
                g_lcdPressed.Add(lcdClip+11);
            }


            public void DisableBlock()
            {
                In  = False;
                Out = False;
            }


            public void MovePatternOff()
            {
                MovePat = False;
            }


            public void ToogleLoop()
            {
                Loop = !Loop;
            }


            public void ToggleMovePattern()
            {
                MovePat = !MovePat;

                if (MovePat)
                    DisableBlock();
            }


            public void ToggleBlock()
            {
                Block = !Block;
            }


            public void ToggleAllPatterns()
            {
                AllPats = !AllPats;
            }


            public void ToggleFollow()
            {
                Follow = !Follow;

                if (Follow)
                {
                    AutoCue = False;
                }
            }


            public void ToggleAutoCue()
            {
                AutoCue = !AutoCue;

                if (AutoCue)
                {
                    Cue();
                    Follow = False;
                }
            }


            public void GetCurPatterns(out int first, out int last)
            {
                GetPatterns(CurPat, out first, out last);
            }


            public void GetPatterns(int pat, out int first, out int last)
            {
                first = pat;
                last  = pat;

                if (AllPats)
                {
                    var b = GetBlock(pat);

                    if (   Block
                        && OK(b))
                    {
                        first = b.First;
                        last  = b.Last;
                    }
                    else
                    {
                        first = 0;
                        last  = Patterns.Count-1;
                    }
                }
            }


            public void StopEdit()
            {
                if (EditNotes.Count > 0)
                    Hold = False;

                EditNotes.Clear();
            }


            public void LimitRecPosition()
            {
                int st, nx;
                GetPosLimits(CurPat, out st, out nx);

                     if (EditPos >= nx) EditPos -= nx - st;
                else if (EditPos <  st) EditPos += nx - st;

                var cp = (int)(EditPos / g_patSteps);
                if (cp != CurPat) SetCurrentPattern(cp);
            }
        }
    }
}
