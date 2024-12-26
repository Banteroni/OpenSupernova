import Tile from "../tile"
import { useNavigate } from "react-router"

export type AlbumProps = {
    id: string
    title: string,
    songs: number,
    imageUrl: string,
}


export default function Album(props: AlbumProps) {
    const navigation = useNavigate()
    const navigate = () => {
        navigation("/app/album/" + props.id)
    }

    return <Tile {...props} subtitle={props.songs + " songs"} imageOnClick={navigate} titleOnClick={navigate} />
}
