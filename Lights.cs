﻿using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using VRageMath;


namespace IngameScript
{
    partial class Program
    {
        static IMyTextPanel
            lblOctave, lblShuffle,
            lblMixerVolumeUp, lblMixerVolumeDown, lblMixerAll, lblMixerMuteAll,
            lblPlay, lblStop,
            lblStep, lblHold, 
            lblEdit, 
            lblChord, lblChord1, lblChord2, lblChord3, lblChord4, lblChordEdit,
            lblEditStep, lblEditLength,
            lblLeft, lblRight,
            lblLoop, lblBlock, 
            lblTransposeUp, lblTransposeDown,
            lblAllPatterns, lblMovePat, lblFollow,
            lblMixerShift, lblClips, lblMemSet, lblAutoCue, lblMemory,
            lblPrev, lblNext, lblMove, lblEnter, lblBack, lblBackOut,
            lblNew, lblDuplicate, lblDelete,
            lblCmd1, lblCmd2, lblCmd3,
            lblSpread, lblRandom,
            lblUp, lblDown, lblShift, 
            lblPrevPat, lblNextPat,
            lblLock, lblAutoLock,
            lblFold, lblGyro, lblNoise;


        List<IMyTextPanel>        lblHigh,
                                  lblLow;

        static List<IMyTextPanel> lblMem;
        static IMyTextPanel[]     lblMems = new IMyTextPanel[nMems];


        IMyInteriorLight warningLight;


        static List<int>          g_mixerPressed  = new List<int>();
        static List<int>           _mixerPressed  = new List<int>();
                                                  
        static List<int>          g_infoPressed   = new List<int>();
        static List<int>           _infoPressed   = new List<int>();
                                                  
        static List<int>          g_clipPressed   = new List<int>();
        static List<int>          g_mainPressed   = new List<int>();

        static List<IMyTextPanel> g_lightsPressed = new List<IMyTextPanel>();
        static List<IMyTextPanel>  _lightsPressed = new List<IMyTextPanel>();

        IMyReflectorLight frontLight;


        void SetLightColor(int iCol)
        {
            g_clip.ColorIndex = MinMax(0, iCol, 6);

            switch (g_clip.ColorIndex)
            {
                case 0: SetLightColor(new Color(255,   0,   0), 0.35f); break;
                case 1: SetLightColor(new Color(255,  92,   0), 0.35f); break;
                case 2: SetLightColor(new Color(255, 255,   0), 0.4f);  break;
                case 3: SetLightColor(new Color(0,   255,   0), 0.35f); break;
                case 4: SetLightColor(new Color(0,    40, 255));        break;
                case 5: SetLightColor(new Color(128,   0, 255), 0.4f);  break;
                case 6: SetLightColor(new Color(255, 255, 255), 0.35f); break;
            }
        }


        Color MakeColor(Color c, float f)
        {
            return new Color(Color.Multiply(c, f), 1);
        }


        void InitLights()
        {
            lblOctave          = Lbl("Octave");
            lblShuffle         = Lbl("Shuffle");
            lblMixerVolumeUp   = Lbl("M Up R");
            lblMixerVolumeDown = Lbl("M Down R");
            lblMixerAll        = Lbl("M Solo R");
            lblMixerMuteAll    = Lbl("M Mute R");

            lblEdit            = Lbl("Edit");

            lblPlay            = Lbl("Play");
            lblStop            = Lbl("Stop");

            lblPrevPat         = Lbl("Prev Pattern");
            lblNextPat         = Lbl("Next Pattern");

            lblLeft            = Lbl("Left");
            lblRight           = Lbl("Right");
            lblEditStep        = Lbl("Edit Step");
            lblEditLength      = Lbl("Edit Length");

            lblLoop            = Lbl("Loop");
            lblBlock           = Lbl("Block");
            lblAllPatterns     = Lbl("All Patterns");
            lblMovePat         = Lbl("Move Pattern");
            lblAutoCue         = Lbl("Auto Cue");
            lblFollow          = Lbl("Follow");

            lblMixerShift      = Lbl("M Shift");
            lblClips           = Lbl("Clips");

            lblMemSet          = Lbl("MemSet");
            lblMemory          = Lbl("Mem");
            lblStep            = Lbl("Step");
            lblHold            = Lbl("Hold");
            lblTransposeUp     = Lbl("Transpose Up");
            lblTransposeDown   = Lbl("Transpose Down");

            lblChord           = Lbl("Chord");
            lblChord1          = Lbl("Chord 1");
            lblChord2          = Lbl("Chord 2");
            lblChord3          = Lbl("Chord 3");
            lblChord4          = Lbl("Chord 4");
            lblChordEdit       = Lbl("Chord Edit");

            lblPrev            = Lbl("Prev");
            lblNext            = Lbl("Next");
            lblEnter           = Lbl("Enter");
            lblBack            = Lbl("Back");
            lblBackOut         = Lbl("Back Out");
            lblMove            = Lbl("Move");
            lblNew             = Lbl("New");
            lblDuplicate       = Lbl("Dup");
            lblDelete          = Lbl(strDel);

            lblCmd1            = Lbl("Command 1");
            lblCmd2            = Lbl("Command 2");
            lblUp              = Lbl("Up");
            lblDown            = Lbl("Down");
            lblShift           = Lbl("Shift");
            lblCmd3            = Lbl("Command 3");

            lblSpread          = Lbl("Spread");
            lblRandom          = Lbl("Random");

            lblLock            = Lbl("Lock");
            lblAutoLock        = Lbl("Auto Lock");
            lblFold            = Lbl("Fold");
            lblGyro            = Lbl("Gyro");
            lblNoise           = Lbl("Noise");


            for (int m = 0; m < nMems; m++)
                lblMems[m] = Lbl("Mem " + S(m));


            lblHigh = new List<IMyTextPanel>();
            lblLow  = new List<IMyTextPanel>();

            Get(lblHigh, l => l.CustomName.Length >= 11 && l.CustomName.Substring(0, 11) == "Label High ");
            lblHigh = lblHigh.OrderBy(l => int.Parse(l.CustomName.Substring(11))).ToList();

            Get(lblLow, l => l.CustomName.Length >= 10 && l.CustomName.Substring(0, 10) == "Label Low ");
            lblLow = lblLow.OrderBy(l => int.Parse(l.CustomName.Substring(10))).ToList();


            lblMem = new List<IMyTextPanel>();
            Get(lblMem, l => l.CustomName.Length >= 10 && l.CustomName.Substring(0, 10) == "Label Mem ");
            lblMem = lblMem.OrderBy(l => int.Parse(l.CustomName.Substring(10))).ToList();


            frontLight = Get("Front Light") as IMyReflectorLight;

            warningLight = Get("Saturation Warning Light") as IMyInteriorLight;
        }


        void SetLightColor(Color c, float f = 1)
        {
            color6 = MakeColor(c, 0.878f * f);
            color5 = MakeColor(c, 0.722f * f);
            color4 = MakeColor(c, 0.353f * f);
            color3 = MakeColor(c, 0.157f * f);
            color2 = MakeColor(c, 0.031f * f);
            color1 = MakeColor(c, 0.020f * f);
            color0 = MakeColor(c, 0.004f * f);


            var labels = new List<IMyTextPanel>();
            Get(labels, l => l.CustomName != "Label Edit");

            foreach (var l in labels)
            {
                l.FontColor       = color6;
                l.BackgroundColor = color0;
            }


            var max = Math.Max(Math.Max(color6.R, color6.G), color6.B);

            var lightColor = new Color(
                color6.R / max * 0xFF,
                color6.G / max * 0xFF,
                color6.B / max * 0xFF);


                 if (g_clip.ColorIndex == 1) lightColor = new Color(0xFF, 0x50, 0);
            else if (g_clip.ColorIndex == 5) lightColor = new Color(0xAA, 0, 0xFF);


            var lights = new List<IMyInteriorLight>();

            var group = GridTerminalSystem.GetBlockGroupWithName("Rear Lights");
            if (group != null) group.GetBlocksOfType(lights);

            foreach (var l in lights)
                l.Color = lightColor;


            frontLight.Color = new Color(
                lightColor.R + (int)((0xFF - lightColor.R) * 0.23f),
                lightColor.G + (int)((0xFF - lightColor.G) * 0.23f),
                lightColor.B + (int)((0xFF - lightColor.B) * 0.23f));


            switch (g_clip.ColorIndex)
            {
            case 0: warningLight.Color = new Color(0,    0,    0xFF); break;
            case 1: warningLight.Color = new Color(0,    0,    0xFF); break;
            case 2: warningLight.Color = new Color(0xFF, 0,    0x80); break;
            case 3: warningLight.Color = new Color(0xFF, 0,    0xFF); break;
            case 4: warningLight.Color = new Color(0xFF, 0x40, 0   ); break;
            case 5: warningLight.Color = new Color(0xFF, 0x30, 0   ); break;
            case 6: warningLight.Color = new Color(0xFF, 0,    0   ); break;
            }


            UpdateLights();
        }



        static void MarkLight(IMyTextPanel light, bool on = true)
        {
            g_lightsPressed.Add(light);
            UpdateLight(light, on);
        }


        void UnmarkLight(IMyTextPanel light, bool on = false, bool half = false)
        {
            UpdateLight(light, on, half);
            _lightsPressed.Remove(light);
        }


        void UnmarkAllLights()
        {
            var be  = g_clip.EditNotes.Count > 0;
            var cur = g_clip.CurSrc > -1;
            var crd = g_clip.ChordEdit;
            var ch  = g_clip.SelChan > -1;
            var mov = g_clip.MovePat;
            var sh  = g_clip.Shift;
            var set = g_clip.CurSet < 0;


            if (_lightsPressed.Contains(lblLeft))      UnmarkLight(lblLeft,  false, be);
            if (_lightsPressed.Contains(lblRight))     UnmarkLight(lblRight, false, be);

            if (_lightsPressed.Contains(lblUp))        UnmarkLight(lblUp,   sh);
            if (_lightsPressed.Contains(lblDown))      UnmarkLight(lblDown, sh);

            if (_lightsPressed.Contains(lblNextPat))   UnmarkLight(lblNextPat, mov);
            if (_lightsPressed.Contains(lblPrevPat))   UnmarkLight(lblPrevPat, mov);

            if (_lightsPressed.Contains(lblNext))      UnmarkLight(lblNext, g_move || cur, ch);
            if (_lightsPressed.Contains(lblPrev))      UnmarkLight(lblPrev, g_move || cur, ch);

            if (_lightsPressed.Contains(lblBackOut))   UnmarkLight(lblBack,  cur, ch);
            if (_lightsPressed.Contains(lblBack))      UnmarkLight(lblBack,  cur, ch);
            if (_lightsPressed.Contains(lblEnter))     UnmarkLight(lblEnter, cur && set, ch && set);

            if (_lightsPressed.Contains(lblNew))       UnmarkLight(lblNew,       cur, ch);
            if (_lightsPressed.Contains(lblDuplicate)) UnmarkLight(lblDuplicate, cur, ch);
            if (_lightsPressed.Contains(lblDelete))    UnmarkLight(lblDelete,    cur, ch);

            if (_lightsPressed.Contains(lblChord1))    UnmarkLight(lblChord1, crd && g_clip.Chord == 0, g_clip.Chords[0].Count > 0);
            if (_lightsPressed.Contains(lblChord2))    UnmarkLight(lblChord2, crd && g_clip.Chord == 1, g_clip.Chords[1].Count > 0);
            if (_lightsPressed.Contains(lblChord3))    UnmarkLight(lblChord3, crd && g_clip.Chord == 2, g_clip.Chords[2].Count > 0);
            if (_lightsPressed.Contains(lblChord4))    UnmarkLight(lblChord4, crd && g_clip.Chord == 3, g_clip.Chords[3].Count > 0);

            if (_lightsPressed.Contains(lblCmd2))      UnmarkLight(lblCmd2, false, copyChan != null);

            foreach (var lbl in _lightsPressed)
                UpdateLight(lbl, false);


            _mixerPressed.Clear();
            _infoPressed  .Clear();
            _lightsPressed.Clear();


            // mark for next cycle and clear pressed list

            _mixerPressed.AddRange(g_mixerPressed);
            g_mixerPressed.Clear();

            _infoPressed.AddRange(g_infoPressed);
            g_infoPressed.Clear();

            g_clipPressed.Clear();
            g_mainPressed.Clear();

            _lightsPressed.AddRange(g_lightsPressed);
            g_lightsPressed.Clear();
        }


        void UpdateLights()
        {
            if (TooComplex) return;

            UpdateLight(lblFollow,      g_clip?.Follow  ?? false);
            UpdateLight(lblLoop,        g_clip?.Loop    ?? false);
            UpdateLight(lblBlock,       g_clip?.Block   ?? false);
            UpdateLight(lblAllPatterns, g_clip?.AllPats ?? false);
            UpdateLight(lblMovePat,     g_clip?.MovePat ?? false);
            UpdateLight(lblAutoCue,     g_clip?.AutoCue ?? false);

            UpdateEditLight(lblEdit, OK(g_clip.EditPos));
            UpdateHoldLight();

            UpdateChordLights();

            UpdateShuffleLight();
            UpdateOctaveLight();

            UpdatePlayStopLights();
            UpdateMemoryLights();
            UpdateEditLights();
            UpdateNewLights();

            UpdateAdjustLights(g_clip);

            UpdateLockLights();
            UpdateGyroLight();
            UpdateTimerLight();

            UpdateClipsLight();
        }


        static void UpdateChordLights()
        {
            //if (TooComplex) return;

            if (    IsCurParam(strTune)
                && !(g_clip.ParamKeys || g_clip.ParamAuto))
            {
                var inst = g_clip.SelectedInstrument;
                var tune = (Tune)GetCurrentParam(inst);

                UpdateLight(lblChord, tune.UseChord);

                UpdateLight(lblChordEdit, tune.UseChord ? strAll : " ", 10, 10);
                UpdateLight(lblChordEdit, tune.AllOctaves);
                // TODO same for source or anything else that needs Tune
            }
            else
            {
                UpdateLight(lblChord, g_clip.ChordEdit ? " " : "Chord", 9, 12);
                UpdateLight(lblChord, g_clip.ChordMode);

                if (g_clip.ChordMode)
                {
                    UpdateLight(lblChordEdit, strAll, 10, 10);
                    UpdateLight(lblChordEdit, g_clip.ChordAll);
                }
                else
                {
                    UpdateLight(lblChordEdit, "Edit", 10, 10);
                    UpdateLight(lblChordEdit, g_clip.ChordEdit);
                }
            }

            UpdateChordLight(lblChord1, 1);
            UpdateChordLight(lblChord2, 2);
            UpdateChordLight(lblChord3, 3);
            UpdateChordLight(lblChord4, 4);
        }


        static void UpdateChordLight(IMyTextPanel lbl, int chord)
        {
            //if (TooComplex) return;

            var c = g_clip.Chords[chord-1];

            string chordName = GetChordName(c, S(chord));

            lbl.WriteText(chordName);

            UpdateLight(
                lbl,
                      g_clip.Chord == chord-1
                   && (   g_clip.ChordEdit
                       || g_clip.ChordMode)
                   && !IsCurParam(strTune)
                || g_lightsPressed.Contains(lbl),
                      g_clip.ChordMode
                   && g_clip.Chord == chord-1 
                || c.Count > 0);
        }


        void MarkChordLight(int chord)
        {
                 if (chord == 1 && g_clip.Chords[0].Count > 0) MarkLight(lblChord1);
            else if (chord == 2 && g_clip.Chords[1].Count > 0) MarkLight(lblChord2);
            else if (chord == 3 && g_clip.Chords[2].Count > 0) MarkLight(lblChord3);
            else if (chord == 4 && g_clip.Chords[3].Count > 0) MarkLight(lblChord4);
        }


        void UpdateShuffleLight()
        {
            if (g_clip.Spread)
            {
                UpdateLight(lblShuffle, "Sprd", 10, 10);
            }
            else if (ShowPiano)
            {
                UpdateLight(
                    lblShuffle,
                    " ▄█   █ █ ██ █ █ █   █▄ \n" +
                   " ▀██   █▄█▄██▄█▄█▄█   ██▀ \n" +
                     " ▀   ▀▀▀▀▀▀▀▀▀▀▀▀   ▀ ",
                    2,
                    32);
            }
            else
            {
                UpdateLight(lblShuffle, "Shuf", 10, 10);
            }
        }


        void UpdateOctaveLight()
        {
            if (TooComplex) return;
            
            int val = 0;

                //if (g_chordMode) 
                    if (g_clip.Spread) val = g_clip.ChordSpread;
            else if (ShowPiano)     val = g_clip.CurrentChannel.Transpose;
            else                    val = g_clip.CurrentChannel.Shuffle;

            lblOctave.WriteText((val > 0 ? "+" : "") + S(val), false);
        }


        void UpdatePlayStopLights()
        {
            UpdateLight(lblPlay, OK(g_clip.PlayTime));
            UpdateLight(lblStop, OK(g_clip.PlayTime));
        }


        static void UpdateMemoryLights()
        {
            UpdateLight(lblMemory, g_clip.MemSet);

            for (int m = 0; m < nMems; m++)
            {
                lblMems[m].WriteText(
                      S((char)(65 + m)) + " "
                    + (g_clip.Mems[m] > -1 ? S(g_clip.Mems[m] + 1).PadLeft(3) : " "));
            }
        }


        void UpdateEditLights() 
        {
            var strStep = 
                EditStep == 0.5f
                ? "½"
                : S0(EditStep);

            string strLength;

                 if (EditStepLength == 0.25f )    strLength = "¼";
            else if (EditStepLength == 0.5f  )    strLength = "½";
            else if (EditStepLength == float_Inf) strLength = "∞";
            else                                  strLength = S0(EditStepLength);

            lblEditStep  .WriteText("·· " + strStep);
            lblEditLength.WriteText("─ "  + strLength);
        }


        void UpdateNewLights()
        {
            UpdateLabelColor(lblBackOut);
            UpdateLabelColor(lblBack);
            UpdateEnterLight();

            UpdateLabelColor(lblPrev);
            UpdateLabelColor(lblNext);
            UpdateLabelColor(lblNew);
            UpdateLabelColor(lblDuplicate);
            UpdateLabelColor(lblDelete);

            UpdateLight(lblMove, g_move ^ (g_clip.CurSrc > -1), g_clip.SelChan > -1 && !g_move);
        }


        void UpdateLabelColor(IMyTextPanel lbl) 
        {
            UpdateLight(lbl, g_clip.CurSrc > -1, g_clip.SelChan > -1); 
        }


        void UpdateEnterLight()
        {
            UpdateLight(lblEnter, g_clip.CurSet < 0 && g_clip.CurSrc < 0 ? "└►" : " ", 10, 10);
            UpdateLight(lblEnter, g_clip.CurSet < 0 && g_clip.CurSrc > -1, g_clip.SelChan > -1 && g_clip.CurSet < 0);
        }


        void UpdateAdjustLights(Clip song)
        {
            if (ModDestConnecting != null)
            {
                UpdateLight(lblCmd1, "Conn", 10, 10);
                UpdateLight(lblCmd1, true);
                return;
            }


            if (g_clip.CurSet > -1)
            {
                var path = g_settings.Last().GetPath(g_clip.CurSrc);

                if (g_clip.ParamKeys)
                {
                    UpdateLight(lblCmd1, "Inter", 10, 10);

                    UpdateLight(
                        lblCmd3,
                        g_clip.SelectedChannel.Notes.Find(n =>
                               n.SongStep >= song.EditPos
                            && n.SongStep <  song.EditPos+1
                            && n.Keys.Find(k => k.Path == path) != null) != null
                        ? "X"
                        : " ",
                        10, 
                        10);
                }
                else if (g_clip.ParamAuto)
                {
                    if (OK(song.EditPos))
                    { 
                        if (g_clip.SelectedChannel.AutoKeys.Find(k =>
                                k.Path == path
                                && k.StepTime >= (song.EditPos % g_nSteps)
                                && k.StepTime <  (song.EditPos % g_nSteps) + 1) != null)
                        {
                            UpdateLight(lblCmd1, "Move", 10, 10);
                            UpdateLight(lblCmd3, "X",    10, 10);
                        }
                        else
                        {
                            UpdateLight(lblCmd1, " ", 10, 10);
                            UpdateLight(lblCmd3, "+", 10, 10);
                        }
                    }
                    else
                        UpdateLight(lblCmd3, " ", 10, 10);
                }
                else
                {
                    UpdateLight(lblCmd1, HasTag(CurSetting, strMod) ? "Conn" : " ", 10, 10);
                    UpdateLight(lblCmd1, false);

                    UpdateLight(lblCmd3, CurSetting.CanDelete() ? "X" : " ", 10, 10);
                    UpdateLight(lblCmd3, false);
                }

                UpdateLight(lblCmd2, " ", 10, 10);
            }
            else
            {
                if (g_clip.CurSrc > -1)
                {
                    UpdateLight(lblCmd1, "On",    10, 10);
                    UpdateLight(lblCmd1, g_clip.SelectedSource.On);
                    UpdateLight(lblCmd2, "Osc ↕", 10, 10);
                    UpdateLight(lblCmd3, " ",     10, 10);
                    UpdateLight(lblCmd3, false);
                }
                else
                { 
                    UpdateLight(lblCmd1, g_clip.SelChan < 0 ? "Copy" : " ", 10, 10);
                    UpdateLight(lblCmd1, false);

                    UpdateLight(lblCmd2, g_clip.SelChan < 0 ? "Paste" : " ", 10, 10);
                    UpdatePasteLight();

                    UpdateLight(
                        lblCmd3,     
                        g_clip.SelChan < 0 
                        ? " ▄█   █ █ ██ █ █ █   █▄ \n" +
                         " ▀██   █▄█▄██▄█▄█▄█   ██▀ \n" +  
                           " ▀   ▀▀▀▀▀▀▀▀▀▀▀▀   ▀ " 
                        : " ", 
                        2, 
                        32);

                    UpdateLight(
                        lblCmd3, 
                           g_clip.SelChan < 0 
                        && g_clip.Transpose, 
                        g_clip.EditNotes.Count > 0);
                }
            }


            bool canAdjust = 
                   IsCurParam()
                || IsCurSetting(typeof(Harmonics))
                ||    g_clip.Transpose 
                   && g_clip.SelChan < 0;


            var _strUp   = strRight;
            var _strDown = strLeft;

            if (      canAdjust
                   && (   IsCurParam(strVol)
                       || IsCurParam(strTune)
                       || IsCurParam(strSus)
                       || IsCurParam(strAmp)
                       || IsCurParam(strLvl)
                       || IsCurParam(strPow)
                       ||     IsCurParam(strCnt)
                          && (g_clip.ParamKeys || g_clip.ParamAuto)
                       || IsCurSetting(typeof(Harmonics)))
                || g_clip.Transpose)
            {
                _strUp   = strUp;
                _strDown = strDown;
            }

            UpdateLight(lblShift, canAdjust ?  strShft : " ", 10, 10);
            UpdateLight(lblDown,  canAdjust ? _strDown : " ", 10, 10);
            UpdateLight(lblUp,    canAdjust ? _strUp   : " ", 10, 10);
            
            UpdateLight(lblShift, canAdjust && g_clip.Shift);
            UpdateLight(lblDown,  canAdjust && g_clip.Shift);
            UpdateLight(lblUp,    canAdjust && g_clip.Shift);

            UpdateLight(lblLeft,  g_lightsPressed.Contains(lblLeft),  g_clip.EditNotes.Count > 0);
            UpdateLight(lblRight, g_lightsPressed.Contains(lblRight), g_clip.EditNotes.Count > 0);
        }


        void UpdatePasteLight()
        {
            UpdateLight(
                lblCmd2,
                _lightsPressed.Contains(lblCmd2),
                copyChan != null);//g_clip.Shift && bc);
        }


        void UpdateLockLights()
        {
            UpdateLight(lblLock,     g_locks.Find(l => l.IsLocked) != null);
            UpdateLight(lblAutoLock, g_locks.Find(l => l.AutoLock) != null);
        }


        void UpdateTimerLight()
        {
            UpdateLight(lblNoise, g_timers.Find(t => !t.Enabled) == null);
        }


        void UpdateClipsLight()
        {
            UpdateLight(lblClips, g_session ? "Set" : "Clips", 8, 18);
            UpdateLight(lblClips, false);
        }


        void UpdateGyroLight()
        {
            UpdateLight(lblGyro, g_gyros.Find(g => !g.Enabled) == null);
        }


        void UpdateKeyLights()
        {
            if (TooComplex) 
                return;
                
            if (ShowPiano)
            {
                UpdateLight(
                    lblHigh[10],
                      "     ║  ███  ║       ║  ███\n"
                    + "     ║       ║       ║     \n"
                    + "═════╬═══════╬═══════╬═════\n"
                    + "     ║       ║       ║     \n"
                    + " ███ ║  ███  ║  ███  ║  ███\n",
                    1.7f,
                    17);

                for (int h = 0; h < lblHigh.Count-1; h++)
                    UpdateLight(lblHigh[h], HighNoteName(h, g_clip.HalfSharp), 10, 10);

                for (int l = 0; l < lblLow.Count-1; l++)
                    UpdateLight(lblLow[l], LowNoteName(l, g_clip.HalfSharp), 10, 10);

                UpdatePianoLights();
            }
            else
            {
                UpdateLight(
                    lblHigh[10],
                      "█ █ ██ █ █ █\n"
                    + "█▄█▄██▄█▄█▄█\n"
                    + "▀▀▀▀▀▀▀▀▀▀▀▀\n",
                    3.7f,
                    10);

                for (int h = 0; h < lblHigh.Count-1; h++)
                    UpdateLight(lblHigh[h], " ", 10, 10);

                lblHigh[0].WriteText("◄∙∙");
                lblHigh[1].WriteText("∙∙►");

                lblHigh[2].WriteText("Pick");
                UpdateLight(lblHigh[2], g_clip.Pick);
                UpdateLight(lblHigh[3], "All Ch", 7.6f, 19.5f);
                UpdateLight(lblHigh[3], g_clip.AllChan);
                lblHigh[4].WriteText("Inst");
                UpdateLight(lblHigh[4], g_clip.RndInst);

                lblHigh[5].WriteText("Rnd");
                lblHigh[6].WriteText("Clr");

                lblHigh[7].WriteText("1/4");
                lblHigh[8].WriteText("1/8");
                lblHigh[9].WriteText("Flip");

                for (int l = 0; l < lblLow.Count; l++)
                    UpdateLight(lblLow[l], " ", 10, 10);

                UpdateStepLights();
            }
        }


        void UpdatePianoLights()
        {
            UpdateHighLights(g_clip.CurrentPattern, g_clip.CurrentChannel);
            UpdateLowLights (g_clip.CurrentPattern, g_clip.CurrentChannel);
        }


        void UpdateHighLights(Pattern pat, Channel chan)
        {
            for (int h = 0; h < lblHigh.Count-1; h++)
                UpdateLight(pat, chan, lblHigh[h], HighToNote(h));//num);
        }


        void UpdateLowLights(Pattern pat, Channel chan)
        {
            UpdateLight(lblLow[15], ShowPiano ? "‡" : " ", 8, 17);
            
            if (ShowPiano)
                UpdateLight(lblLow[15], g_clip.HalfSharp);

            for (int l = 0; l < lblLow.Count-1; l++)
                UpdateLight(pat, chan, lblLow[l], LowToNote(l));//num);
        }


        void UpdateLight(Pattern pat, Channel chan, IMyTextPanel light, int num)
        {
            var step = g_clip.PlayStep % g_nSteps;

            var p = g_clip.Patterns.IndexOf(pat);


            if (IsCurParam(strTune))
            {
                var tune =
                    g_clip.CurSrc > -1
                    ? g_clip.SelectedSource    .Tune
                    : g_clip.SelectedInstrument.Tune;

                if (tune.UseChord)
                { 
                    UpdateLight(
                        light, 
                        tune.Chord     .Contains(num),
                        tune.FinalChord.Contains(num));
                }
            }
            else if (g_clip.Chord > -1
                  && g_clip.ChordEdit)
            {
                UpdateLight(light, g_clip.Chords[g_clip.Chord].Contains(num));
            }
            else
            {
                var thisChan =
                       chan.Notes.FindIndex(n =>
                              num == n.Number
                           && (      g_clip.PlayStep >= p * g_nSteps + n.PatStep + n.ShOffset
                                  && g_clip.PlayStep <  p * g_nSteps + n.PatStep + n.ShOffset + n.StepLength
                               ||    p * g_nSteps + n.PatStep >= g_clip.EditPos 
                                  && p * g_nSteps + n.PatStep <  g_clip.EditPos + EditStep)) > -1
                    ||    g_clip.Hold
                       && g_notes.FindIndex(n =>
                                 num == n.Number
                              && g_clip.PlayStep >= n.PatStep
                              && g_clip.PlayStep <  n.PatStep + n.StepLength) > -1;


                var otherChans = false;

                if (!thisChan)
                {
                    for (int ch = 0; ch < g_nChans; ch++)
                    {
                        var _chan = pat.Channels[ch];

                        otherChans |= _chan.Notes.FindIndex(n =>
                                  num == n.Number
                               && ch  == n.iChan
                               && (   g_clip.PlayStep >= p * g_nSteps + n.PatStep + n.ShOffset
                                   && g_clip.PlayStep <  p * g_nSteps + n.PatStep + n.ShOffset + n.StepLength
                            ||    p * g_nSteps + n.PatStep >= g_clip.EditPos 
                               && p * g_nSteps + n.PatStep <  g_clip.EditPos + EditStep)) > -1;
                    }
                }


                var down = false;

                if (g_lightsPressed.Contains(light))
                    down = true;

                UpdateLight(light, thisChan || down, otherChans);
            }
        }


        void UpdateStepLights()
        {
            for (int step = 0; step < g_nSteps; step++)
            {
                var light = lblLow[step];

                var _step = step + g_clip.CurPat * g_nSteps;

                var on = g_clip.CurrentChannel.Notes.Find(n => 
                       n.PatStep >= step
                    && n.PatStep <  step+1) != null;

                Color c;

                if (   OK(g_clip.PlayStep)
                    && _step == (int)g_clip.PlayStep
                    && g_clip.CurPat == g_clip.PlayPat) c = on ? color0 : color6;
                else if (on)                            c = color6;
                else if (g_clip.EditPos == _step)       c = color3;
                else                                    c = step % 4 == 0 ? color2 : color0;

                light.BackgroundColor = c;
            }
        }


        static void UpdateLight(IMyTextPanel light, string text, float size, float pad)
        {
            if (light == null) return;

            light.WriteText(text);
            light.FontSize    = size;
            light.TextPadding = pad;
        }


        static void UpdateLight(IMyTextPanel light, bool b, bool b2 = false)
        {
            if (light == null) return;

            if (b)
            {
                light.FontColor       = color0;
                light.BackgroundColor = color6;
            }
            else
            {
                light.FontColor       = color6;
                light.BackgroundColor = b2 ? color3 : color0;
            }
        }


        static void UpdateEditLight(IMyTextPanel light, bool b)
        {
            if (light == null) return;

            if (b)
            {
                light.FontColor       = redColor0;
                light.BackgroundColor = redColor6;
            }
            else
            {
                light.FontColor       = redColor6;
                light.BackgroundColor = redColor0;
            }
        }


        static void UpdateHoldLight() 
        { 
            UpdateLight(
                lblHold, 
                   g_clip.Hold 
                && (  !OK(g_clip.EditPos) 
                    || g_clip.EditNotes.Count > 0)); 
        }
    }
}
