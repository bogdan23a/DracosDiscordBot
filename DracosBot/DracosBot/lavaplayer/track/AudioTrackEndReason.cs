using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace com.sedmelluq.discord.lavaplayer.track
{
    /// <summary>
    /// Reason why a track stopped playing.
    /// </summary>
    public sealed class AudioTrackEndReason
    {
        /// <summary>
        /// This means that the track itself emitted a terminator. This is usually caused by the track reaching the end,
        /// however it will also be used when it ends due to an exception.
        /// </summary>
        public static readonly AudioTrackEndReason FINISHED = new AudioTrackEndReason("FINISHED", InnerEnum.FINISHED, true);
        /// <summary>
        /// This means that the track failed to start, throwing an exception before providing any audio.
        /// </summary>
        public static readonly AudioTrackEndReason LOAD_FAILED = new AudioTrackEndReason("LOAD_FAILED", InnerEnum.LOAD_FAILED, true);
        /// <summary>
        /// The track was stopped due to the player being stopped by either calling stop() or playTrack(null).
        /// </summary>
        public static readonly AudioTrackEndReason STOPPED = new AudioTrackEndReason("STOPPED", InnerEnum.STOPPED, false);
        /// <summary>
        /// The track stopped playing because a new track started playing. Note that with this reason, the old track will still
        /// play until either its buffer runs out or audio from the new track is available.
        /// </summary>
        public static readonly AudioTrackEndReason REPLACED = new AudioTrackEndReason("REPLACED", InnerEnum.REPLACED, false);
        /// <summary>
        /// The track was stopped because the cleanup threshold for the audio player was reached. This triggers when the amount
        /// of time passed since the last call to AudioPlayer#provide() has reached the threshold specified in player manager
        /// configuration. This may also indicate either a leaked audio player which was discarded, but not stopped.
        /// </summary>
        public static readonly AudioTrackEndReason CLEANUP = new AudioTrackEndReason("CLEANUP", InnerEnum.CLEANUP, false);

        private static readonly IList<AudioTrackEndReason> valueList = new List<AudioTrackEndReason>();

        static AudioTrackEndReason()
        {
            valueList.Add(FINISHED);
            valueList.Add(LOAD_FAILED);
            valueList.Add(STOPPED);
            valueList.Add(REPLACED);
            valueList.Add(CLEANUP);
        }

        public enum InnerEnum
        {
            FINISHED,
            LOAD_FAILED,
            STOPPED,
            REPLACED,
            CLEANUP
        }

        public readonly InnerEnum innerEnumValue;
        private readonly string nameValue;
        private readonly int ordinalValue;
        private static int nextOrdinal = 0;

        /// <summary>
        /// Indicates whether a new track should be started on receiving this event. If this is false, either this event is
        /// already triggered because another track started (REPLACED) or because the player is stopped (STOPPED, CLEANUP).
        /// </summary>
        public readonly bool mayStartNext;

        internal AudioTrackEndReason(string name, InnerEnum innerEnum, bool mayStartNext)
        {
            this.mayStartNext = mayStartNext;

            nameValue = name;
            ordinalValue = nextOrdinal++;
            innerEnumValue = innerEnum;
        }

        public static IList<AudioTrackEndReason> values()
        {
            return valueList;
        }

        public int ordinal()
        {
            return ordinalValue;
        }

        public override string ToString()
        {
            return nameValue;
        }

        public static AudioTrackEndReason valueOf(string name)
        {
            foreach (AudioTrackEndReason enumInstance in AudioTrackEndReason.valueList)
            {
                if (enumInstance.nameValue == name)
                {
                    return enumInstance;
                }
            }
            throw new System.ArgumentException(name);
        }
    }
}
