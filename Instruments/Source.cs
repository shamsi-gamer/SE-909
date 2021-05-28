﻿using System;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRageMath;


namespace IngameScript
{
    partial class Program
    {
        public class Source
        {
            public Instrument Instrument;
            public bool       On;
            
            public Oscillator Oscillator;

            public Parameter  Offset;
            public Parameter  Volume;
            public Tune       Tune;
            public Harmonics  Harmonics;
            public Filter     Filter;
            public Delay      Delay;

            public float      CurVolume;

            public int        Index => Instrument.Sources.IndexOf(this);


            public Source(Instrument inst)
            {
                Instrument = inst;
                On         = True;
                           
                Oscillator = OscSine;

                Offset     = Parameter_null;
                Volume     = (Parameter)NewSettingFromTag(strVol, Setting_null, inst, this);
                Tune       =      Tune_null;
                Harmonics  = Harmonics_null;
                Filter     =    Filter_null;
                Delay      =     Delay_null;

                CurVolume  = 0;
            }


            public Source(Source src, Instrument inst)
            {
                Instrument = inst;
                On         = src.On;

                Oscillator = src.Oscillator;
                
                Offset     = src.Offset   ?.Copy(Setting_null);
                Volume     = new Parameter(src.Volume, Setting_null);                               
                Tune       = src.Tune     ?.Copy();
                Harmonics  = src.Harmonics?.Copy();
                Filter     = src.Filter   ?.Copy();
                Delay      = src.Delay    ?.Copy();

                CurVolume  = src.CurVolume;
            }


            public string GetSample(int note)
            {
                return g_samples[Oscillator.Samples[note - 12*NoteScale].Index];
            }


            public float OscMult { get  
            {
                     if (Oscillator == OscSine     ) return 1;
                else if (Oscillator == OscTriangle ) return 0.8f;
                else if (Oscillator == OscSaw      ) return 0.4f;
                else if (Oscillator == OscSquare   ) return 0.35f;
                else if (Oscillator == OscLowNoise ) return 0.5f;
                else if (Oscillator == OscHighNoise) return 0.5f;
                else if (Oscillator == OscBandNoise) return 0.5f;
                else if (Oscillator == OscSlowSweepDown
                      || Oscillator == OscFastSweepDown
                      || Oscillator == OscSlowSweepUp  
                      || Oscillator == OscFastSweepUp  
                      || Oscillator == OscCrunch       
                      || Oscillator == OscSample)    return 1;

                return float_NaN;
            } }


            public float GetWaveform(float f)
            {
                     if (Oscillator == OscLowNoise ) f /= 24;
                else if (Oscillator == OscBandNoise) f /= 12;

                f *= 6;
                f -= (float)Math.Floor(f);

                while (f < 0) f += 1;

                float w = 0;

                     if (Oscillator == OscSine     )  w = (float)Math.Sin(f * Tau);
                else if (Oscillator == OscTriangle )  w =  1 - 2*Math.Abs(2*(f%1)-1);
                else if (Oscillator == OscSaw      )  w =  2*f - 1;
                else if (Oscillator == OscSquare   )  w =  f < 0.5f ? 1 : -1;
                else if (Oscillator == OscLowNoise )  w = -1 + g_random[(int)(f * 100)] * 2;
                else if (Oscillator == OscHighNoise)  w = -1 + g_random[(int)(f * 100)] * 2;
                else if (Oscillator == OscBandNoise)  w = -1 + g_random[(int)(f * 100)] * 2;

                w *= Volume.Value;

                return w;
            }


            public void CreateSounds(List<Sound> sounds, Note note, Program prog)
            {
                var  inst   = Instrument;
                var _sounds = new List<Sound>();

                var triggerValues = new List<TriggerValue>();


                var sndTime = note.Time + 1;

                var lTime = g_time - EditedClip.Track.StartTime - note.SongTime;
                var sTime = IsPlaying ? g_time - EditedClip.Track.StartTime : lTime;

                var tp = new TimeParams(sndTime, lTime, sTime, note, note.FrameLength, Index, triggerValues, prog);


                if (OK(Offset))
                    sndTime += (int)Math.Round(Offset.UpdateValue(tp) * FPS);


                var noteNum = AdjustNoteNumber(note, this, note.FrameLength, prog);

                if (   Oscillator == OscSample
                    && (   noteNum % NoteScale > 0
                        || noteNum >= (12 + OscSample.Samples.Count) * NoteScale))
                    return;

                var vol = note.Volume;
                

                // populate triggerValues
                tp.SourceIndex = -1;    inst.Volume .UpdateValue(tp);
                tp.SourceIndex = Index;      Volume?.UpdateValue(tp);


                string relPath = "";
                
                     if (OK(     Volume.Envelope)) relPath =      Volume.Envelope.Release.GetPath(Index);
                else if (OK(inst.Volume.Envelope)) relPath = inst.Volume.Envelope.Release.GetPath(-1);


                var _relLen = triggerValues.Find(v => v.Path == relPath);
                var  relLen = (int)((_relLen?.Value ?? 0) * FPS);

                var sample = GetSample(noteNum);


                if (OK(Harmonics))
                    Harmonics.CreateSounds(_sounds, this, note, noteNum, sndTime, note.FrameLength, relLen, triggerValues, prog);

                else
                {
                    if (   noteNum <  12*NoteScale
                        || noteNum > 150*NoteScale)
                        return;

                    _sounds.Add(new Sound(
                        sample,
                        note.Channel,
                        note.iChan,
                        sndTime,
                        note.FrameLength,
                        relLen,
                        vol * OscMult,
                        Instrument,
                        Index,
                        note,
                        triggerValues,
                        False,
                        Sound_null,
                        0));
                }


                // add sound and echos
                foreach (var snd in _sounds)
                { 
                    if (prog.TooComplex) break;

                    var del = Delay ?? inst.Delay;

                    if (OK(del))
                        prog.AddSoundAndEchos(
                            sounds, 
                            snd, 
                            del);
                    else
                        sounds.Add(snd);
                }
            }


            public void Randomize(List<Oscillator> used, Program prog)
            {
                if (prog.TooComplex) return;


                Oscillator = OscillatorFromType((OscType)(int)(Math.Pow(RND, 2) * (int)OscType.Sample));


                if (   RND > 0.7f
                    && !used.Contains(Oscillator))
                {
                    Offset = (Parameter)NewSettingFromTag(strOff, Setting_null, Instrument, this);
                    Offset.Randomize(prog);
                }
                else
                    Offset = Parameter_null;


                if (Index == 0)
                    Volume.SetValue(1, Note_null, Index);
                else
                    Volume.Randomize(prog);


                if (   Index > 0
                    && (   RND > 0.7f
                        || used.Contains(Oscillator)))
                {
                    Tune = new Tune(Instrument, this);
                    Tune.Randomize(prog);
                }
                else
                    Tune = Tune_null;


                if (   (   Oscillator == OscSine
                        || Oscillator == OscBandNoise)
                    && RND > 0.7f
                    && !used.Contains(Oscillator))
                {
                    Harmonics = new Harmonics(Instrument, this);
                    Harmonics.Randomize(prog);
                }
                else
                    Harmonics = Harmonics_null;


                if (   OK(Harmonics)
                    && RND > 0.7f)
                {
                    Filter = new Filter(Instrument, this);
                    Filter.Randomize(prog);
                }
                else
                    Filter = Filter_null;


                if (RND > 0.9f)
                {
                    Delay = new Delay(Instrument, this);
                    Delay.Randomize(prog);
                }
                else
                    Delay = Delay_null;


                used.Add(Oscillator);
            }


            public Setting GetOrAddSettingFromTag(string tag)
            {
                switch (tag)
                {
                    case strVol:  return Volume;
                    case strOff:  return Offset    ?? (Offset    = (Parameter)NewSettingFromTag(tag, Setting_null, Instrument, this));
                    case strTune: return Tune      ?? (Tune      = new Tune     (Instrument, this));
                    case strHrm:  return Harmonics ?? (Harmonics = new Harmonics(Instrument, this));
                    case strFlt:  return Filter    ?? (Filter    = new Filter   (Instrument, this));
                    case strDel:  return Delay     ?? (Delay     = new Delay    (Instrument, this));
                }

                return Setting_null;
            }


            public void Delete(Clip clip)
            {
                // this method removes note and channel automation associated with this source

                Offset   ?.Delete(clip, Index);
                Volume   ?.Delete(clip, Index);
                Tune     ?.Delete(clip, Index);
                Harmonics?.Delete(clip, Index);
                Filter   ?.Delete(clip, Index);
                Delay    ?.Delete(clip, Index);
            }


            public string Save()
            {
                return
                      WS((int)Oscillator.Type)
                    + WB(On)

                    + Volume.Save()
                    
                    + Program.SaveSetting(Offset)
                    + Program.SaveSetting(Tune)

                    + Program.SaveSetting(Harmonics)
                    + Program.SaveSetting(Filter)

                    + Program.SaveSetting(Delay);
            }


            public static void Load(string[] lines, ref int line, Instrument inst, int iSrc)
            {
                var data = lines[line++].Split(';');
                var i    = 0;

                var src = new Source(inst);
                inst.Sources.Add(src);

                src.Oscillator = OscillatorFromType((OscType)int_Parse(data[i++]));
                src.On         = data[i++] == "1";

                src.Volume = Parameter.Load(data, ref i, inst, iSrc, Setting_null);

                while (i < data.Length
                    && (   data[i] == strOff
                        || data[i] == strTune
                        || data[i] == strHrm
                        || data[i] == strFlt
                        || data[i] == strDel))
                { 
                    switch (data[i])
                    { 
                        case strOff:  src.Offset    = Parameter.Load(data, ref i, inst, iSrc, Setting_null); break;
                        case strTune: src.Tune      = Tune     .Load(data, ref i, inst, iSrc);               break;
                        case strHrm:  src.Harmonics = Harmonics.Load(data, ref i, inst, iSrc);               break;
                        case strFlt:  src.Filter    = Filter   .Load(data, ref i, inst, iSrc);               break;
                        case strDel:  src.Delay     = Delay    .Load(data, ref i, inst, iSrc);               break;
                    }
                }
            }


            public void DrawLabels(List<MySprite> sprites, float x, float y, DrawParams dp)
            {
                Offset   ?.DrawLabels(sprites, x, y, dp); 
                Volume    .DrawLabels(sprites, x, y, dp); 
                Tune     ?.DrawLabels(sprites, x, y, dp); 
                Harmonics?.DrawLabels(sprites, x, y, dp); 
                Filter   ?.DrawLabels(sprites, x, y, dp); 
                Delay    ?.DrawLabels(sprites, x, y, dp); 
            }


            public void DrawSource(List<MySprite> sprites, float x, ref float y, float w, Program prog)
            {
                var active = 
                       CurSrc == Index 
                    && CurSet <  0;


                var dp1 = new DrawParams(active, prog);
                DrawLabels(null, 0, 0, dp1); // don'g draw anything, just init the values


                var sh = dp1.OffY + 20;

                if (CurSrc == Index)
                    FillRect(sprites, x, y, w, sh, CurSet < 0 ? color6 : color3);


                var col_0 = On && CurSrc > -1 ? color6 : color4;
                var col_1 = On && CurSrc > -1 ? color0 : color5;

                if (Oscillator == OscSample)
                {
                    DrawString(sprites, Oscillator.ShortName, x + 10, y + sh/2 - 10, 0.7f, CurSrc == Index ? col_1 : col_0, TA_CENTER);
                }
                else
                { 
                    DrawSample(sprites,                       x +  10, y + sh/2 - 10, 50, 20, active, CurSrc > -1);
                    DrawString(sprites, Oscillator.ShortName, x + 100, y + sh/2 - 10, 0.6f, active ? col_1 : col_0, TA_CENTER);
                }


                var dp2 = new DrawParams(active, prog);
                DrawLabels(sprites, x + 139, y + sh/2 - dp1.OffY/2 + 2, dp2);

                FillRect(sprites, x, y + sh, w, 1, color3);

                y += sh;
            }


            void DrawSample(List<MySprite> sprites, float x, float y, float w, float h, bool active, bool bright)
            {
                var col_0 = On && bright ? color6 : color3;
                var col_1 = On && bright ? color0 : color5;

                var pPrev = new Vector2(float_NaN, float_NaN);


                var df = 1/24f;

                for (float f = 0; f <= 1; f += df)
                {
                    float wf;
                
                    /*if (Oscillator == OscClick)
                    { 
                             if (f == 0   ) wf =  0;
                        else if (f == df  ) wf =  1;
                        else if (f == df*2) wf = -1;
                        else                wf =  0;
                    }
                    else*/ if (Oscillator == OscCrunch)
                    { 
                        var _f = f % (1/4f);

                             if (fequal(_f, 0   )) wf =  0;
                        else if (fequal(_f, df  )) wf =  1;
                        else if (fequal(_f, df*2)) wf = -1;
                        else                       wf =  0;
                    }
                    else 
                    { 
                        wf = GetWaveform(f*2.1f / Tau);
                    }

                    var p = new Vector2(
                        x + w * f,
                        y + h/2 - wf * h/2);

                    if (   OK(pPrev.X)
                        && OK(pPrev.Y))
                        DrawLine(sprites, pPrev, p, active ? col_1 : col_0, active ? 2 : 1);

                    pPrev = p;
                }
            }


            public void DrawFuncButtons(List<MySprite> sprites, float w, float y, Channel chan)
            {
                if (CurSet > -1)
                { 
                    CurSetting.DrawFuncButtons(sprites, w, y, chan);
                }
                else
                {
                    DrawFuncButton(sprites, strOff,  0, w, y, True, OK(Offset   ));
                    DrawFuncButton(sprites, strVol,  1, w, y, True,    Volume.HasDeepParams(chan, Index));
                    DrawFuncButton(sprites, strTune, 2, w, y, True, OK(Tune     ));
                    DrawFuncButton(sprites, strHrm,  3, w, y, True, OK(Harmonics));
                    DrawFuncButton(sprites, strFlt,  4, w, y, True, OK(Filter   ));
                    DrawFuncButton(sprites, strDel,  5, w, y, True, OK(Delay    ));
                }
            }


            public void ResetValues()
            {
                Offset   ?.Reset();
                Volume   ?.Reset();
                Tune     ?.Reset();
                Harmonics?.Reset();
                Filter   ?.Reset();
                Delay    ?.Reset();
            }
        }
    }
}
