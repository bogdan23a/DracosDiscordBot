using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sedmelluq.discord.lavaplayer.track
{
    /// <summary>
    /// An audio item which refers to an unloaded audio item. Source managers can return this to indicate a redirection,
    /// which means that the item referred to in it is loaded instead.
    /// </summary>
    public class AudioReference : AudioItem
    {
        public static readonly AudioReference NO_TRACK = new AudioReference(null, null);

        /// <summary>
        /// The identifier of the other item.
        /// </summary>
        public readonly string identifier;
        /// <summary>
        /// The title of the other item, if known.
        /// </summary>
        public readonly string title;

        /// <param name="identifier"> The identifier of the other item. </param>
        /// <param name="title"> The title of the other item, if known. </param>
        public AudioReference(string identifier, string title)
        {
            this.identifier = identifier;
            this.title = title;
        }
    }
}
