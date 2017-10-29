using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 
namespace DracosBot.Music
{
    using AudioLoadResultHandler = com.sedmelluq.discord.lavaplayer.player.AudioLoadResultHandler;
    using AudioPlayerManager = com.sedmelluq.discord.lavaplayer.player.AudioPlayerManager;
    using DefaultAudioPlayerManager = com.sedmelluq.discord.lavaplayer.player.DefaultAudioPlayerManager;
    using AudioSourceManagers = com.sedmelluq.discord.lavaplayer.source.AudioSourceManager;
    using FriendlyException = com.sedmelluq.discord.lavaplayer.tools.FriendlyException;
    using AudioPlaylist = com.sedmelluq.discord.lavaplayer.track.AudioPlaylist;
    using AudioTrack = com.sedmelluq.discord.lavaplayer.track.AudioTrack;


    public class MusicHandler
    {
        private readonly AudioPlayerManager playerManager;
        private readonly Map<long, GuildMusicManager>
    }
}
