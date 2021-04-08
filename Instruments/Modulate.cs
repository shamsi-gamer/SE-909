﻿using System;


namespace IngameScript
{
    partial class Program
    {
        public class Modulate : Setting
        {
            public Parameter Amount,
                             Attack,
                             Release;


            public Modulate(Setting parent) : base("Mod", parent)
            {
                Amount  = (Parameter)NewSettingFromTag("Amt", this);
                Attack  = (Parameter)NewSettingFromTag("Att", this);
                Release = (Parameter)NewSettingFromTag("Rel", this);
            }


            public Modulate(Modulate mod, Setting parent) : base(mod.Tag, parent, mod.Prototype)
            {
                Amount  = new Parameter(mod.Amount,  this);
                Attack  = new Parameter(mod.Attack,  this);
                Release = new Parameter(mod.Release, this);
            }


            public Modulate Copy(Setting parent) 
            {
                return new Modulate(this, parent);
            }


            public float GetValue(long gTime, long lTime, long sTime, int noteLen, Note note, int src, Program prog)
            {
                if (prog.TooComplex) return 0;

                return 0;
            }


            public override bool HasDeepParams(Channel chan, int src)
            {
                return false;
            }


            public override Setting GetOrAddSettingFromTag(string tag)
            {
                switch (tag)
                {
                    case "Amt": return Amount  ?? (Amount  = (Parameter)NewSettingFromTag("Amt", this));
                    case "Att": return Attack  ?? (Attack  = (Parameter)NewSettingFromTag("Att", this));
                    case "Rel": return Release ?? (Release = (Parameter)NewSettingFromTag("Rel", this));
                }

                return null;
            }


            public override void GetLabel(out string str, out float width)
            {
                width = 120;

                str =
                      printValue(Amount .CurValue, 2, true, 0).PadLeft(4) + "  "
                    + printValue(Attack .CurValue, 2, true, 0).PadLeft(4) + "  "
                    + printValue(Release.CurValue, 2, true, 0).PadLeft(4);
            }


            public void Delete(Song song, int iSrc)
            {
                // this method removes note and channel automation associated with this setting

                Amount .Delete(song, iSrc);
                Attack .Delete(song, iSrc);
                Release.Delete(song, iSrc);
            }


            public override string Save()
            {
                return
                      W(Tag)

                    + W(Amount .Save())
                    + W(Attack .Save())
                    +   Release.Save();
            }


            public static Modulate Load(string[] data, ref int i, Instrument inst, int iSrc, Setting parent)
            {
                var tag = data[i++];

                var mod = new Modulate(parent);

                mod.Amount  = Parameter.Load(data, ref i, inst, iSrc, mod);
                mod.Attack  = Parameter.Load(data, ref i, inst, iSrc, mod);
                mod.Release = Parameter.Load(data, ref i, inst, iSrc, mod);

                return mod;
            }
        }
    }
}