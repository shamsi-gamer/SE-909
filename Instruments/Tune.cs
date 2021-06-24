﻿using System.Collections.Generic;


namespace IngameScript
{
    partial class Program
    {
        public class Tune : Parameter
        {
            public bool      UseChord,
                             AllOctaves;

            public List<int> Chord,
                             FinalChord;


            public Tune(Instrument inst, Source src) 
                : base(strTune, -240, 240, -12, 12, 0.5f, 12, 0, Setting_null, inst, src)
            {
                UseChord   = False;
                AllOctaves = False;

                Chord      = new List<int>();
                FinalChord = new List<int>();
            }


            public Tune(Tune tune) : base(tune, Setting_null)
            {
                UseChord   = tune.UseChord;
                AllOctaves = tune.AllOctaves;

                Chord = new List<int>();
                foreach (var note in tune.Chord)
                    Chord.Add(note);

                FinalChord = new List<int>();
                foreach (var note in tune.FinalChord)
                    FinalChord.Add(note);
            }


            public Tune Copy()
            {
                return new Tune(this);
            }


            public override void Randomize()
            {
                m_value = NormalMin + RND * (NormalMax - NormalMin);

                if (RND > 1/3f) m_value = (int)(m_value/ 7)* 7;
                else            m_value = (int)(m_value/12)*12;                


                if (   !TooComplex
                    && !AnyParentIsEnvelope
                    && (  !IsDigit(Tag[0]) && RND > 0.5f
                        || IsDigit(Tag[0]) && RND > 0.9f))
                {
                    Envelope = new Envelope(this, Instrument, Source);
                    Envelope.Randomize();
                }
                else 
                    Envelope = Envelope_null;


                if (   !TooComplex
                    && RND > 0.8f)
                {
                    Lfo = new LFO(this, Instrument, Source);
                    Lfo.Randomize();
                }
                else
                { 
                    if (OK(Lfo))
                        g_lfo.Remove(Lfo);

                    Lfo = LFO_null;
                }
            }


            public override string GetLabel(out float width)
            {
                width = 90f; 
                return PrintValue(Value, 2, True, 1).PadLeft(5);
            }


            public override string Save()
            {
                var tune =
                      W(base.Save())
                    + WB(UseChord)
                    + WB(AllOctaves);

                tune += S(Chord.Count);

                for (int i = 0; i < Chord.Count; i++)
                    tune += PS(Chord[i]);

                return tune;
            }


            public static Tune Load(string[] data, ref int i, Instrument inst, int iSrc)
            {
                var tune = new Tune(
                    inst, 
                    OK(iSrc) ? inst.Sources[iSrc] : Source_null);

                Parameter.Load(data, ref i, inst, iSrc, Setting_null, tune);

                tune.UseChord   = data[i++] == "1";
                tune.AllOctaves = data[i++] == "1";

                var nChords = int_Parse(data[i++]);

                for (int j = 0; j < nChords; j++)
                    tune.Chord.Add(int_Parse(data[i++]));

                return tune;
            }


            public override bool CanDelete()
            {
                return True;
            }
        }
    }
}
