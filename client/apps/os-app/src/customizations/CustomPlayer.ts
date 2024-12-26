import Player from "@opensupernova/player";
import { setPlay } from "../hooks/playerReducer";

export default class CustomPlayer extends Player {
    public Play = () => {
        setPlay(true);
        this._Play();
    }

    public Pause = () => {
        setPlay(false);
        this.Pause();
    }
    
}