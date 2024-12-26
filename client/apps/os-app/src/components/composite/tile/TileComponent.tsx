import { TileProps } from ".";

export default function TileComponent(props: TileProps) {

    return (<div className="p-4 bg-gradient-to-t from-white/5 to-white/10 rounded-xl flex flex-col hover:bg-white/5 duration-200">
        <img src={props.imageUrl} onClick={props.imageOnClick} alt={props.title} className={`w-full h-40 object-cover rounded-xl ${props.imageOnClick && "hover:cursor-pointer"}`} />
        <span className={`pt-2 w-40 ${props.titleOnClick && "hover:cursor-pointer"}`} onClick={props.titleOnClick}>{props.title}</span>
        <span className={`text-secondary text-sm ${props.subtitleOnClick && "hover:cursor-pointer"}`} onClick={props.subtitleOnClick}>{props.subtitle}</span>
    </div>)
}