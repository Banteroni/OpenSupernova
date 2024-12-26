import { useAppSelector } from "../../../hooks";

import SideBarPlayerComponent from "./SidebarPlayerComponent";
import usePlayer from "../../../hooks/usePlayer";
import { useEffect } from "react";

export default function SideBarPlayer() {
    const isPlaying = useAppSelector(state => state.player.isPlaying);
    const album = useAppSelector(state => state.player.album);
    const artist = useAppSelector(state => state.player.artist);
    const track = useAppSelector(state => state.player.track);
    const [isPlayerReady, player] = usePlayer();

    const Toggle = () => {
        if (!isPlayerReady) return;
        if (isPlaying) {
            player?.Pause();
        } else {
            player?.Play();
        }
    }

    useEffect(() => {
        if (isPlayerReady && track) {
            player?.Stream(track.id);
        }
    }, [track])

    useEffect(() => {
        if (isPlayerReady) console.log("Player is ready")
    }, [isPlayerReady])

    return <SideBarPlayerComponent Toggle={Toggle}  IsPlaying={isPlaying} track={track} artist={artist} album={album} />
}