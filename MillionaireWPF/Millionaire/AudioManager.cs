using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Millionaire
{
    internal class AudioManager
    {
        private enum AudioActionType
        {
            PushState,
            PopState,
            PlaySong,
            StopSong,
            PlayCue
        }

        private record AudioState(Song Song, TimeSpan Position, bool IsRepeating);
        private record AudioAction(AudioActionType Type, string Song = null, bool IsRepeating = false);

        private static Song activeSong;
        private static Dictionary<string, Song> songs;
        private static Dictionary<string, SoundEffect> soundEffects;
        private static Stack<AudioState> states = new Stack<AudioState>();
        private static ConcurrentQueue<AudioAction> actions = new ConcurrentQueue<AudioAction>();

        private static void AudioLoop()
        {
            var wait = new SpinWait();
            while (true)
            {
                wait.SpinOnce();

                FrameworkDispatcher.Update();

                if (actions.TryDequeue(out var action))
                {
                    if (action.Type == AudioActionType.PushState)
                    {
                        states.Push(new AudioState(activeSong, MediaPlayer.PlayPosition, MediaPlayer.IsRepeating));
                    }

                    if (action.Type == AudioActionType.PopState)
                    {
                        var state = states.Pop();
                        if (state.Song != null)
                        {
                            MediaPlayer.Stop();
                            MediaPlayer.Volume = 0;
                            MediaPlayer.IsRepeating = state.IsRepeating;
                            MediaPlayer.Play(state.Song, state.Position);

                            for (int i = 0; i < 100; i++)
                            {
                                MediaPlayer.Volume = i / 100.0f;
                                Thread.Sleep(5);
                            }
                        }
                        else
                        {
                            MediaPlayer.Stop();
                        }

                        activeSong = state.Song;
                    }

                    if (action.Type == AudioActionType.PlaySong)
                    {
                        var songItem = songs[action.Song];
                        if (songItem != activeSong)
                        {
                            MediaPlayer.Volume = 1;
                            MediaPlayer.IsRepeating = action.IsRepeating;
                            MediaPlayer.Play(activeSong = songItem);
                        }
                    }

                    if (action.Type == AudioActionType.StopSong)
                    {
                        activeSong = null;
                        MediaPlayer.Stop();
                    }
                }
            }
        }


        public static void Load()
        {
            MediaPlayer.IsRepeating = true;

            songs = new Dictionary<string, Song>();
            soundEffects = new Dictionary<string, SoundEffect>();

            var audioPath = Path.Combine(Directory.GetCurrentDirectory(), "audio");
            foreach (var file in Directory.GetFiles(Path.Combine(audioPath, "sfx"), "*.wav"))
            {
                soundEffects[Path.GetFileNameWithoutExtension(file)] = SoundEffect.FromFile(file);
            }

            foreach (var file in Directory.GetFiles(Path.Combine(audioPath, "bgm")))
            {
                songs[Path.GetFileNameWithoutExtension(file)] = Song.FromUri(Path.GetFileNameWithoutExtension(file), new Uri(file));
            }

            var thread = new Thread(() => AudioLoop());
            thread.IsBackground = true;
            thread.Start();
        }

        public static void PushState()
        {
            actions.Enqueue(new AudioAction(AudioActionType.PushState));
        }

        public static void PopState()
        {
            actions.Enqueue(new AudioAction(AudioActionType.PopState));
        }

        public static void PlaySong(string song, bool loop = true)
        {
            actions.Enqueue(new AudioAction(AudioActionType.PlaySong, song, loop));
        }

        public static void StopSong()
        {
            actions.Enqueue(new AudioAction(AudioActionType.StopSong));
        }

        public static void PlayCue(string cue)
        {
            soundEffects[cue].Play();
        }
    }
}
