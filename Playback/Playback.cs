﻿using System.Linq;


namespace IngameScript
{
    partial class Program
    {
        void Play(bool play = True)
        {
            if (!OK(Tracks))
                return;


            if (  !Playing // play
                && play) 
            {
                foreach (var track in Tracks)
                {
                    if (OK(track.NextClip))
                        track.NextPat = 0;
                }
            }

            else // stop
            { 
                foreach (var track in Tracks)
                {
                    if (!OK(track.PlayClip))
                        continue;

                    if (   OK(track.NextClip)
                        && CueClip)
                        track.NextClip = -1;
                    else
                    {
                        var playClip = track.Clips[track.PlayClip];

                        var b = playClip.GetBlock(playClip.CurPat);

                        var _block =
                               playClip.Block
                            && OK(b)
                            && CurPat > b.First;

                        playClip.SetCurrentPattern(_block ? b.First : 0);
                        playClip.TrimCurrentNotes();

                        track.Stop();

                        if (!CueClip)
                            track.NextClip = -1;
                    }
                }

                lastNotes.Clear();
            }
        }


        void Stop() { Play(False); }


        void CheckIfMustStop()
        {
            var playing = OK(Tracks.Find(track => 
                   OK(track.PlayClip) 
                || OK(track.NextClip)));

            if (    Playing
                && !playing)
                Stop();
        }


        void UpdatePlayback()
        {
            var cueNext = False;

            foreach (var track in Tracks)
                cueNext |= track.GetCueNextPattern();


            if (cueNext)
            {
                foreach (var track in Tracks)
                {
                    if (track.NextClip != track.PlayClip)
                    {
                        track.PlayClip = track.NextClip;
                        track.NextPat  = 0;
                    }

                    //if (   OK(track.NextClip)
                    //    && track.NextClip != track.PlayClip)
                    //    track.NextPat = 0;
                }
            }


            foreach (var track in Tracks)
            {                
                if (!OK(track.PlayClip))
                {
                    track.PlayPat = -1;
                    track.NextPat = -1;
                    continue;
                }

                var clip = track.Clips[track.PlayClip];
                if (!OK(clip)) continue;

                track.CueNextPattern(clip);

                if (   clip == EditedClip
                    && clip.Follow) 
                    clip.SetCurrentPattern(track.PlayPat);

                AddPlaybackNotes(clip);
            }


            StopNotes();
            DeleteSounds(StopSounds());
            UpdateSounds();


            foreach (var track in Tracks)
                track.UpdateVolumes(this);

            foreach (var inst in Instruments)
                inst.ResetValues();
        }
    }
}
