import { HeartFill, PlayCircleFill, SkipEndFill, SkipStartFill, VolumeDownFill } from "react-bootstrap-icons";

export type SideBarPlayerProps = {
    Play?: () => void;
    Pause?: () => void;
    IsPlaying?: boolean;
    Stream?: (id: string) => void;
}

export default function SideBarPlayerComponent(props: SideBarPlayerProps) {
    return <div className=" h-full w-full relative overflow-hidden flex flex-col gap-y-10 p-6">
        <img src="https://navidrome.lucadamelio.online/rest/getCoverArt?u=luca&t=8ba40be6e789702ab4f09696cf08ab85&s=6d547f&f=json&v=1.8.0&c=NavidromeUI&id=al-1657b604d8365803c5fc3f528d546812&_=2024-12-12T16%3A36%3A53Z&size=300" className="object-cover absolute right-0 left-0 bottom-0 top-0 w-full h-full blur-xl scale-110 opacity-30" />
        <span className="text-md">Reproducing</span>
        <div className="flex z-10 gap-x-3">
            <img src="https://navidrome.lucadamelio.online/rest/getCoverArt?u=luca&t=8ba40be6e789702ab4f09696cf08ab85&s=6d547f&f=json&v=1.8.0&c=NavidromeUI&id=al-1657b604d8365803c5fc3f528d546812&_=2024-12-12T16%3A36%3A53Z&size=300" className="w-44 h-44 rounded-md" />
            <div className="flex flex-col gap-y-1 justify-center">
                <span className="fo">Tranquility Base Hotel & Casino</span>
                <span className="text-sm text-secondary">Arctic Monkeys</span>
            </div>
        </div>
        <div className=" text-xs flex gap-x-3 items-center z-10">
            <span>1:42</span>
            <div className="h-1 w-full flex-1 bg-white">
                <div className="w-[50%] h-full bg-primary"></div>
            </div>
            <span>3:21</span>
        </div>
        <div className="grid grid-cols-3 items-center justify-between z-10">
            <div className="col-span-1"><HeartFill className="text-primary" /></div>
            <div className="text-xl flex gap-x-3 items-center col-span-1">
                <SkipStartFill className="text-white" />
                <div className="relative">
                    <PlayCircleFill className="text-3xl text-primary" />
                    <PlayCircleFill className="text-3xl text-primary blur-lg absolute top-0" />
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