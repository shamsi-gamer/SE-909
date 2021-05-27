﻿namespace IngameScript
{
    partial class Program
    {
        void InitChordLabels()
        {
            lblChord     = new Label(GetLabel("Chord"),      ChordIsBright,     null,          UpdateChord);
            lblChord1    = new Label(GetLabel("Chord 1"),    ChordNumIsBright,  ChordNumIsDim, UpdateChordNum, null, 1);
            lblChord2    = new Label(GetLabel("Chord 2"),    ChordNumIsBright,  ChordNumIsDim, UpdateChordNum, null, 2);
            lblChord3    = new Label(GetLabel("Chord 3"),    ChordNumIsBright,  ChordNumIsDim, UpdateChordNum, null, 3);
            lblChord4    = new Label(GetLabel("Chord 4"),    ChordNumIsBright,  ChordNumIsDim, UpdateChordNum, null, 4);
            lblChordEdit = new Label(GetLabel("Chord Edit"), ChordEditIsBright, null,          UpdateChordEdit);
            lblSpread    = new Label(GetLabel("Spread"),     lbl => EditedClip.Spread);
        }


        bool ChordIsBright(Label lbl)
        {
            var tune =
                IsCurParam(strTune)
                ? (Tune)GetCurrentParam(SelInstrument) 
                : null;

            if (   !(EditedClip.ParamKeys || EditedClip.ParamAuto)
                && OK(tune))
                return tune.UseChord;
            else
                return EditedClip.ChordMode;
        }


        void UpdateChord(Label lbl)
        {
            if (   !IsCurParam(strTune)
                || EditedClip.ParamKeys 
                || EditedClip.ParamAuto)
                lbl.SetText(EditedClip.ChordEdit ? " " : "Chord", 9, 12);
        }


        bool ChordNumIsBright(Label lbl)
        {
            var chord = lbl.Data;

            return
                   EditedClip.Chord == chord-1
                && (   EditedClip.ChordEdit
                    || EditedClip.ChordMode)
                && !IsCurParam(strTune);
        }


        bool ChordNumIsDim(Label lbl)
        {
            var chord = lbl.Data;

            return
                      EditedClip.ChordMode
                   && EditedClip.Chord == chord-1 
                || EditedClip.Chords[chord-1].Count > 0;
        }


        void UpdateChordNum(Label lbl)
        {
            var chord = lbl.Data;
            lbl.SetText(GetChordName(EditedClip.Chords[chord-1], S(chord)));
        }


        void UpdateChordEdit(Label lbl)
        {
            if (    IsCurParam(strTune)
                && !(EditedClip.ParamKeys || EditedClip.ParamAuto))
            {
                var tune =
                    IsCurParam(strTune)
                    ? (Tune)GetCurrentParam(SelInstrument) 
                    : null;

                lbl.SetText(
                       OK(tune)
                    && tune.UseChord 
                    ? strAll : " ");
            }
            else
                lbl.SetText(EditedClip.ChordMode ? strAll : "Edit");
        }


        bool ChordEditIsBright(Label lbl)
        {
            var tune =
                IsCurParam(strTune)
                ? (Tune)GetCurrentParam(SelInstrument)
                : null;

            if (   IsCurParam(strTune)
                && !(EditedClip.ParamKeys || EditedClip.ParamAuto))
                return
                       OK(tune) 
                    && tune.AllOctaves;

            else return
                      EditedClip.ChordMode
                   && EditedClip.ChordAll
                || EditedClip.ChordEdit;
        }
    }
}
