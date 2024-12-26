import Player from "@opensupernova/player";
import { load } from "@tauri-apps/plugin-store";
import { useEffect, useState } from "react";
import { StoreType } from "./useFetch";

function usePlayer() : [boolean, Player | null] {
    const [player, setPlayer] = useState<Player | null>(null);

    useEffect(() => {
        (async () => {
            const store = await load("config.json", { autoSave: false })

            var url = await store.get<StoreType>("url")
            var token = await store.get<StoreType>("token")
            if (url && token) {
                setPlayer(new Player(url.value, token.value))
            }
        })()
    }, []);

    return [player != null, player];
}

export default usePlayer;