import { HeartFill, PauseCircleFill, PlayCircleFill, SkipEndFill, SkipStartFill, VolumeDownFill } from "react-bootstrap-icons";
import { PlayerState } from "../../../hooks/playerReducer";

export type SideBarPlayerProps = {
    Toggle: () => void;
    IsPlaying: boolean;
    albumUrl?: string;
    album: PlayerState["album"];
    artist: PlayerState["artist"];
    track: PlayerState["track"];
}

export default function SideBarPlayerComponent(props: SideBarPlayerProps) {
    return <div className=" h-full w-full relative overflow-hidden flex flex-col gap-y-10 p-6">
        {props.albumUrl ? <div className="object-cover absolute right-0 left-0 bottom-0 top-0 w-full h-full blur-xl scale-110 opacity-30 bg-secondary" /> : <img src={props.albumUrl} className="object-cover absolute right-0 left-0 bottom-0 top-0 w-full h-full blur-xl scale-110 opacity-30" />}

        <span className="text-md">Reproducing</span>
        <div className="flex z-10 gap-x-3">
            {
                props.albumUrl ? <img src={props.albumUrl} className="w-44 h-44 rounded-md" /> : <div className="w-44 h-44 bg-secondary/20 rounded-md" />
            }
            <div className="flex flex-col gap-y-1 justify-center">
                {props.IsPlaying && <>
                    <span className="fo">{props.album.name}</span>
                    <span className="text-sm text-secondary">{props.artist.name}</span>
                </>
                }
            </div>
        </div>
        {props.track.name == "" ? <div className="h-1 w-full"></div> : <div className=" text-xs flex gap-x-3 items-center z-10">
            <span>1:42</span>
            <div className="h-1 w-full flex-1 bg-white">
                <div className="w-[50%] h-full bg-primary"></div>
            </div>
            <span>3:21</span>
        </div>}
        <div className="grid grid-cols-3 items-center justify-between z-10">
            <div className="col-span-1"><HeartFill className="text-primary" /></div>
            <div className="text-xl flex gap-x-3 items-center col-span-1">
                <SkipStartFill className="text-white" />
                <div onClick={props.Toggle} className="cursor-pointer">
                    {props.IsPlaying ?
                        <div className="relative">
                            <PauseCircleFill className="text-3xl text-primary" />
                            <PauseCircleFill className="text-3xl text-primary blur-lg absolute top-0" />
                        </div>
                        :
                        <div className="relative">
                            <PlayCircleFill className="text-3xl text-white" />
                            <PlayCircleFill className="text-3xl text-white blur-lg absolute top-0" />
                        </div>
                    }
                </div>
                <SkipEndFill className="text-white" />
            </div>
        </div>
        <div className="col-span-1 z-10 flex justify-center items-center gap-x-3">
            <VolumeDownFill className="text-white text-lg" />
            <div className="w-24 h-1 bg-white rounded">
                <div className="w-[50%] h-full bg-primary" />
            </div>
            <VolumeDownFill className="text-white text-lg opacity-0" />
        </div>
    </div>
}