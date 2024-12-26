import { useEffect } from "react";
import {  useAppSelector } from "../../../hooks";

import SideBarPlayerComponent from "./SidebarPlayerComponent";
import usePlayer from "../../../hooks/usePlayer";

export default function SideBarPlayer() {
    const isPlaying = useAppSelector(state => state.player.isPlaying);
    const [isPlayerReady, player] = usePlayer();

    useEffect(() => {
        // make async function
    }, [isPlaying])



    return <SideBarPlayerComponent Play={player?.Play} Pause={player?.Pause} Stream={player?.Stream} IsPlaying={isPlaying}  />
}