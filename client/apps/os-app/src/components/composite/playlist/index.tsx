import Tile from "../tile"
import { useNavigate } from "react-router"

export type PlaylistProps = {
    id: string
    title: string,
    songs: number,
    imageUrl: string,
}


export default function Playlist(props: PlaylistProps) {
    const navigation = useNavigate()
    const navigate = () => {
        navigation("/app/playlist/" + props.id)
    }

    return <Tile {...props} subtitle={props.songs + " songs"} imageOnClick={navigate} titleOnClick={navigate} />
}
