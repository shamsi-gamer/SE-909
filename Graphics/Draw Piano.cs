﻿using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.GUI.TextPanel;
using VRageMath;


namespace IngameScript
{
    partial class Program
    {
        void DrawPianoDisplay(List<MySprite> sprites, float x, float y, float w, float h, Clip clip, int pat, bool isolated, Arpeggio arp)
        {
            FillRect(sprites, x, y, w, h, color0);

            var  rh = h - 90;
            var irh = h - 50;


            if (!OK(arp))
                DrawChannelList(sprites, x, y, 340, rh);
            else
            {
                var dp = new DrawParams(this);
                SelInstrument.DrawLabels(sprites, x + 5, y + 10, dp);

                var lenCol = CurSetting == arp.Length ? color6 : color3;
                DrawString(sprites, "Length",                                 x + 30, y + 160, 1f,   lenCol);
                DrawString(sprites, PrintValue(arp.Length.Value, 0, T, 0), x + 40, y + 200, 0.8f, lenCol);

                var sclCol = CurSetting == arp.Scale ? color6 : color3;
                DrawString(sprites, "Scale",                                  x + 30, y + 260, 1f,   sclCol);
                DrawString(sprites, PrintValue(arp.Scale.Value, -3, T, 0), x + 40, y + 300, 0.8f, sclCol);
            }


            var _dummy = new List<TriggerValue>();

            var tp = new TimeParams(g_time, 0, g_time - EditedClip.Track.StartTime, null, EditedClip.EditLength, CurSrc, _dummy, this);

            var songSteps =
                OK(arp)
                ? (int)Math.Round(arp.Length.UpdateValue(tp))
                : g_patSteps;


            var xt = 340;
            var wt = (float)(w - xt) / g_patSteps;

            DrawPianoGrid(sprites, x + xt, y, w - xt, rh, songSteps);


            // draw edit position
            if (   OK(clip.EditPos)
                && clip.EditPos >= CurPat      * g_patSteps
                && clip.EditPos < (CurPat + 1) * g_patSteps)
            { 
                FillRect(
                    sprites, 
                    x + xt + wt * (clip.EditPos % g_patSteps), 
                    y, 
                    wt / (EditedClip.EditStepLength == 0.5f ? 2 : 1), 
                    EditedClip.ParamKeys || EditedClip.ParamAuto ? h : rh, 
                    color3);
            }


            DrawPianoRoll(sprites, x + xt, y, w - xt, rh, clip, pat, 2, OK(arp), songSteps);


            // draw position line/s
            if (   IsPlaying
                && PlayPat == pat)
            {
                if (IsCurOrParentSetting(typeof(Arpeggio)))
                { 
                    var arpNotes = new List<Note>();

                    for (int p = 0; p <= CurPat; p++)
                    { 
                        var notes = EditedClip.Patterns[p].Channels[SelChan].Notes.FindAll(n => 
                                  OK(n.Instrument.Arpeggio)
                               && EditedClip.Track.PlayTime >= n.Step*TicksPerStep + n.Time                
                               && EditedClip.Track.PlayTime <  n.Step*TicksPerStep + n.Time + n.FrameLength);

                        foreach (var n in notes)
                            arpNotes.Add(n);
                    }

                    foreach (var an in arpNotes)
                    {
                        var ft = (int)((an.ArpPlayTime / TicksPerStep) % g_patSteps);

                        FillRect(sprites, x + xt + wt * ft, y, wt, rh, color6);
                        DrawPianoNeg(sprites, x + xt, y, w - xt, rh, clip, pat, ft, T);
                    }
                }
                else
                { 
                    FillRect(sprites, x + xt + wt * ((int)PlayStep % g_patSteps), y, wt, rh, color6);
                    DrawPianoNeg(sprites, x + xt, y, w - xt, rh, EditedClip, pat, (int)PlayStep, T);
                }
            }


            FillRect(sprites, x, y + (OK(arp) ? irh : rh), w, 1, color6);

            if (IsCurParam())
                DrawKeysAndAuto(sprites, CurParam, x, y, w, h, xt, rh, clip, pat);

            if (SelChan < 0 || OK(arp))
                DrawFuncButtons(sprites, w, h, clip);
        }


        void DrawPianoGrid(List<MySprite> sprites, float x, float y, float w, float h, int songSteps)
        {
            var wt = w/g_patSteps;
            var ht = h/25;

            for (int t = 0; t < songSteps; t += 4)
                FillRect(sprites, x + t * wt, y, wt, h, color1);

            int[] black = { 1, 3, 6, 8, 10, 13, 15, 18, 20, 22 };

            for (int n = 0; n < 25; n++)
            {
                if (!black.Contains(n))
                    FillRect(sprites, x, y + h - (n + 1) * ht + 1, wt * songSteps, ht - 2, color2);
            }

            for (int t = 0; t < songSteps; t++)
                FillRect(sprites, x + t * wt, y, 1, h, color3);
        }


        void DrawPianoRoll(List<MySprite> sprites, float x, float y, float w, float h, Clip clip, int p, int gs, bool arpeggio, int songSteps)
        {
            if (!arpeggio)
            { 
                for (int ch = 0; ch < g_nChans; ch++)
                {
                    if (ch != CurChan)
                        DrawChanNotes(sprites, x, y, w, h, clip, p, gs, ch, color3);
                }
            }

            DrawChanNotes(sprites, x, y, w, h, clip, p, gs, CurChan, color6, songSteps);
        }


        void DrawChanNotes(List<MySprite> sprites, float x, float y, float w, float h, Clip clip, int p, int gs, int ch, Color col, int songSteps = g_patSteps)
        {
            var wt = w/g_patSteps;
            var ht = h/ 25;
                     
            var bw = w/ 76;
            var bh = h/200;
            var th = ht - bh*2;

            for (int _p = 0; _p <= p; _p++)
            {
                var patStart = p * g_patSteps;
                var patEnd   = patStart + songSteps;

                var pat  = clip.Patterns[_p];
                var chan = pat.Channels[ch];

                var min  = (60 + chan.Transpose*12) * NoteScale;
                var max  = (84 + chan.Transpose*12) * NoteScale;

                foreach (var n in chan.Notes)
                {
                    if (   n.Number < min
                        || n.Number > max)
                        continue;

                    var noteStart = n.SongStep + n.ShOffset;
                    var noteEnd   = noteStart + n.StepLength;

                    if (   noteEnd   <= patStart
                        || noteStart >= patEnd)
                        continue;

                    noteStart = Math.Max(noteStart, patStart);
                    noteEnd   = Math.Min(noteEnd,   patEnd);

                    var yLine = y + h - ((float)n.Number/NoteScale - chan.Transpose * 12 - 60 + 1) * ht;

                    var pt = new Vector2(
                        x + wt * (noteStart-patStart),
                        yLine + ht/2);

                    var tw = (float)Math.Floor(wt * (noteEnd-noteStart)) - gs*2;

                    FillRect(sprites, 
                        pt.X + gs,
                        pt.Y - th/2,
                        tw,
                        th,
                        col);
                }
            }
        }


        void DrawPianoNeg(List<MySprite> sprites, float x, float y, float w, float h, Clip clip, int pat, float step, bool isolated)
        {
            for (int ch = 0; ch < g_nChans; ch++)
            {
                var wt = w/g_patSteps;
                var ht = h/25;

                var _step = step % g_patSteps;

                var bh = h/200;
                var th = ht - bh*2;

                var col =
                    ch == CurChan
                    ? color2
                    : color5;


                for (int _p = 0; _p <= pat; _p++)
                {
                    var patStart =  pat   *g_patSteps;
                    var patEnd   = (pat+1)*g_patSteps;

                    var chan = clip.Patterns[_p].Channels[ch];


                    var min = (60 + chan.Transpose*12) * NoteScale;
                    var max = (84 + chan.Transpose*12) * NoteScale;


                    foreach (var n in chan.Notes)
                    {
                        if (   n.Number < min
                            || n.Number > max)
                            continue;

                        var noteStart = n.SongStep + n.ShOffset;
                        var noteEnd   = noteStart + n.StepLength;

                        if (   noteEnd   <= patStart
                            || noteStart >= patEnd)
                            continue;

                        var yLine = y + h - ((float)n.Number/NoteScale - chan.Transpose * 12 - 60 + 1) * ht;

                        var pt = new Vector2(
                            x + wt * (isolated ? _step : step),
                            yLine + ht/2);

                        if (   step >= noteStart
                            && step <  noteEnd)
                        {
                            var tw = (float)Math.Floor(wt * (noteEnd - noteStart));

                            FillRect(sprites, 
                                pt.X + 1,
                                pt.Y - th/2,
                                Math.Min(tw, wt) - 2,
                                th,
                                col);
                        }
                    }
                }
            }
        }
    }
}
