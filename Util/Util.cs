﻿using System;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;


namespace IngameScript
{
    public static class Extensions
    {
        public static T[] Subarray<T>(this T[] array, int offset, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }

        public static T    Last      <T>(this T[] array)         { return array[array.Length-1]; }
        public static T    Last      <T>(this List<T> list)      { return list[list.Count-1]; }

        public static void RemoveLast<T>(this List<T> list)      { list.RemoveAt(list.Count-1); }

        public static int  IndexOf   <T>(this T[] array, T item) { return Array.IndexOf(array, item); }
        public static bool Contains  <T>(this T[] array, T item) { return Array.IndexOf(array, item) > -1; }
    }


    partial class Program
    {                                                                              
        static List<Setting> g_settings   = new List<Setting>();

        static Setting   LastSetting  => g_settings.Count > 0 ? g_settings.Last()  : Setting_null;
        static Setting   CurSetting   => OK(CurSet)           ? g_settings[CurSet] : Setting_null;

        static Parameter CurParam     => (Parameter)CurSetting;
        static Modulate  CurModulate  => (Modulate) CurSetting;

        static Harmonics CurHarmonics => (Harmonics)CurSetting;
        static Harmonics CurOrParentHarmonics =>
            IsCurSetting(typeof(Harmonics))
            ? CurHarmonics
            : (Harmonics)CurSetting.Parent;


        static bool IsCurParam()
        {
            return IsParam(CurSetting);
        }


        static bool IsCurParam(string tag)
        {
            return HasTag(CurSetting, tag);
        }


        static bool IsCurSetting(Type type)
        {
            //return
            //       OK(CurSet)
            //    && g_settings[CurSet].GetType() == type;

            return CurSetting?.GetType() == type;
        }


        static void UpdateDspOffset(ref int off, int pos, int count, int max, int dOff1, int dOff2)
        {
            if (   pos >= max/2         + off
                || pos <  max/2 - dOff1 + off)
                off = pos - max/2 + dOff2;

                 if (max >= count      ) off = 0;
            else if (off >  count - max) off = count - max;
            else if (pos >= max + off  ) off = Math.Max(0, pos - max + 1);
            else if (pos <  off        ) off = pos;
            else if (off <  0          ) off = 0;
        }


        static void UpdateSongOff()
        {
            UpdateDspOffset(
                ref EditedClip.SongOff, 
                CurPat, 
                EditedClip.Patterns.Count, 
                maxDspPats, 
                1,
                1);
        }


        void UpdateInstOff(int ch)
        {
            var curInst = Instruments.IndexOf(CurPattern.Channels[ch].Instrument);
            UpdateDspOffset(ref EditedClip.InstOff, curInst, Instruments.Count, maxDspInst, 0, 1);
        }


        void UpdateSrcOff()
        {
            UpdateDspOffset(
                ref EditedClip.SrcOff, 
                CurSrc, 
                EditedClip.CurInstrument.Sources.Count, 
                maxDspSrc, 
                0,
                0);
        }


        void SetCurInst(Instrument inst)
        {
            int first, last;
            EditedClip.GetCurPatterns(out first, out last);

            for (int p = first; p <= last; p++)
                EditedClip.Patterns[p].Channels[CurChan].Instrument = inst;
        }


        bool IsModPresent()
        {
            return _loadStep < 10
                ||    (OscSine         ?.Samples.Count ?? 0) > 0
                   && (OscTriangle     ?.Samples.Count ?? 0) > 0
                   && (OscSaw          ?.Samples.Count ?? 0) > 0
                   && (OscSquare       ?.Samples.Count ?? 0) > 0
                   && (OscLowNoise     ?.Samples.Count ?? 0) > 0
                   && (OscHighNoise    ?.Samples.Count ?? 0) > 0
                   && (OscBandNoise    ?.Samples.Count ?? 0) > 0
                   && (OscSlowSweepDown?.Samples.Count ?? 0) > 0
                   && (OscFastSweepDown?.Samples.Count ?? 0) > 0
                   && (OscSlowSweepUp  ?.Samples.Count ?? 0) > 0
                   && (OscFastSweepUp  ?.Samples.Count ?? 0) > 0
                   && (OscCrunch       ?.Samples.Count ?? 0) > 0;
        }


        static string GetNewName(string name, Func<string, bool> exists)
        {
            var numLength = GetNumLength(name);

            if (numLength > 0)
            {
                var len = name.Length - numLength;
                var num = int_Parse(name.Substring(len));

                string newName = "";
                while (newName == "" || exists(newName))
                    newName = name.Substring(0, len) + S(++num);

                return newName;
            }
            else
            {
                if (exists(name)
                    && numLength == 0)
                    name += " 2";

                return name;
            }
        }


        static void UpdateClipName(Clip clip, Clip[] clips)
        {
            clip.Name = GetNewName(clip.Name, newName => 
                Array.Exists(clips, c => 
                       OK(c) 
                    && c.Name == newName));
        }


        static long GetAnyCurrentPlayTime()
        {
            var track = Tracks.Find(t => OK(t.PlayTime));

            return 
                OK(track) 
                ? track.PlayTime
                : long_NaN;
        }

        
        static bool EditedClipIsPlaying => EditedClip.Track.Clips.IndexOf(EditedClip) == EditedClip.Track.PlayClip;


        bool ShowPiano { get 
        {
            var tune = SelSource    ?.Tune
                    ?? SelInstrument?.Tune;

            return
                   EditedClip.Piano
                ||    EditedClip.ChordEdit 
                   && OK(EditedClip.Chord)
                ||    IsCurParam(strTune)
                   && (tune?.UseChord ?? False)
                   && !(   EditedClip.ParamKeys 
                        || EditedClip.ParamAuto);
        }}


        static void UpdateInstName(bool add = True)
        {
            if (    OK(SelChan)
                &&  OK(CurPat)
                && !OK(CurSrc)
                && !OK(CurSet)
                && OK(SelChan))
                dspMain.Panel.WriteText(add ? SelChannel.Instrument.Name : "", False);
        }


        float GetBPM()
        {
            return 120f / (TicksPerStep * g_patSteps) * 120f;
        }


        void Lock()
        {
            foreach (var l in g_locks)
                l.ToggleLock();
        }


        void NoiseEmitters()
        {
            NoiseEmitters(!g_timers[0].Enabled);
        }


        void NoiseEmitters(bool on)
        {
            g_lightPiston.Enabled = on;
            g_lightHinge1.Enabled = on;
            g_lightHinge2.Enabled = on;
            g_hingeL     .Enabled = on;
            g_hingeR     .Enabled = on;

            foreach (var timer in g_timers)
                timer.Enabled = on;
        }


        void Gyro()
        {
            var on = g_gyros[0].Enabled;

            foreach (var gyro in g_gyros)
                gyro.Enabled = !on;
        }


        void AutoLock()
        {
            var auto = False;

            foreach (var l in g_locks) auto |= l.AutoLock;
            foreach (var l in g_locks) l.AutoLock = !auto;
        }


        void ToggleLabel()
        {
            var open  = Get("Timer Open 1")  as IMyTimerBlock;
            var close = Get("Timer Close 1") as IMyTimerBlock;

            var p = g_lightPiston;

            if (   !OK(p)
                || !OK(open)
                || !OK(close))
                return;


            NoiseEmitters(True);


            if (p.CurrentPosition <= (p.MinLimit + p.MaxLimit) / 2) open .Trigger();
            else                                                    close.Trigger();
        }


        void ToggleFold()
        {
            var hinge = Get("Hinge R") as IMyMotorStator;

            var fold  = Get("Timer Fold 1")    as IMyTimerBlock;
            var recl  = Get("Timer Recline 1") as IMyTimerBlock;

            if (   !OK(fold)
                || !OK(recl))
                return;

            if (hinge.Angle > (hinge.LowerLimitRad + hinge.UpperLimitRad) / 2) fold.Trigger();
            else                                                               recl.Trigger();

            lblFold.Mark();
        }


        static long GetPatTime(int pat) 
        {
            return pat * g_patSteps * TicksPerStep; 
        } 


        static float note2freq(int note)
        {
            return 440 * (float)Math.Pow(2, (note/(float)NoteScale - 69) / 12f);
        }


        static int freq2note(double freq)
        {
            return (int)Math.Round((12 * Math.Log(freq / 440, 2) + 69) * NoteScale);
        }


        //static float dbAdd(float a, float b)
        //{
        //    return (float)(10 * Math.Log10(Math.Pow(10, a/10) + Math.Pow(10, b/10)));
        //}


        static float sndAdd(float a, float b)
        {
            return 1 + (float)Math.Log10(a + b);
        }


        static void Swap<T>(ref T clip1, ref T clip2) 
        {
            var swap  = clip2;
                clip2 = clip1;
                clip1 = swap;
        }


        bool TooComplex =>
               Runtime.CurrentCallChainDepth   / (float)Runtime.MaxCallChainDepth   > 0.8f
            || Runtime.CurrentInstructionCount / (float)Runtime.MaxInstructionCount > 0.8f;


        void             Get<T>(List<T> blocks)                          where T : class { GridTerminalSystem.GetBlocksOfType(blocks);            }
        void             Get<T>(List<T> blocks, Func<T, bool> condition) where T : class { GridTerminalSystem.GetBlocksOfType(blocks, condition); }

        IMyTerminalBlock Get       (string s)             { return GridTerminalSystem.GetBlockWithName(s); }
        IMyMotorBase     GetMotor  (string s)             { return Get(s) as IMyMotorBase; }
        IMyMotorBase     GetHinge  (string s)             { return GetMotor("Hinge " + s); }
        IMyTextPanel     GetLcd    (string s)             { return Get(s) as IMyTextPanel; }
        IMyTextPanel     GetLabel  (string s)             { return GetLcd("Label " + s); }
        IMyTextPanel     GetDisplay(string s, int i = -1) { return GetLcd(s + " Display" + (OK(i) ? strEmpty + S(i) : "")); }


        static void SkipWhiteSpace(string[] lines, ref int line)
        {
            while (line < lines.Length
                && lines[line].Trim() == "") line++;
        }


        static int   int_Parse(string str) => int.Parse(str);
        static bool  int_TryParse(string str, out int val) => int.TryParse(str, out val);

        static bool long_TryParse(string str, out long val)
        {
            if (str == "?")
            { 
                val = long_NaN;
                return True;
            }
            else
                return long.TryParse(str, out val);
        }


        static bool IsPressed(Label lbl) { return g_labelsPressed.Contains(lbl); }
        static bool IsPressed(int   lbl) { return    g_lcdPressed.Contains(lbl); }


        static int        CurPat          => EditedClip.CurPat;
                                          
        static int        CurChan         { get { return EditedClip.CurChan; } set { EditedClip.CurChan = value; } }
        static int        SelChan         { get { return EditedClip.SelChan; } set { EditedClip.SelChan = value; } }
        static int        CurSrc          { get { return EditedClip.CurSrc;  } set { EditedClip.CurSrc  = value; } }
        static int        CurSet          { get { return EditedClip.CurSet;  } set { EditedClip.CurSet  = value; } }
                                          
        static Pattern    CurPattern      => EditedClip.CurPattern;
        static Channel    CurChannel      => EditedClip.CurChannel;
        static Pattern    PlayPattern     => EditedClip.Patterns[EditedClip.Track.PlayPat];
                                          
        static Source     SelSource       => EditedClip.SelSource;
        static Instrument SelInstrument   => EditedClip.SelInstrument;
        static Channel    SelChannel      => EditedClip.SelChannel;

        static bool       SessionHasClips => Tracks.Exists(t => Array.Exists(t.Clips, c => OK(c)));
    }
}
